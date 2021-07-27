using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Core
{
    public class DateTimeModelBinder : DefaultModelBinder
    {
        private string _customFormat;

        public DateTimeModelBinder(string customFormat)
        {
            _customFormat = customFormat;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var DateFormat = bindingContext.ValueProvider.GetValue("DateFormat");
            if (DateFormat != null && !String.IsNullOrEmpty(DateFormat.AttemptedValue))
                _customFormat = DateFormat.AttemptedValue;
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (!String.IsNullOrEmpty(value.AttemptedValue))
            {
                try {
                    return DateTime.ParseExact(value.AttemptedValue, _customFormat, CultureInfo.InvariantCulture);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }
}