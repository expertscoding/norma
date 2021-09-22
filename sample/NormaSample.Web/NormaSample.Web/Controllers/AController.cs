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

        [NormaAction("Detalles")]
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
    }
}
