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
    public class GlucoseReadingResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public GlucoseReadingResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.GlucoseReadingResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.GlucoseReadingResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]GlucoseReadingResource glucoseReadingResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.GlucoseReadingResources.Add(glucoseReadingResource);
            await _context.SaveChangesAsync();
            return Created(glucoseReadingResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<GlucoseReadingResource> glucoseReadingResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.GlucoseReadingResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            glucoseReadingResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!glucoseReadingResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] GlucoseReadingResource update)
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
                if (!glucoseReadingResourceExists(key))
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
            var glucoseReadingResource = await _context.GlucoseReadingResources.FindAsync(key);
            if (glucoseReadingResource == null)
            {
                return NotFound();
            }
            _context.GlucoseReadingResources.Remove(glucoseReadingResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool glucoseReadingResourceExists(int key)
        {
            return _context.GlucoseReadingResources.Any(x => x.Id == key);
        }
    }
}
