using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using NormaPolicyProvider = EC.Norma.Core.NormaPolicyProvider;

namespace EC.Norma.Options.Tests
{
    [Collection("TestServer collection")]
    public class NormaPolicyProviderTests
    {
        private readonly NormaTestsFixture<Startup> fixture;

        public NormaPolicyProviderTests(NormaTestsFixture<Startup> fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async void GetPolicyAsync_WithoutPermissionsAndOptionLogOnly_NoExceptionThrown()
        {
            var policyProvider = (NormaPolicyProvider)fixture.WebAppFactory.Services.GetService<IAuthorizationPolicyProvider>();
            
            var policy = await Record.ExceptionAsync(() => policyProvider.GetPolicyAsync($"{nameof(TestController.WithoutRequirement)}|{TestController.Name}"));

            policy.Should().BeNull();
        }

        [Fact]
        public async void GetPolicyAsync_WithoutConfiguredRequirementAndOptionLogOnly_NoExceptionThrown()
        {
            var policyProvider = (NormaPolicyProvider)fixture.WebAppFactory.Services.GetService<IAuthorizationPolicyProvider>();
            var policy = await Record.ExceptionAsync(() => policyProvider.GetPolicyAsync($"{nameof(TestController.WithoutConfiguredRequirement)}|{TestController.Name}"));

            policy.Should().BeNull();

        }
    }
}