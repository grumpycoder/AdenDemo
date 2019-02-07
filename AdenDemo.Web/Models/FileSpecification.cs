using System;
using System.Collections.Generic;

namespace Aden.Web.Models
{
    public class FileSpecification
    {
        public int Id { get; set; }
        public string FileNumber { get; set; }
        public string FileName { get; set; }
        public bool? IsRetired { get; set; }
        public string FileNameFormat { get; set; }
        public string ReportAction { get; set; }
        public int DataYear { get; set; }
        public string DataGroups { get; set; }
        public string Application { get; set; }
        public string Collection { get; set; }
        public string SupportGroup { get; set; }

        public string Section { get; set; }
        public string GenerationUserGroup { get; set; }
        public string ApprovalUserGroup { get; set; }
        public string SubmissionUserGroup { get; set; }

        public int? GenerationGroupId { get; set; }
        public int? ApprovalGroupId { get; set; }
        public int? SubmissionGroupId { get; set; }

        public Group GenerationGroup { get; set; }
        public Group ApprovalGroup { get; set; }
        public Group SubmissionGroup { get; set; }

        public bool IsSEA { get; set; }
        public bool IsLEA { get; set; }
        public bool IsSCH { get; set; }
        public DateTime DueDate { get; set; }

        public string FileDisplayName => $"{FileName} ({FileNumber})";

        public List<Submission> Submissions { get; set; }

        private FileSpecification()
        {
            Submissions = new List<Submission>();
        }
        public override string ToString()
        {
            return $"{FileNumber} {FileName}";
        }



    }
}
