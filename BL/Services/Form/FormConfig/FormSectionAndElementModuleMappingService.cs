using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using BL.Models.Form;
using BL.Models.Template;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Access;
using DAL.Models.Administration;
using DAL.Models.Form;
using DAL.Models.FormPage;
using DAL.Models.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Form.FormConfig
{
    public class FormSectionAndElementModuleMappingService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private RowAccessService service;
        private MatrixService Mxservice;
        public UserMatrixDTO UM = new UserMatrixDTO();
        public FormSectionAndElementModuleMappingService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            service = new RowAccessService();
            Mxservice = new MatrixService();
            UM = Mxservice.sGetMatrixUserByUserID();
        }

        public BaseResponseDTO<List<FormSectionAndElementModuleMappingListDTO>> GetAll()
        {
            BaseResponseDTO<List<FormSectionAndElementModuleMappingListDTO>> dto = new BaseResponseDTO<List<FormSectionAndElementModuleMappingListDTO>>();
            List<FormSectionAndElementModuleMappingListDTO> result = new List<FormSectionAndElementModuleMappingListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_FRM_FormPageSectionModuleMapping
                        where a.DeleteStatus == false
                        select new FormSectionAndElementModuleMappingListDTO()
                        {
                            Id = a.Id,
                            Module = context.NPF_SYS_Modules.FirstOrDefault(x => x.Id == a.ModuleId).Name,
                            Form = context.NPF_FRM_Form.FirstOrDefault(x => x.Id == a.FormId).Name,
                        })
                       .GroupBy(x => new { x.Module, x.Form }) // Group by Module and Form
                       .Select(group => group.First()) // Select first element of each group
                       .ToList();
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
        public BaseResponseDTO<FormSectionAndElementModuleMappingCRUDTO> GetById(long Id)
        {
            BaseResponseDTO<FormSectionAndElementModuleMappingCRUDTO> dto = new BaseResponseDTO<FormSectionAndElementModuleMappingCRUDTO>();
            FormSectionAndElementModuleMappingCRUDTO result = new FormSectionAndElementModuleMappingCRUDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_FRM_FormPageSectionModuleMapping
                          where a.Id == Id
                          select new FormSectionAndElementModuleMappingCRUDTO()
                          {
                              Form = a.FormId,
                              Module = a.ModuleId,
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

                List<NPF_FRM_Elements> PL = (from a in context.NPF_FRM_Elements
                                             join b in context.NPF_FRM_FormSectionMapping on a.ServiceId equals b.SectionId
                                             where b.FormId == Id && b.DeleteStatus == false && a.DeleteStatus == false
                                             select new NPF_FRM_Elements
                                             {
                                                 Id = a.Id,
                                                 Name = context.NPF_FRM_Section.Where(x => x.Id == b.SectionId).FirstOrDefault().Name + " - " + a.Name,
                                             }).ToList();
                Dd.title = "Element";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_Elements P in PL)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = P.Id;
                    Ddi.text = P.Name;
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<FormSectionAndElementModuleMappingDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
           
   
            try
            {
                List<long> Ids = context.NPF_FRM_FormSectionMapping.Where(x => x.FormId == dataToSave.Data.Form).Select(p => p.SectionId).ToList();
                foreach (long id in Ids)
                {
                    NPF_FRM_FormPageSectionModuleMapping sec = new NPF_FRM_FormPageSectionModuleMapping();
                    sec.FormId = dataToSave.Data.Form;
                    sec.SectionId = id;
                    sec.ModuleId = Guid.Parse(dataToSave.Data.Module);
                    sec.ReadOnly = true;
                    sec.Hidden = true;
                    context.NPF_FRM_FormPageSectionModuleMapping.Add(sec);
                    context.SaveChanges();
                }
                foreach (ElementMappingDTO ele in dataToSave.Data.Element)
                {
                    NPF_FRM_Elements element = context.NPF_FRM_Elements.Where(x => x.Id == ele.ElementId).FirstOrDefault();
                    long FkId = context.NPF_FRM_FormPageSectionModuleMapping.Where(x => x.FormId == dataToSave.Data.Form && x.ModuleId == Guid.Parse(dataToSave.Data.Module) && x.SectionId == element.ServiceId && x.DeleteStatus == false).FirstOrDefault().Id;
                    NPF_FRM_FormPageElementModuleMapping elem = new NPF_FRM_FormPageElementModuleMapping()
                    { 

                        Fk_FormSectionModuleId =FkId,
                        ElementId = ele.ElementId,
                        ReadOnly = ele.ReadOnly,
                        Hidden = false,
                        IsListing = false,
                    };
                    context.NPF_FRM_FormPageElementModuleMapping.Add(elem);
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<FormSectionAndElementModuleMappingDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            List<NPF_FRM_FormPageSectionModuleMapping> sec = new List<NPF_FRM_FormPageSectionModuleMapping>();

            try
            {
                sec = context.NPF_FRM_FormPageSectionModuleMapping.Where(x => x.ModuleId.ToString() == dataToUpdate.Data.Module && x.FormId == dataToUpdate.Data.Form && x.DeleteStatus == false).ToList();
                foreach (NPF_FRM_FormPageSectionModuleMapping s in sec)
                {
                    s.DeleteStatus = true;
                    context.NPF_FRM_FormPageSectionModuleMapping.Update(s);
                    context.SaveChanges();

                    List<NPF_FRM_FormPageElementModuleMapping> elem = context.NPF_FRM_FormPageElementModuleMapping.Where(p => p.Fk_FormSectionModuleId == s.Id).ToList();
                    if (elem != null)
                    {
                        foreach (NPF_FRM_FormPageElementModuleMapping ele in elem)
                        {
                            ele.DeleteStatus = true;
                            context.NPF_FRM_FormPageElementModuleMapping.Update(ele);
                            context.SaveChanges();
                        }
                    }
                }
                BaseResponseDTO<bool> save = await SaveAsync(dataToUpdate);
                if (save.Data)
                {
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Error Updating Record";
                    BaseDto.QryResult = queryResult.FAILED;
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

        public async Task<BaseResponseDTO<List<ElementMappingDTO>>> GetTableById(FormSectionAndElementModuleMappingTableDTO dt)
        {
            BaseResponseDTO<List<ElementMappingDTO>> BaseDto = new BaseResponseDTO<List<ElementMappingDTO>>();

            try
            {
                List<long> Ids = context.NPF_FRM_FormPageSectionModuleMapping.Where(f => f.FormId == dt.Form && f.ModuleId.ToString() == dt.Module && f.DeleteStatus == false).Select(p => p.Id).ToList();

                BaseDto.Data = (from b in context.NPF_FRM_FormPageElementModuleMapping
                                where b.DeleteStatus == false && Ids.Contains(b.Fk_FormSectionModuleId)
                                select new ElementMappingDTO()
                                {
                                    Id = b.Id,
                                    ElementId = b.ElementId,
                                    ElementName = context.NPF_FRM_Elements.Where(e => e.Id == b.ElementId).FirstOrDefault().Name,
                                    ReadOnly = b.ReadOnly,
                                }).ToList();
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = new List<ElementMappingDTO>();
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }

        public BaseResponseDTO<bool> Delete(BaseRequestDTO<long> dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            FormSectionAndElementModuleMappingCRUDTO del = new FormSectionAndElementModuleMappingCRUDTO();
            List<NPF_FRM_FormPageSectionModuleMapping> sec = new List<NPF_FRM_FormPageSectionModuleMapping>();
            try
            {
                del = (from a in context.NPF_FRM_FormPageSectionModuleMapping
                       where a.Id == dataToDelId.Data
                       select new FormSectionAndElementModuleMappingCRUDTO
                       {
                           Form = a.FormId,
                           Module = a.ModuleId,
                       }).FirstOrDefault();

                sec = context.NPF_FRM_FormPageSectionModuleMapping.Where(x => x.ModuleId == del.Module && x.FormId == del.Form && x.DeleteStatus == false).ToList();
                foreach(NPF_FRM_FormPageSectionModuleMapping s in sec)
                {
                    s.DeleteStatus = true;
                    context.NPF_FRM_FormPageSectionModuleMapping.Update(s);
                    context.SaveChanges();

                    List<NPF_FRM_FormPageElementModuleMapping> elem = context.NPF_FRM_FormPageElementModuleMapping.Where(p => p.Fk_FormSectionModuleId == s.Id).ToList();
                    if (elem != null)
                    {
                        foreach(NPF_FRM_FormPageElementModuleMapping ele in elem)
                        {
                            ele.DeleteStatus = true;
                            context.NPF_FRM_FormPageElementModuleMapping.Update(ele);
                            context.SaveChanges();
                        }
                    }
                }
                

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
