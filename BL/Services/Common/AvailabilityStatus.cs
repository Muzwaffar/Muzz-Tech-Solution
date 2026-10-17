using BL.Constants;
using BL.Models.Common;
using BL.Models.Schedule;
using BL.Services.Source;
using DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class AvailabilityStatus
    {
        private NPFContext context;
        private QueryResult queryResult;
        public SourceControlService Sservice;
        public AvailabilityStatus()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Sservice = new SourceControlService();
        }
        public async Task<BaseResponseDTO<bool>> CheckResAvailById(GetByDateDTO dto)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                BaseResponseDTO<bool> roster = await CheckRosterAvailById(dto);
                if (roster.QryResult == new QueryResult().SUCEEDED)
                {
                    string status = "";
                    bool rst = roster.AdditionalData == "Available" ? true : false;
                    bool sst = roster.AdditionalData == "Available" ? true : false;
                    if (rst && sst)
                    {
                        status = "Available";
                    }
                    else if (!rst)
                    {
                        status = "Not Present";
                    }
                    else if (rst && !sst)
                    {
                        status = "Booked";
                    }
                    BaseDto.Data = true;
                    BaseDto.AdditionalData = status;
                    BaseDto.QryResult = new QueryResult().SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "An Error Occured";
                    BaseDto.QryResult = new QueryResult().FAILED;
                }

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> CheckRosterAvailById(GetByDateDTO dto)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                bool check = context.NPF_SYS_RosterResourceMapping
                .Join(context.NPF_SYS_RosterShiftResourceMapping, a => a.RosterId, b => b.RosterId, (a, b) => new { a, b })
                .Join(context.NPF_SYS_Shift, ab => ab.b.ShiftId, c => c.Id, (ab, c) => new { ab.a, ab.b, c })
                .AsEnumerable() // Switch to client-side evaluation
                .Where(x => x.a.ResourceId == dto.Id
                             && !x.a.DeleteStatus
                             && !x.b.DeleteStatus
                             && !x.c.DeleteStatus
                             && ((x.c.TimeFrom.TimeOfDay <= dto.StartTime.TimeOfDay && x.c.TimeTo.TimeOfDay >= dto.StartTime.TimeOfDay)
                                 || (x.c.TimeFrom.TimeOfDay <= dto.EndTime.TimeOfDay && x.c.TimeTo.TimeOfDay >= dto.EndTime.TimeOfDay))
                            )
                .Any();
                BaseDto.Data = true;
                BaseDto.AdditionalData = check ? "Available" : "Not Present";
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }

    }
}
