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
using OData.Models.Meta;
using OData.Utils;

namespace OData.Controllers
{
    public class DynamicTemplatesController : ODataController
    {
        private readonly ODataDbContext _context;

        public DynamicTemplatesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.DynamicTemplates);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.DynamicTemplates.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]DynamicTemplate dynamicTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DynamicTemplates.Add(dynamicTemplate);
            await _context.SaveChangesAsync();
            return Created(dynamicTemplate);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<DynamicTemplate> dynamicTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.DynamicTemplates.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            dynamicTemplate.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!dynamicTemplateExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] DynamicTemplate update)
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
                if (!dynamicTemplateExists(key))
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
            var dynamicTemplate = await _context.DynamicTemplates.FindAsync(key);
            if (dynamicTemplate == null)
            {
                return NotFound();
            }
            _context.DynamicTemplates.Remove(dynamicTemplate);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool dynamicTemplateExists(int key)
        {
            return _context.DynamicTemplates.Any(x => x.Id == key);
        }
    }
}
