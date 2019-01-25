using AdenDemo.Web.Helpers;
using AdenDemo.Web.Models;
using System;
using System.Collections.Generic;

namespace AdenDemo.Web.ViewModels
{
    public class ReportViewDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileNumber { get; set; }
        public string DisplayFileName => $"{FileName} ({FileNumber})";

        public int DataYear { get; set; }

        public string DisplayDataYear => $"{DataYear - 1}-{DataYear}";

        public ReportState ReportState { get; set; }
        public string ReportStateDisplay => ReportState.GetDisplayName();
        public DateTime? GeneratedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }

        public List<DocumentViewDto> Documents { get; set; }
    }
}
