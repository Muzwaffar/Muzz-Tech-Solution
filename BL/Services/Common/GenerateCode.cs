using BL.Models.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Administration;
using DAL.Models.Form;
using DAL.Models.FormPage;
using LinqToExcel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Remotion.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace BL.Services.Common
{
    public class GenerateCode
    {
        public async Task<BaseResponseDTO<string>> GCode()
        {
            BaseResponseDTO<string> baseResponseDTO = new BaseResponseDTO<string>();
            NPFContext context = new NPFContext();
            try
            {
                NPF_SYS_GlobalParam Gp = context.NPF_SYS_GlobalParam.Where(x => x.Name == "UniqueCodeGenerator").FirstOrDefault();
                Gp.Value = (Convert.ToInt32(Gp.Value) + 1).ToString();
                context.NPF_SYS_GlobalParam.Update(Gp);
                context.SaveChanges();
                int length = Gp.Value.Length;
                baseResponseDTO.Data = Gp.Value.Length < 6 ? Gp.Value.PadLeft(6, '0') : Gp.Value;
            }
            catch (Exception ex)
            {
                int _min = 100000000;
                int _max = 999999999;
                Random _rdm = new Random();
                baseResponseDTO.Data = _rdm.Next(_min, _max).ToString();
            }

            return baseResponseDTO;

        }
        public async Task<BaseResponseDTO<string>> FormPageNumber()
        {
            BaseResponseDTO<string> baseResponseDTO = new BaseResponseDTO<string>();
            NPFContext context = new NPFContext();
            try
            {
                NPF_SYS_GlobalParam Gp = context.NPF_SYS_GlobalParam.Where(x => x.Name == "FormPageNumber").FirstOrDefault();
                int additionalValueYear = int.Parse(Gp.AdditionalValue);

                // Check if AdditionalValue is less than the current year
                if (additionalValueYear < DateTime.Now.Year)
                {
                    Gp.Value = "0";
                    context.NPF_SYS_GlobalParam.Update(Gp);
                    context.SaveChanges();
                }
                // Increment Gp.Value
                Gp.Value = (Convert.ToInt32(Gp.Value) + 1).ToString();
                context.NPF_SYS_GlobalParam.Update(Gp);
                context.SaveChanges();
                string formPageNumber = Gp.Value.PadLeft(3, '0');
                // Format baseResponseDTO.Data
                baseResponseDTO.Data = $"EL/{Gp.AdditionalValue}/{formPageNumber}";
            }
            catch (Exception ex)
            {
                int randomFormPageNumber = new Random().Next(1, 1000);
                baseResponseDTO.Data = $"EL/{DateTime.Now.Year}/{randomFormPageNumber:D3}";
            }

            return baseResponseDTO;

        }
        public async Task<BaseResponseDTO<string>> GCodeIncident(string UserToken, string TableName)
        {
            BaseResponseDTO<string> baseResponseDTO = new BaseResponseDTO<string>();
            NPFContext context = new NPFContext();
            string code = "";
            string pref = "";
            int Value = 0;
            try
            {

                NPF_SYS_GlobalParam Gp = context.NPF_SYS_GlobalParam.Where(x => x.Name == "IncidentNo").FirstOrDefault();

                if(Convert.ToInt32(Gp.AdditionalValue) !=DateTime.Now.Year)
                {
                    Gp.AdditionalValue = DateTime.Now.Year.ToString();
                    Value =1;
                }
                else
                {
                    Value = Convert.ToInt32(Gp.Value) + 1;
                }
                Gp.Value = Value.ToString();
                code = Value.ToString();
                context.NPF_SYS_GlobalParam.Update(Gp);
                context.SaveChanges();
                pref = context.NPF_SYS_GlobalParam.Where(x => x.Name == "IncidentPrefix").FirstOrDefault().Value;
            }
            catch (Exception ex)
            {
                code = (await new GenerateCode().GCode()).Data;
            }
            baseResponseDTO.Data = pref+"/"+ code+"/"+DateTime.Now.Year+"/";
            return baseResponseDTO;

        }
        public async Task<BaseResponseDTO<string>> GCodeMain(string UserToken, string TableName)
        {
            BaseResponseDTO<string> baseResponseDTO = new BaseResponseDTO<string>();
            NPFContext context = new NPFContext();
            DateTime now = DateTime.Now;
            Guid Uid = Guid.Parse(UserToken);
            int compid = 0;
            string code = "";
            string pref = "";
            string value = "";
            string format = "";
            try
            {
                NPF_SYS_CompanyList cl = context.NPF_SYS_CompanyList.Where(x => x.Name == "Default").FirstOrDefault();
                compid = cl.Id;
                var usr = context.Users.Where(x => x.UserToken == Uid).FirstOrDefault();
                if (usr != null)
                {
                    if (usr.Company != 0)
                    {
                        compid = usr.Company;
                    }
                }
                var tblconfig = (from a in context.NPF_SYS_TableCodeConfigurations
                                 join b in context.NPF_SYS_CodeConfiguration on a.ConfigurationId equals b.Id
                                 where a.CompanyId == compid && a.TableName == TableName
                                 select new
                                 {
                                     a.Prefix,
                                     b.Date,
                                     b.DateFormat,
                                     b.Month,
                                     b.MonthFormat,
                                     b.Year,
                                     b.YearFormat,
                                     b.UsePrefix,
                                     b.PaddingNo,
                                     IsReset = b.Reset == null ? false : Convert.ToBoolean(b.Reset),
                                     b.ResetConfig
                                 }
                                 ).FirstOrDefault();
                if (tblconfig != null)
                {

                    if (tblconfig.UsePrefix == true)
                    {
                        pref = tblconfig.Prefix;
                    }
                    if (tblconfig.Year == true)
                    {
                        format = tblconfig.YearFormat;
                        if (tblconfig.Month == true)
                        {
                            format += tblconfig.MonthFormat;
                            if (tblconfig.Date == true)
                            {
                                format += tblconfig.DateFormat;
                            }
                        }
                        try
                        {
                            format = now.ToString(format);
                        }
                        catch (Exception ex)
                        {

                        }
                        code += format;
                    }


                    if (tblconfig.ResetConfig == "Year" && tblconfig.Year == true)
                    {
                        value = await GetCode(code, tblconfig.IsReset, tblconfig.ResetConfig, TableName);
                    }
                    else if (tblconfig.ResetConfig == "Month" && tblconfig.Year == true && tblconfig.Month == true)
                    {
                        value = await GetCode(code, tblconfig.IsReset, tblconfig.ResetConfig, TableName);
                    }
                    else if (tblconfig.ResetConfig == "Day" && tblconfig.Year == true && tblconfig.Month == true && tblconfig.Date == true)
                    {
                        value = await GetCode(code, tblconfig.IsReset, tblconfig.ResetConfig, TableName);
                    }
                    else
                    {
                        value = await GetCode(code, false, tblconfig.ResetConfig, TableName);
                    }

                    int length = value.Length + code.Length;
                    if (length < tblconfig.PaddingNo)
                    {
                        value = value.PadLeft(tblconfig.PaddingNo, '0');
                    }

                    code += value;

                }
                else
                {
                    code = (await new GenerateCode().GCode()).Data;
                }

            }
            catch (Exception ex)
            {
                code = (await new GenerateCode().GCode()).Data;
            }
            baseResponseDTO.Data = pref + code;
            return baseResponseDTO;

        }

        public async Task<BaseResponseDTO<string>> GCodeForm(long FormId, string ElemName = "", string AddiPrefix = "")
        {
            BaseResponseDTO<string> baseResponseDTO = new BaseResponseDTO<string>();
            NPFContext context = new NPFContext();
            DateTime now = DateTime.Now;
            string code = "";
            string pref = "";
            string value = "";
            string format = "/";
            try
            {
                NPF_FRM_Form Form = context.NPF_FRM_Form.Where(x => x.Id == FormId).FirstOrDefault();
                if (Form.ParentId != null)
                {
                    Form = context.NPF_FRM_Form.Where(x => x.Id == Convert.ToInt64(Form.ParentId)).FirstOrDefault();
                    FormId = Form.Id;
                }
                var tblconfig = (from a in context.NPF_SYS_TableCodeConfigurations
                                 join b in context.NPF_SYS_CodeConfiguration on a.ConfigurationId equals b.Id
                                 where a.TableName == Form.Name && a.Comment == ElemName
                                 select new
                                 {
                                     a.Prefix,
                                     b.Date,
                                     b.DateFormat,
                                     b.Month,
                                     b.MonthFormat,
                                     b.Year,
                                     b.YearFormat,
                                     b.UsePrefix,
                                     b.PaddingNo,
                                     IsReset = b.Reset == null ? false : Convert.ToBoolean(b.Reset),
                                     b.ResetConfig,
                                     a.HasAddi
                                 }
                                 ).FirstOrDefault();
                if (tblconfig != null)
                {

                    if (tblconfig.UsePrefix == true)
                    {
                        pref = tblconfig.Prefix;
                    }
                    if (tblconfig.Year == true)
                    {
                        format = tblconfig.YearFormat;
                        if (tblconfig.Month == true)
                        {
                            format += tblconfig.MonthFormat;
                            if (tblconfig.Date == true)
                            {
                                format += tblconfig.DateFormat;
                            }
                        }
                        try
                        {
                            format = now.ToString(format);
                        }
                        catch (Exception ex)
                        {

                        }
                        code += format + "/";
                    }

                    value = await GetCodeFRM(FormId, tblconfig.Year, tblconfig.Month, tblconfig.Date);

                    int length = value.Length + code.Length;
                    if (length < tblconfig.PaddingNo)
                    {
                        value = value.PadLeft(tblconfig.PaddingNo - length, '0');
                    }

                    code += value;

                }
                else
                {
                    code = (await new GenerateCode().GCode()).Data;
                }
                if (tblconfig.HasAddi == true)
                {
                    pref = pref + AddiPrefix + "/";
                }
            }
            catch (Exception ex)
            {
                code = (await new GenerateCode().GCode()).Data;
            }
            baseResponseDTO.Data = pref + code;
            return baseResponseDTO;

        }
        public async Task<string> GetCodeFRM(long FormId, bool Year, bool Month, bool Date)
        {
            BaseResponseDTO<string> baseResponseDTO = new BaseResponseDTO<string>();
            NPFContext context = new NPFContext();
            DateTime now = DateTime.Now;
            try
            {
                List<NPF_FRM_FormPage> dt = new List<NPF_FRM_FormPage>();

                List<NPF_FRM_FormPage> Plist = context.NPF_FRM_FormPage.Where(x => x.PFormId == FormId).ToList();
                if (Plist.Count > 0)
                {
                    dt.AddRange(Plist);
                }

                List<NPF_FRM_FormPage> Clist = context.NPF_FRM_FormPage.Where(x => x.Id == FormId).ToList();

                if (Clist.Count > 0)
                {
                    dt.AddRange(Clist);
                }

                if (dt.Count > 0)
                {
                    var dx = dt.OrderByDescending(y => y.Id).FirstOrDefault();

                    if (Year)
                    {

                        if (Month)
                        {
                            if (Date)
                            {
                                if (dx.CreatedDate.Year < DateTime.Now.Year && dx.CreatedDate.Month < DateTime.Now.Month && dx.CreatedDate.Date < DateTime.Now.Date)
                                {
                                    return "1";
                                }
                            }
                            else
                            {
                                if (dx.CreatedDate.Year < DateTime.Now.Year && dx.CreatedDate.Month < DateTime.Now.Month)
                                {
                                    return "1";
                                }
                            }
                        }
                        else
                        {
                            if (dx.CreatedDate.Year < DateTime.Now.Year)
                            {
                                return "1";
                            }
                        }

                    }

                    // List<NPF_FRM_FormPage> DTl = context.NPF_FRM_FormPage.Where(x => x.CreatedDate.Year == DateTime.Now.Year && x.Id == FormPageId).ToList();
                    List<NPF_FRM_FormPage> DTl = new List<NPF_FRM_FormPage>();
                    if (Year)
                    {
                        if (Month)
                        {
                            if (Date)
                            {
                                DTl = dt.Where(x => x.CreatedDate.Year == DateTime.Now.Year && x.CreatedDate.Month == DateTime.Now.Month && x.CreatedDate.Date == DateTime.Now.Date).ToList();
                            }
                            else
                            {
                                DTl = dt.Where(x => x.CreatedDate.Year == DateTime.Now.Year && x.CreatedDate.Month == DateTime.Now.Month).ToList();
                            }
                        }
                        else
                        {
                            DTl = dt.Where(x => x.CreatedDate.Year == DateTime.Now.Year).ToList();
                        }
                    }
                    else
                    {
                        DTl = dt;
                    }
                    return (DTl.Count() + 1).ToString();

                }
                return "1";

            }
            catch (Exception ex)
            {
                int _min = 100000000;
                int _max = 999999999;
                Random _rdm = new Random();
                baseResponseDTO.Data = "0121" + _rdm.Next(_min, _max).ToString();
            }

            return baseResponseDTO.Data;

        }

        public async Task<string> GetCode(string Code, bool Isreset, string reset, string Table)
        {
            BaseResponseDTO<string> baseResponseDTO = new BaseResponseDTO<string>();
            NPFContext context = new NPFContext();
            DateTime now = DateTime.Now;
            try
            {
                switch (Table)
                {
                    
                    case "NPF_SYS_Station":
                        {
                            var dt = context.NPF_SYS_Station.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    
                    case "NPF_SYS_ResourceType":
                        {
                            var dt = context.NPF_SYS_ResourceType.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_DIR_NAA":
                        {
                            var dt = context.NPF_DIR_NAA.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_Resource":
                        {
                            var dt = context.NPF_SYS_Resource.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    
                    case "NPF_SYS_Department":
                        {
                            var dt = context.NPF_SYS_Department.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_Division":
                        {
                            var dt = context.NPF_SYS_Division.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                   
                    case "NPF_SYS_OfficeZones":
                        {
                            var dt = context.NPF_SYS_OfficeZones.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_FRM_FormType":
                        {
                            var dt = context.NPF_FRM_FormType.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_FRM_Section":
                        {
                            var dt = context.NPF_FRM_Section.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_FRM_Form":
                        {
                            var dt = context.NPF_FRM_Form.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_FRM_FormPage":
                        {
                            var dt = context.NPF_FRM_FormPage.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    
                    case "NPF_STR_Store":
                        {
                            var dt = context.NPF_STR_Store.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_STR_StoreItem":
                        {
                            var dt = context.NPF_STR_StoreItem.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_STR_StoreTransaction":
                        {
                            var dt = context.NPF_STR_StoreTransaction.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_WorkFlowHeader":
                        {
                            var dt = context.NPF_SYS_WorkFlowHeader.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_WorkFlowDetails":
                        {
                            var dt = context.NPF_SYS_WorkFlowDetails.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                   
                    case "NPF_SCH_Schedule":
                        {
                            var dt = context.NPF_SCH_Schedule.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_ZoneType":
                        {
                            var dt = context.NPF_SYS_ZoneType.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_Zone":
                        {
                            var dt = context.NPF_SYS_Zone.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_ExternalAgencies":
                        {
                            var dt = context.NPF_SYS_ExternalAgencies.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_StationType":
                        {
                            var dt = context.NPF_SYS_StationType.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_IncidentType":
                        {
                            var dt = context.NPF_SYS_IncidentType.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_UserType":
                        {
                            var dt = context.NPF_SYS_UserType.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_NatureOfIncident":
                        {
                            var dt = context.NPF_SYS_NatureOfIncident.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_IncidentSubType":
                        {
                            var dt = context.NPF_SYS_IncidentSubType.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_IncidentCode":
                        {
                            var dt = context.NPF_SYS_IncidentCode.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_AssetMake":
                        {
                            var dt = context.NPF_SYS_AssetMake.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_Rank":
                        {
                            var dt = context.NPF_SYS_Rank.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_AssetModel":
                        {
                            var dt = context.NPF_SYS_AssetModel.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_Shift":
                        {
                            var dt = context.NPF_SYS_Shift.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_Equipment":
                        {
                            var dt = context.NPF_SYS_Equipment.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_Vehicle":
                        {
                            var dt = context.NPF_SYS_Vehicle.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_TRN_Incident":
                        {
                            var dt = context.NPF_TRN_Incident.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_TRN_IncidentNumber":
                        {
                            var dt = context.NPF_TRN_Incident.Where(z =>z.IncidentNo != string.Empty).OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.IncidentNo, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.IncidentNo)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_Roster":
                        {
                            var dt = context.NPF_SYS_Roster.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_PDA":
                        {
                            var dt = context.NPF_SYS_PDA.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                    case "NPF_SYS_Priority":
                        {
                            var dt = context.NPF_SYS_Priority.OrderByDescending(y => y.Id).FirstOrDefault();
                            if (dt != null)
                            {
                                if (Isreset)
                                {
                                    return await Generator(Code, dt.Code, dt.CreatedDate, reset);
                                }
                                else
                                {
                                    string Num = (await GetNumber(dt.Code)).Remove(0, Code.Length);
                                    return (Convert.ToInt32(Num) + 1).ToString();
                                }
                            }
                            return "1";
                        }
                }

            }
            catch (Exception ex)
            {
                int _min = 100000000;
                int _max = 999999999;
                Random _rdm = new Random();
                baseResponseDTO.Data = "0121" + _rdm.Next(_min, _max).ToString();
            }

            return baseResponseDTO.Data;

        }

        public async Task<string> Generator(string Code, string dtcode, DateTime Createddate, string reset)
        {
            DateTime now = DateTime.Now;
            if (reset == "Year")
            {
                if (Createddate.Year == now.Year)
                {
                    return await GenNumber(dtcode, Code);
                }
            }
            else if (reset == "Month")
            {
                if (Createddate.Year == now.Year && Createddate.Month == now.Month)
                {
                    return await GenNumber(dtcode, Code);
                }
            }
            else if (reset == "Day")
            {
                if (Createddate.Year == now.Year && Createddate.Month == now.Month && Createddate.Day == now.Day)
                {
                    return await GenNumber(dtcode, Code);
                }

            }
            return "1";
        }

        public static async Task<string> GetNumber(string input)
        {
            return Regex.Replace(input, "[^0-9.]", "");
        }
        public static async Task<string> GenNumber(string dtcode, string Code)
        {
            string Num = (await GetNumber(dtcode)).Remove(0, Code.Length);
            return (Convert.ToInt32(Num) + 1).ToString();
        }

    }
}
