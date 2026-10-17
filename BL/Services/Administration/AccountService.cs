using BL.Constants;
using BL.Models.Common;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class AccountService
    {
        public static async Task<IList<string>> GetRole(UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            return await userManager.GetRolesAsync(user).ConfigureAwait(true);
        }

        public static async Task<ApplicationRole> GetRoleId(RoleManager<ApplicationRole> rolemanager, string rolename)
        {
            ApplicationRole oo = await rolemanager.FindByIdAsync(rolename);

            return oo;
        }

        public static BaseResponseDTO<ApplicationUser> castUser(ApplicationUser u, Guid oneTimeToken, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> rolemanager, string sQryResult,string errorMsg)
        {
      
            BaseResponseDTO<ApplicationUser> d = new BaseResponseDTO<ApplicationUser>();
            ApplicationUser au = new ApplicationUser();
          
                au.Id = u.Id;
                au.Email = u.Email;
                au.UserName = u.UserName;
                au.Status = u.Status;
                au.UserToken = u.UserToken;
                au.FirstName = u.FirstName;
                au.LastName = u.LastName;
                au.OTT = u.OTT;
                au.Logout = false;

                d.Data = au;
                d.oneTimeToken = oneTimeToken;
                d.AdditionalData = errorMsg;
                d.QryResult = sQryResult;


                Task<IList<string>> userole = GetRole(userManager, au);
                if (userole.Result.Count > 0)
                {
                    var a = rolemanager.FindByNameAsync(userole.Result[0].ToString()).Result;
                    au.PhoneNumber = a.Id.ToString();
                    d.RoleId = Guid.Parse(a.Id);
                }
            

            return d;
        }

    }
}
