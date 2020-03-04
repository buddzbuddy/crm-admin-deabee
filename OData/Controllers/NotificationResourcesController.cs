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
    public class NotificationResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public NotificationResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.NotificationResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.NotificationResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]NotificationResource notificationResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            notificationResource.CreatedAt = DateTime.Now;
            notificationResource.HasRead = 0;
            _context.NotificationResources.Add(notificationResource);
            await _context.SaveChangesAsync();
            return Created(notificationResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<NotificationResource> notificationResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.NotificationResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            notificationResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!notificationResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] NotificationResource update)
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
                if (!notificationResourceExists(key))
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
            var notificationResource = await _context.NotificationResources.FindAsync(key);
            if (notificationResource == null)
            {
                return NotFound();
            }
            _context.NotificationResources.Remove(notificationResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool notificationResourceExists(int key)
        {
            return _context.NotificationResources.Any(x => x.Id == key);
        }
    }
}
