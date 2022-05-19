using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EC.Norma.Core;
using EC.Norma.Filters;
using EC.Norma.Options;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace EC.Norma.Tests
{
    [Collection("TestServer collection Without Default Requirements")]
    public class NormaActionFilterTests
    {
        private readonly NormaTestsFixtureWithoutDefaultRequirement<Startup> fixture;

        public NormaActionFilterTests(NormaTestsFixtureWithoutDefaultRequirement<Startup> fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async void OnActionExecute_WithoutPermissionsDefinedAndNoPermissionActionSuccess_PassThroughToTheAction()
        {
            var services = fixture.WebAppFactory.Services;
            var normaOptions = new NormaOptions { NoPermissionAction = NoPermissionsBehaviour.Success };
            var monitor = Mock.Of<IOptionsMonitor<NormaOptions>>(opt => opt.CurrentValue == normaOptions);

            var action = new NormaActionFilter(services.GetService<IAuthorizationPolicyProvider>(), services.GetService<IAuthorizationService>(), services.GetService<INormaProvider>(), monitor, NullLogger<NormaActionFilter>.Instance);

            var actionDelegate = Mock.Of<ActionExecutionDelegate>();
            var context = GetMockActionExecutingContext(typeof(TestController), nameof(TestController.WithoutPermissions));

            await action.OnActionExecutionAsync(context, actionDelegate);

            Mock.Get(actionDelegate).Verify(a => a.Invoke());
        }

        private ActionExecutingContext GetMockActionExecutingContext(Type controller, string method)
        {
            const string ctrConst = "Controller";
            var controllerName = controller.Name.EndsWith(ctrConst, StringComparison.CurrentCultureIgnoreCase)
                ? controller.Name.Substring(0, controller.Name.Length - 10)
                : controller.Name;
            var methodInfo = controller.GetMethod(method);
            var controllerActionDescriptor = new ControllerActionDescriptor
            {
                ActionName = method,
                ControllerName = controllerName,
                MethodInfo = methodInfo,
                RouteValues = new Dictionary<string, string> { { ctrConst, controllerName }, {"action", method} }
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new(JwtClaimTypes.Name, "Manu"), 
                new(JwtClaimTypes.Email, "none@none.com"),
                new(JwtClaimTypes.Role, "User")
            }));
            var values = new[] {new KeyValuePair<string, object>(ctrConst, controllerName), new KeyValuePair<string, object>("Action", method)};
            var routeData = new RouteData(RouteValueDictionary.FromArray(values));
            var actionContext = new ActionContext(
                new DefaultHttpContext { RequestServices = fixture.WebAppFactory.Services, User = user },
                routeData,
                controllerActionDescriptor,
                new ModelStateDictionary()
            );

            var routePattern = RoutePatternFactory.Parse("{controller}/{action}", new { controller = "Home", action = "Index" }, null, RouteValueDictionary.FromArray(new[]{new KeyValuePair<string, object>("controller", controllerName), new KeyValuePair<string, object>("action", method)}));
            actionContext.HttpContext.SetEndpoint(new RouteEndpoint(c => Task.CompletedTask, routePattern, 0, new EndpointMetadataCollection(methodInfo.GetCustomAttributes(false), controllerActionDescriptor), method));

            return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), new TestController());
        }
    }
}
