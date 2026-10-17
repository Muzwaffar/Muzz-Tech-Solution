using BL.Constants;
using BL.Models.Form;
using BL.Models.Common;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using DAL.Models.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remotion.Collections;

namespace BL.Services.Form
{
    public class FormService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public FormService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<FormListDTO>> GetAll()
        {
            BaseResponseDTO<List<FormListDTO>> dto = new BaseResponseDTO<List<FormListDTO>>();
            List<FormListDTO> result = new List<FormListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_FRM_Form
                          where a.DeleteStatus == false
                          select new FormListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              StartDate = a.StartDate,
                              EndDate = a.EndDate,
                              FormTypeName = a.FormType.Name,

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

        public BaseResponseDTO<FormDTO> GetById(long Id)
        {
            BaseResponseDTO<FormDTO> dto = new BaseResponseDTO<FormDTO>();
            FormDTO result = new FormDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_FRM_Form
                          where a.Id == Id
                          select new FormDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              StartDate = a.StartDate,
                              EndDate = a.EndDate,
                              FormTypeId = a.FormType.Id
                          }).FirstOrDefault();
                if (result == null)
                {
                    dto.Data = result;
                    dto.QryResult = queryResult.SUCEEDED;
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<FormSectionDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_FRM_Form.Any(x => x.Name.ToLower() == dataToSave.Data.Form.Name.ToLower() && x.FormType.Id == dataToSave.Data.Form.FormType.Id && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_FRM_Form");
                    dataToSave.Data.Form.Code = BaseDtoS.Data;// "PLN" + 
                    NPF_FRM_FormType pt = context.NPF_FRM_FormType.Where(x => x.Id == dataToSave.Data.Form.FormType.Id).FirstOrDefault();
                    context.NPF_FRM_FormType.Update(pt);
                    dataToSave.Data.Form.FormType = pt;
                    context.NPF_FRM_Form.Add(dataToSave.Data.Form);
                    context.SaveChanges();
                    if (dataToSave.Data.Sec.Count > 0)
                    {
                        foreach (var ser in dataToSave.Data.Sec)
                        {
                            NPF_FRM_FormSectionMapping psm = new NPF_FRM_FormSectionMapping()
                            {
                                FormId = dataToSave.Data.Form.Id,
                                SectionId = ser.Id,
                                Order = ser.Order
                            };
                            context.NPF_FRM_FormSectionMapping.Add(psm);
                            context.SaveChanges();
                        }

                    }

                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;

                   
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Form Already Exists";
                    BaseDto.QryResult = queryResult.FAILED;
                }
            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<FormSectionDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_FRM_Form.Any(x => x.Name.ToLower() == dataToUpdate.Data.Form.Name.ToLower() && x.Id != dataToUpdate.Data.Form.Id && x.DeleteStatus != true))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Form Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                else
                {
                    List<NPF_FRM_FormSectionMapping> psml = context.NPF_FRM_FormSectionMapping.Where(x => x.FormId == dataToUpdate.Data.Form.Id && x.DeleteStatus == false).ToList();
                    if (dataToUpdate.Data.Sec != null)
                    {
                        foreach (NPF_FRM_Section se in dataToUpdate.Data.Sec.Where(x => x.check == true).ToList())
                        {
                            var check = psml.Where(x => x.SectionId == se.Id).FirstOrDefault();
                            if (check == null)
                            {
                                NPF_FRM_FormSectionMapping sem = new NPF_FRM_FormSectionMapping()
                                {
                                    SectionId = se.Id,
                                    FormId = dataToUpdate.Data.Form.Id,
                                    Order = se.Order
                                };
                                context.NPF_FRM_FormSectionMapping.Add(sem);
                                context.SaveChanges();
                            }
                            else
                            {
                                check.Order = se.Order;
                                context.NPF_FRM_FormSectionMapping.Update(check);
                                context.SaveChanges();
                                psml.Remove(check);
                            }

                        }

                    }
                    if (psml != null)
                    {
                        foreach (NPF_FRM_FormSectionMapping ps in psml)
                        {
                            ps.DeleteStatus = true;
                            context.NPF_FRM_FormSectionMapping.Update(ps);
                            context.SaveChanges();
                        }

                    }
                    dataToUpdate.Data.Form.FormType = context.NPF_FRM_FormType.Where(x => x.Id == dataToUpdate.Data.Form.FormType.Id).FirstOrDefault();
                    context.NPF_FRM_Form.Update(dataToUpdate.Data.Form);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.QryResult = new QueryResult().SUCEEDED;
                }

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
                NPF_FRM_Form upd = context.NPF_FRM_Form.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_FRM_Form.Update(upd);
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
        public BaseResponseDTO<List<DropDown>> GetAllDropDownValues()
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                BaseRequestDTO<string> dt = new BaseRequestDTO<string>();
               
                List<NPF_FRM_FormType> FT = context.NPF_FRM_FormType.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "FormType";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_FormType F in FT)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = F.Id;
                    Ddi.text = F.Name;
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
        public BaseResponseDTO<List<NPF_FRM_Section>> GetAllSection(long Id)
        {
            BaseResponseDTO<List<NPF_FRM_Section>> dto = new BaseResponseDTO<List<NPF_FRM_Section>>();
            List<NPF_FRM_Section> result = new List<NPF_FRM_Section>();
            string errorMsg = "No Data Found";
            try
            {
                result = context.NPF_FRM_Section.Where(x => x.DeleteStatus == false).ToList();
                if (Id == 0)
                {
                    dto.Data = result;
                    dto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    List<NPF_FRM_FormSectionMapping> Psml = context.NPF_FRM_FormSectionMapping.Where(x => x.FormId == Id && x.DeleteStatus == false).ToList();
                    foreach (NPF_FRM_FormSectionMapping psm in Psml)
                    {
                        result.Where(x => x.Id == psm.SectionId).FirstOrDefault().check = true;
                        result.Where(x => x.Id == psm.SectionId).FirstOrDefault().Order = psm.Order;
                    }
                    dto.Data = result.OrderByDescending(x => x.check).ToList();
                    dto.QryResult = queryResult.SUCEEDED;
                }

            }
            catch (Exception ex)
            {
                dto.Data = result;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }

            return dto;
        }
    }
}
