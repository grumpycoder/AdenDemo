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

        [HttpPost, Route("waiver/{id}")]
        public async Task<object> Waiver(int id, SubmissionAuditEntryDto model)
        {
            var submission = await _context.Submissions.FirstOrDefaultAsync(s => s.Id == id);
            if (submission == null) return NotFound();

            //TODO: Complete Waiver processing
            //Change state
            submission.SubmissionState = SubmissionState.Waived;

            //Create report
            //var report = submission.CreateReport();

            //Create Audit record

            //Save changes
            //_context.SaveChanges();

            return Ok(submission);
        }

    }
}
