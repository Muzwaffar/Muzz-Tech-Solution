using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.DBContext;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class LogoutService
    {

        public static async Task<BaseResponseDTO<dynamic>> LogOut(SignInManager<ApplicationUser> _signInManager)
        {
            BaseResponseDTO<dynamic> BaseResponseDTO = new BaseResponseDTO<dynamic>();
            try
            {
                //NPFContext.UGuid = null;
                await _signInManager.SignOutAsync();
                BaseResponseDTO.Data = new QueryResult().SUCEEDED;

            }
            catch (Exception)
            {
                BaseResponseDTO.Data = new QueryResult().FAILED;
            }

            return BaseResponseDTO;

        }
        public static async Task<BaseResponseDTO<dynamic>> LogOutMobile(SignInManager<ApplicationUser> _signInManager, LogoutDTO dto)
        {
            NPFContext context = new NPFContext();
            BaseResponseDTO<dynamic> BaseResponseDTO = new BaseResponseDTO<dynamic>();
            try
            {
                ApplicationUser User = context.Users.Where(x => x.Id == dto.id).FirstOrDefault();
                if (dto.fireBaseToken == null || dto.fireBaseToken == "" || dto.fireBaseToken == " ")
                {
                    BaseResponseDTO.Data = new QueryResult().FAILED;
                    return BaseResponseDTO;
                }
                context.Users.Update(User);
                await _signInManager.SignOutAsync();
                BaseResponseDTO.Data = new QueryResult().SUCEEDED;

            }
            catch (Exception ex)
            {
                BaseResponseDTO.Data = new QueryResult().FAILED;
            }

            return BaseResponseDTO;

        }
    }
}
