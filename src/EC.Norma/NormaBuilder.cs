using System;
using Microsoft.Extensions.DependencyInjection;

namespace EC.Norma
{
    public class NormaBuilder : INormaBuilder
    {
        public NormaBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }
    }
}
