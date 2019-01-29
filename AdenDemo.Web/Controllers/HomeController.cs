using AdenDemo.Web.Data;
using AdenDemo.Web.Helpers;
using AdenDemo.Web.Models;
using AdenDemo.Web.Services;
using AdenDemo.Web.ViewModels;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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

        public ActionResult Submissions()
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
            ViewBag.SubmissionId = id;
            return PartialView("_History");
        }

        public async Task<ActionResult> Review(int dataYear, string filenumber)
        {
            //TODO: Move to mapping profile
            var dto = await _context.Reports
                .Where(f => (f.Submission.FileSpecification.FileNumber == filenumber && f.Submission.DataYear == dataYear) || string.IsNullOrEmpty(filenumber))
                .Select(m =>
                    new ReportViewDto
                    {
                        Id = m.Id,
                        FileName = m.Submission.FileSpecification.FileName,
                        FileNumber = m.Submission.FileSpecification.FileNumber,
                        DataYear = m.Submission.FileSpecification.DataYear,
                        ReportState = m.ReportState,
                        ApprovedDate = m.ApprovedDate,
                        GeneratedDate = m.GeneratedDate,
                        SubmittedDate = m.SubmittedDate,
                        Documents = m.Documents.Select(d => new DocumentViewDto()
                        {
                            Id = d.Id,
                            Filename = d.Filename,
                            Version = d.Version,
                            FileSize = d.FileSize,
                        }).ToList()
                    }
                )
                .ToListAsync();
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

            var workItem = await _context.WorkItems.FindAsync(id);
            if (workItem == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            //Complete current work item
            workItem.CompletedDate = DateTime.Now;
            workItem.WorkItemState = WorkItemState.Completed;

            //Create new generation work item
            var report = await _context.Reports.Include(s => s.Submission.FileSpecification).SingleOrDefaultAsync(r => r.Id == workItem.ReportId);

            var assignedUser = "mark@mail.com";

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

            WorkEmailer.Send(wi, report.Submission, model.Files);

            await _context.SaveChangesAsync();

            return new HttpStatusCodeResult(HttpStatusCode.OK);

        }
    }
}