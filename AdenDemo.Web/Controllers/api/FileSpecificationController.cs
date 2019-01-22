using AdenDemo.Web.Data;
using AdenDemo.Web.ViewModels;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;

namespace AdenDemo.Web.Controllers.api
{
    [RoutePrefix("api/filespecification")]
    public class FileSpecificationController : ApiController
    {
        private AdenContext _context;
        public FileSpecificationController()
        {
            _context = new AdenContext();
        }

        [HttpGet]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            var dto = await _context.FileSpecifications.ProjectTo<FileSpecificationViewDto>().ToListAsync();

            //TODO: Move closer to mapping
            //foreach (var d in dto)
            //{
            //    d.GenerationUserGroup = d.GenerationUserGroup.Humanize(LetterCasing.Title);
            //    d.ApprovalUserGroup = d.ApprovalUserGroup.Humanize(LetterCasing.Title);
            //    d.SubmissionUserGroup = d.SubmissionUserGroup.Humanize(LetterCasing.Title);
            //}
            return Ok(DataSourceLoader.Load(dto, loadOptions));
        }

        [HttpPut, Route("{id}")]
        public object Post(int id, UpdateFileSpecificationDto dto)
        {
            var model = _context.FileSpecifications.Find(id);

            if (model == null) return NotFound();

            Mapper.Map(dto, model);

            _context.SaveChanges();

            return Ok(dto);
        }
    }
}
