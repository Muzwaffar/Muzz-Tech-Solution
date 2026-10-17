using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using BL.Models.Template;
using BL.Models.Workflow;
using BL.Services.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models.Access;
using DAL.Models.Activities;
using DAL.Models.Form;
using DAL.Models.Template;
using DAL.Models.WorkFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Workflow.WorkflowMapping
{
    public class WorkflowMappingService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private MatrixService Mxservice;
        public UserMatrixDTO UM = new UserMatrixDTO();
        public WorkflowMappingService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Mxservice = new MatrixService();
            UM = Mxservice.sGetMatrixUserByUserID();
        }

        public BaseResponseDTO<List<WorkflowMappingListDTO>> GetAll()
        {
            BaseResponseDTO<List<WorkflowMappingListDTO>> dto = new BaseResponseDTO<List<WorkflowMappingListDTO>>();
            List<WorkflowMappingListDTO> result = new List<WorkflowMappingListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = result = (from a in context.NPF_SYS_WorkFlowHeader
                                   where a.DeleteStatus == false
                                   select new WorkflowMappingListDTO()
                                   {
                                       Id = a.Id,
                                       Code = a.Code,
                                       Name = a.Name,
                                       WFType = a.WFType.ToString(),
                                       AttributeType = a.AttributeType.ToString(),
                                       Module = context.NPF_SYS_Modules.Where(x => x.Id == a.ModuleId).FirstOrDefault().Name,
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

        public BaseResponseDTO<List<DropDownLogin>> GetAllDropDownValues()
        {
            BaseResponseDTO<List<DropDownLogin>> dto = new BaseResponseDTO<List<DropDownLogin>>();
            List<DropDownLogin> Ddl = new List<DropDownLogin>();
            DropDownLogin Dd = new DropDownLogin();
            string errorMsg = "No Data Found";

            try
            {
                //Module List
                List<NPF_SYS_Modules> mdl = context.NPF_SYS_Modules.Where(x => x.DeleteStatus == false && x.Url !="").ToList();
                Dd = new DropDownLogin();
                Dd.title = "Module";
                Dd.items = new List<DropDownItemLogin>();
                foreach (NPF_SYS_Modules md in mdl)
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    Ddi.id = md.Id.ToString();
                    Ddi.text = md.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //AttributeType List
                Dd = new DropDownLogin();
                Dd.title = "AttributeType";
                Dd.items = new List<DropDownItemLogin>();
                foreach (AttributeType st in Enum.GetValues(typeof(AttributeType)))
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    var number = (int)st;
                    Ddi.id = number.ToString();
                    Ddi.text = st.ToString();
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //CaseStatus Type
                Dd = new DropDownLogin();
                Dd.title = "CaseStatus";
                Dd.items = new List<DropDownItemLogin>();
                foreach (CaseStatus st in Enum.GetValues(typeof(CaseStatus)))
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    var number = (int)st;
                    Ddi.id = number.ToString();
                    Ddi.text = st.ToString();
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //WFType Type
                Dd = new DropDownLogin();
                Dd.title = "WFType";
                Dd.items = new List<DropDownItemLogin>();
                foreach (WFType st in Enum.GetValues(typeof(WFType)))
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    var number = (int)st;
                    Ddi.id = number.ToString();
                    Ddi.text = st.ToString();
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //Activity Type
                List<NPF_ACT_ActivityEventType> aet = context.NPF_ACT_ActivityEventType.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDownLogin();
                Dd.title = "ActivityType";
                Dd.items = new List<DropDownItemLogin>();
                foreach (NPF_ACT_ActivityEventType ae in aet)
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    Ddi.id = ae.Id.ToString();
                    Ddi.text = ae.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //AType
                Dd = new DropDownLogin();
                Dd.title = "AType";
                Dd.items = new List<DropDownItemLogin>();
                foreach (AType st in Enum.GetValues(typeof(AType)))
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    var number = (int)st;
                    Ddi.id = number.ToString();
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

        public BaseResponseDTO<string> GetURLName(string Id)//FormId
        {
            BaseResponseDTO<string> dto = new BaseResponseDTO<string>();
            string result = string.Empty;
            string errorMsg = "No Data Found";
            try
            {

                var urlName = context.NPF_SYS_Modules.Where(x => x.Id.ToString() == Id).FirstOrDefault().Url;
                if (urlName != "")
                {
                    result = urlName.Split("/")[0];
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

        public BaseResponseDTO<WorkflowMappingCruDTO> GetById(long Id)
        {
            BaseResponseDTO<WorkflowMappingCruDTO> dto = new BaseResponseDTO<WorkflowMappingCruDTO>();
            WorkflowMappingCruDTO result = new WorkflowMappingCruDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_WorkFlowHeader
                          where a.Id == Id && a.DeleteStatus == false
                          select new WorkflowMappingCruDTO()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              ModuleId = a.ModuleId,
                              Code = a.Code,
                              WFType = (long)a.WFType,
                              AttributeType = (long)a.AttributeType,
                              CaseStatus = (long)a.CStatus
                          }).FirstOrDefault();
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<WorkflowMappingDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            BaseResponseDTO<string> BaseDtoSs = new BaseResponseDTO<string>();
            try
            {
                BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_WorkFlowHeader");
                NPF_SYS_WorkFlowHeader dt = new NPF_SYS_WorkFlowHeader()
                {
                    Code = BaseDtoS.Data,
                    Name = dataToSave.Data.Name,
                    CStatus = (CaseStatus)dataToSave.Data.CaseStatus,
                    WFType = (WFType)dataToSave.Data.WFType,
                    Sequence = dataToSave.Data.Sequence,
                    AttributeType = (AttributeType)dataToSave.Data.AttributeType,
                    ModuleId = Guid.Parse(dataToSave.Data.ModuleId)
                };
                context.NPF_SYS_WorkFlowHeader.Add(dt);
                context.SaveChanges();
                BaseDtoSs = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_WorkFlowDetails");
                foreach (WorkflowMappingDetailsDTO elm in dataToSave.Data.Details)
                {
                    NPF_SYS_WorkFlowDetails wfd = new NPF_SYS_WorkFlowDetails();
                    wfd.FkWFHId = dt.Id;
                    wfd.Code = BaseDtoSs.Data;
                    wfd.Name = elm.Name;
                    wfd.Description = elm.Description;
                    wfd.Ext1 = elm.Ext1;
                    wfd.Ext2 = elm.Ext2;
                    wfd.CStatus = (CaseStatus)elm.CaseStatusId;
                    wfd.AType = (AType)elm.ATypeId;
                    wfd.WFType = (WFType)elm.WFTypeId;
                    wfd.Sequence = elm.Sequence;
                    wfd.ActivityType = elm.ActivityTypeId;
                    wfd.FormSequence = elm.FormSequence;
                    wfd.ModuleId = Guid.Parse(elm.ModuleIdCurrent);
                    wfd.Message = elm.Message;
                    context.NPF_SYS_WorkFlowDetails.Add(wfd);
                    context.SaveChanges();

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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<WorkflowMappingDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoSs = new BaseResponseDTO<string>();

            try
            {
                NPF_SYS_WorkFlowHeader dt = context.NPF_SYS_WorkFlowHeader.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                context.NPF_SYS_WorkFlowHeader.Update(dt);
                context.SaveChanges();

                List<NPF_SYS_WorkFlowDetails> Currentls = context.NPF_SYS_WorkFlowDetails.Where(x => x.FkWFHId == dt.Id && x.DeleteStatus == false).ToList();
                foreach (WorkflowMappingDetailsDTO elm in dataToUpdate.Data.Details)
                {
                    if (Currentls.Any(y => y.Id == elm.Id))
                    {
                        Currentls.RemoveAll(x => x.Id == elm.Id);
                    }
                    else
                    {
                        BaseDtoSs = await new GenerateCode().GCodeMain(dataToUpdate.userToken, "NPF_SYS_WorkFlowDetails");
                        NPF_SYS_WorkFlowDetails wfd = new NPF_SYS_WorkFlowDetails();
                        wfd.FkWFHId = dt.Id;
                        wfd.Code = BaseDtoSs.Data;
                        wfd.Name = elm.Name;
                        wfd.Description = elm.Description;
                        wfd.Ext1 = elm.Ext1;
                        wfd.Ext2 = elm.Ext2;
                        wfd.CStatus = (CaseStatus)elm.CaseStatusId;
                        wfd.AType = (AType)elm.ATypeId;
                        wfd.WFType = (WFType)elm.WFTypeId;
                        wfd.Sequence = elm.Sequence;
                        wfd.ActivityType = elm.ActivityTypeId;
                        wfd.FormSequence = elm.FormSequence;
                        wfd.ModuleId = Guid.Parse(elm.ModuleIdCurrent);
                        wfd.Message = elm.Message;
                        context.NPF_SYS_WorkFlowDetails.Add(wfd);
                        context.SaveChanges();
                    }
                }
                if (Currentls.Count > 0)
                {
                    foreach (NPF_SYS_WorkFlowDetails d in Currentls)
                    {
                        d.DeleteStatus = true;
                        context.NPF_SYS_WorkFlowDetails.Update(d);
                        context.SaveChanges();
                    }
                }

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

        public async Task<BaseResponseDTO<List<WorkflowMappingDetailsDTO>>> GetTableById(long Id)
        {
            BaseResponseDTO<List<WorkflowMappingDetailsDTO>> BaseDto = new BaseResponseDTO<List<WorkflowMappingDetailsDTO>>();

            try
            {
                BaseDto.Data = (from b in context.NPF_SYS_WorkFlowDetails
                                where b.FkWFHId == Id && b.DeleteStatus == false
                                select new WorkflowMappingDetailsDTO()
                                {
                                    Id = b.Id,
                                    Name = b.Name,
                                    Description = b.Description,
                                    Ext1 = b.Ext1,
                                    ModuleNext = context.NPF_SYS_Modules.Where(x => x.DeleteStatus == false && x.Id.ToString() == b.Ext1).FirstOrDefault().Name,
                                    Ext2 = b.Ext2,
                                    CaseStatusId = (long)b.CStatus,
                                    CaseStatus = b.CStatus.ToString(),
                                    ATypeId = (long)b.AType,
                                    AType = b.AType.ToString(),
                                    WFTypeId = (long)b.WFType,
                                    WFType = b.WFType.ToString(),
                                    Sequence = b.Sequence,
                                    ActivityTypeId = (long)b.ActivityType,
                                    ActivityType = context.NPF_ACT_ActivityEventType.Where(x => x.DeleteStatus == false && x.Id == b.ActivityType).FirstOrDefault().Name,
                                    FormSequence = b.FormSequence,
                                    ModuleIdCurrent = b.ModuleId.ToString(),
                                    ModuleCurrent = context.NPF_SYS_Modules.Where(x => x.DeleteStatus == false && x.Id == b.ModuleId).FirstOrDefault().Name,
                                    Message = b.Message

                                }).ToList();
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = new List<WorkflowMappingDetailsDTO>();
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }

        public BaseResponseDTO<bool> Delete(BaseRequestDTO<long> dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_SYS_WorkFlowHeader upd = context.NPF_SYS_WorkFlowHeader.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_WorkFlowHeader.Update(upd);
                context.SaveChanges();

                List<NPF_SYS_WorkFlowDetails> Currentls = context.NPF_SYS_WorkFlowDetails.Where(x => x.FkWFHId == dataToDelId.Data).ToList();
                foreach (NPF_SYS_WorkFlowDetails d in Currentls)
                {
                    d.DeleteStatus = true;
                    context.NPF_SYS_WorkFlowDetails.Update(d);
                    context.SaveChanges();
                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Deleting Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
    }
}
