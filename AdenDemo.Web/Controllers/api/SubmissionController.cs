using AdenDemo.Web.Data;
using AdenDemo.Web.Models;
using AdenDemo.Web.ViewModels;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace AdenDemo.Web.Controllers.api
{
    [RoutePrefix("api/submission")]
    public class SubmissionController : ApiController
    {
        private AdenContext _context;
        public SubmissionController()
        {
            _context = new AdenContext();
        }

        [HttpGet]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            var dto = await _context.Submissions.ProjectTo<SubmissionViewDto>().ToListAsync();

            return Ok(DataSourceLoader.Load(dto.OrderBy(x => x.DueDate).ThenByDescending(x => x.Id), loadOptions));
        }

        [HttpPost, Route("waive/{id}")]
        public async Task<object> Waive(int id, SubmissionAuditEntryDto model)
        {
            var submission = await _context.Submissions.FirstOrDefaultAsync(s => s.Id == id);
            if (submission == null) return NotFound();

            //TODO: Refactor model
            //Change state
            submission.SubmissionState = SubmissionState.Waived;
            submission.LastUpdated = DateTime.Now;

            //Create waived report
            var report = new Report() { SubmissionId = submission.Id, DataYear = submission.DataYear, ReportState = ReportState.Waived };
            submission.Reports.Add(report);

            //Create Audit record
            var user = "mark";
            var message = $"Waived by {user}: {model.Message}";
            var audit = new SubmissionAudit(submission.Id, message);
            submission.SubmissionAudits.Add(audit);

            //Save changes
            _context.SaveChanges();

            return Ok("Success");
        }

        [HttpPost, Route("start/{id}")]
        public async Task<object> Start(int id)
        {
            var submission = await _context.Submissions.Include(f => f.FileSpecification).FirstOrDefaultAsync(x => x.Id == id);
            if (submission == null) return NotFound();

            if (string.IsNullOrWhiteSpace(submission.FileSpecification.GenerationUserGroup))
                return BadRequest($"No generation group defined for File { submission.FileSpecification.FileNumber }");

            //TODO: Get next assignee
            var assignee = "mark";

            //Change state
            submission.SubmissionState = SubmissionState.AssignedForGeneration;
            submission.CurrentAssignee = assignee;
            submission.LastUpdated = DateTime.Now;

            //Create report
            var report = new Report() { SubmissionId = submission.Id, DataYear = submission.DataYear, ReportState = ReportState.AssignedForGeneration };
            submission.Reports.Add(report);

            //Create work item
            var workItem = new WorkItem()
            {
                WorkItemAction = WorkItemAction.Generate,
                WorkItemState = WorkItemState.NotStarted,
                AssignedUser = assignee
            };
            report.WorkItems.Add(workItem);

            _context.SaveChanges();

            return Ok(submission);
        }
    }
}
