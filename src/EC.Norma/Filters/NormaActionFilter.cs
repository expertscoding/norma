using EC.Norma.Core;
using EC.Norma.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace EC.Norma.Filters
{
    public class NormaActionFilter : NormaEngine, IAsyncActionFilter
    {
        public NormaActionFilter(IAuthorizationPolicyProvider policyProvider, IAuthorizationService authorizationService
            , INormaProvider provider, IOptionsMonitor<NormaOptions> options
            , ILogger<NormaActionFilter> logger) : base(policyProvider, authorizationService, provider, options, logger)
        {
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context == null || context.HttpContext == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var result = await base.EvalPermissions(context.HttpContext);

            if (result == null || result.Succeeded)
            {
                await next();
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
