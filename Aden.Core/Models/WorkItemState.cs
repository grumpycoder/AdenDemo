using System.ComponentModel.DataAnnotations;

namespace Aden.Core.Models
{
    public enum WorkItemState
    {
        [Display(Name = "Not Started", ShortName = "Not Started", Description = "Not Started")]
        NotStarted = 1,
        [Display(Name = "Cancelled", ShortName = "Cancelled", Description = "Cancelled")]
        Cancelled = 2,
        [Display(Name = "Complete", ShortName = "Complete", Description = "Complete")]
        Completed = 3,
        [Display(Name = "Reassigned", ShortName = "Reassigned", Description = "Reassigned")]
        Reassigned = 4,
        [Display(Name = "Reject", ShortName = "Reject", Description = "Reject File")]
        Reject = 5,
    }
}
