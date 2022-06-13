using System;
using System.Linq;
using System.Threading.Tasks;
using EC.Norma.Core;
using Microsoft.AspNetCore.Authorization;

namespace NormaSample.Web
{
    public class HeadQuartersHandler : AuthorizationHandler<HeadQuartersRequirement>

    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HeadQuartersRequirement requirement)
        {
            if (context.User.Claims.FirstOrDefault(c => c.Type.Equals("OfficeLocation", StringComparison.OrdinalIgnoreCase) && c.Value.Equals("HQ", StringComparison.OrdinalIgnoreCase)) != null)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
