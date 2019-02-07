using Aden.Web.Models;
using System.Collections.Generic;

namespace AdenDemo.Web.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<UserProfile> Users { get; set; }

    }
}
