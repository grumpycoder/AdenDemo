using Aden.Web.Services;
using Aden.Web.ViewModels;
using AdenDemo.Web.Data;
using AdenDemo.Web.Models;
using AdenDemo.Web.ViewModels;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aden.Web.Controllers.api
{
    [RoutePrefix("api/submission")]
    [Authorize(Roles = "AdenAppUsers")]
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

            var submission = await _context.Submissions.Include(f => f.FileSpecification).FirstOrDefaultAsync(s => s.Id == id);
            if (submission == null) return NotFound();

            submission.Waive(model.Message, _currentUserFullName);

            _context.SaveChanges();

            //TODO: Refactor. Do not have access to new report until after save
            submission.CurrentReportId = submission.Reports.LastOrDefault().Id;

            _context.SaveChanges();

            var dto = Mapper.Map<SubmissionViewDto>(submission);

            return Ok(dto);
        }

        [HttpPost, Route("start/{id}")]
        public async Task<object> Start(int id)
        {
            var submission = await _context.Submissions
                .Include(f => f.FileSpecification)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (submission == null) return NotFound();

            if (submission.FileSpecification.GenerationUserGroup == null)
                return BadRequest($"No generation group defined for File { submission.FileSpecification.FileNumber }");

            var group = _context.Groups.Include(x => x.Users).FirstOrDefault(x => x.Id == submission.FileSpecification.GenerationGroupId);
            var assignedUser = _membershipService.GetAssignee(group);

            if (assignedUser == null)
                return BadRequest($"No group members to assign next task. ");

            var workItem = submission.Start(assignedUser);

            WorkEmailer.Send(workItem, submission);

            _context.SaveChanges();

            submission.CurrentReportId = submission.Reports.LastOrDefault().Id;

            _context.SaveChanges();

            var dto = Mapper.Map<SubmissionViewDto>(submission);

            return Ok(dto);
        }

        [HttpPost, Route("cancel/{id}")]
        public async Task<object> Cancel(int id)
        {
            var submission = await _context.Submissions
                .Include(f => f.FileSpecification)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (submission == null) return NotFound();

            //TODO: Need to remove retrieval to current work item
            var workItem = _context.WorkItems.Include(x => x.AssignedUser)
                .FirstOrDefault(x => x.ReportId == submission.CurrentReportId && x.WorkItemState == WorkItemState.NotStarted);

            //Create copy because removing workitems produces null assignee 
            var wi = workItem.DeepCopy();

            //TODO: Submission does not need to be aware of data context
            //Remove Reports/Documents/WorkItems from submission
            var report = await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == submission.CurrentReportId);
            if (report != null)
            {
                var workItems = _context.WorkItems.Where(w => w.ReportId == report.Id);
                _context.WorkItems.RemoveRange(workItems);

                var docs = _context.ReportDocuments.Where(d => d.ReportId == report.Id);
                _context.ReportDocuments.RemoveRange(docs);

                _context.Reports.Remove(report);
            }

            submission.Cancel(_currentUserFullName);

            WorkEmailer.Send(wi, submission);

            _context.SaveChanges();

            var dto = Mapper.Map<SubmissionViewDto>(submission);

            return Ok(dto);

        }

        [HttpPost, Route("reopen/{id}")]
        public async Task<object> ReOpen(int id, SubmissionReOpenAuditEntryDto model)
        {

            if (model == null) return BadRequest("No audit entry found in request");

            //TODO: Pulling too much data here
            var submission = await _context.Submissions
                .Include(f => f.FileSpecification.GenerationGroup)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (submission == null) return NotFound();

            if (string.IsNullOrWhiteSpace(submission.FileSpecification.GenerationUserGroup))
                return BadRequest($"No generation group defined for File { submission.FileSpecification.FileNumber }");

            var group = _context.Groups.Include(x => x.Users)
                .FirstOrDefault(x => x.Id == submission.FileSpecification.GenerationGroupId);
            var assignedUser = _membershipService.GetAssignee(group);

            var workItem = submission.Reopen(_currentUserFullName, model.Message, assignedUser, model.NextSubmissionDate);

            WorkEmailer.Send(workItem, submission);

            _context.SaveChanges();

            //TODO: Refactor. Do not have access to new report until after save
            submission.CurrentReportId = submission.Reports.LastOrDefault().Id;

            _context.SaveChanges();

            var dto = Mapper.Map<SubmissionViewDto>(submission);

            return Ok(dto);

        }

    }
}
