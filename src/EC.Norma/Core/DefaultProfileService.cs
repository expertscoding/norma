using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EC.Norma.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EC.Norma.Core
{
    public class DefaultProfileService : IProfileService
    {
        private readonly NormaOptions normaOptions;

        public DefaultProfileService(IOptions<NormaOptions> normaOptions)
        {
            this.normaOptions = normaOptions.Value;
        }

        public Task<IEnumerable<string>> GetProfilesAsync(AuthorizationHandlerContext context, CancellationToken cancellationToken = new())
        {
            return Task.FromResult(context.User.Claims.Where(c => c.Type == normaOptions.ProfileClaim).Select(c => c.Value).Distinct().ToList() as IEnumerable<string>);
        }
    }
}
