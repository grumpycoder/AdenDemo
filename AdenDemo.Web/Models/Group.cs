using System.Collections.Generic;

namespace Aden.Web.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<UserProfile> Users { get; set; }

    }
}
