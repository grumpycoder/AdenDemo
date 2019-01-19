using System;
using System.Collections.Generic;

namespace AdenDemo.Web.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime? NextDueDate { get; set; }
        public int DataYear { get; set; }
        public bool IsSEA { get; set; }
        public bool IsLEA { get; set; }
        public bool IsSCH { get; set; }
        public SubmissionState SubmissionState { get; set; }
        public DateTime? LastUpdated { get; set; }
        public List<Report> Reports { get; set; }
        public List<SubmissionAudit> SubmissionAudits { get; set; }
        public byte[] SpecificationDocument { get; set; }
        public int FileSpecificationId { get; set; }
        public FileSpecification FileSpecification { get; set; }
        public string CurrentAssignee { get; set; }

        public Submission()
        {
            Reports = new List<Report>();
            SubmissionAudits = new List<SubmissionAudit>();
        }
    }
}
