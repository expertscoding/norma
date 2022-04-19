using EC.Norma.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace EC.Norma.Tests
{
    public class TestController : Controller
    {
        public const string Name = "Test";

        public IActionResult PlainAction() => Ok();
        public IActionResult PlainActionApplication2() => Ok();
        public IActionResult DefaultAction() => Ok();

        [ByPassNorma]
        public IActionResult ByPassedAction() => Ok();

        [NormaAction("List")]
        public IActionResult AnnotatedAction() => Ok();

        [NormaAction("TwoRequirementsAction")]
        public IActionResult TwoRequirementsAction() => Ok();

        [ByPassNorma]
        [NormaAction("List")]
        public IActionResult ByPassedAnnotatedAction() => Ok();

        public IActionResult WithoutPermissions() => Ok();

        public IActionResult WithoutRequirement() => Ok();

        public IActionResult WithoutConfiguredRequirement() => Ok();
    }
}
