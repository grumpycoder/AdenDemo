namespace AdenDemo.Web.ViewModels
{
    public class SubmissionWaiveAuditEntryDto
    {
        public int SubmissionId { get; set; }
        public string Message { get; set; }

        public SubmissionWaiveAuditEntryDto(int submissionId)
        {
            SubmissionId = submissionId;
        }

        private SubmissionWaiveAuditEntryDto()
        {
            
        }
    }
}
