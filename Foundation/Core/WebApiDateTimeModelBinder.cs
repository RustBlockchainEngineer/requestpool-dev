using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Foundation.Core
{
    public class WebApiDateTimeModelBinder : IModelBinder
    {
        private string _customFormat;

        public WebApiDateTimeModelBinder(string customFormat)
        {
            _customFormat = customFormat;
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var DateFormat = bindingContext.ValueProvider.GetValue("DateFormat");
            if (DateFormat != null && !String.IsNullOrEmpty(DateFormat.AttemptedValue))
                _customFormat = DateFormat.AttemptedValue;
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null || String.IsNullOrEmpty(DateFormat.AttemptedValue))
                return false;
            try
            {
                DateTime.ParseExact(value.AttemptedValue, _customFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
                bindingContext.ModelState.AddModelError(
                key: bindingContext.ModelName,
                errorMessage: $"Invalid date format for {bindingContext.ModelName}:{value}. Should be in '{DateFormat}'");
                return false;
            }

            return false;
        }
    }
}