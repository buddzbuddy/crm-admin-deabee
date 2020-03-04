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
    public class UserTypeResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public UserTypeResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.UserTypeResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.UserTypeResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]UserTypeResource userTypeResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UserTypeResources.Add(userTypeResource);
            await _context.SaveChangesAsync();
            return Created(userTypeResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<UserTypeResource> userTypeResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.UserTypeResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            userTypeResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!userTypeResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] UserTypeResource update)
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
                if (!userTypeResourceExists(key))
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
            var userTypeResource = await _context.UserTypeResources.FindAsync(key);
            if (userTypeResource == null)
            {
                return NotFound();
            }
            _context.UserTypeResources.Remove(userTypeResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool userTypeResourceExists(int key)
        {
            return _context.UserTypeResources.Any(x => x.Id == key);
        }
    }
}
