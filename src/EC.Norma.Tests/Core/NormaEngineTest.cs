using EC.Norma.Core;
using EC.Norma.Metadata;
using EC.Norma.Options;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EC.Norma.TestUtils;
using Xunit;

namespace EC.Norma.Tests.Core
{
    [Collection("TestServer collection")]
    public class NormaEngineTest
    {
        private readonly NormaTestsFixtureWithDefaultRequirement<Startup> fixtureWithDefaultRequirement;

        private Mock<NormaEngine> cut;
        protected IAuthorizationPolicyProvider mPolicyProvider;
        protected IAuthorizationService mAuthorizationService;
        protected INormaProvider mProvider;
        protected IOptionsMonitor<NormaOptions> mNormaOptions;
        protected ILogger mLogger;

        public NormaEngineTest(NormaTestsFixtureWithDefaultRequirement<Startup> fixtureWithDefaultRequirement)
        {
            this.fixtureWithDefaultRequirement = fixtureWithDefaultRequirement;
            CreateCUT();
        }

        private void CreateCUT(bool noPermissionActionSuccess = true)
        {
            var normaOptions = new NormaOptions
            {
                NoPermissionAction = (noPermissionActionSuccess ? NoPermissionsBehaviour.Success : NoPermissionsBehaviour.Failure)
            };

            mPolicyProvider = Mock.Of<IAuthorizationPolicyProvider>();
            mAuthorizationService = Mock.Of<IAuthorizationService>();
            mProvider = Mock.Of<INormaProvider>();
            mNormaOptions = Mock.Of<IOptionsMonitor<NormaOptions>>(opt => opt.CurrentValue == normaOptions);
            mLogger = NoOpLoggerFactory.Instance.CreateLogger("");

            cut = new Mock<NormaEngine>(mPolicyProvider, mAuthorizationService, mProvider, mNormaOptions, mLogger) { CallBase = true };
        }

        [Fact]
        public async void EvalPermissions_ArgumentContextNull_ThrowsException()
        {
            var exception = await Record.ExceptionAsync(() => cut.Object.EvalPermissions(null));

            exception.Should().BeOfType<ArgumentNullException>();
        }


        [Fact]
        public async void EvalPermissions_Always_LogBeginningProcess()
        {
            await Record.ExceptionAsync(() => cut.Object.EvalPermissions(null));

            var logEntry = ((NoOpLogger)mLogger).LogEvents.LastOrDefault(l => l.Message.Equals("Beginning NormaEngine evaluation:", StringComparison.OrdinalIgnoreCase));
            logEntry.Should().NotBeNull();
            logEntry.Level.Should().Be(LogLevel.Information);
        }

        [Fact]
        public async void EvalPermissions_NoEndpoint_ThrowsException()
        {
            var context = Mock.Of<HttpContext>();

            cut.Protected().Setup<Endpoint>("GetEndpoint", context).Returns(null as Endpoint);

            var exception = await Record.ExceptionAsync(() => cut.Object.EvalPermissions(context));

            exception.Should().BeOfType<NullReferenceException>();
        }

        [Fact]
        public async void EvalPermissions_ByPassedIsTrue_LogByPass()
        {
            var endpoint = CreateEndpoint(new ByPassNormaAttribute());
            var context = GetHttpContext(endpoint: endpoint);

            await cut.Object.EvalPermissions(context);

            var logEntry = ((NoOpLogger)mLogger).LogEvents.LastOrDefault();
            logEntry.Should().NotBeNull();
            logEntry.Level.Should().Be(LogLevel.Trace);
            logEntry.Message.Should().BeEquivalentTo("ByPassed by attribute");
        }

        [Fact]
        public async void EvalPermissions_ByPassedIsTrue_ReturnsNull()
        {
            var endpoint = CreateEndpoint(new ByPassNormaAttribute());
            var context = GetHttpContext(endpoint: endpoint);

            var result = await cut.Object.EvalPermissions(context);

            result.Should().BeNull();
        }

        // From here ByPassed is always false (BpF)

        [Fact]
        public async void EvalPermissions_BpF_Always_CallsGetPermission()
        {
            var endpoint = CreateEndpoint();
            var context = GetHttpContext(endpoint: endpoint);

            await cut.Object.EvalPermissions(context);

            cut.Protected().Verify("GetPermissions", Times.Once(), true, endpoint);
        }

        [Fact]
        public async void EvalPermissions_BpF_Always_CallsGetActions()
        {
            var endpoint = CreateEndpoint();
            var context = GetHttpContext(endpoint: endpoint);

            await cut.Object.EvalPermissions(context);

            cut.Protected().Verify("GetActions", Times.Once(), true, endpoint);
        }

