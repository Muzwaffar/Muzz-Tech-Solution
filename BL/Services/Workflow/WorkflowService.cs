using BL.Constants;
using BL.Models.Access;
using BL.Models.Activities;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Models.Dashboard;
using BL.Models.FormPage;
using BL.Models.Modules.Research;
using BL.Models.Workflow;
using BL.Services.Common;
using BL.Services.ServiceRequest;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Activities;
using DAL.Models.Administration;
using DAL.Models.Common;
using DAL.Models.Form;
using DAL.Models.FormPage;
using DAL.Models.Notifications;
using DAL.Models.WorkFlow;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Activities
{
    public class WorkflowService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private NotificationService service;
        private MatrixService Mservice;
        public FormPageService Fservice;
        public ActivitiesService ACservice;
        public UserMatrixDTO UM = new UserMatrixDTO();

        public WorkflowService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Fservice = new FormPageService();
            ACservice = new ActivitiesService();
            Mservice = new MatrixService();
            UM = Mservice.sGetMatrixUserByUserID();
        }
        #region FormpageWorkflow
        public async Task<BaseResponseDTO<bool>> InitialiseProcessWorkflow(BaseRequestDTO<FormPageDTO> dt)
        {
            BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
            string errorMsg = "No Data Found";
            try
            {
                BaseResponseDTO = await Fservice.SaveAsync(dt);
                if (BaseResponseDTO.QryResult != queryResult.SUCEEDED)
                {
                    return BaseResponseDTO;
                }
                long EntityId = Convert.ToInt64(BaseResponseDTO.AdditionalData);
                NPF_SYS_WorkFlowHeader WFH = context.NPF_SYS_WorkFlowHeader.Where(x => x.ModuleId == dt.ModuleId && x.DeleteStatus == false).FirstOrDefault();
                if (WFH != null)
                {
                    List<NPF_SYS_WorkFlowDetails> WFDL = context.NPF_SYS_WorkFlowDetails.Where(x => x.FkWFHId == WFH.Id && x.DeleteStatus == false).OrderBy(y => y.Sequence).ToList();
                    if (WFDL.Count > 0)
                    {
                        NPF_SYS_WorkFlowProcessHeader WFPH = new NPF_SYS_WorkFlowProcessHeader()
                        {
                            Code = WFH.Code,
                            Name = WFH.Name,
                            Description = WFH.Description,
                            Ext1 = WFH.Ext1,
                            Ext2 = WFH.Ext2,
                            Remarks = WFH.Remarks,
                            CStatus = WFH.CStatus,
                            WFType = WFH.WFType,
                            Sequence = WFH.Sequence,
                            ModuleId = WFH.ModuleId,
                            EntityId = EntityId,
                            AttributeType = WFH.AttributeType,//////////
                            AttributeId = string.Empty,
                            ParentId = WFH.ParentId,
                            BIID = WFH.BIID,
                            InterfaceId = WFH.InterfaceId,
                            Print = WFH.Print,
                            Notification = WFH.Notification

                        };
                        context.NPF_SYS_WorkFlowProcessHeader.Add(WFPH);
                        context.SaveChanges();
                        foreach (NPF_SYS_WorkFlowDetails WFD in WFDL)
                        {
                            NPF_SYS_WorkFlowProcessDetails WFPD = new NPF_SYS_WorkFlowProcessDetails()
                            {
                                FkWFHId = WFPH.Id,
                                Code = WFD.Code,
                                Name = WFD.Name,
                                Description = WFD.Description,
                                Message = WFD.Message,
                                Ext1 = WFD.Ext1,
                                Ext2 = WFD.Ext2,
                                Remarks = WFD.Remarks,
                                CStatus = CaseStatus.Draft,
                                GeneralStatus = GeneralStatus.Open,
                                AType = WFD.AType,
                                MatrixId = WFD.MatrixId,
                                WFType = WFD.WFType,
                                Sequence = WFD.Sequence,
                                FormSequence = WFD.FormSequence,
                                ModuleId = WFD.ModuleId,
                                ActivityType = WFD.ActivityType,
                                EntityId = EntityId,
                                AttributeType = AttributeType.none,//////////
                                AttributeId = string.Empty,
                                ParentId = WFD.ParentId,
                                BIID = WFD.BIID,
                                InterfaceId = WFD.InterfaceId,
                                Print = WFD.Print,
                                Notification = WFD.Notification
                            };
                            context.NPF_SYS_WorkFlowProcessDetails.Add(WFPD);
                            context.SaveChanges();

                        }
                        NPF_SYS_WorkFlowProcessDetails dtl = context.NPF_SYS_WorkFlowProcessDetails.Where(x => x.FkWFHId == WFPH.Id).OrderBy(y => y.Sequence).FirstOrDefault();
                        NPF_FRM_FormPage frmp = context.NPF_FRM_FormPage.Where(x => x.Id == EntityId).FirstOrDefault();
                        frmp.CurrentSequence = dtl.FormSequence;
                        switch (dtl.AType)
                        {
                            case AType.Approval:
                                BaseResponseDTO.AType = AType.Approval;
                                frmp.Approval = ApprovalStatus.Draft;
                                break;
                            case AType.Activity:
                                BaseResponseDTO.AType = AType.Activity;
                                frmp.Approval = ApprovalStatus.None;
                                break;
                            case AType.Intergration:
                                BaseResponseDTO.AType = AType.Intergration;
                                frmp.Approval = ApprovalStatus.None;
                                break;
                            default:
                                BaseResponseDTO.AType = AType.None;
                                frmp.Approval = ApprovalStatus.Saved;
                                break;
                        }
                        if (frmp.Status == Status.Draft)
                        {
                            BaseResponseDTO.AType = AType.None;
                        }
                        context.NPF_FRM_FormPage.Update(frmp);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                BaseResponseDTO.Data = false;
                BaseResponseDTO.AdditionalData = errorMsg;
                BaseResponseDTO.QryResult = queryResult.FAILED;
            }

            return BaseResponseDTO;

        }
        public async Task<BaseResponseDTO<bool>> UpdateProcessWorkflow(BaseRequestDTO<FormPageDTO> dt)
        {
            BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
            string errorMsg = "No Data Found";
            try
            {

                BaseResponseDTO = await Fservice.UpdateAsync(dt);
                ApprovalStatus AS = ApprovalStatus.None;
                if (BaseResponseDTO.QryResult != queryResult.SUCEEDED)
                {
                    return BaseResponseDTO;
                }
                if (dt.Data.Status == Status.Draft)
                {
                    BaseResponseDTO.AType = AType.None;
                    return BaseResponseDTO;
                }
                long EntityId = Convert.ToInt64(BaseResponseDTO.AdditionalData);
                NPF_SYS_WorkFlowProcessHeader WFH = context.NPF_SYS_WorkFlowProcessHeader.Where(x => x.AttributeType == AttributeType.FRMP && x.EntityId == EntityId && x.DeleteStatus == false).FirstOrDefault();
                if (WFH != null)
                {
                    NPF_SYS_WorkFlowProcessDetails dtl = context.NPF_SYS_WorkFlowProcessDetails.Where(x => x.FkWFHId == WFH.Id && x.EntityId == EntityId && x.GeneralStatus != GeneralStatus.Completed).OrderBy(y => y.Sequence).FirstOrDefault();
                    if (dtl != null)
                    {
                        BaseResponseDTO.AType = AType.None;
   
                        BaseResponseDTO.GeneralStatus = dtl.GeneralStatus;
                        if (dtl.GeneralStatus != GeneralStatus.Closed)
                        {
                            switch (dtl.AType)
                            {
                                case AType.Approval:
                                    BaseResponseDTO.AType = AType.Approval;
                                    break;
                                case AType.Activity:
                                    BaseResponseDTO.AType = AType.Activity;
                                    break;
                                case AType.Intergration:
                                    BaseResponseDTO.AType = AType.Intergration;
                                    break;
                                default:
                                    BaseResponseDTO.AType = AType.None;
                                    break;
                            }
                        }

                    }
                }
                NPF_FRM_FormPage frmp = context.NPF_FRM_FormPage.Where(x => x.Id == EntityId).FirstOrDefault();
                if (frmp.Approval == ApprovalStatus.Draft)
                {
                    frmp.Approval = AS;
                }
                context.NPF_FRM_FormPage.Update(frmp);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                BaseResponseDTO.Data = false;
                BaseResponseDTO.AdditionalData = errorMsg;
                BaseResponseDTO.QryResult = queryResult.FAILED;
            }

            return BaseResponseDTO;
        }
        public async Task<BaseResponseDTO<bool>> GetWorkflowSeq(BaseRequestDTO<Guid> dt)
        {
            BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
            string errorMsg = "No Data Found";
            try
            {
                bool result = true;
                NPF_SYS_WorkFlowDetails WFH = context.NPF_SYS_WorkFlowDetails.Where(x => x.ModuleId == dt.ModuleId && x.DeleteStatus == false).FirstOrDefault();
                if (WFH != null)
                {
                    if (WFH.Sequence == 1)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                BaseResponseDTO.Data = result;
                BaseResponseDTO.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseResponseDTO.Data = true;
                BaseResponseDTO.AdditionalData = errorMsg;
                BaseResponseDTO.QryResult = queryResult.FAILED;
            }

            return BaseResponseDTO;

        }
        public async Task<BaseResponseDTO<bool>> WorkflowSave(BaseRequestDTO<WorkFlowDTO> dt, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
            string errorMsg = "No Data Found";
            try
            {
                bool result = false;
                long Seq = 0;
                NPF_SYS_WorkFlowProcessHeader WFH = context.NPF_SYS_WorkFlowProcessHeader.Where(x => x.AttributeType == AttributeType.FRMP && x.EntityId == dt.Data.Id && x.DeleteStatus == false).FirstOrDefault();
                if (WFH != null)
                {
                    NPF_SYS_WorkFlowProcessDetails dtl = context.NPF_SYS_WorkFlowProcessDetails.Where(x => x.FkWFHId == WFH.Id && x.EntityId == dt.Data.Id && x.ModuleId == dt.ModuleId && x.GeneralStatus != GeneralStatus.Completed).OrderBy(y => y.Sequence).FirstOrDefault();
                    if (dtl != null)
                    {
                        NPF_FRM_FormPage frmp = context.NPF_FRM_FormPage.Where(x => x.Id == dt.Data.Id).FirstOrDefault();
                        //BaseResponseDTO.GeneralStatus = dtl.GeneralStatus;
                        BaseResponseDTO.AType = AType.None;
                        CaseStatus CS = CaseStatus.Draft;
                        List<WFUserDTO> UsrList = new List<WFUserDTO>();
                        if (dtl.GeneralStatus != GeneralStatus.Closed)
                        {
                            switch (dtl.AType)
                            {
                                case AType.Approval:
                                    BaseResponseDTO.AType = AType.Approval;

                                    switch (dt.Data.CaseStatus)
                                    {
                                        case CaseStatus.AwaitingApproval:

                                            CS = CaseStatus.AwaitingApproval;
                                            frmp.Approval = ApprovalStatus.AwaitingApproval;
                                            context.NPF_FRM_FormPage.Update(frmp);
                                            context.SaveChanges();
                                            result = await InitAssignee(dt.Data.WFUserDTO, dt.Data.Id, dtl.Id, CS, dtl.AType, dt.Data.ApprovalRemarks, dtl.Message, Guid.Parse(dt.ModuleId.ToString()), dt.Data.Url, userManager);
                                            /// Notify Users
                                            dtl.GeneralStatus = GeneralStatus.Inprogress;
                                            context.NPF_SYS_WorkFlowProcessDetails.Update(dtl);
                                            context.SaveChanges();
                                            break;
                                        case CaseStatus.Approved:
                                            CS = CaseStatus.Approved;
                                            UsrList = (from a in context.NPF_SYS_WorkFlowProcessAssignmentDetails
                                                       where a.FkWFPDetails == dtl.Id && a.DeleteStatus == false
                                                       select new WFUserDTO
                                                       {
                                                           TId = a.Id,
                                                           UId = a.UserId.ToString(),
                                                       }).ToList();
                                            if (dtl.Ext1 != null && dtl.Ext2 != null)
                                            {
                                                try
                                                {
                                                    string URL = dt.Data.Url;
                                                    string beforebarkurl = URL.Split(new[] { "Back=/" }, StringSplitOptions.None)[0] + "Back=/";
                                                    string Afterbarkurl = "/List" + URL.Split(new[] { "/List" }, StringSplitOptions.None)[1];
                                                    string removemid = Afterbarkurl.Split(new[] { "&MId" }, StringSplitOptions.None)[0] + "&MId";
                                                    dt.Data.Url = beforebarkurl + dtl.Ext1 + removemid + dtl.Ext2;
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                            }
                                            result = await InitAssignee(UsrList, dt.Data.Id, dtl.Id, CS, dtl.AType, dt.Data.ApprovalRemarks, dtl.Message, Guid.Parse(dt.ModuleId.ToString()), dt.Data.Url, userManager, false);
                                            /// Notify Users
                                            frmp.Approval = ApprovalStatus.Approved;
                                            context.NPF_FRM_FormPage.Update(frmp);
                                            context.SaveChanges();
                                            dtl.GeneralStatus = GeneralStatus.Completed;
                                            context.NPF_SYS_WorkFlowProcessDetails.Update(dtl);
                                            context.SaveChanges();
                                            dtl = context.NPF_SYS_WorkFlowProcessDetails.Where(x => x.FkWFHId == WFH.Id && x.EntityId == dt.Data.Id && x.Sequence > dtl.Sequence).OrderBy(y => y.Sequence).FirstOrDefault();
                                            if (dtl != null)
                                            {
                                                if (dtl.FormSequence > 0)
                                                {
                                                    if (frmp.CurrentSequence != null)
                                                    {
                                                        Seq = dtl.FormSequence > frmp.CurrentSequence ? dtl.FormSequence : Convert.ToInt64(frmp.CurrentSequence);
                                                    }
                                                    frmp.CurrentSequence = Seq;
                                                    context.NPF_FRM_FormPage.Update(frmp);
                                                    context.SaveChanges();
                                                }
                                            }
                                            else
                                            {
                                                WFH.CStatus = CaseStatus.Completed;
                                                context.NPF_SYS_WorkFlowProcessHeader.Update(WFH);
                                                context.SaveChanges();
                                            };
                                            break;
                                        case CaseStatus.Declined:
                                            CS = CaseStatus.Declined;
                                            UsrList = (from a in context.NPF_SYS_WorkFlowProcessAssignmentDetails
                                                       where a.FkWFPDetails == dtl.Id && a.DeleteStatus == false
                                                       select new WFUserDTO
                                                       {
                                                           TId = a.Id,
                                                           UId = a.UserId.ToString(),
                                                       }).ToList();
                                            result = await InitAssignee(UsrList, dt.Data.Id, dtl.Id, CS, dtl.AType, dt.Data.ApprovalRemarks, dtl.Message, Guid.Parse(dt.ModuleId.ToString()), dt.Data.Url, userManager, false);

                                            /// Notify Users
                                            frmp.Approval = ApprovalStatus.Draft;
                                            frmp.Status = Status.Rework;
                                            context.NPF_FRM_FormPage.Update(frmp);
                                            context.SaveChanges();
                                            dtl.GeneralStatus = GeneralStatus.Open;
                                            context.NPF_SYS_WorkFlowProcessDetails.Update(dtl);
                                            context.SaveChanges();
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case AType.Activity:
                                    BaseResponseDTO.AType = AType.Activity;

                                    switch (dt.Data.CaseStatus)
                                    {
                                        case CaseStatus.Draft:
                                            CS = CaseStatus.Draft;
                                            dtl.GeneralStatus = GeneralStatus.Inprogress;
                                            context.NPF_SYS_WorkFlowProcessDetails.Update(dtl);
                                            context.SaveChanges();
                                            break;
                                        case CaseStatus.Submitted:
                                            /// Notify Users
                                            CS = CaseStatus.Submitted;
                                            if (dtl.Ext1 != null && dtl.Ext2 != null)
                                            {
                                                try
                                                {
                                                    string URL = dt.Data.Url;
                                                    string beforebarkurl = URL.Split(new[] { "Back=/" }, StringSplitOptions.None)[0] + "Back=/";
                                                    string Afterbarkurl = "/List" + URL.Split(new[] { "/List" }, StringSplitOptions.None)[1];
                                                    string removemid = Afterbarkurl.Split(new[] { "&MId=" }, StringSplitOptions.None)[0] + "&MId=";
                                                    dt.Data.Url = beforebarkurl + dtl.Ext1 + removemid + dtl.Ext2;
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                            }
                                            result = await InitAssignee(dt.Data.WFUserDTO, dt.Data.Id, dtl.Id, CS, dtl.AType, dt.Data.ApprovalRemarks, dtl.Message, Guid.Parse(dt.ModuleId.ToString()), dt.Data.Url, userManager);
                                            dtl.GeneralStatus = GeneralStatus.Completed;
                                            context.NPF_SYS_WorkFlowProcessDetails.Update(dtl);
                                            context.SaveChanges();
                                            dtl = context.NPF_SYS_WorkFlowProcessDetails.Where(x => x.FkWFHId == WFH.Id && x.EntityId == dt.Data.Id && x.Sequence > dtl.Sequence).OrderBy(y => y.Sequence).FirstOrDefault();
                                            if (dtl != null)
                                            {
                                                if (dtl.FormSequence > 0)
                                                {
                                                    if (frmp.CurrentSequence != null)
                                                    {
                                                        Seq = dtl.FormSequence > frmp.CurrentSequence ? dtl.FormSequence : Convert.ToInt64(frmp.CurrentSequence);
                                                    }
                                                    frmp.CurrentSequence = Seq;
                                                    context.NPF_FRM_FormPage.Update(frmp);
                                                    context.SaveChanges();
                                                }
                                            }
                                            else
                                            {
                                                WFH.CStatus = CaseStatus.Completed;
                                                context.NPF_SYS_WorkFlowProcessHeader.Update(WFH);
                                                context.SaveChanges();
                                            }
                                            break;

                                        default:
                                            break;
                                    }

                                    break;
                                case AType.Intergration:
                                    BaseResponseDTO.AType = AType.Intergration;
                                    break;
                                default:
                                    BaseResponseDTO.AType = AType.None;
                                    break;
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                BaseResponseDTO.Data = false;
                BaseResponseDTO.AdditionalData = errorMsg;
                BaseResponseDTO.QryResult = queryResult.FAILED;
            }

            return BaseResponseDTO;

        }
        public async Task<bool> InitAssignee(List<WFUserDTO> dt, long EntityId, long FKDetailsId, CaseStatus CS, AType AType, string? Remarks, string? Message, Guid ModuleId, string? Url, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, bool IsSave = true)
        {
            bool status = false;
            try
            {
                foreach (WFUserDTO d in dt)
                {
                    if (IsSave)
                    {
                        NPF_SYS_WorkFlowProcessAssignmentDetails Au = new NPF_SYS_WorkFlowProcessAssignmentDetails()
                        {
                            FkWFPDetails = FKDetailsId,
                            EntityId = EntityId,
                            CaseStatus = CS,
                            AType = AType,
                            UserId = long.Parse(d.UId),
                        };
                        context.NPF_SYS_WorkFlowProcessAssignmentDetails.Add(Au);
                        context.SaveChanges();
                        d.TId = Au.Id;
                    }
                    else
                    {
                        NPF_SYS_WorkFlowProcessAssignmentDetails pAU = context.NPF_SYS_WorkFlowProcessAssignmentDetails.Where(x => x.Id == d.TId).FirstOrDefault();
                        pAU.DeleteStatus = true;
                        pAU.CaseStatus = CaseStatus.Completed;
                        context.NPF_SYS_WorkFlowProcessAssignmentDetails.Update(pAU);
                        context.SaveChanges();
                    }
                    GeneralStatus GS = GeneralStatus.Open;
                    long ActivityType = 0;
                    switch (CS)
                    {
                        case CaseStatus.Approved:
                            GS = GeneralStatus.Completed;
                            ActivityType = context.NPF_ACT_ActivityEventType.Where(x => x.Name == "Scheme - Approval" && x.DeleteStatus == false).FirstOrDefault().Id;
                            break;
                        case CaseStatus.Declined:
                            GS = GeneralStatus.Closed;
                            ActivityType = context.NPF_ACT_ActivityEventType.Where(x => x.Name == "Scheme - Approval" && x.DeleteStatus == false).FirstOrDefault().Id;
                            break;
                        case CaseStatus.AwaitingApproval:
                            GS = GeneralStatus.Open;
                            ActivityType = context.NPF_ACT_ActivityEventType.Where(x => x.Name == "Scheme - Approval" && x.DeleteStatus == false).FirstOrDefault().Id;
                            break;
                        case CaseStatus.Submitted:
                            GS = GeneralStatus.Open;
                            ActivityType = context.NPF_ACT_ActivityEventType.Where(x => x.Name == "Scheme - Submission" && x.DeleteStatus == false).FirstOrDefault().Id;
                            break;
                        default:
                            break;
                    }
                    ActivitiesDTO ac = new ActivitiesDTO()
                    {
                        ParentId = d.TId,
                        Remarks = Remarks,
                        Msg = Message,
                        Status = GS,
                        URL = Url,
                        ActivityTypeId = ActivityType,
                        User = long.Parse(d.UId),
                    };
                    BaseRequestDTO<ActivitiesDTO> Bdto = new BaseRequestDTO<ActivitiesDTO>();
                    Bdto.Data = ac;
                    BaseResponseDTO<bool> BR = new BaseResponseDTO<bool>();
                    if (IsSave)
                    {
                        BR = await ACservice.SaveAsync(Bdto, userManager, true);
                    }
                    else
                    {
                        NPF_ACT_Activities acitvity = context.NPF_ACT_Activities.Where(x => x.ParentId == d.TId).FirstOrDefault();
                        ac = new ActivitiesDTO()
                        {
                            Id = acitvity.Id,
                            Remarks = Message + " " + Remarks,
                            Status = GS,
                            URL = Url,
                            ActivityTypeId = ActivityType,
                            User = long.Parse(d.UId),
                            ParentId = acitvity.Id,
                        };
                        Bdto.Data = ac;
                        BR = await ACservice.UpdateAsync(Bdto);
                    }

                }
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        public async Task<BaseResponseDTO<FormPageDTO>> WFGetById(long Id, Guid ModuleId,bool IsMobile = false)
        {
            BaseResponseDTO<FormPageDTO> BaseResponseDTO = new BaseResponseDTO<FormPageDTO>();
            string errorMsg = "No Data Found";
            try
            {
                if (IsMobile)
                {
                    try
                    {
                        ModuleId = Guid.Parse(context.NPF_FRM_Form.Where(x => x.Id == (context.NPF_FRM_FormPage.Where(x => x.Id == Id).FirstOrDefault().FormId)).FirstOrDefault().ModuleId.ToString());
                    }
                    catch (Exception ex)
                    {

                    }
                }
                BaseResponseDTO.AdditionalData = context.NPF_SYS_Modules.Where(x => x.Id == ModuleId).FirstOrDefault().DisplayName;
                long wfid = 0;
                NPF_SYS_WorkFlowProcessHeader WFH = context.NPF_SYS_WorkFlowProcessHeader.Where(x => x.AttributeType == AttributeType.FRMP && x.EntityId == Id && x.DeleteStatus == false).FirstOrDefault();
                if (WFH != null)
                {
                    wfid = WFH.Id;
                }
                BaseResponseDTO = await Fservice.GetById(Id, ModuleId, wfid);
                BaseResponseDTO.AType = AType.None;
                BaseResponseDTO.Data.ApprovalS = ApprovalStatus.None;
                if (WFH != null)
                {
                    NPF_SYS_WorkFlowProcessDetails dtl = context.NPF_SYS_WorkFlowProcessDetails.Where(x => x.FkWFHId == WFH.Id && x.EntityId == Id && x.ModuleId == ModuleId).OrderBy(y => y.Sequence).FirstOrDefault();
                    if (dtl != null)
                    {
                        //BaseResponseDTO.GeneralStatus = dtl.GeneralStatus;
                        NPF_SYS_WorkFlowProcessDetails dtlc = context.NPF_SYS_WorkFlowProcessDetails.Where(x => x.FkWFHId == WFH.Id && x.EntityId == Id && x.ModuleId == ModuleId && x.GeneralStatus != GeneralStatus.Closed && x.GeneralStatus != GeneralStatus.Completed).OrderBy(y => y.Sequence).FirstOrDefault();

                        BaseResponseDTO.AType = AType.None;
                        BaseResponseDTO.Data.GeneralStatus = dtl.GeneralStatus.ToString() == "0" ? GeneralStatus.Open : dtl.GeneralStatus;
                        if (dtl.GeneralStatus != GeneralStatus.Closed && dtl.GeneralStatus != GeneralStatus.Completed)
                        {
                            switch (dtl.AType)
                            {
                                case AType.Approval:

                                    BaseResponseDTO.AType = AType.Approval;
                                    break;
                                case AType.Activity:
                                    BaseResponseDTO.AType = AType.Activity;
                                    break;
                                case AType.Intergration:
                                    BaseResponseDTO.AType = AType.Intergration;
                                    break;
                                default:
                                    BaseResponseDTO.AType = AType.None;
                                    break;
                            }
                        }
                        else
                        {
                            if (dtl.GeneralStatus != GeneralStatus.Completed)
                            {
                                BaseResponseDTO.Data.Approval = ApprovalStatus.Saved;

                            }
                        }

                        if (dtlc != null)
                        {
                            if (dtl.Id != dtlc.Id)
                            {
                                if (context.NPF_SYS_WorkFlowProcessAssignmentDetails.Any(x => x.FkWFPDetails == dtl.Id && x.DeleteStatus == false && x.UserId == UM.UID))
                                {
                                    BaseResponseDTO.Data.GeneralStatus = dtlc.GeneralStatus.ToString() == "0" ? GeneralStatus.Open : dtlc.GeneralStatus;
                                    if (dtlc.GeneralStatus != GeneralStatus.Closed && dtlc.GeneralStatus != GeneralStatus.Completed)
                                    {
                                        switch (dtlc.AType)
                                        {
                                            case AType.Approval:
                                                BaseResponseDTO.AType = AType.Approval;
                                                break;
                                            case AType.Activity:
                                                BaseResponseDTO.AType = AType.Activity;
                                                break;
                                            case AType.Intergration:
                                                BaseResponseDTO.AType = AType.Intergration;
                                                break;
                                            default:
                                                BaseResponseDTO.AType = AType.None;
                                                break;
                                        }
                                    }

                                }
                            }

                        }

                    }
                }

            }
            catch (Exception e)
            {
                BaseResponseDTO.QryResult = queryResult.FAILED;
            }
            BaseResponseDTO.GeneralStatus = GeneralStatus.Open;
            BaseResponseDTO.AType = AType.None;
            return BaseResponseDTO;
        }
        #endregion

        #region GeneralWorkFlow
        public async Task<BaseResponseDTO<bool>> InitialiseGeneralProcessWorkflow(BaseRequestDTO<long> dt)
        {
            BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
            string errorMsg = "No Data Found";
            try
            {
                long EntityId = dt.Data;
                NPF_SYS_WorkFlowHeader WFH = context.NPF_SYS_WorkFlowHeader.Where(x => x.ModuleId == dt.ModuleId && x.DeleteStatus == false).FirstOrDefault();
                if (WFH != null)
                {
                    List<NPF_SYS_WorkFlowDetails> WFDL = context.NPF_SYS_WorkFlowDetails.Where(x => x.FkWFHId == WFH.Id && x.DeleteStatus == false).OrderBy(y => y.Sequence).ToList();
                    if (WFDL.Count > 0)
                    {
                        NPF_SYS_WorkFlowProcessHeader WFPH = new NPF_SYS_WorkFlowProcessHeader()
                        {
                            Code = WFH.Code,
                            Name = WFH.Name,
                            Description = WFH.Description,
                            Ext1 = WFH.Ext1,
                            Ext2 = WFH.Ext2,
                            Remarks = WFH.Remarks,
                            CStatus = WFH.CStatus,
                            WFType = WFH.WFType,
                            Sequence = WFH.Sequence,
                            ModuleId = WFH.ModuleId,
                            EntityId = EntityId,
                            AttributeType = WFH.AttributeType,//////////
                            AttributeId = string.Empty,
                            ParentId = WFH.ParentId,
                            BIID = WFH.BIID,
                            InterfaceId = WFH.InterfaceId,
                            Print = WFH.Print,
                            Notification = WFH.Notification

                        };
                        context.NPF_SYS_WorkFlowProcessHeader.Add(WFPH);
                        context.SaveChanges();
                        foreach (NPF_SYS_WorkFlowDetails WFD in WFDL)
                        {
                            NPF_SYS_WorkFlowProcessDetails WFPD = new NPF_SYS_WorkFlowProcessDetails()
                            {
                                FkWFHId = WFPH.Id,
                                Code = WFD.Code,
                                Name = WFD.Name,
                                Description = WFD.Description,
                                Message = WFD.Message,
                                Ext1 = WFD.Ext1,
                                Ext2 = WFD.Ext2,
                                Remarks = WFD.Remarks,
                                CStatus = CaseStatus.Draft,
                                GeneralStatus = GeneralStatus.Open,
                                AType = WFD.AType,
                                MatrixId = WFD.MatrixId,
                                WFType = WFD.WFType,
                                Sequence = WFD.Sequence,
                                FormSequence = WFD.FormSequence,
                                ModuleId = WFD.ModuleId,
                                ActivityType = WFD.ActivityType,
                                EntityId = EntityId,
                                AttributeType = AttributeType.none,//////////
                                AttributeId = string.Empty,
                                ParentId = WFD.ParentId,
                                BIID = WFD.BIID,
                                InterfaceId = WFD.InterfaceId,
                                Print = WFD.Print,
                                Notification = WFD.Notification
                            };
                            context.NPF_SYS_WorkFlowProcessDetails.Add(WFPD);
                            context.SaveChanges();

                        }
                        NPF_SYS_WorkFlowProcessDetails dtl = context.NPF_SYS_WorkFlowProcessDetails.Where(x => x.FkWFHId == WFPH.Id).OrderBy(y => y.Sequence).FirstOrDefault();
                        switch (dtl.AType)
                        {
                            case AType.Approval:
                                BaseResponseDTO.AType = AType.Approval;
                                break;
                            case AType.Activity:
                                BaseResponseDTO.AType = AType.Activity;
                                break;
                            case AType.Intergration:
                                BaseResponseDTO.AType = AType.Intergration;
                                break;
                            default:
                                BaseResponseDTO.AType = AType.None;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BaseResponseDTO.Data = false;
                BaseResponseDTO.AdditionalData = errorMsg;
                BaseResponseDTO.QryResult = queryResult.FAILED;
            }

            return BaseResponseDTO;

        }

        public async Task<bool> InitActivity( long EntityId, long ActivityType, GeneralStatus GS, long UId, string Url, string Remarks, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, string? Message)
        {
            bool status = false;
            try
            {
                ActivitiesDTO ac = new ActivitiesDTO()
                {
                    ParentId = EntityId,
                    Remarks = Remarks,
                    Msg = Message,
                    Status = GS,
                    URL = Url,
                    ActivityTypeId = ActivityType,
                    User = UId,
                };

                BaseRequestDTO<ActivitiesDTO> Bdto = new BaseRequestDTO<ActivitiesDTO>();
                Bdto.Data = ac;
                BaseResponseDTO<bool> BR = new BaseResponseDTO<bool>();

                BR = await ACservice.SaveAsync(Bdto, userManager, true);


            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }
        #endregion
    }
}
