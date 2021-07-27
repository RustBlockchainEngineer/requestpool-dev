using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Globalization;

namespace MobileServies.Core
{

    public class DateManager
    {
        public static IFormatProvider CultureEN = new CultureInfo("en-US", true);
        public static IFormatProvider CultureAR = new CultureInfo("ar-EG", true);

        #region GetForView(String date)
        public static String GetForView(String date)
        {
            if (String.IsNullOrEmpty(date))
            {
                return "";
            }
            try
            {
                return DateTime.Parse(date).ToString(CultureAR);
            }
            catch
            {
                return "";
            }

        }
        #endregion

        #region GetForView(String date,String format)
        public static String GetForView(String date, String format)
        {
            if (String.IsNullOrEmpty(date))
            {
                return "";
            }
            try
            {
                if (String.IsNullOrEmpty(format))
                {
                    return DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString(CultureAR);
                }
                else
                {
                    return DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString(format, CultureAR);
                }
            }
            catch
            {
                return date;
            }

        }
        #endregion


    }
}