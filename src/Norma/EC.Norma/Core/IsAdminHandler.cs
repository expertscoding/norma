using System.Threading.Tasks;
using EC.Norma.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EC.Norma.Core
{
    public class IsAdminHandler : AuthorizationHandler<IsAdminRequirement>
    {
        private readonly ILogger<IsAdminHandler> logger;
        private readonly NormaOptions normaOptions;

        public IsAdminHandler(IOptions<NormaOptions> normaOptions, ILogger<IsAdminHandler> logger)
        {
            this.logger = logger;
            this.normaOptions = normaOptions.Value;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminRequirement requirement)
        {
            logger.LogTrace("Checking IsAdmin requirement.");

            if (context.User.IsInRole(normaOptions.AdministratorRoleName))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            
            logger.LogTrace("  IsAdmin requirement result is {result}", context.HasSucceeded);

            return Task.CompletedTask;
        }
    }
}