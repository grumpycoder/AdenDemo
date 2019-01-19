using System;

namespace AdenDemo.Web.Models
{
    public class SubmissionAudit
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }

        public DateTime AuditDate { get; set; }
        public string Message { get; set; }

        public SubmissionAudit(int submissionId, string message)
        {
            SubmissionId = submissionId;
            AuditDate = DateTime.Now;
            Message = message;
        }

        private SubmissionAudit()
        {

        }
    }
}
