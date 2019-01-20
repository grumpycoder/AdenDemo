using AdenDemo.Web.Models;
using System.Collections.Generic;

namespace AdenDemo.Web.ViewModels
{
    public class HistoryViewDto
    {
        public IList<WorkItemHistoryDto> WorkItemHistory { get; set; }
        public IList<SubmissionAudit> SubmissionAudits { get; set; }

    }
}
