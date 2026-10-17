using BL.Constants;
using BL.Models.Common;
using BL.Models.FormPage;
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
using DAL.Models.FormPage;
using DAL.Models.Notifications;
using DAL.Models.Schedule;
using DAL.Models.Transaction;
using DAL.Models.WorkFlow;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Files = BL.Models.Schedule.Files;

namespace BL.Services.Dispatch
{
    public class DispatchService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public ColourService CS;
        public SourceControlService Sservice;
        public NotificationService Nhs;
        public EventService Event;
        public FormPageService Fservice;
        public UserManager<ApplicationUser> UserManager;
        public DispatchService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Nhs = new NotificationService();
        }

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

        #region MoblieApi

        public async Task<BaseResponseDTO<List<ClientSchSerReqDTO>>> GetClientOverview(string dataUserId)
        {
            BaseResponseDTO<List<ClientSchSerReqDTO>> BaseDto = new BaseResponseDTO<List<ClientSchSerReqDTO>>();
            List<ClientSchSerReqDTO> reponse = new List<ClientSchSerReqDTO>();
            try
            {
                long Uid = context.NPF_SYS_User.Where(x => x.AId == dataUserId).FirstOrDefault().Id;
                NPF_SYS_ResourceType ResT = context.NPF_SYS_ResourceType.Where(x => x.Name == "Station").FirstOrDefault();
                bool Istation = context.NPF_SYS_UserDetails.Any(x => x.User.Id == Uid && x.Type == ResT.Id);
                if (Istation)
                {
                    List<long> StationIds = context.NPF_SYS_Resource.Where(x => x.UserId == Uid && x.DeleteStatus == false && (x.ResourceType.Name == "Station" || x.ResourceType.Name == "Station Commander")).Select(y => y.StationUnitName.Id).ToList();
                    List<long> IncidentIds = new List<long>();
                    IncidentIds.AddRange(context.NPF_TRN_IncidentStation.Where(x => StationIds.Contains(x.StationId) && x.DeleteStatus == false).Select(y => y.IncidentId).ToList());
                    IncidentIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => StationIds.Contains(x.OtherStationId) && x.DeleteStatus == false).Select(y => y.IncidentId).ToList());
                    if (IncidentIds.Count > 0)
                    {
                        List<long> SortedIncidentIds = IncidentIds.GroupBy(x => x).Select(grp => grp.First()).ToList();
                        SortedIncidentIds = SortedIncidentIds.Distinct().ToList();
                        reponse = (
                                    from Inc in context.NPF_TRN_Incident
                                    where Inc.DeleteStatus == false && SortedIncidentIds.Contains(Inc.Id)
                                    select new ClientSchSerReqDTO
                                    {
                                        scheduleId = Inc.Id,
                                        clientId = 0,
                                        clientName = Inc.IncidentNo,
                                        additional = context.NPF_SYS_IncidentCode.Where(x => x.Id == Inc.IncidentCode).FirstOrDefault().Name,
                                        scheduleName = context.NPF_SYS_NatureOfIncident.Where(x => x.Id == (context.NPF_TRN_IncidentStation.Where(x => x.Id == Inc.Id).FirstOrDefault().NatureOfIncidentId)).FirstOrDefault().Name,//AssignedTo
                                        serviceRequestCode = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == Inc.Id).FirstOrDefault().Address,
                                    }).ToList();
                    }
                }
                else
                {
                    List<long> RsIds = context.NPF_SYS_Resource.Where(x => x.UserId == Uid && x.DeleteStatus == false).Select(x => x.Id).ToList();
                    List<long> IncIds = context.NPF_TRN_IncidentAssignment.Where(x => RsIds.Contains(x.ResourceId) && x.DeleteStatus == false).Select(y => y.IncidentId).ToList();
                    IncIds = IncIds.Distinct().ToList();

                    reponse = (
                                                        from Inc in context.NPF_TRN_Incident
                                                        join Res in context.NPF_TRN_IncidentAssignment on Inc.Id equals Res.IncidentId
                                                        where Inc.DeleteStatus == false && IncIds.Contains(Inc.Id)
                                                        select new ClientSchSerReqDTO
                                                        {
                                                            scheduleId = Inc.Id,
                                                            clientId = 0,
                                                            clientName = Inc.IncidentNo,
                                                            additional = context.NPF_SYS_IncidentCode.Where(x => x.Id == Inc.IncidentCode).FirstOrDefault().Name,
                                                            scheduleName = context.NPF_SYS_NatureOfIncident.Where(x => x.Id == (context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == Inc.Id).FirstOrDefault().NatureOfIncidentId)).FirstOrDefault().Name,//AssignedTo
                                                            serviceRequestCode = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == Inc.Id).FirstOrDefault().Address,
                                                        }).ToList();

                }

                BaseDto.Data = reponse.OrderByDescending(x => x.scheduleId).DistinctBy(y => y.scheduleId).ToList();
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
        public async Task<BaseResponseDTO<List<AllTaskMobileDTO>>> GetAllTaskMobile(DateRangeMobileDTO data)
        {
            BaseResponseDTO<List<AllTaskMobileDTO>> BaseDto = new BaseResponseDTO<List<AllTaskMobileDTO>>();
            List<AllTaskMobileDTO> reponse = new List<AllTaskMobileDTO>();
            try
            {
                long Uid = context.NPF_SYS_User.Where(x => x.AId == data.userId).FirstOrDefault().Id;
                List<long> RsIds = context.NPF_SYS_Resource.Where(x => x.UserId == Uid && x.DeleteStatus == false).Select(x => x.Id).ToList();
                List<long> IncIds = context.NPF_TRN_IncidentAssignment.Where(x => RsIds.Contains(x.ResourceId) && x.DeleteStatus == false).Select(y => y.IncidentId).ToList();
                IncIds = IncIds.Distinct().ToList();
                reponse = (from Inc in context.NPF_TRN_Incident
                           where Inc.DeleteStatus == false && IncIds.Contains(Inc.Id)
                           select new AllTaskMobileDTO
                           {
                               scheduleId = Inc.Id,
                               scheduleName = Inc.IncidentNo,
                               clientName = context.NPF_SYS_Station.Where(x => x.Id == (context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == Inc.Id).FirstOrDefault().StationId)).FirstOrDefault().Name,
                               time = Convert.ToDateTime(Inc.DateTimeAssigned).ToString("dd/MM/yyyy HH:mm"),
                               zoneName = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == Inc.Id).FirstOrDefault().Address,
                               status = Inc.IncidentStatus == IncidentStatus.Leavesite ? IncidentStatus.Completed : Inc.IncidentStatus,
                               numSchedule = 0
                           }).ToList();

                BaseDto.Data = reponse.OrderByDescending(x => x.scheduleId).DistinctBy(y => y.scheduleId).ToList();
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
        public async Task<BaseResponseDTO<GetTaskDetailMobileDTO>> GetTaskDetailMobile(GetTaskDetailMobile dto)
        {
            BaseResponseDTO<GetTaskDetailMobileDTO> BaseDto = new BaseResponseDTO<GetTaskDetailMobileDTO>();
            GetTaskDetailMobileDTO reponse = new GetTaskDetailMobileDTO();
            TaskHeader th = new TaskHeader();
            Info inf = new Info();
            List<Details> dts = new List<Details>();
            List<Equipment> eq = new List<Equipment>();
            List<Vehicle> Veq = new List<Vehicle>();
            List<Remarks> rm = new List<Remarks>();
            List<Files> fl = new List<Files>();
            Route route = new Route();
            route.viaPoint = new List<ViaPoint>();
            List<ViaPoint> vpl = new List<ViaPoint>();
            List<Helper> hps = new List<Helper>();
            List<Agencies> agcl = new List<Agencies>();
            try
            {
                #region Query
                NPF_TRN_Incident sch = context.NPF_TRN_Incident.Where(x => x.Id == dto.id && x.DeleteStatus == false ).FirstOrDefault();
                List<NPF_TRN_IncidentResource> hpl = context.NPF_TRN_IncidentResource.Where(x => x.IncidentId == sch.Id && x.DeleteStatus == false).ToList();
                NPF_TRN_IncidentLocation rml = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == sch.Id && x.DeleteStatus == false).FirstOrDefault();
                List<NPF_TRN_IncidentFiles> flsl = context.NPF_TRN_IncidentFiles.Where(x => x.IncidentId == dto.id && x.DeleteStatus == false).ToList();
                List<NPF_TRN_IncidentEquipment> eqc = context.NPF_TRN_IncidentEquipment.Where(x => x.IncidentId == sch.Id && x.DeleteStatus == false).ToList();
                List<NPF_TRN_IncidentVehicle> ivh = context.NPF_TRN_IncidentVehicle.Where(x => x.IncidentId == sch.Id && x.DeleteStatus == false).ToList();
                List<NPF_TRN_IncidentExternalAgency> ag = context.NPF_TRN_IncidentExternalAgency.Where(x => x.IncidentId == sch.Id && x.DeleteStatus == false).ToList();
                #endregion
                th.taskTitle = context.NPF_SYS_IncidentType.Where(x => x.Id == sch.IncidentType).FirstOrDefault().Name;
                NPF_SYS_IncidentCode incCode = context.NPF_SYS_IncidentCode.Where(x => x.Id == sch.IncidentCode).FirstOrDefault();
                th.code = incCode == null ? string.Empty : incCode.Name;
                th.client = sch.IncidentNo;
                th.additional = context.NPF_SYS_NatureOfIncident.Where(x => x.Id == context.NPF_TRN_IncidentStation.Where(y => y.IncidentId == sch.Id).FirstOrDefault().NatureOfIncidentId).FirstOrDefault().Name;
                th.id = sch.Id;
                th.Status = sch.IncidentStatus == IncidentStatus.Leavesite ? IncidentStatus.Completed : sch.IncidentStatus;
                th.formId = sch.Id;
                inf.startTime = Convert.ToDateTime(sch.DateTimeAssigned);
                inf.endTime = DateTime.Now;
                inf.contactPerson = string.Empty;
                inf.contactNumber = string.Empty;
                try
                {
                    NPF_SYS_Caller cll = context.NPF_SYS_Caller.Where(y => y.Id == (context.NPF_TRN_IncidentCaller.Where(x => x.IncidentId == sch.Id).FirstOrDefault().CallerId)).FirstOrDefault();
                    if (cll != null)
                    {
                        try
                        {
                            inf.contactNumber = cll.NAA.MobileNo;
                        }
                        catch (Exception ex)
                        {
                            inf.contactNumber = string.Empty;

                        }
                        try
                        {
                            inf.contactPerson = cll.NAA.FullName;
                        }
                        catch (Exception ex)
                        {
                            inf.contactPerson = string.Empty;

                        }


                    }

                }
                catch (Exception ex)
                {
                    inf.contactPerson = string.Empty;
                    inf.contactNumber = string.Empty;
                }
                if (ivh.Count > 0)
                {
                    foreach (NPF_TRN_IncidentVehicle h in ivh)
                    {
                        Vehicle hp = new Vehicle()
                        {
                            vehicle = context.NPF_SYS_Vehicle.Where(x => x.Id == h.VehicleId).FirstOrDefault().RegistrationNo
                        };
                        Veq.Add(hp);
                    }
                }
                inf.vehicle = Veq;
                if (hpl.Count > 0)
                {
                    foreach (NPF_TRN_IncidentResource h in hpl)
                    {
                        Helper hp = new Helper()
                        {
                            helper = context.NPF_SYS_Resource.Where(x => x.Id == h.ResourceId).FirstOrDefault().NAA.FullName,
                            Id = h.Id
                        };
                        hps.Add(hp);
                    }
                }
                inf.helper = hps;


                if (eqc.Count > 0)
                {
                    foreach (NPF_TRN_IncidentEquipment eql in eqc)
                    {
                        Equipment e = new Equipment()
                        {
                            title = context.NPF_SYS_Equipment.Where(x => x.Id == eql.EquipmentId).FirstOrDefault().Name,
                            equipmentName = string.Empty,
                            quantity = 0
                        };
                        eq.Add(e);
                    }
                }
                inf.equipment = eq;
                if (ag.Count > 0)
                {
                    foreach (NPF_TRN_IncidentExternalAgency eql in ag)
                    {
                        NPF_SYS_ExternalAgencies tag = context.NPF_SYS_ExternalAgencies.Where(x => x.Id == eql.ExternalAgencyId).FirstOrDefault();
                        Agencies e = new Agencies()
                        {
                            agencyName = tag.Name,
                            agencyPhone = tag.ContactNumber
                        };
                        agcl.Add(e);
                    }

                }
                inf.agencies = agcl;
                if (rml != null)
                {
                    // string[] rem = rml.LandMark.Trim().Split('\n');
                    //for (int i = 0; i < rem.Length; i++)
                    //{
                    //    Remarks r = new Remarks() { remarks = rem[i] };
                    //    rm.Add(r);
                    //}
                    Remarks r = new Remarks() { remarks = rml.LandMark };
                    rm.Add(r);
                }

                if (flsl.Count > 0)
                {
                    foreach (NPF_TRN_IncidentFiles fll in flsl)
                    {
                        Files f = new Files() { file = fll.File, fileId = fll.Id, fileName = fll.FileName };
                        fl.Add(f);
                    };
                }
                if (rml != null)
                {
                    if (rml.Latitude != null && rml.Longitude != null)
                    {
                        ViaPoint vp = new ViaPoint()
                        {
                            viaPointName = "Current Location",
                            viaPoinTime = Convert.ToDateTime(sch.DateTimeAssigned),
                            isFirst = true,
                            centerPoint = new CenterPoint()
                            {
                                lat = dto.Latitude,
                                lon = dto.Longitude
                            },
                        };
                        vpl.Add(vp);
                        vp = new ViaPoint()
                        {
                            viaPointName = rml.Address,
                            viaPoinTime = Convert.ToDateTime(sch.DateTimeAssigned),
                            isFirst = false,
                            centerPoint = new CenterPoint()
                            {
                                lat = rml.Latitude,
                                lon = rml.Longitude
                            },
                        };
                        vpl.Add(vp);
                    }
                }

                route.viaPoint.AddRange(vpl);
                reponse.taskHeader = th;
                reponse.info = inf;
                reponse.details = dts;
                reponse.equipment = eq;
                reponse.remarks = rm;
                reponse.File = fl;
                reponse.route = route;
                BaseDto.Data = reponse;
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
        public async Task<BaseResponseDTO<bool>> SaveRemarks(BaseRequestDTO<SaveFileRemarks> dataId)//ScheduleId
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_SCH_Remarks Rm = new NPF_SCH_Remarks()
                {
                    ScheduleId = dataId.Data.scheduleId,
                    Remarks = dataId.Data.str,
                    CreatedBy = Guid.Parse(dataId.userToken.ToString()),
                    CreatedDate = DateTime.Now,
                    Type = "Mobile"
                };
                context.NPF_SCH_Remarks.Add(Rm);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> SaveFile(BaseRequestDTO<SaveFileRemarks> dataId)//ScheduleId
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                long UID = context.NPF_SYS_User.Where(x => x.AId.ToLower() == dataId.userToken.ToLower()).FirstOrDefault().Id;
                NPF_TRN_IncidentFiles f = new NPF_TRN_IncidentFiles()
                {
                    IncidentId = dataId.Data.scheduleId,
                    File = dataId.Data.str,
                    FileName = dataId.Data.fileName,
                    CreatedBy = Guid.Parse(dataId.userToken.ToString()),
                    CreatedDate = DateTime.Now
                };
                context.NPF_TRN_IncidentFiles.Add(f);
                context.SaveChanges();
                NPF_TRN_Incident inc = context.NPF_TRN_Incident.Where(x => x.Id == f.IncidentId).FirstOrDefault();
                List<long> StationIds = new List<long>();
                List<long> UsersIds = new List<long>();
                List<long> MobUsersIds = new List<long>();
                List<string> stalist = new List<string>();

                Notification notif = new Notification()
                {
                    Id = inc.Id,
                    UserId = 0,
                    MType = MessageType.Txt,
                    MName = PageName.Incident,
                    TaskName = "Incident No:" + inc.IncidentNo + " New File Received.",
                    URL = "/IncidentHandling/AddEdit?Id=" + inc.Id + "&Code=" + inc.Code + "&Field=file#scrollspy-file",
                    AUserId = dataId.userToken.ToLower()

                };
                StationIds.Add(context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == inc.Id && x.DeleteStatus == false).FirstOrDefault().StationId);
                StationIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => x.IncidentId == inc.Id && x.DeleteStatus == false).Select(y => y.OtherStationId).ToList());
                //Start Changed for CRO 20241112
                StationIds.AddRange(context.NPF_SYS_Station.Where(x => x.Name == "Control Room" && x.DeleteStatus == false).Select(y => y.Id).ToList());
                //End Changed for CRO 20241112
                stalist.Add("Station Commander");
                stalist.Add("Station");
                stalist.Add("CRO");
                long Cuid = context.NPF_SYS_User.Where(x => x.AId == inc.CreatedBy.ToString()).FirstOrDefault().Id;
                UsersIds = context.NPF_SYS_Resource.Where(x => x.UserId != null && StationIds.Contains(x.StationUnitName.Id) && stalist.Contains(x.ResourceType.Name)).Select(x => Convert.ToInt64(x.UserId)).ToList();
                if (UsersIds.Any(x => x != Cuid))
                {
                    UsersIds.Add(Cuid);
                }
                if (UsersIds.Any(x => x == UID))
                {
                    UsersIds.RemoveAll(x => x == UID);
                }
                UsersIds = UsersIds.Distinct().ToList();
                foreach (long user in UsersIds)
                {
                    if (user > 0)
                    {
                        notif.UserId = user;
                        new NotificationService().SendNotification(notif);

                    }

                }
                MobUsersIds = context.NPF_TRN_IncidentAssignment.Where(x => x.IncidentId == inc.Id).Select(y => y.ResourceId).ToList();
                UsersIds = new List<long>();
                UsersIds.AddRange(context.NPF_SYS_Resource.Where(x => x.UserId != null && MobUsersIds.Contains(x.Id)).Select(x => Convert.ToInt64(x.UserId)).ToList());
                if (UsersIds.Any(x => x == UID))
                {
                    UsersIds.RemoveAll(x => x == UID);
                }
                UsersIds = UsersIds.Distinct().ToList();
                foreach (long user in UsersIds)
                {
                    if (user > 0)
                    {
                        PushNotif pn = new PushNotif()
                        {
                            Id = inc.Id,
                            Type = NotifType.Push,
                            ResId = user,
                            MType = MessageType.Txt
                        };
                        try
                        {
                            bool x = await new NotificationHandlerService().UnotNotification(pn);
                        }
                        catch (Exception ex)
                        {

                        }

                    }

                }
                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> HelperStatus(BaseRequestDTO<Resstat> dataId)//ScheduleId
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_TRN_IncidentResource h = context.NPF_TRN_IncidentResource.Where(x => x.Id == dataId.Data.Id).FirstOrDefault();
                h.ResourceStatus = dataId.Data.Status;
                h.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                h.UpdatedDate = DateTime.Now;
                context.NPF_TRN_IncidentResource.Update(h);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> DeleteFile(BaseRequestDTO<long> dataFileId)//FileId
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                long FileId = dataFileId.Data;
                NPF_TRN_IncidentFiles f = context.NPF_TRN_IncidentFiles.Where(x => x.Id == FileId).FirstOrDefault();
                f.DeleteStatus = true;
                f.UpdatedBy = Guid.Parse(dataFileId.userToken.ToString());
                f.UpdatedDate = DateTime.Now;
                context.NPF_TRN_IncidentFiles.Update(f);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> SaveZone(BaseRequestDTO<ScheZone> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();

            try
            {
                NPF_TRN_Incident In = context.NPF_TRN_Incident.Where(x => x.Id == dataToSave.Data.ScheduleId && x.DeleteStatus == false).FirstOrDefault();
                In.IncidentStatus = IncidentStatus.Onsite;
                In.UpdatedBy = Guid.Parse(dataToSave.userToken.ToString());
                In.UpdatedDate = DateTime.Now;
                In.DateTimeTurnOut = DateTime.Now;
                context.NPF_TRN_Incident.Update(In);
                context.SaveChanges();
                NPF_TRN_IncidentLocation Inc = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == dataToSave.Data.ScheduleId).FirstOrDefault();
                Inc.GeomDataText = "{'type': 'Feature','properties': {'radius': 20},'geometry': {'type': 'Point','coordinates': [" + dataToSave.Data.CenterPointLat + "," + dataToSave.Data.CenterPointLat + "]}}";
                Inc.Latitude = dataToSave.Data.CenterPointLat;
                Inc.Longitude = dataToSave.Data.CenterPointLong;
                Inc.Type = dataToSave.Data.Type;
                Inc.UpdatedBy = Guid.Parse(dataToSave.userToken.ToString());
                Inc.UpdatedDate = DateTime.Now;
                context.NPF_TRN_IncidentLocation.Update(Inc);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Updating Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> StatusUpdateTaskMobile(BaseRequestDTO<IncidentStatusChange> dataId)//ScheduleId
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            bool istrue = true;
            try
            {
                NPF_TRN_Incident Inc = context.NPF_TRN_Incident.Where(x => x.Id == dataId.Data.Id && x.DeleteStatus == false).FirstOrDefault();
                switch (dataId.Data.Status)
                {
                    case IncidentStatus.Accepted:

                        Inc.IncidentStatus = IncidentStatus.Accepted;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeAccepted = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        break;
                    case IncidentStatus.TurnOut:
                        Inc.IncidentStatus = IncidentStatus.TurnOut;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeTurnOut = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        break;
                    case IncidentStatus.Onsite:
                        Inc.IncidentStatus = IncidentStatus.Onsite;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeTurnOut = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        try
                        {
                            NPF_TRN_IncidentLocation IncL = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == Inc.Id).FirstOrDefault();
                            IncL.GeomDataText = "{'type': 'Feature','properties': {'radius': 20},'geometry': {'type': 'Point','coordinates': [" + dataId.Data.CenterPointLat + "," + dataId.Data.CenterPointLat + "]}}";
                            IncL.Latitude = dataId.Data.CenterPointLat;
                            IncL.Longitude = dataId.Data.CenterPointLong;
                            IncL.Type = dataId.Data.Type;
                            IncL.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                            IncL.UpdatedDate = DateTime.Now;
                            context.NPF_TRN_IncidentLocation.Update(IncL);
                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                        }
                        break;
                    case IncidentStatus.UnderControl:
                        Inc.IncidentStatus = IncidentStatus.UnderControl;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeCompleted = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        break;
                    case IncidentStatus.Completed:
                        Inc.IncidentStatus = IncidentStatus.Completed;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeCompleted = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        break;
                    case IncidentStatus.LeftSite:
                        Inc.IncidentStatus = IncidentStatus.LeftSite;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeLeftSite = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        break;
                    case IncidentStatus.ReachedStation:
                        Inc.IncidentStatus = IncidentStatus.ReachedStation;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeReachedStation = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        break;
                    default:
                        istrue = false;
                        BaseDto.Data = false;
                        BaseDto.AdditionalData = "An Error Occured";
                        BaseDto.QryResult = new QueryResult().FAILED;
                        return BaseDto;
                }
                if (istrue)
                {
                    try
                    {
                        List<long> StationIds = new List<long>();
                        StationIds.Add(context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == Inc.Id && x.DeleteStatus == false).FirstOrDefault().StationId);
                        StationIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => x.IncidentId == Inc.Id && x.DeleteStatus == false).Select(y => y.OtherStationId).ToList());
                        //Start Changed for CRO 20241112
                        StationIds.AddRange(context.NPF_SYS_Station.Where(x => x.Name == "Control Room" && x.DeleteStatus == false).Select(y => y.Id).ToList());
                        //End Changed for CRO 20241112
                        List<string> stalist = new List<string>();
                        stalist.Add("Station Commander");
                        stalist.Add("Station");
                        stalist.Add("CRO");
                        List<long> ResIds = new List<long>();
                        var UsersIds = context.NPF_SYS_Resource.Where(x => x.UserId != null && x.UserId != 0 && StationIds.Contains(x.StationUnitName.Id) && stalist.Contains(x.ResourceType.Name)).Select(x => new { UserId = Convert.ToInt64(x.UserId), Id = x.Id }).ToList();

                        Notification notif = new Notification()
                        {
                            Id = Inc.Id,
                            UserId = 0,
                            MType = MessageType.Status,
                            MName = PageName.Incident,
                            TaskName = "Incident No:" + Inc.IncidentNo + "Status Change To " + dataId.Data.Status.ToString(),
                            URL = "/IncidentHandling/AddEdit?Id=" + Inc.Id + "&Code=" + Inc.Code + "#"

                        };
                        UsersIds = UsersIds.Distinct().ToList();
                        foreach (var user in UsersIds)
                        {
                            notif.UserId = user.UserId;
                            notif.ResId = user.Id;
                            notif.AUserId = dataId.userToken.ToString();
                            new NotificationService().SendNotification(notif);


                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
                return BaseDto;

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> StatusUpdateTaskDetailMobile(BaseRequestDTO<IncidentStatusChange> dataId)//NPF_TRN_IncidentServiceMapping ID
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            bool istrue = true;
            try
            {
                NPF_TRN_Incident Inc = context.NPF_TRN_Incident.Where(x => x.Id == dataId.Data.Id && x.DeleteStatus == false).FirstOrDefault();

                switch (dataId.Data.Status)
                {
                    case IncidentStatus.Accepted:

                        Inc.IncidentStatus = IncidentStatus.Accepted;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeAccepted = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        break;
                    case IncidentStatus.TurnOut:
                        Inc.IncidentStatus = IncidentStatus.TurnOut;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeTurnOut = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        break;
                    case IncidentStatus.Onsite:
                        Inc.IncidentStatus = IncidentStatus.Onsite;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeOnsite = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        try
                        {
                            NPF_TRN_IncidentLocation IncL = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == Inc.Id).FirstOrDefault();
                            IncL.GeomDataText = "{'type': 'Feature','properties': {'radius': 20},'geometry': {'type': '" + dataId.Data.Type + "','coordinates': [" + dataId.Data.CenterPointLat + "," + dataId.Data.CenterPointLat + "]}}";
                            IncL.Latitude = dataId.Data.CenterPointLat;
                            IncL.Longitude = dataId.Data.CenterPointLong;
                            IncL.Type = dataId.Data.Type;
                            IncL.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                            IncL.UpdatedDate = DateTime.Now;
                            context.NPF_TRN_IncidentLocation.Update(IncL);
                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                        }
                        break;
                    case IncidentStatus.UnderControl:
                        Inc.IncidentStatus = IncidentStatus.UnderControl;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeUnderControl = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        break;
                    case IncidentStatus.Completed:
                        Inc.IncidentStatus = IncidentStatus.Completed;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeCompleted = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        break;
                    case IncidentStatus.LeftSite:
                        Inc.IncidentStatus = IncidentStatus.LeftSite;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeLeftSite = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        break;
                    case IncidentStatus.ReachedStation:
                        Inc.IncidentStatus = IncidentStatus.ReachedStation;
                        Inc.UpdatedBy = Guid.Parse(dataId.userToken.ToString());
                        Inc.UpdatedDate = DateTime.Now;
                        Inc.DateTimeReachedStation = DateTime.Now;
                        context.NPF_TRN_Incident.Update(Inc);
                        context.SaveChanges();
                        break;
                    default:
                        istrue = false;
                        BaseDto.Data = false;
                        BaseDto.AdditionalData = "An Error Occured";
                        BaseDto.QryResult = new QueryResult().FAILED;
                        return BaseDto;
                }
                if (istrue)
                {
                    try
                    {
                        List<long> StationIds = new List<long>();
                        StationIds.Add(context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == Inc.Id && x.DeleteStatus == false).FirstOrDefault().StationId);
                        StationIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => x.IncidentId == Inc.Id && x.DeleteStatus == false).Select(y => y.OtherStationId).ToList());
                        //Start Changed for CRO 20241112
                        StationIds.AddRange(context.NPF_SYS_Station.Where(x => x.Name == "Control Room" && x.DeleteStatus == false).Select(y => y.Id).ToList());
                        //End Changed for CRO 20241112
                        List<string> stalist = new List<string>();
                        stalist.Add("Station Commander");
                        stalist.Add("Station");
                        stalist.Add("CRO");
                        List<long> ResIds = new List<long>();
                        var UsersIds = context.NPF_SYS_Resource.Where(x => x.UserId != null && x.UserId != 0 && StationIds.Contains(x.StationUnitName.Id) && stalist.Contains(x.ResourceType.Name)).Select(x => new { UserId = Convert.ToInt64(x.UserId), Id = x.Id }).ToList();

                        Notification notif = new Notification()
                        {
                            Id = Inc.Id,
                            UserId = 0,
                            MType = MessageType.Status,
                            MName = PageName.Incident,
                            TaskName = "Incident No:" + Inc.IncidentNo + "Status Change To " + dataId.Data.Status.ToString(),
                            URL = "/IncidentHandling/AddEdit?Id=" + Inc.Id + "&Code=" + Inc.Code + "#"

                        };
                        UsersIds = UsersIds.Distinct().ToList();
                        foreach (var user in UsersIds)
                        {
                            notif.UserId = user.UserId;
                            notif.ResId = user.Id;
                            notif.AUserId = dataId.userToken.ToString();
                            new NotificationService().SendNotification(notif);


                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
                return BaseDto;

            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }
        #endregion

    }
}
