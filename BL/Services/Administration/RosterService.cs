using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using Org.BouncyCastle.Pqc.Crypto.Lms;

namespace BL.Services.Administration
{
    public class RosterService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public RosterService service;
        public RosterService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<NPF_SYS_RosterShiftResourceMapping>> GetAll()
        {
            BaseResponseDTO<List<NPF_SYS_RosterShiftResourceMapping>> dto = new BaseResponseDTO<List<NPF_SYS_RosterShiftResourceMapping>>();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";
            DateTime date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            List<NPF_SYS_RosterShiftResourceMapping> result = context.NPF_SYS_RosterShiftResourceMapping.Where(x => x.DeleteStatus == false && x.Date >= firstDayOfMonth && x.Date <= lastDayOfMonth).ToList();
            try
            {
                foreach (NPF_SYS_RosterShiftResourceMapping res in result)
                {
                    res.Roster = context.NPF_SYS_Roster.Where(x => x.Id == res.RosterId && x.DeleteStatus != true).FirstOrDefault();
                    res.Shift = context.NPF_SYS_Shift.Where(x => x.Id == res.ShiftId && x.DeleteStatus != true).FirstOrDefault();
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

        public BaseResponseDTO<RosterShiftResourceMappingDTO> GetById(long? Id)
        {
            BaseResponseDTO<RosterShiftResourceMappingDTO> dto = new BaseResponseDTO<RosterShiftResourceMappingDTO>();
            dto.Data = new RosterShiftResourceMappingDTO();
            NPF_SYS_Roster dt = new NPF_SYS_Roster();
            List<NPF_SYS_RosterShiftResourceMapping> rsrml = new List<NPF_SYS_RosterShiftResourceMapping>();
            NPF_SYS_RosterShiftResourceMapping rsrm = new NPF_SYS_RosterShiftResourceMapping();
            try
            {
                string errorMsg = "No Data Found";
                rsrm = context.NPF_SYS_RosterShiftResourceMapping.Where(x => x.Id == Id && x.DeleteStatus != true).FirstOrDefault();
                dt = context.NPF_SYS_Roster.Where(x => x.Id == rsrm.RosterId && x.DeleteStatus != true).FirstOrDefault();
                rsrml = context.NPF_SYS_RosterShiftResourceMapping.Where(x => x.RosterId == dt.Id && x.DeleteStatus != true).ToList();

                if (dt == null)
                {
                    dto.Data.Roster = dt;
                    dto.Data.RSR = null;
                    dto.QryResult = queryResult.FAILED;
                }
                else
                {
                    dto.Data.Roster = dt;
                    dto.Data.RSR = rsrml;
                    dto.QryResult = queryResult.SUCEEDED;
                }

                return dto;
            }
            catch (Exception ex)
            {
                dto.Data.Roster = dt;
                dto.Data.RSR = null;
                dto.QryResult = queryResult.FAILED;
                return dto;
            }

        }

        public async Task<BaseResponseDTO<RosterShiftResourceMappingDTO>> GetIndividualShifts(BaseRequestDTO<RosterShiftResourceMappingDTO> dataToSave)
        {
            BaseResponseDTO<RosterShiftResourceMappingDTO> Bdto = new BaseResponseDTO<RosterShiftResourceMappingDTO>();
            RosterShiftResourceMappingDTO dto = new RosterShiftResourceMappingDTO();
            List<ShiftDateDTO> shiftList = new List<ShiftDateDTO>();
            List<ShiftDateDTO> shiftToReturn = new List<ShiftDateDTO>();

            try
            {

                dataToSave.Data.Shift = ConvertDtoDays(dataToSave.Data.Shift);

                if (dataToSave.Data.Shift != null)
                {
                    List<NPF_SYS_RosterShiftResourceMapping> rsrml = context.NPF_SYS_RosterShiftResourceMapping.Where(x => x.RosterId == dataToSave.Data.Roster.Id).ToList();
                    for (var day = dataToSave.Data.Roster.RosterFrom; day.Date <= dataToSave.Data.Roster.RosterTo; day = day.AddDays(1))
                    {
                        if (rsrml.Count() != 0)
                        {
                            NPF_SYS_RosterShiftResourceMapping x1 = context.NPF_SYS_RosterShiftResourceMapping.Where(x => x.RosterId == dataToSave.Data.Roster.Id && x.Date == day && x.DeleteStatus == false).FirstOrDefault();
                            if (x1 != null)
                            {
                                ShiftDateDTO rosterShift = new ShiftDateDTO();
                                rosterShift.ShiftId = x1.ShiftId;
                                rosterShift.day = day.DayOfWeek.ToString();
                                rosterShift.date = day;
                                rosterShift.isIndividual = x1.Exists;
                                shiftList.Add(rosterShift);
                            }
                            else
                            {
                                foreach (ShiftDateDTO sd in dataToSave.Data.Shift)
                                {
                                    if (day.DayOfWeek.ToString() == sd.day)
                                    {
                                        try
                                        {
                                            ShiftDateDTO rosterShift = new ShiftDateDTO();
                                            rosterShift.ShiftId = sd.ShiftId;
                                            rosterShift.date = day;
                                            rosterShift.day = day.DayOfWeek.ToString();
                                            shiftList.Add(rosterShift);
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (ShiftDateDTO sd in dataToSave.Data.Shift)
                            {
                                if (day.DayOfWeek.ToString() == sd.day)
                                {
                                    try
                                    {
                                        ShiftDateDTO rosterShift = new ShiftDateDTO();
                                        rosterShift.ShiftId = sd.ShiftId;
                                        rosterShift.date = day;
                                        rosterShift.day = day.DayOfWeek.ToString();
                                        shiftList.Add(rosterShift);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                        }
                    }


                    foreach (ShiftDateDTO shiftx in dataToSave.Data.Shift)
                    {
                        foreach (ShiftDateDTO shiftlistx in shiftList)
                        {
                            if (shiftx.day == shiftlistx.day)
                            {

                                if (shiftlistx.ShiftId != shiftx.ShiftId && shiftlistx.isIndividual == false)
                                {
                                    shiftlistx.ShiftId = shiftx.ShiftId;
                                }
                                shiftToReturn.Add(shiftlistx);
                            }
                        }
                    }
                }

                List<DropDown> Ddl = new List<DropDown>();
                DropDown Dd = new DropDown();
                List<NPF_SYS_Shift> shift = context.NPF_SYS_Shift.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Shift";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_Shift s in shift)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = s.Id;
                    Ddi.text = s.Name;
                    Dd.items.Add(Ddi);
                    Ddl.Add(Dd);
                }

                shiftToReturn = shiftToReturn.OrderBy(e => e.date).ToList();

                dto.Shift = new List<ShiftDateDTO>();

                dto.Shift.AddRange(shiftToReturn);
                dto.Shift = shiftToReturn;
                dto.dropdown = Ddl;
                Bdto.Data = new RosterShiftResourceMappingDTO();
                Bdto.Data = dto;
                Bdto.TotalItems = shiftList.Count();
                Bdto.QryResult = new QueryResult().SUCEEDED;



            }

            catch (Exception ex)
            {
                dto.Shift = new List<ShiftDateDTO>();
                dto.RSR = new List<NPF_SYS_RosterShiftResourceMapping>();
                Bdto.TotalItems = 0;
                Bdto.AdditionalData = ex.ToString();
                Bdto.QryResult = new QueryResult().FAILED;
            }

            return Bdto;
        }

        public async Task<BaseResponseDTO<RosterShiftResourceMappingDTO>> GetDaySelectionRosterId(long Id)
        {
            BaseResponseDTO<RosterShiftResourceMappingDTO> Bdto = new BaseResponseDTO<RosterShiftResourceMappingDTO>();
            RosterShiftResourceMappingDTO dto = new RosterShiftResourceMappingDTO();
            NPF_SYS_RosterShiftResourceMapping rsrm = new NPF_SYS_RosterShiftResourceMapping();
            List<NPF_SYS_RosterShiftResourceMapping> rsrml = new List<NPF_SYS_RosterShiftResourceMapping>();
            NPF_SYS_Roster rs = new NPF_SYS_Roster();
            List<ShiftDateDTO> shiftList = new List<ShiftDateDTO>();

            try
            {
                string errorMsg = "No Data Found";
                rsrml = context.NPF_SYS_RosterShiftResourceMapping.Where(x => x.RosterId == Id && x.DeleteStatus != true).ToList();

                for (int i = 0; i < rsrml.Count(); i++)
                {
                    ShiftDateDTO sd = new ShiftDateDTO();
                    sd.day = rsrml[i].Date.DayOfWeek.ToString();
                    sd.ShiftId = rsrml[i].ShiftId;
                    sd.isChecked = true;
                    sd.date = rsrml[i].Date;
                    sd.isIndividual = rsrml[i].Exists;
                    rsrml[i].day = sd.day;
                    shiftList.Add(sd);

                }

                shiftList = ConvertDaystoD(shiftList);

                List<DropDown> Ddl = new List<DropDown>();
                DropDown Dd = new DropDown();
                List<NPF_SYS_Shift> shift = context.NPF_SYS_Shift.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Shift";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_Shift s in shift)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = s.Id;
                    Ddi.text = s.Name;
                    Dd.items.Add(Ddi);
                    Ddl.Add(Dd);
                }

                dto.Shift = new List<ShiftDateDTO>();
                rsrml = rsrml.OrderBy(e => e.Date).ToList();
                dto.Shift.AddRange(shiftList);
                dto.Shift = shiftList;
                dto.RSR = rsrml;
                dto.dropdown = Ddl;
                Bdto.Data = new RosterShiftResourceMappingDTO();
                Bdto.Data = dto;
                Bdto.TotalItems = shiftList.Count();
                Bdto.QryResult = new QueryResult().SUCEEDED;



            }

            catch (Exception ex)
            {
                dto.Shift = new List<ShiftDateDTO>();
                dto.RSR = new List<NPF_SYS_RosterShiftResourceMapping>();
                Bdto.TotalItems = 0;
                Bdto.AdditionalData = ex.ToString();
                Bdto.QryResult = new QueryResult().FAILED;
            }

            return Bdto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<RosterShiftResourceMappingDTO> dataToSave)
        {
            dataToSave.Data.Shift = ConvertDtoDays(dataToSave.Data.Shift);

            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            List<NPF_SYS_RosterShiftResourceMapping> TRM = new List<NPF_SYS_RosterShiftResourceMapping>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            bool rosterExist = false;
            try
            {
                //check if there is atleast 1 roster within date range
                if (dataToSave.Data.Shift != null)
                {
                    for (var day = dataToSave.Data.Roster.RosterFrom; day.Date <= dataToSave.Data.Roster.RosterTo; day = day.AddDays(1))
                    {
                        foreach (ShiftDateDTO sd in dataToSave.Data.Shift)
                        {
                            if (day.DayOfWeek.ToString() == sd.day)
                            {
                                rosterExist = true;
                            }
                        }
                    }
                }

                if (rosterExist == false)
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "No Shift Exists Within Roster Period";
                    BaseDto.QryResult = new QueryResult().FAILED;
                    return BaseDto;

                }

                //check if roster exists
                if (context.NPF_SYS_Roster.Any(x => x.Name.ToLower() == dataToSave.Data.Roster.Name.ToLower() && x.DeleteStatus != true))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Roster Already Exist";
                    BaseDto.QryResult = new QueryResult().FAILED;
                    return BaseDto;
                }
                else
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_Roster");
                    dataToSave.Data.Roster.Code = BaseDtoS.Data; //"ROS" + 
                    context.NPF_SYS_Roster.Add(dataToSave.Data.Roster);
                    context.SaveChanges();
                    try
                    {
                        //save resources and teams
                        if (dataToSave.Data.RSR != null)
                        {
                            foreach (NPF_SYS_RosterShiftResourceMapping rsrm in dataToSave.Data.RSR)
                            {
                                try
                                {
                                    NPF_SYS_RosterResourceMapping tm = new NPF_SYS_RosterResourceMapping();
                                    tm.RosterId = dataToSave.Data.Roster.Id;
                                    tm.ResourceId = rsrm.ResourceId;
                                    context.NPF_SYS_RosterResourceMapping.Add(tm);
                                    context.SaveChanges();

                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }

                        //meets condition when user clicked on edit individual shifts
                        if (dataToSave.Data.ShiftUpdate.Count() > 0)
                        {
                            List<NPF_SYS_RosterShiftResourceMapping> indivualList = new List<NPF_SYS_RosterShiftResourceMapping>();
                            foreach (ShiftDateDTO shiftMap in dataToSave.Data.ShiftUpdate)
                            {
                                NPF_SYS_RosterShiftResourceMapping rsrm = new NPF_SYS_RosterShiftResourceMapping();
                                rsrm.RosterId = dataToSave.Data.Roster.Id;
                                rsrm.ShiftId = shiftMap.ShiftId;
                                rsrm.Date = shiftMap.date;

                                foreach (ShiftDateDTO x in dataToSave.Data.Shift)
                                {
                                    if (x.day == rsrm.Date.DayOfWeek.ToString() && x.ShiftId != rsrm.ShiftId)
                                    {
                                        indivualList.Add(rsrm);
                                    }
                                }

                                context.NPF_SYS_RosterShiftResourceMapping.Add(rsrm);
                                context.SaveChanges();
                            }

                            foreach (NPF_SYS_RosterShiftResourceMapping y in indivualList)
                            {
                                y.Exists = true;
                                context.NPF_SYS_RosterShiftResourceMapping.Update(y);
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            //meets condition when user only selected shifts from grid (No Individual)
                            if (dataToSave.Data.Shift != null)
                            {
                                for (var day = dataToSave.Data.Roster.RosterFrom; day.Date <= dataToSave.Data.Roster.RosterTo; day = day.AddDays(1))
                                {
                                    foreach (ShiftDateDTO sd in dataToSave.Data.Shift)
                                    {
                                        if (day.DayOfWeek.ToString() == sd.day)
                                        {
                                            try
                                            {
                                                NPF_SYS_RosterShiftResourceMapping rsrm = new NPF_SYS_RosterShiftResourceMapping();
                                                rsrm.RosterId = dataToSave.Data.Roster.Id;
                                                rsrm.ShiftId = sd.ShiftId;
                                                rsrm.Date = day;
                                                context.NPF_SYS_RosterShiftResourceMapping.Add(rsrm);
                                                context.SaveChanges();
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }

                    BaseDto.Data = true;
                    BaseDto.QryResult = new QueryResult().SUCEEDED;
                }
            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<RosterShiftResourceMappingDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            bool rosterExist = false;

            try
            {
                //check if there is atleast 1 roster within date range
                if (dataToUpdate.Data.Shift != null)
                {
                    dataToUpdate.Data.Shift = ConvertDtoDays(dataToUpdate.Data.Shift);
                    for (var day = dataToUpdate.Data.Roster.RosterFrom; day.Date <= dataToUpdate.Data.Roster.RosterTo; day = day.AddDays(1))
                    {
                        foreach (ShiftDateDTO sd in dataToUpdate.Data.Shift)
                        {
                            if (day.DayOfWeek.ToString() == sd.day)
                            {
                                rosterExist = true;
                            }
                        }
                    }
                }

                if (rosterExist == false)
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "No Shift Exists Within Roster Period";
                    BaseDto.QryResult = new QueryResult().FAILED;
                    return BaseDto;

                }

                if (context.NPF_SYS_Roster.Any(x => x.Name.ToLower() == dataToUpdate.Data.Roster.Name.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Data.Roster.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Roster Already Exist";
                    BaseDto.QryResult = new QueryResult().FAILED;
                    return BaseDto;
                }

                if (dataToUpdate.Data.RSR != null)
                {
                    List<NPF_SYS_RosterResourceMapping> rsl = context.NPF_SYS_RosterResourceMapping.Where(x => x.RosterId == dataToUpdate.Data.Roster.Id && x.DeleteStatus == false).ToList();

                    foreach (NPF_SYS_RosterShiftResourceMapping rs in dataToUpdate.Data.RSR)
                    {
                        var check = rsl.Where(x => x.RosterId == dataToUpdate.Data.Roster.Id && x.ResourceId == rs.ResourceId).FirstOrDefault();
                        if (check == null)
                        {
                            try
                            {
                                NPF_SYS_RosterResourceMapping rrmn = new NPF_SYS_RosterResourceMapping();
                                rrmn.RosterId = dataToUpdate.Data.Roster.Id;
                                rrmn.ResourceId = rs.ResourceId;
                                context.NPF_SYS_RosterResourceMapping.Add(rrmn);
                                context.SaveChanges();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        else
                        {
                            context.NPF_SYS_RosterResourceMapping.Update(check);
                            rsl.Remove(check);
                            context.SaveChanges();
                        }


                    }
                    if (rsl.Count() > 0)
                    {
                        foreach (NPF_SYS_RosterResourceMapping rs in rsl)
                        {
                            rs.DeleteStatus = true;
                            context.NPF_SYS_RosterResourceMapping.Update(rs);
                            context.SaveChanges();
                        }
                    }

                }
                else
                {
                    List<NPF_SYS_RosterResourceMapping> rsl = context.NPF_SYS_RosterResourceMapping.Where(x => x.RosterId == dataToUpdate.Data.Roster.Id && x.DeleteStatus == false).ToList();
                    if (rsl.Count() > 0)
                    {
                        foreach (NPF_SYS_RosterResourceMapping rs in rsl)
                        {
                            rs.DeleteStatus = true;
                            context.NPF_SYS_RosterResourceMapping.Update(rs);
                            context.SaveChanges();
                        }
                    }
                }

                if (dataToUpdate.Data.ShiftUpdate.Count != 0)
                {
                    List<NPF_SYS_RosterShiftResourceMapping> rsrmlDb = context.NPF_SYS_RosterShiftResourceMapping.Where(x => x.RosterId == dataToUpdate.Data.Roster.Id && x.DeleteStatus == false).ToList();

                    foreach (ShiftDateDTO shift in dataToUpdate.Data.ShiftUpdate)
                    {
                        var check = rsrmlDb.Where(x => x.RosterId == dataToUpdate.Data.Roster.Id && x.ShiftId == shift.ShiftId && x.Date == shift.date).FirstOrDefault();
                        List<NPF_SYS_RosterShiftResourceMapping> indivualList = new List<NPF_SYS_RosterShiftResourceMapping>();
                        if (check == null)
                        {
                            try
                            {
                                NPF_SYS_RosterShiftResourceMapping rsrm = new NPF_SYS_RosterShiftResourceMapping();
                                rsrm.RosterId = dataToUpdate.Data.Roster.Id;
                                rsrm.ShiftId = shift.ShiftId;
                                rsrm.Date = shift.date;
                                var day = shift.date.DayOfWeek.ToString();

                                var rosterShiftList = context.NPF_SYS_RosterShiftResourceMapping.Where(x => x.RosterId == dataToUpdate.Data.Roster.Id && x.DeleteStatus == false /*&& x.ShiftId == Guid.Parse(shift.ShiftId) && x.Exists == false*/).ToList();
                                foreach (NPF_SYS_RosterShiftResourceMapping x in rosterShiftList)
                                {
                                    if (x.Date.DayOfWeek.ToString() == day && x.Exists == false && x.Date != rsrm.Date)
                                    {
                                        indivualList.Add(x);
                                    }
                                }
                                if (indivualList.Count() >= 1)
                                {
                                    rsrm.Exists = true;
                                }

                                context.NPF_SYS_RosterShiftResourceMapping.Add(rsrm);
                                context.SaveChanges();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        else
                        {
                            context.NPF_SYS_RosterShiftResourceMapping.Update(check);
                            rsrmlDb.Remove(check);
                            context.SaveChanges();
                        }
                    }

                    if (rsrmlDb.Count() > 0)
                    {
                        foreach (NPF_SYS_RosterShiftResourceMapping rs in rsrmlDb)
                        {
                            rs.DeleteStatus = true;
                            context.NPF_SYS_RosterShiftResourceMapping.Update(rs);
                            context.SaveChanges();
                        }
                    }
                }
                else
                {
                    if (dataToUpdate.Data.Shift != null)
                    {


                        List<NPF_SYS_RosterShiftResourceMapping> rsrml = new List<NPF_SYS_RosterShiftResourceMapping>();

                        for (var day = dataToUpdate.Data.Roster.RosterFrom; day.Date <= dataToUpdate.Data.Roster.RosterTo; day = day.AddDays(1))
                        {

                            foreach (ShiftDateDTO sd in dataToUpdate.Data.Shift)
                            {

                                if (day.DayOfWeek.ToString() == sd.day)
                                {
                                    try
                                    {
                                        NPF_SYS_RosterShiftResourceMapping rsrm = new NPF_SYS_RosterShiftResourceMapping();
                                        rsrm.RosterId = dataToUpdate.Data.Roster.Id;
                                        rsrm.ShiftId = sd.ShiftId;
                                        rsrm.Date = day;
                                        rsrml.Add(rsrm);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                        }

                        List<NPF_SYS_RosterShiftResourceMapping> rsrmlDb = context.NPF_SYS_RosterShiftResourceMapping.Where(x => x.RosterId == dataToUpdate.Data.Roster.Id && x.DeleteStatus == false).ToList();

                        foreach (NPF_SYS_RosterShiftResourceMapping rs in rsrml)
                        {
                            var check = rsrmlDb.Where(x => x.RosterId == rs.RosterId && x.ShiftId == rs.ShiftId && x.Date == rs.Date).FirstOrDefault();
                            if (check == null)
                            {
                                try
                                {
                                    context.NPF_SYS_RosterShiftResourceMapping.Add(rs);
                                    context.SaveChanges();
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            else
                            {
                                context.NPF_SYS_RosterShiftResourceMapping.Update(check);
                                rsrmlDb.Remove(check);
                                context.SaveChanges();
                            }

                        }
                        if (rsrmlDb.Count() > 0)
                        {
                            foreach (NPF_SYS_RosterShiftResourceMapping rs1 in rsrmlDb)
                            {
                                rs1.DeleteStatus = true;
                                context.NPF_SYS_RosterShiftResourceMapping.Update(rs1);
                                context.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        List<NPF_SYS_RosterShiftResourceMapping> rsl = context.NPF_SYS_RosterShiftResourceMapping.Where(x => x.RosterId == dataToUpdate.Data.Roster.Id && x.DeleteStatus == false).ToList();
                        if (rsl.Count() > 0)
                        {
                            foreach (NPF_SYS_RosterShiftResourceMapping rs in rsl)
                            {
                                rs.DeleteStatus = true;
                                context.NPF_SYS_RosterShiftResourceMapping.Update(rs);
                                context.SaveChanges();
                            }
                        }
                    }

                }


                context.NPF_SYS_Roster.Update(dataToUpdate.Data.Roster);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Updating Record";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<bool> Delete(BaseRequestDTO<long> dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                List<NPF_SYS_RosterShiftResourceMapping> rsrml = context.NPF_SYS_RosterShiftResourceMapping.Where(x => x.Id == dataToDelId.Data && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_RosterShiftResourceMapping rsm in rsrml)
                {
                    rsm.DeleteStatus = true;
                    context.NPF_SYS_RosterShiftResourceMapping.Update(rsm);
                    context.SaveChanges();
                }
                List<NPF_SYS_RosterShiftResourceMapping> rsrm = context.NPF_SYS_RosterShiftResourceMapping.Where(x => x.RosterId == rsrml[0].RosterId && x.DeleteStatus == false).ToList();
                if (rsrm.Count() == 0)
                {
                    NPF_SYS_Roster rs = context.NPF_SYS_Roster.Where(x => x.Id == rsrml[0].RosterId).FirstOrDefault();
                    rs.DeleteStatus = true;
                    context.NPF_SYS_Roster.Update(rs);
                    context.SaveChanges();
                }

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

        public BaseResponseDTO<List<NPF_SYS_RosterResourceMapping>> GetDetailsById(long Id)
        {
            //init return dto
            BaseResponseDTO<List<NPF_SYS_RosterResourceMapping>> dto = new BaseResponseDTO<List<NPF_SYS_RosterResourceMapping>>();
            dto.Data = new List<NPF_SYS_RosterResourceMapping>();
            //init result
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";
            string sQryResult = queryResult.FAILED;
            //init to get listing 
            List<NPF_SYS_RosterResourceMapping> resList = new List<NPF_SYS_RosterResourceMapping>();
            //get listing
            resList = context.NPF_SYS_RosterResourceMapping.Where(x => x.RosterId == Id && x.DeleteStatus != true).ToList();
            if (resList.Count > 0)
            {
                //loop in listing to add to return dto
                foreach (NPF_SYS_RosterResourceMapping re in resList)
                {
                    re.Resource = context.NPF_SYS_Resource.Where((x) => x.Id == re.ResourceId).FirstOrDefault();
                    dto.Data.Add(re);
                }
            }

            dto.QryResult = queryResult.SUCEEDED;
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

                //Get Shift Dropdown
                List<NPF_SYS_Shift> shift = context.NPF_SYS_Shift.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Shift";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_Shift s in shift)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = s.Id;
                    Ddi.text = s.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);
                dto.Data = Ddl;

                //Get Resource Dropdown
                List<NPF_SYS_Resource> Rel = context.NPF_SYS_Resource.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Resource";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_Resource re in Rel)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = re.Id;
                    Ddi.text = re.NAA.FullName;
                    Ddi.lot = re.Code;
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

        public BaseResponseDTO<List<NPF_SYS_RosterShiftResourceMapping>> filterDatatableByRosterPeriod(BaseRequestDTO<NPF_SYS_Roster> roster)
        {
            BaseResponseDTO<List<NPF_SYS_RosterShiftResourceMapping>> dto = new BaseResponseDTO<List<NPF_SYS_RosterShiftResourceMapping>>();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";
            List<NPF_SYS_RosterShiftResourceMapping> result = context.NPF_SYS_RosterShiftResourceMapping.Where(x => x.DeleteStatus == false && x.Date >= roster.Data.RosterFrom && x.Date <= roster.Data.RosterTo).ToList();
            try
            {
                foreach (NPF_SYS_RosterShiftResourceMapping res in result)
                {
                    res.Roster = context.NPF_SYS_Roster.Where(x => x.Id == res.RosterId && x.DeleteStatus != true).FirstOrDefault();
                    res.Shift = context.NPF_SYS_Shift.Where(x => x.Id == res.ShiftId && x.DeleteStatus != true).FirstOrDefault();
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

        public List<ShiftDateDTO> ConvertDaystoD(List<ShiftDateDTO> shift)
        {
            foreach (ShiftDateDTO sd in shift)
            {
                switch (sd.day)
                {
                    case "Monday":
                        sd.day = "Mon";
                        break;
                    case "Tuesday":
                        sd.day = "Tue";
                        break;
                    case "Wednesday":
                        sd.day = "Wed";
                        break;
                    case "Thursday":
                        sd.day = "Thu";
                        break;
                    case "Friday":
                        sd.day = "Fri";
                        break;
                    case "Saturday":
                        sd.day = "Sat";
                        break;
                    case "Sunday":
                        sd.day = "Sun";
                        break;
                }
            }
            return shift;

        }

        public List<ShiftDateDTO> ConvertDtoDays(List<ShiftDateDTO> shift)
        {
            foreach (ShiftDateDTO s in shift)
            {
                switch (s.day)
                {
                    case "Mon":
                        s.day = "Monday";
                        break;
                    case "Tue":
                        s.day = "Tuesday";
                        break;
                    case "Wed":
                        s.day = "Wednesday";
                        break;
                    case "Thu":
                        s.day = "Thursday";
                        break;
                    case "Fri":
                        s.day = "Friday";
                        break;
                    case "Sat":
                        s.day = "Saturday";
                        break;
                    case "Sun":
                        s.day = "Sunday";
                        break;
                }
            }
            return shift;

        }
    }
}
