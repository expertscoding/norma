using System.Linq;
using System.Threading.Tasks;
using EC.Norma.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EC.Norma.Core
{
    public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
    {
        private readonly INormaProvider provider;
        private readonly ILogger<HasPermissionHandler> logger;
        private readonly NormaOptions normaOptions;

        public HasPermissionHandler(INormaProvider provider, ILogger<HasPermissionHandler> logger,
            IOptionsMonitor<NormaOptions> normaOptions)
        {
            this.provider = provider;
            this.logger = logger;
            this.normaOptions = normaOptions.CurrentValue;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
        {
            logger.LogTrace("Checking HasPermission ( for {permission}) requirement.", requirement.Permission);

            var claimValues = context.User.Claims.Where(c => c.Type == normaOptions.ProfileClaim).Select(c => c.Value).Distinct().ToArray();

            logger.LogTrace("Got profiles [{profiles} ]", claimValues.Aggregate("", (s,p) => string.Concat(s," ", p)));

            var assignments = provider.GetAssignmentsForRoles(requirement.Permission, claimValues);

            logger.LogTrace("Got permissions for profiles  [{permissions} ]", assignments.Select(a => a.Permission.Name).Aggregate("", (s, p) => string.Concat(s, " ", p)));

            if (assignments.Any())
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            logger.LogTrace("Requirement result is {result}", context.HasSucceeded);

            return Task.CompletedTask;
        }
    }
}
