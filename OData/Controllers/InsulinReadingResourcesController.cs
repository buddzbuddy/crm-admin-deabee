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
    public class InsulinReadingResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public InsulinReadingResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.InsulinReadingResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.InsulinReadingResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]InsulinReadingResource insulinReadingResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.InsulinReadingResources.Add(insulinReadingResource);
            await _context.SaveChangesAsync();
            return Created(insulinReadingResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<InsulinReadingResource> insulinReadingResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.InsulinReadingResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            insulinReadingResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!insulinReadingResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] InsulinReadingResource update)
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
                if (!insulinReadingResourceExists(key))
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
            var insulinReadingResource = await _context.InsulinReadingResources.FindAsync(key);
            if (insulinReadingResource == null)
            {
                return NotFound();
            }
            _context.InsulinReadingResources.Remove(insulinReadingResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool insulinReadingResourceExists(int key)
        {
            return _context.InsulinReadingResources.Any(x => x.Id == key);
        }
    }
}
