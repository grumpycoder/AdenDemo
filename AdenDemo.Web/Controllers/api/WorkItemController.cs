using AdenDemo.Web.Data;
using AdenDemo.Web.Models;
using AdenDemo.Web.ViewModels;
using AutoMapper;
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
    [RoutePrefix("api/workitem")]
    public class WorkItemController : ApiController
    {
        private AdenContext _context;
        public WorkItemController()
        {
            _context = new AdenContext();
        }

        [HttpGet, Route("{id}")]
        public async Task<object> Get(string id, DataSourceLoadOptions loadOptions)
        {
            var username = id;
            if (username == null) return NotFound();

            var dto = await _context.WorkItems
                .Where(u => u.AssignedUser == username && u.WorkItemState == WorkItemState.NotStarted)
                .ProjectTo<WorkItemViewDto>().ToListAsync();

            return Ok(DataSourceLoader.Load(dto.OrderBy(x => x.AssignedDate), loadOptions));
        }

        [HttpGet, Route("finished/{id}")]
        public async Task<object> Finished(string id, DataSourceLoadOptions loadOptions)
        {
            var username = id;
            if (username == null) return NotFound();

            var dto = await _context.WorkItems
                .Where(u => u.AssignedUser == username && u.WorkItemState == WorkItemState.Completed)
                .ProjectTo<WorkItemViewDto>().ToListAsync();

            return Ok(DataSourceLoader.Load(dto.OrderByDescending(x => x.CompletedDate).ThenByDescending(d => d.Action), loadOptions));
        }

        [HttpGet, Route("history/{id}")]
        public async Task<object> History(int id, DataSourceLoadOptions loadOptions)
        {
            var dto = await _context.WorkItems.Where(w => w.ReportId == id)
                            .ProjectTo<WorkItemHistoryDto>().ToListAsync();

            //TODO: Set GlobalAdmin variable
            var IsAdmin = true;
            foreach (var item in dto)
            {
                if (item.WorkItemState == WorkItemState.NotStarted && IsAdmin) item.CanReassign = true;
            }
            return Ok(DataSourceLoader.Load(dto.OrderByDescending(x => x.Id), loadOptions));
        }

        [HttpPost, Route("assign")]
        public async Task<object> Assign(AssignmentDto model)
        {
            var workItem = await _context.WorkItems.Include(r => r.Report).FirstOrDefaultAsync(x => x.Id == model.WorkItemId);
            if (workItem == null) return NotFound();

            var submission = await _context.Submissions.FindAsync(workItem.Report.SubmissionId);

            //Update assigned user
            workItem.AssignedUser = model.AssignedUser;
            submission.CurrentAssignee = model.AssignedUser;

            //Create Audit record
            //TODO: Get current user
            var user = "mark";
            var message = $"Reassigned by {user}: {model.Reason}";
            var audit = new SubmissionAudit(submission.Id, message);
            submission.SubmissionAudits.Add(audit);

            _context.SaveChanges();

            //Send assignment notification
            //TODO: Send notification


            return Ok(model);
        }


        [HttpPost, Route("complete/{id}")]
        public async Task<object> Complete(int id)
        {
            var workItem = await _context.WorkItems.FindAsync(id);

            if (workItem == null) return NotFound();

            var report = await _context.Reports.Include(s => s.Submission.FileSpecification).SingleOrDefaultAsync(r => r.Id == workItem.ReportId);

            //TODO: Handle null report

            if (workItem.WorkItemAction == WorkItemAction.Generate)
            {
                //Create documents
                //TODO: Flesh out Generate documents from stored procdure
                //report.Documents.Add(new ReportDocument() { ReportLevel = ReportLevel.SCH, Version = 1 });
                //_context.SaveChanges();
            }

            //Complete work item
            workItem.CompletedDate = DateTime.Now;
            workItem.WorkItemState = WorkItemState.Completed;


            //Start new work item
            var wi = new WorkItem() { WorkItemState = WorkItemState.NotStarted, AssignedDate = DateTime.Now };
            report.Submission.LastUpdated = DateTime.Now;

            if (workItem.WorkItemAction == WorkItemAction.Generate)
            {
                wi.WorkItemAction = WorkItemAction.Review;
                report.ReportState = ReportState.AssignedForReview;
                report.Submission.SubmissionState = SubmissionState.AssignedForReview;
            }

            if (workItem.WorkItemAction == WorkItemAction.Review)
            {
                wi.WorkItemAction = WorkItemAction.Approve;
                report.ReportState = ReportState.AwaitingApproval;
                report.Submission.SubmissionState = SubmissionState.AwaitingApproval;
            }

            if (workItem.WorkItemAction == WorkItemAction.Approve)
            {
                wi.WorkItemAction = WorkItemAction.Submit;
                report.ReportState = ReportState.AssignedForSubmission;
                report.Submission.SubmissionState = SubmissionState.AssignedForSubmission;
            }

            if (workItem.WorkItemAction == WorkItemAction.Reject)
            {
                wi.WorkItemAction = WorkItemAction.Generate;
                report.ReportState = ReportState.AssignedForGeneration;
                report.Submission.SubmissionState = SubmissionState.AssignedForGeneration;
            }

            if (workItem.WorkItemAction == WorkItemAction.SubmitWithError)
            {
                wi.WorkItemAction = WorkItemAction.ReviewError;
                report.ReportState = ReportState.CompleteWithError;
                report.Submission.SubmissionState = SubmissionState.CompleteWithError;
            }

            if (workItem.WorkItemAction == WorkItemAction.ReviewError)
            {
                wi.WorkItemAction = WorkItemAction.Generate;
                report.ReportState = ReportState.AssignedForGeneration;
                report.Submission.SubmissionState = SubmissionState.AssignedForGeneration;
            }

            if (workItem.WorkItemAction == WorkItemAction.Submit)
            {
                report.ReportState = ReportState.Complete;
                report.Submission.SubmissionState = SubmissionState.Complete;
            }


            //TODO: Get workitem assignee
            var assignedUser = "mark";

            wi.AssignedUser = assignedUser;
            report.Submission.CurrentAssignee = assignedUser;

            if (workItem.WorkItemAction != WorkItemAction.Submit) report.WorkItems.Add(wi);

            _context.SaveChanges();

            //TODO: Send notifications 

            return Ok("completed work item task");

        }

        [HttpPost, Route("reject/{id}")]
        public async Task<object> Reject(int id)
        {
            var workItem = await _context.WorkItems.FindAsync(id);

            if (workItem == null) return NotFound();

            var report = await _context.Reports.Include(s => s.Submission.FileSpecification).SingleOrDefaultAsync(r => r.Id == workItem.ReportId);

            workItem.WorkItemState = WorkItemState.Reject;
            workItem.CompletedDate = DateTime.Now;

            //Start new work item
            //TODO: Get workitem assignee
            var assignedUser = "mark";

            var wi = new WorkItem()
            {
                WorkItemState = WorkItemState.NotStarted,
                AssignedDate = DateTime.Now,
                WorkItemAction = WorkItemAction.Generate,
                AssignedUser = assignedUser
            };
            report.Submission.LastUpdated = DateTime.Now;

            report.ReportState = ReportState.AssignedForGeneration;
            report.Submission.SubmissionState = SubmissionState.AssignedForGeneration;
            report.Submission.CurrentAssignee = assignedUser;

            report.WorkItems.Add(wi);


            await _context.SaveChangesAsync();

            //TODO Send notifications

            var dto = Mapper.Map<WorkItemViewDto>(wi);
            return Ok(dto);
        }


    }
}
