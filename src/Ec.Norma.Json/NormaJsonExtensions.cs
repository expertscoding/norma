using EC.Norma.Json.Entities;
using EC.Norma.Json.Providers;
using EC.Norma.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace EC.Norma.Json
{
    public static class NormaJsonExtensions
    {

        public static IServiceCollection AddNormaJsonStore(this IServiceCollection services, IEnumerable<Profile> profiles)
        {
            services.AddSingleton(sp => new NormaContext(profiles));

            services.AddSingleton<INormaProvider, JsonNormaProvider>();

            return services;
        }
    }
}
