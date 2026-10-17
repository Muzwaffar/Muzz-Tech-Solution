using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models.Access;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Access.AccessRights
{
    public class AccessRightService
    {

        private NPFContext context;
        public AccessRightService()
        {
            context = new NPFContext();
        }
        ////Comments

        #region new
        public BaseResponseDTO<List<AccessRightsDTO>> GetAll()
        {
            BaseResponseDTO<List<AccessRightsDTO>> dto = new BaseResponseDTO<List<AccessRightsDTO>>();
            AccessRightsDTO user = new AccessRightsDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";
            try
            {

                List<AccessRightsDTO> result = (from r in context.Roles
                                                where r.DeleteStatus == false
                                                select new AccessRightsDTO
                                                {
                                                    Id = r.Id,
                                                    RoleName = r.Name,
                                                }).ToList();
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<AccessRightsDTO>();
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }
        public BaseResponseDTO<AccessRightsDTO> GetById(string Id)
        {
            BaseResponseDTO<AccessRightsDTO> dto = new BaseResponseDTO<AccessRightsDTO>();

            try
            {
                AccessRightsDTO AR = new AccessRightsDTO();
                var Roles = context.Roles.FirstOrDefault(d => d.Id == Id);
                AR.RoleName = Roles.Name;
                AR.Id = Roles.Id;
                dto.Data = AR;
                dto.TotalItems = 0;
                dto.QryResult = new QueryResult().SUCEEDED;

            }

            catch (Exception ex)
            {
                dto.Data = new AccessRightsDTO();
                dto.TotalItems = 0;
                dto.AdditionalData = ex.ToString();
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;
        }
        public BaseResponseDTO<List<AccessRightDTO>> GetModules()
        {
            BaseResponseDTO<List<AccessRightDTO>> dto = new BaseResponseDTO<List<AccessRightDTO>>();
            dto.Data = new List<AccessRightDTO>();
            try
            {
                List<NPF_SYS_Modules> Ml = context.NPF_SYS_Modules.Where(x => x.ParentId != null && x.DeleteStatus == false).ToList();
                List<NPF_SYS_Modules> Mdls = context.NPF_SYS_Modules.Where(x => x.ParentId == null && x.DeleteStatus == false).ToList();
                List<AccessRightDTO> AccessRightDTO = new List<AccessRightDTO>();
                List<Guid> AccessRightsIds = new List<Guid>();
                List<AccessRightDetailDTO> ARDDTOL = new List<AccessRightDetailDTO>();
                foreach (AccessOperationType g in Enum.GetValues(typeof(AccessOperationType)))
                {
                    AccessRightDetailDTO ARDDTO = new AccessRightDetailDTO()
                    {
                        Id = null,
                        OperationType = g,
                        OperationTypeName = g.ToString(),
                        Permission = false
                    };
                    ARDDTOL.Add(ARDDTO);
                }
                #region Level1
                foreach (NPF_SYS_Modules Md in Mdls)
                {
                    if (Ml.Any(x => x.ParentId == Md.Id))
                    {
                        AccessRightsIds.Add(Md.Id);
                    }
                    else
                    {
                        AccessRightDTO Ardt = new AccessRightDTO()
                        {
                            Id = Md.Id,
                            Level1 = Md.Name,
                            AccessRightDetailDTO = ARDDTOL
                        };
                        AccessRightDTO.Add(Ardt);
                    }
                }
                List<AccessRightsDTO> lAR = (from child in context.NPF_SYS_Modules.Where(x => AccessRightsIds.Contains(x.ParentId.Value))
                                             join parent in context.NPF_SYS_Modules on child.ParentId equals parent.Id
                                             select new AccessRightsDTO
                                             {
                                                 AccessRightId = "00000000-0000-0000-0000-000000000000",
                                                 MenuId = parent.Id.ToString(),
                                                 Menu = parent.Name,
                                                 SubMenuId = child.Id.ToString(),
                                                 SubMenu = child.Name,
                                                 order = 200


                                             }).ToList();
                AccessRightsIds = new List<Guid>();
                #endregion
                #region Level2
                foreach (AccessRightsDTO Md in lAR)
                {
                    if (Ml.Any(x => x.ParentId == Guid.Parse(Md.SubMenuId)))
                    {
                        AccessRightsIds.Add(Guid.Parse(Md.MenuId));
                    }
                    else
                    {
                        AccessRightDTO Ardt = new AccessRightDTO()
                        {
                            Id = Guid.Parse(Md.SubMenuId),
                            Level1 = Md.Menu,
                            Level2 = Md.SubMenu,
                            AccessRightDetailDTO = ARDDTOL
                        };
                        AccessRightDTO.Add(Ardt);
                    }
                }
                List<NPF_SYS_Modules> x = context.NPF_SYS_Modules.Where(x => AccessRightsIds.Contains(x.ParentId.Value)).ToList(); // level 2
                List<Guid> xl = x.Select(x => x.Id).ToList(); // level 2 list
                var x2 = context.NPF_SYS_Modules.Where(x => xl.Contains(x.ParentId.Value)).ToList(); // level 3
                lAR = new List<AccessRightsDTO>();
                foreach (var ne in x2)
                {
                    var l = x.Where(z => z.Id == ne.ParentId).FirstOrDefault();
                    AccessRightsDTO n = new AccessRightsDTO()
                    {
                        AccessRightId = "00000000-0000-0000-0000-000000000000",
                        GMenuId = l.ParentId.ToString(),
                        GMenu = context.NPF_SYS_Modules.Where(x => x.Id == l.ParentId).FirstOrDefault().Name,
                        MenuId = l.Id.ToString(),
                        Menu = l.Name,
                        SubMenuId = ne.Id.ToString(),
                        SubMenu = ne.Name,
                        order = 200
                    };
                    lAR.Add(n);
                }

                //lAR = (from child in context.NPF_SYS_Modules.Where(x => AccessRightsIds.Contains(x.ParentId.Value))
                //         join parent in context.NPF_SYS_Modules on child.ParentId equals parent.Id
                //         join Gparent in context.NPF_SYS_Modules on parent.ParentId equals Gparent.Id
                //         select new AccessRightsDTO
                //         {
                //             AccessRightId = "00000000-0000-0000-0000-000000000000",
                //             GMenuId = parent.Id.ToString(),
                //             GMenu = parent.Name,
                //             MenuId = parent.Id.ToString(),
                //             Menu = parent.Name,
                //             SubMenuId = child.Id.ToString(),
                //             SubMenu = child.Name,
                //             order = 200


                //         }).ToList();
                AccessRightsIds = new List<Guid>();
                #endregion
                #region Level3
                foreach (AccessRightsDTO Md in lAR)
                {
                    if (Ml.Any(x => x.ParentId == Guid.Parse(Md.SubMenuId)))
                    {
                        AccessRightsIds.Add(Guid.Parse(Md.MenuId));
                    }
                    else
                    {
                        AccessRightDTO Ardt = new AccessRightDTO()
                        {
                            Id = Guid.Parse(Md.SubMenuId),
                            Level1 = Md.GMenu,
                            Level2 = Md.Menu,
                            Level3 = Md.SubMenu,
                            AccessRightDetailDTO = ARDDTOL
                        };
                        AccessRightDTO.Add(Ardt);
                    }
                }
                #endregion

                dto.Data = AccessRightDTO;
                dto.TotalItems = AccessRightDTO.Count();
                dto.QryResult = new QueryResult().SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.TotalItems = 0;
                dto.AdditionalData = ex.ToString();
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;
        }
        public async Task<BaseResponseDTO<MainARDTO>> GetARById(string Id)
        {
            BaseResponseDTO<MainARDTO> dto = new BaseResponseDTO<MainARDTO>();
            dto.Data = new MainARDTO();
            try
            {
                DAL.Models.ApplicationRole Rolesdt = context.Roles.Where(x => x.Id == Id && x.DeleteStatus == false).FirstOrDefault();
                List<NPF_SYS_Modules> Ml = context.NPF_SYS_Modules.Where(x => x.ParentId != null && x.DeleteStatus == false).ToList();
                List<NPF_SYS_Modules> Mdls = context.NPF_SYS_Modules.Where(x => x.ParentId == null && x.DeleteStatus == false).ToList();
                List<AccessRightDTO> AccessRightDTO = new List<AccessRightDTO>();
                List<Guid> AccessRightsIds = new List<Guid>();
                List<AccessRightDetailDTO> ARDDTOL = new List<AccessRightDetailDTO>();
                foreach (AccessOperationType g in Enum.GetValues(typeof(AccessOperationType)))
                {
                    AccessRightDetailDTO ARDDTO = new AccessRightDetailDTO()
                    {
                        Id = null,
                        OperationType = g,
                        OperationTypeName = g.ToString(),
                        Permission = false
                    };
                    ARDDTOL.Add(ARDDTO);
                }
                #region Level1
                foreach (NPF_SYS_Modules Md in Mdls)
                {
                    if (Ml.Any(x => x.ParentId == Md.Id))
                    {
                        AccessRightsIds.Add(Md.Id);
                    }
                    else
                    {
                        List<AccessRightDetailDTO> ARDDTO = (from a in context.NPF_SYS_AccessRightsDetails
                                                             where a.AccessRights.Role.Id == Id && a.AccessRights.Module.Id == Md.Id
                                                             select new AccessRightDetailDTO
                                                             {
                                                                 Id = a.Id,
                                                                 OperationTypeName = a.AOT.ToString(),
                                                                 OperationType = a.AOT,
                                                                 Permission = a.Permission

                                                             }).ToList();
                        if (ARDDTO.Count != ARDDTOL.Count())
                        {
                            if (ARDDTO.Count <= 0)
                            {
                                ARDDTO = ARDDTOL;
                            }
                            else
                            {
                                List<AccessRightDetailDTO> ARDDTOLN = new List<AccessRightDetailDTO>();
                                ARDDTOLN = ARDDTOL;
                                foreach (AccessRightDetailDTO dt in ARDDTO)
                                {
                                    ARDDTOLN.RemoveAll(x => x.OperationType == dt.OperationType);
                                }
                                ARDDTO.AddRange(ARDDTOLN);
                            }

                        }
                        AccessRightDTO Ardt = new AccessRightDTO()
                        {
                            Id = Md.Id,
                            Level1 = Md.Name,
                            AccessRightDetailDTO = ARDDTO,
                        };
                        AccessRightDTO.Add(Ardt);
                    }
                }
                List<AccessRightsDTO> lAR = (from child in context.NPF_SYS_Modules.Where(x => AccessRightsIds.Contains(x.ParentId.Value))
                                             join parent in context.NPF_SYS_Modules on child.ParentId equals parent.Id
                                             select new AccessRightsDTO
                                             {
                                                 AccessRightId = "00000000-0000-0000-0000-000000000000",
                                                 MenuId = parent.Id.ToString(),
                                                 Menu = parent.Name,
                                                 SubMenuId = child.Id.ToString(),
                                                 SubMenu = child.Name,
                                                 order = 200


                                             }).ToList();
                AccessRightsIds = new List<Guid>();
                #endregion
                #region Level2
                foreach (AccessRightsDTO Md in lAR)
                {
                    if (Ml.Any(x => x.ParentId == Guid.Parse(Md.SubMenuId)))
                    {
                        AccessRightsIds.Add(Guid.Parse(Md.MenuId));
                    }
                    else
                    {
                        List<AccessRightDetailDTO> ARDDTO = (from a in context.NPF_SYS_AccessRightsDetails
                                                             where a.AccessRights.Role.Id == Id && a.AccessRights.Module.Id == Guid.Parse(Md.SubMenuId)
                                                             select new AccessRightDetailDTO
                                                             {
                                                                 Id = a.Id,
                                                                 OperationTypeName = a.AOT.ToString(),
                                                                 OperationType = a.AOT,
                                                                 Permission = a.Permission

                                                             }).ToList();
                        if (ARDDTO.Count != ARDDTOL.Count())
                        {
                            if (ARDDTO.Count <= 0)
                            {
                                ARDDTO = ARDDTOL;
                            }
                            else
                            {
                                List<AccessRightDetailDTO> ARDDTOLN = new List<AccessRightDetailDTO>();
                                ARDDTOLN = ARDDTOL;
                                foreach (AccessRightDetailDTO dt in ARDDTO)
                                {
                                    ARDDTOLN.RemoveAll(x => x.OperationType == dt.OperationType);
                                }
                                ARDDTO.AddRange(ARDDTOLN);
                            }

                        }
                        AccessRightDTO Ardt = new AccessRightDTO()
                        {
                            Id = Guid.Parse(Md.SubMenuId),
                            Level1 = Md.Menu,
                            Level2 = Md.SubMenu,
                            AccessRightDetailDTO = ARDDTO,
                        };
                        AccessRightDTO.Add(Ardt);
                    }
                }
                //lAR = (from child in context.NPF_SYS_Modules.Where(x => AccessRightsIds.Contains(x.ParentId.Value))
                //       join parent in context.NPF_SYS_Modules on child.ParentId equals parent.Id
                //       join Gparent in context.NPF_SYS_Modules on parent.ParentId equals Gparent.Id
                //       select new AccessRightsDTO
                //       {
                //           AccessRightId = "00000000-0000-0000-0000-000000000000",
                //           GMenuId = parent.Id.ToString(),
                //           GMenu = parent.Name,
                //           MenuId = parent.Id.ToString(),
                //           Menu = parent.Name,
                //           SubMenuId = child.Id.ToString(),
                //           SubMenu = child.Name,
                //           order = 200


                //       }).ToList();
                List<NPF_SYS_Modules> x = context.NPF_SYS_Modules.Where(x => AccessRightsIds.Contains(x.ParentId.Value)).ToList(); // level 2
                List<Guid> xl = x.Select(x => x.Id).ToList(); // level 2 list
                var x2 = context.NPF_SYS_Modules.Where(x => xl.Contains(x.ParentId.Value)).ToList(); // level 3
                lAR = new List<AccessRightsDTO>();
                foreach (var ne in x2)
                {
                    var l = x.Where(z => z.Id == ne.ParentId).FirstOrDefault();
                    AccessRightsDTO n = new AccessRightsDTO()
                    {
                        AccessRightId = "00000000-0000-0000-0000-000000000000",
                        GMenuId = l.ParentId.ToString(),
                        GMenu = context.NPF_SYS_Modules.Where(x => x.Id == l.ParentId).FirstOrDefault().Name,
                        MenuId = l.Id.ToString(),
                        Menu = l.Name,
                        SubMenuId = ne.Id.ToString(),
                        SubMenu = ne.Name,
                        order = 200
                    };
                    lAR.Add(n);
                }
                AccessRightsIds = new List<Guid>();
                #endregion
                #region Level3
                foreach (AccessRightsDTO Md in lAR)
                {
                    if (Ml.Any(x => x.ParentId == Guid.Parse(Md.SubMenuId)))
                    {
                        AccessRightsIds.Add(Guid.Parse(Md.MenuId));
                    }
                    else
                    {
                        List<AccessRightDetailDTO> ARDDTO = (from a in context.NPF_SYS_AccessRightsDetails
                                                             where a.AccessRights.Role.Id == Id && a.AccessRights.Module.Id == Guid.Parse(Md.SubMenuId)
                                                             select new AccessRightDetailDTO
                                                             {
                                                                 Id = a.Id,
                                                                 OperationTypeName = a.AOT.ToString(),
                                                                 OperationType = a.AOT,
                                                                 Permission = a.Permission

                                                             }).ToList();
                        if (ARDDTO.Count != ARDDTOL.Count())
                        {
                            if (ARDDTO.Count <= 0)
                            {
                                ARDDTO = ARDDTOL;
                            }
                            else
                            {
                                List<AccessRightDetailDTO> ARDDTOLN = new List<AccessRightDetailDTO>();
                                ARDDTOLN = ARDDTOL;
                                foreach (AccessRightDetailDTO dt in ARDDTO)
                                {
                                    ARDDTOLN.RemoveAll(x => x.OperationType == dt.OperationType);
                                }
                                ARDDTO.AddRange(ARDDTOLN);
                            }

                        }
                        AccessRightDTO Ardt = new AccessRightDTO()
                        {
                            Id = Guid.Parse(Md.SubMenuId),
                            Level1 = Md.GMenu,
                            Level2 = Md.Menu,
                            Level3 = Md.SubMenu,
                            AccessRightDetailDTO = ARDDTO,
                        };
                        AccessRightDTO.Add(Ardt);
                    }
                }
                #endregion

                dto.Data.RoleName = Rolesdt.Name;
                dto.Data.RoleId = Id;
                dto.Data.AccessRightDTO = AccessRightDTO;
                dto.TotalItems = AccessRightDTO.Count();
                dto.QryResult = new QueryResult().SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.TotalItems = 0;
                dto.AdditionalData = ex.ToString();
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;

        }
        public async Task<BaseResponseDTO<bool>> AddInsertAR(BaseRequestDTO<List<AccessRightDTO>> dataToSave, DAL.Models.ApplicationRole Rolesdt)
        {
            BaseResponseDTO<bool> res = new BaseResponseDTO<bool>();
            string message = "An error has occurred. Please contact Administrator.";
            NPF_SYS_AccessRights accessRights= new NPF_SYS_AccessRights();
            try 
            {

                res.Data = true;
                res.AdditionalData = string.Empty;
                res.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                res.Data = false;
                res.AdditionalData = message;
                res.QryResult = new QueryResult().FAILED;
            }

            return res;
        }
        public async Task<BaseResponseDTO<bool>> AddAR(BaseRequestDTO<List<AccessRightDTO>> dataToSave, DAL.Models.ApplicationRole Rolesdt)
        {
            BaseResponseDTO<bool> BaseDtoAccessRights = new BaseResponseDTO<bool>();

            string message = "An error has occurred. Please contact Administrator.";

            try
            {
                List<string> GMenuIds = new List<string>();
                List<string> MenuIds = new List<string>();
                foreach (AccessRightDTO Ardt in dataToSave.Data)
                {
                    NPF_SYS_AccessRights ar = new NPF_SYS_AccessRights();
                    NPF_SYS_AccessRightsDetails ards = new NPF_SYS_AccessRightsDetails();
                    NPF_SYS_Modules m = context.NPF_SYS_Modules.Where(x => x.Id == Ardt.Id).FirstOrDefault();
                    bool HasAccess = Ardt.AccessRightDetailDTO.Any(x => x.Permission == true);
                    if (HasAccess)
                    { 
                        if(!context.NPF_SYS_AccessRights.Any(x => x.Module.Id== m.Id && x.Role.Id == Rolesdt.Id))
                        {
                            if (m.ParentId != null)
                            {
                                if (!MenuIds.Contains(m.ParentId.ToString()))
                                {
                                    NPF_SYS_Modules Pm = context.NPF_SYS_Modules.Where(x => x.Id == m.ParentId).FirstOrDefault();
                                    MenuIds.Add(Pm.Id.ToString());

                                    ar = new NPF_SYS_AccessRights();
                                    ar.Module = Pm;
                                    ar.hasRight = true;
                                    ar.Role = Rolesdt;
                                    ar.CreatedDate = DateTime.Now;
                                    ar.CreatedBy = Guid.Parse(dataToSave.userToken);
                                    context.Add(ar);
                                    context.SaveChanges();
                                    foreach (var item in Ardt.AccessRightDetailDTO)
                                    {
                                        ards = new NPF_SYS_AccessRightsDetails()
                                        {
                                            Permission = item.Permission,
                                            AOT = item.OperationType,
                                            AccessRights = ar
                                        };
                                        context.NPF_SYS_AccessRightsDetails.Add(ards);
                                        context.SaveChanges();
                                    }
                                    if (Pm.ParentId != null)
                                    {
                                        if (!GMenuIds.Contains(Pm.ParentId.ToString()))
                                        {
                                            Pm = context.NPF_SYS_Modules.Where(x => x.Id == ar.Module.ParentId).FirstOrDefault();
                                            GMenuIds.Add(Pm.Id.ToString());

                                            ar = new NPF_SYS_AccessRights();
                                            ar.Module = Pm;
                                            ar.hasRight = true;
                                            ar.Role = Rolesdt;
                                            ar.CreatedDate = DateTime.Now;
                                            ar.CreatedBy = Guid.Parse(dataToSave.userToken);
                                            context.Add(ar);
                                            context.SaveChanges();
                                            foreach (var item in Ardt.AccessRightDetailDTO)
                                            {
                                                ards = new NPF_SYS_AccessRightsDetails()
                                                {
                                                    Permission = item.Permission,
                                                    AOT = item.OperationType,
                                                    AccessRights = ar
                                                };
                                                context.NPF_SYS_AccessRightsDetails.Add(ards);
                                                context.SaveChanges();
                                            }
                                        }
                                    }

                                }

                            }
                            ar = new NPF_SYS_AccessRights();
                            ar.Module = m;
                            ar.Role = Rolesdt;
                            ar.hasRight = HasAccess;
                            ar.CreatedDate = DateTime.Now;
                            ar.CreatedBy = Guid.Parse(dataToSave.userToken);
                            context.Add(ar);
                            context.SaveChanges();
                            foreach (var item in Ardt.AccessRightDetailDTO)
                            {
                                ards = new NPF_SYS_AccessRightsDetails()
                                {
                                    Permission = item.Permission,
                                    AOT = item.OperationType,
                                    AccessRights = ar
                                };
                                context.NPF_SYS_AccessRightsDetails.Add(ards);
                                context.SaveChanges();
                            }
                        }

                    }


                }
                BaseDtoAccessRights.Data = true;
                BaseDtoAccessRights.AdditionalData = string.Empty;
                BaseDtoAccessRights.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDtoAccessRights.Data = false;
                BaseDtoAccessRights.AdditionalData = message;
                BaseDtoAccessRights.QryResult = new QueryResult().FAILED;
            }

            return BaseDtoAccessRights;
        }
        public async Task<BaseResponseDTO<bool>> SaveUpdateAR(BaseRequestDTO<MainARDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDtoAccessRights = new BaseResponseDTO<bool>();
            BaseRequestDTO<List<AccessRightDTO>> AddRequest = new BaseRequestDTO<List<AccessRightDTO>>();
            AddRequest.Data = new List<AccessRightDTO>();


            string message = "An error has occurred. Please contact Administrator.";

            try
            {


                DAL.Models.ApplicationRole Rolesdt = context.Roles.Where(x => x.Name == dataToSave.Data.RoleName && x.DeleteStatus == false).FirstOrDefault();
                if (dataToSave.Data.RoleId == null) //add
                {

                    if (Rolesdt == null)
                    {

                        Rolesdt = new DAL.Models.ApplicationRole();
                        Rolesdt.NormalizedName = dataToSave.Data.RoleName.ToUpper();
                        Rolesdt.Name = dataToSave.Data.RoleName;
                        context.Roles.Add(Rolesdt);
                        context.SaveChanges();
                        AddRequest.Data = dataToSave.Data.AccessRightDTO;
                        AddRequest.userToken = dataToSave.userToken;
                        BaseDtoAccessRights = await AddAR(AddRequest, Rolesdt);
                        if (BaseDtoAccessRights.Data == false)
                        {
                            BaseDtoAccessRights.Data = false;
                            BaseDtoAccessRights.QryResult = new QueryResult().FAILED;
                            BaseDtoAccessRights.AdditionalData = "An error has occurred. Please contact Administrator.";
                            return BaseDtoAccessRights;
                        }
                    }
                    else
                    {
                        BaseDtoAccessRights.Data = false;
                        BaseDtoAccessRights.QryResult = new QueryResult().FAILED;
                        BaseDtoAccessRights.AdditionalData = "Role " + dataToSave.Data.RoleName + " already exists on system.";
                        return BaseDtoAccessRights;
                    }
                    message = "Save Succesfully";


                }
                else //edit
                {
                    Rolesdt = context.Roles.Where(x => x.Name == dataToSave.Data.RoleName && x.Id != dataToSave.Data.RoleId && x.DeleteStatus == false).FirstOrDefault();

                    if (Rolesdt != null)
                    {
                        BaseDtoAccessRights.Data = false;
                        BaseDtoAccessRights.QryResult = new QueryResult().SUCEEDED;
                        BaseDtoAccessRights.AdditionalData = "Role " + dataToSave.Data.RoleName + " already exists on system.";
                        return BaseDtoAccessRights;
                    }
                    else
                    {
                        //update role 
                        Rolesdt = context.Roles.Where(x => x.Id == dataToSave.Data.RoleId).FirstOrDefault();
                        if (Rolesdt.Name != dataToSave.Data.RoleName)
                        {
                            Rolesdt.Name = dataToSave.Data.RoleName;
                            Rolesdt.NormalizedName = dataToSave.Data.RoleName.ToUpper();
                            context.Update(Rolesdt);
                            context.SaveChanges();
                        }
                        List<NPF_SYS_AccessRightsDetails> ardet = context.NPF_SYS_AccessRightsDetails.Where(x => x.AccessRights.Role.Id == dataToSave.Data.RoleId).ToList();
                        List<NPF_SYS_AccessRights> arhead = context.NPF_SYS_AccessRights.Where(x => x.Role.Id == dataToSave.Data.RoleId).ToList();
                        //         foreach (NPF_SYS_AccessRightsDetails ardy in ardet)
                        //         {
                        //             context.NPF_SYS_AccessRightsDetails.Remove(ardy);
                        //             context.SaveChanges();
                        //         }
                        //List<Guid> arhead = context.NPF_SYS_AccessRights.Where(x => x.Role.Id == dataToSave.Data.RoleId).Select(x => x.id).ToList();
                        //foreach (Guid arheady in arhead)
                        //{
                        //    context.NPF_SYS_AccessRights.Remove(context.NPF_SYS_AccessRights.Where(x => x.id == arheady).FirstOrDefault());
                        //    context.SaveChanges();
                        //}
                        context.NPF_SYS_AccessRightsDetails.RemoveRange(ardet);
                        context.SaveChanges();
                        context.NPF_SYS_AccessRights.RemoveRange(arhead);
                        context.SaveChanges();
                        AddRequest.Data = dataToSave.Data.AccessRightDTO;
                        AddRequest.userToken = dataToSave.userToken;
                        BaseDtoAccessRights = await AddAR(AddRequest, Rolesdt);
                        if (BaseDtoAccessRights.Data == false)
                        {
                            BaseDtoAccessRights.Data = false;
                            BaseDtoAccessRights.QryResult = new QueryResult().FAILED;
                            BaseDtoAccessRights.AdditionalData = "An error has occurred. Please contact Administrator.";
                            return BaseDtoAccessRights;
                        }
                        message = "Update Succesfully";
                    }





                }

                BaseDtoAccessRights.Data = true;
                BaseDtoAccessRights.AdditionalData = message;
                BaseDtoAccessRights.QryResult = new QueryResult().SUCEEDED;



            }
            catch (Exception ex)
            {
                BaseDtoAccessRights.Data = false;
                BaseDtoAccessRights.AdditionalData = message;
                BaseDtoAccessRights.QryResult = new QueryResult().FAILED;
            }

            return BaseDtoAccessRights;
        }
        public async Task<BaseResponseDTO<bool>> SaveUpdateAROld(BaseRequestDTO<MainARDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDtoAccessRights = new BaseResponseDTO<bool>();


            string message = "An error has occurred. Please contact Administrator.";

            try
            {


                DAL.Models.ApplicationRole Rolesdt = context.Roles.Where(x => x.Name == dataToSave.Data.RoleName && x.DeleteStatus == false).FirstOrDefault();
                if (dataToSave.Data.RoleId == null) //add
                {

                    if (Rolesdt == null)
                    {

                        Rolesdt = new DAL.Models.ApplicationRole();
                        Rolesdt.NormalizedName = dataToSave.Data.RoleName.ToUpper();
                        Rolesdt.Name = dataToSave.Data.RoleName;
                        context.Roles.Add(Rolesdt);
                        context.SaveChanges();

                    }
                    else
                    {
                        BaseDtoAccessRights.Data = false;
                        BaseDtoAccessRights.QryResult = new QueryResult().SUCEEDED;
                        BaseDtoAccessRights.AdditionalData = "Role " + dataToSave.Data.RoleName + " already exists on system.";
                        return BaseDtoAccessRights;
                    }
                    List<string> GMenuIds = new List<string>();
                    List<string> MenuIds = new List<string>();
                    foreach (AccessRightDTO Ardt in dataToSave.Data.AccessRightDTO)
                    {
                        NPF_SYS_AccessRights ar = new NPF_SYS_AccessRights();
                        NPF_SYS_AccessRightsDetails ards = new NPF_SYS_AccessRightsDetails();
                        NPF_SYS_Modules m = context.NPF_SYS_Modules.Where(x => x.Id == Ardt.Id).FirstOrDefault();
                        bool HasAccess = Ardt.AccessRightDetailDTO.Any(x => x.Permission == true);

                        if (m.ParentId != null)
                        {
                            if (!MenuIds.Contains(m.ParentId.ToString()))
                            {
                                NPF_SYS_Modules Pm = context.NPF_SYS_Modules.Where(x => x.Id == m.ParentId).FirstOrDefault();
                                MenuIds.Add(Pm.Id.ToString());

                                ar = new NPF_SYS_AccessRights();
                                ar.Module = Pm;
                                ar.Role = Rolesdt;
                                ar.CreatedDate = DateTime.Now;
                                ar.CreatedBy = Guid.Parse(dataToSave.userToken);
                                context.Add(ar);
                                context.SaveChanges();
                                foreach (var item in Ardt.AccessRightDetailDTO)
                                {
                                    ards = new NPF_SYS_AccessRightsDetails()
                                    {
                                        Permission = item.Permission,
                                        AOT = item.OperationType,
                                        AccessRights = ar
                                    };
                                    context.NPF_SYS_AccessRightsDetails.Add(ards);
                                    context.SaveChanges();
                                }

                                if (!GMenuIds.Contains(Pm.ParentId.ToString()) && Pm.ParentId != null)
                                {
                                    Pm = context.NPF_SYS_Modules.Where(x => x.Id == ar.Module.ParentId).FirstOrDefault();
                                    GMenuIds.Add(Pm.Id.ToString());

                                    ar = new NPF_SYS_AccessRights();
                                    ar.Module = Pm;
                                    ar.Role = Rolesdt;
                                    ar.CreatedDate = DateTime.Now;
                                    ar.CreatedBy = Guid.Parse(dataToSave.userToken);
                                    context.Add(ar);
                                    context.SaveChanges();
                                    foreach (var item in Ardt.AccessRightDetailDTO)
                                    {
                                        ards = new NPF_SYS_AccessRightsDetails()
                                        {
                                            Permission = item.Permission,
                                            AOT = item.OperationType,
                                            AccessRights = ar
                                        };
                                        context.NPF_SYS_AccessRightsDetails.Add(ards);
                                        context.SaveChanges();
                                    }
                                }
                            }

                        }
                        ar = new NPF_SYS_AccessRights();
                        ar.Module = m;
                        ar.Role = Rolesdt;
                        ar.hasRight = HasAccess;
                        ar.CreatedDate = DateTime.Now;
                        ar.CreatedBy = Guid.Parse(dataToSave.userToken);
                        context.Add(ar);
                        context.SaveChanges();
                        foreach (var item in Ardt.AccessRightDetailDTO)
                        {
                            ards = new NPF_SYS_AccessRightsDetails()
                            {
                                Permission = item.Permission,
                                AOT = item.OperationType,
                                AccessRights = ar
                            };
                            context.NPF_SYS_AccessRightsDetails.Add(ards);
                            context.SaveChanges();
                        }

                    }


                    message = "Save Succesfully";

                }
                else //edit
                {
                    Rolesdt = context.Roles.Where(x => x.Name == dataToSave.Data.RoleName && x.Id != dataToSave.Data.RoleId && x.DeleteStatus == false).FirstOrDefault();

                    if (Rolesdt != null)
                    {
                        BaseDtoAccessRights.Data = false;
                        BaseDtoAccessRights.QryResult = new QueryResult().SUCEEDED;
                        BaseDtoAccessRights.AdditionalData = "Role " + dataToSave.Data.RoleName + " already exists on system.";
                        return BaseDtoAccessRights;
                    }
                    else
                    {
                        //update role 
                        Rolesdt = context.Roles.Where(x => x.Id == dataToSave.Data.RoleId).FirstOrDefault();
                        if (Rolesdt.Name != dataToSave.Data.RoleName)
                        {
                            Rolesdt.Name = dataToSave.Data.RoleName;
                            Rolesdt.NormalizedName = dataToSave.Data.RoleName.ToUpper();
                            context.Update(Rolesdt);
                            context.SaveChanges();
                        }
                        int coount = 0;

                        List<string> GMenuIds = new List<string>();
                        List<string> MenuIds = new List<string>();
                        foreach (AccessRightDTO Ardt in dataToSave.Data.AccessRightDTO)
                        {
                            coount++;
                            NPF_SYS_AccessRights ar = new NPF_SYS_AccessRights();
                            NPF_SYS_AccessRightsDetails ards = new NPF_SYS_AccessRightsDetails();
                            NPF_SYS_Modules m = context.NPF_SYS_Modules.Where(x => x.Id == Ardt.Id).FirstOrDefault();
                            bool HasAccess = Ardt.AccessRightDetailDTO.Any(x => x.Permission == true);

                            if (m.ParentId != null)
                            {
                                if (!MenuIds.Contains(m.ParentId.ToString()))
                                {
                                    NPF_SYS_Modules Pm = context.NPF_SYS_Modules.Where(x => x.Id == m.ParentId).FirstOrDefault();
                                    MenuIds.Add(Pm.Id.ToString());

                                    foreach (var item in Ardt.AccessRightDetailDTO)
                                    {
                                        ar = context.NPF_SYS_AccessRights.Where(x => x.Role.Id == Rolesdt.Id && x.Module.Id == Pm.Id).FirstOrDefault();
                                        ards = context.NPF_SYS_AccessRightsDetails.Where(x => x.AccessRights.id == ar.id).FirstOrDefault();
                                        ards.Permission = item.Permission;
                                        ards.AOT = item.OperationType;
                                        ards.AccessRights = ar;
                                        context.NPF_SYS_AccessRightsDetails.Update(ards);
                                        context.SaveChanges();
                                    }
                                    if (Pm.ParentId != null)
                                    {
                                        Guid? Pid = Pm.ParentId;
                                        if (!GMenuIds.Contains(Pm.ParentId.ToString()))
                                        {
                                            Pm = context.NPF_SYS_Modules.Where(x => x.Id == Pid).FirstOrDefault();
                                            GMenuIds.Add(Pm.Id.ToString());

                                            foreach (var item in Ardt.AccessRightDetailDTO)
                                            {
                                                ar = context.NPF_SYS_AccessRights.Where(x => x.Role.Id == Rolesdt.Id && x.Module.Id == Pm.Id).FirstOrDefault();
                                                ards = context.NPF_SYS_AccessRightsDetails.Where(x => x.AccessRights.id == ar.id).FirstOrDefault();
                                                ards.Permission = item.Permission;
                                                ards.AOT = item.OperationType;
                                                ards.AccessRights = ar;
                                                context.NPF_SYS_AccessRightsDetails.Update(ards);
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }

                            }
                            foreach (var item in Ardt.AccessRightDetailDTO)
                            {
                                ar = context.NPF_SYS_AccessRights.Where(x => x.Role.Id == Rolesdt.Id && x.Module.Id == Ardt.Id).FirstOrDefault();
                                if (ar != null)
                                {
                                    ar.hasRight = HasAccess;
                                    context.NPF_SYS_AccessRights.Update(ar);
                                    context.SaveChanges();
                                    ards = context.NPF_SYS_AccessRightsDetails.Where(x => x.Id == item.Id).FirstOrDefault();
                                    if (ards != null)
                                    {
                                        ards.Permission = item.Permission;
                                        ards.AOT = item.OperationType;
                                        ards.AccessRights = ar;
                                        context.NPF_SYS_AccessRightsDetails.Update(ards);
                                        context.SaveChanges();
                                    }

                                }

                            }
                        }

                        message = "Update Succesfully";
                    }





                }

                BaseDtoAccessRights.Data = true;
                BaseDtoAccessRights.AdditionalData = message;
                BaseDtoAccessRights.QryResult = new QueryResult().SUCEEDED;



            }
            catch (Exception ex)
            {
                BaseDtoAccessRights.Data = false;
                BaseDtoAccessRights.AdditionalData = message;
                BaseDtoAccessRights.QryResult = new QueryResult().FAILED;
            }

            return BaseDtoAccessRights;
        }
        public BaseResponseDTO<bool> Delete(BaseRequestDTO<string> dataToDelId)
        {
            BaseResponseDTO<bool> dto = new BaseResponseDTO<bool>();
            try
            {
                if (context.UserRoles.Any(x => x.RoleId == dataToDelId.Data))
                {
                    dto.Data = false;
                    dto.AdditionalData = "Role is currently assigned. Cannot be deleted";
                    dto.QryResult = new QueryResult().FAILED;
                    return dto;
                }
                DAL.Models.ApplicationRole AR = context.Roles.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                AR.DeleteStatus = true;
                AR.Name = AR.Name + "_Deleted" + DateTime.Now.ToString();
                AR.NormalizedName = AR.NormalizedName + "_DELETED" + DateTime.Now.ToString();
                AR.UpdatedDate = DateTime.Now;
                AR.UpdatedBy = Guid.Parse(dataToDelId.userToken);
                context.Update(AR);
                context.SaveChanges();

                dto.Data = true;
                dto.QryResult = new QueryResult().SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = false;
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;
        }

        public async Task<BaseResponseDTO<List<MainMenuDTO>>> getAccessRightByUserRoleId(string RoleId, string userId)
        {
            BaseResponseDTO<List<MainMenuDTO>> dto = new BaseResponseDTO<List<MainMenuDTO>>();

            try
            {
            //    DateTime time = DateTime.Now;
                List<MainMenuDTO> GARDL = (from a in context.NPF_SYS_AccessRights
                                               // join b in context.NPF_SYS_AccessRightsDetails on a.id equals b.AccessRights.id
                                           where a.Role.Id == RoleId && a.Module.ParentId == null && a.DeleteStatus == false && a.hasRight == true
                                           select new MainMenuDTO
                                           {
                                               Id = a.Module.Id,
                                               Name = a.Module.DisplayName,
                                               Icon = a.Module.Icon,
                                               Order = a.Module.Order,
                                               ParentId = a.Module.ParentId,
                                               Url = a.Module.Url,
                                               SubMenu = (from c in context.NPF_SYS_AccessRights
                                                              //  join d in context.NPF_SYS_AccessRightsDetails on c.id equals d.AccessRights.id
                                                          where c.Role.Id == RoleId && c.Module.ParentId == a.Module.Id && c.DeleteStatus == false && c.hasRight == true
                                                          select new SubMenuDTO
                                                          {
                                                              Id = c.Module.Id,
                                                              Name = c.Module.DisplayName,
                                                              Icon = c.Module.Icon,
                                                              Order = c.Module.Order,
                                                              ParentId = c.Module.ParentId,
                                                              Url = c.Module.Url,
                                                              Child = (from e in context.NPF_SYS_AccessRights
                                                                           //   join f in context.NPF_SYS_AccessRightsDetails on e.id equals f.AccessRights.id
                                                                       where e.Role.Id == RoleId && e.Module.ParentId == c.Module.Id && e.DeleteStatus == false  && e.hasRight == true
                                                                       select new ChildDTO
                                                                       {
                                                                           Id = e.Module.Id,
                                                                           Name = e.Module.DisplayName,
                                                                           Icon = e.Module.Icon,
                                                                           Order = e.Module.Order,
                                                                           ParentId = e.Module.ParentId,
                                                                           Url = e.Module.Url
                                                                       })/*.GroupBy(p1 => p1.Id).Select(g1 => g1.First())*/.OrderBy(x => x.Order).ToList().ToList()
                                                          })/*.GroupBy(p2 => p2.Id).Select(g2 => g2.First())*/.OrderBy(x => x.Order).ToList().ToList()
                                           }).GroupBy(p3 => p3.Id).Select(g3 => g3.First()).ToList();
                //Console.WriteLine("New Query =>" + (DateTime.Now - time));
                //time = DateTime.Now;
                //List<MainMenuDTO> GARDL = (from a in context.NPF_SYS_AccessRights
                //                           join b in context.NPF_SYS_AccessRightsDetails on a.id equals b.AccessRights.id
                //                           where a.Role.Id == RoleId && a.Module.ParentId == null  && a.DeleteStatus == false && b.Permission == true
                //                           select new MainMenuDTO
                //                           {
                //                               Id = a.Module.Id,
                //                               Name = a.Module.DisplayName,
                //                               Icon = a.Module.Icon,
                //                               Order = a.Module.Order,
                //                               ParentId = a.Module.ParentId,
                //                               Url = a.Module.Url,
                //                               SubMenu = (from c in context.NPF_SYS_AccessRights
                //                                          join d in context.NPF_SYS_AccessRightsDetails on c.id equals d.AccessRights.id
                //                                          where c.Role.Id == RoleId && c.Module.ParentId == a.Module.Id && c.DeleteStatus == false && d.Permission == true
                //                                          select new SubMenuDTO
                //                                          {
                //                                              Id = c.Module.Id,
                //                                              Name = c.Module.DisplayName,
                //                                              Icon = c.Module.Icon,
                //                                              Order = c.Module.Order,
                //                                              ParentId = c.Module.ParentId,
                //                                              Url = c.Module.Url,
                //                                              Child = (from e in context.NPF_SYS_AccessRights
                //                                                       join f in context.NPF_SYS_AccessRightsDetails on e.id equals f.AccessRights.id
                //                                                       where e.Role.Id == RoleId && e.Module.ParentId == c.Module.Id && c.DeleteStatus == false && f.Permission == true 
                //                                                       select new ChildDTO
                //                                                       {
                //                                                           Id = e.Module.Id,
                //                                                           Name = e.Module.DisplayName,
                //                                                           Icon = e.Module.Icon,
                //                                                           Order = e.Module.Order,
                //                                                           ParentId = e.Module.ParentId,
                //                                                           Url = e.Module.Url
                //                                                       }).GroupBy(p1 => p1.Id).Select(g1 => g1.First()).ToList()
                //                                          }).GroupBy(p2 => p2.Id).Select(g2 => g2.First()).ToList()
                //                           }).GroupBy(p3 => p3.Id).Select(g3 => g3.First()).ToList();
                //Console.WriteLine("OLD Query =>"+(DateTime.Now - time));



                dto.Data = GARDL.OrderBy(x => x.Order).ToList();
                dto.TotalItems = GARDL.Count();
                dto.QryResult = new QueryResult().SUCEEDED;

            }
            catch (Exception ex)
            {
                //  dto.Data = new List<AccessRightsDTO>();
                dto.TotalItems = 0;
                dto.AdditionalData = ex.ToString();
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;
        }

        #endregion
        public async Task<BaseResponseDTO<string>> GetViewTitle(string PageName)
        {

            BaseResponseDTO<string> dtofm = new BaseResponseDTO<string>();
            try
            {
                dtofm.Data = context.NPF_SYS_Modules.Where(d => d.Name == PageName && d.DeleteStatus == false).FirstOrDefault().DisplayName;
                dtofm.TotalItems = 0;
                dtofm.QryResult = new QueryResult().SUCEEDED;


            }
            catch (Exception ex)
            {
                dtofm.Data = "";
                dtofm.TotalItems = 0;
                dtofm.QryResult = new QueryResult().FAILED;
            }


            return dtofm;
        }
        public async Task<BaseResponseDTO<string>> GetViewTitle(string PageName, string ParentPageName, string GParentPageName)
        {

            BaseResponseDTO<string> dtofm = new BaseResponseDTO<string>();
            try
            {
                Guid GPid = context.NPF_SYS_Modules.Where(d => d.Name == GParentPageName && d.DeleteStatus == false).FirstOrDefault().Id;
                Guid Pid = context.NPF_SYS_Modules.Where(d => d.Name == ParentPageName && d.ParentId == GPid && d.DeleteStatus == false).FirstOrDefault().Id;
                dtofm.Data = context.NPF_SYS_Modules.Where(d => d.Name == PageName && d.ParentId == Pid && d.DeleteStatus == false).FirstOrDefault().DisplayName;
                dtofm.TotalItems = 0;
                dtofm.QryResult = new QueryResult().SUCEEDED;


            }
            catch (Exception ex)
            {
                dtofm.Data = "";
                dtofm.TotalItems = 0;
                dtofm.QryResult = new QueryResult().FAILED;
            }


            return dtofm;
        }
        public async Task<BaseResponseDTO<bool>> createUpdateMenu(BaseRequestDTO<MenuDTO> dataToSave)
        {
            BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();


            string message = "An error has occurred. Please contact Administrator.";

            try
            {
                NPF_SYS_Modules mod = context.NPF_SYS_Modules.Where(x => x.Id == dataToSave.Data.Id).FirstOrDefault();
                mod.DisplayName = dataToSave.Data.DisplayName;
                context.NPF_SYS_Modules.Update(mod);
                context.SaveChanges();
                dt.Data = true;
            }
            catch (Exception ex)
            {
                dt.Data = false;
                dt.AdditionalData = message;
                dt.QryResult = new QueryResult().FAILED;
            }

            return dt;
        }
        public BaseResponseDTO<MenuDTO> GetMenuById(string Id)
        {
            BaseResponseDTO<MenuDTO> dto = new BaseResponseDTO<MenuDTO>();

            try
            {
                dto.Data = (from r in context.NPF_SYS_Modules
                            where r.Id == Guid.Parse(Id)
                            select new MenuDTO
                            {
                                Id = r.Id,
                                Name = r.Name,
                                DisplayName = r.DisplayName,
                            }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                dto.Data = new MenuDTO();
                dto.TotalItems = 0;
                dto.AdditionalData = ex.ToString();
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;
        }
        public BaseResponseDTO<List<MenuDTO>> GetMenuList()
        {
            BaseResponseDTO<List<MenuDTO>> dto = new BaseResponseDTO<List<MenuDTO>>();
            MenuDTO user = new MenuDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";
            Guid gid = context.NPF_SYS_Modules.Where(x => x.Name == "Access").Select(x => x.Id).FirstOrDefault();
            try
            {

                List<MenuDTO> result = (from r in context.NPF_SYS_Modules
                                        where r.DeleteStatus == false && r.Id != gid && r.ParentId != gid
                                        select new MenuDTO
                                        {
                                            Id = r.Id,
                                            Name = r.Name,
                                            DisplayName = r.DisplayName,
                                        }).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<MenuDTO>();
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

    }
}
