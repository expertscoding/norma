using EC.Norma.Core;
using EC.Norma.MiddleWares;
using EC.Norma.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EC.Norma
{
    public static class NormaExtensions
    {
        public static IServiceCollection AddNorma(this IServiceCollection services)
        {
            services.AddTransient<HasPermissionRequirement>();
            services.AddTransient<IsAdminRequirement>();

            services.AddTransient<IAuthorizationHandler, HasPermissionHandler>();
            services.AddTransient<IAuthorizationHandler, IsAdminHandler>();

            services.AddSingleton<IAuthorizationPolicyProvider, NormaPolicyProvider>();

            return services;
        }

        public static IServiceCollection AddNorma(this IServiceCollection services, Action<NormaOptions> options)
        {
            services.AddNorma();

            services.Configure(options);

            return services;
        }

        public static IApplicationBuilder UseNorma(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<NormaMiddleware>();

            return appBuilder;
        }
    }
}
