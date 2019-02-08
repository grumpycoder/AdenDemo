using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;

namespace Aden.Web.Controllers.api
{
    [RoutePrefix("api/mail")]
    public class MailController : ApiController
    {

        [HttpPost, Route("delete/{id}")]
        public async Task<object> Delete(string id)
        {
            var path = HostingEnvironment.MapPath(@"/App_Data");
            File.Delete($@"{path}/{id}.eml");
            return Ok();
        }
    }
}
