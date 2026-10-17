using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Ocsp;
using System.Globalization;
using static UI.Controllers.Background.Background;

namespace UI.Controllers.Background
{
    public class Background : BackgroundService
    {
        private readonly string _filePath;
        private int _run;
        private int _timer;
        private int Count;

        public Background()
        {
            // Set the file path where you want to write logs
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logsBackground.txt");
            NPFContext context = new NPFContext();
            ConfigureParam(context);
            _run = Convert.ToInt32(context.NPF_SYS_GlobalParam.Where(x => x.Name == "RunBackgroundService").FirstOrDefault().Value);
            _timer = Convert.ToInt32(context.NPF_SYS_GlobalParam.Where(x => x.Name == "BackgroundServiceTimer").FirstOrDefault().Value);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                WriteToFile("Background Service Started.");

                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_run > 0)
                    {
                        try
                        {
                            WriteToFile("Background Service running.");
                            WriteToFile("Executing task at:" + DateTime.Now.ToString());
                            // Call your method here
                            YourRepeatedMethod();
                            // Call your method here
                            WriteToFile("Task Completed at:" + DateTime.Now.ToString());


                        }
                        catch (Exception ex)
                        {
                            WriteToFile("Error:" + ex.Message.ToString());
                        }
                        NPFContext context = new NPFContext();
                        _run = Convert.ToInt32(context.NPF_SYS_GlobalParam.Where(x => x.Name == "RunBackgroundService").FirstOrDefault().Value);
                        _timer = Convert.ToInt32(context.NPF_SYS_GlobalParam.Where(x => x.Name == "BackgroundServiceTimer").FirstOrDefault().Value);
                        Count = 0;
                        await Task.Delay(_timer, stoppingToken);


                    }
                    else
                    {
                        if (Count == 0)
                        {
                            WriteToFile("Background Service Stopped.");
                            Count = 1;
                        }
                        await Task.Delay(10, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {

            }


        }

        private void YourRepeatedMethod()
        {
            // Your method logic here
            WriteToFile("YourRepeatedMethod called at:" + DateTime.Now.ToString());
            BaseDTO<dynamic> response = new BaseDTO<dynamic>();

            List<string> lheaders = new List<string>();
            NPFContext context = new NPFContext();

            string Email = context.NPF_SYS_GlobalParam.Where(x => x.Name == "FEmail").FirstOrDefault().Value;
            string Password = context.NPF_SYS_GlobalParam.Where(x => x.Name == "FPassword").FirstOrDefault().Value;
            string companyToken = context.NPF_SYS_GlobalParam.Where(x => x.Name == "FcompanyToken").FirstOrDefault().Value;
            Guid UserToken = Guid.Parse(context.NPF_SYS_GlobalParam.Where(x => x.Name == "FUserToken").FirstOrDefault().Value);
            string FBaseUrl = context.NPF_SYS_GlobalParam.Where(x => x.Name == "FleetDataURL").FirstOrDefault().Value;

            LoginFleetDTO logindto = new LoginFleetDTO();
            logindto.user = new User();
            logindto.user.userToken = UserToken;
            logindto.user.email = Email;
            logindto.user.password = Password;
            logindto.companyToken = companyToken;

            lheaders.Add("workspace");
            string urllogin = FBaseUrl + context.NPF_SYS_GlobalParam.Where(x => x.Name == "FLogin").FirstOrDefault().Value;

            //LOGIN
            string loginReponse = new HttpRequestService().ApiCall("Post", urllogin, logindto, lheaders);

            var deserializeloginreponse = JsonConvert.DeserializeObject<dynamic>(loginReponse);
            var sQryResult = deserializeloginreponse.sQryResult;
            var oneTimeToken = deserializeloginreponse.oneTimeToken;
            if (sQryResult == "Succeeded")
            {

                lheaders = new List<string>();
                lheaders.Add(UserToken.ToString());
                lheaders.Add(oneTimeToken.ToString());
                lheaders.Add("workspace");
                Guid createdBy = new Guid();
                if (context.Users.Any(x => x.Email.ToLower() == "admin@naveo.mu"))
                {
                    createdBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@naveo.mu").Select(x => x.Id).FirstOrDefault());
                }
                try
                {
                    //Get All asset
                    string urlGetAssets = FBaseUrl + context.NPF_SYS_GlobalParam.Where(x => x.Name == "FGetMasterData").FirstOrDefault().Value + "?&nm=Assets";
                    string assetlist = new HttpRequestService().ApiCall("Get", urlGetAssets, null, lheaders);
                    var deserializeassetlist = JsonConvert.DeserializeObject<dynamic>(assetlist);
                    var listofasset = deserializeassetlist.data.masterDataItems;

                    foreach (var y in listofasset)
                    {
                        long AsstId = Convert.ToInt64(y.id);
                        NPF_SYS_Vehicle ct = context.NPF_SYS_Vehicle.Where(x => x.FMSAssetId == AsstId).FirstOrDefault();
                        if (ct == null)
                        {
                            NPF_SYS_Vehicle save = new NPF_SYS_Vehicle()
                            {
                                Code = "System_Generated_VH_" + AsstId,
                                RegistrationNo = y.description,
                                Description = string.Empty,
                                FMSAssetId = AsstId,
                                Station = 0,
                                Category = 0,
                                Abbreviation = 0,
                                ParentVehicle = null,
                                Make = 0,
                                Model = 0,
                                Manufacturer = string.Empty,
                                ManufacturingDate = null,
                                SerialNo = string.Empty,
                                ExternalReferenceCode = string.Empty,
                                CreatedBy = createdBy,
                                CreatedDate = DateTime.Now
                            };
                            context.NPF_SYS_Vehicle.Add(save);
                            context.SaveChanges();

                        }
                        else
                        {
                            ct.RegistrationNo = y.description;
                            ct.UpdatedBy = createdBy;
                            ct.UpdatedDate = DateTime.Now;
                            context.NPF_SYS_Vehicle.Update(ct);
                            context.SaveChanges();

                        }
                    }
                }
                catch(Exception ex)
                {

                }

                try
                {
                    ////Get All Equipment
                    string UrlEquipment = FBaseUrl + context.NPF_SYS_GlobalParam.Where(x => x.Name == "FEquipment").FirstOrDefault().Value;
                    string ListEquipmentRes = new HttpRequestService().ApiCall("Get", UrlEquipment, null, lheaders);
                    var deserializeEquipmentlist = JsonConvert.DeserializeObject<dynamic>(ListEquipmentRes);
                    var listofEquipment = deserializeEquipmentlist.data;
                    foreach (var y in listofEquipment)
                    {
                        long AssgnId = Convert.ToInt64(y.FMSAssetId);//AssignmentDescId
                        long EqId = Convert.ToInt64(y.Id);//AssignmentDesc
                        NPF_SYS_Equipment eq = context.NPF_SYS_Equipment.Where(x => x.GMSEquipmentId == EqId).FirstOrDefault();
                        if (eq == null)
                        {
                            NPF_SYS_Equipment equipment = new NPF_SYS_Equipment()
                            {
                                Code = "System_Generated_EQ_" + EqId,
                                Name = y.Name,
                                GMSEquipmentId = EqId,
                                FMSMappingDescription = y.AssignmentDesc,
                                FMSMappingtId = y.AssgnId,
                                Description = string.Empty,
                                Category = 0,
                                Abbreviation = 0,
                                Make = 0,
                                Model = 0,
                                Manufacturer = string.Empty,
                                ManufacturingDate = null,
                                SerialNo = string.Empty,
                                ExternalReferenceCode = string.Empty,
                                ParentEquipment = null,
                                CreatedBy = createdBy,
                                CreatedDate = DateTime.Now
                            };

                            context.NPF_SYS_Equipment.Add(equipment);
                            context.SaveChanges();

                        }
                        else
                        {
                            eq.FMSMappingDescription = y.AssignmentDesc;
                            eq.FMSMappingtId = y.AssgnId;
                            eq.UpdatedBy = createdBy;
                            eq.UpdatedDate = DateTime.Now;
                            context.NPF_SYS_Equipment.Update(eq);
                            context.SaveChanges();

                        }
                    }
                }
                catch (Exception ex)
                {

                }

                ////call live
                //string urllive = FBaseUrl + context.NPF_SYS_GlobalParam.Where(x => x.Name == "FLive").FirstOrDefault().Value;0
                //string LiveReponse = new HttpRequestService().ApiCall("Post", urllive, t, lheaders);
                //response.data = LiveReponse;


            }
        }
        private async Task<bool> ConfigureParam(NPFContext context)
        {
            if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "BackgroundServiceTimer"))
            {
                NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                GP.Name = "BackgroundServiceTimer";
                GP.Value = "300";
                GP.Comment = "Param in seconds";
                GP.AdminOnly = true;
                context.NPF_SYS_GlobalParam.Add(GP);
                context.SaveChanges();
            }
            if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "RunBackgroundService"))
            {
                NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                GP.Name = "RunBackgroundService";
                GP.Value = "0";
                GP.Comment = "If 1 then true : 0";
                GP.AdminOnly = true;
                context.NPF_SYS_GlobalParam.Add(GP);
                context.SaveChanges();
            }
            if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FUserToken"))
            {
                NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                GP.Name = "FUserToken";
                GP.Value = "userToken";
                GP.Comment = "Fleet UserToken";
                GP.AdminOnly = true;
                context.NPF_SYS_GlobalParam.Add(GP);
                context.SaveChanges();
            }
            if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FEmail"))
            {
                NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                GP.Name = "FEmail";
                GP.Value = "FEmail";
                GP.Comment = "Fleet Email";
                GP.AdminOnly = true;
                context.NPF_SYS_GlobalParam.Add(GP);
                context.SaveChanges();
            }
            if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FPassword"))
            {
                NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                GP.Name = "FPassword";
                GP.Value = "FPassword";
                GP.Comment = "Fleet Password";
                GP.AdminOnly = true;
                context.NPF_SYS_GlobalParam.Add(GP);
                context.SaveChanges();
            }

            if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FcompanyToken"))
            {
                NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                GP.Name = "FcompanyToken";
                GP.Value = "hA2.*c9x6a2PgaZMi_6Sg328@1pc35p9";
                GP.Comment = "Fleet companyToken";
                GP.AdminOnly = true;
                context.NPF_SYS_GlobalParam.Add(GP);
                context.SaveChanges();
            }
            if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FLogin"))
            {
                NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                GP.Name = "FLogin";
                GP.Value = "Login";
                GP.Comment = "Fleet Login";
                GP.AdminOnly = true;
                context.NPF_SYS_GlobalParam.Add(GP);
                context.SaveChanges();
            }
            if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FGetMasterData"))
            {
                NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                GP.Name = "FGetMasterData";
                GP.Value = "GetMasterData";
                GP.Comment = "Fleet GetMasterData";
                GP.AdminOnly = true;
                context.NPF_SYS_GlobalParam.Add(GP);
                context.SaveChanges();
            }
            if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FLive"))
            {
                NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                GP.Name = "FLive";
                GP.Value = "Live";
                GP.Comment = "Fleet Live";
                GP.AdminOnly = true;
                context.NPF_SYS_GlobalParam.Add(GP);
                context.SaveChanges();
            }
            if (!context.NPF_SYS_GlobalParam.Any(x => x.Name == "FEquipment"))
            {
                NPF_SYS_GlobalParam GP = new NPF_SYS_GlobalParam();
                GP.Name = "FEquipment";
                GP.Value = "GetEquipmentList";
                GP.Comment = "Fleet Equipment";
                GP.AdminOnly = true;
                context.NPF_SYS_GlobalParam.Add(GP);
                context.SaveChanges();
            }
            return true;
        }
        private void WriteToFile(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_filePath, true))
                {
                    writer.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions here (e.g., log them using another method)
                // For simplicity, just write to console here
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }
        public class LoginFleetDTO
        {

            public User user { get; set; }
            public string companyToken { get; set; }
        }



        public class User
        {
            public int userId { get; set; }
            public Guid userToken { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string email { get; set; }
            public string password { get; set; }
            public string oldPassword { get; set; }
            public List<int> luserID { get; set; } = new List<int>();
            public DateTime lastLogin { get; set; }
            public DateTime lastLogout { get; set; }
            public string firebaseToken { get; set; }


        }
        public class Report
        {
            public List<int> assetIds { get; set; } = new List<int>();
        }
        public class BaseDTO<T>
        {
            public T data { get; set; }
            //public int totalItems { get; set; }
            public Guid oneTimeToken { get; set; }
            public Guid RoleId { get; set; }
            public string sQryResult { get; set; }
            public string errorMessage { get; set; }

        }

        public class BaseAdditionalDataDTO<T, R> : BaseDTO<T>
        {
            public R additionalData { get; set; }

        }
    }
}