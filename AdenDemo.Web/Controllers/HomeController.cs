using AdenDemo.Web.Data;
using AdenDemo.Web.Filters;
using AdenDemo.Web.Helpers;
using AdenDemo.Web.Models;
using AdenDemo.Web.Services;
using AdenDemo.Web.ViewModels;
using Alsde.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AdenDemo.Web.Controllers
{
    [CustomAuthorize(Roles = "AdenAppUsers")]
    public class HomeController : Controller
    {

        private AdenContext _context;
        private MembershipService _membershipService;

        public HomeController()
        {
            _context = new AdenContext();
            _membershipService = new MembershipService(_context);
        }

        public ActionResult Submissions()
        {
            return View();
        }

        [CustomAuthorize(Roles = Constants.FileSpecificationAdministratorGroup)]
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
            var submission = _context.Submissions.FirstOrDefault(x => x.Id == id);
            var dto = new HistoryViewDto() { CurrentReportId = submission.CurrentReportId, SubmissionId = submission.Id };
            return PartialView("_History", dto);
        }

        public async Task<ActionResult> Review(int dataYear, string filenumber)
        {
            var dto = await _context.Reports
                .Where(f => (f.Submission.FileSpecification.FileNumber == filenumber &&
                             f.Submission.DataYear == dataYear) || string.IsNullOrEmpty(filenumber))
                .ProjectTo<ReportViewDto>().ToListAsync();

            //TODO: Move to mapping profile
            foreach (ReportViewDto item in dto)
            {
                item.Documents.ForEach(x =>
                {
                    x.FileSizeInMb = x.FileSize.ToFileSize();
                    x.FileSizeMb = x.FileSize / 1024;
                    x.FileSizeMb = x.FileSize.ConvertBytesToMega();
                });
            }

            var list = dto.SelectMany(x => x.Documents.Where(d => d.Version < x.CurrentDocumentVersion)).ToList();
            ViewBag.Old = list;
            return View(dto);
            ;
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
            return PartialView("_WorkItemAssignment", dto);
        }

        public ActionResult EditFileSpecification(int id)
        {
            var model = _context.FileSpecifications
                .Include(g => g.GenerationGroup.Users)
                .Include(g => g.ApprovalGroup.Users)
                .Include(g => g.SubmissionGroup.Users)
                .FirstOrDefault(x => x.Id == id);

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

            var applications = new IdemService().GetApplication().ConvertAll(a => new SelectListItem() { Text = a.Title, Value = a.Title });

            ViewBag.Groups = _context.Groups.OrderBy(g => g.Name).ToList()
                .ConvertAll(a => new SelectListItem() { Text = a.Name, Value = a.Id.ToString() });


            ViewBag.Applications = applications;
            ViewBag.DataGroups = dataGroups;
            ViewBag.Collections = collections;
            return PartialView("_FileSpecificationEditForm", dto);
        }

        public async Task<ActionResult> ErrorReport(int id)
        {
            var dto = await _context.WorkItems.ProjectTo<SubmissionErrorDto>().FirstOrDefaultAsync(x => x.Id == id);

            return PartialView("_ErrorReportForm", dto);
        }

        public async Task<ActionResult> WorkItemImages(int id)
        {
            var wi = await _context.WorkItems.Include(x => x.WorkItemImages).FirstOrDefaultAsync(x => x.Id == id);

            return PartialView("_WorkItemImage", wi);
        }

        public ActionResult Document(int id)
        {
            ViewBag.Id = id;
            return PartialView("_Document");
        }

        public async Task<FileResult> Download(int id)
        {
            var document = await _context.ReportDocuments.FindAsync(id);
            return File(document.FileData, System.Net.Mime.MediaTypeNames.Application.Octet, document.Filename);
        }

        public async Task<object> ReportError(SubmissionErrorDto model)
        {
            //TODO: Will not work in WebApi. Convert to Webapi method /api/workitem/reporterror

            var id = model.Id;

            if (model.Files.Length == 0) ModelState.AddModelError("", "You must include at least 1 file");
            if (!ModelState.IsValid)
            {

                var errors = new List<string>();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //TODO: Refactor to submission model

            var workItem = await _context.WorkItems.FindAsync(id);
            if (workItem == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            //Complete current work item
            workItem.CompletedDate = DateTime.Now;
            workItem.WorkItemState = WorkItemState.Completed;

            //Create new generation work item
            var report = await _context.Reports.Include(s => s.Submission.FileSpecification.GenerationGroup.Users).SingleOrDefaultAsync(r => r.Id == workItem.ReportId);

            
            var assignedUser = _membershipService.GetAssignee(report.Submission.FileSpecification.GenerationGroup);

            var wi = new WorkItem()
            {
                WorkItemState = WorkItemState.NotStarted,
                AssignedDate = DateTime.Now,
                WorkItemAction = WorkItemAction.ReviewError,
                AssignedUser = assignedUser,
                Description = model.Description
            };
            report.Submission.LastUpdated = DateTime.Now;

            report.ReportState = ReportState.CompleteWithError;
            report.Submission.SubmissionState = SubmissionState.CompleteWithError;
            report.Submission.CurrentAssignee = assignedUser;


            foreach (var f in model.Files)
            {
                wi.WorkItemImages.Add(new WorkItemImage() { Image = f.ConvertToByte(), });
            }

            report.WorkItems.Add(wi);

            //WorkEmailer.Send(wi, report.Submission, model.Files);

            //await _context.SaveChangesAsync();

            return new HttpStatusCodeResult(HttpStatusCode.OK);

        }

        public ActionResult EditGroupMembership(int id)
        {
            var dto = _context.Groups.Include(u => u.Users).FirstOrDefault(x => x.Id == id);


            //return PartialView("_GroupMembershipEditForm", dto);

            var displayGroupName = dto.Name.Humanize().ToTitleCase().RemoveExactWord("App");
            ViewBag.GroupName = dto.Name;
            ViewBag.DisplayGroupName = displayGroupName;
            ViewBag.IsGroupDefined = true;


            //var displayGroupName = id.Humanize().ToTitleCase().RemoveExactWord("App");
            //ViewBag.GroupName = id;
            //ViewBag.DisplayGroupName = displayGroupName;
            //ViewBag.IsGroupDefined = true;

            //var groupExists = _membershipService.GroupExists(id);

            //if (!groupExists)
            //{
            //    ViewBag.IsGroupDefined = false;
            //    return PartialView("_GroupMembershipEditForm");
            //}

            //var membersResult = _membershipService.GetGroupMembers(id);

            //if (membersResult.IsSuccess) ViewBag.Members = membersResult.Value;


            //if (membersResult.IsFailure) ViewBag.IsGroupDefined = false;

            //return PartialView("_GroupMembershipEditForm");

            return PartialView("_GroupMembershipEditForm", dto);

        }
    }
}