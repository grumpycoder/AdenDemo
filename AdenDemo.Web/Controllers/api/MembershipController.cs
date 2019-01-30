using AdenDemo.Web.Data;
using ALSDE.Services;
using System.Linq;
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
            //var client = new SmtpClient();
            ////Regex.Replace(groupName.Humanize().ToTitleCase(), " App ", " ", RegexOptions.ExplicitCapture);
            //var message = new MailMessage()
            //{
            //    Body = $"Please ADD user {model.Email} to group <br />{ Regex.Replace(model.GroupName.Humanize().ToTitleCase().TrimEnd('s'), " App ", " ", RegexOptions.ExplicitCapture) }",
            //    To = { "helpdesk@alsde.edu" },
            //    From = new MailAddress(User.Identity.Name),
            //    IsBodyHtml = true
            //};

            //client.Send(message);

            return Ok($"Added {model.Email} to {model.GroupName}");
        }

        [HttpDelete, Route("groupmembers")]
        public object DeleteGroupMember(UpdateGroupMemberDto model)
        {

            //var client = new SmtpClient();
            //var message = new MailMessage()
            //{
            //    Body = $"Please REMOVE user {model.Email} from group <br />{ Regex.Replace(model.GroupName.Humanize().ToTitleCase().TrimEnd('s'), " App ", " ", RegexOptions.ExplicitCapture) }",
            //    To = { "helpdesk@alsde.edu" },
            //    From = new MailAddress(User.Identity.Name),
            //    IsBodyHtml = true
            //};

            //client.Send(message);

            return Ok($"Deleted {model.Email} to {model.GroupName}");
        }
    }

    public class UpdateGroupMemberDto
    {
        public string GroupName { get; set; }
        public string Email { get; set; }
    }

    public class UserModel
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
    }
}
