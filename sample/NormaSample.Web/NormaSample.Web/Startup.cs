using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using EC.Norma;
using EC.Norma.Json;
using EC.Norma.Json.Entities;
using EC.Norma.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

using EC.Norma.EF;
using Microsoft.EntityFrameworkCore;

namespace NormaSample.Web
{
    public class Startup
    {
        private const string OidCScheme = "oidc";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options => options.Filters.Add(new AuthorizeFilter()));

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OidCScheme;
                options.DefaultForbidScheme = OidCScheme;
            })
            .AddCookie("Cookies", options =>
            {
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
                options.AccessDeniedPath = "/Home/AccessDenied";
            })
            .AddOpenIdConnect(OidCScheme, options =>
            {
                options.AccessDeniedPath = "/Home/AccessDenied";
                options.SignInScheme = "Cookies";

                options.Authority = Configuration.GetValue<string>("AppGlobal:Oidc:Authority");
                options.RequireHttpsMetadata = false;

                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");

                options.SignedOutRedirectUri = Configuration.GetValue<string>("AppGlobal:Oidc:SignedOutRedirectUri");

                options.ClientId = Configuration.GetValue<string>("AppGlobal:Oidc:ClientId");
                options.ClientSecret = Configuration.GetValue<string>("AppGlobal:Oidc:ClientSecret");
                options.GetClaimsFromUserInfoEndpoint = true;


                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = IdentityModel.JwtClaimTypes.Name,
                    RoleClaimType = IdentityModel.JwtClaimTypes.Role
                };
            });

            services.AddNorma(opt =>
            {
                opt.MissingRequirementAction = MissingRequirementBehaviour.LogOnly;
                opt.NoPermissionAction = NoPermissionsBehaviour.Failure;
            })
            .AddNormaEFStore(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("Norma"));
                opt.EnableSensitiveDataLogging();
            });
            //.AddNormaJsonStore(Configuration.GetSection("profiles").Get<List<Profile>>());

            services.AddMemoryCache();
        }

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
            app.UseNorma();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
