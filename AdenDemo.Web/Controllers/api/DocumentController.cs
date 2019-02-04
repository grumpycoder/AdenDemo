using AdenDemo.Web.Data;
using AdenDemo.Web.ViewModels;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace AdenDemo.Web.Controllers.api
{
    [RoutePrefix("api/document")]
    [Authorize(Roles = "AdenAppUsers")]
    public class DocumentController : ApiController
    {
        private AdenContext _context;
        public DocumentController()
        {
            _context = new AdenContext();
        }

        [HttpGet, Route("{id:int}")]
        public async Task<object> Document(int id)
        {

            var dto = await _context.ReportDocuments.Where(d => d.Id == id).Select(r => new FileViewDto()
            {
                Filename = r.Filename,
                Version = r.Version,
                Id = r.Id,
                FileData = r.FileData,
                FileSize = r.FileSize

            }).FirstOrDefaultAsync();


            return Ok(dto);
        }
    }
}
