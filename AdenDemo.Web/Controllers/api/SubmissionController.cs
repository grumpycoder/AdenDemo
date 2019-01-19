using AdenDemo.Web.Data;
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
        [HttpGet]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            var _context = new AdenContext();

            var dto = await _context.Submissions.ProjectTo<SubmissionViewDto>().ToListAsync();

            return Ok(DataSourceLoader.Load(dto.OrderBy(x => x.DueDate).ThenByDescending(x => x.Id), loadOptions));
        }

        [HttpPost, Route("waiver/{id}")]
        public async Task<object> Waiver(int id)
        {
            var _context = new AdenContext();
            var submission = await _context.Submissions.FirstOrDefaultAsync(x => x.Id == id);

            return Ok(submission);
        }
    }
}
