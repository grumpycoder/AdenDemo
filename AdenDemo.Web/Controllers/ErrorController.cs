using System.Web.Mvc;

namespace AdenDemo.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Error = Session["Error"];
            return View();
        }
        public ActionResult NotFound()
        {
            return View();
        }

    }
}
