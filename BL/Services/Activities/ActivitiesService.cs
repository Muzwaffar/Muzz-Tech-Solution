using BL.Constants;
using BL.Models.Access;
using BL.Models.Activities;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Models.Dashboard;
using BL.Services.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Activities;
using DAL.Models.Administration;
using DAL.Models.Common;
using DAL.Models.Form;
using DAL.Models.FormPage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Mathematics;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Org.BouncyCastle.Asn1.X9;
using Remotion.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Activities
{
    public class ActivitiesService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private NotificationService service;
        private MatrixService Mservice;
        public UserMatrixDTO UM = new UserMatrixDTO();

        public ActivitiesService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            service = new NotificationService();
            Mservice = new MatrixService();
            UM = Mservice.sGetMatrixUserByUserID();
        }
        #region Graph
        public async Task<BaseResponseDTO<MyActivitiesGraphDTO>> GetAllGraph(BaseRequestDTO<DateRangeModuleDTO> dt)
        {
            BaseResponseDTO<MyActivitiesGraphDTO> dto = new BaseResponseDTO<MyActivitiesGraphDTO>();
            MyActivitiesGraphDTO dtx = new MyActivitiesGraphDTO();
            string errorMsg = "No Data Found";
            try
            {
                dtx.ByItems = await GetByItems(dt);
                dtx.PieData = await GetPieData(dt);
                dto.Data = dtx;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = dtx;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }
        public async Task<List<OverDictionanry>> GetByItems(BaseRequestDTO<DateRangeModuleDTO> dt)
        {
            List<OverDictionanry> dto = new List<OverDictionanry>();
            try
            {
                DateTime today = dt.Data.To.EndOfDay();
                DateTime loop = dt.Data.From.StartOfDay();
                List<long> ActivityTypes = new List<long>();
                if (dt.Data.ActivityModule == ActivityModule.Station)
                {
                    ActivityTypes = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "ActivityTypeMappingList" && x.Name == "Station").Select(y => y.Id).ToList();
                }
                else
                {
                    ActivityTypes = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "ActivityTypeMappingList" && x.Name != "Station").Select(y => y.Id).ToList();
                }
                List<NPF_ACT_Activities> Sr = context.NPF_ACT_Activities.Where(s => s.DeleteStatus == false && s.CreatedDate >= loop && s.CreatedDate <= today && ActivityTypes.Contains(s.ActivityType.Type.Id)).ToList();
                List<long> Srl = Sr.GroupBy(p => p.ActivityType).Select(g => g.First()).Select(y => y.ActivityType.Id).ToList();
                foreach (long s in Srl)
                {
                    OverDictionanry keyValuePairs = new OverDictionanry();
                    int count = Sr.Where(x => x.ActivityType.Id == s).Count();
                    keyValuePairs.Key = context.NPF_ACT_ActivityEventType.Where(x => x.Id == s).FirstOrDefault().Name;
                    keyValuePairs.value = count;

                    dto.Add(keyValuePairs);
                }
            }
            catch (Exception ex)
            {
            }
            return dto;
        }
        public async Task<List<OverDictionanry>> GetPieData(BaseRequestDTO<DateRangeModuleDTO> dt)
        {
            List<OverDictionanry> dto = new List<OverDictionanry>();
            try
            {
                DateTime today = dt.Data.To.EndOfDay();
                DateTime loop = dt.Data.From.StartOfDay();
                List<long> ActivityTypes = new List<long>();
                if (dt.Data.ActivityModule == ActivityModule.Station)
                {
                    ActivityTypes = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "ActivityTypeMappingList" && x.Name == "Station").Select(y => y.Id).ToList();
                }
                else
                {
                    ActivityTypes = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "ActivityTypeMappingList" && x.Name != "Station").Select(y => y.Id).ToList();
                }
                List<NPF_ACT_Activities> Sr = context.NPF_ACT_Activities.Where(s => s.DeleteStatus == false && s.CreatedDate >= loop && s.CreatedDate <= today && ActivityTypes.Contains(s.ActivityType.Type.Id)).ToList();
                List<GeneralStatus> Srl = Sr.GroupBy(p => p.Status).Select(g => g.First()).Select(y => y.Status).ToList();
                foreach (GeneralStatus s in Srl)
                {
                    OverDictionanry keyValuePairs = new OverDictionanry();
                    int count = Sr.Where(x => x.Status == s).Count();
                    keyValuePairs.Key = s.ToString();
                    keyValuePairs.value = count;

                    dto.Add(keyValuePairs);
                }
            }
            catch (Exception ex)
            {
            }
            return dto;

        }
        #endregion

        #region GetById - View and Details
        public async Task<BaseResponseDTO<ActivitiesDTO>> GetById(long Id)
        {
            BaseResponseDTO<ActivitiesDTO> dto = new BaseResponseDTO<ActivitiesDTO>();
            dto.Data = new ActivitiesDTO();
            string errorMsg = "No Data Found";
            try
            {
                var MId = context.NPF_SYS_Modules.Where(x => x.Name == "Form Page").FirstOrDefault().Id;
                NPF_ACT_Activities x = context.NPF_ACT_Activities.Where(x => x.Id == Id).FirstOrDefault();
                dto.Data.EndDate = x.EndDate;
                dto.Data.StartDate = x.StartDate;
                dto.Data.Remarks = x.Remarks;
                dto.Data.Status = x.Status;
                dto.Data.ActivityTypeId = x.ActivityType.Id;
                dto.Data.MId = MId.ToString();
                dto.Data.Id = x.Id;
                dto.Data.User = x.User;
                dto.Data.URL = x.URL;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception e)
            {
                dto.Data = new ActivitiesDTO();
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }

            return dto;
        }
        public async Task<BaseResponseDTO<List<ActivitiesDetailsDTO>>> GetByIdDetails(long Id)
        {
            BaseResponseDTO<List<ActivitiesDetailsDTO>> dto = new BaseResponseDTO<List<ActivitiesDetailsDTO>>();
            dto.Data = new List<ActivitiesDetailsDTO>();
            string errorMsg = "No Data Found";
            try
            {
                List<NPF_ACT_ActivitiesDetails> listDetails = context.NPF_ACT_ActivitiesDetails.Where(x => x.Activity.Id == Id && x.DeleteStatus == false).ToList();
                foreach (NPF_ACT_ActivitiesDetails item in listDetails)
                {
                    ActivitiesConfigDTO config = new ActivitiesConfigDTO(){
                        Id = item.Config.Id,
                        ActivityTypeId = item.Config.ActivityType,
                        FieldId = item.Config.FieldId,
                        LabelName = item.Config.LabelName,
                        Type = item.Config.Type,
                        Source = item.Config.Source,
                        ParentId = item.Config.ParentId,
                    };

                    ActivitiesDetailsDTO datum = new ActivitiesDetailsDTO();
                    datum.Id = item.Id;
                    datum.Config = config;
                    datum.Value = item.Value;
                    dto.Data.Add(datum);
                }
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception e)
            {
                dto.Data = new List<ActivitiesDetailsDTO>();
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }
        public async Task<BaseResponseDTO<List<ActivitiesConfigDTO>>> GetByIdConfig(long Id)
        {
            BaseResponseDTO<List<ActivitiesConfigDTO>> dto = new BaseResponseDTO<List<ActivitiesConfigDTO>>();
            dto.Data = new List<ActivitiesConfigDTO>();
            string errorMsg = "No Data Found";
            try
            {
                List<NPF_ACT_ActivitiesConfig> listDetails = context.NPF_ACT_ActivitiesConfig.Where(x => x.ActivityType == Id && x.DeleteStatus == false).ToList();
                if(listDetails.Count > 0)
                {
                    foreach (NPF_ACT_ActivitiesConfig item in listDetails)
                    {
                        ActivitiesConfigDTO config = new ActivitiesConfigDTO()
                        {
                            Id = item.Id,
                            ActivityTypeId = item.ActivityType,
                            FieldId = item.FieldId,
                            LabelName = item.LabelName,
                            Type = item.Type,
                            Source = item.Source,
                            ParentId = item.ParentId,
                        };

                        dto.Data.Add(config);
                    }
                }
                
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception e)
            {
                dto.Data = new List<ActivitiesConfigDTO>();
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }
        #endregion

        #region Dropdowns
        public BaseResponseDTO<List<DropDown>> GetAllDropDownValues(ActivityModule am)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            string errorMsg = "No Data Found";
            DropDown Dd = new DropDown();
            List<long> ActivityTypes = new List<long>();
            if (am == ActivityModule.Station)
            {
                ActivityTypes = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "ActivityTypeMappingList" && x.Name == "Station").Select(y => y.Id).ToList();
            }
            else
            {
                ActivityTypes = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "ActivityTypeMappingList" && x.Name != "Station").Select(y => y.Id).ToList();
            }
            try
            {
                //list of users
                List<ResourceUserDTO> LUsers = (from c in context.NPF_SYS_User
                                                join d in context.Users on c.AId equals d.Id
                                                join e in context.NPF_SYS_UserDetails on c.Id equals e.User.Id
                                                where c.DeleteStatus == false
                                                select new ResourceUserDTO()
                                                {
                                                    Name = d.FullName,
                                                    Id = c.Id,
                                                }).ToList();
                Dd.title = "User";
                Dd.items = new List<DropDownItem>();
                foreach (ResourceUserDTO u in LUsers)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = u.Id;
                    Ddi.text = u.Name;
                    Dd.items.Add(Ddi);
                }

                Ddl.Add(Dd);

                //Activity task list
                List<NPF_ACT_ActivityEventType> Lvl = context.NPF_ACT_ActivityEventType.Where(x => x.DeleteStatus == false && x.IsActivity == true && ActivityTypes.Contains(x.Type.Id)).ToList();
                Dd = new DropDown();
                Dd.title = "ActivitiesTaskList";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_ACT_ActivityEventType lv in Lvl)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lv.Id;
                    Ddi.text = lv.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //Activity Status list
                Dd = new DropDown();
                Dd.title = "ActivitiesStatus";
                Dd.items = new List<DropDownItem>();
                foreach (GeneralStatus st in Enum.GetValues(typeof(GeneralStatus)))
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = (int)st;
                    Ddi.text = st.ToString();
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                dto.Data = Ddl;
                dto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = Ddl;
                dto.AdditionalData = errorMsg;
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }
        public BaseResponseDTO<List<DropDown>> GetDrpdownValuesByLabel(ActivitiesDropdownSearchDTO searchDTO)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();

            List<DropDown> Ddl = new List<DropDown>();
            string errorMsg = "No Data Found";
            DropDown Dd = new DropDown();
            try
            {
                switch (searchDTO.LabelName)
                {
                    case "Scheme Name":
                        List<NPF_FRM_Form> FRM = context.NPF_FRM_Form.Where(x => x.DeleteStatus == false).ToList();
                        Dd = new DropDown();
                        Dd.title = "Scheme Name";
                        Dd.items = new List<DropDownItem>();
                        foreach (NPF_FRM_Form FR in FRM)
                        {
                            DropDownItem Ddi = new DropDownItem();
                            Ddi.Id = FR.Id;
                            Ddi.text = FR.Name;
                            Dd.items.Add(Ddi);
                        }

                        Ddl.Add(Dd);

                        break;

                    case "Scheme Code":
                        List<NPF_FRM_FormPage> FRP = context.NPF_FRM_FormPage.Where(x => x.FormId == searchDTO.Parameter && x.Status == Status.Submitted && x.DeleteStatus == false).ToList();
                        Dd.title = "Scheme Code";
                        Dd.items = new List<DropDownItem>();
                        foreach (NPF_FRM_FormPage m in FRP)
                        {
                            DropDownItem Ddi = new DropDownItem();
                            Ddi.Id = m.Id;
                            Ddi.text = m.Code;
                            Dd.items.Add(Ddi);
                        }

                        Ddl.Add(Dd);
                        break;

                    case "Section":
                        List<NPF_FRM_Section> SEC = (from a in context.NPF_FRM_FormPage
                                                     join b in context.NPF_FRM_FormSectionMapping on a.FormId equals b.FormId
                                                     join c in context.NPF_FRM_Section on b.SectionId equals c.Id
                                                     where a.Id == searchDTO.Parameter && c.DeleteStatus == false
                                                     select new NPF_FRM_Section
                                                     {
                                                         Id = c.Id,
                                                         Code = c.Code,
                                                         Name = c.Name,
                                                         Description = c.Description,
                                                         Print = c.Print,
                                                         Multiple = c.Multiple,
                                                         Notification = c.Notification,
                                                         AssetTypeId = c.AssetTypeId,
                                                         Order = b.Order
                                                     }).ToList();
                        Dd.title = "Section";
                        Dd.items = new List<DropDownItem>();
                        foreach (NPF_FRM_Section m in SEC)
                        {
                            DropDownItem Ddi = new DropDownItem();
                            Ddi.Id = m.Id;
                            Ddi.text = m.Name;
                            Dd.items.Add(Ddi);
                        }

                        Ddl.Add(Dd);
                        break;

                    default:
                        break;
                }

                dto.Data = Ddl;
                dto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = Ddl;
                dto.AdditionalData = errorMsg;
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }
        #endregion

        #region Create - Update - Listing 
        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<ActivitiesDTO> dataToSave, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager , bool IsWF = false, bool IsEvent = false)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {

                NPF_ACT_Activities ac = new NPF_ACT_Activities()
                {
                    StartDate = dataToSave.Data.StartDate,
                    EndDate = dataToSave.Data.EndDate,
                    URL = dataToSave.Data.URL,
                    Remarks = dataToSave.Data.Remarks,
                    Status = dataToSave.Data.Status,
                    ActivityType = context.NPF_ACT_ActivityEventType.Where(x => x.Id == dataToSave.Data.ActivityTypeId).FirstOrDefault(),
                    User = dataToSave.Data.User,
                    MUserId = UM.UID,
                    Access = dataToSave.Data.Access,
                    ParentId = dataToSave.Data.ParentId,
                };
                if (IsEvent)
                {
                    ac.URL = dataToSave.Data.Msg;
                }
                context.NPF_ACT_Activities.Add(ac);
                context.SaveChanges();
                BaseDto.Id = ac.Id;
                if (dataToSave.Data.Details.Count > 0)
                {
                    foreach (ActivitiesDetailsDTO datum in dataToSave.Data.Details)
                    {


                        NPF_ACT_ActivitiesDetails ad = new NPF_ACT_ActivitiesDetails()
                        {
                            Activity = ac,
                            Config = context.NPF_ACT_ActivitiesConfig.Where(x => x.Id == datum.Config.Id).FirstOrDefault(),
                            Value = datum.Value
                        };
                        context.NPF_ACT_ActivitiesDetails.Add(ad);
                        context.SaveChanges();
                    }
                }
               
                Notification notif = new Notification()
                {
                    Id = ac.Id,
                    UserId = ac.User,
                    MType = MessageType.Assigned,
                    MName = PageName.MyActivities,
                    TaskName = ac.ActivityType.Name,
                    URL = dataToSave.Data.URL + ac.Id

                };

                if (IsWF)
                {
                    notif.TaskName = dataToSave.Data.Msg;
                    notif.URL = dataToSave.Data.URL;
                }
                service.SendNotification(notif);
                //Prepare email model to sent to user
                NPF_SYS_GlobalParam Gpa = context.NPF_SYS_GlobalParam.Where(x => x.Name == "ApplicationURL").FirstOrDefault();
                string URL = Gpa.Value + notif.URL;
                string Name = context.Users.Where(x => x.Id == UM.AUID).FirstOrDefault().FullName;
                ApplicationUser au = (from c in context.NPF_SYS_User
                                      join d in context.Users on c.AId equals d.Id
                                      where c.Id == notif.UserId && c.DeleteStatus == false
                                      select new ApplicationUser()
                                      {
                                          FirstName = d.FirstName,
                                          LastName = d.LastName,
                                          Email = d.Email
                                      }).FirstOrDefault();

                EmailSenderModel<dynamic> esm = new EmailSenderModel<dynamic>();
                esm.To = new List<string>() { au.Email };
                esm.Subject = notif.TaskName + " Task Assigned";
                esm.From = "noreply@naveo.mu";
                esm.Body = new List<string>() { "Dear " + au.FullName + ",", "Please note that " + notif.TaskName + " Has Been Assigned To You By " + Name, " ", "Please find link below: ", URL + "", "" };
                if (IsWF)
                {
                    esm.Body = new List<string>() { "Dear " + au.FullName + ",", "Please note that " + dataToSave.Data.Msg + " Has Been Assigned To You By " + Name, " ", "Please find link below: ", URL + "", "" };
                }
                var aaa = await new EmailSender().SendEmail(esm, userManager);
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<ActivitiesDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            Notification notif = new Notification();
            try
            {
                NPF_ACT_Activities ac = context.NPF_ACT_Activities.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                long oldUser = ac.User;
                ac.Remarks = dataToUpdate.Data.Remarks;
                ac.Status = dataToUpdate.Data.Status;

                context.NPF_ACT_Activities.Update(ac);
                context.SaveChanges();

                long CreatedUser = (from a in context.Users
                                    join b in context.NPF_SYS_User on a.Id equals b.AId
                                    where a.Id == ac.CreatedBy.ToString()
                                    select b.Id).FirstOrDefault();
                long currentUser = (from c in context.Users
                                    join d in context.NPF_SYS_User on c.Id equals d.AId
                                    where c.Id == dataToUpdate.userToken
                                    select d.Id).FirstOrDefault();

                if ((oldUser == currentUser) && (ac.User == oldUser) && (currentUser != CreatedUser))
                {
                    notif = new Notification()
                    {
                        Id = ac.Id,
                        UserId = CreatedUser,
                        MType = MessageType.Status,
                        MName = PageName.MyActivities,
                        TaskName = ac.ActivityType.Name,
                        URL = dataToUpdate.Data.URL + ac.Id

                    };
                    await service.SendNotification(notif);

                }
                else if ((oldUser != currentUser) && (ac.User == oldUser) && (currentUser == CreatedUser))
                {
                    notif = new Notification()
                    {
                        Id = ac.Id,
                        UserId = ac.User,
                        MType = MessageType.Status,
                        MName = PageName.MyActivities,
                        TaskName = ac.ActivityType.Name,
                        URL = dataToUpdate.Data.URL + ac.Id

                    };
                    await service.SendNotification(notif);

                }
                else if ((oldUser != ac.User))
                {
                    notif = new Notification()
                    {
                        Id = ac.Id,
                        UserId = oldUser,
                        MType = MessageType.UnAssigned,
                        MName = PageName.MyActivities,
                        TaskName =ac.ActivityType.Name,
                        URL = string.Empty

                    };
                    await service.SendNotification(notif);
                    if (ac.User != CreatedUser)
                    {
                        notif = new Notification()
                        {
                            Id = ac.Id,
                            UserId = ac.User,
                            MType = MessageType.Assigned,
                            MName = PageName.MyActivities,
                            TaskName = ac.ActivityType.Name,
                            URL = dataToUpdate.Data.URL + ac.Id

                        };
                        await service.SendNotification(notif);
                    }
                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<List<ActivitiesListDTO>>> FilterDatatableByPeriod(BaseRequestDTO<DateRangeModuleDTO> dt)
        {
            BaseResponseDTO<List<ActivitiesListDTO>> dto = new BaseResponseDTO<List<ActivitiesListDTO>>();
            List<ActivitiesListDTO> result = new List<ActivitiesListDTO>();
            List<ActivitiesListDTO> Fresult = new List<ActivitiesListDTO>();

            if (dt == null)
            {
                dt = new BaseRequestDTO<DateRangeModuleDTO>();
                dt.Data = new DateRangeModuleDTO();
                dt.Data.From = DateTime.Now.AddDays(-14);
                dt.Data.To = DateTime.Now;
                dt.Data.ActivityModule = ActivityModule.General;
            }

            dt.Data.From = dt.Data.From.StartOfDay();

            dt.Data.To = dt.Data.To.EndOfDay();

            string errorMsg = "No Data Found";
            try
            {
                long Id = (from a in context.Users
                           join b in context.NPF_SYS_User on a.Id equals b.AId
                           where a.UserToken == Guid.Parse(dt.userToken)
                           select b.Id).FirstOrDefault();
                List<long> ActivityTypes = new List<long>();
                if(dt.Data.ActivityModule == ActivityModule.Station)
                {
                    ActivityTypes = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "ActivityTypeMappingList" && x.Name == "Station").Select(y => y.Id).ToList();
                }
                else
                {
                    ActivityTypes = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "ActivityTypeMappingList" && x.Name != "Station").Select(y => y.Id).ToList();
                }
                List<NPF_ACT_Activities> srl = context.NPF_ACT_Activities.Where(x => x.DeleteStatus == false && x.CreatedDate >= dt.Data.From && x.CreatedDate <= dt.Data.To && ActivityTypes.Contains(x.ActivityType.Type.Id)).OrderBy(y=> y.CreatedDate).ToList();
                //       List<NPF_ACT_Activities> srl = srln.GroupBy(x => x.Id).Select(l => l.First()).ToList();
                foreach (NPF_ACT_Activities item in srl)
                {
                    try
                    {
                        ActivitiesListDTO dtx = new ActivitiesListDTO()
                        {
                            Id = item.Id,
                            ActivityTypeName = item.ActivityType.Name,
                            EndDate = item.EndDate == null ? "" : Convert.ToDateTime(item.EndDate).ToString("dd/MMM/yyyy"),
                            StartDate = item.StartDate == null ? "" : Convert.ToDateTime(item.StartDate).ToString("dd/MMM/yyyy"),
                            Remarks = item.Remarks,
                            Status = item.Status.ToString(),
                            CreatedBy = context.Users.Where(x => x.Id == item.CreatedBy.ToString()).FirstOrDefault().FullName,
                            AssignedTo = (from c in context.NPF_SYS_User
                                          join d in context.Users on c.AId equals d.Id
                                          where c.DeleteStatus == false && c.Id == item.User
                                          select d.FullName).FirstOrDefault(),
                            AssignedToId = (from c in context.NPF_SYS_User
                                            join d in context.Users on c.AId equals d.Id
                                            where c.DeleteStatus == false && c.Id == item.User
                                            select c.Id).FirstOrDefault(),
                            UserId = (long)item.MUserId,
                            Access = item.Access,
                            ML = context.NPF_SYS_GroupMatrixUser.Any(x => x.IID == item.MUserId && UM.IDS.Contains(x.GMID) && !UM.ParentIDS.Contains(x.GMID)),

                        };
                        result.Add(dtx);
                    }
                    catch (Exception ex)
                    {

                    }

                }
                Fresult.AddRange(result.Where(x => x.UserId == UM.UID || x.AssignedToId == UM.UID || x.Access == AccessLevel.Matrix && x.ML == true && !UM.ParentIDS.All(i => Mservice.sGetParentIds(x.UserId).Contains(i))).ToList());//Adding Self Created

                dto.Data = Fresult;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = result;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }

            return dto;
        }
        #endregion

    }
}
