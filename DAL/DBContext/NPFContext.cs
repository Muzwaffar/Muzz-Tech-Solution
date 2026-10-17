using DAL.Models;
using Microsoft.EntityFrameworkCore;
using DAL.Common;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using DAL.Models.Administration;
using DAL.Models.Access;
using System.Reflection.Emit;
using Microsoft.Extensions.Options;
using DAL.Models.Notifications;
using DAL.Models.Common;
using DAL.Models.Form;
using DAL.Models.FormPage;
using DAL.Models.Activities;
using DAL.Models.WorkFlow;
using DAL.Models.Template;
using DAL.Models.Store;
using DAL.Models.Schedule;
using DAL.Models.Research;
using System.ComponentModel;
using DAL.Models.Transaction;
using DAL.Models.Request;

namespace DAL.DBContext
{
    public class NPFContext : AuditContext<ApplicationUser, ApplicationRole, string>
    {
        public NPFContext()
        {

        }
        private readonly string _username;

        public NPFContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
        : base(options)
        {
            var claimsPrincipal = httpContextAccessor.HttpContext?.User;
            _username = claimsPrincipal?.Claims?.SingleOrDefault(c => c.Type == "username")?.Value ?? "Unauthenticated user";
            ////Comments
        }


        //public static string? UGuid { get; set; }// = Session["somekey"] = theObject; 
        // public static Guid? OTT { get; set; } = null;
        //public static string UserLanguage { get; set; } = "English";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string DBServer = new ApplicationSettingsHandler().settings.Params.GetValueOrDefault("DBServer").ToString().ToUpper();
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies();
                switch (DBServer)
                {
                    case "SQL":
                        optionsBuilder.UseSqlServer(new ApplicationSettingsHandler().settings.ConnectionStrings.GetValueOrDefault(Databases.NPF), builder => builder.EnableRetryOnFailure());
                        optionsBuilder.UseSqlServer(new ApplicationSettingsHandler().settings.ConnectionStrings.GetValueOrDefault(Databases.NPF), x => x.UseNetTopologySuite());
                        break;
                    case "POSTGRES":
                        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                        optionsBuilder.UseNpgsql(new ApplicationSettingsHandler().settings.ConnectionStrings.GetValueOrDefault(Databases.NPF), builder => builder.EnableRetryOnFailure());
                        optionsBuilder.UseNpgsql(new ApplicationSettingsHandler().settings.ConnectionStrings.GetValueOrDefault(Databases.NPF), x => x.UseNetTopologySuite());
                        break;
                    default:
                        optionsBuilder.UseSqlServer(new ApplicationSettingsHandler().settings.ConnectionStrings.GetValueOrDefault(Databases.NPF), builder => builder.EnableRetryOnFailure());
                        optionsBuilder.UseSqlServer(new ApplicationSettingsHandler().settings.ConnectionStrings.GetValueOrDefault(Databases.NPF), x => x.UseNetTopologySuite());
                        break;
                }

            }


        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();


        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AuditEntry>().Property(ae => ae.Changes).HasConversion(value => JsonConvert.SerializeObject(value), serializedValue => JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedValue));
            builder.Entity<AuditEntry>().Property(ae => ae.OriginalValues).HasConversion(value => JsonConvert.SerializeObject(value), serializedValue => JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedValue));
            string DBServer = new ApplicationSettingsHandler().settings.Params.GetValueOrDefault("DBServer").ToString().ToUpper();
            switch (DBServer)
            {
                case "POSTGRES":
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                    builder.HasPostgresExtension("postgis");
                    break;
                default:
                    break;
            }
            base.OnModelCreating(builder);
        }
        #region AUDITMethod
        public async override Task<int> SaveChangesAsync()
        {
            // Get audit entries
            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            string claimsPrincipal = null;
            Guid gidc = Guid.Empty;
            if (httpContextAccessor.HttpContext != null)
            {
                claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
            }
            else
            {
                claimsPrincipal = this.Users.Where(x => x.Email.ToLower() == "admin@naveo.mu").Select(x => x.Id).FirstOrDefault();
            }

            var UGuid = this.Users.Where(x => x.UserName == claimsPrincipal).Select(x => x.Id).FirstOrDefault();
            if (UGuid == null || UGuid == String.Empty)
            {
                try
                {
                    var checkAdded = this.ChangeTracker.Entries().Where(t => t.State == EntityState.Added).Select(t => t.Entity).ToArray();
                    foreach (var entity in checkAdded)
                    {
                        if (entity is BaseAuditModel)
                        {
                            var track = entity as BaseAuditModel;
                            if (track.CreatedBy != gidc)
                            {
                                UGuid = track.CreatedBy.ToString();
                            }

                        }
                    }
                    var modified = this.ChangeTracker.Entries().Where(t => t.State == EntityState.Modified).Select(t => t.Entity).ToArray();

                    foreach (var entity in modified)
                    {
                        if (entity is BaseAuditModel)
                        {
                            var track = entity as BaseAuditModel;
                            if (track.UpdatedBy != gidc)
                            {
                                UGuid = track.CreatedBy.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            var auditEntries = OnBeforeSaveChanges(UGuid);
            if (UGuid != null && UGuid != String.Empty)
            {
                Guid Userguid = Guid.Parse(UGuid);
                this.ChangeTracker.DetectChanges();
                var added = this.ChangeTracker.Entries()
                            .Where(t => t.State == EntityState.Added)
                            .Select(t => t.Entity)
                            .ToArray();

                foreach (var entity in added)
                {
                    if (entity is BaseAuditModel)
                    {
                        var track = entity as BaseAuditModel;
                        track.CreatedDate = DateTime.Now;
                        track.CreatedBy = Userguid;
                    }
                }

                var modified = this.ChangeTracker.Entries()
                            .Where(t => t.State == EntityState.Modified)
                            .Select(t => t.Entity)
                            .ToArray();

                foreach (var entity in modified)
                {
                    if (entity is BaseAuditModel)
                    {
                        var track = entity as BaseAuditModel;
                        track.UpdatedDate = DateTime.Now;
                        track.UpdatedBy = Userguid;
                    }
                }
            }
            // Save current entity
            var result = await base.SaveChangesAsync();

            // Save audit entries
            OnAfterSaveChangesAsync(auditEntries.AUL);
            return result;
        }
        //public static string Sess()
        //{
        //    ISession session = HttpContext.Session;
        //    session.SetString("Username", "ffff");
        //}
        public override int SaveChanges()
        {
            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            string claimsPrincipal = null;
            if (httpContextAccessor.HttpContext != null)
            {
                claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
            }
            else
            {
                claimsPrincipal = this.Users.Where(x => x.Email.ToLower() == "admin@naveo.mu").Select(x => x.Id).FirstOrDefault();
            }
            Guid gidc = Guid.Empty;
            DateTime dtc = DateTime.MinValue;
            var UGuid = this.Users.Where(x => x.UserName == claimsPrincipal).Select(x => x.Id).FirstOrDefault();

            if (UGuid == null || UGuid == String.Empty)
            {
                try
                {
                    var checkAdded = this.ChangeTracker.Entries().Where(t => t.State == EntityState.Added).Select(t => t.Entity).ToArray();
                    foreach (var entity in checkAdded)
                    {
                        if (entity is BaseAuditModel)
                        {
                            var track = entity as BaseAuditModel;
                            if (track.CreatedBy != gidc)
                            {
                                UGuid = track.CreatedBy.ToString();
                            }

                        }
                    }
                    var modified = this.ChangeTracker.Entries().Where(t => t.State == EntityState.Modified).Select(t => t.Entity).ToArray();

                    foreach (var entity in modified)
                    {
                        if (entity is BaseAuditModel)
                        {
                            var track = entity as BaseAuditModel;
                            if (track.CreatedBy != gidc)
                            {
                                UGuid = track.CreatedBy.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            // string
            if ((UGuid == null || UGuid == String.Empty || UGuid == "00000000-0000-0000-0000-000000000000") && claimsPrincipal != null)
            {
                UGuid = this.Users.Where(x => x.UserName == claimsPrincipal).Select(x => x.Id).FirstOrDefault();
            }
            // Get audit entries
            var auditEntries = OnBeforeSaveChanges(UGuid);
            if (UGuid != null && UGuid != String.Empty)
            {
                Guid Userguid = Guid.Parse(UGuid);
                this.ChangeTracker.DetectChanges();
                var added = this.ChangeTracker.Entries()
                            .Where(t => t.State == EntityState.Added)
                            .Select(t => t.Entity)
                            .ToArray();


                foreach (var entity in added)
                {
                    if (entity is BaseAuditModel)
                    {
                        var track = entity as BaseAuditModel;
                        if (track.CreatedDate == dtc)
                        {
                            track.CreatedDate = DateTime.Now;
                        }
                        if (track.CreatedBy == gidc)
                        {
                            track.CreatedBy = Userguid;
                        }

                    }
                }

                var modified = this.ChangeTracker.Entries()
                            .Where(t => t.State == EntityState.Modified)
                            .Select(t => t.Entity)
                            .ToArray();

                foreach (var entity in modified)
                {
                    if (entity is BaseAuditModel)
                    {
                        var track = entity as BaseAuditModel;
                        if (track.UpdatedDate == dtc)
                        {
                            track.UpdatedDate = DateTime.Now;
                        }
                        if (track.UpdatedBy == gidc)
                        {
                            track.UpdatedBy = Userguid;
                        }
                        track.UpdatedDate = DateTime.Now;
                        track.UpdatedBy = Userguid;
                        if (track.CreatedBy == null || track.CreatedBy == new Guid())
                        {
                            if (auditEntries.CreatedBy == null)
                            {
                                track.CreatedBy = new Guid();
                            }
                            else
                            {
                                track.CreatedBy = Guid.Parse(auditEntries.CreatedBy.ToString());
                            }
                        }
                        else
                        {
                            track.CreatedBy = track.CreatedBy;
                        }

                        if (track.CreatedDate == null || track.CreatedDate == DateTime.MinValue)
                        {
                            track.CreatedDate = Convert.ToDateTime(auditEntries.CreatedDate);
                        }
                        else
                        {
                            track.CreatedDate = track.CreatedDate;
                        }
                    }
                }

            }
            // Save current entity
            var result = base.SaveChanges();

            // Save audit entries
            OnAfterSaveChangesAsync(auditEntries.AUL);
            return result;
        }
        public Dictionary<string, object> newval { get; set; }
        public Dictionary<string, object> ori { get; set; }
        public class Auditdto
        {
            public List<AuditEntry> AUL { get; set; } = new List<AuditEntry>();
            public DateTime? CreatedDate { get; set; }
            public Guid? CreatedBy { get; set; }
        }
        private Auditdto OnBeforeSaveChanges(string UGuid)
        {
            ChangeTracker.DetectChanges();
            var entries = new List<AuditEntry>();
            Auditdto adto = new Auditdto();

            foreach (var entry in ChangeTracker.Entries())
            {
                // Prevent Circular Dependencies
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged || !(entry.Entity is IAuditable))
                {
                    continue;
                }
                else if (entry.State == EntityState.Added)
                {
                    newval = entry.Properties.Select(p => new { p.Metadata.Name, p.CurrentValue }).ToDictionary(i => i.Name, i => i.CurrentValue);
                    newval["CreatedBy"] = UGuid;
                    newval["CreatedDate"] = DateTime.Now;
                    var auditEntry = new AuditEntry
                    {
                        ActionType = "INSERT",
                        EntityId = entry.Properties.Single(p => p.Metadata.IsPrimaryKey()).CurrentValue.ToString(),
                        EntityName = entry.Metadata.ClrType.Name,
                        UserId = UGuid == null ? "System-Generated" : UGuid == string.Empty ? "System-Generated" : UGuid,
                        TimeStamp = DateTime.UtcNow,
                        OriginalValues = newval,
                        Changes = newval,
                        TempProperties = entry.Properties.Where(p => p.IsTemporary).ToList(),
                    };

                    entries.Add(auditEntry);
                }
                else if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    Dictionary<string, object> ori = new Dictionary<string, object>();
                    Dictionary<string, object> newval = new Dictionary<string, object>();
                    Dictionary<string, object> cori = new Dictionary<string, object>();
                    Dictionary<string, object> cnewval = new Dictionary<string, object>();
                    newval = entry.Properties.Select(p => new { p.Metadata.Name, p.CurrentValue }).ToDictionary(i => i.Name, i => i.CurrentValue);
                    //ori = entry.Properties.Select(p => new { p.Metadata.Name, p.OriginalValue }).ToDictionary(i => i.Name, i => i.OriginalValue);
                    newval["UpdatedBy"] = UGuid;
                    newval["UpdatedDate"] = DateTime.Now;
                    var modifiedEntities = ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).ToList();
                    var now = DateTime.UtcNow;

                    foreach (var change in modifiedEntities)
                    {
                        var entityName = change.Entity.GetType().Name;

                        foreach (var prop in change.OriginalValues.Properties)
                        {
                            try
                            {
                                var value = change.GetDatabaseValues().GetValue<object>(prop.Name);
                                if (prop.Name == "CreatedBy")
                                {
                                    adto.CreatedBy = Guid.Parse(value.ToString());
                                }
                                if (prop.Name == "CreatedDate")
                                {
                                    adto.CreatedDate = Convert.ToDateTime(value.ToString());
                                }
                                if (ori.Where(x => x.Key == prop.Name).Any())
                                {
                                    string Pname = entityName + prop.Name;
                                    int count = ori.Where(x => x.Key == Pname).Count();
                                    if (count > 0)
                                    {
                                        Pname += (count + 1).ToString();
                                    }
                                    ori.Add(Pname, value);
                                }
                                else
                                {
                                    ori.Add(prop.Name, value);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    foreach (var x in newval)
                    {
                        string name = x.Key;
                        string newv = newval[x.Key] != null ? newval[x.Key].ToString() : "";
                        string oriv = string.Empty;
                        if (entry.State == EntityState.Deleted)
                        {
                            oriv = newv;
                        }
                        else
                        {
                            oriv = ori[x.Key] != null ? ori[x.Key].ToString() : "";
                        }

                        if (newv != oriv && x.Key != "CreatedBy" && x.Key != "CreatedDate")
                        {
                            cori.Add(x.Key, oriv);
                            cnewval.Add(x.Key, newv);
                        }
                    }
                    var auditEntry = new AuditEntry
                    {
                        ActionType = entry.State == EntityState.Deleted ? "DELETE" : "UPDATE",
                        EntityId = entry.Properties.Single(p => p.Metadata.IsPrimaryKey()).CurrentValue.ToString(),
                        EntityName = entry.Metadata.ClrType.Name,
                        UserId = UGuid == null ? "System-Generated" : UGuid == string.Empty ? "System-Generated" : UGuid,
                        TimeStamp = DateTime.UtcNow,
                        OriginalValues = cori,
                        Changes = cnewval,
                        TempProperties = entry.Properties.Where(p => p.IsTemporary).ToList(),
                    };

                    entries.Add(auditEntry);
                }

            }
            adto.AUL = entries;

            return adto;
        }

        private async void OnAfterSaveChangesAsync(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
            {
                return;
            }
            NPFContext fc = new NPFContext();
            //return Task.CompletedTask;

            foreach (var entry in auditEntries)
            {
                foreach (var prop in entry.TempProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        entry.EntityId = prop.CurrentValue.ToString();
                        entry.Changes[prop.Metadata.Name] = prop.CurrentValue;
                        entry.OriginalValues[prop.Metadata.Name] = prop.OriginalValue;
                    }
                    else
                    {
                        entry.Changes[prop.Metadata.Name] = prop.CurrentValue;
                        entry.OriginalValues[prop.Metadata.Name] = prop.OriginalValue;
                    }
                }
            }

            fc.AuditEntries.AddRange(auditEntries);
            fc.SaveChangesAsync();
        }
        #endregion

        #region DBSET
        #region ACT
        public DbSet<NPF_ACT_Activities> NPF_ACT_Activities { get; set; }
        public DbSet<NPF_ACT_ActivitiesDetails> NPF_ACT_ActivitiesDetails { get; set; }
        public DbSet<NPF_ACT_ActivitiesConfig> NPF_ACT_ActivitiesConfig { get; set; }
        public DbSet<NPF_ACT_ActivityEventType> NPF_ACT_ActivityEventType { get; set; }
        #endregion
        #region SLN
        public DbSet<AuditEntry> AuditEntries { get; set; }
        public DbSet<NPF_DIR_NAA> NPF_DIR_NAA { get; set; }
        public DbSet<EmailConfiguration> EmailConfiguration { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<FileProperties> FileProperties { get; set; }
        public DbSet<FileIndex> FileIndex { get; set; }
        public DbSet<FileTags> FileTags { get; set; }
        #endregion
        #region SYS
        public DbSet<NPF_SYS_LookUpValue> NPF_SYS_LookUpValue { get; set; }
        public DbSet<NPF_SYS_LookUpType> NPF_SYS_LookUpType { get; set; }
        public DbSet<NPF_SYS_Modules> NPF_SYS_Modules { get; set; }
        public DbSet<NPF_SYS_AccessRights> NPF_SYS_AccessRights { get; set; }
        public DbSet<NPF_SYS_GeoTest> NPF_SYS_GeoTest { get; set; }
        public DbSet<NPF_SYS_User> NPF_SYS_User { get; set; }
        public DbSet<NPF_SYS_UserDetails> NPF_SYS_UserDetails { get; set; }
        public DbSet<NPF_SYS_AccessRightsDetails> NPF_SYS_AccessRightsDetails { get; set; }
        public DbSet<NPF_SYS_GroupMatrix> NPF_SYS_GroupMatrix { get; set; }
        public DbSet<NPF_SYS_GroupMatrixUser> NPF_SYS_GroupMatrixUser { get; set; }
        public DbSet<NPF_SYS_RowAccess> NPF_SYS_RowAccess { get; set; }
        public DbSet<NPF_SYS_GlobalParam> NPF_SYS_GlobalParam { get; set; }
        public DbSet<NPF_SYS_CodeConfiguration> NPF_SYS_CodeConfiguration { get; set; }
        public DbSet<NPF_SYS_TableCodeConfigurations> NPF_SYS_TableCodeConfigurations { get; set; }
        public DbSet<NPF_SYS_CompanyList> NPF_SYS_CompanyList { get; set; }
        public DbSet<NPF_SYS_UomHeader> NPF_SYS_UomHeader { get; set; }
        public DbSet<NPF_SYS_UomDetails> NPF_SYS_UomDetails { get; set; }
        public DbSet<NPF_SYS_Station> NPF_SYS_Station { get; set; }//C
        public DbSet<NPF_SYS_ResourceType> NPF_SYS_ResourceType { get; set; }//C
        public DbSet<NPF_SYS_Resource> NPF_SYS_Resource { get; set; }
        public DbSet<NPF_SYS_Department> NPF_SYS_Department { get; set; }
        public DbSet<NPF_SYS_Division> NPF_SYS_Division { get; set; }
        public DbSet<NPF_SYS_OfficeZones> NPF_SYS_OfficeZones { get; set; }
        public DbSet<NPF_SYS_ZoneType> NPF_SYS_ZoneType { get; set; }
        public DbSet<NPF_SYS_Zone> NPF_SYS_Zone { get; set; }
        public DbSet<NPF_SYS_ExternalAgencies> NPF_SYS_ExternalAgencies { get; set; }
        public DbSet<NPF_SYS_StationType> NPF_SYS_StationType { get; set; }
        public DbSet<NPF_SYS_IncidentType> NPF_SYS_IncidentType { get; set; }
        public DbSet<NPF_SYS_IncidentSubType> NPF_SYS_IncidentSubType { get; set; }
        public DbSet<NPF_SYS_Shift> NPF_SYS_Shift { get; set; }
        public DbSet<NPF_SYS_Priority> NPF_SYS_Priority { get; set; }
        public DbSet<NPF_SYS_Roster> NPF_SYS_Roster { get; set; }
        public DbSet<NPF_SYS_RosterShiftResourceMapping> NPF_SYS_RosterShiftResourceMapping { get; set; }
        public DbSet<NPF_SYS_RosterResourceMapping> NPF_SYS_RosterResourceMapping { get; set; }

        public DbSet<NPF_SYS_UserType> NPF_SYS_UserType { get; set; }
        public DbSet<NPF_SYS_NatureOfIncident> NPF_SYS_NatureOfIncident { get; set; }
        public DbSet<NPF_SYS_IncidentCode> NPF_SYS_IncidentCode { get; set; }
        public DbSet<NPF_SYS_AssetMake> NPF_SYS_AssetMake { get; set; }
        public DbSet<NPF_SYS_Rank> NPF_SYS_Rank { get; set; }
        public DbSet<NPF_SYS_AssetModel> NPF_SYS_AssetModel { get; set; }
        public DbSet<NPF_SYS_Equipment> NPF_SYS_Equipment { get; set; }
        public DbSet<NPF_SYS_Vehicle> NPF_SYS_Vehicle { get; set; }

        public DbSet<NPF_SYS_PDA> NPF_SYS_PDA { get; set; }
        public DbSet<NPF_SYS_PDAVehicleMapping> NPF_SYS_PDAVehicleMapping { get; set; }
        public DbSet<NPF_SYS_PDAEquipmentMapping> NPF_SYS_PDAEquipmentMapping { get; set; }
        public DbSet<NPF_SYS_PDAResourceMapping> NPF_SYS_PDAResourceMapping { get; set; }
        public DbSet<NPF_SYS_EquipmentMake> NPF_SYS_EquipmentMake { get; set; }
        public DbSet<NPF_SYS_EquipmentModel> NPF_SYS_EquipmentModel { get; set; }
        public DbSet<NPF_SYS_Caller> NPF_SYS_Caller { get; set; }

        #endregion
        #region TRN
        public DbSet<NPF_TRN_IncidentAssignment> NPF_TRN_IncidentAssignment { get; set; }
        public DbSet<NPF_TRN_Incident> NPF_TRN_Incident { get; set; }
        public DbSet<NPF_TRN_IncidentCaller> NPF_TRN_IncidentCaller { get; set; }
        public DbSet<NPF_TRN_IncidentEquipment> NPF_TRN_IncidentEquipment { get; set; }
        public DbSet<NPF_TRN_IncidentExternalAgency> NPF_TRN_IncidentExternalAgency { get; set; }
        public DbSet<NPF_TRN_IncidentLocation> NPF_TRN_IncidentLocation { get; set; }
        public DbSet<NPF_TRN_IncidentOtherStation> NPF_TRN_IncidentOtherStation { get; set; }
        public DbSet<NPF_TRN_IncidentResource> NPF_TRN_IncidentResource { get; set; }
        public DbSet<NPF_TRN_IncidentStation> NPF_TRN_IncidentStation { get; set; }
        public DbSet<NPF_TRN_IncidentVehicle> NPF_TRN_IncidentVehicle { get; set; }
        public DbSet<NPF_TRN_IncidentCasualty> NPF_TRN_IncidentCasualty { get; set; }
        public DbSet<NPF_TRN_IncidentCasualtyDetails> NPF_TRN_IncidentCasualtyDetails { get; set; }
        public DbSet<NPF_TRN_IncidentDamage> NPF_TRN_IncidentDamage { get; set; }
        public DbSet<NPF_TRN_IncidentFatality> NPF_TRN_IncidentFatality { get; set; }
        public DbSet<NPF_TRN_IncidentFatalityDetails> NPF_TRN_IncidentFatalityDetails { get; set; }
        public DbSet<NPF_TRN_IncidentFiles> NPF_TRN_IncidentFiles { get; set; }
        public DbSet<NPF_TRN_IncidentLineChat> NPF_TRN_IncidentLineChat { get; set; }
        public DbSet<NPF_TRN_IncidentChat> NPF_TRN_IncidentChat { get; set; }
        #endregion
        #region FORM
        public DbSet<NPF_FRM_Form> NPF_FRM_Form { get; set; }
        public DbSet<NPF_FRM_FormSectionMapping> NPF_FRM_FormSectionMapping { get; set; }
        public DbSet<NPF_FRM_FormType> NPF_FRM_FormType { get; set; }
        public DbSet<NPF_FRM_Elements> NPF_FRM_Elements { get; set; }
        public DbSet<NPF_FRM_InputTypeDetails> NPF_FRM_InputTypeDetails { get; set; }
        public DbSet<NPF_FRM_InputType> NPF_FRM_InputType { get; set; }
        public DbSet<NPF_FRM_Source> NPF_FRM_Source { get; set; }
        public DbSet<NPF_FRM_SourceType> NPF_FRM_SourceType { get; set; }
        public DbSet<NPF_FRM_Section> NPF_FRM_Section { get; set; }
        public DbSet<NPF_FRM_Parameters> NPF_FRM_Parameters { get; set; }
        public DbSet<NPF_FRM_ParametersList> NPF_FRM_ParametersList { get; set; }
        #endregion
        #region FORM PAGE
        public DbSet<NPF_FRM_FormPage> NPF_FRM_FormPage { get; set; }
        public DbSet<NPF_FRM_FormPageSectionModuleMapping> NPF_FRM_FormPageSectionModuleMapping { get; set; }
        public DbSet<NPF_FRM_FormPageElementModuleMapping> NPF_FRM_FormPageElementModuleMapping { get; set; }
        public DbSet<NPF_FRM_WOElementMapping> NPF_FRM_WOElementMapping { get; set; }
        public DbSet<NPF_FRM_WorkOrder> NPF_FRM_WorkOrder { get; set; }
        public DbSet<NPF_FRM_FormModuleMappingSequence> NPF_FRM_FormModuleMappingSequence { get; set; }
        #endregion
        #region NOT
        public DbSet<NPF_NOT_Event> NPF_NOT_Event { get; set; }
        public DbSet<NPF_NOT_NotificationHeader> NPF_NOT_NotificationHeader { get; set; }
        public DbSet<NPF_NOT_NotificationDetails> NPF_NOT_NotificationDetails { get; set; }
        public DbSet<NPF_NOT_UserMobiles> NPF_NOT_UserMobiles { get; set; }

        #endregion
        #region Workflow
        public DbSet<NPF_SYS_WorkFlowDetails> NPF_SYS_WorkFlowDetails { get; set; }
        public DbSet<NPF_SYS_WorkFlowHeader> NPF_SYS_WorkFlowHeader { get; set; }
        public DbSet<NPF_SYS_WorkFlowProcessAssignmentDetails> NPF_SYS_WorkFlowProcessAssignmentDetails { get; set; }
        public DbSet<NPF_SYS_WorkFlowProcessDetails> NPF_SYS_WorkFlowProcessDetails { get; set; }
        public DbSet<NPF_SYS_WorkFlowProcessHeader> NPF_SYS_WorkFlowProcessHeader { get; set; }
        #endregion
        #region Store
        public DbSet<NPF_STR_StoreMatrix> NPF_STR_StoreMatrix { get; set; }
        public DbSet<NPF_STR_Store> NPF_STR_Store { get; set; }
        public DbSet<NPF_STR_StoreItem> NPF_STR_StoreItem { get; set; }
        public DbSet<NPF_STR_StoreTransaction> NPF_STR_StoreTransaction { get; set; }
        public DbSet<NPF_STR_StockRequest> NPF_STR_StockRequest { get; set; }
        #endregion
        #region SCH
        public DbSet<NPF_SCH_Schedule> NPF_SCH_Schedule { get; set; }
        public DbSet<NPF_SCH_ScheduleServiceMapping> NPF_SCH_ScheduleServiceMapping { get; set; }
        public DbSet<NPF_SCH_EquipmentConsumables> NPF_SCH_EquipmentConsumables { get; set; }
        public DbSet<NPF_SCH_Helpers> NPF_SCH_Helpers { get; set; }
        public DbSet<NPF_SCH_Remarks> NPF_SCH_Remarks { get; set; }
        public DbSet<NPF_SCH_Files> NPF_SCH_Files { get; set; }
        public DbSet<NPF_SCH_SheduleWOChecklist> NPF_SCH_SheduleWOChecklist { get; set; }
        #endregion
        #region Template
        public DbSet<NPF_TMP_TemplateElementMapping> NPF_TMP_TemplateElementMapping { get; set; }
        public DbSet<NPF_TMP_TemplateFormMapping> NPF_TMP_TemplateFormMapping { get; set; }
        #endregion
        #region Approval
        public DbSet<NPF_RES_ApprovalUsers> NPF_RES_ApprovalUsers { get; set; }
        #endregion
        #region Request
        public DbSet<NPF_REQ_Request> NPF_REQ_Request { get; set; }        
        #endregion
        #endregion
    }
}
