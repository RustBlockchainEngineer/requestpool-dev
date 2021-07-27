using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Foundation.Core
{
    public class ApiLocalizationMessageHandler: DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CultureInfo cultureInfo = new CultureInfo("ar");

            try
            {
                var cultrueHeader = request.Headers.GetValues("X-Culture");
                cultureInfo = new CultureInfo(cultrueHeader.First());
            }
            catch
            {
            }
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
            return await base.SendAsync(request,cancellationToken);
        }
    }
}