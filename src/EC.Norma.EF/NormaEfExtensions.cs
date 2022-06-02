using EC.Norma.Core;
using EC.Norma.EF.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EC.Norma.EF
{
    public static class NormaEfExtensions
    {
        public static INormaBuilder AddNormaEFStore(this INormaBuilder builder, System.Action<DbContextOptionsBuilder> options)
        {
            builder.Services.AddSingleton<INormaProvider, EFNormaProvider>();

            builder.Services.AddDbContext<NormaContext>(options, ServiceLifetime.Singleton);
            
            return builder;
        }
    }
}