        [Fact]
        public async void EvalPermissions_BpF_Always_CallsGetResource()
        {
            var endpoint = CreateEndpoint();
            var context = GetHttpContext(endpoint: endpoint);

            await cut.Object.EvalPermissions(context);

            cut.Protected().Verify("GetResource", Times.Once(), true, endpoint);
        }

        [Fact]
        public async void EvalPermissions_BpF_Always_CallsGetCombinedPolicy()
        {
            var endpoint = CreateEndpoint();
            var context = GetHttpContext(endpoint: endpoint);

            await cut.Object.EvalPermissions(context);

            cut.Protected().Verify("GetCombinedPolicyAsync", Times.Once(), ItExpr.IsAny<IEnumerable<string>>(), ItExpr.IsNull<string>(), ItExpr.IsAny<IEnumerable<string>>());
        }


        [Fact]
        public async void EvalPermissions_CombinedPolicyIsNull_LogNoPermissionsFound()
        {
            var endpoint = CreateEndpoint();
            var context = GetHttpContext(endpoint: endpoint);

            cut.Protected()
                .Setup<Task<AuthorizationPolicy>>("GetCombinedPolicyAsync"
                                                        , ItExpr.IsAny<IEnumerable<string>>()
                                                        , ItExpr.IsNull<string>()
                                                        , ItExpr.IsAny<IEnumerable<string>>())
                .Returns(Task.FromResult(null as AuthorizationPolicy));


            await cut.Object.EvalPermissions(context);

            var logEntry = ((NoOpLogger)mLogger).LogEvents.LastOrDefault();
            logEntry.Should().NotBeNull();
            logEntry.Level.Should().Be(LogLevel.Warning);
            logEntry.Message.Should().StartWithEquivalentOf("No Permissions found");
        }


        [Fact]
        public async void EvalPermissions_CombinedPolicyIsNullAndNoPermissionActionIsSuccess_ReturnsSuccess()
        {
            var endpoint = CreateEndpoint();
            var context = GetHttpContext(endpoint: endpoint);

            cut.Protected()
                .Setup<Task<AuthorizationPolicy>>("GetCombinedPolicyAsync"
                                                        , ItExpr.IsAny<IEnumerable<string>>()
                                                        , ItExpr.IsNull<string>()
                                                        , ItExpr.IsAny<IEnumerable<string>>())
                .Returns(Task.FromResult(null as AuthorizationPolicy));

            var result = await cut.Object.EvalPermissions(context);

            result.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async void EvalPermissions_CombinedPolicyIsNullAndNoPermissionActionIsFailure_ReturnsFailure()
        {
            CreateCUT(false);

            var endpoint = CreateEndpoint();
            var context = GetHttpContext(endpoint: endpoint);

            cut.Protected()
                .Setup<Task<AuthorizationPolicy>>("GetCombinedPolicyAsync"
                                                        , ItExpr.IsAny<IEnumerable<string>>()
                                                        , ItExpr.IsNull<string>()
                                                        , ItExpr.IsAny<IEnumerable<string>>())
                .Returns(Task.FromResult(null as AuthorizationPolicy));

            var result = await cut.Object.EvalPermissions(context);

            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async void EvalPermissions_CombinedPolicyIsNotNull_CallsAuthorizeAsyncWithPolicy()
        {
            // Arrange
            CreateCUT(false);

            var endpoint = CreateEndpoint();
            var context = GetHttpContext(endpoint: endpoint);

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new HasPermissionRequirement())
                .AddRequirements(new IsAdminRequirement())
                .Build();


            cut.Protected()
                .Setup<Task<AuthorizationPolicy>>("GetCombinedPolicyAsync"
                                                        , ItExpr.IsAny<IEnumerable<string>>()
                                                        , ItExpr.IsNull<string>()
                                                        , ItExpr.IsAny<IEnumerable<string>>())
                .Returns(Task.FromResult(policy));

            var authService = Mock.Get(mAuthorizationService);

            // Act
            await cut.Object.EvalPermissions(context);

            // Assert
            authService.Verify(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, It.IsAny<IEnumerable<IAuthorizationRequirement>>()), Times.Once);
        }


        [Fact]
        public async void EvalPermissions_CombinedPolicyIsNotNullAndAuthorizeAsyncSuccess_ReturnsSuccess()
        {
            var endpoint = CreateEndpoint();
            var context = GetHttpContext(endpoint: endpoint);

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new HasPermissionRequirement())
                .Build();


            cut.Protected()
                .Setup<Task<AuthorizationPolicy>>("GetCombinedPolicyAsync"
                                                        , ItExpr.IsAny<IEnumerable<string>>()
                                                        , ItExpr.IsNull<string>()
                                                        , ItExpr.IsAny<IEnumerable<string>>())
                .Returns(Task.FromResult(policy));

