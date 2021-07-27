using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Foundation.Core
{
    public static class MailService
    {
        public static bool Send(string address, string subject, string message)
        {
            if (String.IsNullOrEmpty(address) || AppSettings.UseDefaultEmail)
            {
                address = AppSettings.DefaultEmail;
            }
            
            bool Result = false;
            MailMessage mailMessage = new MailMessage();
            if (String.IsNullOrEmpty(subject))
            {
                mailMessage.Subject = Resources.Mail.default_subject;
            }
            mailMessage.To.Add(new MailAddress(address));
            mailMessage.Subject = Resources.Front.activation_subject;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient();
            try
            {
                smtpClient.Send(mailMessage);
                Result = true;
            }
            catch (Exception ex)
            {
            }
            return Result;
        }
    }
}