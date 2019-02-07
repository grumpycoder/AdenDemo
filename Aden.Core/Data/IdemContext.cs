using System.Data.Entity;
using System.Diagnostics;

namespace Aden.Core.Data
{
    public class IdemContext : DbContext
    {
        public IdemContext()
            : base("IdemContext")
        {
            Database.Log = msg => Debug.WriteLine(msg);
            Database.SetInitializer<AdenContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

        }
    }
}
