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
    public class MenuItemsController : ODataController
    {
        private readonly ODataDbContext _context;

        public MenuItemsController(ODataDbContext context) => _context = context;

        [EnableQuery(MaxExpansionDepth = 5)]
        public IActionResult Get()
        {
            return Ok(_context.MenuItems.OrderBy(x => x.OrderIndex));
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.MenuItems.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]MenuItem menuItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            return Created(menuItem);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<MenuItem> menuItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.MenuItems.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            menuItem.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!menuItemExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] MenuItem update)
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
                if (!menuItemExists(key))
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
            var menuItem = await _context.MenuItems.FindAsync(key);
            if (menuItem == null)
            {
                return NotFound();
            }
            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool menuItemExists(int key)
        {
            return _context.MenuItems.Any(x => x.Id == key);
        }
    }
}
