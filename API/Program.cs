using BL.Services.Common;
using DAL.DBContext;
using DAL.Models;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("NPF");
string DBServer = new ApplicationSettingsHandler().settings.Params.GetValueOrDefault("DBServer").ToString().ToUpper();
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
}).AddEntityFrameworkStores<NPFContext>();



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
