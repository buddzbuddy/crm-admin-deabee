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
    public class CholesterolReadingResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public CholesterolReadingResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.CholesterolReadingResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.CholesterolReadingResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]CholesterolReadingResource cholesterolReadingResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CholesterolReadingResources.Add(cholesterolReadingResource);
            await _context.SaveChangesAsync();
            return Created(cholesterolReadingResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<CholesterolReadingResource> cholesterolReadingResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.CholesterolReadingResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            cholesterolReadingResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!cholesterolReadingResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] CholesterolReadingResource update)
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
                if (!cholesterolReadingResourceExists(key))
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
            var cholesterolReadingResource = await _context.CholesterolReadingResources.FindAsync(key);
            if (cholesterolReadingResource == null)
            {
                return NotFound();
            }
            _context.CholesterolReadingResources.Remove(cholesterolReadingResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool cholesterolReadingResourceExists(int key)
        {
            return _context.CholesterolReadingResources.Any(x => x.Id == key);
        }
    }
}
