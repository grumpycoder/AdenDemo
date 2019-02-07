using AdenDemo.Web.Models;
using System.Data.Entity.ModelConfiguration;

namespace Aden.Web.Data.Configuration
{
    public class FileSpecificationConfiguration : EntityTypeConfiguration<FileSpecification>
    {
        public FileSpecificationConfiguration()
        {
            ToTable("Aden.FileSpecifications");
            Property(s => s.Id).HasColumnName("FileSpecificationId");
            Property(s => s.FileNumber).HasMaxLength(8);
            Property(s => s.FileName).HasMaxLength(250);

            HasOptional(x => x.GenerationGroup).WithMany().HasForeignKey(x => x.GenerationGroupId);
            HasOptional(x => x.ApprovalGroup).WithMany().HasForeignKey(x => x.ApprovalGroupId);
            HasOptional(x => x.SubmissionGroup).WithMany().HasForeignKey(x => x.SubmissionGroupId);
        }


    }

}
