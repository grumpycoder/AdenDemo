namespace AdenDemo.Web.ViewModels
{
    public class FileSpecificationViewDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileNumber { get; set; }

        public bool? IsRetired { get; set; }
        public string FileNameFormat { get; set; }
        public string ReportAction { get; set; }
        public int? DataYear { get; set; }
        public string DisplayDataYear => $"{DataYear - 1}-{DataYear}";

        public string Section { get; set; }
        public string DataGroups { get; set; }
        public string Application { get; set; }
        public string Collection { get; set; }
        public string SupportGroup { get; set; }


        public string GenerationUserGroup { get; set; }
        public string ApprovalUserGroup { get; set; }
        public string SubmissionUserGroup { get; set; }

        public bool CanRetire => (bool)(IsRetired.HasValue ? !IsRetired : true);
        public bool CanActivate => !CanRetire;
    }
}
