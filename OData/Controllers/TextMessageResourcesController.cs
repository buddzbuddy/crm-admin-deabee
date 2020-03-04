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
    public class TextMessageResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public TextMessageResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.TextMessageResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.TextMessageResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]TextMessageResource textMessageResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            textMessageResource.CreatedAt = DateTime.Now;
            _context.TextMessageResources.Add(textMessageResource);
            await _context.SaveChangesAsync();
            return Created(textMessageResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<TextMessageResource> textMessageResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.TextMessageResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            textMessageResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!textMessageResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] TextMessageResource update)
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
                if (!textMessageResourceExists(key))
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
            var textMessageResource = await _context.TextMessageResources.FindAsync(key);
            if (textMessageResource == null)
            {
                return NotFound();
            }
            _context.TextMessageResources.Remove(textMessageResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool textMessageResourceExists(int key)
        {
            return _context.TextMessageResources.Any(x => x.Id == key);
        }
    }
}
