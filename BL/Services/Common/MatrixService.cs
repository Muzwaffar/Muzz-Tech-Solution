using BL.Constants;
using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class MatrixService
    {
        public UserMatrixDTO sGetMatrixUserByMail(string email)
        {
            UserMatrixDTO dt = new UserMatrixDTO();
            NPFContext context = new NPFContext();
            try
            {
                string AID = context.Users.Where(x => x.Email.ToLower() == email).Select(x => x.Id).FirstOrDefault();
                long UsrId = context.NPF_SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;

                List<MatrixListDTO> MLL = (from a in context.NPF_SYS_GroupMatrixUser
                                           join b in context.NPF_SYS_GroupMatrix on a.GMID equals b.GMID
                                           where a.IID == UsrId && a.DeleteStatus == false && b.DeleteStatus == false
                                           select new MatrixListDTO()
                                           {
                                               GMID = a.GMID,
                                               GMDescription = b.GMDescription
                                           }).ToList();
                List<MatrixListDTO> ML = new List<MatrixListDTO>();
                ML.AddRange(MLL);
                foreach (MatrixListDTO m in MLL)
                {
                    ML.AddRange(GetMatrixChild(m.GMID));
                }
                dt.ParentIDS = MLL.Select(y => y.GMID).ToList();
                dt.UID = UsrId;
                dt.AUID = AID;
                dt.IDS = ML.Select(y => y.GMID).ToList();

            }
            catch (Exception ex)
            {

            }
            return dt;
        }
        public UserMatrixDTO sGetMatrixUserByUserID()
        {
            UserMatrixDTO dt = new UserMatrixDTO();
            NPFContext context = new NPFContext();
            try
            {
                IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
                string claimsPrincipal = null;
                if (httpContextAccessor.HttpContext != null)
                {
                    claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
                    if (claimsPrincipal == null)
                    {
                        claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@naveo.mu").Select(x => x.Id).FirstOrDefault();
                    }
                }
                else
                {
                    claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@naveo.mu").Select(x => x.Id).FirstOrDefault();
                }
                string AID = context.Users.Where(x => x.Email.ToLower() == claimsPrincipal.ToLower()).Select(x => x.Id).FirstOrDefault();
                long UsrId = context.NPF_SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;
                //ml = (from a in context.NPF_SYS_GroupMatrixUser
                //      join b in context.NPF_SYS_GroupMatrix on a.GMID equals b.GMID
                //      where a.IID == UsrId select a.GMID).ToList();
                List<MatrixListDTO> MLL = (from a in context.NPF_SYS_GroupMatrixUser
                                           join b in context.NPF_SYS_GroupMatrix on a.GMID equals b.GMID
                                           where a.IID == UsrId && a.DeleteStatus == false && b.DeleteStatus == false
                                           select new MatrixListDTO()
                                           {
                                               GMID = a.GMID,
                                               GMDescription = b.GMDescription
                                           }).ToList();
                List<MatrixListDTO> ML = new List<MatrixListDTO>();
                ML.AddRange(MLL);
                foreach (MatrixListDTO m in MLL)
                {
                    ML.AddRange(GetMatrixChild(m.GMID));
                }
                dt.ParentIDS = MLL.Select(y => y.GMID).ToList();
                dt.UID = UsrId;
                dt.AUID = AID;
                dt.IDS = ML.Select(y => y.GMID).ToList();

            }
            catch (Exception ex)
            {

            }
            return dt;
        }
        public List<long> sGetParentIds(long Uid)
        {
            NPFContext context = new NPFContext();
            List<long> MLL = (from a in context.NPF_SYS_GroupMatrixUser
                              join b in context.NPF_SYS_GroupMatrix on a.GMID equals b.GMID
                              where a.IID == Uid && a.DeleteStatus == false && b.DeleteStatus == false
                              select a.GMID).ToList();
            return MLL;
        }
        public async Task<DropDownItem> sGetNextParentIds(long Id)
        {
            NPFContext context = new NPFContext();
            DropDownItem GM = new DropDownItem();
            long? Pid = context.NPF_SYS_GroupMatrix.Where(x => x.GMID == Id).FirstOrDefault().ParentGMID;
            if (Pid != null)
            {
                 GM = (from a in context.NPF_SYS_GroupMatrix
                                   where a.GMID == Pid && a.DeleteStatus == false && a.DeleteStatus == false
                                   select new DropDownItem()
                                   {
                                       Id = a.GMID,
                                       text = a.GMDescription
                                   }).FirstOrDefault();

            }

            return GM;
        }
        public UserMatrixDTO sGetMatrixUserByUserIDParam(string claimsPrincipal)
        {
            UserMatrixDTO dt = new UserMatrixDTO();
            NPFContext context = new NPFContext();
            try
            {

                string AID = context.Users.Where(x => x.Id.ToLower() == claimsPrincipal).Select(x => x.Id).FirstOrDefault();
                long UsrId = context.NPF_SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;
                List<MatrixListDTO> MLL = (from a in context.NPF_SYS_GroupMatrixUser
                                           join b in context.NPF_SYS_GroupMatrix on a.GMID equals b.GMID
                                           where a.IID == UsrId && a.DeleteStatus == false && b.DeleteStatus == false
                                           select new MatrixListDTO()
                                           {
                                               GMID = a.GMID,
                                               GMDescription = b.GMDescription
                                           }).ToList();
                List<MatrixListDTO> ML = new List<MatrixListDTO>();
                ML.AddRange(MLL);
                foreach (MatrixListDTO m in MLL)
                {
                    ML.AddRange(GetMatrixChild(m.GMID));
                }
                dt.ParentIDS = MLL.Select(y => y.GMID).ToList();
                dt.UID = UsrId;
                dt.AUID = AID;
                dt.IDS = ML.Select(y => y.GMID).ToList();

            }
            catch (Exception ex)
            {

            }
            return dt;
        }
        public async Task<List<MatrixDTO>> GetMatrixName()
        {
            List<MatrixDTO> ml = new List<MatrixDTO>();
            NPFContext context = new NPFContext();
            try
            {
                IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
                string claimsPrincipal = null;
                if (httpContextAccessor.HttpContext != null)
                {
                    claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
                }
                else
                {
                    claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@naveo.mu").Select(x => x.Email).FirstOrDefault();
                }
                string AID = context.Users.Where(x => x.Email.ToLower() == claimsPrincipal).Select(x => x.Id).FirstOrDefault();
                long UsrId = context.NPF_SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;
                ml = (from a in context.NPF_SYS_GroupMatrixUser
                      join b in context.NPF_SYS_GroupMatrix on a.GMID equals b.GMID
                      where a.IID == UsrId && a.DeleteStatus == false && b.DeleteStatus == false
                      select new MatrixDTO()
                      {
                          MID = a.MID,
                          IID = a.IID,
                          GMID = a.GMID,
                          GMDescription = b.GMDescription
                      }).ToList();

            }
            catch (Exception ex)
            {

            }
            return ml;
        }
        public async Task<List<MatrixListDTO>> GetAllMatrix()
        {
            List<MatrixListDTO> ml = new List<MatrixListDTO>();
            List<MatrixListDTO> MLL = new List<MatrixListDTO>();
            NPFContext context = new NPFContext();
            try
            {
                IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
                string claimsPrincipal = null;
                if (httpContextAccessor.HttpContext != null)
                {
                    claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
                }
                else
                {
                    claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@naveo.mu").Select(x => x.Email).FirstOrDefault();
                }
                string AID = context.Users.Where(x => x.Email.ToLower() == claimsPrincipal).Select(x => x.Id).FirstOrDefault();
                long UsrId = context.NPF_SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;
                ml = (from a in context.NPF_SYS_GroupMatrixUser
                      join b in context.NPF_SYS_GroupMatrix on a.GMID equals b.GMID
                      where a.IID == UsrId && a.DeleteStatus == false && b.DeleteStatus == false
                      select new MatrixListDTO()
                      {
                          GMID = a.GMID,
                          GMDescription = b.GMDescription
                      }).ToList();
                MLL.AddRange(ml);
                foreach (MatrixListDTO m in ml)
                {
                    MLL.AddRange(GetMatrixChild(m.GMID));
                }

            }
            catch (Exception ex)
            {

            }
            return MLL;
        }
        public List<MatrixListDTO> GetMatrixChild(long GMID)
        {
            List<MatrixListDTO> ml = new List<MatrixListDTO>();
            List<MatrixListDTO> MLL = new List<MatrixListDTO>();
            NPFContext context = new NPFContext();
            try
            {
                ml = (from a in context.NPF_SYS_GroupMatrix
                      where a.ParentGMID == GMID && a.DeleteStatus == false
                      select new MatrixListDTO()
                      {
                          GMID = a.GMID,
                          GMDescription = a.GMDescription
                      }).ToList();
                if (ml.Count > 0)
                {
                    MLL.AddRange(ml);
                    foreach (MatrixListDTO m in ml)
                    {
                        MLL.AddRange(GetMatrixChild(m.GMID));
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return MLL;
        }

        public async Task<BaseResponseDTO<List<CRUDMatrix>>> GetTree()
        {
            BaseResponseDTO<List<CRUDMatrix>> BaseDto = new BaseResponseDTO<List<CRUDMatrix>>();
            List<CRUDMatrix> mlRoot = new List<CRUDMatrix>();
            List<CRUDMatrix> ml = new List<CRUDMatrix>();
            NPFContext context = new NPFContext();
            try
            {
                UserMatrixDTO UM = sGetMatrixUserByUserID();

                mlRoot = (from a in context.NPF_SYS_GroupMatrix
                          where UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
                          select new CRUDMatrix()
                          {
                              id = a.GMID.ToString(),
                              text = a.GMDescription,
                              parent = "#",
                          }).ToList();
                ml = (from a in context.NPF_SYS_GroupMatrix
                      where UM.IDS.Contains(a.GMID) && !UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
                      select new CRUDMatrix()
                      {
                          id = a.GMID.ToString(),
                          text = a.GMDescription,
                          parent = a.ParentGMID.ToString(),
                      }).ToList();
                ml.AddRange(mlRoot);
                BaseDto.Data = ml;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = ml;
                BaseDto.AdditionalData = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<List<CRUDMatrix>>> GetTreeUser(string UserId)
        {
            BaseResponseDTO<List<CRUDMatrix>> BaseDto = new BaseResponseDTO<List<CRUDMatrix>>();
            List<CRUDMatrix> mlRoot = new List<CRUDMatrix>();
            List<CRUDMatrix> ml = new List<CRUDMatrix>();
            NPFContext context = new NPFContext();
            try
            {
                UserMatrixDTO UM = sGetMatrixUserByUserID();
                UserMatrixDTO UMId = sGetMatrixUserByUserIDParam(UserId);

                mlRoot = (from a in context.NPF_SYS_GroupMatrix
                          where UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
                          select new CRUDMatrix()
                          {
                              id = a.GMID.ToString(),
                              text = a.GMDescription,
                              parent = "#",
                              state = new state()
                              {
                                  Checked = false,
                                  Opened = true
                              }
                          }).ToList();
                ml = (from a in context.NPF_SYS_GroupMatrix
                      where UM.IDS.Contains(a.GMID) && !UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
                      select new CRUDMatrix()
                      {
                          id = a.GMID.ToString(),
                          text = a.GMDescription,
                          parent = a.ParentGMID.ToString(),
                      }).ToList();
                ml.AddRange(mlRoot);
                foreach (long gmid in UMId.ParentIDS)
                {
                    ml.Where(x => x.id == gmid.ToString()).Select(w => { w.state.Checked = true; return w; }).ToList();
                }
                BaseDto.Data = ml;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = ml;
                BaseDto.AdditionalData = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> CRUDM(List<CRUDMatrix> dt)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                UserMatrixDTO UM = sGetMatrixUserByUserID();
                bool Incoming = await HandleIncoming(dt);
                bool Update = await HandleUpdate(dt);
                bool Removed = await HandleRemoved(UM, dt);
                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
                if (!Incoming && !Removed)
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

        public async Task<bool> HandleIncoming(List<CRUDMatrix> dt)
        {
            List<CRUDMatrix> mlRoot = new List<CRUDMatrix>();
            List<CRUDMatrix> Incoming = new List<CRUDMatrix>();
            bool status = false;
            try
            {
                Incoming = dt.Where(x => x.id.Contains("j1_")).ToList();
                mlRoot = Incoming.Where(x => !x.parent.Contains("j1_")).ToList();
                foreach (var matrix in mlRoot)
                {
                    long result = await CreateMatrix(Convert.ToInt64(matrix.parent), matrix.text);
                    if (result > 0)
                    {
                        Incoming.Where(x => x.parent == matrix.id).Select(c => { c.parent = result.ToString(); return c; }).ToList();
                        Incoming.Where(x => x.id == matrix.id).Select(c => { c.id = result.ToString(); return c; }).ToList();
                    }
                }
                if (Incoming.Where(x => x.id.Contains("j1_")).ToList().Count > 0)
                {
                    status = await HandleIncoming(Incoming);
                    if (!status)
                    {
                        return status;
                    }
                }
                status = true;
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        public async Task<bool> HandleUpdate(List<CRUDMatrix> dt)
        {
            List<CRUDMatrix> update = new List<CRUDMatrix>();
            NPFContext context = new NPFContext();
            bool status = false;
            try
            {
                update = dt.Where(x => !x.id.Contains("j1_")).ToList();
                foreach (CRUDMatrix u in update)
                {
                    NPF_SYS_GroupMatrix gm = context.NPF_SYS_GroupMatrix.Where(x => x.GMID == Convert.ToInt64(u.id)).FirstOrDefault();
                    if (gm.GMDescription != u.text)
                    {
                        gm.GMDescription = u.text;
                        context.NPF_SYS_GroupMatrix.Update(gm);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        public async Task<bool> HandleRemoved(UserMatrixDTO UM, List<CRUDMatrix> dt)
        {
            List<long> Removed = new List<long>();
            bool status = false;
            try
            {
                List<long> Existing = await GetExisting(dt);
                Removed = UM.IDS;
                Removed.RemoveAll(x => Existing.Contains(x));
                status = true;
                status = await DeleteMatrix(Removed);
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        public async Task<List<long>> GetExisting(List<CRUDMatrix> dt)
        {
            List<long> Existing = new List<long>();
            try
            {
                foreach (string e in dt.Where(x => !x.id.Contains("j1_")).Select(y => y.id).ToList())
                {
                    Existing.Add(Convert.ToInt64(e));
                }
            }
            catch (Exception ex)
            {

            }
            return Existing;
        }
        public async Task<long> CreateMatrix(long pgmid, string gmdes)
        {
            NPFContext context = new NPFContext();
            long id = 0;
            try
            {
                NPF_SYS_GroupMatrix gm = new NPF_SYS_GroupMatrix()
                {
                    ParentGMID = pgmid,
                    GMDescription = gmdes,
                    Remarks = string.Empty,
                    IsCompany = true,
                    CompanyCode = string.Empty,
                    LegalName = string.Empty,
                };
                context.NPF_SYS_GroupMatrix.Add(gm);
                context.SaveChanges();
                id = gm.GMID;
            }
            catch (Exception ex)
            {

            }
            return id;
        }
        public async Task<bool> DeleteMatrix(List<long> Removed)
        {
            NPFContext context = new NPFContext();
            bool status = false;
            try
            {
                foreach (long Id in Removed)
                {
                    NPF_SYS_GroupMatrix gm = context.NPF_SYS_GroupMatrix.Where(x => x.GMID == Id).FirstOrDefault();
                    gm.DeleteStatus = true;
                    context.NPF_SYS_GroupMatrix.Update(gm);
                    context.SaveChanges();
                }
                status = true;
            }
            catch (Exception ex)
            {

            }
            return status;
        }

        public async Task<BaseResponseDTO<bool>> SaveUserM(List<long> Ids, string AUID)
        {
            NPFContext context = new NPFContext();
            BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
            BaseResponseDTO.Data = false;
            try
            {
                long SysUId = context.NPF_SYS_User.Where(x => x.AId == AUID).FirstOrDefault().Id;
                foreach (long Id in Ids)
                {
                    NPF_SYS_GroupMatrixUser gm = new NPF_SYS_GroupMatrixUser()
                    {
                        IID = SysUId,
                        GMID = Id
                    };
                    context.NPF_SYS_GroupMatrixUser.Add(gm);
                    context.SaveChanges();
                }
                BaseResponseDTO.Data = true;
                BaseResponseDTO.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseResponseDTO.Data = false;
                BaseResponseDTO.ErrorMessage = "Error Saving Matrix";
                BaseResponseDTO.QryResult = new QueryResult().FAILED;
            }
            return BaseResponseDTO;
        }
        public async Task<BaseResponseDTO<bool>> UpdateUserM(List<long> Ids, string AUID)
        {
            NPFContext context = new NPFContext();
            BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
            BaseResponseDTO.Data = false;
            try
            {
                long SysUId = context.NPF_SYS_User.Where(x => x.AId == AUID).FirstOrDefault().Id;
                List<long> GmUL = context.NPF_SYS_GroupMatrixUser.Where(x => x.IID == SysUId && x.DeleteStatus == false).Select(y => y.GMID).ToList();
                foreach (long Id in Ids)
                {
                    if (!GmUL.Contains(Id))
                    {
                        NPF_SYS_GroupMatrixUser gm = new NPF_SYS_GroupMatrixUser()
                        {
                            IID = SysUId,
                            GMID = Id
                        };
                        context.NPF_SYS_GroupMatrixUser.Add(gm);
                        context.SaveChanges();

                    }
                    else
                    {
                        GmUL.RemoveAll(x => x == Id);
                    }

                }
                foreach (long Id in GmUL)
                {
                    NPF_SYS_GroupMatrixUser gm = context.NPF_SYS_GroupMatrixUser.Where(x => x.GMID == Id && x.IID == SysUId && x.DeleteStatus == false).FirstOrDefault();
                    if (gm != null)
                    {
                        gm.DeleteStatus = true;
                        context.NPF_SYS_GroupMatrixUser.Update(gm);
                        context.SaveChanges();
                    }

                }
                BaseResponseDTO.Data = true;
                BaseResponseDTO.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseResponseDTO.Data = false;
                BaseResponseDTO.ErrorMessage = "Error Saving Matrix";
                BaseResponseDTO.QryResult = new QueryResult().FAILED;
            }
            return BaseResponseDTO;
        }
    }
}
