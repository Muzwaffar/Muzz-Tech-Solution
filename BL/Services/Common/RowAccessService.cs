using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class RowAccessService
    {
        public List<long> GetRowAccessById(long RowId, ModuleName Mn) // Return list of users Ids who has access
        {
            List<long> UIds = new List<long>();
            NPFContext context = new NPFContext();
            try
            {
                List<NPF_SYS_RowAccess> RAL = context.NPF_SYS_RowAccess.Where(x => x.ModuleName == Mn && x.ModuleId == RowId).ToList();
                foreach (NPF_SYS_RowAccess Ra in RAL)
                {
                    switch (Ra.Struc)
                    {
                        case RowStructType.User:
                            var User = context.NPF_SYS_User.Where(x => x.Id == Convert.ToInt32(Ra.StrucId)).FirstOrDefault();
                            if (User != null)
                            {
                                UIds.Add(User.Id);
                            }
                            break;
                        case RowStructType.Role:
                            var Users = (from a in context.UserRoles
                                         join b in context.NPF_SYS_User on a.UserId equals b.AId
                                         where a.RoleId == Ra.StrucId
                                         select b.Id);
                            if (Users != null)
                            {
                                UIds.AddRange(Users);
                            }
                            break;
                        default:
                            break;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return UIds;
        }
        public List<MAccessDTO> GetMenuAccessByIds(Guid? ModuleId, long UserId) // Return User Access on Specific screen
        {
            List<MAccessDTO> dto = new List<MAccessDTO>();
            NPFContext context = new NPFContext();
            try
            {
                if (ModuleId != null)
                {
                    string AUserid = context.NPF_SYS_User.Where(x => x.Id == UserId).FirstOrDefault().AId;
                    string RoleId = context.UserRoles.Where(x => x.UserId == AUserid).FirstOrDefault().RoleId;
                    List<NPF_SYS_AccessRightsDetails> dt = context.NPF_SYS_AccessRightsDetails.Where(x => x.AccessRights.Module.Id == ModuleId && x.AccessRights.Role.Id == RoleId).ToList();
                    foreach (NPF_SYS_AccessRightsDetails g in dt)
                    {
                        MAccessDTO d = new MAccessDTO()
                        {
                            AccessType = g.AOT,
                            HasAccess = g.Permission
                        };
                        dto.Add(d);
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return dto;
        }
        public List<MAccessDTO> GetMenuAccessByIds(Guid? ModuleId) // Return User Access on Specific screen
        {
            List<MAccessDTO> dto = new List<MAccessDTO>();
            NPFContext context = new NPFContext();
            try
            {
                UserMatrixDTO UM = new UserMatrixDTO();
                UM = new MatrixService().sGetMatrixUserByUserID();//Service All user's matrix access into UM
                if (ModuleId != null)
                {
                    string AUserid = context.NPF_SYS_User.Where(x => x.Id == UM.UID).FirstOrDefault().AId;
                    string RoleId = context.UserRoles.Where(x => x.UserId == AUserid).FirstOrDefault().RoleId;
                    List<NPF_SYS_AccessRightsDetails> dt = context.NPF_SYS_AccessRightsDetails.Where(x => x.AccessRights.Module.Id == ModuleId && x.AccessRights.Role.Id == RoleId).ToList();
                    foreach (NPF_SYS_AccessRightsDetails g in dt)
                    {
                        MAccessDTO d = new MAccessDTO()
                        {
                            AccessType = g.AOT,
                            HasAccess = g.Permission
                        };
                        dto.Add(d);
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return dto;
        }

    }
}
