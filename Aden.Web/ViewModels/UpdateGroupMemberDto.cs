using System;

namespace Aden.Web.ViewModels
{
    public class UpdateGroupMemberDto
    {
        public int GroupId { get; set; }
        public Guid IdentityGuid { get; set; }
    }
}