using System.ComponentModel.DataAnnotations;

namespace AdenDemo.Web.ViewModels
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

        [Display(Name = "Generation User Group")]
        public string GenerationUserGroup { get; set; }
        [Display(Name = "Approval User Group")]
        public string ApprovalUserGroup { get; set; }
        [Display(Name = "Submission User Group")]
        public string SubmissionUserGroup { get; set; }

        [Display(Name = "Filename Format")]
        public string FileNameFormat { get; set; }

        [Display(Name = "Report Action")]
        public string ReportAction { get; set; }

    }
}
