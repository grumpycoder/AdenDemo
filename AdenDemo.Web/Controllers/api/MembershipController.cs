using AdenDemo.Web.Data;
using ALSDE.Dtos;
using Dapper;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace AdenDemo.Web.Controllers.api
{
    [RoutePrefix("api/membership")]
    public class MembershipController : ApiController
    {
        private AdenContext _context;
        private IdemContext _idemContext;

        public MembershipController()
        {
            _context = new AdenContext();
            _idemContext = new IdemContext();
        }

        [HttpGet, Route("{username}")]
        public object Users(string username = null)
        {
            //TODO: Move to external library
            var query = "select top 15 LastName, FirstName, EmailAddress, " +
                        "IdentityGuid from Idem.Identities " +
                        "WHERE EmailAddress like '%' + @SearchString + '%' OR " +
                        "LastName like '%' + @SearchString + '%' OR " +
                        "PrintName like '%' + @SearchString + '%'";
            using (var cn = new SqlConnection(_idemContext.Database.Connection.ConnectionString))
            {
                var list = cn.Query<AuthenticatedUserDto>(query, new { @SearchString = username }).ToList();
                return Ok(list);
            }
        }

        [HttpGet, Route("groupmembers/{groupId}")]
        public async Task<object> GroupMembers(int groupId)
        {
            var dto = await _context.Groups.Include(u => u.Users).FirstOrDefaultAsync(x => x.Id == groupId);

            return Ok(dto.Users);
        }

        [HttpPut, Route("groupmembers")]
        public object AddGroupUser(UpdateGroupMemberDto model)
        {
            var group = _context.Groups.Include(u => u.Users).FirstOrDefault(x => x.Id == model.GroupId);

            if (group == null) return BadRequest("Group does not exists");

            var user = _context.Users.FirstOrDefault(x => x.IdentityGuid == model.IdentityGuid);

            AuthenticatedUserDto idemUser;


            //TODO: Move to external library
            var query = "select top 1 LastName, FirstName, EmailAddress, IdentityGuid from Idem.Identities WHERE IdentityGuid = @IdentityGuid";
            using (var cn = new SqlConnection(_idemContext.Database.Connection.ConnectionString))
            {
                idemUser = cn.Query<AuthenticatedUserDto>(query, new { @IdentityGuid = model.IdentityGuid }).SingleOrDefault();
            }

            if (idemUser != null)
            {
                user.EmailAddress = idemUser.EmailAddress;
                user.FirstName = idemUser.Firstname;
                user.LastName = idemUser.Lastname;
                user.IdentityGuid = idemUser.IdentityGuid;
                user.FullName = $"{user.FirstName} {user.LastName}";
            }

            _context.Users.AddOrUpdate(user);

            group.Users.Add(user);

            _context.SaveChanges();

            return Ok(user);
        }

        [HttpDelete, Route("groupmembers")]
        public object DeleteGroupMember(UpdateGroupMemberDto model)
        {
            var group = _context.Groups.Include(u => u.Users).FirstOrDefault(x => x.Id == model.GroupId);

            if (group == null) return BadRequest("Group does not exists");

            var user = group.Users.FirstOrDefault(x => x.IdentityGuid == model.IdentityGuid);

            group.Users.Remove(user);

            _context.SaveChanges();

            return Ok($"Deleted {user.FullName} to {group.Name}");
        }
    }

    public class UpdateGroupMemberDto
    {
        public int GroupId { get; set; }
        public Guid IdentityGuid { get; set; }
    }

    public class UserModel
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
    }
}
