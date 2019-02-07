using System.Data.Entity.ModelConfiguration;
using Aden.Core.Models;

namespace Aden.Core.Data.Configuration
{
    public class ReportConfiguration : EntityTypeConfiguration<Report>
    {
        public ReportConfiguration()
        {
            ToTable("Aden.Reports");
            Property(s => s.Id).HasColumnName("ReportId");
            Property(s => s.ReportState).HasColumnName("ReportStateId");
            Property(s => s.SubmittedUser).HasMaxLength(75);
            Property(s => s.ApprovedUser).HasMaxLength(75);
            Property(s => s.SubmittedUser).HasMaxLength(75);

        }
    }
}
