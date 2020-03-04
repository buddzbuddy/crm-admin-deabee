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
    public class PressureReadingResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public PressureReadingResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.PressureReadingResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.PressureReadingResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]PressureReadingResource pressureReadingResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PressureReadingResources.Add(pressureReadingResource);
            await _context.SaveChangesAsync();
            return Created(pressureReadingResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<PressureReadingResource> pressureReadingResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.PressureReadingResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            pressureReadingResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!pressureReadingResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] PressureReadingResource update)
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
                if (!pressureReadingResourceExists(key))
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
            var pressureReadingResource = await _context.PressureReadingResources.FindAsync(key);
            if (pressureReadingResource == null)
            {
                return NotFound();
            }
            _context.PressureReadingResources.Remove(pressureReadingResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool pressureReadingResourceExists(int key)
        {
            return _context.PressureReadingResources.Any(x => x.Id == key);
        }
    }
}
