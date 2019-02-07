using System;
using System.Collections.Generic;

namespace Aden.Core.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public Guid IdentityGuid { get; set; }

        public List<Group> Groups { get; set; }


    }
}
