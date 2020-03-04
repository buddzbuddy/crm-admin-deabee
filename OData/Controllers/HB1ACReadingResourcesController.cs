using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OData;
using OData.Models.Data;
using OData.Utils;

namespace OData.Controllers
{
    public class HB1ACReadingResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public HB1ACReadingResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.HB1ACReadingResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.HB1ACReadingResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]HB1ACReadingResource hB1ACReadingResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.HB1ACReadingResources.Add(hB1ACReadingResource);
            await _context.SaveChangesAsync();
            return Created(hB1ACReadingResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<HB1ACReadingResource> hB1ACReadingResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.HB1ACReadingResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            hB1ACReadingResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!hB1ACReadingResourceExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(entity);
        }

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] HB1ACReadingResource update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.Id)
            {
                return BadRequest();
            }
            _context.Entry(update).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!hB1ACReadingResourceExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(update);
        }

        public async Task<ActionResult> Delete([FromODataUri] int key)
        {
            var hB1ACReadingResource = await _context.HB1ACReadingResources.FindAsync(key);
            if (hB1ACReadingResource == null)
            {
                return NotFound();
            }
            _context.HB1ACReadingResources.Remove(hB1ACReadingResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool hB1ACReadingResourceExists(int key)
        {
            return _context.HB1ACReadingResources.Any(x => x.Id == key);
        }
    }
}
