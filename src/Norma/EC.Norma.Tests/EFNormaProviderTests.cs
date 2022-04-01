using System.Collections.Generic;
using System.Linq;
using EC.Norma.Core;
using EC.Norma.EF.Providers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using NormaPolicyProvider = EC.Norma.Core.NormaPolicyProvider;

namespace EC.Norma.Tests
{
    [Collection("TestServer collection")]
    public class EFNormaProviderTests
    {
        private readonly NormaTestsFixture<Startup> fixture;

        public EFNormaProviderTests(NormaTestsFixture<Startup> fixture)
        {
            this.fixture = fixture;
        }
     

        [Fact]
        public void GetGetPermissions_WithName_ReturnsPermissionForApplication()
        {            
            EFNormaProvider provider = (EFNormaProvider)fixture.WebAppFactory.Services.GetService<INormaProvider>();

            var permissions = provider.GetPermissions($"{nameof(TestController.PlainAction)}-{TestController.Name}");

            permissions.Count.Should().Be(1);         

        }

        [Fact]
        public void GetGetPermissions_WithActionAndResource_ReturnsPermissionForApplication()
        {
            EFNormaProvider provider = (EFNormaProvider)fixture.WebAppFactory.Services.GetService<INormaProvider>();

            var permissions = provider.GetPermissions(nameof(TestController.PlainAction), TestController.Name);

            permissions.Count.Should().Be(1);
        }

        [Fact]
        public void GetPoliciesForPermission_WithName_ReturnsPermissionForApplication()
        {
            EFNormaProvider provider = (EFNormaProvider)fixture.WebAppFactory.Services.GetService<INormaProvider>();

            var permissions = provider.GetPoliciesForPermission($"{nameof(TestController.PlainAction)}-{TestController.Name}");

            permissions.Count.Should().Be(1);

        }

        [Fact]
        public void GetPoliciesForActionResource_WithActionAndResource_ReturnsPermissionForApplication()
        {
            EFNormaProvider provider = (EFNormaProvider)fixture.WebAppFactory.Services.GetService<INormaProvider>();

            var permissions = provider.GetPoliciesForActionResource(nameof(TestController.PlainAction), TestController.Name);

            permissions.Count.Should().Be(1);
        }


        [Fact]
        public async void GetAssignmentsForRoles_WithPermissionsAndProfiles_ReturnsAssignmentsForApplication()
        {
            var policyProvider = (NormaPolicyProvider)fixture.WebAppFactory.Services.GetService<IAuthorizationPolicyProvider>();

            var policy = await policyProvider.GetPolicyAsync($"{nameof(TestController.PlainAction)}|{TestController.Name}");
            var requirement = policy.Requirements.First();

            var permission = ((HasPermissionRequirement)requirement).Permission;
            
            EFNormaProvider provider = (EFNormaProvider)fixture.WebAppFactory.Services.GetService<INormaProvider>();

            IEnumerable<string> profiles = new List<string> { "User" };
            var assignments = provider.GetAssignmentsForRoles(permission, profiles);

            assignments.Count.Should().Be(1);
        }

    }
}