using EC.Norma.Core;
using EC.Norma.Metadata;
using EC.Norma.Options;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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

        private Mock<NormaEngine> cut;
        protected IAuthorizationPolicyProvider mPolicyProvider;
        protected IAuthorizationService mAuthorizationService;
        protected INormaProvider mProvider;
        protected IOptionsMonitor<NormaOptions> mNormaOptions;
        protected ILogger mLogger;

        private readonly NormaTestsFixture<Startup> fixture;

        public NormaEngineTest(NormaTestsFixture<Startup> fixture) 
        {
            this.fixture = fixture;

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

            cut = new Mock<NormaEngine>(mPolicyProvider, mAuthorizationService, mProvider, mNormaOptions, mLogger) {CallBase = true};
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

            var logEntry = ((NoOpLogger)mLogger).LogEvents.FirstOrDefault();
            logEntry.Should().NotBeNull();
            logEntry.Level.Should().Be(LogLevel.Trace);
            logEntry.Message.Should().StartWith("Beginning");
        }

        [Fact]
        public async void EvalPermissions_NoEndpoint_ThrowsException()
        {
            HttpContext context = Mock.Of<HttpContext>();

            cut.Protected().Setup<Endpoint>("GetEndpoint", context).Returns(null as Endpoint);

            var exception = await Record.ExceptionAsync(() => cut.Object.EvalPermissions(context));

            exception.Should().BeOfType<NullReferenceException>();
        }

        [Fact]
        public async void EvalPermissions_ByPassedIsTrue_LogByPass()
        {
            Endpoint endpoint = CreateEndpoint(new ByPassNormaAttribute());

            HttpContext context = GetHttpContext(endpoint: endpoint);

            await cut.Object.EvalPermissions(context);

            var logEntry = ((NoOpLogger)mLogger).LogEvents.LastOrDefault();
            logEntry.Should().NotBeNull();
            logEntry.Level.Should().Be(LogLevel.Trace);
            logEntry.Message.Should().BeEquivalentTo("ByPassed by attribute");
        }

        [Fact]
        public async void EvalPermissions_ByPassedIsTrue_ReturnsNull()
        {
            Endpoint endpoint = CreateEndpoint(new ByPassNormaAttribute());

            HttpContext context = GetHttpContext(endpoint: endpoint);

            var result = await cut.Object.EvalPermissions(context);

            result.Should().BeNull();
        }

        // From here ByPassed is always false (BpF)

        [Fact]
        public async void EvalPermissions_BpF_Always_CallsGetPermission()
        {
            Endpoint endpoint = CreateEndpoint();
            HttpContext context = GetHttpContext(endpoint: endpoint);

            await cut.Object.EvalPermissions(context);

            cut.Protected().Verify("GetPermissions", Times.Once(), true, endpoint);
        }

        [Fact]
        public async void EvalPermissions_BpF_Always_CallsGetActions()
        {
            Endpoint endpoint = CreateEndpoint();
            HttpContext context = GetHttpContext(endpoint: endpoint);

            await cut.Object.EvalPermissions(context);

            cut.Protected().Verify("GetActions", Times.Once(), true, endpoint);
        }

        [Fact]
        public async void EvalPermissions_BpF_Always_CallsGetResource()
        {
            Endpoint endpoint = CreateEndpoint();
            HttpContext context = GetHttpContext(endpoint: endpoint);

            await cut.Object.EvalPermissions(context);

            cut.Protected().Verify("GetResource", Times.Once(), true, endpoint);
        }

        [Fact]
        public async void EvalPermissions_BpF_Always_CallsGetCombinedPolicy()
        {
            Endpoint endpoint = CreateEndpoint();
            HttpContext context = GetHttpContext(endpoint: endpoint);

            await cut.Object.EvalPermissions(context);

            cut.Protected().Verify("GetCombinedPolicyAsync", Times.Once(), ItExpr.IsAny<IEnumerable<string>>(), ItExpr.IsNull<string>(), ItExpr.IsAny<IEnumerable<string>>());
        }


        [Fact]
        public async void EvalPermissions_CombinedPolicyIsNull_LogNoPermissionsFound()
        {
            Endpoint endpoint = CreateEndpoint();
            HttpContext context = GetHttpContext(endpoint: endpoint);

            cut.Protected()
                .Setup<Task<AuthorizationPolicy>>("GetCombinedPolicyAsync"
                                                        , ItExpr.IsAny<IEnumerable<string>>()
                                                        , ItExpr.IsNull<string>()
                                                        , ItExpr.IsAny<IEnumerable<string>>())
                .Returns(Task.FromResult(null as AuthorizationPolicy));


            await cut.Object.EvalPermissions(context);

            var logEntry = ((NoOpLogger)mLogger).LogEvents.LastOrDefault();
            logEntry.Should().NotBeNull();
            logEntry.Level.Should().Be(LogLevel.Trace);
            logEntry.Message.Should().StartWithEquivalentOf("No Permissions found");
        }


        [Fact]
        public async void EvalPermissions_CombinedPolicyIsNullAndNoPermissionActionIsSuccess_ReturnsSuccess()
        {
            Endpoint endpoint = CreateEndpoint();
            HttpContext context = GetHttpContext(endpoint: endpoint);

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

            Endpoint endpoint = CreateEndpoint();
            HttpContext context = GetHttpContext(endpoint: endpoint);

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

            Endpoint endpoint = CreateEndpoint();
            HttpContext context = GetHttpContext(endpoint: endpoint);

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
            
            // Act
            await cut.Object.EvalPermissions(context);

            // Assert
            authService.Verify(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, It.IsAny<IEnumerable<IAuthorizationRequirement>>()), Times.Once);
        }


        [Fact]
        public async void EvalPermissions_CombinedPolicyIsNotNullAndAuthorizeAsyncSuccess_ReturnsSuccess()
        {
            Endpoint endpoint = CreateEndpoint();
            HttpContext context = GetHttpContext(endpoint: endpoint);

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
        public async void EvalPermissions_CombinedPolicyIsNotNullAndAuthorizeAsyncFails_ReturnsFailure()
        {
            Endpoint endpoint = CreateEndpoint();
            HttpContext context = GetHttpContext(endpoint: endpoint);

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


        private AuthorizationMiddleware CreateMiddleware(RequestDelegate requestDelegate = null, IAuthorizationPolicyProvider policyProvider = null)
        {
            requestDelegate ??= context => Task.CompletedTask;

            return new AuthorizationMiddleware(requestDelegate, policyProvider);
        }

        private Endpoint CreateEndpoint(params object[] metadata)
        {
            return new Endpoint(context => Task.CompletedTask, new EndpointMetadataCollection(metadata), "Test endpoint");
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

            // ServiceProvider
            var serviceCollection = new ServiceCollection();

            authenticationService ??= Mock.Of<IAuthenticationService>();

            serviceCollection.AddSingleton(authenticationService);
            serviceCollection.AddOptions();
            serviceCollection.AddLogging();
            serviceCollection.AddAuthorization();
            registerServices?.Invoke(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var httpContext = new DefaultHttpContext();
            if (endpoint != null)
            {
                httpContext.SetEndpoint(endpoint);
            }
            httpContext.RequestServices = serviceProvider;
            if (!anonymous)
            {
                httpContext.User = validUser;
            }

            return httpContext;
        }

        private class TestRequestDelegate
        {
            private readonly int statusCode;

            public bool Called => CalledCount > 0;
            
            public int CalledCount { get; private set; }

            public TestRequestDelegate(int statusCode = 200)
            {
                this.statusCode = statusCode;
            }

            public Task Invoke(HttpContext context)
            {
                CalledCount++;
                context.Response.StatusCode = statusCode;
                return Task.CompletedTask;
            }
        }
    }
}
