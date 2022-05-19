using EC.Norma.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace EC.Norma.Options.Tests
{
    public class TestController : Controller
    {
        public const string Name = "Test";

        public IActionResult PlainAction() => Ok();

        [NormaAction("List")]
        public IActionResult AnnotatedAction() => Ok();

        public IActionResult WithoutPermissions() => Ok();

        public IActionResult WithoutRequirement() => Ok();

        public IActionResult WithoutConfiguredRequirement() => Ok();
    }
}