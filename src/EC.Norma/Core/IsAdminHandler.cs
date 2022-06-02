using System;
using System.Linq;
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
        private readonly IProfileService profileService;
        private readonly NormaOptions normaOptions;

        public IsAdminHandler(IOptions<NormaOptions> normaOptions, ILogger<IsAdminHandler> logger, IProfileService profileService)
        {
            this.logger = logger;
            this.profileService = profileService;
            this.normaOptions = normaOptions.Value;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminRequirement requirement)
        {
            logger.LogTrace("Checking IsAdmin requirement.");

            var profiles = await profileService.GetProfilesAsync(context);

            if (profiles.Any(p => p.Equals(normaOptions.AdministratorRoleName, StringComparison.OrdinalIgnoreCase)))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            
            logger.LogTrace("  IsAdmin requirement result is {result}", context.HasSucceeded);
        }
    }
}
