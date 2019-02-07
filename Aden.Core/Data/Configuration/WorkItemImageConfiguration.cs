using System.Data.Entity.ModelConfiguration;
using Aden.Core.Models;

namespace Aden.Core.Data.Configuration
{
    public class WorkItemImageConfiguration : EntityTypeConfiguration<WorkItemImage>
    {
        public WorkItemImageConfiguration()
        {
            ToTable("Aden.WorkItemImages");
            Property(s => s.Id).HasColumnName("WorkItemImageId");
        }
    }
}
