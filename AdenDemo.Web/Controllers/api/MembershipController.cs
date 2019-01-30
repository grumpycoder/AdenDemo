using AdenDemo.Web.Data;
using AdenDemo.Web.Services;
using Alsde.Extensions;
using ALSDE.Services;
using Humanizer;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace AdenDemo.Web.Controllers.api
{
    [RoutePrefix("api/membership")]
    public class MembershipController : ApiController
    {
        private AdenContext _context;
        private IdemUserService _userService;

        public MembershipController()
        {
            _context = new AdenContext();
            _userService = new IdemUserService();
        }

        [HttpGet, Route("{username}")]
        public object Users(string username = null)
        {
            var users = _userService.GetUsers(username);
            return Ok(users.ToList());
        }

        [HttpPut, Route("groupmembers")]
        public object AddGroupUser(UpdateGroupMemberDto model)
        {
            model.Action = "ADD";
            model.GroupName = Regex.Replace(model.GroupName.Humanize().ToTitleCase().TrimEnd('s'), " App ", " ",
                RegexOptions.ExplicitCapture);
            WorkEmailer.SendRequest(model);
            return Ok($"Added {model.Email} to {model.GroupName}");
        }

        [HttpDelete, Route("groupmembers")]
        public object DeleteGroupMember(UpdateGroupMemberDto model)
        {
            model.Action = "DELETE";
            model.GroupName = Regex.Replace(model.GroupName.Humanize().ToTitleCase().TrimEnd('s'), " App ", " ",
                RegexOptions.ExplicitCapture);
            WorkEmailer.SendRequest(model);

            return Ok($"Deleted {model.Email} to {model.GroupName}");
        }
    }

    public class UpdateGroupMemberDto
    {
        public string GroupName { get; set; }
        public string Email { get; set; }

        public string Action { get; set; }
    }

    public class UserModel
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
    }
}
