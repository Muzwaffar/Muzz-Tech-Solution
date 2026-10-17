using BL.Constants;
using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Access;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class UserService
    {

        private NPFContext context;
        private QueryResult queryResult;
        public UserService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }
        public static async Task<BaseResponseDTO<bool>> ChangePassword(BaseRequestDTO<ChangePasswordDTO> dataToReset, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            BaseResponseDTO<Boolean> BaseDto = new BaseResponseDTO<Boolean>();

            try
            {
                var user = await userManager.FindByIdAsync(dataToReset.userToken);
                var token = userManager.GeneratePasswordResetTokenAsync(user).Result;
                IdentityResult result = userManager.ResetPasswordAsync(user, token, dataToReset.Data.NewPassword).Result;

                if (result.Succeeded)
                {
                    user.LockoutEnd = null;
                    user.AccessFailedCount = 0;
                    user.LastPasswordChangedDate = DateTime.UtcNow;
                    user.Status = "AC";
                    await userManager.UpdateAsync(user);
                    BaseDto.Data = true;
                    BaseDto.QryResult = new QueryResult().SUCEEDED;
                   // var x = new HttpRequestService().Call("post", "https://naydra.naveo.mu/User/ChangePasswordTunnel", dataToReset);
                }
                else
                {
                    BaseDto.AdditionalData = "Error in changing passsword";
                    BaseDto.Data = false;
                    BaseDto.QryResult = new QueryResult().FAILED;
                }

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.QryResult = new QueryResult().FAILED;
            }


            return BaseDto;
        }

        public static async Task<BaseResponseDTO<bool>> ResetPassword(BaseRequestDTO<string> dataToReset, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            BaseResponseDTO<Boolean> BaseDto = new BaseResponseDTO<Boolean>();
            BaseResponseDTO<string> bdt = new BaseResponseDTO<string>();
            NPFContext context = new NPFContext();
            try
            {
                var user = await userManager.FindByIdAsync(dataToReset.Data);
                var token = userManager.GeneratePasswordResetTokenAsync(user).Result;
                NPF_SYS_GlobalParam Gp = context.NPF_SYS_GlobalParam.Where(x => x.Name == "DefaultPassword").FirstOrDefault();
                string NewPassword = Gp.Value;
                IdentityResult result = userManager.ResetPasswordAsync(user, token, NewPassword).Result;

                if (result.Succeeded)
                {
                    user.LockoutEnd = null;
                    user.AccessFailedCount = 0;
                    user.LastPasswordChangedDate = DateTime.UtcNow;
                    user.Status = "CP";
                    await userManager.UpdateAsync(user);
                    BaseDto.Data = true;
                    BaseDto.QryResult = new QueryResult().SUCEEDED;

                    bdt.Data = user.Email;

                  //  var x = new HttpRequestService().Call("post", "https://naydra.naveo.mu/User/ResetPasswordTunnel", bdt);
                }
                else
                {
                    BaseDto.AdditionalData = "Error in changing passsword";
                    BaseDto.Data = false;
                    BaseDto.QryResult = new QueryResult().FAILED;
                }

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.QryResult = new QueryResult().FAILED;
            }


            return BaseDto;
        }


        public static async Task<BaseResponseDTO<bool>> ForgotPassword(string Email, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();

            try
            {
                var user = await userManager.FindByEmailAsync(Email);

                if (user != null && user.Status == "AC") //check if user exist and is active
                {

                    //string sBody = "";
                    var randPassword = ConversionService.GenerateRandomPassword();


                    var token = userManager.GeneratePasswordResetTokenAsync(user).Result; //generate password reset token
                    IdentityResult result = userManager.ResetPasswordAsync(user, token, randPassword).Result; //reset password using token


                    //sBody = "Dear Valued Customer \r\n\r\n";
                    //sBody += "Please login to NPF Application using one time password '" + randPassword + "'\r\n";

                    //Prepare email model to sent to user with one time password


                    user.LastPasswordChangedDate = DateTime.UtcNow;
                    user.Status = "CP";
                    await userManager.UpdateAsync(user);
                    BaseDto.Data = true;
                    BaseDto.QryResult = new QueryResult().SUCEEDED;



                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "User doesn't exist or inactive on system!!";
                    BaseDto.QryResult = new QueryResult().SUCEEDED;
                }



            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.QryResult = new QueryResult().FAILED;
            }


            return BaseDto;
        }

        public BaseResponseDTO<bool> Delete(BaseRequestDTO<string> dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                Guid Id = Guid.Parse(dataToDelId.Data);
                BaseResponseDTO<ApplicationUser> dto = new BaseResponseDTO<ApplicationUser>();

                ApplicationUser user = context.Users.Where((x) => x.Id == dataToDelId.Data).FirstOrDefault();
                user.DeleteStatus = true;
                user.UserName = DateTime.Now.ToString("yyyyMMddHHmmss") + user.UserName;
                user.Email = DateTime.Now.ToString("yyyyMMddHHmmss") + user.Email;
                user.NormalizedEmail = DateTime.Now.ToString("yyyyMMddHHmmss") + user.NormalizedEmail;
                user.NormalizedUserName = DateTime.Now.ToString("yyyyMMddHHmmss") + user.NormalizedUserName;
                context.Users.Attach(user).Property(e => e.DeleteStatus).IsModified = true;
                context.SaveChanges();

                NPF_SYS_User usr = context.NPF_SYS_User.Where((x) => x.AId == dataToDelId.Data).FirstOrDefault();
                usr.DeleteStatus = true;
                context.NPF_SYS_User.Update(usr);
                context.SaveChanges();

                NPF_SYS_UserDetails userDetails = context.NPF_SYS_UserDetails.Where((x) => x.User.Id == usr.Id).FirstOrDefault();
                userDetails.DeleteStatus = true;
                context.NPF_SYS_UserDetails.Update(userDetails);
                context.SaveChanges();

                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;

            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Deleting User";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }

        public BaseResponseDTO<List<ApplicationUser>> GetAll()
        {
            BaseResponseDTO<List<ApplicationUser>> dto = new BaseResponseDTO<List<ApplicationUser>>();
            ApplicationUser user = new ApplicationUser();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";
            string[] exEmail = new string[] { "superadmin@naveo.mu", "admin@naveo.mu" }; //Exclude Emails
            try
            {

                string sQryResult = queryResult.FAILED;
                List<ApplicationUser> result = (List<ApplicationUser>)(from c in context.NPF_SYS_User
                                                                       join d in context.Users on c.AId equals d.Id
                                                                       join u in context.NPF_SYS_UserDetails on c.Id equals u.User.Id
                                                                       where c.DeleteStatus == false
                                                                       select new ApplicationUser
                                                                       {
                                                                           Id = d.Id,
                                                                           FirstName = d.FirstName,
                                                                           LastName = d.LastName,
                                                                           UserCode = d.UserCode,
                                                                           Email = d.Email,
                                                                           Office = context.NPF_SYS_OfficeZones.Where(x => x.Id == u.Office).FirstOrDefault().OfficeName,
                                                                           PostHeld = context.NPF_SYS_LookUpValue.Where(x => x.Id == u.PostHeld).FirstOrDefault().Name,
                                                                           UserType = context.NPF_SYS_UserType.Where(x => x.Id == u.UserType).FirstOrDefault().Name,
                                                                       }).ToList();
                //List<ApplicationUser> result = context.Users.Where(x => !exEmail.Contains(x.Email) && x.DeleteStatus == false).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<ApplicationUser>();
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
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
                List<AccessRightsDTO> result = (from r in context.Roles
                                                join ar in context.NPF_SYS_AccessRights on r.Id equals ar.Role.Id
                                                where r.DeleteStatus == false
                                                select new AccessRightsDTO
                                                {
                                                    Id = r.Id,
                                                    RoleName = r.Name
                                                }
                                             ).GroupBy(p => p.RoleName).Select(g => g.First()).ToList();


                Dd.title = "Roles";
                Dd.items = new List<DropDownItemLogin>();
                foreach (AccessRightsDTO r in result)
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    Ddi.id = r.Id;
                    Ddi.text = r.RoleName;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                string[] values = new string[] { "Title", "PostHeld" };
                List<NPF_SYS_LookUpType> Ltl = context.NPF_SYS_LookUpType.Where(x => values.Contains(x.Name) && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_LookUpType lt in Ltl)
                {
                    Dd = new DropDownLogin();
                    Dd.title = lt.Name;
                    List<NPF_SYS_LookUpValue> Lvl = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Id == lt.Id && x.DeleteStatus == false).ToList();
                    Dd.items = new List<DropDownItemLogin>();
                    foreach (NPF_SYS_LookUpValue lv in Lvl)
                    {
                        DropDownItemLogin Ddi = new DropDownItemLogin();
                        Ddi.id = lv.Id.ToString();
                        Ddi.text = lv.Name;
                        Dd.items.Add(Ddi);
                    }

                    Ddl.Add(Dd);
                }


                Dd = new DropDownLogin();
                List<NPF_SYS_ResourceType> RTL = context.NPF_SYS_ResourceType.Where(x => x.IsSystem == true && x.DeleteStatus == false).ToList();
                Dd.title = "ResourceType";
                Dd.items = new List<DropDownItemLogin>();
                foreach (NPF_SYS_ResourceType rt in RTL)
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    Ddi.id = rt.Id.ToString();
                    Ddi.text = rt.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDownLogin();
                List<NPF_SYS_OfficeZones> OFZ = context.NPF_SYS_OfficeZones.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "Office";
                Dd.items = new List<DropDownItemLogin>();
                foreach (NPF_SYS_OfficeZones of in OFZ)
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    Ddi.id = of.Id.ToString();
                    Ddi.text = of.OfficeName;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDownLogin();
                List<NPF_SYS_UserType> UT = context.NPF_SYS_UserType.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "UserType";
                Dd.items = new List<DropDownItemLogin>();
                foreach (NPF_SYS_UserType of in UT)
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    Ddi.id = of.Id.ToString();
                    Ddi.text = of.Name;
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

        public BaseResponseDTO<UserDTO> GetById(string Id)
        {
            BaseResponseDTO<UserDTO> dto = new BaseResponseDTO<UserDTO>();
            ApplicationUser result = new ApplicationUser();
            try
            {
                result = context.Users.Where(x => x.Id == Id).FirstOrDefault();
                UserDTO usr = new UserDTO()
                {
                    Id = Id,
                    Code = result.UserCode,
                    UserToken = result.UserToken,
                    Title = context.NPF_SYS_LookUpValue.Where(x => x.Name == result.Title).FirstOrDefault().Id,
                    FirstName = result.FirstName,
                    LastName = result.LastName,
                    Email = result.Email,
                    TelephoneNo = result.PhoneNumber,
                    Roles = context.UserRoles.Where(x => x.UserId == Id).FirstOrDefault().RoleId,
#pragma warning disable CS8629 // Nullable value type may be null.
                    ResourceType = (long)(from ur in context.NPF_SYS_User
                                          join u in context.NPF_SYS_UserDetails on ur.Id equals u.User.Id
                                          where ur.AId == Id
                                          select u.Type).FirstOrDefault(),
                    Office = (from ur in context.NPF_SYS_User
                              join u in context.NPF_SYS_UserDetails on ur.Id equals u.User.Id
                              where ur.AId == Id
                              select u.Office).FirstOrDefault(),
                    PostHeld = (from ur in context.NPF_SYS_User
                                join u in context.NPF_SYS_UserDetails on ur.Id equals u.User.Id
                                where ur.AId == Id
                                select u.PostHeld).FirstOrDefault(),
                    UserType = (from ur in context.NPF_SYS_User
                                join u in context.NPF_SYS_UserDetails on ur.Id equals u.User.Id
                                where ur.AId == Id
                                select u.UserType).FirstOrDefault(),
                };
#pragma warning restore CS8629 // Nullable value type may be null.
                if (result == null)
                {
                    dto.Data = usr;
                    dto.QryResult = queryResult.FAILED;
                }
                else
                {
                    dto.Data = usr;
                    dto.QryResult = queryResult.SUCEEDED;
                }
            }
            catch (Exception ex)
            {
                dto.Data = new UserDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public BaseResponseDTO<bool> Save(BaseRequestDTO<ApplicationUser> dataToSave)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponseDTO<bool>> SaveUser(BaseRequestDTO<UserDTO> dataToSave, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> roleManager)
        {
            BaseResponseDTO<bool> dto = new BaseResponseDTO<bool>();
            BaseResponseDTO<ApplicationUser> dt = new BaseResponseDTO<ApplicationUser>();
            string message = "";

            try
            {
                if (!userManager.Users.Any(x => x.Email.ToLower() == dataToSave.Data.Email.ToLower() && x.DeleteStatus != true))
                {

                    ApplicationUser au = new ApplicationUser();
                    au.Id = Guid.NewGuid().ToString();
                    au.FirstName = dataToSave.Data.FirstName;
                    au.LastName = dataToSave.Data.LastName;
                    au.PhoneNumber = dataToSave.Data.TelephoneNo;
                    au.Title = context.NPF_SYS_LookUpValue.Where(x => x.Id == dataToSave.Data.Title).FirstOrDefault().Name;
                    au.UserToken = Guid.Parse(au.Id);
                    au.UserName = dataToSave.Data.Email;
                    //au.PasswordHash = dataToSave.Data.PasswordHash;
                    au.Email = dataToSave.Data.Email;
                    au.NormalizedUserName = dataToSave.Data.Email.ToUpper();
                    au.NormalizedEmail = dataToSave.Data.Email.ToUpper();
                    au.Status = "CP";
                    au.CreatedDate = DateTime.Now;
                    au.CreatedBy = Guid.Parse(dataToSave.userToken);
                    var code = await new GenerateCode().GCode();
                    au.UserCode = "USR" + code.Data;
                    IdentityResult result = userManager.CreateAsync(au).Result;


                    if (result.Succeeded)
                    {
                        var roleName = roleManager.FindByIdAsync(dataToSave.Data.Roles);
                        userManager.AddToRoleAsync(au, roleName.Result.Name).Wait();


                        #region generate randdom password and sent to user by mail 
                        try
                        {
                            var randPassword = ConversionService.GenerateRandomPassword();
                            var user = await userManager.FindByEmailAsync(au.Email);
                            var token = userManager.GeneratePasswordResetTokenAsync(user).Result; //generate password reset token
                            IdentityResult resultResetPasword = userManager.ResetPasswordAsync(user, token, randPassword).Result; //reset password using token
                            au.PasswordHash = randPassword;
                            dt.Data = au;
                            try
                            {
                                BaseRequestDTO<TunnelApplicationUser> dtt = new BaseRequestDTO<TunnelApplicationUser>();
                                TunnelApplicationUser Tasur = new TunnelApplicationUser()
                                {
                                    Title = au.Title,
                                    Email = au.Email,
                                    NormEmail = au.NormalizedEmail,
                                    NormUsername = au.NormalizedUserName,
                                    Username = au.UserName,
                                    FirstName = au.FirstName,
                                    LastName = au.LastName,
                                    FkID = au.Id,
                                };
                                dtt.Data = Tasur;

                                NPF_SYS_GlobalParam Gp = context.NPF_SYS_GlobalParam.Where(x => x.Name == "ExternalURL").FirstOrDefault();
                                string url = Gp.Value;
                                var x = new HttpRequestService().Call("post", url+"/Users/CreateTunnel", dtt);
                            }
                            catch (Exception ex)
                            {

                            }

                            //Prepare email model to sent to user with one time password
                            NPF_SYS_GlobalParam Gpa = context.NPF_SYS_GlobalParam.Where(x => x.Name == "ApplicationURL").FirstOrDefault();
                            string URL = Gpa.Value;
                            EmailSenderModel<dynamic> esm = new EmailSenderModel<dynamic>();
                            esm.To = new List<string>() { user.Email };
                            esm.Subject = "User Creation";
                            esm.From = "noreply@naveo.mu";
                            esm.Body = new List<string>() { "Dear Valued Customer", "Please note that you have added to the Naveo Platform Application. ", "This is your login credentials ; ", "Email : " + user.Email + " ", "Password : " + randPassword + "", "URL : " + URL + "" };
                            var aaa = await new EmailSender().SendEmail(esm, userManager);
                        }
                        catch (Exception ex)
                        {
                            dto.Data = false;
                            dto.AdditionalData = "User save Sucessfully but couldn't mail user its password";
                            dto.QryResult = new QueryResult().FAILED;
                        }
                        #endregion

                        NPF_SYS_User usr = new NPF_SYS_User()
                        {
                            AId = au.Id
                        };
                        context.NPF_SYS_User.Add(usr);
                        context.SaveChanges();

                        NPF_SYS_UserDetails nusr = new NPF_SYS_UserDetails()
                        {
                            User = context.NPF_SYS_User.Where(x => x.AId == au.Id).FirstOrDefault(),
                            Type = dataToSave.Data.ResourceType,
                            Office = dataToSave.Data.Office,
                            PostHeld = dataToSave.Data.PostHeld,
                            UserType = dataToSave.Data.UserType,

                        };
                        context.NPF_SYS_UserDetails.Add(nusr);
                        context.SaveChanges();

                        dto.Data = true;
                        dto.AdditionalData = "User save Successfully";
                        dto.ExtData = usr.AId;
                        dto.QryResult = new QueryResult().SUCEEDED;


                    }
                    else
                    {
                        dto.Data = false;
                        dto.AdditionalData = "User save Fail";
                        dto.QryResult = new QueryResult().FAILED;
                    }

                }
                else
                {
                    dto.Data = false;
                    dto.AdditionalData = "User Already Exist";
                    dto.QryResult = new QueryResult().FAILED;
                }

            }
            catch (Exception ex)
            {
                dto.Data = false;
                dto.AdditionalData = "User save Fail";
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;
        }

        public BaseResponseDTO<bool> Update(BaseRequestDTO<ApplicationUser> dataToUpdate)
        {
            throw new NotImplementedException();
        }
        public async Task<BaseResponseDTO<SyncUserDTO>> GetTunnelDT(string Usetoken, string password)
        {
            NPFContext context = new NPFContext();
            BaseResponseDTO<SyncUserDTO> BaseResponseDTO = new BaseResponseDTO<SyncUserDTO>();
            try
            {
                var usr = context.Users.Where(x => x.UserToken == Guid.Parse(Usetoken)).FirstOrDefault();
                SyncUserDTO su = new SyncUserDTO()
                {
                    Email = usr.Email,
                    Password = password,
                    UserId = usr.Id
                };

                BaseResponseDTO.Data = su;
                BaseResponseDTO.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseResponseDTO.ErrorMessage = "Error";
                BaseResponseDTO.QryResult = new QueryResult().FAILED;
            }
            return BaseResponseDTO;
        }

        public BaseResponseDTO<bool> UpdateUser(BaseRequestDTO<UserDTO> dataToUpdate, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> roleManager)
        {
            BaseResponseDTO<bool> dto = new BaseResponseDTO<bool>();
            BaseResponseDTO<ApplicationUser> dt = new BaseResponseDTO<ApplicationUser>();
            string message = "";

            try
            {
                var au = userManager.FindByIdAsync(dataToUpdate.Data.Id).Result;
                au.FirstName = dataToUpdate.Data.FirstName;
                au.LastName = dataToUpdate.Data.LastName;
                au.PhoneNumber = dataToUpdate.Data.TelephoneNo;
                au.Title = context.NPF_SYS_LookUpValue.Where(x => x.Id == dataToUpdate.Data.Title).FirstOrDefault().Name;
                au.UserName = dataToUpdate.Data.Email;
                au.Email = dataToUpdate.Data.Email;
                au.NormalizedUserName = dataToUpdate.Data.Email.ToUpper();
                au.NormalizedEmail = dataToUpdate.Data.Email.ToUpper();
                au.UpdatedDate = DateTime.Now;
                au.UpdatedBy = Guid.Parse(dataToUpdate.userToken);

                IdentityResult result = userManager.UpdateAsync(au).Result;
                dt.Data = au;
                try
                {
                    BaseRequestDTO<TunnelApplicationUser> dtt = new BaseRequestDTO<TunnelApplicationUser>();
                    TunnelApplicationUser Tasur = new TunnelApplicationUser()
                    {
                        Title = au.Title,
                        Email = au.Email,
                        NormEmail = au.NormalizedEmail,
                        NormUsername = au.NormalizedUserName,
                        Username = au.UserName,
                        FirstName = au.FirstName,
                        LastName = au.LastName,
                        FkID = au.Id,
                    };

                    dtt.Data = Tasur;
                    NPF_SYS_GlobalParam Gp = context.NPF_SYS_GlobalParam.Where(x => x.Name == "ExternalURL").FirstOrDefault();
                    string url = Gp.Value;
                    var x = new HttpRequestService().Call("post", url+"/Users/UpdateTunnel", dtt);
                }
                catch (Exception ex)
                {

                }

                NPF_SYS_User usr = context.NPF_SYS_User.Where(x => x.AId == dataToUpdate.Data.Id).FirstOrDefault();
                usr.AId = dataToUpdate.Data.Id;
                context.NPF_SYS_User.Update(usr);
                context.SaveChanges();

                NPF_SYS_UserDetails nusr = context.NPF_SYS_UserDetails.Where(x => x.User.Id == usr.Id).FirstOrDefault();
                nusr.User = usr;
                nusr.Type = dataToUpdate.Data.ResourceType;
                nusr.Office = dataToUpdate.Data.Office;
                nusr.PostHeld = dataToUpdate.Data.PostHeld;
                nusr.UserType = dataToUpdate.Data.UserType;
                context.NPF_SYS_UserDetails.Update(nusr);
                context.SaveChanges();

                var roles = userManager.GetRolesAsync(au).Result;
                if (roles.Count == 0)
                {
                    var roleName = roleManager.FindByIdAsync(dataToUpdate.Data.Roles).Result;
                    userManager.AddToRoleAsync(au, roleName.Name).Wait();

                    dto.Data = true;
                    dto.QryResult = new QueryResult().SUCEEDED;
                }
                else
                {

                    var resultremoveroles = userManager.RemoveFromRolesAsync(au, roles).Result;
                    var roleName = roleManager.FindByIdAsync(dataToUpdate.Data.Roles).Result;
                    userManager.AddToRoleAsync(au, roleName.Name).Wait();

                    dto.Data = true;
                    dto.ExtData = usr.AId;
                    dto.QryResult = new QueryResult().SUCEEDED;

                }



            }
            catch (Exception ex)
            {
                dto.Data = false;
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;

        }
    }
}
