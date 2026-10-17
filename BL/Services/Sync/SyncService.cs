using BL.Constants;
using BL.Models.Access;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Sync
{
    public class SyncService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private readonly string _filePath;
        private int _run;
        private int _timer;
        private int Count;
        public SyncService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logsBackground.txt");
            _run = Convert.ToInt32(context.NPF_SYS_GlobalParam.Where(x => x.Name == "RunBackgroundService").FirstOrDefault().Value);
            _timer = Convert.ToInt32(context.NPF_SYS_GlobalParam.Where(x => x.Name == "BackgroundServiceTimer").FirstOrDefault().Value);
        }
        public void SService()
        {
            try
            {
                WriteToFile("Background Service Started.");
                WriteToFile("Executing task at:" + DateTime.Now.ToString());
                // Call your method here
                Sync();
                // Call your method here
                WriteToFile("Task Completed at:" + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {

            }


        }
        public void Sync()
        {
            // Your method logic here
            WriteToFile("YourRepeatedMethod called at:" + DateTime.Now.ToString());
            BaseDTO<dynamic> response = new BaseDTO<dynamic>();

            List<string> lheaders = new List<string>();

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
                catch (Exception ex)
                {

                }

                try
                {
                     loginReponse = new HttpRequestService().ApiCall("Post", urllogin, logindto, lheaders);

                     deserializeloginreponse = JsonConvert.DeserializeObject<dynamic>(loginReponse);
                     sQryResult = deserializeloginreponse.sQryResult;
                     oneTimeToken = deserializeloginreponse.oneTimeToken;
                    if (sQryResult == "Succeeded")
                    {
                        lheaders = new List<string>();
                        lheaders.Add(UserToken.ToString());
                        lheaders.Add(oneTimeToken.ToString());
                        lheaders.Add("workspace");
                        ////Get All Equipment
                        string UrlEquipment = FBaseUrl + context.NPF_SYS_GlobalParam.Where(x => x.Name == "FEquipment").FirstOrDefault().Value;
                        string ListEquipmentRes = new HttpRequestService().ApiCall("Get", UrlEquipment, null, lheaders);
                        var deserializeEquipmentlist = JsonConvert.DeserializeObject<dynamic>(ListEquipmentRes);
                        var listofEquipment = deserializeEquipmentlist.data;
                        foreach (var y in listofEquipment)
                        {
                            //long AssgnId = Convert.ToInt64(y.FMSAssetId);//AssignmentDescId
                            long EqId = Convert.ToInt64(y.id);//AssignmentDesc
                            NPF_SYS_Equipment eq = context.NPF_SYS_Equipment.Where(x => x.GMSEquipmentId == EqId).FirstOrDefault();
                            if (eq == null)
                            {
                                NPF_SYS_Equipment equipment = new NPF_SYS_Equipment()
                                {
                                    Code = "System_Generated_EQ_" + EqId,
                                    Name = y.Name,
                                    GMSEquipmentId = EqId,
                                    FMSMappingDescription = string.Empty,
                                    FMSMappingtId = null,
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
                                eq.Name = y.Name;
                                eq.FMSMappingDescription =string.Empty;
                                eq.FMSMappingtId = null;
                                eq.UpdatedBy = createdBy;
                                eq.UpdatedDate = DateTime.Now;
                                context.NPF_SYS_Equipment.Update(eq);
                                context.SaveChanges();

                            }
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
        #region DTO
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
        #endregion
    }
}
