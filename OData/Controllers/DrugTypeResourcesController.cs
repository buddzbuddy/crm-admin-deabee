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
    public class DrugTypeResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public DrugTypeResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.DrugTypeResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.DrugTypeResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]DrugTypeResource drugTypeResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DrugTypeResources.Add(drugTypeResource);
            await _context.SaveChangesAsync();
            return Created(drugTypeResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<DrugTypeResource> drugTypeResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.DrugTypeResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            drugTypeResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!drugTypeResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] DrugTypeResource update)
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
                if (!drugTypeResourceExists(key))
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
            var drugTypeResource = await _context.DrugTypeResources.FindAsync(key);
            if (drugTypeResource == null)
            {
                return NotFound();
            }
            _context.DrugTypeResources.Remove(drugTypeResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool drugTypeResourceExists(int key)
        {
            return _context.DrugTypeResources.Any(x => x.Id == key);
        }
    }
}