            var authService = Mock.Get(mAuthorizationService);
            authService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .Returns(Task.FromResult(AuthorizationResult.Success()));

            // Act
            var result = await cut.Object.EvalPermissions(context);

            // Assert
            result.Succeeded.Should().BeTrue();
        }


        [Fact]
        public async void EvalPermissions_CombinedPolicyWithTwoPriorities_MIN_PriorityRequirement_ReturnsSuccess()
        {
            var endpoint = CreateEndpoint();
            var context = GetHttpContext(endpoint: endpoint);

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new HasPermissionRequirement())
                .Build();

            cut.Protected()
                .Setup<Task<AuthorizationPolicy>>("GetCombinedPolicyAsync"
                                                        , ItExpr.IsAny<IEnumerable<string>>()
                                                        , ItExpr.IsNull<string>()
                                                        , ItExpr.IsAny<IEnumerable<string>>())
                .Returns(Task.FromResult(policy));

            var authService = Mock.Get(mAuthorizationService);
            authService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .Returns(Task.FromResult(AuthorizationResult.Failed()));

            // Act
            var result = await cut.Object.EvalPermissions(context);

            // Assert
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async void EvalPermissions_CombinedPolicyWithTwoPriorities_MAX_PriorityRequirement_ReturnsSuccess()
        {
            // Arrange
            CreateCUT(false);

            var endpoint = CreateEndpoint(new NormaPermissionAttribute("TwoRequirementsAction-Test"));
            var context = GetHttpContext(endpoint: endpoint);

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new HasPermissionRequirement() { Priority = 2 })
                .AddRequirements(new IsAdminRequirement() { Priority = 1 })
                .Build();

            cut.Protected()
                .Setup<Task<AuthorizationPolicy>>("GetCombinedPolicyAsync"
                                                        , ItExpr.IsAny<IEnumerable<string>>()
                                                        , ItExpr.IsNull<string>()
                                                        , ItExpr.IsAny<IEnumerable<string>>())
                .Returns(Task.FromResult(policy));

            var authService = Mock.Get(mAuthorizationService);
            authService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .Returns(Task.FromResult(AuthorizationResult.Success()));

            // Act
            var result = await cut.Object.EvalPermissions(context);

            // Act
            result.Succeeded.Should().BeTrue();

            // Assert
            authService.Verify(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, It.IsAny<IEnumerable<IAuthorizationRequirement>>()), Times.Once);
        }


        [Fact]
        public async void EvalPermissions_CombinedPolicyWithTwoPriorities_ReturnsSuccess2()
        {
            // Arrange
            CreateCUT(false);

            var endpoint = CreateEndpoint(new NormaPermissionAttribute("TwoRequirementsAction-Test"));
            var context = GetHttpContext(endpoint: endpoint);

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new HasPermissionRequirement() { Priority = 2 })
                .AddRequirements(new IsAdminRequirement() { Priority = 1 })
                .Build();

            cut.Protected()
                .Setup<Task<AuthorizationPolicy>>("GetCombinedPolicyAsync"
                                                        , ItExpr.IsAny<IEnumerable<string>>()
                                                        , ItExpr.IsNull<string>()
                                                        , ItExpr.IsAny<IEnumerable<string>>())
                .Returns(Task.FromResult(policy));

            var authService = Mock.Get(mAuthorizationService);

            authService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, It.Is<IEnumerable<IAuthorizationRequirement>>(x => x.Any(r => (r as NormaRequirement).Priority == 2))))
                .Returns(Task.FromResult(AuthorizationResult.Success()));

            authService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, It.Is<IEnumerable<IAuthorizationRequirement>>(x => x.Any(r => (r as NormaRequirement).Priority == 1))))
                .Returns(Task.FromResult(AuthorizationResult.Failed()));

            // Act
            var result = await cut.Object.EvalPermissions(context);

            // Act
            result.Succeeded.Should().BeTrue();

            // Assert
            authService.Verify(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, It.IsAny<IEnumerable<IAuthorizationRequirement>>()), Times.Exactly(2));
        }

        private Endpoint CreateEndpoint(params object[] metadata)
        {
            return new Endpoint(_ => Task.CompletedTask, new EndpointMetadataCollection(metadata), "Test endpoint");
        }

        private HttpContext GetHttpContext(bool anonymous = false, Endpoint endpoint = null)
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

            httpContext.RequestServices = fixtureWithDefaultRequirement.WebAppFactory.Services;

            if (!anonymous)
            {
                httpContext.User = validUser;
            }

            return httpContext;
        }
    }
}
