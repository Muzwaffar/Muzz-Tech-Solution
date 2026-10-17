using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using BL.Models.Form;
using BL.Models.Template;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Access;
using DAL.Models.Form;
using DAL.Models.Template;

namespace BL.Services.Form.FormConfig
{
    public class FormModuleMappingSequenceService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private RowAccessService service;
        private MatrixService Mxservice;
        public UserMatrixDTO UM = new UserMatrixDTO();
        public FormModuleMappingSequenceService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            service = new RowAccessService();
            Mxservice = new MatrixService();
            UM = Mxservice.sGetMatrixUserByUserID();
        }

        public BaseResponseDTO<List<FormModuleMappingSequenceListDTO>> GetAll()
        {
            BaseResponseDTO<List<FormModuleMappingSequenceListDTO>> dto = new BaseResponseDTO<List<FormModuleMappingSequenceListDTO>>();
            List<FormModuleMappingSequenceListDTO> result = new List<FormModuleMappingSequenceListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_FRM_FormModuleMappingSequence
                          where a.DeleteStatus == false
                          select new FormModuleMappingSequenceListDTO()
                          {
                              Id = a.Id,
                              Module = a.Module.Name,
                              FormName = a.Form.Name,
                              Sequence = a.Sequence,
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

                //Form List
                List<NPF_FRM_Form> Pll = context.NPF_FRM_Form.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDownLogin();
                Dd.title = "Form";
                Dd.items = new List<DropDownItemLogin>();
                foreach (NPF_FRM_Form pl in Pll)
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    Ddi.id = pl.Id.ToString();
                    Ddi.text = pl.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //Module List
                List<NPF_SYS_Modules> mdl = context.NPF_SYS_Modules.Where(x => x.DeleteStatus == false && x.Url != "").ToList();
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
        public BaseResponseDTO<FormModuleMappingSequenceDTO> GetById(long Id)
        {
            BaseResponseDTO<FormModuleMappingSequenceDTO> dto = new BaseResponseDTO<FormModuleMappingSequenceDTO>();
            FormModuleMappingSequenceDTO result = new FormModuleMappingSequenceDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_FRM_FormModuleMappingSequence
                          where a.Id == Id
                          select new FormModuleMappingSequenceDTO()
                          {
                              Id = a.Id,
                              Module = a.Module.Id,
                              Form = a.Form.Id,
                              Sequence = a.Sequence,
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
        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<FormModuleMappingSequenceDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();

            try
            {
                NPF_FRM_FormModuleMappingSequence dt = new NPF_FRM_FormModuleMappingSequence()
                {
                    Module = context.NPF_SYS_Modules.Where(x => x.Id == dataToSave.Data.Module && x.DeleteStatus == false).FirstOrDefault(),
                    Form = context.NPF_FRM_Form.Where(x => x.Id == dataToSave.Data.Form && x.DeleteStatus == false).FirstOrDefault(),
                    Sequence = dataToSave.Data.Sequence
                };
                context.NPF_FRM_FormModuleMappingSequence.Add(dt);
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<FormModuleMappingSequenceDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();

            try
            {
                NPF_FRM_FormModuleMappingSequence dt = context.NPF_FRM_FormModuleMappingSequence.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                dt.Module = context.NPF_SYS_Modules.Where(x => x.Id == dataToUpdate.Data.Module && x.DeleteStatus == false).FirstOrDefault();
                dt.Form = context.NPF_FRM_Form.Where(x => x.Id == dataToUpdate.Data.Form && x.DeleteStatus == false).FirstOrDefault();
                dt.Sequence = dataToUpdate.Data.Sequence;
                context.NPF_FRM_FormModuleMappingSequence.Update(dt);
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

        public BaseResponseDTO<bool> Delete(BaseRequestDTO<long> dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_FRM_FormModuleMappingSequence upd = context.NPF_FRM_FormModuleMappingSequence.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_FRM_FormModuleMappingSequence.Update(upd);
                context.SaveChanges();
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
