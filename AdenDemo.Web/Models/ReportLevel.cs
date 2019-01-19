using System.ComponentModel.DataAnnotations;

namespace AdenDemo.Web.Models
{
    public enum ReportLevel
    {
        [Display(Name = "SEA")]
        SEA = 1,
        [Display(Name = "LEA")]
        LEA = 2,
        [Display(Name = "SCH")]
        SCH = 3
    }
}
