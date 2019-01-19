namespace AdenDemo.Web.ViewModels
{
    public class SubmissionAuditEntryDto
    {
        public int SubmissionId { get; set; }
        public string Message { get; set; }

        public SubmissionAuditEntryDto(int submissionId)
        {
            SubmissionId = submissionId;
        }
    }
}
