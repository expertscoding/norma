using EC.Norma.Core;
using EC.Norma.EF.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EC.Norma.EF
{
    public static class NormaEfExtensions
    {
        public static IServiceCollection AddNormaEFStore(this IServiceCollection services, System.Action<DbContextOptionsBuilder> options)
        {
            services.AddSingleton<INormaProvider, EFNormaProvider>();

            services.AddDbContext<NormaContext>(options, ServiceLifetime.Singleton);
            
            return services;
        }
    }
}
