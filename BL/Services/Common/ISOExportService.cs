using BL.Constants;
using BL.Models.Common;
using BL.Models.Form;
using BL.Models.FormPage;
using BL.Models.Template;
using BL.Services.ServiceRequest;
using BL.Services.Source;
using DAL.DBContext;
using DAL.Models.Form;
using DAL.Models.FormPage;
using DAL.Models.Template;
using iTextSharp.text.pdf;
using iTextSharp.xmp.impl;
using Microsoft.AspNetCore.Mvc;
using NPOI.Util;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class ISOExportService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public SourceControlService Sservice;
        public FormPageService service;

        public ISOExportService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Sservice = new SourceControlService();
            service = new FormPageService();
        }

        public async Task<BaseResponseDTO<string>> ExportISO([FromBody] BaseRequestDTO<TemplateDownloadDTO> data)
        {
            BaseResponseDTO<string> dto = new BaseResponseDTO<string>();
            try
            {
                long formId = context.NPF_FRM_FormPage.Where(x => x.Id == data.Data.Id).FirstOrDefault().FormId;
                NPF_TMP_TemplateFormMapping TFM = context.NPF_TMP_TemplateFormMapping.Where(f => f.DeleteStatus == false && f.FormId == formId && f.ModuleId == data.ModuleId).FirstOrDefault();
                dto.AdditionalData = TFM.TemplateName;
                PdfReader pdfReader = new PdfReader(TFM.URL);
                ByteArrayOutputStream baos = new ByteArrayOutputStream();
                PdfStamper pdfStamper = new PdfStamper(pdfReader, baos);
                AcroFields pdfFormFields = pdfStamper.AcroFields;
                var result = (from f in context.NPF_TMP_TemplateFormMapping
                              where f.DeleteStatus == false && f.FormId == formId && f.ModuleId == data.ModuleId
                              select new TemplateFormMappingDTO
                              {
                                  Id = f.Id,
                                  FormId = f.FormId,
                                  TemplateName = f.TemplateName,
                                  Element = (from e in context.NPF_TMP_TemplateElementMapping
                                             where e.DeleteStatus == false && e.FkTFM == f.Id && e.ParentId == 0
                                             select new TemplateElementMappingDTO
                                             {
                                                 Id = e.Id,
                                                 FkTFM = e.FkTFM,
                                                 ElementId = e.ElementId,
                                                 TextName = e.TextName,
                                                 ElementType = e.ElementType,
                                                 Column = e.Column
                                             }).ToList()
                              }).FirstOrDefault();


                List<long> woId = context.NPF_FRM_WorkOrder.Where(x => x.FormPageId == data.Data.Id && x.DeleteStatus == false).Select(y => y.Id).ToList();


                List<NPF_FRM_WOElementMapping> WOML = context.NPF_FRM_WOElementMapping.Where(y => woId.Contains(y.WOId)).ToList();


                foreach (TemplateElementMappingDTO rs in result.Element)
                {
                    try
                    {

                        var check = WOML.Where(x => x.ElementId == rs.ElementId).FirstOrDefault();

                        if (check != null)
                        {
                            if (rs.ElementType == "Datatable")
                            {
                                var childValues = context.NPF_TMP_TemplateElementMapping.Where(x => x.ParentId == rs.ElementId).ToList();
                                var dtIds = WOML.Where(x => x.ParentId == rs.ElementId).Select(y => y.ElementId).ToList();
                                List<long?> dtPlist = new List<long?>();
                                foreach (var id in dtIds)
                                {
                                    dtPlist.Add(id);
                                }
                                var dtvall = WOML.Where(x => dtPlist.Contains(x.ParentId)).OrderBy(y => y.Row).ToList();
                                if (dtvall.Count > 0)
                                {
                                    int constant = (int)dtvall.Max(x => x.Row);
                                    foreach (var datum in dtvall)
                                    {
                                        int? cs = (datum.Row - 1) - constant;
                                        datum.Row = cs > 0 ? cs : cs * -1;
                                    }
                                    foreach (var cv in childValues)
                                    {
                                        var val = dtvall.Where(x => x.Row == cv.Row && x.Column == cv.Column).FirstOrDefault();
                                        if (val != null)
                                        {
                                            pdfFormFields.SetField(cv.TextName, val.Value);
                                        }

                                    }
                                }

                            }
                            else if (rs.ElementType == "Dropdown")
                            {
                                var dtVall = Sservice.SourceValues(rs.ElementId);
                                var Val = dtVall.Result.Where(x => x.id.ToString() == check.Value).FirstOrDefault();
                                if (Val != null)
                                {
                                    pdfFormFields.SetField(rs.TextName, Val.text);
                                }
                                else
                                {
                                    pdfFormFields.SetField(rs.TextName, string.Empty);
                                }
                            }
                            else if (rs.ElementType == "Signature")
                            {
                                if (check.AdditionalValue != "")
                                {

                                    string base64ImageString = check.AdditionalValue.Split(',')[1];
                                    byte[] imageBytes = Convert.FromBase64String(base64ImageString);
                                    pdfFormFields.SetField(rs.TextName, base64ImageString);
                                }
                            }
                            else if (rs.ElementType == "Checkbox With Remarks")
                            {
                                pdfFormFields.SetField(rs.TextName, check.Value);
                                pdfFormFields.SetField(rs.TextName + "Text", check.AdditionalValue);
                            }
                            else if (rs.ElementType == "Dropdown with Details")
                            {
                                if(check.Value != null)
                                {
                                    try
                                    {
                                        DetailsByIdSearchDTO datum = new DetailsByIdSearchDTO()
                                        {
                                            ElementId = check.ElementId,
                                            DetailsId = Convert.ToInt64(check.Value)
                                        };
                                        BaseResponseDTO<List<CustomDictionary>> SA = new BaseResponseDTO<List<CustomDictionary>>();
                                        SA = await service.GetCustomDetailsByElementId(datum);
                                        if (SA != null)
                                        {
                                            string v = SA.Data.Where(x => x.key.Contains(rs.TextName.Split('#')[0])).FirstOrDefault().value;
                                            pdfFormFields.SetField(rs.TextName, v);
                                        }

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }

                            }
                            else
                            {
                                pdfFormFields.SetField(rs.TextName, check.Value);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                pdfStamper.FormFlattening = false;
                // close the pdf  
                pdfStamper.Close();

                //String pdfBytes = Base64.Encode( baos.ToByteArray()).ToString();
                var base666 = Convert.ToBase64String(baos.ToByteArray());
                dto.Data = "data:application/vnd.openxmlformats-officedocument.wordprocessingml.document;base64," + base666;
            }
            catch (Exception ex)
            {
            }
            return dto;
        }
    }
}
