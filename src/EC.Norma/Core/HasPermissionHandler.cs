using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace EC.Norma.Core
{
    public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
    {
        private readonly INormaProvider provider;
        private readonly ILogger<HasPermissionHandler> logger;
        private readonly IProfileService profileService;

        public HasPermissionHandler(INormaProvider provider, ILogger<HasPermissionHandler> logger, IProfileService profileService)
        {
            this.provider = provider;
            this.logger = logger;
            this.profileService = profileService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
        {
            logger.LogTrace("Checking HasPermission ( for {permission}) requirement.", requirement.Permission);

            var profiles = (await profileService.GetProfilesAsync(context))?.ToList() ?? new List<string>();

            logger.LogTrace("Got profiles [{profiles} ]", profiles.Aggregate("", (s,p) => string.Concat(s," ", p)));

            var assignments = provider.GetAssignmentsForRoles(requirement.Permission, profiles);

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
        }
    }
}
