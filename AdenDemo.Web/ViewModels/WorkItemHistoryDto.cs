using AdenDemo.Web.Helpers;
using AdenDemo.Web.Models;
using System;

namespace AdenDemo.Web.ViewModels
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

        public bool CanReassign { get; set; }

        public bool CanReviewError => Action == WorkItemAction.ReviewError; 
    }
}
