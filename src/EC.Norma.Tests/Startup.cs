using System;
using System.IdentityModel.Tokens.Jwt;
using EC.Norma.Core;
using EC.Norma.EF;
using EC.Norma.EF.Providers;
using EC.Norma.Filters;
using EC.Norma.TestUtils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EC.Norma.Tests
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
               {
                   options.Filters.Add(new AuthorizeFilter());
                   options.Filters.Add<NormaActionFilter>();
               });


            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies", options =>
            {
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
            });

            services.AddNorma(config =>
            {
                config.CacheExpiration = 10;
                config.ApplicationKey = "application1";
            });
            services.AddTransient<INormaProvider, EFNormaProvider>();
            var dbName = Configuration.GetValue<string>("dbName") ?? "TestNorma";
            services.AddDbContext<NormaContext>(options => options.UseInMemoryDatabase(dbName));

            services.AddTransient<NormaEngine>();

            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
       
    }

    public class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) => new HostBuilder().ConfigureLogging(builder =>
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerFactory>(NoOpLoggerFactory.Instance));
            builder.Services.AddSingleton<ILogger>(NoOpLogger.Instance);
        });
    }
}
