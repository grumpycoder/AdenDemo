using System.Data.Entity.ModelConfiguration;
using Aden.Core.Models;

namespace Aden.Core.Data.Configuration
{
    public class SubmissionAuditConfiguration : EntityTypeConfiguration<SubmissionAudit>
    {
        public SubmissionAuditConfiguration()
        {
            ToTable("Aden.SubmissionAudits");
            Property(s => s.Id).HasColumnName("SubmissionAuditId");
        }
    }
}
