using System;
using System.Collections.Generic;

namespace AdenDemo.Web.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string AssignedUser { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Description { get; set; }
        public List<WorkItemImage> WorkItemImages { get; set; }
        public WorkItemAction WorkItemAction { get; set; }
        public WorkItemState WorkItemState { get; set; }
        public Report Report { get; set; }

        public WorkItem()
        {
            WorkItemImages = new List<WorkItemImage>();
        }

    }
}
