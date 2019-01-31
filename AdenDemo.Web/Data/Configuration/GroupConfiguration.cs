using AdenDemo.Web.Models;
using System.Data.Entity.ModelConfiguration;

namespace AdenDemo.Web.Data.Configuration
{
    public class GroupConfiguration : EntityTypeConfiguration<Group>
    {
        public GroupConfiguration()
        {
            ToTable("Groups", "Aden");
            Property(s => s.Id).HasColumnName("GroupId");
            Property(s => s.Name).HasColumnName("GroupName");

            HasMany<UserProfile>(s => s.Users)
                .WithMany(c => c.Groups)
                .Map(x =>
                {
                    x.MapLeftKey("GroupId");
                    x.MapRightKey("UserProfileId");
                    x.ToTable("GroupUserProfiles", "Aden");
                });


        }
    }
}
