using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace EC.Norma.Core
{
    public interface IProfileService : INormaReplaceable
    {
        Task<IEnumerable<string>> GetProfilesAsync(AuthorizationHandlerContext context, CancellationToken cancellationToken = new());
    }
}
