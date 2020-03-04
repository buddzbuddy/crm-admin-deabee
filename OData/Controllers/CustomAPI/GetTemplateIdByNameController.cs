using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OData.Controllers.CustomAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetTemplateIdByNameController : ControllerBase
    {
        private readonly ODataDbContext _context;

        public GetTemplateIdByNameController(ODataDbContext context) => _context = context;
        [HttpGet]
        public ActionResult<int> Get(string name)
        {
            return _context.DynamicTemplates.FirstOrDefault(x => x.Name == name)?.Id;
        }
    }
}