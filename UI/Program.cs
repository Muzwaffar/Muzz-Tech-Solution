using BL.Services.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Npgsql;
using UI.Controllers.Background;

var builder = WebApplication.CreateBuilder(args);
var context = new ContextService().getNPFContext();
string DBServer = new ApplicationSettingsHandler().settings.Params.GetValueOrDefault("DBServer").ToString().ToUpper();
SeedService seeding = new SeedService();
var connectionString = builder.Configuration.GetConnectionString("NPF");
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
//Configure Background Service
//builder.Services.AddHostedService<Background>();
switch (DBServer)
{
    case "SQL":
        builder.Services.AddDbContext<NPFContext>(options => options.UseSqlServer(connectionString, x => x.UseNetTopologySuite()));
        break;
    case "POSTGRES":
        builder.Services.AddDbContext<NPFContext>(opt => opt.UseNpgsql(connectionString, ob => ob.UseNetTopologySuite()));
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        break;
    default:
         builder.Services.AddDbContext<NPFContext>(options => options.UseSqlServer(connectionString, x => x.UseNetTopologySuite()));
        break;
}

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<ApplicationRole>().AddEntityFrameworkStores<NPFContext>();
int AccountLockedAttempts = Convert.ToInt32(new ApplicationSettingsHandler().settings.Params.GetValueOrDefault("AccountLockedAttempts"));
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
{
    //previous code removed for clarity reasons
    opt.Lockout.AllowedForNewUsers = true;
    opt.Lockout.MaxFailedAccessAttempts = AccountLockedAttempts;
}).AddEntityFrameworkStores<NPFContext>()
.AddDefaultTokenProviders();

//builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
//   opt.TokenLifespan = TimeSpan.FromHours(2));
builder.Services.ConfigureApplicationCookie(options =>
{
   // options.ExpireTimeSpan = TimeSpan.FromMinutes(.01);
    options.ExpireTimeSpan = TimeSpan.FromSeconds(2700);
    options.LoginPath = "/Login/Login";
    //options.LogoutPath = "/Identity/Account/Logout";
   //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(2700);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var db = scope.ServiceProvider.GetRequiredService<NPFContext>();
//db.Database.EnsureCreated();
    db.Database.Migrate();
    SeedService.SeedData(userManager, roleManager);

}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Scripts")),
    RequestPath = "/Scripts"
});

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
