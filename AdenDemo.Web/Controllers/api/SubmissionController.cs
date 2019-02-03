using AdenDemo.Web.Data;
using AdenDemo.Web.Models;
using AdenDemo.Web.Services;
using AdenDemo.Web.ViewModels;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AdenDemo.Web.Controllers.api
{
    [RoutePrefix("api/submission")]
    public class SubmissionController : ApiController
    {
        private AdenContext _context;
        private MembershipService _membershipService;
        private string _currentUserFullName;

        public SubmissionController()
        {
            _context = new AdenContext();
            _membershipService = new MembershipService(_context);
            _currentUserFullName = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
        }

        [HttpGet]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            var dto = await _context.Submissions.ProjectTo<SubmissionViewDto>().ToListAsync();

            return Ok(DataSourceLoader.Load(dto.OrderBy(x => x.DueDate).ThenByDescending(x => x.Id), loadOptions));
        }

        [HttpPost, Route("waive/{id}")]
        public async Task<object> Waive(int id, SubmissionWaiveAuditEntryDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Message)) return BadRequest("No message provided");

            var submission = await _context.Submissions.FirstOrDefaultAsync(s => s.Id == id);
            if (submission == null) return NotFound();

            submission.Waive(model.Message, _currentUserFullName);

            _context.SaveChanges();

            return Ok("Success");
        }

        [HttpPost, Route("start/{id}")]
        public async Task<object> Start(int id)
        {
            //TODO: Getting too much data
            var submission = await _context.Submissions.Include(f => f.FileSpecification.GenerationGroup.Users).FirstOrDefaultAsync(x => x.Id == id);
            if (submission == null) return NotFound();

            if (submission.FileSpecification.GenerationUserGroup == null)
                return BadRequest($"No generation group defined for File { submission.FileSpecification.FileNumber }");

            if (!submission.FileSpecification.GenerationGroup.Users.Any())
                return BadRequest($"No group members to assign next task. ");

            var assignedUser = _membershipService.GetAssignee(submission.FileSpecification.GenerationGroup);

            if (string.IsNullOrWhiteSpace(assignedUser)) return BadRequest("No group members to assign next task. ");

            //Change state
            submission.SubmissionState = SubmissionState.AssignedForGeneration;
            submission.CurrentAssignee = assignedUser;
            submission.LastUpdated = DateTime.Now;

            //Create report
            var report = new Report() { SubmissionId = submission.Id, DataYear = submission.DataYear, ReportState = ReportState.AssignedForGeneration };
            submission.Reports.Add(report);

            //Create work item
            var workItem = new WorkItem()
            {
                WorkItemAction = WorkItemAction.Generate,
                WorkItemState = WorkItemState.NotStarted,
                AssignedDate = DateTime.Now,
                AssignedUser = assignedUser
            };
            report.WorkItems.Add(workItem);

            //WorkEmailer.Send(workItem, submission);

            _context.SaveChanges();

            submission.CurrentReportId = report.Id;

            _context.SaveChanges();

            return Ok(submission);
        }

        [HttpPost, Route("cancel/{id}")]
        public async Task<object> Cancel(int id)
        {
            var submission = await _context.Submissions.Include(f => f.FileSpecification).FirstOrDefaultAsync(x => x.Id == id);
            if (submission == null) return NotFound();

            var workItem = _context.WorkItems.SingleOrDefault(x => x.ReportId == submission.CurrentReportId && x.WorkItemState == WorkItemState.NotStarted);

            //Set Submission State and clear assignee
            submission.SubmissionState = SubmissionState.NotStarted;
            submission.CurrentAssignee = string.Empty;

            //Create Audit record
            var message = $"{_currentUserFullName} cancelled submission";
            var audit = new SubmissionAudit(submission.Id, message);
            submission.SubmissionAudits.Add(audit);

            //Remove Reports/Documents/WorkItems
            var report = await _context.Reports.FirstOrDefaultAsync(r => r.SubmissionId == id);
            if (report != null)
            {
                var workItems = _context.WorkItems.Where(w => w.ReportId == report.Id);
                _context.WorkItems.RemoveRange(workItems);

                var docs = _context.ReportDocuments.Where(d => d.ReportId == report.Id);
                _context.ReportDocuments.RemoveRange(docs);

                _context.Reports.Remove(report);
            }

            WorkEmailer.Send(workItem, submission);
            _context.SaveChanges();

            return Ok();

        }

        //[HttpPost, Route("restart")]
        [HttpPost, Route("reopen/{id}")]
        public async Task<object> ReOpen(int id, SubmissionReOpenAuditEntryDto model)
        {

            if (model == null) return BadRequest("No audit entry found in request");

            //TODO: Pulling too much data here
            var submission = await _context.Submissions.Include(f => f.FileSpecification.GenerationGroup.Users).FirstOrDefaultAsync(x => x.Id == id);
            if (submission == null) return NotFound();

            if (string.IsNullOrWhiteSpace(submission.FileSpecification.GenerationUserGroup))
                return BadRequest($"No generation group defined for File { submission.FileSpecification.FileNumber }");

            //Create Audit record
            var message = $"{_currentUserFullName} reopened submission: { model.Message }";
            var audit = new SubmissionAudit(submission.Id, message);
            submission.SubmissionAudits.Add(audit);

            var assignedUser = _membershipService.GetAssignee(submission.FileSpecification.GenerationGroup);

            //Change state
            submission.SubmissionState = SubmissionState.AssignedForGeneration;
            submission.CurrentAssignee = assignedUser;
            submission.LastUpdated = DateTime.Now;
            submission.NextDueDate = model.NextSubmissionDate;

            //Create report
            var report = new Report() { SubmissionId = submission.Id, DataYear = submission.DataYear, ReportState = ReportState.AssignedForGeneration };
            submission.Reports.Add(report);

            //Create work item
            var workItem = new WorkItem()
            {
                WorkItemAction = WorkItemAction.Generate,
                WorkItemState = WorkItemState.NotStarted,
                AssignedUser = assignedUser
            };
            report.WorkItems.Add(workItem);

            //WorkEmailer.Send(workItem, submission);

            _context.SaveChanges();

            return Ok("Successfully repopened");

        }

    }
}
