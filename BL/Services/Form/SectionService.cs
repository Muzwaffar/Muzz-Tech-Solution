using BL.Constants;
using BL.Models.Common;
using BL.Models.Form;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using DAL.Models.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Form
{
    public class SectionService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public SectionService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }
        public BaseResponseDTO<bool> Delete(BaseRequestDTO<long> dataToDelId)
        {
            long Id = dataToDelId.Data;
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.NPF_FRM_FormSectionMapping.Any(x => x.SectionId == Id && x.DeleteStatus == false))
                {
                    NPF_FRM_Section upd = context.NPF_FRM_Section.Where(x => x.Id == Id).FirstOrDefault();
                    List<NPF_FRM_Elements> elml = context.NPF_FRM_Elements.Where(x => x.ServiceId == upd.Id && x.DeleteStatus == false).ToList();
                    foreach (NPF_FRM_Elements elm in elml)
                    {
                        elm.DeleteStatus = true;
                        context.NPF_FRM_Elements.Update(elm);
                        context.SaveChanges();

                        List<NPF_FRM_Parameters> paramcheckl = context.NPF_FRM_Parameters.Where(x => x.ElementId == elm.Id).ToList();
                        if (paramcheckl.Count > 0)
                        {
                            foreach (NPF_FRM_Parameters paramcheck in paramcheckl)
                            {
                                paramcheck.DeleteStatus = true;
                                context.NPF_FRM_Parameters.Update(paramcheck);
                                context.SaveChanges();
                            }
                        }

                    }
                    upd.DeleteStatus = true;
                    context.NPF_FRM_Section.Update(upd);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = new QueryResult().SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Cannot Delete Record. Record Currently In Use";
                    BaseDto.QryResult = queryResult.FAILED;
                }

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Deleting Record";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<List<DropDown>> GetAllDropDownValues()
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            string[] values = new string[] { "UOM" };

            try
            {
                Dd = new DropDown();
                List<NPF_SYS_UomHeader> UOML = context.NPF_SYS_UomHeader.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "UOM";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_UomHeader UOM in UOML)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = UOM.Id;
                    Ddi.text = UOM.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);
                List<NPF_FRM_Source> Srl = context.NPF_FRM_Source.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Source";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_Source sr in Srl)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = sr.Id;
                    Ddi.text = sr.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);
                List<NPF_FRM_InputTypeDetails> Prl = context.NPF_FRM_InputTypeDetails.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "InputType";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_InputTypeDetails pr in Prl)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = pr.Id;
                    Ddi.text = pr.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);
                List<NPF_FRM_InputTypeDetails> Irl = context.NPF_FRM_InputTypeDetails.Where(x => x.DeleteStatus == false && x.IsDatatable == true).ToList();
                Dd = new DropDown();
                Dd.title = "InputTypeDatatable";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_InputTypeDetails pr in Irl)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = pr.Id;
                    Ddi.text = pr.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);
                //Asset Type has been removed
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
        public BaseResponseDTO<List<DropDown>> GetAllParameters(long Id)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                List<NPF_FRM_ParametersList> Prl = context.NPF_FRM_ParametersList.Where(x => x.SourceId == Id && x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "ParametersList";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_FRM_ParametersList pr in Prl)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = pr.Id;
                    Ddi.text = pr.Name;
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
        public BaseResponseDTO<List<NPF_FRM_Section>> GetAll()
        {
            BaseResponseDTO<List<NPF_FRM_Section>> dto = new BaseResponseDTO<List<NPF_FRM_Section>>();
            List<NPF_FRM_Section> result = new List<NPF_FRM_Section>();
            string errorMsg = "No Data Found";
            try
            {
                result = context.NPF_FRM_Section.Where(x => x.DeleteStatus == false).ToList();
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
        public BaseResponseDTO<NPF_FRM_Section> GetById(long Id)
        {
            BaseResponseDTO<NPF_FRM_Section> dto = new BaseResponseDTO<NPF_FRM_Section>();
            NPF_FRM_Section result = new NPF_FRM_Section();
            string errorMsg = "No Data Found";
            try
            {
                result = context.NPF_FRM_Section.Where(x => x.Id == Id).FirstOrDefault();
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
        public BaseResponseDTO<List<ElementDTO>> GetElementById(long Id)
        {
            BaseResponseDTO<List<ElementDTO>> dto = new BaseResponseDTO<List<ElementDTO>>();
            List<ElementDTO> result = new List<ElementDTO>();
            List<ElementDTO> resultc = new List<ElementDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_FRM_Section
                          join b in context.NPF_FRM_Elements on a.Id equals b.ServiceId
                          where a.Id == Id && b.DeleteStatus == false && b.ParentId == null
                          select new ElementDTO
                          {
                              Id = b.Id,
                              Order = b.Order,
                              ServiceId = a.Id,
                              Name = b.Name,
                              AdditionalLabel = b.AdditionalLabel,
                              InputTypeDetails = b.InputTypeDetails,
                              Source = b.Source,
                              LookUpValue = b.UOM,
                              Notification = b.Notification,
                              IsMobileChecklist = b.IsMobileChecklist,
                              Parameters = context.NPF_FRM_Parameters.Where(x => x.ElementId == b.Id).ToList(),
                              IsListing = b.IsListing,
                              URL = b.URL,
                              ListingName = b.ListingName,

                          }).ToList();
                foreach (ElementDTO element in result)
                {
                    resultc = (from a in context.NPF_FRM_Section
                               join b in context.NPF_FRM_Elements on a.Id equals b.ServiceId
                               where a.Id == Id && b.DeleteStatus == false && b.ParentId == element.Id
                               select new ElementDTO
                               {
                                   Id = b.Id,
                                   Order = b.Order,
                                   ServiceId = a.Id,
                                   Name = b.Name,
                                   AdditionalLabel = b.AdditionalLabel,
                                   InputTypeDetails = b.InputTypeDetails,
                                   ParentId = b.ParentId,
                                   Source = b.Source,
                                   LookUpValue = b.UOM,
                                   Notification = b.Notification,
                                   IsMobileChecklist = b.IsMobileChecklist,
                                   Parameters = context.NPF_FRM_Parameters.Where(x => x.ElementId == b.Id).ToList(),
                                   IsListing = b.IsListing,
                                   URL = b.URL,
                                   ListingName = b.ListingName,

                               }).ToList();
                    element.InnerElemList = resultc;
                }
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
        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<SectionElementDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (context.NPF_FRM_Section.Any(x => x.Name.ToLower() == dataToSave.Data.Sec.Name.ToLower() && x.DeleteStatus != true))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Section With Same Name Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                else
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_FRM_Section");
                    dataToSave.Data.Sec.Code = BaseDtoS.Data; //"SER" + 
                    dataToSave.Data.Sec.Multiple = dataToSave.Data.ElemList.Count > 0 ? true : false;
                    context.NPF_FRM_Section.Add(dataToSave.Data.Sec);
                    context.SaveChanges();
                    if (dataToSave.Data.ElemList.Count > 0)
                    {
                        await SaveElem(dataToSave.Data.ElemList, dataToSave.Data.Sec.Id);
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
        public async Task<BaseResponseDTO<bool>> SaveElem(List<ElementDTO> dt, long ServiceId, long? ParentId = null)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                foreach (ElementDTO elm in dt)
                {
                    NPF_FRM_InputTypeDetails itd = context.NPF_FRM_InputTypeDetails.Where(x => x.Id == elm.InputTypeDetails.Id).FirstOrDefault();
                    context.NPF_FRM_InputTypeDetails.Update(itd);
                    NPF_FRM_Source? src = null;
                    if (elm.Source != null)
                    {
                        src = context.NPF_FRM_Source.Where(x => x.Id == elm.Source.Id).FirstOrDefault();
                        context.NPF_FRM_Source.Update(src);
                    }

                    NPF_SYS_UomHeader? lpv = null;
                    if (elm.LookUpValue != null)
                    {
                        if (elm.LookUpValue.Id != 0)
                        {
                            lpv = context.NPF_SYS_UomHeader.Where(x => x.Id == elm.LookUpValue.Id).FirstOrDefault();
                            context.NPF_SYS_UomHeader.Update(lpv);
                        }
                    }
                    NPF_FRM_Elements element = new NPF_FRM_Elements()
                    {
                        Order = elm.Order,
                        Name = elm.Name,
                        AdditionalLabel = elm.AdditionalLabel,
                        ServiceId = ServiceId,
                        InputTypeDetails = itd,
                        Source = src,
                        UOM = lpv,
                        Notification = elm.Notification,
                        IsMobileChecklist = elm.IsMobileChecklist,
                        ParentId = ParentId,
                        IsListing = elm.IsListing,
                        URL = elm.URL,
                        ListingName = elm.ListingName
                    };
                    context.NPF_FRM_Elements.Add(element);
                    context.SaveChanges();
                    if (elm.Parameters != null)
                    {
                        foreach (var para in elm.Parameters)
                        {
                            NPF_FRM_ParametersList pl = context.NPF_FRM_ParametersList.Where(x => x.Id == para.ParameterList.Id).FirstOrDefault();
                            NPF_FRM_Parameters Pa = new NPF_FRM_Parameters()
                            {
                                Name = para.Name,
                                Value = para.Value,
                                ParameterList = pl,
                                ElementId = element.Id,

                            };
                            context.NPF_FRM_Parameters.Add(Pa);
                            context.SaveChanges();
                        }
                    }
                    if (elm.InnerElemList.Count > 0)
                    {
                        await SaveElem(elm.InnerElemList, ServiceId, element.Id);
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
        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<SectionElementDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_FRM_Section.Any(x => x.Name.ToLower() == dataToUpdate.Data.Sec.Name.ToLower() && x.Id != dataToUpdate.Data.Sec.Id && x.DeleteStatus != true))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Section With Same Name Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                else
                {
                    List<NPF_FRM_Elements> eleml = context.NPF_FRM_Elements.Where(x => x.ServiceId == dataToUpdate.Data.Sec.Id && x.DeleteStatus == false && x.ParentId == null).ToList();
                    List<NPF_FRM_Elements> elemlc = context.NPF_FRM_Elements.Where(x => x.ServiceId == dataToUpdate.Data.Sec.Id && x.DeleteStatus == false && x.ParentId != null).ToList();
                    if (dataToUpdate.Data.ElemList != null)
                    {
                        foreach (ElementDTO elm in dataToUpdate.Data.ElemList)
                        {
                            List<NPF_FRM_Parameters> Paral = context.NPF_FRM_Parameters.Where(x => x.ElementId == elm.Id && x.DeleteStatus == false).ToList();
                            var check = eleml.Where(x => x.Name.ToLower() == elm.Name.ToLower()).FirstOrDefault();

                            if (check == null)
                            {
                                NPF_FRM_InputTypeDetails itd = context.NPF_FRM_InputTypeDetails.Where(x => x.Id == elm.InputTypeDetails.Id).FirstOrDefault();
                                context.NPF_FRM_InputTypeDetails.Update(itd);

                                NPF_FRM_Source? src = null;
                                if (elm.Source != null)
                                {
                                    src = context.NPF_FRM_Source.Where(x => x.Id == elm.Source.Id).FirstOrDefault();
                                    context.NPF_FRM_Source.Update(src);
                                }

                                NPF_SYS_UomHeader? lpv = null;
                                if (elm.LookUpValue != null)
                                {
                                    lpv = context.NPF_SYS_UomHeader.Where(x => x.Id == elm.LookUpValue.Id).FirstOrDefault();
                                    context.NPF_SYS_UomHeader.Update(lpv);
                                }


                                NPF_FRM_Elements element = new NPF_FRM_Elements()
                                {
                                    Order = elm.Order,
                                    Name = elm.Name,
                                    AdditionalLabel = elm.AdditionalLabel,
                                    ServiceId = dataToUpdate.Data.Sec.Id,
                                    InputTypeDetails = itd,
                                    Source = src,
                                    UOM = lpv,
                                    Notification = elm.Notification,
                                    IsMobileChecklist = elm.IsMobileChecklist,
                                    IsListing = elm.IsListing,
                                    URL = elm.URL,
                                    ListingName = elm.ListingName,
                                };
                                context.NPF_FRM_Elements.Add(element);
                                context.SaveChanges();
                                if (elm.Parameters != null)
                                {
                                    foreach (var para in elm.Parameters)
                                    {

                                        NPF_FRM_ParametersList pl = context.NPF_FRM_ParametersList.Where(x => x.Id == para.ParameterList.Id).FirstOrDefault();
                                        context.NPF_FRM_ParametersList.Update(pl);
                                        NPF_FRM_Parameters Pa = new NPF_FRM_Parameters()
                                        {
                                            Name = para.Name,
                                            Value = para.Value,
                                            ParameterList = pl,
                                            ElementId = element.Id,

                                        };
                                        context.NPF_FRM_Parameters.Add(Pa);
                                        context.SaveChanges();
                                    }
                                }
                                if (elm.InnerElemList != null)
                                {
                                    foreach (ElementDTO elmc in elm.InnerElemList)
                                    {
                                        List<NPF_FRM_Parameters> Paralc = context.NPF_FRM_Parameters.Where(x => x.ElementId == elmc.Id && x.DeleteStatus == false).ToList();
                                        var checkc = elemlc.Where(x => x.Name.ToLower() == elmc.Name.ToLower()).FirstOrDefault();
                                        if (checkc == null)
                                        {
                                            NPF_FRM_InputTypeDetails itdc = context.NPF_FRM_InputTypeDetails.Where(x => x.Id == elmc.InputTypeDetails.Id).FirstOrDefault();
                                            context.NPF_FRM_InputTypeDetails.Update(itdc);

                                            NPF_FRM_Source? srcc = null;
                                            if (elmc.Source != null)
                                            {
                                                srcc = context.NPF_FRM_Source.Where(x => x.Id == elmc.Source.Id).FirstOrDefault();
                                                context.NPF_FRM_Source.Update(srcc);
                                            }

                                            NPF_SYS_UomHeader? lpvc = null;
                                            if (elmc.LookUpValue != null && elmc.LookUpValue.Id != 0)
                                            {
                                                lpvc = context.NPF_SYS_UomHeader.Where(x => x.Id == elmc.LookUpValue.Id).FirstOrDefault();
                                                context.NPF_SYS_UomHeader.Update(lpvc);
                                            }


                                            NPF_FRM_Elements elementc = new NPF_FRM_Elements()
                                            {
                                                Order = elmc.Order,
                                                Name = elmc.Name,
                                                AdditionalLabel = elmc.AdditionalLabel,
                                                ServiceId = dataToUpdate.Data.Sec.Id,
                                                InputTypeDetails = itdc,
                                                Source = srcc,
                                                UOM = lpvc,
                                                Notification = elmc.Notification,
                                                ParentId = elm.Id == 0 ? element.Id : elm.Id,
                                                IsMobileChecklist = elmc.IsMobileChecklist,
                                                IsListing = elmc.IsListing,
                                                URL = elmc.URL,
                                                ListingName = elmc.ListingName,
                                            };
                                            context.NPF_FRM_Elements.Add(elementc);
                                            context.SaveChanges();
                                            if (elmc.Parameters != null)
                                            {
                                                foreach (var parac in elmc.Parameters)
                                                {

                                                    NPF_FRM_ParametersList plc = context.NPF_FRM_ParametersList.Where(x => x.Id == parac.ParameterList.Id).FirstOrDefault();
                                                    context.NPF_FRM_ParametersList.Update(plc);
                                                    NPF_FRM_Parameters Pac = new NPF_FRM_Parameters()
                                                    {
                                                        Name = parac.Name,
                                                        Value = parac.Value,
                                                        ParameterList = plc,
                                                        ElementId = elementc.Id,

                                                    };
                                                    context.NPF_FRM_Parameters.Add(Pac);
                                                    context.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NPF_FRM_InputTypeDetails itd = context.NPF_FRM_InputTypeDetails.Where(x => x.Id == elm.InputTypeDetails.Id).FirstOrDefault();
                                context.NPF_FRM_InputTypeDetails.Update(itd);
                                NPF_FRM_Source? src = null;
                                if (elm.Source != null)
                                {
                                    src = context.NPF_FRM_Source.Where(x => x.Id == elm.Source.Id).FirstOrDefault();
                                    context.NPF_FRM_Source.Update(src);
                                }

                                NPF_SYS_UomHeader? lpv = null;
                                if (elm.LookUpValue != null)
                                {
                                    lpv = context.NPF_SYS_UomHeader.Where(x => x.Id == elm.LookUpValue.Id).FirstOrDefault();
                                    context.NPF_SYS_UomHeader.Update(lpv);
                                }
                                check.AdditionalLabel = elm.AdditionalLabel;
                                check.Source = src;
                                check.Order = elm.Order;
                                check.UOM = lpv;
                                check.InputTypeDetails = itd;
                                check.URL = elm.URL;
                                check.ListingName = elm.ListingName;
                                context.NPF_FRM_Elements.Update(check);
                                context.SaveChanges();
                                if (elm.Parameters != null)
                                {
                                    foreach (NPF_FRM_Parameters p in elm.Parameters)
                                    {
                                        var pcheck = Paral.Where(x => x.Name.ToLower() == p.Name.ToLower() && x.DeleteStatus == false).FirstOrDefault();
                                        if (pcheck != null)
                                        {
                                            NPF_FRM_ParametersList pl = context.NPF_FRM_ParametersList.Where(x => x.Id == p.ParameterList.Id).FirstOrDefault();
                                            context.NPF_FRM_ParametersList.Update(pl);
                                            pcheck.ParameterList = pl;
                                            pcheck.Value = p.Value;
                                            context.NPF_FRM_Parameters.Update(pcheck);
                                            Paral.Remove(pcheck);
                                        }
                                        else
                                        {
                                            NPF_FRM_ParametersList pl = context.NPF_FRM_ParametersList.Where(x => x.Id == p.ParameterList.Id).FirstOrDefault();
                                            context.NPF_FRM_ParametersList.Update(pl);
                                            NPF_FRM_Parameters Pa = new NPF_FRM_Parameters()
                                            {
                                                Name = p.Name,
                                                Value = p.Value,
                                                ParameterList = pl,
                                                ElementId = check.Id,
                                            };
                                            context.NPF_FRM_Parameters.Add(Pa);
                                            context.SaveChanges();
                                        }
                                    }

                                }
                                if (Paral.Count > 0)
                                {
                                    foreach (NPF_FRM_Parameters rpara in Paral)
                                    {
                                        rpara.DeleteStatus = true;
                                        context.NPF_FRM_Parameters.Update(rpara);
                                        context.SaveChanges();
                                    }
                                }
                                eleml.Remove(check);
                                if (elm.InnerElemList != null)
                                {
                                    foreach (ElementDTO elmc in elm.InnerElemList)
                                    {
                                        List<NPF_FRM_Parameters> Paralc = context.NPF_FRM_Parameters.Where(x => x.ElementId == elmc.Id && x.DeleteStatus == false).ToList();
                                        var checkc = elemlc.Where(x => x.Name.ToLower() == elmc.Name.ToLower()).FirstOrDefault();
                                        NPF_FRM_InputTypeDetails itdc = context.NPF_FRM_InputTypeDetails.Where(x => x.Id == elmc.InputTypeDetails.Id).FirstOrDefault();
                                        context.NPF_FRM_InputTypeDetails.Update(itdc);
                                        NPF_FRM_Source? srcc = null;
                                        if (elmc.Source != null)
                                        {
                                            srcc = context.NPF_FRM_Source.Where(x => x.Id == elmc.Source.Id).FirstOrDefault();
                                            context.NPF_FRM_Source.Update(srcc);
                                        }

                                        NPF_SYS_UomHeader? lpvc = null;
                                        if (elmc.LookUpValue != null)
                                        {
                                            lpvc = context.NPF_SYS_UomHeader.Where(x => x.Id == elmc.LookUpValue.Id).FirstOrDefault();
                                            context.NPF_SYS_UomHeader.Update(lpvc);
                                        }
                                        checkc.AdditionalLabel = elmc.AdditionalLabel;
                                        checkc.Source = srcc;
                                        checkc.Order = elmc.Order;
                                        checkc.UOM = lpvc;
                                        checkc.ParentId = elmc.ParentId;
                                        checkc.InputTypeDetails = itdc;
                                        checkc.URL = elmc.URL;
                                        checkc.ListingName = elmc.ListingName;
                                        context.NPF_FRM_Elements.Update(checkc);
                                        context.SaveChanges();
                                        if (elm.Parameters != null)
                                        {
                                            foreach (NPF_FRM_Parameters pc in elmc.Parameters)
                                            {
                                                var pcheckc = Paral.Where(x => x.Name.ToLower() == pc.Name.ToLower() && x.DeleteStatus == false).FirstOrDefault();
                                                if (pcheckc != null)
                                                {
                                                    NPF_FRM_ParametersList plc = context.NPF_FRM_ParametersList.Where(x => x.Id == pc.ParameterList.Id).FirstOrDefault();
                                                    context.NPF_FRM_ParametersList.Update(plc);
                                                    pcheckc.ParameterList = plc;
                                                    pcheckc.Value = pc.Value;
                                                    context.NPF_FRM_Parameters.Update(pcheckc);
                                                    Paral.Remove(pcheckc);
                                                }
                                                else
                                                {
                                                    NPF_FRM_ParametersList plc = context.NPF_FRM_ParametersList.Where(x => x.Id == pc.ParameterList.Id).FirstOrDefault();
                                                    context.NPF_FRM_ParametersList.Update(plc);
                                                    NPF_FRM_Parameters Pac = new NPF_FRM_Parameters()
                                                    {
                                                        Name = pc.Name,
                                                        Value = pc.Value,
                                                        ParameterList = plc,
                                                        ElementId = checkc.Id,
                                                    };
                                                    context.NPF_FRM_Parameters.Add(Pac);
                                                    context.SaveChanges();
                                                }
                                            }

                                        }
                                        if (Paralc.Count > 0)
                                        {
                                            foreach (NPF_FRM_Parameters rparac in Paralc)
                                            {
                                                rparac.DeleteStatus = true;
                                                context.NPF_FRM_Parameters.Update(rparac);
                                                context.SaveChanges();
                                            }
                                        }
                                        elemlc.Remove(check);
                                    }
                                }
                            }

                        }

                    }
                    if (eleml != null)
                    {
                        foreach (NPF_FRM_Elements el in eleml)
                        {
                            el.DeleteStatus = true;
                            context.NPF_FRM_Elements.Update(el);
                            context.SaveChanges();
                            List<NPF_FRM_Parameters> paramcheckl = context.NPF_FRM_Parameters.Where(x => x.ElementId == el.Id).ToList();
                            if (paramcheckl.Count > 0)
                            {
                                foreach (NPF_FRM_Parameters paramcheck in paramcheckl)
                                {
                                    paramcheck.DeleteStatus = true;
                                    context.NPF_FRM_Parameters.Update(paramcheck);
                                    context.SaveChanges();
                                }
                            }
                        }

                    }
                    //if (elemlc != null)
                    //{
                    //    foreach (NPF_FRM_Elements el in elemlc)
                    //    {
                    //        el.DeleteStatus = true;
                    //        context.NPF_FRM_Elements.Update(el);
                    //        context.SaveChanges();
                    //        List<NPF_FRM_Parameters> paramcheckl = context.NPF_FRM_Parameters.Where(x => x.ElementId == el.Id).ToList();
                    //        if (paramcheckl.Count > 0)
                    //        {
                    //            foreach (NPF_FRM_Parameters paramcheck in paramcheckl)
                    //            {
                    //                paramcheck.DeleteStatus = true;
                    //                context.NPF_FRM_Parameters.Update(paramcheck);
                    //                context.SaveChanges();
                    //            }
                    //        }
                    //    }

                    //}
                    dataToUpdate.Data.Sec.Multiple = dataToUpdate.Data.ElemList != null ? true : false;
                    context.NPF_FRM_Section.Update(dataToUpdate.Data.Sec);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.QryResult = new QueryResult().SUCEEDED;
                }

            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Updating Record";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;

        }
    }
}
