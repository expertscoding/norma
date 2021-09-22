using System;
using System.Linq;
using EC.Norma.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using NormaPolicyProvider = EC.Norma.Core.NormaPolicyProvider;

namespace EC.Norma.Tests
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
    }
}