using AdenDemo.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace Aden.Web.ViewModels
{
    public class UpdateFileSpecificationDto
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "File Number")]
        public string FileNumber { get; set; }
        [Display(Name = "File Name"), Required]
        public string FileName { get; set; }

        public string Section { get; set; }
        [Display(Name = "Data Groups")]
        public string DataGroups { get; set; }
        public string Application { get; set; }
        public string Collection { get; set; }
        public string SupportGroup { get; set; }


        [Display(Name = "Filename Format")]
        public string FileNameFormat { get; set; }

        [Display(Name = "Report Action")]
        public string ReportAction { get; set; }


        public Group GenerationGroup { get; set; }

        public Group ApprovalGroup { get; set; }
        public Group SubmissionGroup { get; set; }

        public int? GenerationGroupCount { get; set; }
        public int? ApprovalGroupCount { get; set; }
        public int? GenerationGroupId { get; set; }
        public int? ApprovalGroupId { get; set; }
        public int? SubmissionGroupId { get; set; }
    }
}
