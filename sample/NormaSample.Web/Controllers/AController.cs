using EC.Norma.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace NormaSample.Web.Controllers
{
    public class AController : Controller
    {
        [NormaAction("Listar")]
        public ActionResult Index()
        {
            return View();
        }

        [NormaAction("Consultar")]
        public ActionResult Details(int id)
        {
            return View(id);
        }

        public ActionResult Edit(int id)
        {
            return View(id);
        }

        public ActionResult Delete(int id)
        {
            return View(id);
        }

        [NormaAction("Protect")]
        [NormaResource("B")]
        //[NormaPermission("VerySpecialPermission")] // To test a specific permission in App 'APPKEY-WITH-DEFAULT-REQUIREMENTS'
        public ActionResult Protect()
        {
            return View();
        }

        //[NormaAction("NoDefinido")] // Intentionally commented to illustrate the configuration usage of NormaOptions.NoPermissionAction; 
        public ActionResult NotDefined()
        {
            return View("Index");
        }

        public ActionResult Manage()
        {
            return View();
        }
    }
}
