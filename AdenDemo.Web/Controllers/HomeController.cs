using AdenDemo.Web.Data;
using AdenDemo.Web.ViewModels;
using System.Web.Mvc;

namespace AdenDemo.Web.Controllers
{
    public class HomeController : Controller
    {
        private AdenContext _context;
        public HomeController()
        {
            _context = new AdenContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Audit(int id)
        {
            var audit = new SubmissionAuditEntryDto(id);
            return PartialView("_SubmissionAuditEntry", audit);
        }

        public ActionResult Waiver()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

    }
}