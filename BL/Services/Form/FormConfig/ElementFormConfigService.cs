using BL.Constants;
using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Models.Form;
using BL.Services.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models.Access;
using DAL.Models.Administration;
using DAL.Models.Form;
using DAL.Models.FormPage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Form
{
    public class ElementFormConfigService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public ElementFormConfigService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
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
                List<NPF_FRM_Form> FRM = context.NPF_FRM_Form.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Form";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_Form D in FRM)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = D.Id;
                    Ddi.text = D.Name;
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

        public BaseResponseDTO<List<DropDownLogin>> GetAllDropDownValuesMenu()
        {
            BaseResponseDTO<List<DropDownLogin>> dto = new BaseResponseDTO<List<DropDownLogin>>();
            List<DropDownLogin> Ddl = new List<DropDownLogin>();
            DropDownLogin Dd = new DropDownLogin();
            string errorMsg = "No Data Found";
            try
            {
                Dd = new DropDownLogin();
                Dd.title = "Menu";
                Dd.items = new List<DropDownItemLogin>();
                //
                Dd.items = (from a in context.NPF_SYS_Modules
                            join b in context.NPF_SYS_Modules on a.ParentId equals b.Id into ab
                            from b in ab.DefaultIfEmpty()
                            join c in context.NPF_SYS_Modules on b.ParentId equals c.Id into bc
                            from c in bc.DefaultIfEmpty()
                            select new DropDownItemLogin()
                            {
                                id = a.Id.ToString(),
                                text = (c.DisplayName ?? "") + (c.DisplayName != null ? " - " : "") + (b.DisplayName ?? "") + (b.DisplayName != null ? " - " : "") + (a.DisplayName ?? "")
                            }).ToList();

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
        public BaseResponseDTO<ElementFormConfigDTO> GetElementFormConfig(BaseRequestDTO<RetrieveElementFormConfigDTO> dt)
        {
            BaseResponseDTO<ElementFormConfigDTO> dto = new BaseResponseDTO<ElementFormConfigDTO>();
            ElementFormConfigDTO result = new ElementFormConfigDTO();
            List<SectionConfigDTO> sectionConfig = new List<SectionConfigDTO>();
            string errorMsg = "No Data Found";
            try
            {
               var check = context.NPF_FRM_FormPageSectionModuleMapping.Where(x => x.FormId == dt.Data.FormId && x.ModuleId.ToString() == dt.Data.ModuleId && x.DeleteStatus == false).FirstOrDefault();
                if (check == null)
                {
                    List<long> sectionId = context.NPF_FRM_FormSectionMapping.Where(x => x.FormId == dt.Data.FormId && x.DeleteStatus == false).Select(y => y.SectionId).ToList();
                    List<NPF_FRM_Section> sections = context.NPF_FRM_Section.Where(x => sectionId.Contains(x.Id) && x.DeleteStatus == false).ToList();
                    List<NPF_FRM_Elements> elements = context.NPF_FRM_Elements.Where(x => sectionId.Contains((long)x.ServiceId) && x.DeleteStatus == false).ToList();
                    result.Sequence = 0;
                    foreach (NPF_FRM_Section sec in sections)
                    {
                        SectionConfigDTO s = new SectionConfigDTO();
                        s.SectionId = sec.Id;
                        s.SectionName = sec.Name;

                        List<ElementConfigDTO> elementConfig = new List<ElementConfigDTO>();
                        foreach (NPF_FRM_Elements elem in elements)
                        {
                            if (sec.Id == elem.ServiceId)
                            {
                                ElementConfigDTO e = new ElementConfigDTO();
                                e.ElementId = elem.Id;
                                e.ElementName = elem.Name;
                                elementConfig.Add(e);
                                s.Elements = elementConfig;
                            }
                        }
                       sectionConfig.Add(s);
                    }
                    result.Sections = sectionConfig;
                }
                else
                {
                    result.Sequence = context.NPF_FRM_FormModuleMappingSequence.Where(x => x.Module.Id.ToString() == dt.Data.ModuleId && x.Form.Id == dt.Data.FormId).FirstOrDefault().Sequence;
                    sectionConfig = (from a in context.NPF_FRM_FormPageSectionModuleMapping
                                     where a.FormId == dt.Data.FormId && a.ModuleId.ToString() == dt.Data.ModuleId && a.DeleteStatus == false
                                     select new SectionConfigDTO()
                                     {
                                         SectionId = a.SectionId,
                                         SectionName = context.NPF_FRM_Section
                                                          .Where(x => x.Id == a.SectionId)
                                                          .FirstOrDefault()
                                                          .Name,
                                         ReadOnly = a.ReadOnly,
                                         Hidden = a.Hidden,
                                         Elements = (from b in context.NPF_FRM_FormPageElementModuleMapping
                                                     where b.Fk_FormSectionModuleId == a.Id && b.DeleteStatus == false
                                                     select new ElementConfigDTO()
                                                     {
                                                         ElementId = b.ElementId,
                                                         ElementName = context.NPF_FRM_Elements
                                                                          .Where(x => x.Id == b.ElementId)
                                                                          .FirstOrDefault()
                                                                          .Name,
                                                         ReadOnly = b.ReadOnly,
                                                         Hidden = b.Hidden,
                                                     }).ToList(),
                                     }).GroupBy(s => s.SectionId)
                   .Select(g => g.First())
                   .ToList();
                }
                result.Sections = sectionConfig;
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
        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<ElementFormConfigDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                var check = context.NPF_FRM_FormPageSectionModuleMapping.Where(x => x.FormId == dataToSave.Data.FormId && x.ModuleId.ToString() == dataToSave.Data.ModuleId && x.DeleteStatus == false).FirstOrDefault();
                if (check == null)
                {
                    NPF_FRM_FormModuleMappingSequence FMS = new NPF_FRM_FormModuleMappingSequence();
                    FMS.Module = context.NPF_SYS_Modules.Where(x => x.Id.ToString() == dataToSave.Data.ModuleId).FirstOrDefault();
                    FMS.Form = context.NPF_FRM_Form.Where(x => x.Id == dataToSave.Data.FormId).FirstOrDefault();
                    FMS.Sequence = dataToSave.Data.Sequence;
                    context.NPF_FRM_FormModuleMappingSequence.Add(FMS);
                    context.SaveChanges();
                    foreach (SectionConfigDTO sec in dataToSave.Data.Sections)
                    {
                        NPF_FRM_FormPageSectionModuleMapping FPS = new NPF_FRM_FormPageSectionModuleMapping();
                        FPS.FormId = FMS.Form.Id;
                        FPS.SectionId = sec.SectionId;
                        FPS.ModuleId = FMS.Module.Id;
                        FPS.ReadOnly = sec.ReadOnly;
                        FPS.Hidden = sec.Hidden;
                        context.NPF_FRM_FormPageSectionModuleMapping.Add(FPS);
                        context.SaveChanges();
                        foreach (ElementConfigDTO elem in sec.Elements)
                        {
                            NPF_FRM_FormPageElementModuleMapping FPE = new NPF_FRM_FormPageElementModuleMapping();
                            FPE.Fk_FormSectionModuleId = FPS.Id;
                            FPE.ElementId = elem.ElementId;
                            FPE.ReadOnly = elem.ReadOnly;
                            FPE.Hidden = elem.Hidden;
                            context.NPF_FRM_FormPageElementModuleMapping.Add(FPE);
                            context.SaveChanges();
                        }
                    }
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    NPF_FRM_FormModuleMappingSequence FS = context.NPF_FRM_FormModuleMappingSequence.Where(x => x.Module.Id.ToString() == dataToSave.Data.ModuleId && x.Form.Id == dataToSave.Data.FormId && x.DeleteStatus == false).FirstOrDefault();
                    FS.Module = context.NPF_SYS_Modules.Where(x => x.Id.ToString() == dataToSave.Data.ModuleId).FirstOrDefault();
                    FS.Form = context.NPF_FRM_Form.Where(x => x.Id == dataToSave.Data.FormId).FirstOrDefault();
                    FS.Sequence = dataToSave.Data.Sequence;
                    context.NPF_FRM_FormModuleMappingSequence.Update(FS);
                    context.SaveChanges();

                    foreach (SectionConfigDTO sec in dataToSave.Data.Sections)
                    {
                        NPF_FRM_FormPageSectionModuleMapping sm = context.NPF_FRM_FormPageSectionModuleMapping.Where(x => x.FormId == FS.Form.Id && x.ModuleId == FS.Module.Id && x.SectionId == sec.SectionId && x.DeleteStatus == false).FirstOrDefault();
                        sm.DeleteStatus = true;
                        context.NPF_FRM_FormPageSectionModuleMapping.Update(sm);
                        context.SaveChanges();

                        NPF_FRM_FormPageSectionModuleMapping FPS = new NPF_FRM_FormPageSectionModuleMapping();
                        FPS.FormId = FS.Form.Id;
                        FPS.SectionId = sec.SectionId;
                        FPS.ModuleId = FS.Module.Id;
                        FPS.ReadOnly = sec.ReadOnly;
                        FPS.Hidden = sec.Hidden;
                        context.NPF_FRM_FormPageSectionModuleMapping.Add(FPS);
                        context.SaveChanges();
                        foreach (ElementConfigDTO elem in sec.Elements)
                        {
                            NPF_FRM_FormPageElementModuleMapping em = context.NPF_FRM_FormPageElementModuleMapping.Where(x => x.ElementId == elem.ElementId && x.Fk_FormSectionModuleId == sm.Id && x.DeleteStatus == false).FirstOrDefault();
                            em.DeleteStatus = true;
                            context.NPF_FRM_FormPageElementModuleMapping.Update(em);
                            context.SaveChanges();

                            NPF_FRM_FormPageElementModuleMapping FPE = new NPF_FRM_FormPageElementModuleMapping();
                            FPE.Fk_FormSectionModuleId = FPS.Id;
                            FPE.ElementId = elem.ElementId;
                            FPE.ReadOnly = elem.ReadOnly;
                            FPE.Hidden = elem.Hidden;
                            context.NPF_FRM_FormPageElementModuleMapping.Add(FPE);
                            context.SaveChanges();
                        }
                    }
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
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
    }
}
