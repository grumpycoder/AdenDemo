using AdenDemo.Web.Data;
using AdenDemo.Web.ViewModels;
using AutoMapper;
using System.Collections.Generic;
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

        public ActionResult FileSpecifications()
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

        public ActionResult Waiver(int id)
        {
            var audit = new SubmissionWaiveAuditEntryDto(id);
            return PartialView("_SubmissionWaiverAuditEntry", audit);
        }

        public ActionResult ReOpen(int id)
        {
            var audit = new SubmissionReOpenAuditEntryDto(id);
            return PartialView("_SubmissionReOpenAuditEntry", audit);
        }

        public ActionResult Reassign(int id)
        {
            var workItem = _context.WorkItems.FirstOrDefault(x => x.Id == id);

            var dto = Mapper.Map<AssignmentDto>(workItem);
            //TODO: Display typeahead selection for assignment field
            return PartialView("_WorkItemAssignment", dto);
        }

        public ActionResult EditFileSpecification(int id)
        {
            var model = _context.FileSpecifications.Find(id);

            var dto = Mapper.Map<UpdateFileSpecificationDto>(model);

            //TODO: Check for null file specification


            var dataGroups = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = "Data", Text = "Data"},
                new SelectListItem(){ Value = "Development", Text = "Development"},
                new SelectListItem(){ Value = "Section", Text = "Section"}
            };

            var collections = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = "Accumulator", Text = "Accumulator"},
                new SelectListItem(){ Value = "Assessment", Text = "Assessment"},
                new SelectListItem(){ Value = "AnnualDataReport", Text = "Annual Data Report"},
                new SelectListItem(){ Value = "Application", Text = "Application"},
                new SelectListItem(){ Value = "ChildCount", Text = "Child Count"},
                new SelectListItem(){ Value = "Financials", Text = "Financials"},
                new SelectListItem(){ Value = "EOY-9thMonth", Text = "EOY - 9th Month"},
                new SelectListItem(){ Value = "Fall-20Day", Text = "Fall-20 Day"},
                new SelectListItem(){ Value = "Manual", Text = "Manual"},
                new SelectListItem(){ Value = "SIR", Text = "SIR"},
                new SelectListItem(){ Value = "Schedules", Text = "Schedules"}
            };

            //TODO: Get application from IDEM
            var applications = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = "APplication1", Text = "Application 1"},
                new SelectListItem(){ Value = "Application2", Text = "Application 2"},
                new SelectListItem(){ Value = "Application3", Text = "Application 3"}
            };

            //TODO: Get member group count
            ViewBag.GenerationGroupMemberCount = 3;
            ViewBag.ApprovalGroupMemberCount = 2;

            ViewBag.Applications = applications;
            ViewBag.DataGroups = dataGroups;
            ViewBag.Collections = collections;
            return PartialView("_FileSpecificationEditForm", dto);
        }
    }
}