using System.Data.Entity.ModelConfiguration;
using Aden.Core.Models;

namespace Aden.Core.Data.Configuration
{
    public class UserProfileConfiguration : EntityTypeConfiguration<UserProfile>
    {
        public UserProfileConfiguration()
        {
            ToTable("UserProfiles", "Aden");
            Property(s => s.Id).HasColumnName("UserProfileId");

            HasMany<Group>(s => s.Groups)
                .WithMany(c => c.Users)
                .Map(x =>
                {
                    x.MapLeftKey("UserProfileId");
                    x.MapRightKey("GroupId");
                    x.ToTable("GroupUserProfiles", "Aden");
                });



        }
    }
}
