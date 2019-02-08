using Aden.Web.Data;
using Aden.Web.Filters;
using Aden.Web.Helpers;
using Aden.Web.MailMessage;
using Aden.Web.Models;
using Aden.Web.Services;
using Aden.Web.ViewModels;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Aden.Web.Controllers
{
    [CustomAuthorize(Roles = "AdenAppUsers")]
    public class HomeController : Controller
    {

        private AdenContext _context;
        private MembershipService _membershipService;

        public HomeController()
        {
            _context = new AdenContext();
            _membershipService = new MembershipService(_context);
        }

        public ActionResult Submissions()
        {
            return View();
        }

        [CustomAuthorize(Roles = "AppGlobalAdministrator, AppAdministrators")]
        public ActionResult FileSpecifications()
        {
            return View();
        }

        public ActionResult Assignments()
        {
            return View();
        }

        public ActionResult History(int id)
        {
            var submission = _context.Submissions.FirstOrDefault(x => x.Id == id);
            var dto = new HistoryViewDto() { CurrentReportId = submission.CurrentReportId, SubmissionId = submission.Id };
            return PartialView("_History", dto);
        }

        public async Task<ActionResult> Review(int dataYear, string filenumber)
        {
            var dto = await _context.Reports
                .Where(f => (f.Submission.FileSpecification.FileNumber == filenumber &&
                             f.Submission.DataYear == dataYear) || string.IsNullOrEmpty(filenumber))
                .ProjectTo<ReportViewDto>().ToListAsync();

            //TODO: Move to mapping profile
            foreach (ReportViewDto item in dto)
            {
                item.Documents.ForEach(x =>
                {
                    x.FileSizeInMb = x.FileSize.ToFileSize();
                    x.FileSizeMb = x.FileSize / 1024;
                    x.FileSizeMb = x.FileSize.ConvertBytesToMega();
                });
            }

            var list = dto.SelectMany(x => x.Documents.Where(d => d.Version < x.CurrentDocumentVersion)).ToList();
            ViewBag.Old = list;
            return View(dto);
            ;
        }

        public ActionResult Waiver(int id)
        {
            var audit = new SubmissionWaiveAuditEntryDto(id);
            return PartialView("_SubmissionWaiverAuditEntry", audit);
        }

        public ActionResult ReOpen(int id)
        {
            var audit = new SubmissionReOpenAuditEntryDto(id);
            return PartialView("_SubmissionReOpenAuditEntry", audit);
        }

        public ActionResult Reassign(int id)
        {
            var workItem = _context.WorkItems.FirstOrDefault(x => x.Id == id);

            var dto = Mapper.Map<AssignmentDto>(workItem);
            return PartialView("_WorkItemAssignment", dto);
        }

        public ActionResult EditFileSpecification(int id)
        {
            var model = _context.FileSpecifications
                .Include(g => g.GenerationGroup.Users)
                .Include(g => g.ApprovalGroup.Users)
                .Include(g => g.SubmissionGroup.Users)
                .FirstOrDefault(x => x.Id == id);

            var dto = Mapper.Map<UpdateFileSpecificationDto>(model);

            //TODO: Check for null file specification

            var dataGroups = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = "Data", Text = "Data"},
                new SelectListItem(){ Value = "Development", Text = "Development"},
                new SelectListItem(){ Value = "Section", Text = "Section"}
            };

            var collections = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = "Accumulator", Text = "Accumulator"},
                new SelectListItem(){ Value = "Assessment", Text = "Assessment"},
                new SelectListItem(){ Value = "AnnualDataReport", Text = "Annual Data Report"},
                new SelectListItem(){ Value = "Application", Text = "Application"},
                new SelectListItem(){ Value = "ChildCount", Text = "Child Count"},
                new SelectListItem(){ Value = "Financials", Text = "Financials"},
                new SelectListItem(){ Value = "EOY-9thMonth", Text = "EOY - 9th Month"},
                new SelectListItem(){ Value = "Fall-20Day", Text = "Fall-20 Day"},
                new SelectListItem(){ Value = "Manual", Text = "Manual"},
                new SelectListItem(){ Value = "SIR", Text = "SIR"},
                new SelectListItem(){ Value = "Schedules", Text = "Schedules"}
            };

            var applications = new IdemService().GetApplication().ConvertAll(a => new SelectListItem() { Text = a.Title, Value = a.Title });

            ViewBag.Groups = _context.Groups.OrderBy(g => g.Name).ToList()
                .ConvertAll(a => new SelectListItem() { Text = a.Name, Value = a.Id.ToString() });


            ViewBag.Applications = applications;
            ViewBag.DataGroups = dataGroups;
            ViewBag.Collections = collections;
            return PartialView("_FileSpecificationEditForm", dto);
        }

        public async Task<ActionResult> ErrorReport(int id)
        {
            var dto = await _context.WorkItems.ProjectTo<SubmissionErrorDto>().FirstOrDefaultAsync(x => x.Id == id);

            return PartialView("_ErrorReportForm", dto);
        }

        public async Task<ActionResult> WorkItemImages(int id)
        {
            var wi = await _context.WorkItems.Include(x => x.WorkItemImages).FirstOrDefaultAsync(x => x.Id == id);

            return PartialView("_WorkItemImage", wi);
        }

        public ActionResult Document(int id)
        {
            ViewBag.Id = id;
            return PartialView("_Document");
        }

        public async Task<FileResult> Download(int id)
        {
            var document = await _context.ReportDocuments.FindAsync(id);
            return File(document.FileData, System.Net.Mime.MediaTypeNames.Application.Octet, document.Filename);
        }

        public async Task<object> ReportError(SubmissionErrorDto model)
        {
            //TODO: Will not work in WebApi. Convert to Webapi method /api/workitem/reporterror

            var id = model.Id;

            if (model.Files.Length == 0) ModelState.AddModelError("", "You must include at least 1 file");
            if (!ModelState.IsValid)
            {

                var errors = new List<string>();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var workItem = await _context.WorkItems.Include(x => x.Report).FirstOrDefaultAsync(x => x.Id == id);
            if (workItem == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var submission = _context.Submissions.Include(f => f.FileSpecification.GenerationGroup.Users).FirstOrDefault(s => s.Id == workItem.Report.SubmissionId);

            var assignedUser = _membershipService.GetAssignee(submission.FileSpecification.GenerationGroup);

            var wi = submission.CompleteWork(workItem, assignedUser, generateErrorTask: true);
            wi.Description = model.Description;

            foreach (var f in model.Files)
            {
                wi.WorkItemImages.Add(new WorkItemImage() { Image = f.ConvertToByte(), });
            }

            await _context.SaveChangesAsync();

            return new HttpStatusCodeResult(HttpStatusCode.OK);

        }

        public ActionResult EditGroupMembership(int id)
        {
            var dto = _context.Groups.Include(u => u.Users).FirstOrDefault(x => x.Id == id);
            return PartialView("_GroupMembershipEditForm", dto);
        }

        public async Task<ActionResult> UploadReport(int id)
        {
            var wi = await _context.WorkItems.Include(r => r.Report.Submission.FileSpecification).FirstOrDefaultAsync(x => x.Id == id);

            var model = Mapper.Map<WorkItemUploadDto>(wi);
            return PartialView("_ReportUploadForm", model);
        }

        [TrackViewName]
        public ActionResult Mail()
        {
            MimeReader mime = new MimeReader();     

            var vm = new List<MailViewModel>();
            var path = HostingEnvironment.MapPath(@"/App_Data");
            foreach (var file in Directory.GetFiles($@"{path}", "*.eml"))
            {
                RxMailMessage msg = mime.GetEmail(file);


                vm.Add(new MailViewModel()
                {
                    Id = Path.GetFileNameWithoutExtension(file),
                    Sent = msg.DeliveryDate,
                    To = msg.To.Select(s => s.Address.ToString()),
                    CC = msg.CC.Select(s => s.Address.ToString()),

                    From = msg.From.Address,
                    Subject = msg.Subject.Replace("(Trial Version)", ""),
                    Body = msg.Body,
                    Attachments = msg.Attachments.ToList()
                });
            }

            return View(vm.OrderByDescending(x => x.Sent));
        }



        private string GetPlainText(RxMailMessage mm)
        {
            // check for plain text in body
            if (!mm.IsBodyHtml && !string.IsNullOrEmpty(mm.Body))
                return mm.Body;

            string sText = string.Empty;
            foreach (AlternateView av in mm.AlternateViews)
            {
                // check for plain text
                if (string.Compare(av.ContentType.MediaType, "text/plain", true) == 0)
                    continue;// return StreamToString(av.ContentStream);

                // check for HTML text
                if (string.Compare(av.ContentType.MediaType, "text/html", true) == 0)
                    sText = StreamToString(av.ContentStream);
            }

            // HTML is our only hope
            if (sText == string.Empty && mm.IsBodyHtml && !string.IsNullOrEmpty(mm.Body))
                sText = mm.Body;

            if (sText == string.Empty)
                return string.Empty;

            // need to convert the HTML to plaintext
            return sText;
        }

        private static string StreamToString(Stream stream)
        {
            string sText = string.Empty;
            using (StreamReader sr = new StreamReader(stream))
            {
                sText = sr.ReadToEnd();
                stream.Seek(0, SeekOrigin.Begin);   // leave the stream the way we found it
                stream.Close();
            }

            return sText;
        }
    }
}