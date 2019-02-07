using System;
using System.Collections.Generic;

namespace Aden.Web.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public UserProfile AssignedUser { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Description { get; set; }
        public List<WorkItemImage> WorkItemImages { get; set; }
        public WorkItemAction WorkItemAction { get; set; }
        public WorkItemState WorkItemState { get; set; }
        public Report Report { get; set; }

        public int AssignedUserId { get; set; }

        public WorkItem()
        {
            WorkItemImages = new List<WorkItemImage>();
        }

        public WorkItem DeepCopy()
        {
            WorkItem other = (WorkItem)this.MemberwiseClone();
            other.AssignedUser = AssignedUser;

            return other;
        }

    }
}
