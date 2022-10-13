using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using MovieAPI.Models;

namespace MovieAPI.Services.Mail
{
    public static class MailService
    {
        public static bool SendMail(MailModel mailModel)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(AppSettings.Mail, AppSettings.MailTile);
                mailMessage.To.Add(new MailAddress(mailModel.EmailTo));
                mailMessage.Subject = mailModel.Subject;
                mailMessage.Body = mailModel.Body;

                if (mailModel.CC != null)
                {
                    foreach (var cc in mailModel.CC)
                    {
                        mailMessage.CC.Add(cc);
                    }
                }
                if (mailModel.Attachments != null)
                {
                    foreach (var attachment in mailModel.Attachments)
                    {
                        string fileName = Path.GetFileName(attachment.FileName);
                        mailMessage.Attachments.Add(new Attachment(attachment.OpenReadStream(), fileName));
                    }
                }
                mailMessage.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = AppSettings.Host;
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential(AppSettings.Mail, AppSettings.MailAppPassword);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = Int32.Parse(AppSettings.Port);
                smtp.Send(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
