using Aden.Core.Helpers;
using Aden.Core.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Aden.Core.ViewModels
{
    public class WorkItemHistoryDto
    {
        public int Id { get; set; }
        public WorkItemAction Action { get; set; }
        public string ActionDescription => Action.GetDisplayName();
        public string Status => WorkItemState.GetDisplayName();
        public DateTime? AssignedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string AssignedUser { get; set; }
        public WorkItemState WorkItemState { get; set; }
        public string Description { get; set; }

        public bool CanReassign
        {
            get
            {
                return ((HttpContext.Current.User as ClaimsPrincipal).Claims.Any(c => c.Value == Constants.GlobalAdministrators)) && (WorkItemState == WorkItemState.NotStarted || WorkItemState == WorkItemState.Reassigned);
            }
        }

        public bool CanReviewError => Action == WorkItemAction.ReviewError;
    }
}
