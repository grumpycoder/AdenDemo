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
        public int? CurrentReportId { get; internal set; }

        public Submission()
        {
            Reports = new List<Report>();
            SubmissionAudits = new List<SubmissionAudit>();
        }

        public void Waive(string message, string userFullName)
        {
            SubmissionState = SubmissionState.Waived;
            LastUpdated = DateTime.Now;

            var report = new Report() { SubmissionId = Id, DataYear = DataYear, ReportState = ReportState.Waived };
            Reports.Add(report);

            CurrentReportId = report.Id;

            var msg = $"{userFullName} waived submission: {message}";
            var audit = new SubmissionAudit(Id, msg);
            SubmissionAudits.Add(audit);

        }

        public void Reopen(string currentUser, string message, string assignee, DateTime dueDate)
        {

            //Create Audit record
            var msg = $"{currentUser} reopened submission: { message }";
            var audit = new SubmissionAudit(Id, message);
            SubmissionAudits.Add(audit);

            //Create report
            var report = new Report() { SubmissionId = Id, DataYear = DataYear, ReportState = ReportState.AssignedForGeneration };
            Reports.Add(report);

            //Change state
            SubmissionState = SubmissionState.AssignedForGeneration;
            CurrentAssignee = assignee;
            LastUpdated = DateTime.Now;
            NextDueDate = dueDate;

            //Create work item
            var workItem = new WorkItem()
            {
                WorkItemAction = WorkItemAction.Generate,
                WorkItemState = WorkItemState.NotStarted,
                AssignedDate =  DateTime.Now, 
                AssignedUser = assignee
            };
            report.WorkItems.Add(workItem);

        }
    }
}
