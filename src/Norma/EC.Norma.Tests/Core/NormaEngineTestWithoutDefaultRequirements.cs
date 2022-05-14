using EC.Norma.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EC.Norma.TestUtils;
using Microsoft.AspNetCore.Mvc.Controllers;
using Xunit;

namespace EC.Norma.Tests.Core
{
    [Collection("TestServer collection Without Default Requirements")]
    public class NormaEngineTestWithoutDefaultRequirements
    {
        private readonly NormaTestsFixtureWithoutDefaultRequirement<Startup> fixtureWithoutDefaultRequirement;

        public NormaEngineTestWithoutDefaultRequirements(NormaTestsFixtureWithoutDefaultRequirement<Startup> fixtureWithoutDefaultRequirement)
        {
            this.fixtureWithoutDefaultRequirement = fixtureWithoutDefaultRequirement;
        }

        [Fact]
        public async Task EvalPermissions_ActionWithoutPermissionsAndWithoutDefaultRequirements_ReturnSuccess()
        {
            var endpoint = CreateEndpoint(new ControllerActionDescriptor {ActionName = nameof(TestController.WithoutPermissions), ControllerName = TestController.Name});
            var context = GetHttpContext(endpoint: endpoint);

            var normaEngine = fixtureWithoutDefaultRequirement.WebAppFactory.Services.GetRequiredService<NormaEngine>();
            var result = await normaEngine.EvalPermissions(context);

            result.Succeeded.Should().BeTrue();
            NoOpLogger.Instance.LogEvents.LastOrDefault(l => l.Level == LogLevel.Warning
                                                             && l.Message.Equals(
                                                                 "No Permissions found, so result is defined by NoPermissionAction in Options (better define some permissions if you don't want this default behavior)",
                                                                 StringComparison.OrdinalIgnoreCase)).Should().NotBeNull();
        }

        private Endpoint CreateEndpoint(params object[] metadata)
        {
            return new Endpoint(_ => Task.CompletedTask, new EndpointMetadataCollection(metadata), "Test endpoint");
        }

        private HttpContext GetHttpContext(bool anonymous = false, Action<IServiceCollection> registerServices = null, Endpoint endpoint = null, IAuthenticationService authenticationService = null)
        {
            var basicPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] {
                        new Claim("Permission", "CanViewPage"),
                        new Claim(ClaimTypes.Role, "Administrator"),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim(ClaimTypes.NameIdentifier, "John")},
                        "Basic"));

            var validUser = basicPrincipal;

            var bearerIdentity = new ClaimsIdentity(
                    new[] {
                        new Claim("Permission", "CupBearer"),
                        new Claim(ClaimTypes.Role, "Token"),
                        new Claim(ClaimTypes.NameIdentifier, "John Bear")},
                        "Bearer");

            validUser.AddIdentity(bearerIdentity);

            var httpContext = new DefaultHttpContext();
            if (endpoint != null)
            {
                httpContext.SetEndpoint(endpoint);
            }
            httpContext.RequestServices = fixtureWithoutDefaultRequirement.WebAppFactory.Services;
            if (!anonymous)
            {
                httpContext.User = validUser;
            }

            return httpContext;
        }
    }
}
