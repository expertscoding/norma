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

            var requirements = GetNormaRequirements(action, resource).ToArray();
            var policyBuilder = new AuthorizationPolicyBuilder();

            logger.LogTrace("Got policies: [{policies} ]", requirements.Select(p => p.Name).Aggregate("", (s, p) => string.Concat(s, " ", p)));

            if (!requirements.Any())
            {
                logger.LogTrace("No Norma requirement found, requesting fallback provider");

                return FallbackPolicyProvider.GetPolicyAsync(policyName);
            }


            foreach (var requirement in requirements)
            {
                try
                {
                    var requirementName = SPACENAME + requirement.Name + REQ_SUFFIX;

                    logger.LogTrace("Getting requirement {requirement}", requirementName);

                    var type = GetRequirementType(requirementName);

                    logger.LogTrace("Requirement acquired. Getting Permissions.");

                    var cacheKey = $"{CacheKeys.NormaPermissions}|{action}|{resource ?? ""}";

                    var permissions = cache.Get<ICollection<Permission>>(cacheKey);
                    if (permissions == null)
                    {
                        permissions = string.IsNullOrWhiteSpace(resource) ? provider.GetPermissions(action) : provider.GetPermissions(action, resource);
                        cache.Set(cacheKey, permissions, DateTime.Now.AddSeconds(normaOptions.CacheExpiration));
                    }

                    foreach (var priority in GetPriorities(requirement))
                    {
                        if (services.GetService(type) is NormaRequirement normaRequirement)
                        {
                            if (permissions?.Any() == false && !requirement.IsDefault)
                                continue;

                            normaRequirement.Action = action;
                            normaRequirement.Resource = resource;
                            normaRequirement.Permission = requirement.IsDefault ? $"DefaultRequirement for {resource}/{action}" : permissions.First().Name;
                            normaRequirement.Priority = priority;
                            normaRequirement.IsDefault = requirement.IsDefault;

                            policyBuilder.AddRequirements(normaRequirement);

                            logger.LogTrace("Requirement Added (Action: {action}, Resource: {resource}, Permission: {permission}, Priority: {priority})", action, resource, normaRequirement.Permission, priority);
                        }
                        else
                        {
                            throw new Exception("No requirement located.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("No requirement located for requirement {requirement}.", requirement.Name);
                    if (normaOptions.MissingRequirementAction == MissingRequirementBehaviour.ThrowException)
                    {
                        throw new TypeLoadException($"NormaRequirement for {requirement} not found", ex);
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

        private IEnumerable<Requirement> GetNormaRequirements(string action, string resource)
        {
            logger.LogTrace("Getting Norma Requirements");
            var cacheKey = $"{CacheKeys.NormaRequirements}|{action}|{resource ?? ""}";

            var policies = cache.Get<ICollection<Requirement>>(cacheKey);
            if (policies == null)
            {
                policies = string.IsNullOrWhiteSpace(resource)
                    ? provider.GetRequirementsForPermission(action)
                    : provider.GetRequirementsForActionResource(action, resource).Union(provider.GetDefaultRequirements().Select(x => { x.IsDefault = true; return x; })).ToArray();

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

        private ICollection<int> GetPriorities (Requirement requirement)
        {
            if(!requirement.IsDefault && requirement.RequirementsPriorityGroups != null && requirement.RequirementsPriorityGroups.Any())
            {
                logger.LogTrace("Getting priorities from Requirement's PriorityGroups");
                return requirement.RequirementsPriorityGroups.Select(x => x.PriorityGroup.Priority).Distinct().ToArray();
            }
            else
            {
                logger.LogTrace("No priorities defined");
                return new[] { requirement.RequirementsApplications != null && requirement.RequirementsApplications.Any(x => x.IsDefault) ? int.MaxValue : 0 };
            }
        }
    }
}
