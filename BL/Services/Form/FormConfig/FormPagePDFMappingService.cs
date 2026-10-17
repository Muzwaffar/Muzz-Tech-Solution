using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using BL.Models.Form;
using BL.Models.Template;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Form;
using DAL.Models.FormPage;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models.Access;
using BL.Models.Modules.Research;
using DAL.Common;
using DAL.Models.Template;
using System.Reflection;
using System.Security.Policy;

namespace BL.Services.Form.FormConfig
{
    public class FormPagePDFMappingService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private RowAccessService service;
        private MatrixService Mxservice;
        public UserMatrixDTO UM = new UserMatrixDTO();
        public FormPagePDFMappingService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            service = new RowAccessService();
            Mxservice = new MatrixService();
            UM = Mxservice.sGetMatrixUserByUserID();
        }

        public BaseResponseDTO<List<TemplateFormMappingListDTO>> GetAll()
        {
            BaseResponseDTO<List<TemplateFormMappingListDTO>> dto = new BaseResponseDTO<List<TemplateFormMappingListDTO>>();
            List<TemplateFormMappingListDTO> result = new List<TemplateFormMappingListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = result = (from a in context.NPF_TMP_TemplateFormMapping
                                   where a.DeleteStatus == false
                                   select new TemplateFormMappingListDTO()
                                   {
                                       Id = a.Id,
                                       TemplateName = a.TemplateName,
                                       FormName = context.NPF_FRM_Form.Where(x => x.Id == a.FormId).FirstOrDefault().Name
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
                //Element Type
                List<NPF_FRM_InputTypeDetails> Prl = context.NPF_FRM_InputTypeDetails.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDownLogin();
                Dd.title = "ElementType";
                Dd.items = new List<DropDownItemLogin>();
                foreach (NPF_FRM_InputTypeDetails pr in Prl)
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    Ddi.id = pr.Id.ToString();
                    Ddi.text = pr.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //TemplateName
                string directoryPath = "Template\\";
                string[] pdfFiles = Directory.GetFiles(directoryPath, "*.pdf");
                Dd = new DropDownLogin();
                Dd.title = "TemplateName";
                Dd.items = new List<DropDownItemLogin>();
                long id = 1;
                foreach (string pdf in pdfFiles)
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    Ddi.id = id.ToString();
                    Ddi.text = pdf.Split("\\")[1];
                    Dd.items.Add(Ddi);
                    id++;
                }
                Ddl.Add(Dd);

                //Module List
                List<NPF_SYS_Modules> mdl = context.NPF_SYS_Modules.Where(x => x.DeleteStatus == false).ToList();
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
        public BaseResponseDTO<TemplateFormMappingCRUDTO> GetById(long Id)
        {
            BaseResponseDTO<TemplateFormMappingCRUDTO> dto = new BaseResponseDTO<TemplateFormMappingCRUDTO>();
            TemplateFormMappingCRUDTO result = new TemplateFormMappingCRUDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_TMP_TemplateFormMapping
                          where a.Id == Id
                          select new TemplateFormMappingCRUDTO()
                          {
                              Id = a.Id,
                              FormId = a.FormId,
                              ModuleId = (Guid)a.ModuleId,
                              TemplateName = a.TemplateName,
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

        public BaseResponseDTO<List<DropDown>> GetSectionById(long Id)//FormId
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {

                List<NPF_FRM_FormSectionMapping> PL = context.NPF_FRM_FormSectionMapping.Where(x => x.FormId == Id && x.DeleteStatus == false).ToList();
                Dd.title = "Section";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_FormSectionMapping P in PL)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = P.SectionId;
                    Ddi.text = context.NPF_FRM_Section.Where(x => x.Id == P.SectionId && x.DeleteStatus == false).FirstOrDefault().Name;
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

        public BaseResponseDTO<List<DropDown>> GetElementById(long Id)//Section
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {

                List<NPF_FRM_Elements> PL = context.NPF_FRM_Elements.Where(x => x.ServiceId == Id && x.DeleteStatus == false).ToList();
                Dd.title = "Element";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_Elements P in PL)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = P.Id;
                    Ddi.text = P.Id + " - " + P.Name;
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

        public BaseResponseDTO<List<DropDown>> GetTemplateTextById(string Name)//Section
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {

                var pdf_filename = "Template\\" + Name;
                using (var reader = new PdfReader(pdf_filename))
                {
                    var fields = reader.AcroFields.Fields;
                    Dd.title = "TemplateText";
                    Dd.items = new List<DropDownItem>();
                    long id = 1;
                    foreach (var key in fields.Keys)
                    {
                        DropDownItem Ddi = new DropDownItem();
                        Ddi.Id = id;
                        Ddi.text = key;
                        Dd.items.Add(Ddi);
                        id++;
                    }
                    Ddl.Add(Dd);
                }
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<TemplateFormMappingDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();

            try
            {
                NPF_TMP_TemplateFormMapping dt = new NPF_TMP_TemplateFormMapping()
                {
                    TemplateName = dataToSave.Data.TemplateName,
                    ModuleId = context.NPF_SYS_Modules.Where(x => x.Name == dataToSave.Data.ModuleId).FirstOrDefault().Id,
                    URL = "Template\\" + dataToSave.Data.TemplateName,
                    FormId = dataToSave.Data.FormId
                };
                context.NPF_TMP_TemplateFormMapping.Add(dt);
                context.SaveChanges();

                foreach (TemplateElementMappingDTO elm in dataToSave.Data.Element)
                {
                    NPF_TMP_TemplateElementMapping tfm = new NPF_TMP_TemplateElementMapping();
                    tfm.FkTFM = dt.Id;
                    tfm.ElementId = elm.ElementId;
                    tfm.ParentId = elm.ParentId ?? 0;
                    tfm.ElementType = elm.ElementType;
                    tfm.TextName = elm.TextName;
                    tfm.Column = elm.Column ?? 0;
                    tfm.Row = elm.Row ?? 0;
                    context.NPF_TMP_TemplateElementMapping.Add(tfm);
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<TemplateFormMappingDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();

            try
            {
                NPF_TMP_TemplateFormMapping dt = context.NPF_TMP_TemplateFormMapping.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                dt.TemplateName = dataToUpdate.Data.TemplateName;
                dt.ModuleId = context.NPF_SYS_Modules.Where(x => x.Name == dataToUpdate.Data.ModuleId).FirstOrDefault().Id;
                dt.URL = "Template\\" + dataToUpdate.Data.TemplateName;
                dt.FormId = dataToUpdate.Data.FormId;
                context.NPF_TMP_TemplateFormMapping.Update(dt);
                context.SaveChanges();

                List<NPF_TMP_TemplateElementMapping> Currentls = context.NPF_TMP_TemplateElementMapping.Where(x => x.FkTFM == dt.Id && x.DeleteStatus == false).ToList();
                foreach (TemplateElementMappingDTO elm in dataToUpdate.Data.Element)
                {
                    if (Currentls.Any(y => y.Id == elm.Id))
                    {
                        Currentls.RemoveAll(x => x.Id == elm.Id);
                    }
                    else
                    {
                        NPF_TMP_TemplateElementMapping tfm = new NPF_TMP_TemplateElementMapping();
                        tfm.FkTFM = dt.Id;
                        tfm.ElementId = elm.ElementId;
                        tfm.ParentId = elm.ParentId ?? 0;
                        tfm.ElementType = elm.ElementType;
                        tfm.TextName = elm.TextName;
                        tfm.Column = elm.Column ?? 0;
                        tfm.Row = elm.Row ?? 0;
                        context.NPF_TMP_TemplateElementMapping.Add(tfm);
                        context.SaveChanges();
                    }
                }
                if (Currentls.Count > 0)
                {
                    foreach (NPF_TMP_TemplateElementMapping d in Currentls)
                    {
                        d.DeleteStatus = true;
                        context.NPF_TMP_TemplateElementMapping.Update(d);
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

        public async Task<BaseResponseDTO<List<TemplateElementMappingDTO>>> GetTableById(long Id)
        {
            BaseResponseDTO<List<TemplateElementMappingDTO>> BaseDto = new BaseResponseDTO<List<TemplateElementMappingDTO>>();

            try
            {
                BaseDto.Data = (from b in context.NPF_TMP_TemplateElementMapping
                                where b.FkTFM == Id && b.DeleteStatus == false
                                select new TemplateElementMappingDTO()
                                {
                                    Id = b.Id,
                                    FkTFM = b.FkTFM,
                                    ElementId = b.ElementId,
                                    ParentId = b.ParentId,
                                    Row = b.Row,
                                    Column = b.Column,
                                    TextName = b.TextName,
                                    ElementName = context.NPF_FRM_Elements.Where(x => x.Id == b.ElementId).FirstOrDefault().Name,
                                    ElementType = b.ElementType,
                                    ElementTypeId = context.NPF_FRM_InputTypeDetails.Where(x => x.Name == b.ElementType).FirstOrDefault().Id,
                                }).ToList();
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = new List<TemplateElementMappingDTO>();
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
                NPF_TMP_TemplateFormMapping upd = context.NPF_TMP_TemplateFormMapping.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_TMP_TemplateFormMapping.Update(upd);
                context.SaveChanges();

                List<NPF_TMP_TemplateElementMapping> Currentls = context.NPF_TMP_TemplateElementMapping.Where(x => x.FkTFM == dataToDelId.Data).ToList();
                foreach (NPF_TMP_TemplateElementMapping d in Currentls)
                {
                    d.DeleteStatus = true;
                    context.NPF_TMP_TemplateElementMapping.Update(d);
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
