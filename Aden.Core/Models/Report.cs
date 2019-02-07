using System;
using System.Collections.Generic;

namespace Aden.Core.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int? DataYear { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string GeneratedUser { get; set; }
        public string ApprovedUser { get; set; }
        public string SubmittedUser { get; set; }
        public List<ReportDocument> Documents { get; set; }
        public int SubmissionId { get; set; }
        public int? CurrentDocumentVersion { get; set; }
        public Submission Submission { get; set; }
        public ReportState ReportState { set; get; }
        public List<WorkItem> WorkItems { set; get; }

        public Report()
        {
            WorkItems = new List<WorkItem>();
            Documents = new List<ReportDocument>();
        }
    }
}
