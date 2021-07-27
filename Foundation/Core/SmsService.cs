using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foundation.Core
{
    public static class SmsService
    {

        public static bool Send(string phone, string message)
        {
            bool Result = false;

            if (!AppSettings.UseSMS)
            {
                return MailService.Send(null,null, message);
            }
            else
            {
                if (String.IsNullOrEmpty(phone) || AppSettings.UseDefaultPhone)
                {
                    phone = AppSettings.DefaultPhone;
                }
                //try
                //{
                //    Vodafone.Recepient R = new Vodafone.Recepient();
                //    R.ReceiverMSISDN = phone;
                //    R.SMSText = message;
                //    Result = Vodafone.Web2SMS.Send(new List<Vodafone.Recepient>() { R }).FinalStatus;
                //}
                //catch (Exception ex)
                //{
                //    Result = false;
                //}
            }

            return Result;
        }

    }
}