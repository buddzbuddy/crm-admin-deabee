using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OData.Models.Data;

namespace OData.Controllers.CustomAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchFoodController : ControllerBase
    {
        private readonly ODataDbContext _context;

        public SearchFoodController(ODataDbContext context) => _context = context;
        [HttpGet]
        public ActionResult<ProductResource[]> Get(string term)
        {
            if (string.IsNullOrEmpty(term)) return null;
            return Ok(_context.ProductResources.Where(x => x.Name.ToLower().Contains(term.ToLower())).ToList());
        }
    }
}