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
    public class DrugResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public DrugResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.DrugResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.DrugResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]DrugResource drugResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DrugResources.Add(drugResource);
            await _context.SaveChangesAsync();
            return Created(drugResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<DrugResource> drugResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.DrugResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            drugResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!drugResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] DrugResource update)
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
                if (!drugResourceExists(key))
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
            var drugResource = await _context.DrugResources.FindAsync(key);
            if (drugResource == null)
            {
                return NotFound();
            }
            _context.DrugResources.Remove(drugResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool drugResourceExists(int key)
        {
            return _context.DrugResources.Any(x => x.Id == key);
        }
    }
}
