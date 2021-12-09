using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EC.Norma.Core;
using EC.Norma.Entities;
using EC.Norma.Options;
using EC.Norma.TestUtils;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using NormaPolicyProvider = EC.Norma.Core.NormaPolicyProvider;

namespace EC.Norma.Tests
{
    [Collection("TestServer collection")]
    public class NormaPolicyProviderTests
    {
        private readonly NormaTestsFixture<Startup> fixture;
        protected IAuthorizationPolicyProvider mPolicyProvider;
        protected IOptionsMonitor<NormaOptions> mNormaOptions;
        public NormaPolicyProviderTests(NormaTestsFixture<Startup> fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async void GetPolicyAsync_WithData_ReturnsAPolicyWithRequirements()
        {
            var policyProvider = (NormaPolicyProvider)fixture.WebAppFactory.Services.GetService<IAuthorizationPolicyProvider>();
           
            var policy = await policyProvider.GetPolicyAsync($"{nameof(TestController.PlainAction)}|{TestController.Name}");

            policy.Should().NotBeNull();
            policy.Requirements.Count.Should().Be(1);
            var requirement = policy.Requirements.First();
            requirement.Should().BeOfType<HasPermissionRequirement>();
            ((HasPermissionRequirement)requirement).Action.Should().Be(nameof(TestController.PlainAction));
            ((HasPermissionRequirement)requirement).Resource.Should().Be(TestController.Name);
        }

        [Fact]
        public async void GetPolicyAsync_WithoutPermissionsAndOptionThrowException_ThrowsTypeException()
        {
            var policyProvider = (NormaPolicyProvider)fixture.WebAppFactory.Services.GetService<IAuthorizationPolicyProvider>();

            var exception = await Record.ExceptionAsync(() => policyProvider.GetPolicyAsync($"{nameof(TestController.WithoutRequirement)}|{TestController.Name}"));

            exception.Should().BeOfType<TypeLoadException>();
        }

        [Fact]
        public async void GetPolicyAsync_WithoutConfiguredRequirementAndOptionThrowException_ThrowsTypeException()
        {
            var policyProvider = (NormaPolicyProvider)fixture.WebAppFactory.Services.GetService<IAuthorizationPolicyProvider>();
            var exception = await Record.ExceptionAsync(() => policyProvider.GetPolicyAsync($"{nameof(TestController.WithoutConfiguredRequirement)}|{TestController.Name}"));

            exception.Should().BeOfType<TypeLoadException>();
        }

        [Fact]
        public async void GetPolicyAsync_WithData_HasPermissionCacheKeys()
        {   
            var policyProvider = (NormaPolicyProvider)fixture.WebAppFactory.Services.GetService<IAuthorizationPolicyProvider>();            
            var cacheService = (IMemoryCache)fixture.WebAppFactory.Services.GetService<IMemoryCache>();
            var policy = await policyProvider.GetPolicyAsync($"{nameof(TestController.PlainAction)}|{TestController.Name}");

            policy.Should().NotBeNull();

            string cacheKeyPermissions = $"{CacheKeys.NormaPermissions}|{nameof(TestController.PlainAction)}|{TestController.Name}";
            
            cacheService.Should().NotBeNull();

            cacheService.Get<ICollection<Permission>>(cacheKeyPermissions).Should().NotBeNull();
            cacheService.Get<ICollection<Permission>>(cacheKeyPermissions).Count().Should().Be(1);  
        }

        [Fact]
        public async void GetPolicyAsync_WithData_HasPolicyCacheKeys()
        {
            var policyProvider = (NormaPolicyProvider)fixture.WebAppFactory.Services.GetService<IAuthorizationPolicyProvider>();
            var cacheService = (IMemoryCache)fixture.WebAppFactory.Services.GetService<IMemoryCache>();
            var policy = await policyProvider.GetPolicyAsync($"{nameof(TestController.PlainAction)}|{TestController.Name}");

            policy.Should().NotBeNull();

            string cacheKeyPolicies = $"{CacheKeys.NormaPolicies}|{nameof(TestController.PlainAction)}|{TestController.Name}";

            cacheService.Should().NotBeNull();

            cacheService.Get<ICollection<Policy>>(cacheKeyPolicies).Should().NotBeNull();
            cacheService.Get<ICollection<Policy>>(cacheKeyPolicies).Count().Should().Be(1);

        }

        [Fact]
        public async void GetPolicyAsync_WithData_AfterTimeExpiredCleanCacheKeys()
        {
            var policyProvider = (NormaPolicyProvider)fixture.WebAppFactory.Services.GetService<IAuthorizationPolicyProvider>();

            var cacheService = (IMemoryCache)fixture.WebAppFactory.Services.GetService<IMemoryCache>();

            var policy = await policyProvider.GetPolicyAsync($"{nameof(TestController.PlainAction)}|{TestController.Name}");

            policy.Should().NotBeNull();

            string cacheKeyPermissions = $"{CacheKeys.NormaPermissions}|{nameof(TestController.PlainAction)}|{TestController.Name}";
            string cacheKeyPolicies = $"{CacheKeys.NormaPolicies}|{nameof(TestController.PlainAction)}|{TestController.Name}";

            //este tiempo se configura en el startUp del proyecto de test
            Thread.Sleep(10001);

            cacheService.Should().NotBeNull();

            cacheService.Get<ICollection<Policy>>(cacheKeyPermissions).Should().BeNull();
            cacheService.Get<ICollection<Policy>>(cacheKeyPolicies).Should().BeNull();
        }
    }
}