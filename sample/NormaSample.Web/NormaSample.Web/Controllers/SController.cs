using EC.Norma.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace NormaSample.Web.Controllers
{
    public class SController : Controller
    {
        
        [NormaPermission("ListarS")]
        public ActionResult Index()
        {
            return View();
        }

        [NormaPermission("ConsultarS")]
        public ActionResult Details(int id)
        {
            return View(id);
        }

        [NormaPermission("EditarS")]
        public ActionResult Edit(int id)
        {
            return View();
        }

    }
}