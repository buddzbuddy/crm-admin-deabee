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
    public class ICDResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public ICDResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.ICDResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.ICDResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]ICDResource iCDResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ICDResources.Add(iCDResource);
            await _context.SaveChangesAsync();
            return Created(iCDResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<ICDResource> iCDResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.ICDResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            iCDResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!iCDResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] ICDResource update)
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
                if (!iCDResourceExists(key))
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
            var iCDResource = await _context.ICDResources.FindAsync(key);
            if (iCDResource == null)
            {
                return NotFound();
            }
            _context.ICDResources.Remove(iCDResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool iCDResourceExists(int key)
        {
            return _context.ICDResources.Any(x => x.Id == key);
        }
    }
}
