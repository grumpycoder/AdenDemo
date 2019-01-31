using System.Data.Entity;
using AdenDemo.Web.Data;
using ALSDE.Services;
using System.Linq;
using System.Web.Http;

namespace AdenDemo.Web.Controllers.api
{
    [RoutePrefix("api/demo")]
    public class DemoController : ApiController
    {
        private AdenContext _context;

        public DemoController()
        {
            _context = new AdenContext();
        }

        public object Get()
        {
            var dto = _context.Users.Include(g => g.Groups).ToList();
            return Ok(dto);
        }

        [HttpGet, Route("groups")]
        public object Groups()
        {
            var dto = _context.Groups.Include(u => u.Users).ToList();
            return Ok(dto);
        }
    }
}
