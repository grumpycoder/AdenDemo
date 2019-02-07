using System.Data.Entity.ModelConfiguration;
using Aden.Core.Models;

namespace Aden.Core.Data.Configuration
{
    public class SubmissionConfiguration : EntityTypeConfiguration<Submission>
    {
        public SubmissionConfiguration()
        {
            ToTable("Aden.Submissions");
            Property(s => s.Id).HasColumnName("SubmissionId");
            Property(s => s.SubmissionState).HasColumnName("SubmissionStateId");
            Property(s => s.CurrentAssigneeId).HasColumnName("CurrentAssigneeProfileId");

        }

    }
}
