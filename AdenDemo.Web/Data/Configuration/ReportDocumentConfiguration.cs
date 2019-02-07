using AdenDemo.Web.Models;
using System.Data.Entity.ModelConfiguration;

namespace Aden.Web.Data.Configuration
{
    public class ReportDocumentConfiguration : EntityTypeConfiguration<ReportDocument>
    {
        public ReportDocumentConfiguration()
        {
            ToTable("Aden.ReportDocuments");
            Property(s => s.Id).HasColumnName("ReportDocumentId");
            Property(s => s.ReportLevel).HasColumnName("ReportLevelId");
        }
    }
}
