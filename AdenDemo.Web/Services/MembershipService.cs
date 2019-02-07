using AdenDemo.Web.Data;
using AdenDemo.Web.Models;
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


            //var nextAvailable = _context.WorkItems.AsNoTracking()
            //    .Where(u => members.Contains(u.AssignedUser.EmailAddress)).ToList()
            //    .GroupBy(u => u.AssignedUser.EmailAddress).Select(n => new
            //    {
            //        n.Key,
            //        Count = n.Count()
            //    }).OrderBy(x => x.Count).FirstOrDefault();


            return nextAvailable == null ? null : group.Users.FirstOrDefault(x => x.EmailAddress == nextAvailable.Key);

        }

        public Result<List<string>> GetGroupMembers(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName)) return Result.Fail<List<string>>("Group name should not be empty");

            var groupService = new IdemGroupService();
            var members = groupService.GetGroupUsers(groupName);


            if (members == null) return Result.Fail<List<string>>($"No members defined in group {groupName}");

            var list = members.Select(m => m.EmailAddress).ToList();
            return Result.Ok(list);
        }

        public bool GroupExists(string groupName)
        {
            var groupService = new IdemGroupService();
            return groupService.GroupExists(groupName);

        }
    }
}
