using BL.Constants;
using BL.Models.Common;
using BL.Models.FormPage;
using BL.Services.Administration;
using BL.Services.Common;
using BL.Services.Source;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Form;
using DAL.Models.FormPage;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Drawing;
using BL.Models.Access;
using BL.Models.Modules.Research;
using System.Reflection;
using System.Net;
using System.Security.Cryptography;
using DAL.Models.WorkFlow;
using DAL.Models.Access;
using NPOI.SS.Formula.Functions;
using System.Diagnostics;
using BL.Models.Modules.Extension;

namespace BL.Services.ServiceRequest
{
    public class FormPageService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public SourceControlService Sservice;
        private RowAccessService service;
        private MatrixService Mxservice;
        public UserMatrixDTO UM = new UserMatrixDTO();
        public FormPageService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Sservice = new SourceControlService();
            service = new RowAccessService();
            Mxservice = new MatrixService();
            UM = Mxservice.sGetMatrixUserByUserID();
        }
        public BaseResponseDTO<bool> Delete(BaseRequestDTO<long> dataToDelId)
        {
            long Id = dataToDelId.Data;
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_FRM_FormPage upd = context.NPF_FRM_FormPage.Where(x => x.Id == Id).FirstOrDefault();
                List<NPF_FRM_WorkOrder> elml = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == upd.Id && x.DeleteStatus == false).ToList();
                foreach (NPF_FRM_WorkOrder elm in elml)
                {
                    elm.DeleteStatus = true;
                    context.NPF_FRM_WorkOrder.Update(elm);
                    context.SaveChanges();

                    List<NPF_FRM_WOElementMapping> paramcheckl = context.NPF_FRM_WOElementMapping.Where(x => x.WOId == elm.Id).ToList();
                    if (paramcheckl.Count > 0)
                    {
                        foreach (NPF_FRM_WOElementMapping paramcheck in paramcheckl)
                        {
                            paramcheck.DeleteStatus = true;
                            context.NPF_FRM_WOElementMapping.Update(paramcheck);
                            context.SaveChanges();
                        }
                    }

                }
                foreach (NPF_FRM_WorkOrder elm in elml)
                {
                    elm.DeleteStatus = true;
                    context.NPF_FRM_WorkOrder.Update(elm);
                    context.SaveChanges();


                }
                upd.DeleteStatus = true;
                context.NPF_FRM_FormPage.Update(upd);
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
        public BaseResponseDTO<List<NPF_FRM_FormPage>> GetAll()
        {
            BaseResponseDTO<List<NPF_FRM_FormPage>> dto = new BaseResponseDTO<List<NPF_FRM_FormPage>>();
            List<NPF_FRM_FormPage> result = new List<NPF_FRM_FormPage>();
            string errorMsg = "No Data Found";
            try
            {
                result = context.NPF_FRM_FormPage.Where(x => x.DeleteStatus == false).ToList();

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
        public async Task<BaseResponseDTO<List<NPF_FRM_FormPage>>> FilterDatatableByPeriod(BaseRequestDTO<DateRangeDTO> dt)
        {
            BaseResponseDTO<List<NPF_FRM_FormPage>> dto = new BaseResponseDTO<List<NPF_FRM_FormPage>>();
            List<NPF_FRM_FormPage> result = new List<NPF_FRM_FormPage>();

            if (dt == null)
            {
                dt = new BaseRequestDTO<DateRangeDTO>();
                dt.Data = new DateRangeDTO();
                dt.Data.From = DateTime.Now.AddDays(-14);
                dt.Data.To = DateTime.Now;
            }

            dt.Data.From = dt.Data.From.StartOfDay();

            dt.Data.To = dt.Data.To.EndOfDay();

            string errorMsg = "No Data Found";
            try
            {
                result = context.NPF_FRM_FormPage.Where(x => x.DeleteStatus == false && x.CreatedDate >= dt.Data.From && x.CreatedDate <= dt.Data.To).ToList();
                foreach (NPF_FRM_FormPage item in result)
                {
                    item.Additional1 = item.Status.ToString();
                }
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
        public BaseResponseDTO<List<DropDown>> GetAllDropDownValues()
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";

            try
            {

                //Form List
                List<NPF_FRM_Form> Pll = context.NPF_FRM_Form.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "FormList";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_Form pl in Pll)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = pl.Id;
                    Ddi.text = pl.Name;
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
        public async Task<BaseResponseDTO<List<DTSection>>> GetSelectedServices(List<long> Ids)//Added Service Ids
        {
            BaseResponseDTO<List<DTSection>> dto = new BaseResponseDTO<List<DTSection>>();
            dto.Data = new List<DTSection>();
            NPF_FRM_Section s = new NPF_FRM_Section();
            List<DTSection> DTsl = new List<DTSection>();
            List<Values> vl = new List<Values>();
            List<NPF_FRM_Elements> Elml = new List<NPF_FRM_Elements>();
            List<NPF_FRM_Elements> Elmlc = new List<NPF_FRM_Elements>();
            string errorMsg = "No Data Found";

            try
            {
                foreach (long id in Ids)
                {
                    DTSection DTs = new DTSection();
                    s = context.NPF_FRM_Section.Where(x => x.Id == id).FirstOrDefault();
                    DTs.sectionId = s.Id;
                    DTs.name = s.Name;
                    DTs.print = s.Print;
                    DTs.notification = s.Notification;
                    List<Elements> Elements = new List<Elements>();
                    Elml = context.NPF_FRM_Elements.Where(x => x.ServiceId == s.Id && x.DeleteStatus == false).OrderBy(x => x.Order).ToList();
                    foreach (NPF_FRM_Elements elm in Elml)
                    {
                        Elements el = new Elements()
                        {
                            elementId = elm.Id,
                            elementName = elm.Name,
                            elementType = elm.InputTypeDetails.Name,
                            uom = elm.UOM == null ? "" : elm.UOM.Name,
                            uomlist = elm.UOM == null ? new List<Values>() : context.NPF_SYS_UomDetails.Where(x => x.UomHeader.Id == elm.UOM.Id && x.DeleteStatus == false).Select(y => new Values
                            {
                                id = y.Id,
                                text = y.Name
                            }).ToList(),
                            sourceValues = elm.Source == null ? vl : await Sservice.SourceValues(elm.Id)
                        };
                        Elements.Add(el);
                        Elmlc = context.NPF_FRM_Elements.Where(x => x.ServiceId == s.Id && x.DeleteStatus == false && x.ParentId == elm.Id).OrderBy(x => x.Order).ToList();
                        foreach (NPF_FRM_Elements elmc in Elmlc)
                        {
                            NPF_FRM_Source? source = null;
                            if (elmc.Source != null)
                            {
                                source = context.NPF_FRM_Source.Where(x => x.Id == elmc.Source.Id && x.DeleteStatus == false).FirstOrDefault();
                            }
                            Elements elc = new Elements()
                            {
                                elementId = elmc.Id,
                                elementName = elmc.Name,
                                order = elmc.Order,
                                elementType = elmc.InputTypeDetails.Name,
                                uom = elmc.UOM == null ? "" : elmc.UOM.Name,
                                uomlist = elm.UOM == null ? new List<Values>() : context.NPF_SYS_UomDetails.Where(x => x.UomHeader.Id == elm.UOM.Id && x.DeleteStatus == false).Select(y => new Values
                                {
                                    id = y.Id,
                                    text = y.Name
                                }).ToList(),
                                parentid = el.elementId,
                                sourceValues = elmc.Source == null ? vl : await Sservice.SourceValues(elmc.Id),
                                entityname = source != null ? source.Name : null,
                                entitysourceId = source != null ? source.Id : null,
                            };

                            el.innerelements.Add(elc);
                        }
                    }
                    DTs.elements.AddRange(Elements);
                    DTsl.Add(DTs);
                }

                //DTo
                dto.Data = DTsl;
                dto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = DTsl;
                dto.AdditionalData = errorMsg;
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }
        public async Task<BaseResponseDTO<List<NPF_FRM_Section>>> GetAllServiceListing(List<long> Ids)//Current Services Ids
        {
            BaseResponseDTO<List<NPF_FRM_Section>> dto = new BaseResponseDTO<List<NPF_FRM_Section>>();
            List<NPF_FRM_Section> result = new List<NPF_FRM_Section>();
            string errorMsg = "No Data Found";
            try
            {
                result = context.NPF_FRM_Section.Where(x => !Ids.Contains(x.Id) && x.DeleteStatus == false).ToList();
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
        public async Task<BaseResponseDTO<FormPageDTO>> GetDTO(Guid MId)
        {
            BaseResponseDTO<FormPageDTO> dto = new BaseResponseDTO<FormPageDTO>();
            FormPageDTO SSR = new FormPageDTO();
            string errorMsg = "No Data Found";
            try
            {
                dto.AdditionalData = context.NPF_SYS_Modules.Where(x => x.Id == MId).FirstOrDefault().DisplayName;
                SSR.FormPageId = 0;
                SSR.Code = "AUTO-GENERATED";
                SSR.TotalAmount = 0;

                //Section
                SSR.Sections = null;
                dto.Data = SSR;
                dto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception e)
            {
                dto.Data = SSR;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }

            return dto;
        }
        public async Task<BaseResponseDTO<FormPageDTO>> GetById(long Id, Guid ModuleId, long WFHId)
        {
            BaseResponseDTO<FormPageDTO> dto = new BaseResponseDTO<FormPageDTO>();
            NPF_FRM_FormPage result = new NPF_FRM_FormPage();
            FormPageDTO SERRDto = new FormPageDTO();
            List<Elements> ElmL = new List<Elements>();
            string errorMsg = "No Data Found";
            BaseResponseAccessDTO<List<DynamicDTO>> dt = new BaseResponseAccessDTO<List<DynamicDTO>>();
            dt.Access = service.GetMenuAccessByIds(ModuleId, UM.UID);
            bool HasApproval = dt.Access.Any(x => x.AccessType == AccessOperationType.Approve && x.HasAccess == true);
            //bool Approver = context.NPF_RES_ApprovalUsers.Where(x => x.RowId == Id && x.UserId == UM.UID && x.ResearchSubModule == ResearchSubModule.Form).Any();
            bool Approver = context.NPF_SYS_WorkFlowProcessAssignmentDetails.Where(x => x.EntityId == Id && x.UserId == UM.UID).Any();

            bool IsApprover = Approver == true && Approver == true ? true : false;
            try
            {
                long Mseq = 0;
                if (WFHId > 0)
                {
                    NPF_SYS_WorkFlowProcessDetails dtlc = context.NPF_SYS_WorkFlowProcessDetails.Where(x => x.FkWFHId == WFHId && x.EntityId == Id && x.ModuleId == ModuleId && x.GeneralStatus != GeneralStatus.Closed && x.GeneralStatus != GeneralStatus.Completed).OrderBy(y => y.Sequence).FirstOrDefault();
                    try
                    {
                        if (dtlc != null)
                        {
                            Mseq = dtlc.FormSequence;
                        }
                    }
                    catch (Exception ex)
                    {
                        dtlc = context.NPF_SYS_WorkFlowProcessDetails.Where(x => x.FkWFHId == WFHId && x.EntityId == Id && x.ModuleId == ModuleId).OrderByDescending(y => y.Sequence).FirstOrDefault();
                        Mseq = dtlc.FormSequence;
                    }

                }
                result = context.NPF_FRM_FormPage.Where(x => x.Id == Id).FirstOrDefault();
                SERRDto.FormPageId = result.Id;
                SERRDto.Code = result.Code;
                SERRDto.FormId = result.FormId;
                SERRDto.Status = result.Status;
                SERRDto.TotalAmount = result.TotalAmount;
                SERRDto.Colour = result.Colour;
                SERRDto.Approval = result.Approval;
                SERRDto.IsApprover = IsApprover;

                //Section
                List<NPF_FRM_WorkOrder> WOl = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == Id && x.DeleteStatus == false).ToList();

                if (WOl != null)
                {
                    List<DTSection> Dtsl = new List<DTSection>();
                    foreach (NPF_FRM_WorkOrder wo in WOl)
                    {
                        bool ScHidden = false;
                        bool ScReadOnly = false;
                        List<NPF_FRM_FormPageElementModuleMapping> LFEM = new List<NPF_FRM_FormPageElementModuleMapping>();
                        NPF_FRM_FormPageSectionModuleMapping FSM = context.NPF_FRM_FormPageSectionModuleMapping.Where(x => x.FormId == result.FormId && x.Sequence >= Mseq && x.ModuleId == ModuleId && x.SectionId == wo.SectionId && x.DeleteStatus == false).FirstOrDefault();
                        if (FSM != null)
                        {
                            ScHidden = FSM.Hidden;
                            ScReadOnly = FSM.ReadOnly;
                            LFEM = context.NPF_FRM_FormPageElementModuleMapping.Where(x => x.Fk_FormSectionModuleId == FSM.Id && x.DeleteStatus == false).ToList();
                        }
                        DTSection Dts = new DTSection();
                        Dts.sectionId = wo.SectionId;
                        Dts.name = wo.SectionName;
                        Dts.print = wo.Print;
                        Dts.amount = wo.Amount != null ? Convert.ToDecimal(wo.Amount) : 0;
                        Dts.notification = wo.Notification;
                        Dts.Hidden = ScHidden;
                        Dts.ReadOnly = ScReadOnly;

                        Dts.elements = new List<Elements>();
                        List<NPF_FRM_WOElementMapping> listing = context.NPF_FRM_WOElementMapping.Where(x => x.WOId == wo.Id && x.DeleteStatus == false).ToList();
                        List<NPF_FRM_WOElementMapping> WOlm = listing.Where(x => x.Isvalue == null && x.ParentId == null).ToList();
                        if (WOlm.Count > 0)
                        {
                            foreach (NPF_FRM_WOElementMapping wom in WOlm)
                            {
                                NPF_FRM_FormPageElementModuleMapping FEM = new NPF_FRM_FormPageElementModuleMapping();
                                bool Hidden = false;
                                bool ReadOnly = false;
                                if (LFEM != null)
                                {
                                    FEM = LFEM.Where(x => x.Fk_FormSectionModuleId == FSM.Id && x.ElementId == wom.ElementId && x.DeleteStatus == false).FirstOrDefault();
                                }


                                if (FSM != null)
                                {
                                    if (FSM.ReadOnly == true)
                                    {
                                        ReadOnly = FSM.ReadOnly;
                                    }
                                    if (FSM.Hidden == true)
                                    {
                                        Hidden = FSM.Hidden;
                                    }


                                }
                                if (FEM != null)
                                {
                                    Hidden = FEM.Hidden;
                                    ReadOnly = FEM.ReadOnly;
                                    if (FEM.Hidden == false)
                                    {
                                        Dts.Hidden = false;
                                    }
                                    if (FEM.ReadOnly == false)
                                    {
                                        Dts.ReadOnly = false;
                                    }
                                }
                                Elements el = new Elements()
                                {
                                    id = wom.Id,
                                    elementId = wom.ElementId,
                                    elementName = wom.ElementName,
                                    elementType = wom.ElementType,
                                    uom = wom.UOM,
                                    uomlist = wom.UOM == null ? new List<Values>() : context.NPF_SYS_UomDetails.Where(x => x.UomHeader.Name == wom.UOM && x.DeleteStatus == false).Select(y => new Values
                                    {
                                        id = y.Id,
                                        text = y.Name
                                    }).ToList(),
                                    value = wom.Value,
                                    additionalValue = wom.AdditionalValue,
                                    sourceValues = await Sservice.SourceValues(wom.ElementId),
                                    Hidden = Hidden,
                                    ReadOnly = ReadOnly
                                };
                                List<NPF_FRM_WOElementMapping> WOlmc = listing.Where(x => x.Isvalue == false && x.ParentId == wom.ElementId).ToList();
                                List<long?> Glist = new List<long?>();
                                if (WOlmc.Count > 0)
                                {
                                    foreach (NPF_FRM_WOElementMapping womc in WOlmc)
                                    {
                                        if (LFEM != null)
                                        {
                                            FEM = LFEM.Where(x => x.Fk_FormSectionModuleId == FSM.Id && x.ElementId == womc.ElementId && x.DeleteStatus == false).FirstOrDefault();
                                        }


                                        if (FSM != null)
                                        {
                                            if (FSM.ReadOnly == true)
                                            {
                                                ReadOnly = FSM.ReadOnly;
                                            }
                                            if (FSM.Hidden == true)
                                            {
                                                Hidden = FSM.Hidden;
                                            }


                                        }
                                        if (FEM != null)
                                        {
                                            Hidden = FEM.Hidden;
                                            ReadOnly = FEM.ReadOnly;
                                        }
                                        Glist.Add(womc.ElementId);
                                        Elements elc = new Elements()
                                        {
                                            id = womc.Id,
                                            elementId = womc.ElementId,
                                            elementName = womc.ElementName,
                                            elementType = womc.ElementType,
                                            uom = womc.UOM,
                                            uomlist = womc.UOM == null ? new List<Values>() : context.NPF_SYS_UomDetails.Where(x => x.UomHeader.Name == womc.UOM && x.DeleteStatus == false).Select(y => new Values
                                            {
                                                id = y.Id,
                                                text = y.Name
                                            }).ToList(),
                                            value = womc.Value,
                                            additionalValue = womc.AdditionalValue,
                                            parentid = womc.ParentId,
                                            row = womc.Row,
                                            column = womc.Column,
                                            entityid = womc.ElementId,
                                            entityname = womc.EntityName,
                                            entitysourceId = womc.EntitySourceId,
                                            Hidden = Hidden,
                                            ReadOnly = ReadOnly,
                                            sourceValues = await Sservice.SourceValues(womc.ElementId)
                                        };
                                        el.innerelements.Add(elc);
                                    }
                                }
                                List<NPF_FRM_WOElementMapping> WOlmcv = listing.Where(x => x.Isvalue == true && Glist.Contains(x.ParentId)).ToList();
                                if (WOlmcv.Count > 0)
                                {
                                    foreach (NPF_FRM_WOElementMapping womcv in WOlmcv)
                                    {
                                        if (LFEM != null)
                                        {
                                            FEM = LFEM.Where(x => x.Fk_FormSectionModuleId == FSM.Id && x.ElementId == womcv.ElementId && x.DeleteStatus == false).FirstOrDefault();
                                        }


                                        if (FSM != null)
                                        {
                                            if (FSM.ReadOnly == true)
                                            {
                                                ReadOnly = FSM.ReadOnly;
                                            }
                                            if (FSM.Hidden == true)
                                            {
                                                Hidden = FSM.Hidden;
                                            }


                                        }
                                        if (FEM != null)
                                        {
                                            Hidden = FEM.Hidden;
                                            ReadOnly = FEM.ReadOnly;
                                        }
                                        Elements elcv = new Elements()
                                        {
                                            id = womcv.Id,
                                            elementId = (long)womcv.ParentId, // TO CHECK
                                            elementName = womcv.ElementName,
                                            elementType = womcv.ElementType,
                                            uom = womcv.UOM,
                                            uomlist = womcv.UOM == null ? new List<Values>() : context.NPF_SYS_UomDetails.Where(x => x.UomHeader.Name == womcv.UOM && x.DeleteStatus == false).Select(y => new Values
                                            {
                                                id = y.Id,
                                                text = y.Name
                                            }).ToList(),
                                            value = womcv.Value,
                                            additionalValue = womcv.AdditionalValue,
                                            parentid = womcv.ParentId,
                                            row = womcv.Row,
                                            column = womcv.Column,
                                            entityid = womcv.ElementId,
                                            entityname = womcv.EntityName,
                                            entitysourceId = womcv.EntitySourceId,
                                            Hidden = Hidden,
                                            ReadOnly = ReadOnly
                                        };
                                        el.innerelementsvalue.Add(elcv);
                                    }
                                }
                                Dts.elements.Add(el);
                            }
                        }
                        else
                        {
                            Dts.elements = null;
                        }
                        Dtsl.Add(Dts);
                    }
                    SERRDto.Sections = Dtsl;
                }
                else
                {
                    SERRDto.Sections = null;
                }
                if (SERRDto == null)
                {
                    dto.Data = SERRDto;
                    dto.QryResult = queryResult.FAILED;
                }
                else
                {
                    dto.Data = SERRDto;
                    dto.QryResult = queryResult.SUCEEDED;
                }

            }
            catch (Exception e)
            {
                dto.Data = SERRDto;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }

            return dto;
        }
        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<FormPageDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            NPF_FRM_FormPage SERR = new NPF_FRM_FormPage();
            try
            {
                long? PId = context.NPF_FRM_Form.Where(x => x.Id == dataToSave.Data.FormId).FirstOrDefault().ParentId;
                if (PId != null)
                {
                    SERR.PFormId = PId;

                }
                bool check = false;
                string Abbrevation = string.Empty;
                try
                {
                    var UsrZn = context.NPF_SYS_UserDetails.Where(x => x.User.Id == UM.UID).FirstOrDefault();
                    if (UsrZn != null)
                    {
                        if (UsrZn.Office != null && UsrZn.Office != 0)
                        {
                            Abbrevation = context.NPF_SYS_OfficeZones.Where(x => x.Id == Convert.ToInt64(UsrZn.Office)).FirstOrDefault().Abbrevation;
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_FRM_FormPage");
                //ServiceRequest
                SERR.Code = BaseDtoS.Data;// "SRR" + 
                SERR.FormId = dataToSave.Data.FormId;
                SERR.FormName = context.NPF_FRM_Form.Where(x => x.Id == SERR.FormId).FirstOrDefault().Name;
                SERR.Status = dataToSave.Data.Status;
                SERR.TotalAmount = dataToSave.Data.TotalAmount;
                SERR.Colour = dataToSave.Data.Colour;
                SERR.CurrentSequence = 0;
                SERR.IsAuthorised = Signed.NO;
                SERR.Approval = dataToSave.Data.ApprovalS;
                //Add To DB
                context.NPF_FRM_FormPage.Add(SERR);
                context.SaveChanges();
                try
                {
                    NPF_SYS_Modules Mds = context.NPF_SYS_Modules.Where(x => x.Id == dataToSave.ModuleId).FirstOrDefault();
                    if (Mds.HasAutoGeneratedCode == true)
                    {
                        check = true;
                    }
                }
                catch (Exception ex)
                {

                }

                //Service
                if (dataToSave.Data.Sections.Count > 0)
                {
                    foreach (DTSection sr in dataToSave.Data.Sections)
                    {
                        NPF_FRM_WorkOrder WO = new NPF_FRM_WorkOrder()
                        {
                            SectionId = sr.sectionId,
                            FormPageId = SERR.Id,
                            SectionName = sr.name,
                            Amount = sr.amount,
                            Print = sr.print,
                            Status = Status.Draft,
                            Notification = sr.notification
                        };
                        context.NPF_FRM_WorkOrder.Add(WO);
                        context.SaveChanges();
                        if (sr.elements.Count > 0)
                        {
                            foreach (Elements ele in sr.elements)
                            {
                                BaseResponseDTO<string> BaseDtoSs = new BaseResponseDTO<string>();
                                if (check)
                                {
                                    NPF_FRM_Elements echeck = context.NPF_FRM_Elements.Where(x => x.Id == ele.elementId).FirstOrDefault();
                                    if (echeck.IsAutoGenerated == true)
                                    {

                                        BaseDtoSs = await new GenerateCode().GCodeForm(SERR.FormId, ele.elementName, Abbrevation);
                                    }
                                }

                                NPF_FRM_WOElementMapping WOel = new NPF_FRM_WOElementMapping()
                                {
                                    WOId = WO.Id,
                                    ElementId = ele.elementId,
                                    ElementName = ele.elementName,
                                    ElementType = ele.elementType,
                                    UOM = ele.uom,
                                    Value = BaseDtoSs.Data ?? ele.value,
                                    AdditionalValue = ele.additionalValue,
                                    Isvalue = null
                                };
                                context.NPF_FRM_WOElementMapping.Add(WOel);
                                context.SaveChanges();
                                if (ele.innerelements.Count > 0)
                                {
                                    foreach (Elements elmv in ele.innerelements)
                                    {
                                        NPF_FRM_WOElementMapping WOelc = new NPF_FRM_WOElementMapping()
                                        {
                                            WOId = WO.Id,
                                            ElementId = elmv.elementId,
                                            ElementName = elmv.elementName,
                                            ElementType = elmv.elementType,
                                            UOM = elmv.uom,
                                            Value = elmv.value,
                                            AdditionalValue = elmv.additionalValue,
                                            ParentId = WOel.ElementId,//////
                                            Isvalue = false
                                        };
                                        context.NPF_FRM_WOElementMapping.Add(WOelc);
                                        context.SaveChanges();
                                    }
                                }
                                if (ele.innerelementsvalue.Count > 0)
                                {
                                    foreach (Elements elmc in ele.innerelementsvalue)
                                    {
                                        NPF_FRM_WOElementMapping WOelc = new NPF_FRM_WOElementMapping()
                                        {
                                            WOId = WO.Id,
                                            ElementId = elmc.elementId,
                                            ElementName = elmc.elementName,
                                            ElementType = elmc.elementType,
                                            UOM = elmc.uom,
                                            Value = elmc.value,
                                            AdditionalValue = elmc.additionalValue,
                                            ParentId = elmc.parentid,
                                            EntitySourceId = elmc.entitysourceId,
                                            EntityName = elmc.entityname,
                                            EntityId = elmc.entityid,
                                            Column = elmc.column,
                                            Row = elmc.row,
                                            Isvalue = true
                                        };
                                        context.NPF_FRM_WOElementMapping.Add(WOelc);
                                        context.SaveChanges();
                                    }
                                }
                            }

                        }
                    }

                }
                BaseDto.Data = true;
                BaseDto.AdditionalData = SERR.Id.ToString();
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
        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<FormPageDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            NPF_FRM_FormPage SERR = new NPF_FRM_FormPage();
            try
            {
                //if(dataToUpdate.ModuleId.ToString() == "11274736-51f5-4107-592d-08dbf273460c")
                //{
                //    if(dataToUpdate.Data.Status == Status.Submitted)
                //    {
                //        dataToUpdate.Data.Status = Status.SubmittedLvl2;
                //    }
                //    if (dataToUpdate.Data.Status == Status.Draft)
                //    {
                //        dataToUpdate.Data.Status = Status.DrafLvl2;
                //    }
                //}
                //else if (dataToUpdate.ModuleId.ToString() == "046e1192-2ec5-45c3-5384-08dbe4d58ee7")
                //{
                //    if (dataToUpdate.Data.Status == Status.Submitted)
                //    {
                //        dataToUpdate.Data.Status = Status.Completed;
                //    }
                //    if (dataToUpdate.Data.Status == Status.Draft)
                //    {
                //        dataToUpdate.Data.Status = Status.DrafLvl3;
                //    }
                //}
                //else if (dataToUpdate.ModuleId.ToString() == "038df672-8399-414a-d8cc-08dbd9e406e8")
                //{
                //    if (dataToUpdate.Data.Status == Status.Submitted)
                //    {
                //        dataToUpdate.Data.Status = Status.SubmittedLvl2;
                //    }
                //    if (dataToUpdate.Data.Status == Status.Draft)
                //    {
                //        dataToUpdate.Data.Status = Status.DrafLvl2;
                //    }
                //}
                //else if (dataToUpdate.ModuleId.ToString() == "061330ba-e580-4f69-d8cd-08dbd9e406e8")
                //{
                //    if (dataToUpdate.Data.Status == Status.Submitted)
                //    {
                //        dataToUpdate.Data.Status = Status.SubmittedLvl3;
                //    }
                //    if (dataToUpdate.Data.Status == Status.Draft)
                //    {
                //        dataToUpdate.Data.Status = Status.DrafLvl3;
                //    }
                //}
                //else if (dataToUpdate.ModuleId.ToString() == "79724ccb-0f90-4a41-5385-08dbe4d58ee7")
                //{
                //    if (dataToUpdate.Data.Status == Status.Submitted)
                //    {
                //        dataToUpdate.Data.Status = Status.Completed;
                //    }
                //    if (dataToUpdate.Data.Status == Status.Draft)
                //    {
                //        dataToUpdate.Data.Status = Status.DrafLvl4;
                //    }
                //}
                //else if (dataToUpdate.ModuleId.ToString() == "8da2f734-d6d2-43da-01fe-08dc26dc0010")
                //{
                //    if (dataToUpdate.Data.Status == Status.Submitted)
                //    {
                //        dataToUpdate.Data.Status = Status.Completed;
                //    }
                //    if (dataToUpdate.Data.Status == Status.Draft)
                //    {
                //        dataToUpdate.Data.Status = Status.DrafLvl2;
                //    }
                //}
                SERR = context.NPF_FRM_FormPage.Where(c => c.Id == dataToUpdate.Data.FormPageId).FirstOrDefault();
                //ServiceRequest
                SERR.Code = dataToUpdate.Data.Code;
                SERR.FormId = dataToUpdate.Data.FormId;
                SERR.FormName = context.NPF_FRM_Form.Where(x => x.Id == SERR.FormId).FirstOrDefault().Name;
                SERR.Status = dataToUpdate.Data.Status;
                SERR.Colour = dataToUpdate.Data.Colour;
                SERR.TotalAmount = dataToUpdate.Data.TotalAmount;
                SERR.IsAuthorised = Signed.NO;
                SERR.Approval = ApprovalStatus.Draft;
                //Add To DB
                context.NPF_FRM_FormPage.Update(SERR);
                context.SaveChanges();


                //Section
                List<NPF_FRM_WorkOrder> WOLC = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == SERR.Id && x.DeleteStatus == false).ToList();//List All Current Services
                if (dataToUpdate.Data.Sections.Count > 0)// Incoming Services
                {
                    foreach (DTSection sr in dataToUpdate.Data.Sections)//Loop In Incoming Services
                    {
                        NPF_FRM_WorkOrder wc = WOLC.Where(x => x.SectionId == sr.sectionId).FirstOrDefault();// Check Incoming Exist in current
                        if (wc == null)//If true Add
                        {
                            NPF_FRM_WorkOrder WO = new NPF_FRM_WorkOrder()
                            {
                                SectionId = sr.sectionId,
                                FormPageId = SERR.Id,
                                SectionName = sr.name,
                                Amount = sr.amount,
                                Print = sr.print,
                                Status = Status.Draft,
                                Notification = sr.notification
                            };
                            context.NPF_FRM_WorkOrder.Add(WO);
                            context.SaveChanges();
                            if (sr.elements != null)
                            {
                                if (sr.elements.Count > 0)//Check Element If Any Add
                                {
                                    foreach (Elements ele in sr.elements)
                                    {
                                        NPF_FRM_WOElementMapping WOel = new NPF_FRM_WOElementMapping()
                                        {
                                            WOId = WO.Id,
                                            ElementId = ele.elementId,
                                            ElementName = ele.elementName,
                                            ElementType = ele.elementType,
                                            UOM = ele.uom,
                                            Value = ele.value,
                                            AdditionalValue = ele.additionalValue
                                        };
                                        context.NPF_FRM_WOElementMapping.Add(WOel);
                                        context.SaveChanges();
                                        if (ele.innerelements.Count > 0)
                                        {
                                            foreach (Elements elmv in ele.innerelements)
                                            {
                                                NPF_FRM_WOElementMapping WOelc = new NPF_FRM_WOElementMapping()
                                                {
                                                    WOId = WO.Id,
                                                    ElementId = elmv.elementId,
                                                    ElementName = elmv.elementName,
                                                    ElementType = elmv.elementType,
                                                    UOM = elmv.uom,
                                                    Value = elmv.value,
                                                    AdditionalValue = elmv.additionalValue,
                                                    ParentId = ele.elementId,
                                                    Isvalue = false
                                                };
                                                context.NPF_FRM_WOElementMapping.Add(WOelc);
                                                context.SaveChanges();
                                            }
                                        }
                                        if (ele.innerelementsvalue.Count > 0)
                                        {
                                            foreach (Elements elmc in ele.innerelementsvalue)
                                            {
                                                NPF_FRM_WOElementMapping WOelc = new NPF_FRM_WOElementMapping()
                                                {
                                                    WOId = WO.Id,
                                                    ElementId = elmc.elementId,
                                                    ElementName = elmc.elementName,
                                                    ElementType = elmc.elementType,
                                                    UOM = elmc.uom,
                                                    Value = elmc.value,
                                                    AdditionalValue = elmc.additionalValue,
                                                    ParentId = elmc.parentid,
                                                    EntitySourceId = elmc.entitysourceId,
                                                    EntityName = elmc.entityname,
                                                    EntityId = elmc.entityid,
                                                    Column = elmc.column,
                                                    Row = elmc.row,
                                                    Isvalue = true
                                                };
                                                context.NPF_FRM_WOElementMapping.Add(WOelc);
                                                context.SaveChanges();
                                            }
                                        }

                                    }

                                }
                            }

                        }
                        else//Update
                        {
                            //Update Service
                            wc.SectionId = sr.sectionId;
                            wc.FormPageId = SERR.Id;
                            wc.SectionName = sr.name;
                            wc.Amount = sr.amount;
                            wc.Print = sr.print;
                            wc.Status = Status.Draft;
                            wc.Notification = sr.notification;
                            context.NPF_FRM_WorkOrder.Update(wc);
                            context.SaveChanges();
                            WOLC.Remove(wc);// Remove In Current List
                            List<NPF_FRM_WOElementMapping> womcl = context.NPF_FRM_WOElementMapping.Where(x => x.WOId == wc.Id && x.DeleteStatus == false).ToList();//Current Element List
                            if (sr.elements != null)
                            {
                                if (sr.elements.Count > 0)//Check Incoming Element List
                                {
                                    foreach (Elements ele in sr.elements)
                                    {
                                        NPF_FRM_WOElementMapping womc = womcl.Where(x => x.ElementId == ele.elementId && x.ParentId == null).FirstOrDefault(); //Check Incoming Element Exist in current Element
                                        if (womc == null)//If True Add
                                        {
                                            NPF_FRM_WOElementMapping WOel = new NPF_FRM_WOElementMapping()
                                            {
                                                WOId = wc.Id,
                                                ElementId = ele.elementId,
                                                ElementName = ele.elementName,
                                                ElementType = ele.elementType,
                                                UOM = ele.uom,
                                                Value = ele.value,
                                                AdditionalValue = ele.additionalValue
                                            };
                                            context.NPF_FRM_WOElementMapping.Add(WOel);
                                            context.SaveChanges();
                                            if (ele.innerelements.Count > 0)
                                            {
                                                foreach (Elements elmv in ele.innerelements)
                                                {
                                                    NPF_FRM_WOElementMapping WOelc = new NPF_FRM_WOElementMapping()
                                                    {
                                                        WOId = wc.Id,
                                                        ElementId = elmv.elementId,
                                                        ElementName = elmv.elementName,
                                                        ElementType = elmv.elementType,
                                                        UOM = elmv.uom,
                                                        Value = elmv.value,
                                                        AdditionalValue = elmv.additionalValue,
                                                        ParentId = ele.elementId,
                                                        Isvalue = false
                                                    };
                                                    context.NPF_FRM_WOElementMapping.Add(WOel);
                                                    context.SaveChanges();
                                                }
                                            }
                                            if (ele.innerelementsvalue.Count > 0)
                                            {
                                                foreach (Elements elmc in ele.innerelementsvalue)
                                                {
                                                    NPF_FRM_WOElementMapping WOelc = new NPF_FRM_WOElementMapping()
                                                    {
                                                        WOId = wc.Id,
                                                        ElementId = elmc.elementId,
                                                        ElementName = elmc.elementName,
                                                        ElementType = elmc.elementType,
                                                        UOM = elmc.uom,
                                                        Value = elmc.value,
                                                        AdditionalValue = elmc.additionalValue,
                                                        ParentId = elmc.parentid,
                                                        EntitySourceId = elmc.entitysourceId,
                                                        EntityName = elmc.entityname,
                                                        EntityId = elmc.entityid,
                                                        Column = elmc.column,
                                                        Row = elmc.row,
                                                        Isvalue = true
                                                    };
                                                    context.NPF_FRM_WOElementMapping.Add(WOel);
                                                    context.SaveChanges();
                                                }
                                            }
                                        }
                                        else// Update
                                        {
                                            //UpdateAsync Element
                                            womc.ElementId = ele.elementId;
                                            womc.ElementName = ele.elementName;
                                            womc.ElementType = ele.elementType;
                                            womc.UOM = ele.uom;
                                            womc.Value = ele.value;
                                            womc.AdditionalValue = ele.additionalValue;
                                            context.NPF_FRM_WOElementMapping.Update(womc);
                                            context.SaveChanges();
                                            womcl.Remove(womc);// Remove Element In Current List

                                            if (ele.innerelements.Count > 0)
                                            {
                                                foreach (Elements elmv in ele.innerelements)
                                                {
                                                    NPF_FRM_WOElementMapping womcc = womcl.Where(x => x.ParentId == elmv.parentid).FirstOrDefault(); //Check Incoming inner Element Exist in current Element
                                                    if (womc == null)//If True Add
                                                    {
                                                        NPF_FRM_WOElementMapping WOelc = new NPF_FRM_WOElementMapping()
                                                        {
                                                            WOId = wc.Id,
                                                            ElementId = elmv.elementId,
                                                            ElementName = elmv.elementName,
                                                            ElementType = elmv.elementType,
                                                            UOM = elmv.uom,
                                                            Value = elmv.value,
                                                            AdditionalValue = elmv.additionalValue,
                                                            ParentId = ele.elementId,
                                                            Isvalue = false
                                                        };
                                                        context.NPF_FRM_WOElementMapping.Add(WOelc);
                                                        context.SaveChanges();

                                                    }
                                                    else// Update
                                                    {
                                                        //womc.ElementId = ele.elementId;
                                                        //womc.ElementName = ele.elementName;
                                                        //womc.ElementType = ele.elementType;
                                                        //womc.UOM = ele.uom;
                                                        //womc.Value = ele.value;
                                                        //womc.AdditionalValue = ele.additionalValue;
                                                        //context.NPF_FRM_WOElementMapping.Update(womc);
                                                        //context.SaveChanges();
                                                        womcl.Remove(womcc);// Remove Element In Current List
                                                    }
                                                }
                                            }
                                            if (ele.innerelementsvalue.Count > 0)
                                            {
                                                foreach (Elements elmv in ele.innerelementsvalue)
                                                {
                                                    NPF_FRM_WOElementMapping womcc = womcl.Where(x => x.Row == elmv.row && x.Value == elmv.value && x.Column == elmv.column && x.ParentId == x.ParentId).FirstOrDefault(); //Check Incoming inner Element Exist in current Element
                                                    if (womcc == null)//If True Add
                                                    {
                                                        NPF_FRM_WOElementMapping WOelc = new NPF_FRM_WOElementMapping()
                                                        {
                                                            WOId = wc.Id,
                                                            ElementId = elmv.elementId,
                                                            ElementName = elmv.elementName,
                                                            ElementType = elmv.elementType,
                                                            UOM = elmv.uom,
                                                            Value = elmv.value,
                                                            AdditionalValue = elmv.additionalValue,
                                                            ParentId = elmv.parentid,
                                                            Isvalue = true,
                                                            Row = elmv.row,
                                                            Column = elmv.column,
                                                            EntityId = elmv.entityid,
                                                            EntityName = elmv.entityname,
                                                            EntitySourceId = elmv.entitysourceId
                                                        };
                                                        context.NPF_FRM_WOElementMapping.Add(WOelc);
                                                        context.SaveChanges();

                                                    }
                                                    else// Update
                                                    {
                                                        //womc.ElementId = ele.elementId;
                                                        //womc.ElementName = ele.elementName;
                                                        //womc.ElementType = ele.elementType;
                                                        //womc.UOM = ele.uom;
                                                        //womc.Value = ele.value;
                                                        //womc.AdditionalValue = ele.additionalValue;
                                                        //context.NPF_FRM_WOElementMapping.Update(womc);
                                                        //context.SaveChanges();
                                                        womcl.Remove(womcc);// Remove Element In Current List
                                                    }
                                                }
                                            }

                                        }

                                    }
                                    if (womcl.Count > 0) // Check Current Element List if Any
                                    {
                                        // Delete Remaining Element
                                        foreach (NPF_FRM_WOElementMapping w in womcl)
                                        {
                                            w.DeleteStatus = true;
                                            context.NPF_FRM_WOElementMapping.Update(w);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    if (womcl.Count > 0)// Check Current Element List if Any
                                    {
                                        // Delete Remaining Element
                                        foreach (NPF_FRM_WOElementMapping w in womcl)
                                        {
                                            w.DeleteStatus = true;
                                            context.NPF_FRM_WOElementMapping.Update(w);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (WOLC.Count > 0)// Check Current WorkOrder List if Any
                    {
                        foreach (NPF_FRM_WorkOrder WOc in WOLC)
                        {
                            // Delete Remaining WorkOrder
                            WOc.DeleteStatus = true;
                            context.NPF_FRM_WorkOrder.Update(WOc);
                            context.SaveChanges();
                        }
                    }
                }
                else
                {
                    if (WOLC.Count > 0)// Check Current WorkOrder List if Any
                    {
                        foreach (NPF_FRM_WorkOrder WOc in WOLC)
                        {
                            List<NPF_FRM_WOElementMapping> wmp = context.NPF_FRM_WOElementMapping.Where(x => x.WOId == WOc.Id && x.DeleteStatus == false).ToList();// Current Element List
                            if (wmp.Count > 0)// Check Current Element List if Any
                            {
                                // Delete Remaining Element
                                foreach (NPF_FRM_WOElementMapping w in wmp)
                                {
                                    w.DeleteStatus = true;
                                    context.NPF_FRM_WOElementMapping.Update(w);
                                    context.SaveChanges();
                                }
                            }
                            // Delete Remaining WorkOrder
                            WOc.DeleteStatus = true;
                            context.NPF_FRM_WorkOrder.Update(WOc);
                            context.SaveChanges();

                        }
                    }
                }
                //if (SERR.Status == dataToUpdate.Data.Status)
                //{
                //    List<FSM_SER_SRPrint> srpl = context.FSM_SER_SRPrint.Where(x => x.FormPageId == SERR.Id && x.DeleteStatus == false).ToList();
                //    foreach (FSM_SER_SRPrint srp in srpl)
                //    {
                //        srp.DeleteStatus = true;
                //        context.FSM_SER_SRPrint.Update(srp);
                //        context.SaveChanges();
                //    }
                //    FSM_SER_SRPrint sp = new FSM_SER_SRPrint()
                //    {
                //        FormPageId = SERR.Id,
                //        Print = dataToUpdate.Data.Print
                //    };
                //    context.FSM_SER_SRPrint.Add(sp);
                //    context.SaveChanges();
                //    SERR.PrintId = sp.Id;
                //    context.NPF_FRM_FormPage.Update(SERR);
                //    context.SaveChanges();
                //}
                BaseDto.Data = true;
                BaseDto.AdditionalData = SERR.Id.ToString();
                BaseDto.QryResult = queryResult.SUCEEDED;
                //  }

            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<SectionElements>> GetSectionElementsByForm(long id, Guid ModuleId)//FormId
        {
            BaseResponseDTO<SectionElements> dto = new BaseResponseDTO<SectionElements>();
            dto.Data = new SectionElements();
            ElementList Addl = new ElementList();
            List<NPF_FRM_FormSectionMapping> psml = new List<NPF_FRM_FormSectionMapping>();
            List<NPF_FRM_Section> sl = new List<NPF_FRM_Section>();
            List<DTSection> DTsl = new List<DTSection>();
            List<Elements> Elements = new List<Elements>();
            List<NPF_FRM_Elements> Elml = new List<NPF_FRM_Elements>();
            List<Values> vl = new List<Values>();
            string errorMsg = "No Data Found";

            try
            {
                List<NPF_FRM_Elements> Elmlc = new List<NPF_FRM_Elements>();
                sl = (from a in context.NPF_FRM_FormSectionMapping
                      join b in context.NPF_FRM_Section on a.SectionId equals b.Id
                      where a.DeleteStatus == false && a.FormId == id && b.DeleteStatus == false
                      select new NPF_FRM_Section
                      {
                          Id = b.Id,
                          Code = b.Code,
                          Name = b.Name,
                          Description = b.Description,
                          Print = b.Print,
                          Multiple = b.Multiple,
                          Notification = b.Notification,
                          AssetTypeId = b.AssetTypeId,
                          Order = a.Order
                      }).OrderBy(z => z.Order).ToList();
                List<long> llids = sl.Select(x => x.Id).ToList();
                foreach (NPF_FRM_Section s in sl)
                {
                    bool ScHidden = false;
                    bool ScReadOnly = false;

                    List<NPF_FRM_FormPageElementModuleMapping> LFEM = new List<NPF_FRM_FormPageElementModuleMapping>();
                    NPF_FRM_FormPageSectionModuleMapping FSM = context.NPF_FRM_FormPageSectionModuleMapping.Where(x => x.FormId == id && x.ModuleId == ModuleId && x.SectionId == s.Id && x.DeleteStatus == false).FirstOrDefault();
                    if (FSM != null)
                    {
                        ScHidden = FSM.Hidden;
                        ScReadOnly = FSM.ReadOnly;
                        LFEM = context.NPF_FRM_FormPageElementModuleMapping.Where(x => x.Fk_FormSectionModuleId == FSM.Id && x.DeleteStatus == false).ToList();
                    }
                    Elements = new List<Elements>();
                    Elml = new List<NPF_FRM_Elements>();
                    DTSection DTs = new DTSection();
                    DTs.sectionId = s.Id;
                    DTs.name = s.Name;
                    DTs.print = s.Print;
                    DTs.Hidden = ScHidden;
                    DTs.ReadOnly = ScReadOnly;
                    DTs.notification = s.Notification;

                    Elml = context.NPF_FRM_Elements.Where(x => x.ServiceId == s.Id && x.DeleteStatus == false && x.ParentId == null).OrderBy(x => x.Order).ToList();
                    foreach (NPF_FRM_Elements elm in Elml)
                    {
                        NPF_FRM_FormPageElementModuleMapping FEM = new NPF_FRM_FormPageElementModuleMapping();
                        bool Hidden = false;
                        bool ReadOnly = false;
                        if (LFEM != null)
                        {
                            FEM = LFEM.Where(x => x.Fk_FormSectionModuleId == FSM.Id && x.ElementId == elm.Id && x.DeleteStatus == false).FirstOrDefault();
                        }


                        if (FSM != null)
                        {
                            if (FSM.ReadOnly == true)
                            {
                                ReadOnly = FSM.ReadOnly;
                            }
                            if (FSM.Hidden == true)
                            {
                                Hidden = FSM.Hidden;
                            }


                        }
                        if (FEM != null)
                        {
                            Hidden = FEM.Hidden;
                            ReadOnly = FEM.ReadOnly;
                            if (FEM.Hidden == false)
                            {
                                DTs.Hidden = false;
                            }
                            if (FEM.ReadOnly == false)
                            {
                                DTs.ReadOnly = false;
                            }
                        }
                        Elements el = new Elements()
                        {
                            elementId = elm.Id,
                            elementName = elm.Name,
                            elementType = elm.InputTypeDetails.Name,
                            uom = elm.UOM == null ? "" : elm.UOM.Name,
                            uomlist = elm.UOM == null ? new List<Values>() : context.NPF_SYS_UomDetails.Where(x => x.UomHeader.Id == elm.UOM.Id && x.DeleteStatus == false).Select(y => new Values
                            {
                                id = y.Id,
                                text = y.Name
                            }).ToList(),
                            Hidden = Hidden,
                            ReadOnly = ReadOnly,
                            sourceValues = elm.Source == null ? vl : await Sservice.SourceValues(elm.Id),
                            url = elm.URL
                        };
                        Elmlc = context.NPF_FRM_Elements.Where(x => x.ServiceId == s.Id && x.DeleteStatus == false && x.ParentId == elm.Id).OrderBy(x => x.Order).ToList();
                        foreach (NPF_FRM_Elements elmc in Elmlc)
                        {
                            if (LFEM != null)
                            {
                                FEM = LFEM.Where(x => x.Fk_FormSectionModuleId == FSM.Id && x.ElementId == elmc.Id && x.DeleteStatus == false).FirstOrDefault();
                            }


                            if (FSM != null)
                            {
                                if (FSM.ReadOnly == true)
                                {
                                    ReadOnly = FSM.ReadOnly;
                                }
                                if (FSM.Hidden == true)
                                {
                                    Hidden = FSM.Hidden;
                                }


                            }
                            if (FEM != null)
                            {
                                Hidden = FEM.Hidden;
                                ReadOnly = FEM.ReadOnly;
                            }
                            NPF_FRM_Source? source = null;
                            if (elmc.Source != null)
                            {
                                source = context.NPF_FRM_Source.Where(x => x.Id == elmc.Source.Id && x.DeleteStatus == false).FirstOrDefault();
                            }
                            Elements elc = new Elements()
                            {
                                elementId = elmc.Id,
                                elementName = elmc.Name,
                                elementType = elmc.InputTypeDetails.Name,
                                order = elmc.Order,
                                parentid = el.elementId,
                                uom = elmc.UOM == null ? "" : elmc.UOM.Name,
                                uomlist = elm.UOM == null ? new List<Values>() : context.NPF_SYS_UomDetails.Where(x => x.UomHeader.Id == elm.UOM.Id && x.DeleteStatus == false).Select(y => new Values
                                {
                                    id = y.Id,
                                    text = y.Name
                                }).ToList(),
                                sourceValues = elmc.Source == null ? vl : await Sservice.SourceValues(elmc.Id),
                                entityname = source != null ? source.Name : null,
                                entitysourceId = source != null ? source.Id : null,
                                Hidden = Hidden,
                                ReadOnly = ReadOnly,
                                url = elmc.URL
                            };

                            el.innerelements.Add(elc);
                        }
                        Elements.Add(el);

                    }
                    DTs.elements.AddRange(Elements);
                    DTsl.Add(DTs);
                }
                //DTo
                dto.Data.Sections = DTsl;
                dto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data.Sections = DTsl;
                dto.AdditionalData = errorMsg;
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<List<Values>>> GetSourceValueByElementId(long id)//ElementId
        {
            BaseResponseDTO<List<Values>> dto = new BaseResponseDTO<List<Values>>();
            dto.Data = new List<Values>();

            try
            {
                NPF_FRM_Elements Elml = context.NPF_FRM_Elements.Where(x => x.Id == id && x.DeleteStatus == false).FirstOrDefault();
                if (Elml != null)
                {
                    dto.Data = await Sservice.SourceValues(Elml.Id);
                    dto.QryResult = new QueryResult().SUCEEDED;
                }
            }
            catch (Exception ex)
            {
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;
        }

        public async Task<BaseResponseDTO<List<CustomDictionary>>> GetCustomDetailsByElementId(DetailsByIdSearchDTO data)//ElementId
        {
            BaseResponseDTO<List<CustomDictionary>> dto = new BaseResponseDTO<List<CustomDictionary>>();
            dto.Data = new List<CustomDictionary>();

            try
            {
                NPF_FRM_Elements Elml = context.NPF_FRM_Elements.Where(x => x.Id == data.ElementId && x.DeleteStatus == false).FirstOrDefault();
                if (Elml != null)
                {
                    dto.Data = await Sservice.DetailsValues(Elml.Id, data.DetailsId);
                    dto.QryResult = new QueryResult().SUCEEDED;
                }
            }
            catch (Exception ex)
            {
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;
        }
        public async Task<string?> GetEntitiesValue(long Id, string Source)
        {
            string dt = null;
            try
            {
                switch (Source)
                {
                    case "NPF_SYS_LookUpValue":
                        dt = context.NPF_SYS_LookUpValue.Where(x => x.Id == Id).FirstOrDefault().Name;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            return dt;
        }
        #region Dynamic Data
        public async Task<BaseResponseDTO<DynamicDTO>> GetAllDynamicHeaders(string form, Guid ModuleId)
        {
            BaseResponseDTO<DynamicDTO> dto = new BaseResponseDTO<DynamicDTO>();
            dto.Data = new DynamicDTO();
            BaseResponseDTO<List<NPF_FRM_FormPage>> dl = new BaseResponseDTO<List<NPF_FRM_FormPage>>();
            List<NPF_FRM_FormPage> filtered = new List<NPF_FRM_FormPage>();
            BaseRequestDTO<DateRangeDTO> rg = new BaseRequestDTO<DateRangeDTO>();
            List<Headers> Headers = new List<Headers>();
            DataTable dtResp = new DataTable();
            string Plan = string.Empty;
            string errorMsg = "Error in Method";
            try
            {
                rg.Data = new DateRangeDTO();
                rg.Data.From = DateTime.Now.StartOfDay();
                rg.Data.To = DateTime.Now.EndOfDay();
                dl = await FilterDatatableByPeriod(rg);
                dto.Access = service.GetMenuAccessByIds(ModuleId, UM.UID);
                Plan = form;
                List<long> pl = context.NPF_FRM_Form.Where(x => x.Name.Contains(Plan) && x.DeleteStatus == false).Select(y => y.Id).ToList();
                //long FormId = context.NPF_FRM_Form.Where(x => x.Name == Plan && x.DeleteStatus == false).Select(y => y.Id).FirstOrDefault();
                List<long?> Sections = context.NPF_FRM_FormSectionMapping.Where(x => pl.Contains(x.FormId) && x.DeleteStatus == false).Select(y => (long?)y.SectionId).ToList();
                filtered = dl.Data.Where(x => pl.Contains(x.FormId)).ToList();
                List<NPF_FRM_FormPageSectionModuleMapping> LFSM = context.NPF_FRM_FormPageSectionModuleMapping.Where(x => pl.Contains(x.FormId) && x.ModuleId == ModuleId && Sections.Contains(x.SectionId) && x.DeleteStatus == false).ToList();
                List<long> LFSMIds = LFSM.Select(x => x.Id).ToList();

                var EleHideList = (from a in context.NPF_FRM_FormPageElementModuleMapping.Where(x => LFSMIds.Contains(x.Fk_FormSectionModuleId) && x.DeleteStatus == false).ToList()
                                   select new
                                   {
                                       ElId = a.ElementId,
                                       // Hidden = LFSM.Where(x => x.Id == a.Fk_FormSectionModuleId).FirstOrDefault().Hidden == true ? true : a.Hidden
                                       Hidden = a.Hidden

                                   }).ToList();
                List<long> FilteredElemHideIds = EleHideList.Where(x => x.Hidden == true).Select(y => y.ElId).ToList();
                var DictL = (from a in context.NPF_FRM_Elements.Where(x => Sections.Contains(x.ServiceId) && !FilteredElemHideIds.Contains(x.Id) && x.DeleteStatus == false && x.ParentId == null && x.IsListing == true).ToList()
                             select new
                             {
                                 Key = a.ListingName,
                                 Value = a.Name
                             }).ToList();
                Headers hrd = new Headers()
                {
                    title = "Id",
                    data = "id"
                };
                Headers.Add(hrd);
                hrd = new Headers()
                {
                    title = "Code",
                    data = "code"
                };
                Headers.Add(hrd);
                hrd = new Headers()
                {
                    title = "Date",
                    data = "dateCreated"
                };
                Headers.Add(hrd);
                dtResp.Columns.Add("id");
                dtResp.Columns.Add("code");
                dtResp.Columns.Add("dateCreated");
                foreach (var Dict in DictL)
                {
                    string Ky = Dict.Key == null || Dict.Key == string.Empty ? Dict.Value : Dict.Key;
                    hrd = new Headers()
                    {
                        title = Ky,
                        data = Ky.Replace(" ", "").ToLower()
                    };
                    Headers.Add(hrd);
                }
                hrd = new Headers()
                {
                    title = "Approval Status",
                    data = "approval"
                };
                Headers.Add(hrd);
                dtResp.Columns.Add("approval");
                dto.Data.Headers = Headers;
                dto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = new DynamicDTO();
                dto.AdditionalData = errorMsg;
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }
        public async Task<BaseResponseAccessDTO<DynamicDTO>> GetAllDynamicNew(BaseRequestDTO<DateRangeFormDTO> dt)
        {
            BaseResponseAccessDTO<DynamicDTO> dto = new BaseResponseAccessDTO<DynamicDTO>();
            dto.Data = new DynamicDTO();
            BaseResponseDTO<List<NPF_FRM_FormPage>> dl = new BaseResponseDTO<List<NPF_FRM_FormPage>>();
            List<NPF_FRM_FormPage> filtered = new List<NPF_FRM_FormPage>();
            BaseRequestDTO<DateRangeDTO> rg = new BaseRequestDTO<DateRangeDTO>();
            List<Headers> Headers = new List<Headers>();
            DataTable dtResp = new DataTable();
            string Plan = string.Empty;
            string errorMsg = "Error in Method";
            try
            {
                rg.Data = new DateRangeDTO();
                rg.Data.From = dt.Data.From;
                rg.Data.To = dt.Data.To;
                dl = await FilterDatatableByPeriod(rg);
                dto.Access = service.GetMenuAccessByIds(dt.ModuleId, UM.UID);
                Plan = dt.Data.Form;
                long Sequence = 0;
                NPF_FRM_FormModuleMappingSequence FrmSq = context.NPF_FRM_FormModuleMappingSequence.Where(x => x.Module.Id == dt.ModuleId && x.DeleteStatus == false).FirstOrDefault();
                if (FrmSq != null)
                {
                    Sequence = FrmSq.Sequence;
                }
                List<long> pl = context.NPF_FRM_Form.Where(x => x.Name.Contains(Plan) && x.DeleteStatus == false).Select(y => y.Id).ToList();
                //long FormId = context.NPF_FRM_Form.Where(x => x.Name == Plan && x.DeleteStatus == false).Select(y => y.Id).FirstOrDefault();
                List<long?> Sections = context.NPF_FRM_FormSectionMapping.Where(x => pl.Contains(x.FormId) && x.DeleteStatus == false).Select(y => (long?)y.SectionId).ToList();
                filtered = dl.Data.Where(x => pl.Contains(x.FormId)).ToList();
                List<NPF_FRM_FormPageSectionModuleMapping> LFSM = context.NPF_FRM_FormPageSectionModuleMapping.Where(x => pl.Contains(x.FormId) && x.ModuleId == dt.ModuleId && Sections.Contains(x.SectionId) && x.DeleteStatus == false).ToList();
                List<long> LFSMIds = LFSM.Select(x => x.Id).ToList();

                var EleHideList = (from a in context.NPF_FRM_FormPageElementModuleMapping.Where(x => LFSMIds.Contains(x.Fk_FormSectionModuleId) && x.DeleteStatus == false).ToList()
                                   select new
                                   {
                                       ElId = a.ElementId,
                                       Hidden = a.Hidden

                                   }).ToList();
                List<long> FilteredElemHideIds = EleHideList.Where(x => x.Hidden == true).Select(y => y.ElId).ToList();
                var DictL = (from a in context.NPF_FRM_Elements.Where(x => Sections.Contains(x.ServiceId) && !FilteredElemHideIds.Contains(x.Id) && x.DeleteStatus == false && x.ParentId == null && x.IsListing == true).ToList()
                             select new
                             {
                                 Key = a.ListingName,
                                 Value = a.Name
                             }).ToList();
                Headers hrd = new Headers()
                {
                    title = "Id",
                    data = "id"
                };
                Headers.Add(hrd);
                hrd = new Headers()
                {
                    title = "Code",
                    data = "code"
                };
                Headers.Add(hrd);
                hrd = new Headers()
                {
                    title = "Date",
                    data = "dateCreated"
                };
                Headers.Add(hrd);
                dtResp.Columns.Add("id");
                dtResp.Columns.Add("code");
                dtResp.Columns.Add("dateCreated");
                foreach (var Dict in DictL)
                {
                    string Ky = Dict.Key == null || Dict.Key == string.Empty ? Dict.Value : Dict.Key;
                    hrd = new Headers()
                    {
                        title = Ky,
                        data = Ky.Replace(" ", "").ToLower()
                    };
                    Headers.Add(hrd);
                    var dataname = Ky.Replace(" ", "").ToLower();
                    if (!dtResp.Columns.Contains(dataname))
                    {
                        dtResp.Columns.Add(Ky.Replace(" ", "").ToLower());
                    }
                }
                hrd = new Headers()
                {
                    title = "Approval Status",
                    data = "approval"
                };
                Headers.Add(hrd);
                dtResp.Columns.Add("approval");
                filtered = dl.Data.Where(x => pl.Contains(x.FormId)).ToList();
                filtered.Where(w => w.CurrentSequence == null).ToList().ForEach(i => i.CurrentSequence = 0);
                filtered = filtered.Where(x => Sequence <= x.CurrentSequence).ToList();
                #region Commented
                //if (dt.Data.ListingName == "PathologyDraftTest")
                //{
                //    filtered = filtered.Where(x => x.Status == Status.Submitted || x.Status == Status.SubmittedLvl2 || x.Status == Status.DrafLvl2 || x.Status == Status.Completed).ToList();
                //}
                //else if(dt.Data.ListingName == "PathologyTestReport")
                //{
                //    filtered = filtered.Where(x => x.Status == Status.SubmittedLvl2 || x.Status == Status.DrafLvl3 || x.Status == Status.Completed).ToList();

                //}
                //else if (dt.Data.ListingName == "Entomology33B")
                //{
                //   // filtered = filtered.Where(x => x.Status == Status.Submitted || x.Status == Status.SubmittedLvl2 || x.Status == Status.SubmittedLvl3 || x.Status == Status.DrafLvl2 || x.Status == Status.DrafLvl3 || x.Status == Status.DrafLvl4 || x.Status == Status.Completed).ToList();

                //}
                //else if (dt.Data.ListingName == "Entomology55A")
                //{
                //    filtered = filtered.Where(x =>  x.Status == Status.SubmittedLvl2 || x.Status == Status.SubmittedLvl3  || x.Status == Status.DrafLvl3 || x.Status == Status.DrafLvl4 || x.Status == Status.Completed).ToList();

                //}
                //else if (dt.Data.ListingName == "Entomology55B")
                //{
                //    filtered = filtered.Where(x =>  x.Status == Status.SubmittedLvl3 ||  x.Status == Status.DrafLvl4 || x.Status == Status.Completed).ToList();

                //}
                //else if (dt.Data.ListingName == "SoilAndWaterTestReport")
                //{
                //    filtered = filtered.Where(x => x.Status == Status.Submitted || x.Status == Status.DrafLvl2 || x.Status == Status.Completed).ToList();

                //}
                //else
                //{
                //    filtered = filtered.Where(x => x.Status != Status.SubmittedLvl2 || x.Status != Status.DrafLvl2 || x.Status == Status.DrafLvl3).ToList();

                //}
                #endregion
                foreach (NPF_FRM_FormPage l in filtered)
                {
                    try
                    {
                        DataRow row = dtResp.NewRow();
                        List<long> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
                                   where b.IsListing == true && b.DeleteStatus == false && b.ParentId == null
                                   select new
                                   {
                                       Name = b.ListingName == null || b.ListingName == "" ? a.ElementName.Replace(" ", "").ToLower() : b.ListingName.Replace(" ", "").ToLower(),
                                       a.Value,
                                       Type = b.InputTypeDetails.Name,
                                       source = b.Source,
                                       ElValue = a.AdditionalValue,
                                   }).ToList();
                        row["id"] = l.Id;
                        row["code"] = l.Code;
                        row["dateCreated"] = l.CreatedDate.ToShortDateString();
                        row["approval"] = l.Approval;
                        foreach (var Dict in DictL)
                        {
                            try
                            {
                                string Ky = Dict.Key == null || Dict.Key == string.Empty ? Dict.Value : Dict.Key;
                                Ky = Ky.Replace(" ", "").ToLower();
                                //   var temp = Wel.Where(x => x.Name == Dict.Value.Replace(" ", "").ToLower()).FirstOrDefault();
                                var temp = Wel.Where(x => x.Name == Ky).FirstOrDefault();
                                if (temp == null)
                                {
                                    row[Ky] = string.Empty;
                                }
                                else
                                {
                                    if (temp.source != null)
                                    {
                                        try
                                        {
                                            string TableName = context.NPF_FRM_Source.Where(x => x.Id == temp.source.Id).FirstOrDefault().Name;
                                            row[Ky] = await GetEntitiesValue(long.Parse(temp.Value), TableName);
                                        }
                                        catch (Exception ex)
                                        {
                                            row[Ky] = string.Empty;
                                        }
                                    }
                                    else
                                    {
                                        row[Ky] = temp.Value;
                                    }
                                }
                                if(temp.Type == "Dropdown")
                                {
                                    row[Ky] = temp.ElValue;
                                }

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        dtResp.Rows.Add(row);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                dto.Data.Rows = JsonConvert.SerializeObject(dtResp);
                dto.Data.Headers = Headers;
                dto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = new DynamicDTO();
                dto.AdditionalData = errorMsg;
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }
        #endregion
        #region OLD
        //public async Task<BaseResponseDTO<string>> GetAllDynamicFCLVStats(BaseRequestDTO<DateRangeFormDTO> dt)
        //{
        //    BaseResponseDTO<string> dto = new BaseResponseDTO<string>();
        //    dto.Data = string.Empty;
        //    BaseResponseDTO<List<NPF_FRM_FormPage>> dl = new BaseResponseDTO<List<NPF_FRM_FormPage>>();
        //    List<NPF_FRM_FormPage> filtered = new List<NPF_FRM_FormPage>();
        //    BaseRequestDTO<DateRangeDTO> rg = new BaseRequestDTO<DateRangeDTO>();
        //    string Plan = string.Empty;
        //    string errorMsg = "Error in Method";
        //    try
        //    {
        //        rg.Data = new DateRangeDTO();
        //        rg.Data.From = dt.Data.From;
        //        rg.Data.To = dt.Data.To;
        //        dl = await FilterDatatableByPeriod(rg);
        //        Plan = dt.Data.Plan;
        //        NPF_FRM_Form pl = context.NPF_FRM_Form.Where(x => x.Name == Plan && x.DeleteStatus == false).FirstOrDefault();
        //        if (pl != null)
        //        {
        //            switch (Plan)
        //            {
        //                case "Soil and Water Test ":
        //                    List<SoilandWaterLabListDTO> List = new List<SoilandWaterLabListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        SoilandWaterLabListDTO d = new SoilandWaterLabListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.RequestCode = l.Code;
        //                        d.RequesterCategory = Wel.Where(y => y.Name == "requestercategory").FirstOrDefault().Value;
        //                        d.Date = l.CreatedDate.ToShortDateString();
        //                        d.FirstName = Wel.Where(y => y.Name == "firstname").FirstOrDefault().Value;
        //                        d.LastName = Wel.Where(y => y.Name == "surname").FirstOrDefault().Value;
        //                        d.SampleType = Wel.Where(y => y.Name == "samplingtype").FirstOrDefault().Value;
        //                        d.Status = l.Status.ToString();
        //                        d.OfficerName = "";
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        List.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(List);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Seed Request Form":
        //                    List<SeedsRequestListDTO> AList = new List<SeedsRequestListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        SeedsRequestListDTO d = new SeedsRequestListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.Crop = Wel.Where(y => y.Name == "crop").FirstOrDefault().Value;
        //                        d.Date = l.CreatedDate.ToShortDateString();
        //                        d.SerialNumber = "";
        //                        d.Name = Wel.Where(y => y.Name == "name").FirstOrDefault().Value;
        //                        d.PlanterAddress = Wel.Where(y => y.Name == "residentialaddress").FirstOrDefault().Value;
        //                        d.Location = Wel.Where(y => y.Name == "location").FirstOrDefault().Value;
        //                        d.Variety = Wel.Where(y => y.Name == "variety").FirstOrDefault().Value;
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        AList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(AList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Receipt of Umbels Form":
        //                    List<ReceiptofUmbelListDTO> BList = new List<ReceiptofUmbelListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        ReceiptofUmbelListDTO d = new ReceiptofUmbelListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.DeliveryDate = Wel.Where(y => y.Name == "dateofdelivery").FirstOrDefault().Value;
        //                        d.SerialNumber = l.Code;
        //                        d.Name = Wel.Where(y => y.Name == "name").FirstOrDefault().Value;
        //                        d.BatchNumber = Wel.Where(y => y.Name == "batchnumber").FirstOrDefault().Value;
        //                        d.NoBags = Wel.Where(y => y.Name == "no.ofbags").FirstOrDefault().Value;
        //                        d.Amount = Wel.Where(y => y.Name == "amountofumbels").FirstOrDefault().Value;
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        BList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(BList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Seed Processing Form":
        //                    List<SeedProcessingListDTO> CList = new List<SeedProcessingListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        SeedProcessingListDTO d = new SeedProcessingListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.Date = l.CreatedDate.ToShortDateString();
        //                        d.SerialNumber = l.Code;
        //                        d.Name = Wel.Where(y => y.Name == "name").FirstOrDefault().Value;
        //                        d.BatchNumber = Wel.Where(y => y.Name == "batchnumber").FirstOrDefault().Value;
        //                        d.TotalAmount = Wel.Where(y => y.Name == "totalamountofseedsprocessed").FirstOrDefault().Value;
        //                        d.AmountMarketable = Wel.Where(y => y.Name == "amountofmarketableseedsprocessed").FirstOrDefault().Value;
        //                        d.AmountUnmarketable = Wel.Where(y => y.Name == "amountofunmarketableseedsprocessed").FirstOrDefault().Value;
        //                        d.AmountAMD = Wel.Where(y => y.Name == "amountofseedssenttoamb").FirstOrDefault().Value;
        //                        d.AmountProducer = Wel.Where(y => y.Name == "amountofseedssenttoamb").FirstOrDefault().Value;
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        CList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(CList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Mother Plant Collection":
        //                    List<MotherPlantCollectionListDTO> DList = new List<MotherPlantCollectionListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        MotherPlantCollectionListDTO d = new MotherPlantCollectionListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.Date = Wel.Where(y => y.Name == "date").FirstOrDefault().Value;
        //                        d.Crop = Wel.Where(y => y.Name == "crop").FirstOrDefault().Value;
        //                        d.Variety = Wel.Where(y => y.Name == "variety").FirstOrDefault().Value;
        //                        d.OriginatedFrom = Wel.Where(y => y.Name == "originatedform").FirstOrDefault().Value;
        //                        d.Quantity = "";
        //                        d.Liason = "";
        //                        d.RecievingOfficer = Wel.Where(y => y.Name == "receivingofficer").FirstOrDefault().Value;
        //                        d.Officer = context.Users.Where(x => x.UserToken == l.CreatedBy).FirstOrDefault().FullName;
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        DList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(DList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Daily Record Book":
        //                    List<DailyRecordBookListDTO> EList = new List<DailyRecordBookListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        DailyRecordBookListDTO d = new DailyRecordBookListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.Date = Wel.Where(y => y.Name == "date").FirstOrDefault().Value;
        //                        d.Crop = Wel.Where(y => y.Name == "cropname").FirstOrDefault().Value;
        //                        d.ProcessType = Wel.Where(y => y.Name == "processtype").FirstOrDefault().Value;
        //                        d.MStage = "";
        //                        d.MediaUsed = Wel.Where(y => y.Name == "mediaused").FirstOrDefault().Value; ;
        //                        d.Status = l.Status.ToString();
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        EList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(EList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Application for Purchase of Tissue Culture":
        //                    List<ApplicationforPurchaseofTissueCulturePlantListDTO> FList = new List<ApplicationforPurchaseofTissueCulturePlantListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        ApplicationforPurchaseofTissueCulturePlantListDTO d = new ApplicationforPurchaseofTissueCulturePlantListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.DateApplication = l.CreatedDate.ToString();
        //                        d.Crop = Wel.Where(y => y.Name == "crop").FirstOrDefault().Value;
        //                        d.Name = Wel.Where(y => y.Name == "name").FirstOrDefault().Value;
        //                        d.DateSupplied = Wel.Where(y => y.Name == "datesupplied").FirstOrDefault().Value; ;
        //                        d.TelNo = Wel.Where(y => y.Name == "telephonenumber").FirstOrDefault().Value;
        //                        d.Address = Wel.Where(y => y.Name == "address").FirstOrDefault().Value;
        //                        d.Type = Wel.Where(y => y.Name == "type").FirstOrDefault().Value;
        //                        d.Variety = Wel.Where(y => y.Name == "variety").FirstOrDefault().Value;
        //                        d.QuantityRequested = Wel.Where(y => y.Name == "quantityrequested").FirstOrDefault().Value;
        //                        d.Region = Wel.Where(y => y.Name == "regiontobeplanted").FirstOrDefault().Value;
        //                        d.QuantitySupplied = Wel.Where(y => y.Name == "quantitysupplied").FirstOrDefault().Value;
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        FList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(FList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Sample Rejection Log Book":
        //                    List<SampleRejectionLogBookListDTO> GList = new List<SampleRejectionLogBookListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        SampleRejectionLogBookListDTO d = new SampleRejectionLogBookListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.Date = Wel.Where(y => y.Name == "date").FirstOrDefault().Value;
        //                        d.Name = "";
        //                        d.SampleDetails = Wel.Where(y => y.Name == "sampledetails").FirstOrDefault().Value;
        //                        d.ReasonForRejection = Wel.Where(y => y.Name == "reasonforrejection").FirstOrDefault().Value;
        //                        d.ActionTaken = Wel.Where(y => y.Name == "actiontaken").FirstOrDefault().Value;
        //                        d.DiposedMeeans = Wel.Where(y => y.Name == "disposedbymeansnecessary").FirstOrDefault().Value;
        //                        d.Remarks = Wel.Where(y => y.Name == "remarks").FirstOrDefault().Value;
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        GList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(GList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Sample Allocation Sheet":
        //                    List<PlantSampleAllocationSheetListDTO> HList = new List<PlantSampleAllocationSheetListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        PlantSampleAllocationSheetListDTO d = new PlantSampleAllocationSheetListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id; ;
        //                        d.Name = "nameofpersonnel[rs/srs]";
        //                        d.Monday = "";
        //                        d.Tuesday = "";
        //                        d.Wednesday = "";
        //                        d.Thurday = "";
        //                        d.Friday = "";
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        HList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(HList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Specimen Proforma":
        //                    List<SpecimenProformaDTO> IList = new List<SpecimenProformaDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        SpecimenProformaDTO d = new SpecimenProformaDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.SerialNumber = l.Code.ToString();
        //                        d.Date = Wel.Where(y => y.Name == "date").FirstOrDefault().Value;
        //                        d.CaseRef = Wel.Where(y => y.Name == "caserefno").FirstOrDefault().Value;
        //                        d.Customer = "";
        //                        d.Planter = "";
        //                        d.DeliveryMode = "";
        //                        d.Officer = Wel.Where(y => y.Name == "officer").FirstOrDefault().Value;
        //                        d.Sample = Wel.Where(y => y.Name == "conditionofsample").FirstOrDefault().Value;
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        IList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(IList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Test Report Form Pathology":
        //                    List<TestReportLogBookListDTO> JList = new List<TestReportLogBookListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        TestReportLogBookListDTO d = new TestReportLogBookListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.CRN = Wel.Where(y => y.Name == "caserefno").FirstOrDefault().Value;
        //                        d.Customer = Wel.Where(y => y.Name == "nameofcustomer").FirstOrDefault().Value;
        //                        d.Planter = Wel.Where(y => y.Name == "nameofplanter").FirstOrDefault().Value;
        //                        d.Variety = Wel.Where(y => y.Name == "variety").FirstOrDefault().Value; ;
        //                        d.TestMethod = Wel.Where(y => y.Name == "testmethodsused:mstrc_").FirstOrDefault().Value;
        //                        d.TestSignatory = "";
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        JList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(JList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Draft Test Report":
        //                    List<DraftTestReportListDTO> KList = new List<DraftTestReportListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        DraftTestReportListDTO d = new DraftTestReportListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.CRN = Wel.Where(y => y.Name == "casereferenceno.(crn)").FirstOrDefault().Value;
        //                        d.Date = l.CreatedDate.ToString();
        //                        d.Crop = "";
        //                        d.Variety = "";
        //                        d.TestMethod = "";
        //                        d.TestSignatory = "";
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        KList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(KList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Diagnosis Request Form 33a":
        //                    List<DiagnosisRequest33aListDTO> LList = new List<DiagnosisRequest33aListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        DiagnosisRequest33aListDTO d = new DiagnosisRequest33aListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.CaseRef = "";
        //                        d.Date = Wel.Where(y => y.Name == "date").FirstOrDefault().Value;
        //                        d.RequesterName = Wel.Where(y => y.Name == "name").FirstOrDefault().Value;
        //                        d.RequesterCategory = Wel.Where(y => y.Name == "requestercategory").FirstOrDefault().Value; ;
        //                        d.Planter = "";
        //                        d.DeliveryMode = "";
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        LList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(LList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Diagnosis Request Form 33b":
        //                    List<DiagnosisRequest33bListDTO> MList = new List<DiagnosisRequest33bListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        DiagnosisRequest33bListDTO d = new DiagnosisRequest33bListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.CaseRef = Wel.Where(y => y.Name == "caserefno(crn):el/*").FirstOrDefault().Value;
        //                        d.Date = Wel.Where(y => y.Name == "dateofapplicationoftreatment").FirstOrDefault().Value;
        //                        d.Crop = Wel.Where(y => y.Name == "crop").FirstOrDefault().Value;
        //                        d.SampleForwarded = Wel.Where(y => y.Name == "sampleforwarded").FirstOrDefault().Value;
        //                        d.SampleSymptoms = Wel.Where(y => y.Name == "samplesymptoms").FirstOrDefault().Value;
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        MList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(MList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Sample Allocation":
        //                    List<EntomologySampleAllocationSheetListDTO> NList = new List<EntomologySampleAllocationSheetListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        EntomologySampleAllocationSheetListDTO d = new EntomologySampleAllocationSheetListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.Name = Wel.Where(y => y.Name == "nameofpersonnel[rs/srs]").FirstOrDefault().Value;
        //                        d.Monday = "";
        //                        d.Tuesday = "";
        //                        d.Wednesday = "";
        //                        d.Thurday = "";
        //                        d.Friday = "";
        //                        d.Total = Wel.Where(y => y.Name == "total").FirstOrDefault().Value;
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        NList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(NList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                case "Diagnosis Test Result 55a ":
        //                    List<DiagnosisTestResult55aListDTO> OList = new List<DiagnosisTestResult55aListDTO>();
        //                    filtered = dl.Data.Where(x => x.FormId == pl.Id).ToList();
        //                    foreach (NPF_FRM_FormPage l in filtered)
        //                    {
        //                        DiagnosisTestResult55aListDTO d = new DiagnosisTestResult55aListDTO();
        //                        List<Guid> Wguids = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == l.Id && x.DeleteStatus == false).Select(xx => xx.Id).ToList();
        //                        var Wel = (from a in context.NPF_FRM_WOElementMapping.Where(y => Wguids.Contains(y.WOId)).ToList()
        //                                   join b in context.NPF_FRM_Elements on a.ElementId equals b.Id
        //                                   where b.IsListing == true && b.DeleteStatus == false
        //                                   select new
        //                                   {
        //                                       Name = a.ElementName.Replace(" ", "").ToLower(),
        //                                       a.Value
        //                                   }).ToList();
        //                        d.Id = l.Id;
        //                        d.CaseRef = Wel.Where(y => y.Name == "caserefno(crn/el*").FirstOrDefault().Value;
        //                        d.SampleCollected = Wel.Where(y => y.Name == "originofsamplecollected(location)").FirstOrDefault().Value;
        //                        d.SampleSubmitted = Wel.Where(y => y.Name == "samplesubmitted").FirstOrDefault().Value;
        //                        d.DamageSymptoms = Wel.Where(y => y.Name == "damagesymptomsonsample").FirstOrDefault().Value;
        //                        d.CollectionDate = Wel.Where(y => y.Name == "samplecollectiondate").FirstOrDefault().Value;
        //                        d.DiagnosisDate = Wel.Where(y => y.Name == "diagnosisdate").FirstOrDefault().Value;
        //                        d.PestMethod = Wel.Where(y => y.Name == "pestidentificationmethods").FirstOrDefault().Value;
        //                        d.IdentificationOutcome = Wel.Where(y => y.Name == "identificationsoutcome").FirstOrDefault().Value;
        //                        d.OpinionInter = Wel.Where(y => y.Name == "opinionandinterpretation(ifany)").FirstOrDefault().Value;
        //                        d.Recommendation = Wel.Where(y => y.Name == "recommendation").FirstOrDefault().Value;
        //                        d.ApprovalStatus = l.Status.ToString();
        //                        OList.Add(d);
        //                    }
        //                    dto.Data = JsonConvert.SerializeObject(OList);
        //                    dto.QryResult = new QueryResult().SUCEEDED;
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        dto.Data = string.Empty;
        //        dto.AdditionalData = errorMsg;
        //        dto.QryResult = new QueryResult().FAILED;
        //    }
        //    return dto;
        //}
        #endregion


        #region Activity//// No controller for below Services
        public BaseResponseDTO<List<DropDown>> GetSchemeListValuesById(long FormId)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            MatrixScheme MS = new MatrixScheme();
            string errorMsg = "No Data Found";

            try
            {
                List<NPF_FRM_FormPage> Pll = context.NPF_FRM_FormPage.Where(x => x.DeleteStatus == false && x.FormId == FormId).ToList();
                Dd = new DropDown();
                Dd.title = "SchemeList";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_FormPage pl in Pll)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = pl.Id;
                    Ddi.text = pl.Code;
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
        public BaseResponseDTO<List<DropDown>> GetSchemeNames()
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            UserMatrixDTO UM = new UserMatrixDTO();
            MatrixScheme MS = new MatrixScheme();
            string errorMsg = "No Data Found";

            try
            {
                List<NPF_FRM_Form> Pll = context.NPF_FRM_Form.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Forms";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_Form pl in Pll)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = pl.Id;
                    Ddi.text = pl.Name;
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
        public BaseResponseDTO<List<DropDown>> GetSectionById(long SchemeId)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            UserMatrixDTO UM = new UserMatrixDTO();
            MatrixScheme MS = new MatrixScheme();
            string errorMsg = "No Data Found";

            try
            {
                List<NPF_FRM_WorkOrder> Pll = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == SchemeId && x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Forms";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_WorkOrder pl in Pll)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = pl.Id;
                    Ddi.text = pl.SectionName;
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
        #endregion
    }
}
