using System.ComponentModel.DataAnnotations;

namespace AdenDemo.Web.Models
{
    public enum WorkItemAction
    {
        [Display(Name = "Generate", ShortName = "Generate File", Description = "Generate File")]
        Generate = 1,
        [Display(Name = "Accept", ShortName = "Accept File", Description = "Review File")]
        Review = 2,
        //[Display(Name = "Reject File", ShortName = "Reject", Description = "Reject File")]
        //Reject = 7,
        [Display(Name = "Approve", ShortName = "Approve File", Description = "Approve File")]
        Approve = 3,
        [Display(Name = "Submit", ShortName = "Submit File", Description = "Submit File")]
        Submit = 4,

        //[Display(Name = "Submit With Error", ShortName = "SubmitErrorFile", Description = "Submit Error File")]
        //SubmitWithError = 5,
        [Display(Name = "Review Completed", ShortName = "Review Error", Description = "Submission Error Review")]
        ReviewError = 6,

        //[Display(Name = "Completed", ShortName = "Completed", Description = "Submission Complete")]
        //Nothing = 0
    }
}
