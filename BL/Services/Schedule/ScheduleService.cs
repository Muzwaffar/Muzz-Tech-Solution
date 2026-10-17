using BL.Constants;
using BL.Models.Common;
using BL.Models.Form;
using BL.Models.FormPage;
using BL.Models.Modules.Extension;
using BL.Models.Schedule;
using BL.Services.Common;
using BL.Services.ServiceRequest;
using BL.Services.Source;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Access;
using DAL.Models.Administration;
using DAL.Models.Common;
using DAL.Models.Form;
using DAL.Models.FormPage;
using DAL.Models.Notifications;
using DAL.Models.Schedule;
using DAL.Models.WorkFlow;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using NetTopologySuite.Mathematics;
using Newtonsoft.Json;
using NPOI.HPSF;
using NPOI.POIFS.Crypt.Dsig;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace BL.Services.Schedule
{
    public class ScheduleService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public ColourService CS;
        public SourceControlService Sservice;
        public NotificationService Nhs;
        public EventService Event;
        public FormPageService Fservice;
        public UserManager<ApplicationUser> UserManager;
        public ScheduleService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            CS = new ColourService();
            Sservice = new SourceControlService();
            Event = new EventService();
            Nhs = new NotificationService();
            Fservice = new FormPageService();
        }
        //public async Task<BaseResponseDTO<bool>> RemapChecklist()
        //{
        //    BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
        //    try
        //    {
        //        List<long> schlId = context.NPF_SCH_Schedule.Select(x => x.Id).ToList();
        //        foreach (long schid in schlId)
        //        {
        //            List<NPF_SCH_ScheduleServiceMapping> ssml = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == schid).ToList();

        //            if (ssml.Count > 0)
        //            {
        //                foreach (NPF_SCH_ScheduleServiceMapping ssm in ssml)
        //                {
        //                    long SerId = context.NPF_FRM_WorkOrder.Where(x => x.Id == ssm.Id).FirstOrDefault().SectionId;
        //                    List<NPF_FRM_Elements> elml = context.NPF_FRM_Elements.Where(x => x.ServiceId == SerId && x.DeleteStatus == false && x.IsMobileChecklist == true).ToList();
        //                    if (elml.Count > 0)
        //                    {
        //                        foreach (NPF_FRM_Elements el in elml)
        //                        {
        //                            NPF_FRM_WOElementMapping woel = context.NPF_FRM_WOElementMapping.Where(x => x.WOId == ssm.WorkorderId && x.ElementId == el.Id).FirstOrDefault();
        //                            if (woel != null)
        //                            {
        //                                if (woel.Value.ToLower() == "true")
        //                                {
        //                                    NPF_SCH_SheduleWOChecklist swoc = new NPF_SCH_SheduleWOChecklist()
        //                                    {
        //                                        ScheduleId = ssm.ScheduleId,
        //                                        WorkorderId = ssm.WorkorderId,
        //                                        ElementId = woel.ElementId,
        //                                        ElementName = woel.ElementName,
        //                                        ElementType = woel.ElementType,
        //                                        Value = woel.Value,
        //                                        AdditionalValue = woel.AdditionalValue,
        //                                        CheckListValue = false,
        //                                        Status = Status.Draft,
        //                                    };
        //                                    context.NPF_SCH_SheduleWOChecklist.Add(swoc);
        //                                    context.SaveChanges();
        //                                }

        //                            }

        //                        }
        //                    }

        //                }

        //            }
        //        }
        //        BaseDto.Data = true;
        //        BaseDto.QryResult = new QueryResult().SUCEEDED;
        //        return BaseDto;
        //    }
        //    catch (Exception)
        //    {
        //        BaseDto.Data = false;
        //        BaseDto.AdditionalData = "An Error Occured";
        //        BaseDto.QryResult = new QueryResult().FAILED;
        //    }
        //    return BaseDto;
        //}

        #region Web
        public async Task<BaseResponseDTO<bool>> StatusUpdate(BaseRequestDTO<StatusChange> dataToDelId)
        {
            long Id = dataToDelId.Data.Id;
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_FRM_FormPage upd = context.NPF_FRM_FormPage.Where(x => x.Id == Id).FirstOrDefault();
                List<NPF_SCH_Schedule> Schel = context.NPF_SCH_Schedule.Where(x => x.FormPageId == Id && x.DeleteStatus == false).ToList();
                List<NPF_FRM_WorkOrder> WOL = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == upd.Id).ToList();
                switch (dataToDelId.Data.Status)
                {
                    case Status.Unschedule:
                        // check schedule status
                        if (context.NPF_SCH_Schedule.Any(x => x.FormPageId == Id && x.DeleteStatus == false && (x.Status == Status.Stopped || x.Status == Status.Started || x.Status == Status.Completed)))
                        {
                            BaseDto.Data = false;
                            BaseDto.AdditionalData = "Cannot Unschedule, Service(s) Completed/Currently in Progress";
                            BaseDto.QryResult = new QueryResult().FAILED;
                            return BaseDto;
                        }
                        //if no status in progress/completed change SR/WO to draft else promp message 
                        // Delete Schedule
                        else
                        {
                            if (Schel.Count > 0)
                            {
                                foreach (NPF_SCH_Schedule sch in Schel)
                                {
                                    List<NPF_SCH_EquipmentConsumables> ecList = context.NPF_SCH_EquipmentConsumables.Where(x => x.ScheduleId == sch.Id).ToList();
                                    foreach (NPF_SCH_EquipmentConsumables ec in ecList)
                                    {
                                        ec.DeleteStatus = true;
                                        context.NPF_SCH_EquipmentConsumables.Update(ec);
                                        context.SaveChanges();
                                    }
                                    List<NPF_SCH_ScheduleServiceMapping> elml = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == sch.Id && x.DeleteStatus == false).ToList();
                                    foreach (NPF_SCH_ScheduleServiceMapping elm in elml)
                                    {
                                        elm.DeleteStatus = true;
                                        elm.Status = Status.Unschedule;
                                        context.NPF_SCH_ScheduleServiceMapping.Update(elm);
                                        context.SaveChanges();
                                    }
                                    sch.DeleteStatus = true;
                                    context.NPF_SCH_Schedule.Update(sch);
                                    context.SaveChanges();

                                }
                            }
                            if (WOL.Count > 0)
                            {
                                foreach (NPF_FRM_WorkOrder wo in WOL)
                                {
                                    wo.Status = Status.Unschedule;
                                    context.NPF_FRM_WorkOrder.Update(wo);
                                    context.SaveChanges();
                                }
                            }
                            upd.Status = dataToDelId.Data.Status;
                            context.NPF_FRM_FormPage.Update(upd);
                            context.SaveChanges();
                            BaseDto.Data = true;
                            BaseDto.QryResult = new QueryResult().SUCEEDED;
                            return BaseDto;
                        }
                    case Status.Completed:
                        // set all WO/SR/Schedule to Completed
                        if (Schel.Count > 0)
                        {
                            foreach (NPF_SCH_Schedule sch in Schel)
                            {
                                if (sch.Status != Status.Completed || sch.Status != Status.Cancelled || sch.Status != Status.Rejected)
                                {
                                    sch.Status = Status.Completed;
                                    sch.CompletedTime = DateTime.Now;
                                    context.NPF_SCH_Schedule.Update(sch);
                                    context.SaveChanges();
                                    List<long> hlp = context.NPF_SCH_Helpers.Where(x => x.ScheduleId == sch.Id).Select(p => p.ResourceId).ToList();
                                    foreach (long h in hlp)
                                    {
                                        Notification pn = new Notification()
                                        {
                                            Id = sch.Id,
                                            UserId = h,
                                            MType = MessageType.Status
                                        };
                                        await Nhs.SendNotification(pn);

                                    }
                                    List<NPF_SCH_ScheduleServiceMapping> elml = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == sch.Id && x.DeleteStatus == false).ToList();
                                    foreach (NPF_SCH_ScheduleServiceMapping elm in elml)
                                    {
                                        elm.Status = Status.Completed;
                                        elm.CompletedTime = DateTime.Now;
                                        context.NPF_SCH_ScheduleServiceMapping.Update(elm);
                                        context.SaveChanges();
                                    }
                                }

                            }
                        }
                        if (WOL.Count > 0)
                        {
                            foreach (NPF_FRM_WorkOrder wo in WOL)
                            {
                                wo.Status = Status.Completed;
                                context.NPF_FRM_WorkOrder.Update(wo);
                                context.SaveChanges();
                            }
                        }
                        upd.Status = Status.Completed;
                        context.NPF_FRM_FormPage.Update(upd);
                        context.SaveChanges();
                        BaseDto.Data = true;
                        BaseDto.QryResult = new QueryResult().SUCEEDED;
                        return BaseDto;
                    case Status.Cancelled:
                        // set to cancelled 
                        // delete Schedule/Mapping
                        // set to mapping cancelled
                        // check schedule status
                        if (Schel.Count > 0)
                        {
                            foreach (NPF_SCH_Schedule sch in Schel)
                            {
                                if (sch.Status != Status.Completed || sch.Status != Status.Cancelled)
                                {

                                    sch.Status = Status.Cancelled;
                                    context.NPF_SCH_Schedule.Update(sch);
                                    context.SaveChanges();
                                    List<long> hlp = context.NPF_SCH_Helpers.Where(x => x.ScheduleId == sch.Id).Select(p => p.ResourceId).ToList();
                                    foreach (long h in hlp)
                                    {
                                        Notification pn = new Notification()
                                        {
                                            Id = sch.Id,
                                            UserId = h,
                                            MType = MessageType.Status
                                        };
                                        await Nhs.SendNotification(pn);

                                    }
                                    List<NPF_SCH_ScheduleServiceMapping> elml = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == sch.Id && x.DeleteStatus == false).ToList();
                                    foreach (NPF_SCH_ScheduleServiceMapping elm in elml)
                                    {
                                        elm.Status = Status.Cancelled;
                                        context.NPF_SCH_ScheduleServiceMapping.Update(elm);
                                        context.SaveChanges();
                                    }
                                }

                            }
                        }
                        if (WOL.Count > 0)
                        {
                            foreach (NPF_FRM_WorkOrder wo in WOL)
                            {
                                wo.Status = Status.Cancelled;
                                context.NPF_FRM_WorkOrder.Update(wo);
                                context.SaveChanges();
                            }
                        }
                        upd.Status = Status.Cancelled;
                        context.NPF_FRM_FormPage.Update(upd);
                        context.SaveChanges();
                        BaseDto.Data = true;
                        BaseDto.QryResult = new QueryResult().SUCEEDED;
                        return BaseDto;
                    default:
                        BaseDto.Data = false;
                        BaseDto.AdditionalData = "An Error Occured";
                        BaseDto.QryResult = new QueryResult().FAILED;
                        return BaseDto;
                }


            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<bool> DeleteSchedule(BaseRequestDTO<long> dataToDelId)//ScheduleId
        {
            long Id = dataToDelId.Data;
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_SCH_Schedule upd = context.NPF_SCH_Schedule.Where(x => x.Id == Id).FirstOrDefault();
                List<NPF_SCH_ScheduleServiceMapping> elml = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == upd.Id && x.DeleteStatus == false).ToList();
                //   List<Guid> guis = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == upd.Id && x.DeleteStatus == false).Select(x => x.WorkorderId).ToList();
                //   if (context.NPF_FRM_WorkOrder.Any(x => guis.Contains(x.Id) && (x.Status == Status.Started || x.Status == Status.Completed)))
                if (elml.Any(x => x.Status == Status.Started || x.Status == Status.Completed))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Cannot Delete Schedule, Service(s) Completed/Currently in Progress";
                    BaseDto.QryResult = new QueryResult().FAILED;
                    return BaseDto;
                }
                foreach (NPF_SCH_ScheduleServiceMapping elm in elml)
                {
                    elm.DeleteStatus = true;
                    elm.Status = Status.Cancelled;
                    context.NPF_SCH_ScheduleServiceMapping.Update(elm);
                    context.SaveChanges();
                }
                upd.DeleteStatus = true;
                context.NPF_SCH_Schedule.Update(upd);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Deleting Record";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<List<ScheduleListDTO>> FilterDatatableByPeriod(BaseRequestDTO<JobRange> dt)
        {
            //dt.Data.From = dt.Data.From.StartOfDay();
            //dt.Data.To = dt.Data.To.EndOfDay();
            //BaseResponseDTO<List<ScheduleDTO>> dto = new BaseResponseDTO<List<ScheduleDTO>>();
            //List<ScheduleDTO> SDTOL = new List<ScheduleDTO>();
            //List<NPF_FRM_FormPage> Srl = new List<NPF_FRM_FormPage>();
            //string errorMsg = "No Data Found";
            BaseResponseDTO<List<ScheduleListDTO>> dto = new BaseResponseDTO<List<ScheduleListDTO>>();
            List<ScheduleListDTO> result = new List<ScheduleListDTO>();
            if (dt == null)
            {
                dt = new BaseRequestDTO<JobRange>();
                dt.Data = new JobRange();
                dt.Data.From = DateTime.Now.AddDays(-14);
                dt.Data.To = DateTime.Now;
            }

            dt.Data.From = dt.Data.From.StartOfDay();

            dt.Data.To = dt.Data.To.EndOfDay();

            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SCH_Schedule
                          where a.DeleteStatus == false && a.CreatedDate >= dt.Data.From && a.CreatedDate <= dt.Data.To
                          select new ScheduleListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              StartDate = a.StartDateTime,
                              EndDate = a.EndDateTime,
                              Status = a.Status.ToString(),

                          }).ToList();
                //Srl = context.NPF_FRM_FormPage.Where(x => x.DeleteStatus == false
                //&& x.CreatedDate >= dt.Data.From && x.CreatedDate <= dt.Data.To
                //&& x.Status != Status.Completed
                //&& x.Status != Status.Draft
                //&& x.Status != Status.Unschedule
                //&& x.Status != Status.Cancelled).ToList();
                //if (Srl.Count > 0)
                //{
                //    foreach (NPF_FRM_FormPage Sr in Srl)
                //    {
                //        ScheduleDTO SDTO = new ScheduleDTO();
                //        SDTO.ServiceRequestId = Sr.Id;
                //        SDTO.ServiceRequestCode = Sr.Code;
                //        SDTO.Status = Sr.Status;
                //        //SDTO.CustomerName = Sr.CUSName;
                //        //SDTO.Date = Sr.CUSTimeOfDeath != null ? Convert.ToDateTime(Sr.CUSTimeOfDeath) : DateTime.Now;
                //        List<NPF_SCH_Schedule> ScheduleList = context.NPF_SCH_Schedule.Where(x => x.FormPageId == Sr.Id && x.DeleteStatus == false).ToList();
                //        if (ScheduleList.Count > 0)
                //        {
                //            foreach (NPF_SCH_Schedule Schedule in ScheduleList)
                //            {
                //                Schedule.NoOfService = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == Schedule.Id && x.DeleteStatus == false).Count();
                //                Schedule.StartDateTime = Convert.ToDateTime((Schedule.StartDateTime).ToString("yyyy-MM-ddTHH:mm:ss"));

                //            }
                //            SDTO.data = ScheduleList;
                //        }
                //        else
                //        {
                //            SDTO.data = null;
                //        }
                //        SDTOL.Add(SDTO);


                //    }
                //}
                dto.Data = result;
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
        public BaseResponseDTO<List<HelpersTableDTO>> GetHelpersTabeleById(long Id)
        {
            BaseResponseDTO<List<HelpersTableDTO>> dto = new BaseResponseDTO<List<HelpersTableDTO>>();
            List<HelpersTableDTO> result = new List<HelpersTableDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SCH_Helpers
                          join b in context.NPF_SYS_Resource on a.ResourceId equals b.Id
                          where a.ScheduleId == Id && a.DeleteStatus == false && b.DeleteStatus == false
                          select new HelpersTableDTO
                          {
                              ResourceId = a.ResourceId,
                              Name = b.NAA.FullName
                          }).ToList();

                dto.Data = result;
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

        public async Task<BaseResponseDTO<List<DropDown>>> GetDropDownByDate(BaseRequestDTO<GetByDateDTO> data)//ResourceMappingId
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {


                // Supervisor List
                NPF_SYS_LookUpValue lvl = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "ResourceRole" && x.Name == "Supervisor" && x.DeleteStatus == false).FirstOrDefault();
                List<NPF_SYS_Resource> FT = context.NPF_SYS_Resource.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Resource";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_Resource F in FT)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = F.Id;
                    Ddi.text = F.NAA.FullName;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                dto.Data = Ddl;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = Ddl;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }

        public BaseResponseDTO<List<DropDownLogin>> GetSRlDropDownValues()
        {

            BaseResponseDTO<List<DropDownLogin>> dto = new BaseResponseDTO<List<DropDownLogin>>();
            List<DropDownLogin> Ddl = new List<DropDownLogin>();
            DropDownLogin Dd = new DropDownLogin();
            string errorMsg = "No Data Found";
            try
            {
                // Zone List
                List<NPF_SYS_Zone> zone = context.NPF_SYS_Zone.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDownLogin();
                Dd.title = "Zone";
                Dd.items = new List<DropDownItemLogin>();
                foreach (NPF_SYS_Zone zn in zone)
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    Ddi.id = zn.Id.ToString();
                    Ddi.text = zn.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDownLogin();
                Dd.title = "Resource";
                Dd.items = new List<DropDownItemLogin>();
                List<NPF_SYS_Resource> resource = context.NPF_SYS_Resource.Where(x => x.DeleteStatus == false && x.ResourceType.Name == "Station Officer").ToList();
                foreach (NPF_SYS_Resource st in resource)
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    Ddi.id = st.Id.ToString();
                    Ddi.text = st.NAA.FirstName + " " + st.NAA.LastName;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                List<string> names = new List<string>();
                foreach (var item in Enum.GetNames(typeof(ScheduleForm)))
                {
                    names.Add(item);
                }
                Dd = new DropDownLogin();
                Dd.title = "Form";
                Dd.items = new List<DropDownItemLogin>();
                List<Guid> parentModule = context.NPF_SYS_Modules.Where(x => x.DeleteStatus == false && names.Contains(x.Name.Replace(" ", ""))).Select(p => p.Id).ToList();
                List<Guid> childIds = context.NPF_SYS_Modules.Where(x => x.DeleteStatus == false && x.Url == "" && parentModule.Contains((Guid)x.ParentId)).Select(p => p.Id).ToList();
                List<NPF_SYS_Modules> childModule = context.NPF_SYS_Modules.Where(x => x.DeleteStatus == false && x.Url != "" && parentModule.Contains((Guid)x.ParentId)).ToList();
                List<NPF_SYS_Modules> grandChildModule = context.NPF_SYS_Modules.Where(x => x.DeleteStatus == false && x.Url != "" && childIds.Contains((Guid)x.ParentId)).ToList();
                childModule.AddRange(grandChildModule);
                foreach (NPF_SYS_Modules st in childModule)
                {
                    var parentName = context.NPF_SYS_Modules.Where(x => x.DeleteStatus == false && x.Id == st.ParentId).Select(p => p.Name).FirstOrDefault();
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    Ddi.id = st.Id.ToString();
                    Ddi.text = parentName == null ? st.Name : parentName + " - " + st.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                dto.Data = Ddl;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = Ddl;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }
        public BaseResponseDTO<List<DropDown>> GetScheduleItem(string moduleId)//ScheduleId
        {

            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            List<ItemListDTO> Items = new List<ItemListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                bool result = true;
                NPF_SYS_WorkFlowDetails WFH = context.NPF_SYS_WorkFlowDetails.Where(x => x.ModuleId.ToString() == moduleId && x.DeleteStatus == false).FirstOrDefault();
                if (WFH != null)
                {
                    var name = context.NPF_SYS_Modules.Where(x => x.Id.ToString() == moduleId && x.DeleteStatus == false).Select(p => p.Name).FirstOrDefault();
                    //var form = context.NPF_FRM_Form.Where(x => x.Id == id).Select(p => p.Name).FirstOrDefault();
                    Items = (from a in context.NPF_FRM_FormPage
                             where a.DeleteStatus == false && name.Contains(a.FormName)
                             select new ItemListDTO()
                             {
                                 Id = a.Id,
                                 Name = a.Code

                             }).ToList();

                }
                //else
                //{
                //    result = false;
                //    switch (name)
                //    {
                //        case 
                //    }
                //}
                Dd = new DropDown();
                Dd.title = "FormPage";
                Dd.items = new List<DropDownItem>();
                foreach (ItemListDTO lt in Items)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                dto.Data = Ddl;
                dto.QryResult = queryResult.SUCEEDED;
                dto.AdditionalData = result.ToString();
            }
            catch (Exception ex)
            {
                dto.Data = Ddl;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }
        public BaseResponseDTO<List<DropDown>> GetSchDropDownValues(long Id)//ScheduleId
        {

            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                //Dd = new DropDown();
                List<long> Schml = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == Id && x.DeleteStatus == false).Select(x => x.WorkorderId).ToList();

                List<NPF_FRM_WorkOrder> WOl = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == Id && x.Status == Status.Draft && x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Section";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_WorkOrder lt in WOl)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.SectionName;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //WOl = new List<NPF_FRM_WorkOrder>();
                //WOl = context.NPF_FRM_WorkOrder.Where(x => Schml.Contains(x.Id)).ToList();
                //Dd.title = "Section";
                //Dd.items = new List<DropDownItem>();
                //Dd = new DropDown();
                //foreach (NPF_FRM_WorkOrder lt in WOl)
                //{
                //    DropDownItem Ddi = new DropDownItem();
                //    Ddi.Id = lt.Id;
                //    Ddi.text = lt.SectionName;
                //    Dd.items.Add(Ddi);
                //}
                //Ddl.Add(Dd);

                dto.Data = Ddl;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = Ddl;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<AddEditSchduleDTO>> GetById(long Id)//ScheduleId
        {
            BaseResponseDTO<AddEditSchduleDTO> dto = new BaseResponseDTO<AddEditSchduleDTO>();
            AddEditSchduleDTO result = new AddEditSchduleDTO();
            List<ScheduleEqGrid> eqgl = new List<ScheduleEqGrid>();
            string errorMsg = "No Data Found";
            try
            {
                NPF_SCH_Schedule sch = context.NPF_SCH_Schedule.Where(x => x.Id == Id).FirstOrDefault();
                List<long> Services = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == Id && x.DeleteStatus == false).Select(x => x.WorkorderId).ToList();
                List<string> Remarks = context.NPF_SCH_Remarks.Where(x => x.ScheduleId == Id && x.DeleteStatus == false).Select(x => x.Remarks).ToList();
                string Remark = context.NPF_SCH_Remarks.Where(x => x.ScheduleId == Id && x.DeleteStatus == false && x.Type == "WEB").Select(x => x.Remarks).FirstOrDefault();
                result.SchduleId = sch.Id;
                //result.ServiceRquestId = sch.FormPageId;
                result.ScheduleName = sch.Name;
                result.StartTime = sch.StartDateTime;
                result.EndTime = sch.EndDateTime;
                result.Zone = sch.Zone;
                result.Form = sch.Form;
                result.FormPage = sch.FormPageId;
                //result.Services = Services;
                result.Helpers = (from a in context.NPF_SCH_Helpers
                                  join b in context.NPF_SYS_Resource on a.ResourceId equals b.Id
                                  where a.ScheduleId == sch.Id && a.DeleteStatus == false && b.DeleteStatus == false
                                  select new HelpersTableDTO
                                  {
                                      ResourceId = a.ResourceId,
                                      Name = b.NAA.FullName
                                  }).ToList();
                result.Remark = Remark;
                if (result == null)
                {
                    dto.Data = result;
                    dto.QryResult = queryResult.FAILED;
                }
                else
                {
                    dto.Data = result;
                    dto.QryResult = queryResult.SUCEEDED;
                }
            }
            catch (Exception e)
            {
                dto.Data = result;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }


            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<AddEditSchduleDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();

            try
            {
                if (context.NPF_SCH_Schedule.Any(x => x.Name == dataToSave.Data.ScheduleName && x.Form == dataToSave.Data.Form && x.DeleteStatus != true))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Schedule With Same Name Already Exist.";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SCH_Schedule");

                NPF_SCH_Schedule Sch = new NPF_SCH_Schedule()
                {
                    FormPageId = dataToSave.Data.FormPage,
                    Form = dataToSave.Data.Form,
                    Code = BaseDtoS.Data,//"SCH" +
                    Name = dataToSave.Data.ScheduleName,
                    StartDateTime = dataToSave.Data.StartTime,
                    EndDateTime = Convert.ToDateTime(dataToSave.Data.EndTime),
                    Zone = dataToSave.Data.Zone,
                    Status = Status.Schedule,
                    IsMobile = dataToSave.Data.IsMobile
                };
                context.NPF_SCH_Schedule.Add(Sch);
                context.SaveChanges();
                BaseDto.Id = Sch.Id;
                if (dataToSave.Data.Helpers.Count > 0)
                {
                    foreach (HelpersTableDTO hp in dataToSave.Data.Helpers)
                    {
                        NPF_SCH_Helpers dt = new NPF_SCH_Helpers()
                        {
                            ScheduleId = Sch.Id,
                            ResourceId = hp.ResourceId,
                            status = DAL.Common.HelperStatus.Pending
                        };
                        context.NPF_SCH_Helpers.Add(dt);
                        context.SaveChanges();
                        if (!Sch.IsMobile)
                        {
                            Notification pn = new Notification()
                            {
                                Id = Sch.Id,
                                UserId = hp.ResourceId,
                                MType = MessageType.Status
                            };
                            await Nhs.SendNotification(pn);
                        }
                    }

                }
                //if (dataToSave.Data.Services.Count > 0)
                //{
                //    foreach (long sg in dataToSave.Data.Services)
                //    {
                //        long SerId = context.NPF_FRM_WorkOrder.Where(x => x.Id == sg).FirstOrDefault().SectionId;
                //        List<NPF_FRM_Elements> elml = context.NPF_FRM_Elements.Where(x => x.ServiceId == SerId && x.DeleteStatus == false && x.IsMobileChecklist == true).ToList();
                //        if (elml.Count > 0)
                //        {
                //            foreach (NPF_FRM_Elements el in elml)
                //            {
                //                NPF_FRM_WOElementMapping woel = context.NPF_FRM_WOElementMapping.Where(x => x.WOId == sg && x.ElementId == el.Id).FirstOrDefault();
                //                if (woel != null)
                //                {
                //                    if (woel.ElementType == "Checkbox with Text" || woel.ElementType == "Checkbox" || woel.ElementType == "Text")
                //                    {
                //                        NPF_SCH_SheduleWOChecklist swoc = new NPF_SCH_SheduleWOChecklist()
                //                        {
                //                            ScheduleId = Sch.Id,
                //                            WorkorderId = sg,
                //                            ElementId = woel.ElementId,
                //                            ElementName = woel.ElementName,
                //                            ElementType = woel.ElementType,
                //                            Value = woel.Value,
                //                            AdditionalValue = woel.AdditionalValue,
                //                            CheckListValue = false,
                //                            Status = Status.Draft,
                //                        };
                //                        context.NPF_SCH_SheduleWOChecklist.Add(swoc);
                //                        context.SaveChanges();
                //                    }

                //                }

                //            }
                //        }
                NPF_SCH_ScheduleServiceMapping dtx = new NPF_SCH_ScheduleServiceMapping()
                {
                    ScheduleId = Sch.Id,
                    WorkorderId = Convert.ToInt64(Sch.FormPageId),
                    Status = Status.Draft
                };
                context.NPF_SCH_ScheduleServiceMapping.Add(dtx);
                context.SaveChanges();
                //    }

                //}

                NPF_SCH_Remarks dr = new NPF_SCH_Remarks()
                {
                    ScheduleId = Sch.Id,
                    Remarks = dataToSave.Data.Remark,
                    Type = "WEB"
                };
                context.NPF_SCH_Remarks.Add(dr);
                context.SaveChanges();


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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<AddEditSchduleDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();

            try
            {
                if (context.NPF_SCH_Schedule.Any(x => x.Name == dataToUpdate.Data.ScheduleName && x.Id != dataToUpdate.Data.SchduleId && x.FormPageId == dataToUpdate.Data.FormPage && x.DeleteStatus != true))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Schedule With Same Name Already Exist.";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                bool AssignedToCheck = false;
                NPF_SCH_Schedule sc = context.NPF_SCH_Schedule.Where(x => x.Id == dataToUpdate.Data.SchduleId).FirstOrDefault();
                //if (sc.AssignedToId != dataToUpdate.Data.AppUser)
                //{
                //    AssignedToCheck = true;
                //    Notification pn = new Notification()
                //    {
                //        Id = sc.Id,
                //        UserId = sc.AssignedToId,
                //        MType = MessageType.Status
                //    };
                //    await Nhs.SendNotification(pn);
                //}
                sc.FormPageId = dataToUpdate.Data.FormPage;
                sc.Form = dataToUpdate.Data.Form;
                sc.Name = dataToUpdate.Data.ScheduleName;
                sc.StartDateTime = Convert.ToDateTime(dataToUpdate.Data.StartTime);
                sc.EndDateTime = Convert.ToDateTime(dataToUpdate.Data.EndTime);
                sc.Zone = dataToUpdate.Data.Zone;
                sc.Status = Status.Schedule;
                context.NPF_SCH_Schedule.Update(sc);
                context.SaveChanges();

                if (AssignedToCheck == true)
                {
                    List<long> hlp = context.NPF_SCH_Helpers.Where(x => x.ScheduleId == sc.Id).Select(p => p.ResourceId).ToList();
                    foreach (long h in hlp)
                    {
                        Notification pn = new Notification()
                        {
                            Id = sc.Id,
                            UserId = h,
                            MType = MessageType.Status
                        };
                        await Nhs.SendNotification(pn);

                    }
                }
                NPF_SCH_ScheduleServiceMapping dtx = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.Id == sc.Id).FirstOrDefault();
                dtx.WorkorderId = Convert.ToInt64(sc.FormPageId);
                context.NPF_SCH_ScheduleServiceMapping.Update(dtx);
                context.SaveChanges();
                //Add/Update Service
                //List<NPF_SCH_ScheduleServiceMapping> sgc = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == dataToUpdate.Data.SchduleId && x.DeleteStatus == false).ToList();
                //if (dataToUpdate.Data.Services.Count > 0)
                //{
                //    foreach (long sg in dataToUpdate.Data.Services)
                //    {
                //        NPF_SCH_ScheduleServiceMapping c = sgc.Where(x => x.WorkorderId == sg).FirstOrDefault();
                //        if (c == null)
                //        {
                //            long SerId = context.NPF_FRM_WorkOrder.Where(x => x.Id == sg).FirstOrDefault().SectionId;
                //            List<NPF_FRM_Elements> elml = context.NPF_FRM_Elements.Where(x => x.ServiceId == SerId && x.DeleteStatus == false && x.IsMobileChecklist == true).ToList();
                //            if (elml.Count > 0)
                //            {
                //                foreach (NPF_FRM_Elements el in elml)
                //                {
                //                    NPF_FRM_WOElementMapping woel = context.NPF_FRM_WOElementMapping.Where(x => x.WOId == sg && x.ElementId == el.Id).FirstOrDefault();
                //                    if (woel != null)
                //                    {
                //                        if (woel.Value.ToLower() == "true" && (woel.ElementType == "Checkbox with Text" || woel.ElementType == "Checkbox" || woel.ElementType == "Text"))
                //                        {
                //                            NPF_SCH_SheduleWOChecklist swoc = new NPF_SCH_SheduleWOChecklist()
                //                            {
                //                                ScheduleId = sc.Id,
                //                                WorkorderId = sg,
                //                                ElementId = woel.ElementId,
                //                                ElementName = woel.ElementName,
                //                                ElementType = woel.ElementType,
                //                                Value = woel.Value,
                //                                AdditionalValue = woel.AdditionalValue,
                //                                CheckListValue = false,
                //                                Status = Status.Draft,
                //                            };
                //                            context.NPF_SCH_SheduleWOChecklist.Add(swoc);
                //                            context.SaveChanges();
                //                        }
                //                    }
                //                }
                //            }
                //            NPF_SCH_ScheduleServiceMapping dt = new NPF_SCH_ScheduleServiceMapping()
                //            {
                //                ScheduleId = sc.Id,
                //                WorkorderId = sg,
                //                Status = Status.Draft
                //            };
                //            context.NPF_SCH_ScheduleServiceMapping.Add(dt);
                //            context.SaveChanges();

                //        }
                //        else
                //        {
                //            sgc.Remove(c);
                //        }
                //    }

                //}
                //if (sgc.Count > 0)
                //{
                //    foreach (NPF_SCH_ScheduleServiceMapping sm in sgc)
                //    {
                //        long SerId = context.NPF_FRM_WorkOrder.Where(x => x.Id == sm.WorkorderId).FirstOrDefault().SectionId;
                //        List<NPF_SCH_SheduleWOChecklist> swocl = context.NPF_SCH_SheduleWOChecklist.Where(x => x.ScheduleId == sm.ScheduleId && x.WorkorderId == sm.WorkorderId && x.DeleteStatus == false).ToList();
                //        if (swocl.Count > 0)
                //        {
                //            foreach (NPF_SCH_SheduleWOChecklist swoc in swocl)
                //            {


                //                swoc.Status = Status.Cancelled;
                //                swoc.DeleteStatus = true;
                //                context.NPF_SCH_SheduleWOChecklist.Update(swoc);
                //                context.SaveChanges();


                //            }
                //        }

                //        sm.DeleteStatus = true;
                //        sm.Status = Status.Cancelled;
                //        context.NPF_SCH_ScheduleServiceMapping.Update(sm);
                //        context.SaveChanges();
                //    }
                //}
                List<NPF_SCH_Helpers> Currentls = context.NPF_SCH_Helpers.Where(x => x.ScheduleId == dataToUpdate.Data.SchduleId && x.DeleteStatus == false).ToList();
                foreach (HelpersTableDTO d in dataToUpdate.Data.Helpers)
                {
                    if (Currentls.Any(y => y.ScheduleId == dataToUpdate.Data.SchduleId))
                    {
                        Currentls.RemoveAll(x => x.ResourceId == d.ResourceId);
                        Notification pn = new Notification()
                        {
                            Id = sc.Id,
                            UserId = d.ResourceId,
                            MType = MessageType.Status
                        };
                        await Nhs.SendNotification(pn);
                    }
                    else
                    {
                        NPF_SCH_Helpers dt = new NPF_SCH_Helpers()
                        {
                            ScheduleId = sc.Id,
                            ResourceId = d.ResourceId,
                            status = DAL.Common.HelperStatus.Pending
                        };
                        context.NPF_SCH_Helpers.Add(dt);
                        context.SaveChanges();
                        Notification pn = new Notification()
                        {
                            Id = sc.Id,
                            UserId = dt.ResourceId,
                            MType = MessageType.Assigned
                        };
                        await Nhs.SendNotification(pn);
                    }
                }
                if (Currentls.Count > 0)
                {
                    foreach (NPF_SCH_Helpers q in Currentls)
                    {
                        q.DeleteStatus = true;
                        context.NPF_SCH_Helpers.Update(q);
                        context.SaveChanges();
                        Notification pn = new Notification()
                        {
                            Id = sc.Id,
                            UserId = q.ResourceId,
                            MType = MessageType.UnAssigned
                        };
                        await Nhs.SendNotification(pn);
                    }
                }

                List<NPF_SCH_Remarks> rmc = context.NPF_SCH_Remarks.Where(x => x.ScheduleId == dataToUpdate.Data.SchduleId && x.DeleteStatus == false && x.Type == "WEB").ToList();
                if (rmc.Count > 0)
                {
                    foreach (NPF_SCH_Remarks q in rmc)
                    {
                        q.DeleteStatus = true;
                        context.NPF_SCH_Remarks.Update(q);
                        context.SaveChanges();
                    }
                }
                NPF_SCH_Remarks dr = new NPF_SCH_Remarks()
                {
                    ScheduleId = sc.Id,
                    Remarks = dataToUpdate.Data.Remark,
                    Type = "WEB"
                };
                context.NPF_SCH_Remarks.Add(dr);
                context.SaveChanges();


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


        public async Task<BaseResponseDTO<List<DTSection>>> GetServicesByServiceId(BaseRequestDTO<GetServicesDetails> data)
        {
            BaseResponseDTO<List<DTSection>> dto = new BaseResponseDTO<List<DTSection>>();
            try
            {
                List<NPF_FRM_WorkOrder> WOl = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == data.Data.serviceRequestId && data.Data.serviceIds.Contains(x.Id) && x.DeleteStatus == false).ToList();
                if (WOl.Count > 0)
                {
                    List<DTSection> Dtsl = new List<DTSection>();
                    foreach (NPF_FRM_WorkOrder wo in WOl)
                    {
                        DTSection Dts = new DTSection();
                        Dts.sectionId = wo.SectionId;
                        Dts.name = wo.SectionName;
                        Dts.print = wo.Print;
                        Dts.amount = wo.Amount != null ? Convert.ToDecimal(wo.Amount) : 0;
                        Dts.notification = wo.Notification;
                        Dts.elements = new List<Elements>();
                        List<NPF_FRM_WOElementMapping> WOlm = context.NPF_FRM_WOElementMapping.Where(x => x.WOId == wo.Id && x.DeleteStatus == false).ToList();
                        if (WOlm.Count > 0)
                        {
                            foreach (NPF_FRM_WOElementMapping wom in WOlm)
                            {
                                string valueName = string.Empty;
                                List<Values> sourceVal = await Sservice.SourceValues(wom.ElementId);
                                if (sourceVal.Count > 0 && IsValidGUID(wom.Value))
                                {
                                    valueName = sourceVal.Where(x => x.id.ToString() == wom.Value).FirstOrDefault().text;
                                }
                                Elements el = new Elements()
                                {
                                    id = wom.Id,
                                    elementId = wom.ElementId,
                                    elementName = wom.ElementName,
                                    elementType = wom.ElementType,
                                    uom = wom.UOM,
                                    value = sourceVal.Count == 0 ? wom.Value : valueName,
                                    additionalValue = wom.AdditionalValue,
                                    sourceValues = new List<Values>()
                                };
                                Dts.elements.Add(el);
                            }
                        }
                        else
                        {
                            Dts.elements = null;
                        }
                        Dtsl.Add(Dts);
                    }
                    dto.Data = Dtsl;
                }
                else
                {
                    dto.Data = null;
                }
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = null;
                dto.AdditionalData = "Error Fetching Record";
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }

        public BaseResponseDTO<List<DropDown>> GetItemsByFormId(string moduleId)//ScheduleId
        {

            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            List<ItemListDTO> Items = new List<ItemListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                bool result = true;
                //NPF_FRM_FormPage WFH = context.NPF_FRM_FormPage.Where(x => x.FormId.ToString() == moduleId && x.DeleteStatus == false).FirstOrDefault();
                //if (WFH != null)
                ///{
                Items = (from a in context.NPF_FRM_FormPage
                         where a.DeleteStatus == false && a.FormId.ToString() == moduleId
                         select new ItemListDTO()
                         {
                             Id = a.Id,
                             Name = a.Code

                         }).ToList();

                //}
                Dd = new DropDown();
                Dd.title = "Items";
                Dd.items = new List<DropDownItem>();
                foreach (ItemListDTO lt in Items)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                dto.Data = Ddl;
                dto.QryResult = queryResult.SUCEEDED;
                dto.AdditionalData = result.ToString();
            }
            catch (Exception ex)
            {
                dto.Data = Ddl;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }

        private bool IsValidGUID(string GUIDCheck)
        {
            if (!string.IsNullOrEmpty(GUIDCheck))
            {
                return new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$").IsMatch(GUIDCheck);
            }

            return false;
        }


        #endregion

        #region Calendar
        public async Task<BaseResponseDTO<List<CalenderDTO>>> GetCalendar()
        {
            BaseResponseDTO<List<CalenderDTO>> BaseDto = new BaseResponseDTO<List<CalenderDTO>>();
            List<CalenderDTO> BaseDtoS = new List<CalenderDTO>();
            try
            {
                BaseDtoS = (from a in context.NPF_SCH_Schedule
                            join b in context.NPF_FRM_FormPage on a.FormPageId equals b.Id
                            where a.StartDateTime.Month == DateTime.Now.Month && a.DeleteStatus == false
                            select new CalenderDTO()
                            {
                                id = a.Id,
                                title = a.Name,
                                start = a.StartDateTime,
                                end = a.EndDateTime,
                                color = CS.GetColour(a.Status),
                                backgroundcolor = b.Colour
                            }).ToList();
                BaseDto.Data = BaseDtoS;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = null;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<CalenderViewDTO> GetCalendarById(long Id)//ScheduleId
        {
            BaseResponseDTO<CalenderViewDTO> BaseDto = new BaseResponseDTO<CalenderViewDTO>();
            try
            {
                long id = Id;
                NPF_SCH_Schedule sch = context.NPF_SCH_Schedule.Where(x => x.Id == id && x.DeleteStatus != true).FirstOrDefault();
                List<long> Gidl = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == sch.Id && x.DeleteStatus == false).Select(x => x.WorkorderId).ToList();
                List<string> Woname = new List<string>();
                List<Details> Ldet = (from a in context.NPF_SCH_ScheduleServiceMapping
                                      join b in context.NPF_FRM_WorkOrder.Where(x => Gidl.Contains(x.Id)) on a.WorkorderId equals b.Id
                                      where a.ScheduleId == id
                                      select new Details()
                                      {
                                          id = a.Id,
                                          serviceName = b.SectionName,
                                          status = a.Status
                                      }).ToList();
                //  List<NPF_FRM_WorkOrder> Wolist = context.NPF_FRM_WorkOrder.Where(x => Gidl.Contains(x.Id) && x.DeleteStatus != true).ToList();
                foreach (Details det in Ldet)
                {
                    string namestatus = det.serviceName + " - " + det.status.ToString();
                    Woname.Add(namestatus);
                }
                NPF_FRM_FormPage sr = context.NPF_FRM_FormPage.Where(x => x.Id == sch.FormPageId && x.DeleteStatus != true).FirstOrDefault();
                List<NPF_SCH_Helpers> hlpid = context.NPF_SCH_Helpers.Where(x => x.ScheduleId == sch.Id && x.DeleteStatus == false).ToList();
                List<string> hlpl = new List<string>();
                foreach (NPF_SCH_Helpers h in hlpid)
                {
                    string namestatus = context.NPF_SYS_Resource.Where(x => x.Id == h.ResourceId && x.DeleteStatus == false).Select(x => x.NAA.FullName).FirstOrDefault() + " - " + h.status.ToString();
                    hlpl.Add(namestatus);
                }
                List<string> rmk = context.NPF_SCH_Remarks.Where(x => x.ScheduleId == sch.Id && x.DeleteStatus == false).Select(x => x.Remarks).ToList();
                CalenderViewDTO cdto = new CalenderViewDTO()
                {
                    Id = sch.Id,
                    ScheduleName = sch.Name,
                    //ClientName = sr.CUSName,
                    Status = sch.Status,
                    StartTime = sch.StartDateTime,
                    EndTime = sch.EndDateTime,
                    DepartureAdd = context.NPF_SYS_Zone.Where(x => x.Id == sch.Zone).FirstOrDefault().Name,
                    ArrivalAdd = context.NPF_SYS_Zone.Where(x => x.Id == sch.Zone).FirstOrDefault().Name,
                    Services = Woname,
                    //VehicleNo = Vh,
                    Helpers = hlpl,
                    Remarks = rmk,
                };

                BaseDto.Data = cdto;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = null;
                BaseDto.AdditionalData = "Error Fetching Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<List<CalendarGrid>> GetCalendarGrid(long Id)//ScheduleId
        {
            BaseResponseDTO<List<CalendarGrid>> BaseDto = new BaseResponseDTO<List<CalendarGrid>>();
            List<CalendarGrid> BaseDtoS = new List<CalendarGrid>();
            try
            {
                List<NPF_SCH_EquipmentConsumables> eql = context.NPF_SCH_EquipmentConsumables.Where(x => x.ScheduleId == Id && x.DeleteStatus != true).ToList();
                if (eql.Count > 0)
                {
                    foreach (NPF_SCH_EquipmentConsumables eq in eql)
                    {
                        CalendarGrid cg = new CalendarGrid()
                        {
                            AsssetId = eq.Id,
                            AssetName = eq.Name,
                            EquipementConsumable = eq.IsEquipment == true ? "Equipment" : "Concumable",
                            Quantity = eq.Quantity
                        };
                        BaseDtoS.Add(cg);

                    }
                }
                else
                {
                    BaseDto.AdditionalData = "No Data Found";
                }
                BaseDto.Data = BaseDtoS;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = null;
                BaseDto.AdditionalData = "Error Fetching Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<List<FilesGrid>> GetCalendarFiles(long Id)//ScheduleId
        {
            BaseResponseDTO<List<FilesGrid>> BaseDto = new BaseResponseDTO<List<FilesGrid>>();
            List<FilesGrid> BaseDtoS = new List<FilesGrid>();
            try
            {
                List<NPF_SCH_Files> fil = context.NPF_SCH_Files.Where(x => x.ScheduleId == Id && x.DeleteStatus != true).ToList();
                if (fil.Count > 0)
                {
                    foreach (NPF_SCH_Files fi in fil)
                    {
                        FilesGrid cg = new FilesGrid()
                        {
                            FileId = fi.Id,
                            FileName = fi.FileName
                        };
                        BaseDtoS.Add(cg);

                    }
                }
                else
                {
                    BaseDto.AdditionalData = "No Data Found";
                }
                BaseDto.Data = BaseDtoS;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = null;
                BaseDto.AdditionalData = "Error Fetching Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<GetFile> GetFileById(long Id)//FileId
        {
            BaseResponseDTO<GetFile> BaseDto = new BaseResponseDTO<GetFile>();
            GetFile Dto = new GetFile();
            try
            {
                NPF_SCH_Files fil = context.NPF_SCH_Files.Where(x => x.Id == Id).FirstOrDefault();
                Dto.File = fil.File;
                Dto.FileName = fil.FileName;
                BaseDto.Data = Dto;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = Dto;
                BaseDto.AdditionalData = "Error Fetching Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        #endregion

        //#region MoblieApi

        ////public async Task<BaseResponseDTO<List<CalenderMobileDTO>>> GetCalendarMobile(long dataUserId)
        ////{
        ////    BaseResponseDTO<List<CalenderMobileDTO>> BaseDto = new BaseResponseDTO<List<CalenderMobileDTO>>();
        ////    List<CalenderMobileDTO> BaseDtoS = new List<CalenderMobileDTO>();
        ////    try
        ////    {
        ////        List<long> rs = context.NPF_SYS_Resource.Where(x => x.UserId == dataUserId && x.DeleteStatus == false).Select(x => x.Id).ToList();

        ////        List<NPF_SCH_Schedule> SchL = context.NPF_SCH_Schedule.Where(x => x.DeleteStatus != true).ToList();
        ////        foreach (NPF_SCH_Schedule sch in SchL)
        ////        {
        ////            string cl = context.NPF_FRM_FormPage.Where(x => x.Id == sch.FormPageId).FirstOrDefault() == null ? "0XFF05f5d5" : context.NPF_FRM_FormPage.Where(x => x.Id == sch.FormPageId).FirstOrDefault().Colour;
        ////            NPF_FRM_FormPage sr = context.NPF_FRM_FormPage.Where(x => x.Id == sch.FormPageId).FirstOrDefault();
        ////            int index = cl.IndexOf('#');
        ////            string backgroundC = "0XFF" + cl.Substring(index + 1);
        ////            List<long> gc = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == sch.Id && x.DeleteStatus == false).Select(x => x.Id).ToList();
        ////            CalenderMobileDTO cdto = new CalenderMobileDTO()
        ////            {
        ////                id = sch.Id,
        ////                eventName = sch.Name,
        ////                timeFrom = sch.StartDateTime,
        ////                timeTo = sch.EndDateTime,
        ////                status = sch.Status,
        ////                //clientName = sr.CUSName,
        ////                openItem = gc.Count() - context.NPF_SCH_ScheduleServiceMapping.Where(x => gc.Contains(x.Id) && x.Status == Status.Completed).Count(),
        ////                completedItem = context.NPF_SCH_ScheduleServiceMapping.Where(x => gc.Contains(x.Id) && x.Status == Status.Completed).Count(),
        ////                background = backgroundC,

        ////            };
        ////            BaseDtoS.Add(cdto);

        ////        }
        ////        BaseDto.Data = BaseDtoS;
        ////        BaseDto.QryResult = queryResult.SUCEEDED;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        BaseDto.Data = null;
        ////        BaseDto.AdditionalData = "Error Fetching Record";
        ////        BaseDto.QryResult = queryResult.FAILED;
        ////    }
        ////    return BaseDto;
        ////}
        //public async Task<BaseResponseDTO<List<ClientSchSerReqDTO>>> GetClientOverview(string dataUserId)
        //{
        //    BaseResponseDTO<List<ClientSchSerReqDTO>> BaseDto = new BaseResponseDTO<List<ClientSchSerReqDTO>>();
        //    List<ClientSchSerReqDTO> reponse = new List<ClientSchSerReqDTO>();
        //    try
        //    {
        //        long Uid = context.NPF_SYS_User.Where(x => x.AId == dataUserId).FirstOrDefault().Id;
        //        List<long> rs = context.NPF_SYS_Resource.Where(x => x.UserId == Uid && x.DeleteStatus == false).Select(x => x.Id).ToList();

        //        //get client associated with a schedule and service request
        //        reponse = (
        //                                            from sch in context.NPF_SCH_Schedule
        //                                            join serviceRequest in context.NPF_FRM_FormPage on sch.FormPageId equals serviceRequest.Id
        //                                            where sch.DeleteStatus == false
        //                                            //&& serviceRequest.Status == Status.Schedule
        //                                            select new ClientSchSerReqDTO
        //                                            {
        //                                                //clientId = cust.Id,
        //                                                //clientName = cust.NAA.FullName,
        //                                                scheduleName = sch.Name,
        //                                                scheduleId = sch.Id,
        //                                                serviceRequestCode = serviceRequest.Code,

        //                                            }).ToList();



        //        //get all schedule service mapping wrt to scheduleId
        //        foreach (ClientSchSerReqDTO reponseDTO in reponse)
        //        {
        //            List<long> gc = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == reponseDTO.scheduleId && x.DeleteStatus == false).Select(x => x.Id).ToList();
        //            List<NPF_SCH_ScheduleServiceMapping> scheM = context.NPF_SCH_ScheduleServiceMapping.Where(x => gc.Contains(x.Id)).ToList();
        //            reponseDTO.openItem = scheM.Count() - scheM.Where(x => x.Status == Status.Completed).Count();
        //            reponseDTO.completedItem = scheM.Where(x => x.Status == Status.Completed).Count();
        //        }



        //        BaseDto.Data = reponse;
        //        BaseDto.QryResult = queryResult.SUCEEDED;
        //    }
        //    catch (Exception ex)
        //    {
        //        BaseDto.Data = null;
        //        BaseDto.AdditionalData = "Error Fetching Record";
        //        BaseDto.QryResult = queryResult.FAILED;
        //    }
        //    return BaseDto;
        //}

        //public async Task<BaseResponseDTO<List<AllTaskMobileDTO>>> GetAllTaskMobile(DateRangeMobileDTO data)
        //{
        //    BaseResponseDTO<List<AllTaskMobileDTO>> BaseDto = new BaseResponseDTO<List<AllTaskMobileDTO>>();
        //    List<AllTaskMobileDTO> reponse = new List<AllTaskMobileDTO>();
        //    try
        //    {
        //        long Uid = context.NPF_SYS_User.Where(x => x.AId == data.userId).FirstOrDefault().Id;
        //        List<long> rs = context.NPF_SYS_Resource.Where(x => x.UserId == Uid && x.DeleteStatus == false).Select(x => x.Id).ToList();

        //        reponse = (from sch in context.NPF_SCH_Schedule
        //                   join sr in context.NPF_FRM_FormPage on sch.FormPageId equals sr.Id
        //                   //join cust in context.NPF_CLI_Customer on sr.CustomerId equals cust.Id
        //                   where sch.DeleteStatus == false
        //                   select new AllTaskMobileDTO
        //                   {
        //                       scheduleId = sch.Id,
        //                       scheduleName = sch.Name,
        //                       //clientName = sr.CUSName,
        //                       zoneName = context.NPF_SYS_Zone.Where(x => x.Id == sch.Zone).FirstOrDefault().Name,
        //                       time = sch.StartDateTime,
        //                       status = sch.Status,
        //                       numSchedule = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == sch.Id && x.DeleteStatus == false).Count()
        //                   }).Where(x => x.time.Date >= data.from.Date && x.time.Date <= data.to.Date).ToList();

        //        BaseDto.Data = reponse;
        //        BaseDto.QryResult = queryResult.SUCEEDED;
        //    }
        //    catch (Exception ex)
        //    {
        //        BaseDto.Data = null;
        //        BaseDto.AdditionalData = "Error Fetching Record";
        //        BaseDto.QryResult = queryResult.FAILED;
        //    }
        //    return BaseDto;
        //}
        //public async Task<BaseResponseDTO<GetTaskDetailMobileDTO>> GetTaskDetailMobile(long dataScheduleId)
        //{
        //    BaseResponseDTO<GetTaskDetailMobileDTO> BaseDto = new BaseResponseDTO<GetTaskDetailMobileDTO>();
        //    GetTaskDetailMobileDTO reponse = new GetTaskDetailMobileDTO();
        //    TaskHeader th = new TaskHeader();
        //    Info inf = new Info();
        //    List<Details> dts = new List<Details>();
        //    List<Equipment> eq = new List<Equipment>();
        //    List<Remarks> rm = new List<Remarks>();
        //    List<Files> fl = new List<Files>();
        //    Route route = new Route();
        //    route.viaPoint = new List<ViaPoint>();
        //    List<ViaPoint> vpl = new List<ViaPoint>();
        //    List<Helper> hps = new List<Helper>();
        //    try
        //    {
        //        #region Query
        //        NPF_SCH_Schedule sch = context.NPF_SCH_Schedule.Where(x => x.Id == dataScheduleId && x.DeleteStatus == false).FirstOrDefault();
        //        List<NPF_SCH_Helpers> hpl = context.NPF_SCH_Helpers.Where(x => x.ScheduleId == dataScheduleId && x.DeleteStatus == false).ToList();
        //        List<NPF_SCH_Remarks> rml = context.NPF_SCH_Remarks.Where(x => x.ScheduleId == dataScheduleId && x.DeleteStatus == false).ToList();
        //        List<NPF_SCH_Files> flsl = context.NPF_SCH_Files.Where(x => x.ScheduleId == dataScheduleId && x.DeleteStatus == false).ToList();
        //        List<NPF_SCH_EquipmentConsumables> eqc = context.NPF_SCH_EquipmentConsumables.Where(x => x.ScheduleId == dataScheduleId && x.DeleteStatus == false).ToList();
        //        List<long> woid = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == dataScheduleId && x.DeleteStatus == false).Select(x => x.WorkorderId).ToList();
        //        List<NPF_FRM_WorkOrder> wo = context.NPF_FRM_WorkOrder.Where(x => woid.Contains(x.Id) && x.DeleteStatus == false).ToList();
        //        NPF_FRM_FormPage sr = context.NPF_FRM_FormPage.Where(x => x.Id == sch.FormPageId && x.DeleteStatus == false).FirstOrDefault();
        //        List<Details> Ldet = (from a in context.NPF_SCH_ScheduleServiceMapping
        //                              join b in context.NPF_FRM_WorkOrder.Where(x => woid.Contains(x.Id)) on a.WorkorderId equals b.Id
        //                              where a.ScheduleId == dataScheduleId
        //                              select new Details()
        //                              {
        //                                  id = a.Id,
        //                                  serviceName = b.SectionName,
        //                                  status = a.Status
        //                              }).ToList();
        //        #endregion
        //        //th.client = sr.CUSName;
        //        th.taskTitle = sch.Name;
        //        th.code = sr.Code;
        //        th.id = sch.Id;
        //        th.Status = sch.Status;
        //        th.formId = sch.FormPageId;
        //        inf.startTime = sch.StartDateTime;
        //        inf.endTime = sch.EndDateTime;
        //        //inf.contactPerson = sr.CPName;
        //        //inf.contactNumber = sr.CPMobileNo;

        //        if (hpl.Count > 0)
        //        {
        //            foreach (NPF_SCH_Helpers h in hpl)
        //            {
        //                Helper hp = new Helper()
        //                {
        //                    helper = context.NPF_SYS_Resource.Where(x => x.Id == h.ResourceId).FirstOrDefault().NAA.FullName,
        //                    Id = h.Id,
        //                    Status = h.status
        //                };
        //                hps.Add(hp);
        //            }
        //        }
        //        inf.helper = hps;
        //        if (Ldet.Count > 0)
        //        {

        //            dts.AddRange(Ldet);

        //        }

        //        if (eqc.Count > 0)
        //        {
        //            foreach (NPF_SCH_EquipmentConsumables eql in eqc)
        //            {
        //                Equipment e = new Equipment()
        //                {
        //                    title = eql.Name,
        //                    equipmentName = eql.IsEquipment == true ? "Equipment" : "Consumable",
        //                    quantity = eql.Quantity
        //                };
        //                eq.Add(e);
        //            }
        //        }

        //        if (rml.Count > 0)
        //        {
        //            foreach (NPF_SCH_Remarks rmf in rml)
        //            {
        //                string[] rem = rmf.Remarks.Trim().Split('\n');
        //                for (int i = 0; i < rem.Length; i++)
        //                {
        //                    Remarks r = new Remarks() { remarks = rem[i] };
        //                    rm.Add(r);
        //                }


        //            };
        //        }

        //        if (flsl.Count > 0)
        //        {
        //            foreach (NPF_SCH_Files fll in flsl)
        //            {
        //                Files f = new Files() { File = fll.File, Id = fll.Id };
        //                fl.Add(f);
        //            };
        //        }
        //        if(sch != null)
        //        {
        //            //if(sch.Zone != null)
        //            //{
        //            //    ViaPoint vp = new ViaPoint()
        //            //    {
        //            //        viaPointName = context.NPF_SYS_Zone.Where(x => x.Id == sch.Zone).FirstOrDefault().Name,
        //            //        viaPoinTime = sch.StartDateTime,
        //            //        isFirst = true,
        //            //        centerPoint = new CenterPoint()
        //            //        {
        //            //            lat = context.NPF_SYS_Zone.Where(x => x.Id == sch.Zone).FirstOrDefault().CenterPointLat,
        //            //            lon = context.NPF_SYS_Zone.Where(x => x.Id == sch.Zone).FirstOrDefault().CenterPointLong
        //            //        },
        //            //    };
        //            //    vpl.Add(vp);
        //            //    vp = new ViaPoint()
        //            //    {
        //            //        viaPointName = context.NPF_SYS_Zone.Where(x => x.Id == sch.Zone).FirstOrDefault().Name,
        //            //        viaPoinTime = sch.StartDateTime,
        //            //        isFirst = false,
        //            //        centerPoint = new CenterPoint()
        //            //        {
        //            //            lat = context.NPF_SYS_Zone.Where(x => x.Id == sch.Zone).FirstOrDefault().CenterPointLat,
        //            //            lon = context.NPF_SYS_Zone.Where(x => x.Id == sch.Zone).FirstOrDefault().CenterPointLong
        //            //        },
        //            //    };
        //            //    vpl.Add(vp);
        //            //}
        //        }




        //        route.viaPoint.AddRange(vpl);
        //        reponse.taskHeader = th;
        //        reponse.info = inf;
        //        reponse.details = dts;
        //        reponse.equipment = eq;
        //        reponse.remarks = rm;
        //        reponse.File = fl;
        //        reponse.route = route;
        //        BaseDto.Data = reponse;
        //        BaseDto.QryResult = queryResult.SUCEEDED;
        //    }
        //    catch (Exception ex)
        //    {
        //        BaseDto.Data = null;
        //        BaseDto.AdditionalData = "Error Fetching Record";
        //        BaseDto.QryResult = queryResult.FAILED;
        //    }
        //    return BaseDto;
        //}
        //public async Task<BaseResponseDTO<bool>> SaveRemarks(BaseRequestDTO<SaveFileRemarks> dataId)//ScheduleId
        //{
        //    BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
        //    try
        //    {
        //        NPF_SCH_Remarks Rm = new NPF_SCH_Remarks()
        //        {
        //            ScheduleId = dataId.Data.scheduleId,
        //            Remarks = dataId.Data.str,
        //            CreatedBy = Guid.Parse(dataId.userToken.ToString()),
        //            CreatedDate = DateTime.Now,
        //            Type = "Mobile"
        //        };
        //        context.NPF_SCH_Remarks.Add(Rm);
        //        context.SaveChanges();
        //        BaseDto.Data = true;
        //        BaseDto.QryResult = new QueryResult().SUCEEDED;
        //    }
        //    catch (Exception)
        //    {
        //        BaseDto.Data = false;
        //        BaseDto.AdditionalData = "An Error Occured";
        //        BaseDto.QryResult = new QueryResult().FAILED;
        //    }
        //    return BaseDto;
        //}
        //public async Task<BaseResponseDTO<bool>> SaveFile(BaseRequestDTO<SaveFileRemarks> dataId)//ScheduleId
        //{
        //    BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
        //    try
        //    {
        //        NPF_SCH_Files f = new NPF_SCH_Files()
        //        {
        //            ScheduleId = dataId.Data.scheduleId,
        //            File = dataId.Data.str,
        //            FileName = dataId.Data.fileName,
        //            CreatedBy = Guid.Parse(dataId.userToken.ToString()),
        //            CreatedDate = DateTime.Now
        //        };
        //        context.NPF_SCH_Files.Add(f);
        //        context.SaveChanges();
        //        BaseDto.Data = true;
        //        BaseDto.QryResult = new QueryResult().SUCEEDED;
        //    }
        //    catch (Exception)
        //    {
        //        BaseDto.Data = false;
        //        BaseDto.AdditionalData = "An Error Occured";
        //        BaseDto.QryResult = new QueryResult().FAILED;
        //    }
        //    return BaseDto;
        //}
        //public async Task<BaseResponseDTO<bool>> HelperStatus(BaseRequestDTO<Helperstat> dataId)//ScheduleId
        //{
        //    BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
        //    try
        //    {
        //        NPF_SCH_Helpers h = context.NPF_SCH_Helpers.Where(x => x.Id == dataId.Data.Id).FirstOrDefault();
        //        h.status = dataId.Data.Status;
        //        h.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        //        h.UpdatedDate = DateTime.Now;
        //        context.NPF_SCH_Helpers.Update(h);
        //        context.SaveChanges();
        //        BaseDto.Data = true;
        //        BaseDto.QryResult = new QueryResult().SUCEEDED;
        //    }
        //    catch (Exception)
        //    {
        //        BaseDto.Data = false;
        //        BaseDto.AdditionalData = "An Error Occured";
        //        BaseDto.QryResult = new QueryResult().FAILED;
        //    }
        //    return BaseDto;
        //}
        //public async Task<BaseResponseDTO<bool>> DeleteFile(BaseRequestDTO<long> dataScheduleId)//ScheduleId
        //{
        //    BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
        //    try
        //    {
        //        long FileId = dataScheduleId.Data;
        //        NPF_SCH_Files f = context.NPF_SCH_Files.Where(x => x.Id == FileId).FirstOrDefault();
        //        f.DeleteStatus = true;
        //        f.UpdatedBy = Guid.Parse(dataScheduleId.userToken.ToString());
        //        f.UpdatedDate = DateTime.Now;
        //        context.NPF_SCH_Files.Update(f);
        //        context.SaveChanges();
        //        BaseDto.Data = true;
        //        BaseDto.QryResult = new QueryResult().SUCEEDED;
        //    }
        //    catch (Exception)
        //    {
        //        BaseDto.Data = false;
        //        BaseDto.AdditionalData = "An Error Occured";
        //        BaseDto.QryResult = new QueryResult().FAILED;
        //    }
        //    return BaseDto;
        //}
        //public async Task<BaseResponseDTO<List<DropDownApi>>> GetFormList()
        //{
        //    BaseResponseDTO<List<DropDownApi>> BaseDto = new BaseResponseDTO<List<DropDownApi>>();
        //    BaseDto.Data = new List<DropDownApi>();
        //    try
        //    {
        //        List<DropDownApi> f = (from a in context.NPF_FRM_Form
        //                               where a.DeleteStatus == false && a.IsMobile == true
        //                               select new DropDownApi
        //                               {
        //                                   id = a.Id,
        //                                   name = a.Name
        //                               }).ToList();

        //        BaseDto.Data = f;
        //        BaseDto.QryResult = new QueryResult().SUCEEDED;
        //    }
        //    catch (Exception)
        //    {
        //        BaseDto.Data = new List<DropDownApi>();
        //        BaseDto.AdditionalData = "An Error Occured";
        //        BaseDto.QryResult = new QueryResult().FAILED;
        //    }
        //    return BaseDto;
        //}
        //public async Task<BaseResponseDTO<long>> AddFormFromMobile(long FormId, string usertoken)
        //{
        //    BaseResponseDTO<long> BaseDto = new BaseResponseDTO<long>();
        //    try
        //    {
        //        BaseResponseDTO<SectionElements> SA = new BaseResponseDTO<SectionElements>();
        //        long userId = context.NPF_SYS_User.Where(x => x.AId == usertoken).FirstOrDefault().Id;
        //        List<long> FrmpId = context.NPF_FRM_FormPage.Where(x => x.FormId == FormId).Select(y => y.Id).ToList();
        //        long Count = 0;
        //        foreach (long id in FrmpId)
        //        {
        //             Count += context.NPF_SCH_Schedule.Count(x => x.FormPageId == id  && x.IsMobile == true);
        //        }
                
        //        Guid? GUMId = context.NPF_FRM_Form.Where(x => x.Id == FormId).FirstOrDefault().ModuleId;
        //        var sdasf = context.NPF_FRM_Form.Where(x => x.Id == FormId).FirstOrDefault();
        //        Guid Mid = GUMId == null ? new Guid() : Guid.Parse(GUMId.ToString());
        //        SA = await Fservice.GetSectionElementsByForm(FormId, Mid);
        //        if (SA.QryResult == queryResult.SUCEEDED)
        //        {
        //            FormPageDTO FPDTO = new FormPageDTO();
        //            FPDTO.FormId = FormId;
        //            FPDTO.Colour = "#000000";
        //            FPDTO.Status = Status.Submitted;
        //            BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
        //            BaseRequestDTO<FormPageDTO> dt = new BaseRequestDTO<FormPageDTO>();
        //            dt.Data = FPDTO;
        //            dt.Data.Sections = SA.Data.Sections;
        //            dt.userToken = usertoken;
        //            BaseResponseDTO = await Fservice.SaveAsync(dt);
        //            if (BaseResponseDTO.QryResult == queryResult.SUCEEDED)
        //            {
        //                BaseRequestDTO<AddEditSchduleDTO> dto = new BaseRequestDTO<AddEditSchduleDTO>();
        //                dto.Data = new AddEditSchduleDTO();
        //                dto.Data.FormPage = Convert.ToInt64(BaseResponseDTO.AdditionalData);
        //                dto.Data.Form = Mid.ToString();
        //                dto.Data.ScheduleName = "Mobile_Schedule_" + (Count + 1).ToString();
        //                dto.Data.StartTime = DateTime.Now;
        //                dto.Data.EndTime = null;
        //                dto.Data.Zone = null;
        //                dto.Data.IsMobile = true;
        //                dto.userToken = usertoken;
        //                NPF_SYS_Resource resource = context.NPF_SYS_Resource.Where(x => x.DeleteStatus == false && x.ResourceType.Name == "Station Officer" && x.UserId == userId).FirstOrDefault();
        //                HelpersTableDTO hdt = new HelpersTableDTO()
        //                {
        //                    ResourceId = resource.Id,
        //                    Name = resource.NAA.FullName
        //                };
        //                dto.Data.Helpers.Add(hdt);
        //                BaseResponseDTO = await SaveAsync(dto);
        //                BaseDto.Data =BaseResponseDTO.Id;
        //                BaseDto.QryResult = BaseResponseDTO.QryResult;
        //                BaseDto.ErrorMessage = BaseResponseDTO.ErrorMessage;

        //            }


        //        }
        //        else
        //        {
        //            BaseDto.QryResult = new QueryResult().FAILED;
        //        }



        //    }
        //    catch (Exception)
        //    {
        //        BaseDto.Data = 0;
        //        BaseDto.AdditionalData = "An Error Occured";
        //        BaseDto.QryResult = new QueryResult().FAILED;
        //    }
        //    return BaseDto;
        //}
        //public async Task<BaseResponseDTO<bool>> SaveZone(BaseRequestDTO<ScheZone> dataToSave)
        //{
        //    BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
        //    BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();

        //    try
        //    {
        //        NPF_SCH_Schedule sch = context.NPF_SCH_Schedule.Where(x => x.Id == dataToSave.Data.ScheduleId).FirstOrDefault();
        //        if (!context.NPF_SYS_Zone.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.DeleteStatus != true))
        //        {
        //            NPF_SYS_ZoneType zonetype = context.NPF_SYS_ZoneType.Where(x => x.Name.ToLower() == "custom").FirstOrDefault();
        //            BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_Zone");
        //            NPF_SYS_Zone zn = new NPF_SYS_Zone()
        //            {
        //                Name = dataToSave.Data.Name,
        //                Description = dataToSave.Data.Name,
        //                ServiceRequestId = sch.FormPageId,
        //                Type = dataToSave.Data.Type,
        //                GeomDataText = "{'type': 'Feature','properties': {'radius': 20},'geometry': {'type': '" + dataToSave.Data.Type + "','coordinates': [" + dataToSave.Data.CenterPointLat + "," + dataToSave.Data.CenterPointLat + "]}}",
        //                CenterPointLat = dataToSave.Data.CenterPointLat,
        //                CenterPointLong = dataToSave.Data.CenterPointLong,
        //                ZoneType = zonetype,
        //                DeleteStatus = true,
        //                Code = BaseDtoS.Data,//"ZON" + 
        //                CreatedBy = Guid.Parse(dataToSave.userToken.ToString()),
        //                CreatedDate = DateTime.Now
        //            };
        //            context.NPF_SYS_Zone.Add(zn);
        //            context.SaveChanges();
        //            sch.Zone = zn.Id;
        //            sch.UpdatedBy = Guid.Parse(dataToSave.userToken.ToString());
        //            sch.UpdatedDate = DateTime.Now;
        //            context.NPF_SCH_Schedule.Update(sch);
        //            context.SaveChanges();
        //            BaseDto.Data = true;
        //            BaseDto.QryResult = queryResult.SUCEEDED;
        //        }
        //        else
        //        {
        //            BaseDto.Data = false;
        //            BaseDto.AdditionalData = "Zone Name Already Exist";
        //            BaseDto.QryResult = queryResult.FAILED;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        BaseDto.Data = false;
        //        BaseDto.AdditionalData = "Error Adding Record";
        //        BaseDto.QryResult = queryResult.FAILED;
        //    }
        //    return BaseDto;
        //}
        ////public async Task<BaseResponseDTO<bool>> StatusUpdateTaskMobile(BaseRequestDTO<StatusChange> dataId)//ScheduleId
        ////{
        ////    long Id = dataId.Data.Id;
        ////    BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
        ////    try
        ////    {
        ////        NPF_SCH_Schedule Sche = context.NPF_SCH_Schedule.Where(x => x.Id == Id && x.DeleteStatus == false).FirstOrDefault();
        ////        //List<Guid> ScheM = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == Id && x.DeleteStatus == false).Select(x => x.WorkorderId).ToList();
        ////        List<NPF_SCH_ScheduleServiceMapping> ScheM = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.ScheduleId == Id && x.DeleteStatus == false).ToList();
        ////        //List<NPF_SER_WorkOrder> WOL = context.NPF_SER_WorkOrder.Where(x => ScheM.Contains(x.Id)).ToList();
        ////        switch (dataId.Data.Status)
        ////        {
        ////            case Status.Accepted:

        ////                Sche.Status = Status.Accepted;
        ////                Sche.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                Sche.UpdatedDate = DateTime.Now;
        ////                Sche.AcceptedTime = DateTime.Now;
        ////                context.NPF_SCH_Schedule.Update(Sche);
        ////                context.SaveChanges();
        ////                if (ScheM.Count > 0)
        ////                {
        ////                    foreach (NPF_SCH_ScheduleServiceMapping sch in ScheM)
        ////                    {
        ////                        sch.Status = Status.Accepted;
        ////                        sch.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                        sch.UpdatedDate = DateTime.Now;
        ////                        sch.AcceptedTime = DateTime.Now;
        ////                        context.NPF_SCH_ScheduleServiceMapping.Update(sch);
        ////                        context.SaveChanges();
        ////                    }
        ////                }
        ////                BaseDto.Data = true;
        ////                BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////                return BaseDto;
        ////            case Status.Completed:
        ////                Sche.Status = Status.Completed;
        ////                Sche.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                Sche.UpdatedDate = DateTime.Now;
        ////                Sche.CompletedTime = DateTime.Now;
        ////                context.NPF_SCH_Schedule.Update(Sche);
        ////                context.SaveChanges();
        ////                if (ScheM.Count > 0)
        ////                {
        ////                    foreach (NPF_SCH_ScheduleServiceMapping sch in ScheM)
        ////                    {
        ////                        sch.Status = Status.Completed;
        ////                        sch.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                        sch.UpdatedDate = DateTime.Now;
        ////                        sch.CompletedTime = DateTime.Now;
        ////                        context.NPF_SCH_ScheduleServiceMapping.Update(sch);
        ////                        context.SaveChanges();
        ////                    }
        ////                }
        ////                BaseDto.Data = true;
        ////                BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////                return BaseDto;
        ////            case Status.Started:
        ////                Sche.Status = Status.Started;
        ////                Sche.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                Sche.UpdatedDate = DateTime.Now;
        ////                Sche.StartDateTime = DateTime.Now;
        ////                context.NPF_SCH_Schedule.Update(Sche);
        ////                context.SaveChanges();
        ////                if (ScheM.Count > 0)
        ////                {
        ////                    foreach (NPF_SCH_ScheduleServiceMapping sch in ScheM)
        ////                    {
        ////                        sch.Status = Status.Started;
        ////                        sch.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                        sch.UpdatedDate = DateTime.Now;
        ////                        sch.StartedTime = DateTime.Now;
        ////                        context.NPF_SCH_ScheduleServiceMapping.Update(sch);
        ////                        context.SaveChanges();
        ////                    }
        ////                }
        ////                BaseDto.Data = true;
        ////                BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////                return BaseDto;
        ////            case Status.Stopped:
        ////                Sche.Status = Status.Stopped;
        ////                Sche.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                Sche.UpdatedDate = DateTime.Now;
        ////                Sche.StoppedTime = DateTime.Now;
        ////                Sche.Reason = dataId.Data.Reason;
        ////                context.NPF_SCH_Schedule.Update(Sche);
        ////                context.SaveChanges();
        ////                if (ScheM.Count > 0)
        ////                {
        ////                    foreach (NPF_SCH_ScheduleServiceMapping sch in ScheM)
        ////                    {
        ////                        sch.Status = Status.Stopped;
        ////                        sch.Reason = dataId.Data.Reason;
        ////                        sch.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                        sch.UpdatedDate = DateTime.Now;
        ////                        sch.StoppedTime = DateTime.Now;
        ////                        context.NPF_SCH_ScheduleServiceMapping.Update(sch);
        ////                        context.SaveChanges();
        ////                    }
        ////                }
        ////                BaseDto.Data = true;
        ////                BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////                return BaseDto;
        ////            case Status.Rejected:
        ////                Sche.Status = Status.Rejected;
        ////                Sche.Reason = dataId.Data.Reason;
        ////                Sche.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                Sche.UpdatedDate = DateTime.Now;
        ////                Sche.StoppedTime = DateTime.Now;
        ////                context.NPF_SCH_Schedule.Update(Sche);
        ////                context.SaveChanges();
        ////                if (ScheM.Count > 0)
        ////                {
        ////                    foreach (NPF_SCH_ScheduleServiceMapping sch in ScheM)
        ////                    {
        ////                        sch.Status = Status.Rejected;
        ////                        sch.Reason = dataId.Data.Reason;
        ////                        sch.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                        sch.UpdatedDate = DateTime.Now;
        ////                        sch.StoppedTime = DateTime.Now;
        ////                        context.NPF_SCH_ScheduleServiceMapping.Update(sch);
        ////                        context.SaveChanges();
        ////                    }
        ////                }
        ////                BaseDto.Data = true;
        ////                BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////                return BaseDto;
        ////            case Status.Cancelled:
        ////                Sche.Status = Status.Cancelled;
        ////                Sche.Reason = dataId.Data.Reason;
        ////                Sche.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                Sche.UpdatedDate = DateTime.Now;
        ////                context.NPF_SCH_Schedule.Update(Sche);
        ////                context.SaveChanges();
        ////                if (ScheM.Count > 0)
        ////                {
        ////                    foreach (NPF_SCH_ScheduleServiceMapping sch in ScheM)
        ////                    {
        ////                        sch.Status = Status.Rejected;
        ////                        sch.Reason = dataId.Data.Reason;
        ////                        sch.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                        sch.UpdatedDate = DateTime.Now;
        ////                        context.NPF_SCH_ScheduleServiceMapping.Update(sch);
        ////                        context.SaveChanges();
        ////                    }
        ////                }
        ////                BaseDto.Data = true;
        ////                BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////                return BaseDto;
        ////            default:
        ////                BaseDto.Data = false;
        ////                BaseDto.AdditionalData = "An Error Occured";
        ////                BaseDto.QryResult = new QueryResult().FAILED;
        ////                return BaseDto;
        ////        }


        ////    }
        ////    catch (Exception)
        ////    {
        ////        BaseDto.Data = false;
        ////        BaseDto.AdditionalData = "An Error Occured";
        ////        BaseDto.QryResult = new QueryResult().FAILED;
        ////    }
        ////    return BaseDto;
        ////}
        ////public async Task<BaseResponseDTO<bool>> StatusUpdateTaskDetailMobile(BaseRequestDTO<StatusChange> dataId)//NPF_SCH_ScheduleServiceMapping ID
        ////{
        ////    long Id = dataId.Data.Id;
        ////    BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
        ////    try
        ////    {
        ////        // NPF_SCH_ScheduleServiceMapping schm = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.Id == Id).FirstOrDefault();
        ////        NPF_SCH_ScheduleServiceMapping ScheM = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.Id == Id).FirstOrDefault();
        ////        if (ScheM.Status != Status.Cancelled)
        ////        {
        ////            NPF_SCH_Schedule Sche = context.NPF_SCH_Schedule.Where(x => x.Id == ScheM.ScheduleId && x.DeleteStatus == false).FirstOrDefault();

        ////            switch (dataId.Data.Status)
        ////            {

        ////                case Status.Completed:

        ////                    ScheM.Status = Status.Completed;
        ////                    ScheM.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                    ScheM.UpdatedDate = DateTime.Now;
        ////                    ScheM.CompletedTime = DateTime.Now;
        ////                    context.NPF_SCH_ScheduleServiceMapping.Update(ScheM);
        ////                    context.SaveChanges();

        ////                    if (!context.NPF_SCH_ScheduleServiceMapping.Any(x => x.ScheduleId == ScheM.ScheduleId && x.DeleteStatus == false && x.Status != Status.Completed))
        ////                    {
        ////                        Sche.Status = Status.Completed;
        ////                        Sche.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                        Sche.UpdatedDate = DateTime.Now;
        ////                        Sche.CompletedTime = DateTime.Now;
        ////                        context.NPF_SCH_Schedule.Update(Sche);
        ////                        context.SaveChanges();
        ////                    }
        ////                    BaseDto.Data = true;
        ////                    BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////                    return BaseDto;
        ////                case Status.Started:
        ////                    ScheM.Status = Status.Started;
        ////                    ScheM.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                    ScheM.UpdatedDate = DateTime.Now;
        ////                    ScheM.StartedTime = DateTime.Now;
        ////                    context.NPF_SCH_ScheduleServiceMapping.Update(ScheM);
        ////                    context.SaveChanges();
        ////                    if (!context.NPF_SCH_ScheduleServiceMapping.Any(x => x.ScheduleId == ScheM.ScheduleId && x.DeleteStatus == false && x.Status != Status.Started))
        ////                    {
        ////                        Sche.Status = Status.Started;
        ////                        Sche.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                        Sche.UpdatedDate = DateTime.Now;
        ////                        Sche.StartedTime = DateTime.Now;
        ////                        context.NPF_SCH_Schedule.Update(Sche);
        ////                        context.SaveChanges();
        ////                    }
        ////                    BaseDto.Data = true;
        ////                    BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////                    return BaseDto;
        ////                case Status.Stopped:
        ////                    ScheM.Status = Status.Stopped;
        ////                    ScheM.Reason = dataId.Data.Reason;
        ////                    ScheM.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                    ScheM.StoppedTime = DateTime.Now;
        ////                    context.NPF_SCH_ScheduleServiceMapping.Update(ScheM);
        ////                    context.SaveChanges();
        ////                    if (!context.NPF_SCH_ScheduleServiceMapping.Any(x => x.ScheduleId == ScheM.ScheduleId && x.DeleteStatus == false && x.Status != Status.Stopped))
        ////                    {
        ////                        Sche.Status = Status.Stopped;
        ////                        Sche.Reason += dataId.Data.Reason;
        ////                        Sche.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                        Sche.UpdatedDate = DateTime.Now;
        ////                        Sche.StoppedTime = DateTime.Now;
        ////                        context.NPF_SCH_Schedule.Update(Sche);
        ////                        context.SaveChanges();
        ////                    }
        ////                    BaseDto.Data = true;
        ////                    BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////                    return BaseDto;
        ////                case Status.Cancelled:
        ////                    ScheM.Status = Status.Stopped;
        ////                    ScheM.Reason = dataId.Data.Reason;
        ////                    ScheM.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                    ScheM.UpdatedDate = DateTime.Now;
        ////                    context.NPF_SCH_ScheduleServiceMapping.Update(ScheM);
        ////                    context.SaveChanges();
        ////                    if (!context.NPF_SCH_ScheduleServiceMapping.Any(x => x.ScheduleId == ScheM.ScheduleId && x.DeleteStatus == false && x.Status != Status.Cancelled))
        ////                    {
        ////                        Sche.Status = Status.Cancelled;
        ////                        Sche.Reason += dataId.Data.Reason;
        ////                        Sche.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
        ////                        Sche.UpdatedDate = DateTime.Now;
        ////                        context.NPF_SCH_Schedule.Update(Sche);
        ////                        context.SaveChanges();
        ////                    }
        ////                    BaseDto.Data = true;
        ////                    BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////                    return BaseDto;
        ////                default:
        ////                    BaseDto.Data = false;
        ////                    BaseDto.AdditionalData = "An Error Occured";
        ////                    BaseDto.QryResult = new QueryResult().FAILED;
        ////                    return BaseDto;
        ////            }
        ////        }
        ////        else
        ////        {
        ////            BaseDto.Data = false;
        ////            BaseDto.AdditionalData = "Item Was Already Cancelled.";
        ////            BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////        }

        ////    }
        ////    catch (Exception)
        ////    {
        ////        BaseDto.Data = false;
        ////        BaseDto.AdditionalData = "An Error Occured";
        ////        BaseDto.QryResult = new QueryResult().FAILED;
        ////    }
        ////    return BaseDto;
        ////}

        ////public async Task<BaseResponseDTO<List<CheckList>>> GetChecklist(long Id)//WoId*****************************
        ////{
        ////    BaseResponseDTO<List<CheckList>> BaseDto = new BaseResponseDTO<List<CheckList>>();
        ////    List<CheckList> dtl = new List<CheckList>();
        ////    try
        ////    {
        ////        //NPF_SER_WorkOrder WO = context.NPF_SER_WorkOrder.Where(x => x.Id == Id).FirstOrDefault();
        ////        NPF_SCH_ScheduleServiceMapping scheM = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.Id == Id).FirstOrDefault();
        ////        bool IsDisable = scheM.Status == Status.Started ? false : true;
        ////        List<NPF_SCH_SheduleWOChecklist> woel = context.NPF_SCH_SheduleWOChecklist.Where(x => x.WorkorderId == scheM.WorkorderId && x.ScheduleId == scheM.ScheduleId && x.DeleteStatus == false).ToList();
        ////        if (woel.Count > 0)
        ////        {
        ////            foreach (NPF_SCH_SheduleWOChecklist woe in woel)
        ////            {
        ////                NPF_FRM_Elements ele = context.NPF_FRM_Elements.Where(x => x.Id == woe.ElementId).FirstOrDefault();

        ////                CheckList cl = new CheckList()
        ////                {
        ////                    id = woe.Id,
        ////                    name = ele.Name,
        ////                    value = woe.CheckListValue == null ? false : (bool)woe.CheckListValue,
        ////                    isDisable = IsDisable,
        ////                    hasRemarks = ele.InputTypeDetails.Name.Contains("Text") ? true : false,
        ////                    remarks = woe.AdditionalValue == null ? "" : woe.AdditionalValue
        ////                };
        ////                dtl.Add(cl);

        ////            }
        ////        }
        ////        BaseDto.Data = dtl;
        ////        BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        BaseDto.Data = dtl;
        ////        BaseDto.AdditionalData = "Error Fetching Record";
        ////        BaseDto.QryResult = queryResult.FAILED;
        ////    }
        ////    return BaseDto;
        ////}
        ////public async Task<BaseResponseDTO<CustomChecklistWithRemarks>> GetChecklistWithRemarks(long Id)//WoId*****************************
        ////{
        ////    BaseResponseDTO<CustomChecklistWithRemarks> BaseDto = new BaseResponseDTO<CustomChecklistWithRemarks>();
        ////    BaseDto.Data = new CustomChecklistWithRemarks();
        ////    List<CheckListR> dtl = new List<CheckListR>();
        ////    try
        ////    {
        ////        NPF_SCH_ScheduleServiceMapping scheM = context.NPF_SCH_ScheduleServiceMapping.Where(x => x.Id == Id).FirstOrDefault();
        ////        bool IsDisable = scheM.Status == Status.Started ? false : true;
        ////        List<NPF_SCH_SheduleWOChecklist> woel = context.NPF_SCH_SheduleWOChecklist.Where(x => x.WorkorderId == scheM.WorkorderId && x.ScheduleId == scheM.ScheduleId && x.DeleteStatus == false).ToList();
        ////        if (woel.Count > 0)
        ////        {
        ////            foreach (NPF_SCH_SheduleWOChecklist woe in woel)
        ////            {
        ////                NPF_FRM_Elements ele = context.NPF_FRM_Elements.Where(x => x.Id == woe.ElementId).FirstOrDefault();
        ////                if (woe.ElementType == "Checkbox with Text" || woe.ElementType == "Checkbox")
        ////                {
        ////                    CheckListR cl = new CheckListR()
        ////                    {
        ////                        id = woe.Id,
        ////                        name = ele.Name,
        ////                        value = woe.CheckListValue == null ? false : (bool)woe.CheckListValue,
        ////                        isDisable = IsDisable,
        ////                        hasRemarks = ele.InputTypeDetails.Name.Contains("Text") ? true : false,
        ////                        isText = false,
        ////                        remarks = woe.AdditionalValue == null ? "" : woe.AdditionalValue
        ////                    };
        ////                    dtl.Add(cl);
        ////                }
        ////                if (woe.ElementType == "Text")
        ////                {
        ////                    CheckListR clr = new CheckListR()
        ////                    {
        ////                        id = woe.Id,
        ////                        name = ele.Name,
        ////                        value = true,
        ////                        hasRemarks = true,
        ////                        remarks = woe.Value,
        ////                        isText = true,
        ////                        isDisable = IsDisable
        ////                    };
        ////                    dtl.Add(clr);
        ////                }


        ////            }
        ////        }
        ////        BaseDto.Data.checklist = dtl;
        ////        BaseDto.Data.isSave = IsDisable == true ? false : true;
        ////        BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        BaseDto.Data.checklist = dtl;
        ////        BaseDto.Data.isSave = false;
        ////        BaseDto.AdditionalData = "Error Fetching Record";
        ////        BaseDto.QryResult = queryResult.FAILED;
        ////    }
        ////    return BaseDto;
        ////}
        ////#region CustomChecklist
        ////public async Task<BaseResponseDTO<CustomChecklist>> GetCustomChecklist(long Id)//ScheduleId
        ////{
        ////    BaseResponseDTO<CustomChecklist> BaseDto = new BaseResponseDTO<CustomChecklist>();
        ////    BaseDto.Data = new CustomChecklist();
        ////    List<CheckList> dtl = new List<CheckList>();
        ////    try
        ////    {
        ////        NPF_SCH_Schedule Sched = context.NPF_SCH_Schedule.Where(x => x.Id == Id).FirstOrDefault();
        ////        NPF_SYS_GlobalParam lt = context.NPF_SYS_GlobalParam.Where(x => x.Name == "CustomChecklist").FirstOrDefault();
        ////        NPF_FRM_WorkOrder WO = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == Sched.FormPageId && x.SectionName == lt.Value).FirstOrDefault();
        ////        bool IsDisable = WO.Status == Status.Started ? true : false;
        ////        IsDisable = WO.Status == Status.Completed ? true : false;
        ////        List<NPF_FRM_WOElementMapping> woel = context.NPF_FRM_WOElementMapping.Where(x => x.WOId == WO.Id && x.DeleteStatus == false).ToList();
        ////        if (woel.Count > 0)
        ////        {
        ////            foreach (NPF_FRM_WOElementMapping woe in woel)
        ////            {
        ////                NPF_FRM_Elements ele = context.NPF_FRM_Elements.Where(x => x.Id == woe.ElementId).FirstOrDefault();
        ////                if (ele.IsMobileChecklist == true)
        ////                {
        ////                    CheckList cl = new CheckList()
        ////                    {
        ////                        id = woe.Id,
        ////                        name = ele.Name,
        ////                        value = woe.Value == null ? false : woe.Value.ToLower() == "true" ? true : false,
        ////                        isDisable = IsDisable,
        ////                        hasRemarks = ele.InputTypeDetails.Name.Contains("Text") ? true : false,
        ////                        remarks = woe.AdditionalValue
        ////                    };
        ////                    dtl.Add(cl);
        ////                }
        ////            }
        ////        }
        ////        BaseDto.Data.checklist.AddRange(dtl);
        ////        BaseDto.Data.isSave = IsDisable;
        ////        BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        BaseDto.Data.checklist = new List<CheckList>();
        ////        BaseDto.Data.isSave = true;
        ////        BaseDto.QryResult = queryResult.SUCEEDED;
        ////    }
        ////    return BaseDto;
        ////}

        ////public async Task<BaseResponseDTO<CustomChecklistWithRemarks>> GetCustomChecklistWithRemarks(long Id)//ScheduleId
        ////{
        ////    BaseResponseDTO<CustomChecklistWithRemarks> BaseDto = new BaseResponseDTO<CustomChecklistWithRemarks>();
        ////    BaseDto.Data = new CustomChecklistWithRemarks();
        ////    List<CheckListR> dtl = new List<CheckListR>();
        ////    try
        ////    {
        ////        NPF_SCH_Schedule Sched = context.NPF_SCH_Schedule.Where(x => x.Id == Id).FirstOrDefault();
        ////        NPF_SYS_GlobalParam lt = context.NPF_SYS_GlobalParam.Where(x => x.Name == "CustomChecklist").FirstOrDefault();
        ////        NPF_FRM_WorkOrder WO = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == Sched.FormPageId && x.SectionName == lt.Value).FirstOrDefault();
        ////        bool IsDisable = WO.Status == Status.Started ? true : false;
        ////        IsDisable = WO.Status == Status.Completed ? true : false;
        ////        List<NPF_FRM_WOElementMapping> woel = context.NPF_FRM_WOElementMapping.Where(x => x.WOId == WO.Id && x.DeleteStatus == false).ToList();
        ////        if (woel.Count > 0)
        ////        {
        ////            foreach (NPF_FRM_WOElementMapping woe in woel)
        ////            {
        ////                NPF_FRM_Elements ele = context.NPF_FRM_Elements.Where(x => x.Id == woe.ElementId).FirstOrDefault();
        ////                if (ele.IsMobileChecklist == true)
        ////                {
        ////                    if (woe.ElementType == "Checkbox with Text" || woe.ElementType == "Checkbox")
        ////                    {
        ////                        CheckListR cl = new CheckListR()
        ////                        {
        ////                            id = woe.Id,
        ////                            name = ele.Name,
        ////                            value = woe.Value == null ? false : woe.Value.ToLower() == "true" ? true : false,
        ////                            isDisable = IsDisable,
        ////                            isText = false,
        ////                            hasRemarks = ele.InputTypeDetails.Name.Contains("Text") ? true : false,
        ////                            remarks = woe.AdditionalValue
        ////                        };
        ////                        dtl.Add(cl);
        ////                    }
        ////                    if (woe.ElementType == "Text")
        ////                    {
        ////                        CheckListR clr = new CheckListR()
        ////                        {
        ////                            id = woe.Id,
        ////                            name = ele.Name,
        ////                            value = true,
        ////                            isDisable = IsDisable,
        ////                            isText = true,
        ////                            hasRemarks = true,
        ////                            remarks = woe.Value
        ////                        };
        ////                        dtl.Add(clr);
        ////                    }
        ////                }
        ////            }
        ////        }
        ////        BaseDto.Data.checklist = dtl;
        ////        BaseDto.Data.isSave = IsDisable;
        ////        BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        BaseDto.Data.checklist = dtl;
        ////        BaseDto.Data.isSave = true;
        ////        BaseDto.QryResult = queryResult.SUCEEDED;
        ////    }
        ////    return BaseDto;
        ////}
        ////public async Task<BaseResponseDTO<CustomAccess>> CheckCustomAccess(BaseRequestDTO<long> dataId)//UserId
        ////{
        ////    BaseResponseDTO<CustomAccess> BaseDto = new BaseResponseDTO<CustomAccess>();
        ////    BaseDto.Data = new CustomAccess();
        ////    try
        ////    {
        ////        NPF_SYS_Resource Rs = context.NPF_SYS_Resource.Where(x => x.UserId == dataId.Data).FirstOrDefault();
        ////        NPF_SYS_GlobalParam lt = context.NPF_SYS_GlobalParam.Where(x => x.Name == "CustomChecklist").FirstOrDefault();
        ////        if (Rs.CustomAcess == true)
        ////        {
        ////            BaseDto.Data.hasAccess = true;
        ////            BaseDto.Data.name = lt.AdditionalValue;
        ////            BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////        }
        ////        else
        ////        {
        ////            BaseDto.Data.hasAccess = false;
        ////            BaseDto.Data.name = string.Empty;
        ////            BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////        }

        ////    }
        ////    catch (Exception)
        ////    {
        ////        BaseDto.Data.hasAccess = false;
        ////        BaseDto.Data.name = string.Empty;
        ////        BaseDto.QryResult = new QueryResult().SUCEEDED;
        ////    }
        ////    return BaseDto;
        ////}
        //public async Task<BaseResponseDTO<bool>> SaveCustomChecklist(BaseRequestDTO<SaveCustomChecklist> dt)//ScheduleId
        //{
        //    BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
        //    BaseResponseDTO<bool> Bsdt = new BaseResponseDTO<bool>();
        //    NPF_NOT_Event ev = new NPF_NOT_Event();
        //    try
        //    {
        //        List<CheckList> checklistdt = new List<CheckList>();
        //        checklistdt = JsonConvert.DeserializeObject<List<CheckList>>(dt.Data.checklist);
        //        NPF_SCH_Schedule Sched = context.NPF_SCH_Schedule.Where(x => x.Id == dt.Data.scheduleId).FirstOrDefault();
        //        NPF_SYS_GlobalParam lt = context.NPF_SYS_GlobalParam.Where(x => x.Name == "CustomChecklist").FirstOrDefault();
        //        NPF_FRM_FormPage sr = context.NPF_FRM_FormPage.Where(x => x.Id == Sched.FormPageId).FirstOrDefault();
        //        ApplicationUser app = context.Users.Where(x => x.Id.ToUpper() == dt.userToken.ToUpper()).FirstOrDefault();
        //        NPF_FRM_WorkOrder WO = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == Sched.FormPageId && x.SectionName == lt.Value).FirstOrDefault();
        //        bool IsDisable = WO.Status == Status.Started ? false : true;
        //        if (dt.Data.checklist != null)
        //        {

        //            if (checklistdt.Count > 0)
        //            {
        //                foreach (CheckList ckl in checklistdt)
        //                {
        //                    NPF_FRM_WOElementMapping woel = context.NPF_FRM_WOElementMapping.Where(x => x.Id == ckl.id).FirstOrDefault();
        //                    if (woel != null)
        //                    {
        //                        woel.Value = ckl.value.ToString().ToLower();
        //                        woel.AdditionalValue = ckl.value == false ? "" : ckl.remarks == null ? "" : ckl.remarks;
        //                        context.NPF_FRM_WOElementMapping.Update(woel);
        //                        context.SaveChanges();


        //                        woel = new NPF_FRM_WOElementMapping();
        //                    }
        //                }
        //            }
        //            ev = new NPF_NOT_Event()
        //            {
        //                UserId = sr.CreatedBy.ToString(),
        //                ResourceName = app.FullName,
        //                ResourceId = dt.userToken,
        //                Title = "Checklist Update",
        //                Detail = "Service Request Checklist " + sr.Code + " was updated by user:" + app.FullName,
        //                TargetType = TargetType.Redirect,
        //                Target = "/ServiceRequest/AddEditView?ID=",
        //                TargetId = sr.Id,
        //                Priority = Priority.Low,
        //                Colour = "#2870EA",
        //                Image = "",
        //                IsNotified = false,
        //                ActionDate = DateTime.Now,
        //                IsRead = false,
        //                CreatedBy = Guid.Parse(app.Id)
        //            };


        //        }
        //        Bsdt = await Event.SaveEvent(ev);
        //        BaseDto.Data = true;
        //        BaseDto.QryResult = new QueryResult().SUCEEDED;
        //    }
        //    catch (Exception ex)
        //    {
        //        BaseDto.Data = false;
        //        BaseDto.AdditionalData = "Error Saving Record";
        //        BaseDto.QryResult = queryResult.FAILED;
        //    }
        //    return BaseDto;
        //}
        //public async Task<BaseResponseDTO<bool>> SaveCustomChecklistWithRemarks(BaseRequestDTO<SaveCustomChecklist> dt)//ScheduleId
        //{
        //    BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
        //    BaseResponseDTO<bool> Bsdt = new BaseResponseDTO<bool>();
        //    NPF_NOT_Event ev = new NPF_NOT_Event();
        //    try
        //    {
        //        List<CheckList> checklistdt = new List<CheckList>();
        //        checklistdt = JsonConvert.DeserializeObject<List<CheckList>>(dt.Data.checklist);
        //        NPF_SCH_Schedule Sched = context.NPF_SCH_Schedule.Where(x => x.Id == dt.Data.scheduleId).FirstOrDefault();
        //        NPF_SYS_GlobalParam lt = context.NPF_SYS_GlobalParam.Where(x => x.Name == "CustomChecklist").FirstOrDefault();
        //        NPF_FRM_FormPage sr = context.NPF_FRM_FormPage.Where(x => x.Id == Sched.FormPageId).FirstOrDefault();
        //        ApplicationUser app = context.Users.Where(x => x.Id.ToUpper() == dt.userToken.ToUpper()).FirstOrDefault();
        //        //  NPF_SER_WorkOrder WO = context.NPF_SER_WorkOrder.Where(x => x.ServiceRequestId == Sched.FormPageId && x.ServiceName == lt.Value).FirstOrDefault();
        //        if (dt.Data.checklist != null)
        //        {

        //            if (checklistdt.Count > 0)
        //            {
        //                foreach (CheckList ckl in checklistdt)
        //                {
        //                    NPF_FRM_WOElementMapping woel = context.NPF_FRM_WOElementMapping.Where(x => x.Id == ckl.id).FirstOrDefault();
        //                    if (woel != null)
        //                    {
        //                        if (woel.ElementType == "Checkbox with Text" || woel.ElementType == "Checkbox")
        //                        {
        //                            woel.Value = ckl.value.ToString().ToLower();
        //                            woel.AdditionalValue = ckl.remarks == null ? "" : ckl.remarks;
        //                        }
        //                        if (woel.ElementType == "Text")
        //                        {
        //                            woel.Value = ckl.remarks.ToString();
        //                        }
        //                        context.NPF_FRM_WOElementMapping.Update(woel);
        //                        context.SaveChanges();
        //                        woel = new NPF_FRM_WOElementMapping();
        //                    }
        //                }
        //            }
        //            ev = new NPF_NOT_Event()
        //            {
        //                UserId = sr.CreatedBy.ToString(),
        //                ResourceName = app.FullName,
        //                ResourceId = dt.userToken,
        //                Title = "Checklist Update",
        //                Detail = "Service Request Checklist " + sr.Code + " was updated by user:" + app.FullName,
        //                TargetType = TargetType.Redirect,
        //                Target = "/ServiceRequest/AddEditView?ID=",
        //                TargetId = sr.Id,
        //                Priority = Priority.Low,
        //                Colour = "#2870EA",
        //                Image = "",
        //                IsNotified = false,
        //                ActionDate = DateTime.Now,
        //                IsRead = false,
        //                CreatedBy = Guid.Parse(app.Id)
        //            };


        //        }
        //        Bsdt = await Event.SaveEvent(ev);
        //        BaseDto.Data = true;
        //        BaseDto.QryResult = new QueryResult().SUCEEDED;
        //    }
        //    catch (Exception ex)
        //    {
        //        BaseDto.Data = false;
        //        BaseDto.AdditionalData = "Error Saving Record";
        //        BaseDto.QryResult = queryResult.FAILED;
        //    }
        //    return BaseDto;
        //}

        //#endregion
        public async Task<BaseResponseDTO<bool>> UpdateChecklistItem(BaseRequestDTO<UpdateCheckListItem> dt)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_SCH_SheduleWOChecklist woe = context.NPF_SCH_SheduleWOChecklist.Where(x => x.Id == dt.Data.id).FirstOrDefault();
                woe.CheckListValue = dt.Data.value;
                woe.AdditionalValue += dt.Data.remarks;
                woe.UpdatedBy = Guid.Parse(dt.userToken.ToString());
                woe.UpdatedDate = DateTime.Now;
                context.NPF_SCH_SheduleWOChecklist.Update(woe);
                context.SaveChanges();

                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Saving Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> UpdateChecklistItemWithRemarks(BaseRequestDTO<SaveCustomChecklist> dt)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                List<CheckList> checklistdt = new List<CheckList>();
                checklistdt = JsonConvert.DeserializeObject<List<CheckList>>(dt.Data.checklist);
                if (dt.Data.checklist != null)
                {

                    if (checklistdt.Count > 0)
                    {
                        foreach (CheckList ckl in checklistdt)
                        {
                            NPF_SCH_SheduleWOChecklist woe = context.NPF_SCH_SheduleWOChecklist.Where(x => x.Id == ckl.id).FirstOrDefault();
                            if (woe != null)
                            {

                                if (woe.ElementType == "Checkbox with Text" || woe.ElementType == "Checkbox")
                                {
                                    woe.CheckListValue = Convert.ToBoolean(ckl.value.ToString().ToLower());
                                    woe.AdditionalValue = ckl.remarks == null ? "" : ckl.remarks;
                                }
                                if (woe.ElementType == "Text")
                                {
                                    woe.Value = ckl.remarks.ToString();
                                }
                                context.NPF_SCH_SheduleWOChecklist.Update(woe);
                                context.SaveChanges();
                            }
                            woe = new NPF_SCH_SheduleWOChecklist();
                        }
                    }
                }



                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Saving Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        //#endregion
    }

}
