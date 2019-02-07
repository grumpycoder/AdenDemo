using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Aden.Core.ViewModels
{
    public class SubmissionErrorDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileNumber { get; set; }

        public string DisplayFileName => $"{FileName} ({FileNumber})";

        [Required]
        public string Description { get; set; }

        //public List<HttpPostedFileBase> Files { get; set; }

        public HttpPostedFileBase[] Files { get; set; }
    }
}
