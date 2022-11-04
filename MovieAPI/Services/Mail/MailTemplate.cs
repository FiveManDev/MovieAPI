using MovieAPI.Helpers;
using MovieAPI.Models;

namespace MovieAPI.Services.Mail
{
    public static class MailTemplate
    {
        public static void ConfirmEmail(string email, string code)
        {

            var mailName = email.Substring(0, email.IndexOf("@"));
            var mailModel = new MailModel
            {
                EmailTo = email,
                Subject = "Confirm your email address",
                Body = $"Welcome {mailName.ToLower()}!" +
                $"<br/><br/>" +
                $"Thanks for signing up with {AppSettings.MailTile}!" +
                $"<br/><b>{code}</b> is your {AppSettings.MailTile} verification." +
                $" <br/>" +
                $"Thank you," +
                $" <br/>" +
                $"{AppSettings.MailTile} account group"
            };
            MailService.SendMail(mailModel);
        }
        public static void VerifyEmail(string email, string code)
        {

            var mailName = email.Substring(0, email.IndexOf("@"));
            var mailModel = new MailModel
            {
                EmailTo = email,
                Subject = $"Reset {AppSettings.MailTile} account password",
                Body = $"Hello {mailName.ToLower()}!" +
                $"<br/><br/>" +
                $"Please use this code to reset the password for your {AppSettings.MailTile} account {email}" +
                $"<br/>Here is your code: <b>{code}</b>." +
                $" <br/>" +
                $"Thank you," +
                $" <br/>" +
                $"{AppSettings.MailTile} account group"

            };
            MailService.SendMail(mailModel);
        }
        public static void ServerNotify(string bucketName)
        {
            var mailModel = new MailModel
            {
                EmailTo = "Hoangkhang12789@gmail.com",
                Subject = $"Server is currently unavailable",
                Body = $"Server:{bucketName}is currently unavailable" +
                $"<br/><br/>"
                + $"The test execution time is {DateTime.Now}"
            };
            MailService.SendMail(mailModel);
        }
    }
}
