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
    public class DynamicFieldsController : ODataController
    {
        private readonly ODataDbContext _context;

        public DynamicFieldsController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.DynamicFields.OrderBy(x => x.OrderIndex));
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.DynamicFields.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]DynamicField dynamicField)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DynamicFields.Add(dynamicField);
            await _context.SaveChangesAsync();
            return Created(dynamicField);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<DynamicField> dynamicField)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.DynamicFields.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            dynamicField.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!dynamicFieldExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] DynamicField update)
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
                if (!dynamicFieldExists(key))
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
            var dynamicField = await _context.DynamicFields.FindAsync(key);
            if (dynamicField == null)
            {
                return NotFound();
            }
            _context.DynamicFields.Remove(dynamicField);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool dynamicFieldExists(int key)
        {
            return _context.DynamicFields.Any(x => x.Id == key);
        }
    }
}
