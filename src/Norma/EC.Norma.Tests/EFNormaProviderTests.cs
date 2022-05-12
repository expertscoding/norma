using EC.Norma.Core;
using EC.Norma.EF.Providers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using NormaPolicyProvider = EC.Norma.Core.NormaPolicyProvider;

namespace EC.Norma.Tests
{
    [Collection("TestServer collection")]
    public class EFNormaProviderTests
    {
        private readonly NormaTestsFixtureWithDefaultRequirement<Startup> fixtureWithDefaultRequirement;

        public EFNormaProviderTests(NormaTestsFixtureWithDefaultRequirement<Startup> fixtureWithDefaultRequirement)
        {
            this.fixtureWithDefaultRequirement = fixtureWithDefaultRequirement;
        }

        [Fact]
        public void GetPermissions_WithNameForApplication1_ReturnsPermissionForApplication()
        {
            EFNormaProvider provider = (EFNormaProvider)fixtureWithDefaultRequirement.WebAppFactory.Services.GetService<INormaProvider>();

            var permissions = provider.GetPermissions($"{nameof(TestController.PlainAction)}-{TestController.Name}");

            permissions.Count.Should().Be(1);
            permissions.FirstOrDefault().Action.Name.Should().Be(nameof(TestController.PlainAction));
            permissions.FirstOrDefault().Resource.Name.Should().Be(TestController.Name);
            permissions.FirstOrDefault().Resource.Module.Application.Name.Should().Be("application1");
        }

        [Fact]
        public void GetPermissions_WithActionAndResourceForApplication1_ReturnsPermissionForApplication()
        {
            EFNormaProvider provider = (EFNormaProvider)fixtureWithDefaultRequirement.WebAppFactory.Services.GetService<INormaProvider>();

            var permissions = provider.GetPermissions(nameof(TestController.PlainAction), TestController.Name);

            permissions.Count.Should().Be(1);
            permissions.FirstOrDefault().Action.Name.Should().Be(nameof(TestController.PlainAction));
            permissions.FirstOrDefault().Resource.Name.Should().Be(TestController.Name);
            permissions.FirstOrDefault().Resource.Module.Application.Name.Should().Be("application1");
        }

        [Fact]
        public void GetRequirementsForPermission_WithNameForApplication1_ReturnsPermissionForApplication()
        {
            EFNormaProvider provider = (EFNormaProvider)fixtureWithDefaultRequirement.WebAppFactory.Services.GetService<INormaProvider>();

            var policies = provider.GetRequirementsForPermission($"{nameof(TestController.PlainAction)}-{TestController.Name}");

            policies.Count.Should().Be(1);
            policies.FirstOrDefault().Name.Should().Be("HasPermission");
        }

        [Fact]
        public void GetRequirementsForActionResource_WithActionAndResourceForApplication1_ReturnsPermissionForApplication()
        {
            EFNormaProvider provider = (EFNormaProvider)fixtureWithDefaultRequirement.WebAppFactory.Services.GetService<INormaProvider>();

            var policies = provider.GetRequirementsForActionResource(nameof(TestController.PlainAction), TestController.Name);

            policies.Count.Should().Be(1);
            policies.FirstOrDefault().Name.Should().Be("HasPermission");
        }


        [Fact]
        public async void GetAssignmentsForRoles_WithPermissionsAndProfilesForApplication1_ReturnsAssignmentsForApplication()
        {
            var policyProvider = (NormaPolicyProvider)fixtureWithDefaultRequirement.WebAppFactory.Services.GetService<IAuthorizationPolicyProvider>();

            var policy = await policyProvider.GetPolicyAsync($"{nameof(TestController.PlainAction)}|{TestController.Name}");
            var requirement = policy.Requirements.First();

            var permission = ((HasPermissionRequirement)requirement).Permission;

            EFNormaProvider provider = (EFNormaProvider)fixtureWithDefaultRequirement.WebAppFactory.Services.GetService<INormaProvider>();

            IEnumerable<string> profiles = new List<string> { "User" };
            var assignments = provider.GetAssignmentsForRoles(permission, profiles);

            assignments.Count.Should().Be(1);
            assignments.FirstOrDefault().Permission.Name.Should().Be("PlainAction-Test");
            assignments.FirstOrDefault().Permission.Action.Name.Should().Be(nameof(TestController.PlainAction));
            assignments.FirstOrDefault().Permission.Resource.Name.Should().Be(TestController.Name);
            assignments.FirstOrDefault().Profile.Name.Should().Be("User");
        }

    }
}
