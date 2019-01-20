using AdenDemo.Web.Data;
using AdenDemo.Web.ViewModels;
using AutoMapper;
using System.Linq;
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

        public ActionResult Assignments()
        {
            return View();
        }

        public ActionResult History(int id)
        {
            //TODO: Set SectionAction variable
            ViewBag.IsSectionAdmin = true;
            
            //TODO: Refactor using ViewBag for CurrentReport
            var dto = _context.Submissions.FirstOrDefault(x => x.Id == id);
            ViewBag.CurrentReportId = dto.CurrentReportId;
            return PartialView("_History");
        }

        public ActionResult Review(int dataYear, string fileNumber)
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

        public ActionResult Reassign(int id)
        {
            var workItem = _context.WorkItems.FirstOrDefault(x => x.Id == id);

            var dto = Mapper.Map<AssignmentDto>(workItem);
            //TODO: Display typeahead selection for assignment field
            return PartialView("_WorkItemAssignment", dto);
        }
    }
}