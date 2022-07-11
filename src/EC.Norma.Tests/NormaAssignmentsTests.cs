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
    [Collection("TestServer collection With Date Ranged Assignments")]
    public class NormaAssignmentsTests
    {
        private readonly NormaTestsFixtureWithDateRangedAssignments<Startup> fixtureWithDateRangedAssignments;

        public NormaAssignmentsTests(NormaTestsFixtureWithDateRangedAssignments<Startup> fixtureWithDateRangedAssignments)
        {
            this.fixtureWithDateRangedAssignments = fixtureWithDateRangedAssignments;
        }

        [Fact]
        public async void GetAssignmentsForRoles_WithDateRangedAssignmentForApplication1_ReturnsAssignmentsForApplication()
        {
            var policyProvider = (NormaPolicyProvider)fixtureWithDateRangedAssignments.WebAppFactory.Services.GetService<IAuthorizationPolicyProvider>();

            var policy = await policyProvider.GetPolicyAsync($"{nameof(TestController.PlainAction)}|{TestController.Name}");
            var requirement = policy.Requirements.First();

            var permission = ((HasPermissionRequirement)requirement).Permission;

            EFNormaProvider provider = (EFNormaProvider)fixtureWithDateRangedAssignments.WebAppFactory.Services.GetService<INormaProvider>();

            IEnumerable<string> profiles = new List<string> { "With Range User", "Outdated Range Admin" };
            var assignments = provider.GetAssignmentsForRoles(permission, profiles);

            assignments.Count.Should().Be(1);
            assignments.FirstOrDefault().Permission.Name.Should().Be("PlainAction-Test");
            assignments.FirstOrDefault().Permission.Action.Name.Should().Be(nameof(TestController.PlainAction));
            assignments.FirstOrDefault().Permission.Resource.Name.Should().Be(TestController.Name);
            assignments.FirstOrDefault().Profile.Name.Should().Be("With Range User");
        }

        [Fact]
        public async void GetAssignmentsForRoles_NoDateRangedAssignmentForApplication1_ReturnsAssignmentsForApplication()
        {
            var policyProvider = (NormaPolicyProvider)fixtureWithDateRangedAssignments.WebAppFactory.Services.GetService<IAuthorizationPolicyProvider>();

            var policy = await policyProvider.GetPolicyAsync($"{nameof(TestController.PlainAction)}|{TestController.Name}");
            var requirement = policy.Requirements.First();

            var permission = ((HasPermissionRequirement)requirement).Permission;

            EFNormaProvider provider = (EFNormaProvider)fixtureWithDateRangedAssignments.WebAppFactory.Services.GetService<INormaProvider>();

            IEnumerable<string> profiles = new List<string> { "Without Range User" };
            var assignments = provider.GetAssignmentsForRoles(permission, profiles);

            assignments.Count.Should().Be(1);
            assignments.FirstOrDefault().Permission.Name.Should().Be("PlainAction-Test");
            assignments.FirstOrDefault().Permission.Action.Name.Should().Be(nameof(TestController.PlainAction));
            assignments.FirstOrDefault().Permission.Resource.Name.Should().Be(TestController.Name);
            assignments.FirstOrDefault().Profile.Name.Should().Be("Without Range User");
        }

    }
}
