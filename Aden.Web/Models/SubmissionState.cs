using System.ComponentModel.DataAnnotations;

namespace Aden.Web.Models
{
    public enum SubmissionState : byte
    {
        [Display(Name = "Not Started", ShortName = "NotStarted", Description = "Not Started")]
        NotStarted = 1,
        [Display(Name = "Assigned for Generation", ShortName = "AssignedForGenerate", Description = "Assigned for Generate")]
        AssignedForGeneration = 2,
        [Display(Name = "Assigned for Review", ShortName = "AssignedForReview", Description = "Assigned for Review")]
        AssignedForReview = 3,
        [Display(Name = "Assigned for Approval", ShortName = "AssignedForApprove", Description = "Assigned for Approval")]
        AwaitingApproval = 4,
        [Display(Name = "Assigned for Submission", ShortName = "AssignedForSubmit", Description = "Assigned for Submission")]
        AssignedForSubmission = 5,
        [Display(Name = "Assigned for Error Review", ShortName = "CompleteWithErrors", Description = "Completed with Errors")]
        CompleteWithError = 6,
        [Display(Name = "Completed", ShortName = "Completed", Description = "Completed")]
        Complete = 7,
        [Display(Name = "Waived", ShortName = "Waived", Description = "Waived")]
        Waived = 8,
    }
}
