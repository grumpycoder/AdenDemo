using AdenDemo.Web.Data;
using ALSDE.Services;
using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Linq;

namespace AdenDemo.Web.Services
{
    public class MembershipService
    {
        private readonly AdenContext _context;

        public MembershipService(AdenContext context)
        {
            _context = new AdenContext();
        }

        public string GetAssignee(string groupName)
        {
            var members = GetGroupMembers(groupName).Value;

            var alreadyAssignedMembers = _context.WorkItems.AsNoTracking()
                .Where(u => members.Contains(u.AssignedUser))
                .ToLookup(m => m.AssignedUser);

            var firstAvailableMember = members.FirstOrDefault(x => !alreadyAssignedMembers.Contains(x));

            if (firstAvailableMember != null) return firstAvailableMember;

            var nextAvailable = _context.WorkItems.AsNoTracking()
                .Where(u => members.Contains(u.AssignedUser)).ToList()
                .GroupBy(u => u.AssignedUser).Select(n => new
                {
                    n.Key,
                    Count = n.Count()
                }).OrderBy(x => x.Count).FirstOrDefault();

            return nextAvailable != null ? nextAvailable.Key : string.Empty;

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
    }
}
