using EC.Norma.Core;
using EC.Norma.MiddleWares;
using EC.Norma.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace EC.Norma
{
    public static class NormaExtensions
    {
        public static INormaBuilder AddNorma(this IServiceCollection services, Action<NormaOptions> normaOptions = null, Action<AuthorizationOptions> authOptions = null)
        {
            var builder = services.AddNormaCore(normaOptions);

            authOptions ??= options => options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            builder.Services.AddAuthorization(authOptions);

            return builder;
        }

        public static INormaBuilder AddNormaCore(this IServiceCollection services, Action<NormaOptions> normaOptions = null)
        {
            var builder = new NormaBuilder(services);

            var optionsBuilder = builder.Services.AddOptions<NormaOptions>();
            if (normaOptions != null)
            {
                optionsBuilder.Configure(normaOptions);
            }

            services.AddTransient<HasPermissionRequirement>();
            services.AddTransient<IsAdminRequirement>();

            services.AddTransient<IAuthorizationHandler, HasPermissionHandler>();
            services.AddTransient<IAuthorizationHandler, IsAdminHandler>();

            services.AddTransient<IProfileService, DefaultProfileService>();

            services.AddSingleton<IAuthorizationPolicyProvider, NormaPolicyProvider>();

            return builder;
        }

        public static INormaBuilder ReplaceService<TService, TImplService>(this INormaBuilder builder)
            where TService : INormaReplaceable
        {
            var service = builder.Services?.LastOrDefault(s => s.ServiceType == typeof(TService));

            if (service != null)
            {
                builder.Services.Remove(service);
                builder.Services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplService), service.Lifetime));
            }

            return builder;
        }

        public static IApplicationBuilder UseNorma(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<NormaMiddleware>();

            return appBuilder;
        }
    }
}
