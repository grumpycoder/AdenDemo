using System.ComponentModel.DataAnnotations;

namespace AdenDemo.Web.Models
{
    public enum ReportState : byte
    {
        [Display(Name = "Not Started", ShortName = "NotStarted")]
        NotStarted = 1,
        [Display(Name = "Assigned for Generate", ShortName = "AssignedForGenerate")]
        AssignedForGeneration = 2,
        [Display(Name = "Assigned for Review", ShortName = "AssignedForReview")]
        AssignedForReview = 3,
        [Display(Name = "Assigned for Approve", ShortName = "AssignedForApprove")]
        AwaitingApproval = 4,
        [Display(Name = "Assigned for Submit", ShortName = "AssignedForSubmit")]
        AssignedForSubmission = 5,
        [Display(Name = "Complete with Errors", ShortName = "CompleteWithErrors")]
        CompleteWithError = 6,
        [Display(Name = "Completed", ShortName = "Completed")]
        Complete = 7,
        [Display(Name = "Waived", ShortName = "Waived")]
        Waived = 8,
    }
}
