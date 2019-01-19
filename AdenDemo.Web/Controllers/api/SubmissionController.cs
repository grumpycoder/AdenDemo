using AdenDemo.Web.Data;
using AdenDemo.Web.ViewModels;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AdenDemo.Web.Controllers.api
{
    public class SubmissionController : ApiController
    {
        [HttpGet]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            var _context = new AdenContext();

            var dto = await _context.Submissions.ProjectTo<SubmissionViewDto>().ToListAsync();

            return Ok(DataSourceLoader.Load(dto.OrderBy(x => x.DueDate).ThenByDescending(x => x.Id), loadOptions));
        }
    }
}
