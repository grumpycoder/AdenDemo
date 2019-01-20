using AdenDemo.Web.Data;
using AdenDemo.Web.Models;
using AdenDemo.Web.ViewModels;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
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

    }
}
