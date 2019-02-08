using Aden.Web.Data;
using Aden.Web.Models;
using ALSDE.Services;
using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Aden.Web.Services
{
    public class MembershipService
    {
        private readonly AdenContext _context;

        public MembershipService(AdenContext context)
        {
            _context = new AdenContext();
        }

        public UserProfile GetAssignee(Group group)
        {
            if (!group.Users.Any()) return null;

            var members = group.Users.Select(x => x.EmailAddress);

            var currentWorkItems = _context.WorkItems.AsNoTracking()
                .Include(x => x.AssignedUser)
                .Where(x => x.WorkItemState == WorkItemState.NotStarted).ToList();

            var alreadyAssignedMembers = currentWorkItems
                .Where(u => members.Contains(u.AssignedUser.EmailAddress))
                .ToLookup(m => m.AssignedUser.EmailAddress);

            var firstAvailableMember = members.FirstOrDefault(x => !alreadyAssignedMembers.Contains(x));

            if (firstAvailableMember != null)
            {
                var e = group.Users.FirstOrDefault(x => x.EmailAddress == firstAvailableMember);
                return e;
            }

            var nextAvailable = currentWorkItems
                .Where(u => members.Contains(u.AssignedUser.EmailAddress)).ToList()
                .GroupBy(u => u.AssignedUser.EmailAddress).Select(n => new
                {
                    n.Key,
                    Count = n.Count()
                }).OrderBy(x => x.Count).FirstOrDefault();

            return nextAvailable == null ? null : group.Users.FirstOrDefault(x => x.EmailAddress == nextAvailable.Key);

        }

        public List<UserProfile> GetGroupMembers(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName)) return null;

            var group = _context.Groups.Include(u => u.Users).FirstOrDefault(x => x.Name == groupName);

            var users = group.Users;

            return users;
        }

        public bool GroupExists(string groupName)
        {
            var groupService = new IdemGroupService();
            return groupService.GroupExists(groupName);

        }

        public List<Group> GetUserGroups(string username)
        {
            var _user = _context.Users.Include(x => x.Groups).FirstOrDefault(x => x.EmailAddress == username);

            return _user.Groups;
        }

    }
}
