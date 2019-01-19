using System.ComponentModel.DataAnnotations;

namespace AdenDemo.Web.Models
{
    public enum SubmissionState : byte
    {
        [Display(Name = "Not Started", ShortName = "NotStarted", Description = "Not Started")]
        NotStarted = 1,
        [Display(Name = "Assigned for Generate", ShortName = "AssignedForGenerate", Description = "Assigned for Generate")]
        AssignedForGeneration = 2,
        [Display(Name = "Assigned for Review", ShortName = "AssignedForReview", Description = "Assigned for Review")]
        AssignedForReview = 3,
        [Display(Name = "Assigned for Approve", ShortName = "AssignedForApprove", Description = "Assigned for Approval")]
        AwaitingApproval = 4,
        [Display(Name = "Assigned for Submit", ShortName = "AssignedForSubmit", Description = "Assigned for Submission")]
        AssignedForSubmission = 5,
        [Display(Name = "Complete with Errors", ShortName = "CompleteWithErrors", Description = "Completed with Errors")]
        CompleteWithError = 6,
        [Display(Name = "Completed", ShortName = "Completed", Description = "Completed")]
        Complete = 7,
        [Display(Name = "Waived", ShortName = "Waived", Description = "Waived")]
        Waived = 8,
    }
}
