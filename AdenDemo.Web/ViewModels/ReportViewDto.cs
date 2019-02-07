using AdenDemo.Web.Helpers;
using AdenDemo.Web.Models;
using System;
using System.Collections.Generic;

namespace Aden.Web.ViewModels
{
    public class ReportViewDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileNumber { get; set; }
        public string DisplayFileName => $"{FileName} ({FileNumber})";

        public int DataYear { get; set; }

        public DateTime SubmissionDueDate { get; set; }

        public string DisplayDataYear => $"{DataYear - 1}-{DataYear}";

        public ReportState ReportState { get; set; }
        public string ReportStateDisplay => ReportState.GetDisplayName();
        public DateTime? GeneratedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }

        public List<DocumentViewDto> Documents { get; set; }

        public int CurrentDocumentVersion { get; set; }
        public string PanelClass
        {
            get
            {
                {
                    var panelClass = "danger";
                    switch (ReportState)
                    {
                        case ReportState.AssignedForReview:
                            panelClass = "info";
                            break;
                        case ReportState.AwaitingApproval:
                            panelClass = "warning";
                            break;
                        case ReportState.CompleteWithError:
                            panelClass = "danger";
                            break;
                        case ReportState.Complete:
                            panelClass = "success";
                            break;
                        default:
                            break;
                    }
                    return panelClass;
                }
            }
        }
    }
}
