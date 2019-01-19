using AdenDemo.Web.Models;
using System.Data.Entity.ModelConfiguration;

namespace AdenDemo.Web.Data.Configuration
{
    public class SubmissionConfiguration : EntityTypeConfiguration<Submission>
    {
        public SubmissionConfiguration()
        {
            ToTable("Aden.Submissions");
            Property(s => s.Id).HasColumnName("SubmissionId");
            Property(s => s.SubmissionState).HasColumnName("SubmissionStateId");
        }
    }
}
