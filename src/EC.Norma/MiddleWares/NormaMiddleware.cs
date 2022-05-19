using EC.Norma.Core;
using EC.Norma.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace EC.Norma.MiddleWares
{
    public class NormaMiddleware : NormaEngine
    {
        private readonly RequestDelegate next;

        public NormaMiddleware(RequestDelegate next
            , IAuthorizationPolicyProvider policyProvider, IAuthorizationService authorizationService
            , INormaProvider provider, IOptionsMonitor<NormaOptions> options
            , ILogger<NormaMiddleware> logger): base(policyProvider, authorizationService, provider, options, logger)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var result = await base.EvalPermissions(context);

            if (result == null || result.Succeeded)
            {
                await next(context);
                return;
            }

            await context.ForbidAsync();
        }
    }
}
