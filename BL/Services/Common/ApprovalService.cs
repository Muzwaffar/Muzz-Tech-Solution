using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using BL.Models.Modules.Common;
using BL.Models.Modules.Research;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Administration;
using DAL.Models.FormPage;
using DAL.Models.Notifications;
using DAL.Models.Research;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class ApprovalService
    {
        private NPFContext context;//Declare Context
        private QueryResult queryResult;//Declare QueryResult
        private MatrixService Mservice;//Decalre MatrixService
        private EventService Event;
        public UserMatrixDTO UM = new UserMatrixDTO();//Initialise UserMatrixDTO
        public ApprovalService() //Initialised Constructer
        {
            context = new NPFContext();// Initialization of context
            queryResult = new QueryResult(); ;//Constant of Query result
            Mservice = new MatrixService();//Service Initialization of matrix access
            Event = new EventService();
            UM = Mservice.sGetMatrixUserByUserID();//Service All user's matrix access into UM
        }
        public async Task<BaseResponseDTO<bool>> SendForApproval(ApprovalServiceDTO dt)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseDto.Data = false;
            try
            {
                BaseDto.Data = await AwaitApproval(dt.Id, dt.RSM, ApprovalStatus.AwaitingApproval, dt.Approvals);
                if (BaseDto.Data)
                {
                    SendNotification(dt);
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }

            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Sending For Approval";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<ApprovalDTO>> GetAssignToMe(string userToken)
        {
            BaseResponseDTO<ApprovalDTO> BaseDto = new BaseResponseDTO<ApprovalDTO>();
            BaseDto.Data = new ApprovalDTO();
            BaseDto.Data.Approvals = new List<ApprovalUserDTO>();
            try
            {
                ApplicationUser user = context.Users.Where(x => x.UserToken.ToString() == userToken).FirstOrDefault();
                if (user != null)
                {
                    ApprovalUserDTO userAssignToMe = new ApprovalUserDTO()
                    {
                        TId = 1,
                        Username = user.FullName,
                        UId = context.NPF_SYS_User.Where(x => x.AId == user.Id).FirstOrDefault().Id.ToString()
                    };
                    BaseDto.Data.Approvals.Add(userAssignToMe);
                }

            }
            catch (Exception ex)
            {
                BaseDto.AdditionalData = "Error Retrieving Assign To Me";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<List<DropDown>>> GetDepartment()
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                List<MatrixDTO> GML = await Mservice.GetMatrixName();
                Dd.title = "Department";
                Dd.items = new List<DropDownItem>();
                foreach (MatrixDTO GM in GML)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = GM.GMID;
                    Ddi.text = GM.GMDescription;
                    Dd.items.Add(Ddi);
                    DropDownItem x = await Mservice.sGetNextParentIds(GM.GMID);
                    if (x != null)
                    {
                        if (!Dd.items.Any(y => y.Id == x.Id))
                        {
                            Dd.items.Add(x);
                        }
                    }
                }
                Ddl.Add(Dd);
                Dd = new DropDown();
                List<NPF_SYS_ResourceType> RTL = context.NPF_SYS_ResourceType.Where(x => x.Name == "Officer" && x.DeleteStatus == false).ToList();
                Dd.title = "ResourceType";
                Dd.items = new List<DropDownItem>();
                foreach (var rt in RTL)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = rt.Id;
                    Ddi.text = rt.Name;
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
        public async Task<BaseResponseDTO<List<DropDown>>> GetUsers(ApprovalServiceDropdownDTO dt)//DepartmentId,ResourceTypeId
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                List<MatrixListDTO> ML = Mservice.GetMatrixChild(dt.DepartmentId);
                List<long> MIds = ML.Select(y => y.GMID).ToList();
                MIds.Add(dt.DepartmentId);
                var UL = (from a in context.NPF_SYS_GroupMatrixUser
                          join b in context.NPF_SYS_User on a.IID equals b.Id
                          join c in context.Users on b.AId equals c.Id
                          join d in context.NPF_SYS_UserDetails on b.Id equals d.User.Id
                          where MIds.Contains(a.GMID) && d.Type == dt.ResourceTypeId && c.DeleteStatus == false
                          select new { Aid = c.Id, Uid = b.Id, c.FullName })
                          .GroupBy(elem => elem.Aid)
                          .Select(group => group.First())
                          .ToList();
                Dd.title = "Users";
                Dd.items = new List<DropDownItem>();
                foreach (var u in UL)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = u.Uid;
                    Ddi.text = u.FullName;
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
        public async Task<bool> AwaitApproval(long Id, ResearchSubModule RSM, ApprovalStatus AS, List<ApprovalServiceUserDTO> Approvals)
        {
            bool status = false;
            try
            {
                NPF_FRM_FormPage frm = context.NPF_FRM_FormPage.Where(x => x.Id == Id).FirstOrDefault();
                if (frm != null)
                {
                    frm.Approval = AS;
                    context.NPF_FRM_FormPage.Update(frm);
                    context.SaveChanges();
                    status = true;
                }
                if (AS == ApprovalStatus.AwaitingApproval)
                {
                    foreach (ApprovalServiceUserDTO d in Approvals)
                    {
                        NPF_RES_ApprovalUsers Au = new NPF_RES_ApprovalUsers()
                        {
                            ResearchSubModule = RSM,
                            UserId = context.NPF_SYS_User.Where(x => x.Id == long.Parse(d.UId)).FirstOrDefault().Id,
                            RowId = Id,

                        };
                        context.NPF_RES_ApprovalUsers.Add(Au);
                        context.SaveChanges();
                    }
                }
                if (AS == ApprovalStatus.Approved || AS == ApprovalStatus.Declined)
                {
                    NPF_NOT_Event notif = context.NPF_NOT_Event.Where(x => x.TargetId == Id && x.Entity == RSM.ToString() && x.DeleteStatus == false).FirstOrDefault();
                    if (notif != null)
                    {
                        notif.DeleteStatus = true;
                        context.NPF_NOT_Event.Update(notif);
                        context.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return status;
        }
        public async Task<BaseResponseDTO<bool>> ApproveDecline(ApprovalServiceDTO dt)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseDto.Data = false;
            try
            {
                BaseDto.Data = await AwaitApproval(dt.Id, dt.RSM, dt.ApprovalStatus, dt.Approvals);
                if (BaseDto.Data)
                {
                    SendAppNotification(dt);
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }

            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Sending For Approval";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async void SendNotification(ApprovalServiceDTO dt)
        {
            NPF_NOT_Event ev = new NPF_NOT_Event();
            BaseResponseDTO<bool> Bsdt = new BaseResponseDTO<bool>();
            try
            {
                string Name = context.Users.Where(x => x.Id == UM.AUID).FirstOrDefault().FullName;
               
                foreach (ApprovalServiceUserDTO user in dt.Approvals)
                {
                    ev = new NPF_NOT_Event()
                    {
                        UserId = user.UId,
                        ResourceName = Name,
                        ResourceId = UM.AUID,
                        Title = "Approval Pending for " + dt.Form,
                        Detail = Name + " has sent a " + dt.Form + " for approval",
                        AddDetail = "",
                        TargetType = TargetType.Redirect,
                        Target = dt.Url,
                        TargetId = dt.Id,
                        Entity = dt.RSM.ToString(),
                        Priority = Priority.Low,
                        Colour = "#2870EA",
                        Image = "",
                        IsNotified = false,
                        ActionDate = DateTime.Now,
                        IsRead = false,
                    };
                    Bsdt = await Event.SaveEvent(ev);


                }
            }
            catch (Exception ex)
            {

            }
        }
        public async void SendAppNotification(ApprovalServiceDTO dt)
        {
            NPF_NOT_Event ev = new NPF_NOT_Event();
            BaseResponseDTO<bool> Bsdt = new BaseResponseDTO<bool>();
            try
            {
                string Name = context.Users.Where(x => x.Id == UM.AUID).FirstOrDefault().FullName;
                string txt = string.Empty;
                string CreatorId = string.Empty;
                string createdBy = context.NPF_FRM_FormPage.Where(x => x.Id == dt.Id).FirstOrDefault().CreatedBy.ToString();
                CreatorId = (from a in context.Users
                             join b in context.NPF_SYS_User on a.Id equals b.AId
                             where a.Id == createdBy select b.Id.ToString()).FirstOrDefault();

                ev = new NPF_NOT_Event()
                {
                    UserId = CreatorId,
                    ResourceName = Name,
                    ResourceId = UM.AUID,
                    Title = dt.Form + " : " + dt.ApprovalStatus.ToString(),
                    Detail = Name + " has " + dt.ApprovalStatus + " a " + dt.Form,
                    AddDetail = "",
                    TargetType = TargetType.Redirect,
                    Target = dt.Url,
                    TargetId = dt.Id,
                    Entity = dt.RSM.ToString(),
                    Priority = Priority.Low,
                    Colour = "#2870EA",
                    Image = "",
                    IsNotified = false,
                    ActionDate = DateTime.Now,
                    IsRead = false,
                };

                Bsdt = await Event.SaveEvent(ev);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
