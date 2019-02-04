using AdenDemo.Web.Data;
using AdenDemo.Web.Models;
using AdenDemo.Web.Services;
using AdenDemo.Web.ViewModels;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AdenDemo.Web.Controllers.api
{
    [RoutePrefix("api/workitem")]
    [Authorize(Roles = "AdenAppUsers")]
    public class WorkItemController : ApiController
    {
        private AdenContext _context;
        private MembershipService _membershipService;
        private DocumentService _documentService;
        private string _currentUserFullName;
        private string _currentUusername;

        public WorkItemController()
        {
            _context = new AdenContext();
            _membershipService = new MembershipService(_context);
            _currentUserFullName = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
            _currentUusername = User.Identity.Name;
            _documentService = new DocumentService(_context);

        }

        [HttpGet, Route("")]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            if (_currentUusername == null) return NotFound();

            var dto = await _context.WorkItems
                .Where(u => u.AssignedUser == _currentUusername && (u.WorkItemState == WorkItemState.NotStarted || u.WorkItemState == WorkItemState.Reassigned))
                .ProjectTo<WorkItemViewDto>().ToListAsync();

            return Ok(DataSourceLoader.Load(dto.OrderBy(x => x.AssignedDate), loadOptions));
        }

        [HttpGet, Route("finished")]
        public async Task<object> Finished(DataSourceLoadOptions loadOptions)
        {
            if (_currentUusername == null) return NotFound();

            var dto = await _context.WorkItems
                .Where(u => u.AssignedUser == _currentUusername && u.WorkItemState == WorkItemState.Completed)
                .ProjectTo<WorkItemViewDto>().ToListAsync();

            return Ok(DataSourceLoader.Load(dto.OrderByDescending(x => x.CompletedDate).ThenByDescending(d => d.Action), loadOptions));
        }

        [HttpGet, Route("history/{id}")]
        public async Task<object> History(int id, DataSourceLoadOptions loadOptions)
        {
            var dto = await _context.WorkItems.Where(w => w.ReportId == id)
                            .ProjectTo<WorkItemHistoryDto>().ToListAsync();

            return Ok(DataSourceLoader.Load(dto.OrderByDescending(x => x.Id), loadOptions));
        }

        [HttpPost, Route("assign")]
        public async Task<object> Assign(AssignmentDto model)
        {
            var workItem = await _context.WorkItems.Include(r => r.Report).FirstOrDefaultAsync(x => x.Id == model.WorkItemId);
            if (workItem == null) return NotFound();

            var submission = await _context.Submissions
                .Include(s => s.FileSpecification)
                .FirstOrDefaultAsync(x => x.Id == workItem.Report.SubmissionId);

            submission.Reassign(_currentUserFullName, workItem, model.AssignedUser, model.Reason);

            _context.SaveChanges();

            //Send assignment notification
            WorkEmailer.Send(workItem, submission);

            return Ok(model);
        }

        [HttpPost, Route("complete/{id}")]
        public async Task<object> Complete(int id)
        {
            var workItem = await _context.WorkItems.Include(x => x.Report.Submission).FirstOrDefaultAsync(x => x.Id == id);

            if (workItem == null) return NotFound();

            //TODO: Pulling too much data here
            var submission = _context.Submissions
                .Include(f => f.FileSpecification.GenerationGroup.Users)
                .Include(f => f.FileSpecification.ApprovalGroup.Users)
                .Include(f => f.FileSpecification.SubmissionGroup.Users)
                .FirstOrDefault(x => x.Id == workItem.Report.SubmissionId);


            var currentReport = submission.Reports.FirstOrDefault(x => x.Id == submission.CurrentReportId);
            if (workItem.WorkItemAction == WorkItemAction.Generate)
            {
                _documentService.GenerateDocuments(currentReport);
            }

            Group group;
            switch (workItem.WorkItemAction)
            {
                case WorkItemAction.Approve:
                    group = submission.FileSpecification.ApprovalGroup;
                    break;
                case WorkItemAction.Submit:
                    group = submission.FileSpecification.SubmissionGroup;
                    break;
                default:
                    group = submission.FileSpecification.ApprovalGroup;
                    break;
            }
            var assignee = _membershipService.GetAssignee(group);

            var wi = submission.CompleteWork(workItem, assignee);

            WorkEmailer.Send(wi, submission);

            _context.SaveChanges();

            var dto = Mapper.Map<WorkItemViewDto>(workItem);

            return Ok(dto);

        }

        [HttpPost, Route("reject/{id}")]
        public async Task<object> Reject(int id)
        {
            var workItem = await _context.WorkItems.FindAsync(id);

            if (workItem == null) return NotFound();

            //TODO: Pulling too much data here
            var submission = await _context.Submissions
                .Include(x => x.FileSpecification.GenerationGroup.Users)
                .Include(r => r.Reports)
                .FirstOrDefaultAsync(x => x.CurrentReportId == workItem.ReportId);

            if (!submission.FileSpecification.GenerationGroup.Users.Any()) return BadRequest("No group members to assign next task. ");

            var wi = submission.Reject(workItem);

            WorkEmailer.Send(wi, submission);

            await _context.SaveChangesAsync();

            var dto = Mapper.Map<WorkItemViewDto>(wi);
            return Ok(dto);
        }

    }
}
