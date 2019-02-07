using System.Data.Entity.ModelConfiguration;
using Aden.Core.Models;

namespace Aden.Core.Data.Configuration
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
