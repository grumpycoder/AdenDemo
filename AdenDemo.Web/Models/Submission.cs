using System;
using System.Collections.Generic;
using System.Linq;

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
        public int CurrentReportId { get; internal set; }

        public Submission()
        {
            Reports = new List<Report>();
            SubmissionAudits = new List<SubmissionAudit>();
        }

        public void Waive(string message, string userFullName)
        {
            SubmissionState = SubmissionState.Waived;
            LastUpdated = DateTime.Now;

            var report = new Report
            {
                SubmissionId = Id,
                DataYear = DataYear,
                ReportState = ReportState.Waived
            };
            Reports.Add(report);

            //TODO: ReportId not available yet
            CurrentReportId = 0;

            var msg = $"{userFullName} waived submission: {message}";
            var audit = new SubmissionAudit(Id, msg);
            SubmissionAudits.Add(audit);

        }

        public void Reopen(string currentUser, string message, string assignee, DateTime dueDate)
        {

            //Create Audit record
            var msg = $"{currentUser} reopened submission: { message }";
            var audit = new SubmissionAudit(Id, msg);
            SubmissionAudits.Add(audit);

            //Create report
            var report = new Report
            {
                SubmissionId = Id,
                DataYear = DataYear,
                ReportState = ReportState.AssignedForGeneration
            };
            Reports.Add(report);

            //Change state
            SubmissionState = SubmissionState.AssignedForGeneration;
            CurrentAssignee = assignee;
            LastUpdated = DateTime.Now;
            NextDueDate = dueDate;
            CurrentReportId = report.Id;

            //Create work item
            var workItem = new WorkItem()
            {
                WorkItemAction = WorkItemAction.Generate,
                WorkItemState = WorkItemState.NotStarted,
                AssignedDate = DateTime.Now,
                AssignedUser = assignee
            };
            report.WorkItems.Add(workItem);

        }

        public void Cancel(string currentUser)
        {

            //Set Submission State and clear assignee
            SubmissionState = SubmissionState.NotStarted;
            CurrentAssignee = string.Empty;

            var lastReport = Reports.LastOrDefault();

            if (lastReport != null)
            {
                CurrentReportId = lastReport.Id;

                if (lastReport.ReportState == ReportState.Waived) SubmissionState = SubmissionState.Waived;
            }


            //Create Audit record
            var msg = $"{currentUser} cancelled submission";
            var audit = new SubmissionAudit(Id, msg);
            SubmissionAudits.Add(audit);



        }

        public void Start(string assignee)
        {
            //Change state
            SubmissionState = SubmissionState.AssignedForGeneration;
            CurrentAssignee = assignee;
            LastUpdated = DateTime.Now;

            //Create report
            var report = new Report() { SubmissionId = Id, DataYear = DataYear, ReportState = ReportState.AssignedForGeneration };
            Reports.Add(report);

            //Create work item
            var workItem = new WorkItem()
            {
                WorkItemAction = WorkItemAction.Generate,
                WorkItemState = WorkItemState.NotStarted,
                AssignedDate = DateTime.Now,
                AssignedUser = assignee
            };
            report.WorkItems.Add(workItem);

        }
    }
}
