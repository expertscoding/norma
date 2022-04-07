using EC.Norma.Entities;
using EC.Norma.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EC.Norma.Core
{
    public class NormaPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string SPACENAME = "EC.Norma.Core.";
        private const string REQ_SUFFIX = "Requirement";

        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
        protected readonly IMemoryCache cache;
        protected readonly IServiceProvider services;
        protected readonly ILogger<NormaPolicyProvider> logger;
        protected readonly NormaOptions normaOptions;

        protected INormaProvider provider;

        public NormaPolicyProvider(IOptions<AuthorizationOptions> options, INormaProvider provider,
            IOptionsMonitor<NormaOptions> normaOptions,
            IMemoryCache cache, IServiceProvider serviceProvider, ILogger<NormaPolicyProvider> logger)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
            this.provider = provider;
            this.normaOptions = normaOptions.CurrentValue;
            this.cache = cache;
            services = serviceProvider;
            this.logger = logger;
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            logger.LogTrace("Requesting Policy: {policyName}", policyName);

            var reqParams = policyName.Split('|');
            var action = reqParams[0];
            var resource = reqParams.Length > 1 ? reqParams[1] : "";

            var policies = GetNormaPolicies(action, resource).ToArray();
            var policyBuilder = new AuthorizationPolicyBuilder();

            logger.LogTrace("Got policies: [{policies} ]", policies.Select(p => p.Name).Aggregate("", (s, p) => string.Concat(s, " ", p)));

            if (!policies.Any())
            {
                logger.LogTrace("No Norma policy found, requesting fallback provider");

                return FallbackPolicyProvider.GetPolicyAsync(policyName);
            }


            foreach (var policy in policies)
            {
                try
                {
                    var requirementName = SPACENAME + policy.Name + REQ_SUFFIX;

                    logger.LogTrace("Getting requirement {requirement}", requirementName);

                    var type = GetRequirementType(requirementName);

                    logger.LogTrace("Requirement acquired. Getting Permissions.");

                    string cacheKey = $"{CacheKeys.NormaPermissions}|{action}|{resource ?? ""}";

                    var permissions = cache.Get<ICollection<Permission>>(cacheKey);
                    if (permissions == null)
                    {
                        permissions = string.IsNullOrWhiteSpace(resource) ? provider.GetPermissions(action) : provider.GetPermissions(action, resource);
                        cache.Set(cacheKey, permissions, DateTime.Now.AddSeconds(normaOptions.CacheExpiration));
                    }

                    foreach (var priority in GetPriorities(policy))
                    {
                        if (services.GetService(type) is NormaRequirement requirement)
                        {
                            requirement.Action = action;
                            requirement.Resource = resource;
                            requirement.Permission = permissions.First().Name;
                            requirement.Priority = priority;
                            policyBuilder.AddRequirements(requirement);

                            logger.LogTrace("Requirement Added (Action: {action}, Resource: {resource}, Permission: {permission}, Priority: {priority})", action, resource, requirement.Permission, priority);
                        }
                        else
                        {
                            throw new Exception("No requirement located.");
                        }
                    }


                }
                catch (Exception ex)
                {
                    logger.LogError("No requirement located for policy {policy}.", policy.Name);
                    if (normaOptions.MissingRequirementAction == MissingRequirementBehaviour.ThrowException)
                    {
                        throw new TypeLoadException($"NormaRequirement for {policy} not found", ex);
                    }

                }
            }

            if (!policyBuilder.Requirements.Any())
            {
                logger.LogTrace("No requirements found. DefaultRequirement is added to deny access");

                var defaultReq = new DenyRequirement();

                policyBuilder.AddRequirements(defaultReq);
            }

            return Task.FromResult(policyBuilder.Build());
        }

        private IEnumerable<Policy> GetNormaPolicies(string action, string resource)
        {
            logger.LogTrace("Getting Norma Policies");
            string cacheKey = $"{CacheKeys.NormaPolicies}|{action}|{resource ?? ""}";

            var policies = cache.Get<ICollection<Policy>>(cacheKey);
            if (policies == null)
            {
                policies = string.IsNullOrWhiteSpace(resource) ? provider.GetPoliciesForPermission(action) : provider.GetPoliciesForActionResource(action, resource);
                cache.Set(cacheKey, policies, DateTime.Now.AddSeconds(normaOptions.CacheExpiration));
            }

            return policies;
        }


        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();


        private Type GetRequirementType(string className)
        {
            Type requirementType = null;

            if (!string.IsNullOrWhiteSpace(className) && !cache.TryGetValue(className, out requirementType))
            {
                var dependencies = DependencyContext.Default.RuntimeLibraries;
                foreach (var library in dependencies)
                {
                    try
                    {
                        requirementType = Assembly.Load(new AssemblyName(library.Name)).GetType(className);
                    }
                    catch
                    {
                        //ignored. Requirement type checked on caller method
                    }

                    if (requirementType != null) 
                        break;
                }

                cache.Set(className, requirementType, DateTimeOffset.MaxValue);
            }

            return requirementType;
        }

        private ICollection<int> GetPriorities (Policy policy)
        {
            if(policy.PoliciesPriorityGroups != null && policy.PoliciesPriorityGroups.Any())
            {
                logger.LogTrace("Getting priorities from Policy's PriorityGroups");
                return policy.PoliciesPriorityGroups.Select(x => x.PriorityGroup.Priority).Distinct().ToArray();
            }
            else
            {
                logger.LogTrace("No priorities defined");
                return new[] { 0 };
            }
        }
    }
}
