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
    public class MealTimeResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public MealTimeResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.MealTimeResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.MealTimeResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]MealTimeResource mealTimeResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.MealTimeResources.Add(mealTimeResource);
            await _context.SaveChangesAsync();
            return Created(mealTimeResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<MealTimeResource> mealTimeResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.MealTimeResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            mealTimeResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!mealTimeResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] MealTimeResource update)
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
                if (!mealTimeResourceExists(key))
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
            var mealTimeResource = await _context.MealTimeResources.FindAsync(key);
            if (mealTimeResource == null)
            {
                return NotFound();
            }
            _context.MealTimeResources.Remove(mealTimeResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool mealTimeResourceExists(int key)
        {
            return _context.MealTimeResources.Any(x => x.Id == key);
        }
    }
}
