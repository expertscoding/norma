using EC.Norma.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace EC.Norma.Tests
{
    public class TestController : Controller
    {
        public const string Name = "Test";

        public IActionResult PlainAction() => Ok();

        [ByPassNorma]
        public IActionResult ByPassedAction() => Ok();

        [NormaAction("List")]
        public IActionResult AnnotatedAction() => Ok();

        [NormaAction("TwoPoliciesAction")]
        public IActionResult TwoPoliciesAction() => Ok();

        [ByPassNorma]
        [NormaAction("List")]
        public IActionResult ByPassedAnnotatedAction() => Ok();

        public IActionResult WithoutPermissions() => Ok();

        public IActionResult WithoutRequirement() => Ok();

        public IActionResult WithoutConfiguredRequirement() => Ok();
    }
}