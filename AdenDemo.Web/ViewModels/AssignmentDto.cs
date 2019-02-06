using System;

namespace AdenDemo.Web.ViewModels
{
    public class AssignmentDto
    {
        public int WorkItemId { get; set; }
        public string AssignedUser { get; set; }
        public string WorkItemAction { get; set; }
        public string Reason { get; set; }

        public Guid IdentityGuid { get; set; }
    }
}
