using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.DBContext;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class LoginService
    {


        public static async Task<BaseResponseDTO<ApplicationUser>> Login(BaseRequestDTO<LoginDTO> loginDto, SignInManager<ApplicationUser> _signInManager, UserManager<ApplicationUser> _userManager, RoleManager<ApplicationRole> _roleManager, string clientIp)
        {
            BaseResponseDTO<ApplicationUser> dto = new BaseResponseDTO<ApplicationUser>();
            using StreamWriter file = new("logservice.txt");
            file.WriteLine("OK");

            string message = "";
            ApplicationUser user = new ApplicationUser();
            QueryResult queryResult = new QueryResult();
            bool IsSuccess= false;
            bool IsStation= false;
            string errorMsg = "";
            string sQryResult = queryResult.SUCEEDED;
            try
            {
                try
                {
                    if (loginDto.Data == null)
                    {
                        file.WriteLine("null");
                    }
                }
                catch (Exception ex)
                {
                    file.WriteLine(ex);
                }
                var emailexists = await _userManager.FindByEmailAsync(loginDto.Data.Email);
                if (emailexists != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(loginDto.Data.Email, loginDto.Data.Password, loginDto.Data.RememberMe, true);
                    user = await _userManager.FindByNameAsync(loginDto.Data.Email);
                    if (result.Succeeded)
                    {                       
                        if (user.FirstTimeLogin == false)
                        {
                            user.LoginDetail = new LoginLog()
                            {
                                LastKnownIPAddress = clientIp,
                                LastLoginDate = DateTime.Now,
                                Description = "Success Login",
                                userId = user.Id
                            };
                            IsSuccess = true;
                        }
                        else
                        {
                            user.LoginDetail = new LoginLog()
                            {
                                LastKnownIPAddress = clientIp,
                                LastLoginDate = DateTime.Now,
                                Description = "Success First Time Login",
                                userId = user.Id,
                            };
                            IsSuccess = true;
                        }

                        try
                        {
                            if (user.LastPasswordChangedDate.HasValue)
                            {
                                int passwordParamForExpiry = Convert.ToInt32(new ApplicationSettingsHandler().settings.Params.GetValueOrDefault("PasswordEpxiry"));
                                DateTime dateTimeLastPasswordChangedDate = user.LastPasswordChangedDate.Value.AddDays(passwordParamForExpiry);
                                if (dateTimeLastPasswordChangedDate < DateTime.UtcNow)
                                {
                                    errorMsg = "Password Expired.Please change your Password";
                                    user.Status = "PE";

                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else if (result.IsLockedOut)
                    {
                        errorMsg = "Account Locked.Please Contact Administrator";
                        user.Status = "AL";
                        await _userManager.UpdateAsync(user);
                        user = new ApplicationUser();
                        sQryResult = queryResult.FAILED;

                    }
                    else
                    {
                        int AccountLockedAttempts = Convert.ToInt32(new ApplicationSettingsHandler().settings.Params.GetValueOrDefault("AccountLockedAttempts"));
                        errorMsg = "Invalid Credentials!! Please note that your account will be locked after " + AccountLockedAttempts + " fail login attempts.";
                        sQryResult = queryResult.FAILED;
                        user = new ApplicationUser();
                    }
                    user.FirstTimeLogin = false;
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    errorMsg = "Login Failed. You don't have access on this system";
                    sQryResult = queryResult.FAILED;
                    user = new ApplicationUser();
                }
                try
                {
                    if(IsSuccess)
                    {
                        NPFContext context = new NPFContext();
                        long Uid = context.NPF_SYS_User.Where(x => x.AId == user.LoginDetail.userId).FirstOrDefault().Id;
                        List<long> StationIds = context.NPF_SYS_Resource.Where(x => x.UserId == Uid && x.DeleteStatus == false && (x.ResourceType.Name == "Station" || x.ResourceType.Name == "Station Commander")).Select(y => y.StationUnitName.Id).ToList();
                        if(StationIds.Count > 0)
                        {
                            IsStation = true;
                        }
                    }

                }
                catch (Exception ex)
                {

                }

            }
            catch (Exception ex)
            {
                //message += "exception login service line 116 \r\n" + ex;
                sQryResult = queryResult.FAILED;
                errorMsg = ex.ToString();
            }
            Guid oneTimeToken = Guid.NewGuid();
            //message += "login service line 121 \r\n" + sQryResult + "\r\n" +user.Status;
            //file.WriteLine(message);
            user.OTT = oneTimeToken;
            await _userManager.UpdateAsync(user);
            BaseResponseDTO<ApplicationUser> Acc = AccountService.castUser(user, oneTimeToken, _userManager, _roleManager, sQryResult, errorMsg);
            Acc.Data.IsStation = IsStation;
            return Acc;
        }
    }
}
