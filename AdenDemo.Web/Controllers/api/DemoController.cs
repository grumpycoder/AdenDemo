using AdenDemo.Web.Data;
using AdenDemo.Web.ViewModels;
using AutoMapper.QueryableExtensions;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace Aden.Web.Controllers.api
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


        [HttpGet, Route("specs")]
        public object Specs()
        {
            //var dto = _context.FileSpecifications
            //    .Include(x => x.GenerationGroup.Users)
            //    .Include(x => x.ApprovalGroup.Users)
            //    .Include(x => x.SubmissionGroup.Users)
            //    .Where(x => x.FileNumber == "029").ToList();

            var dto = _context.FileSpecifications
                .Include(x => x.GenerationGroup.Users)
                .Include(x => x.ApprovalGroup.Users)
                .Include(x => x.SubmissionGroup.Users)
                .Where(x => x.FileNumber == "029").ProjectTo<FileSpecificationViewDto>().ToList();

            return Ok(dto);
        }
    }
}
