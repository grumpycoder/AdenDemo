namespace AdenDemo.Web.Models
{
    public class ReportDocument
    {
        public int Id { get; set; }
        public ReportLevel ReportLevel { get; set; }
        public byte[] FileData { get; set; }
        public string Filename { get; set; }
        public int Version { get; set; }
        public int ReportId { get; set; }
        public Report Report { get; set; }

        public long FileSize { get; set; }

        public ReportDocument()
        {

        }
    }
}
