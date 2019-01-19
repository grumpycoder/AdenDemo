using AdenDemo.Web.Models;
using System.Data.Entity.ModelConfiguration;

namespace AdenDemo.Web.Data.Configuration
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
