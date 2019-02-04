using AdenDemo.Web.Data;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace AdenDemo.Web.Controllers.api
{
    [RoutePrefix("api/audit")]
    [Authorize(Roles = "AdenAppUsers")]
    public class AuditController : ApiController
    {
        private AdenContext _context;
        public AuditController()
        {
            _context = new AdenContext();
        }

        [HttpGet, Route("{id}")]
        public async Task<object> Get(int id, DataSourceLoadOptions loadOptions)
        {
            var dto = await _context.SubmissionAudits.Where(r => r.SubmissionId == id).ToListAsync();

            return Ok(DataSourceLoader.Load(dto.OrderByDescending(x => x.AuditDate), loadOptions));
        }

    }
}
