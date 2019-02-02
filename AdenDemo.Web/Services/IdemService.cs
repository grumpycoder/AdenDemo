using AdenDemo.Web.Data;
using ALSDE.Idem.Web.UI.AimBanner;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AdenDemo.Web.Services
{
    public class IdemService
    {
        private IdemContext _context;

        public IdemService()
        {
            _context = new IdemContext();
        }

        public List<IdemApplication> GetApplication()
        {
            var query = "select ApplicationId, ApplicationViewKey, WebsiteViewKey, Title, Description, SectionViewKey from Idem.Applications order by title";
            using (var cn = new SqlConnection(_context.Database.Connection.ConnectionString))
            {
                var list = cn.Query<IdemApplication>(query).ToList();
                return list;
            }
        }
    }
}
