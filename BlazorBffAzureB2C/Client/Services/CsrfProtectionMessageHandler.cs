using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorHosted.Client
{
    public class CsrfProtectionMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Add("X-FORCE-CORS-PREFLIGHT", "true");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
