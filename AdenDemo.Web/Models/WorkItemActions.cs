using System.ComponentModel.DataAnnotations;

namespace AdenDemo.Web.Models
{
    public enum WorkItemAction
    {
        [Display(Name = "Generate File", ShortName = "Generate", Description = "Generate file")]
        Generate = 1,
        [Display(Name = "Accept File", ShortName = "Accept", Description = "Review file and accept")]
        Review = 2,
        [Display(Name = "Reject File", ShortName = "Reject", Description = "Reject File")]
        Reject = 7,
        [Display(Name = "Approve File", ShortName = "Approve", Description = "Approve File")]
        Approve = 3,
        [Display(Name = "File Submitted", ShortName = "SubmitFile")]
        Submit = 4,
        [Display(Name = "Submit With Error", ShortName = "SubmitErrorFile")]
        SubmitWithError = 5,
        [Display(Name = "Review Completed", ShortName = "SubmitErrorReview", Description = "Submission error reviewed")]
        ReviewError = 6,

        Nothing = 0
    }
}
