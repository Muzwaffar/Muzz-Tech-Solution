using BL.Models.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class EmailSender
    {
        public async Task<string> SendEmail(EmailSenderModel<dynamic> mailData, UserManager<ApplicationUser> userManager)
        {
            string msg = "Success";
            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            string claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
            string UGuid = new NPFContext().Users.Where(x => x.UserName == claimsPrincipal).Select(x => x.Id).FirstOrDefault();
            ApplicationUser user = new ApplicationUser();
            if (mailData.From != null || mailData.From != string.Empty)
            {
                if (mailData.From == "system")
                {
                    if (userManager.FindByIdAsync(UGuid).Result == null)
                    {
                        mailData.From = "noreply@naveo.mu";
                        msg = "Error User Not Authorised";
                    }
                    else
                    {
                        user = await userManager.FindByIdAsync(UGuid);
                        mailData.From = user.Email;
                    }
                }

            }

            NPFContext context = new NPFContext();
            EmailConfiguration Config = new EmailConfiguration();
            if (context.EmailConfiguration.Any(x => x.CompanyId == user.Company))
            {
                Config = context.EmailConfiguration.Where(x => x.CompanyId == user.Company).FirstOrDefault();
            }
            else
            {
                Config = context.EmailConfiguration.Where(x => x.CompanyId == -1).FirstOrDefault();
                // msg = "No Configuration Found For User";

            }

            // Init the smtp client and setting the network credentials
            SmtpClient smtpClient = new SmtpClient();

            smtpClient.Host = Config.Host;
            smtpClient.Port = Config.Port;
            smtpClient.EnableSsl = false;
            smtpClient.Credentials = new NetworkCredential(Config.Username, Config.Password);

            // Create MailMessage
            MailMessage message = new MailMessage();

            bool isTestEnv = mailData.IsTest;

            List<string> emailList = new List<string>();

            if (isTestEnv)
            {
                foreach (string usermail in mailData.Users)
                {
                    if (usermail.Contains("@naveo.mu"))
                    {
                        emailList.Add(usermail);
                    }
                }
            }
            else
            {
                foreach (string usermail in mailData.Users)// from model
                {
                    emailList.Add(usermail);
                }
            }

            try
            {
                foreach (var ToEmail in mailData.To)
                {
                    message.To.Add(new MailAddress(ToEmail)); //Adding Multiple TO email Id
                }
                foreach (var bccEmail in mailData.Bcc)
                {
                    message.Bcc.Add(new MailAddress(bccEmail)); //Adding Multiple BCC email Id
                }

                foreach (var ccEmail in mailData.Cc)
                {
                    message.CC.Add(new MailAddress(ccEmail)); //Adding Multiple CC email Id
                }
                message.IsBodyHtml = false;
                //for loop  for body and newline
                for (int i = 0; i < mailData.Body.Count; i++)
                {
                    message.Body += mailData.Body[i];
                    message.Body += Environment.NewLine;
                }
                if (isTestEnv)
                {
                    message.Body += "Mail sent for testing purposes. Please disregard." + Environment.NewLine;
                }


                message.Subject = mailData.Subject;///from model

                message.From = new MailAddress(mailData.From, "NAVEO ONE System");//from model then from user table
                smtpClient.Timeout = 1000000;

                // Send Mail via SmtpClient
                if (msg == "Success")
                {

                    smtpClient.Send(message);
                    message.Dispose();
                }
                return msg;
            }
            catch (Exception ex)
            {
                msg = "error";
                return msg;
            }

        }

    }
}
