using AdenDemo.Web.Data;
using AdenDemo.Web.Models;
using AdenDemo.Web.ViewModels;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Humanizer;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Z.EntityFramework.Plus;

namespace AdenDemo.Web.Controllers.api
{
    [RoutePrefix("api/filespecification")]
    [Authorize(Roles = "AdenAppUsers")]
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
            //    d.GenerationUserGroup = d.GenerationUserGroup?.Humanize(LetterCasing.Title);
            //    d.ApprovalUserGroup = d.ApprovalUserGroup?.Humanize(LetterCasing.Title);
            //    d.SubmissionUserGroup = d.SubmissionUserGroup?.Humanize(LetterCasing.Title);
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

        [HttpPost, Route("activate/{id}")]
        public async Task<object> Activate(int id)
        {
            var fileSpecification = await _context.FileSpecifications.FindAsync(id);

            if (fileSpecification == null) return NotFound();


            fileSpecification.IsRetired = false;

            //Create submission record
            var submission = new Submission()
            {
                DueDate = fileSpecification.DueDate,
                DataYear = fileSpecification.DataYear,
                IsLEA = fileSpecification.IsLEA,
                IsSEA = fileSpecification.IsSEA,
                IsSCH = fileSpecification.IsSCH,
                SubmissionState = SubmissionState.NotStarted
            };

            fileSpecification.Submissions.Add(submission);

            await _context.SaveChangesAsync();

            var dto = Mapper.Map<FileSpecificationViewDto>(fileSpecification);
            return Ok(dto);
        }

        [HttpPost, Route("retire/{id}")]
        public async Task<object> Retire(int id)
        {
            var fileSpecification = await _context.FileSpecifications.FindAsync(id);

            if (fileSpecification == null) return NotFound();

            fileSpecification.IsRetired = true;

            //Delete Submissions and child records if Submission State is not completed
            var docs = _context.ReportDocuments.Where(d =>
                d.Report.ReportState < ReportState.CompleteWithError && d.Report.Submission.FileSpecificationId == id).Delete();

            var wi = _context.WorkItems.Where(w =>
                w.Report.Submission.FileSpecificationId == id &&
                w.Report.ReportState < ReportState.CompleteWithError).Delete();

            var reports = _context.Reports.Where(r =>
                r.Submission.FileSpecificationId == id &&
                r.Submission.SubmissionState < SubmissionState.CompleteWithError).Delete();

            var submissions = _context.Submissions.Where(s =>
                s.FileSpecificationId == id && s.SubmissionState < SubmissionState.CompleteWithError).Delete();


            _context.SaveChanges();

            var dto = Mapper.Map<FileSpecificationViewDto>(fileSpecification);

            return Ok(dto);
        }


    }
}
