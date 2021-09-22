using EC.Norma.Metadata;
using EC.Norma.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;

[assembly: InternalsVisibleTo("EC.Norma.Tests")]

namespace EC.Norma.Core
{
    public class NormaEngine
    {

        protected readonly IAuthorizationPolicyProvider policyProvider;
        protected readonly IAuthorizationService authorizationService;
        protected readonly INormaProvider provider;
        protected readonly NormaOptions normaOptions;
        protected readonly ILogger logger;

        public NormaEngine() { }

        public NormaEngine(IAuthorizationPolicyProvider policyProvider, IAuthorizationService authorizationService
            , INormaProvider provider, IOptionsMonitor<NormaOptions> options
            , ILogger logger)
        {
            this.policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
            this.authorizationService = authorizationService;
            this.provider = provider;
            normaOptions = options.CurrentValue;
            this.logger = logger;
        }

        internal virtual async Task<AuthorizationResult> EvalPermissions(HttpContext context)
        {
            logger.LogTrace("Beginning NormaEngine evaluation:");

            if (context == null) throw new ArgumentNullException(nameof(context));

            var endpoint = GetEndpoint(context);

            if (endpoint == null) throw new NullReferenceException("endpoint");
            
            AuthorizationResult result;

            if (ByPassed(endpoint))
            {
                logger.LogTrace("ByPassed by attribute");
                return null;
            }

            var permissions = GetPermissions(endpoint);
            var actions = GetActions(endpoint);
            var resource = GetResource(endpoint);
            var policy = await GetCombinedPolicyAsync(permissions, resource, actions);

            if (policy == null)
            {
                logger.LogTrace("No Permissions found, so result is defined by NoPermissionAction in Options (better define some permissions if you don´t want this default behavior)");
                result = normaOptions.NoPermissionAction == NoPermissionsBehaviour.Success ? AuthorizationResult.Success() : AuthorizationResult.Failed();
            }
            else
            {
                result = await authorizationService.AuthorizeAsync(context.User, policy);
            }

            return result;
        }

        protected virtual Endpoint GetEndpoint(HttpContext context)
        {
            return context.GetEndpoint();
        }

        protected virtual IEnumerable<string> GetPermissions(Endpoint endpoint)
        {
            return endpoint.Metadata.GetOrderedMetadata<NormaPermissionAttribute>().Select(p => p.Permission);
        }

        protected virtual bool ByPassed(Endpoint endpoint)
        {
            return endpoint.Metadata.GetOrderedMetadata<ByPassNormaAttribute>().Any();
        }

        protected virtual async Task<AuthorizationPolicy> GetCombinedPolicyAsync(IEnumerable<string> permissions, string resource, IEnumerable<string> actions)
        {
            AuthorizationPolicyBuilder policyBuilder = null;

            foreach (var permission in permissions)
            {
                var policy = await policyProvider.GetPolicyAsync($"{permission}|");

                if (policy != null)
                {
                    policyBuilder ??= new AuthorizationPolicyBuilder();

                    policyBuilder.Combine(policy);
                }
            }

            if (!string.IsNullOrWhiteSpace(resource))
            {
                foreach (var action in actions)
                {
                    var policy = await policyProvider.GetPolicyAsync($"{action}|{resource}");

                    if (policy != null)
                    {
                        policyBuilder ??= new AuthorizationPolicyBuilder();

                        policyBuilder.Combine(policy);
                    }
                }
            }

            return policyBuilder?.Build();

        }

        protected virtual IEnumerable<string> GetActions(Endpoint endpoint)
        {
            var actions = endpoint.Metadata.GetOrderedMetadata<NormaActionAttribute>().Select(p => p.Action).ToArray();

            if (!actions.Any())
            {
                var action = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()?.ActionName
                             ?? endpoint.Metadata.GetMetadata<PageActionDescriptor>()?.ViewEnginePath.TrimStart('/');
                actions = new[] { action };
            }

            return actions;
        }


        protected virtual string GetResource(Endpoint endpoint)
        {
            var resource = endpoint.Metadata.GetMetadata<NormaResourceAttribute>()?.Resource;

            if (string.IsNullOrWhiteSpace(resource))
            {
                resource = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()?.ControllerName
                             ?? endpoint.Metadata.GetMetadata<PageActionDescriptor>()?.AreaName;
            }

            return resource;
        }
    }
}
