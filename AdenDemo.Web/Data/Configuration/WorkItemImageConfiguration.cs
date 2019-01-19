using AdenDemo.Web.Models;
using System.Data.Entity.ModelConfiguration;

namespace AdenDemo.Web.Data.Configuration
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
