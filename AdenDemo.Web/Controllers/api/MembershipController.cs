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
    }

    public class UserModel
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
    }
}
