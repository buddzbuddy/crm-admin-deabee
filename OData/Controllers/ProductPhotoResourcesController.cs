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
    public class ProductPhotoResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public ProductPhotoResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.ProductPhotoResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.ProductPhotoResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]ProductPhotoResource productPhotoResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductPhotoResources.Add(productPhotoResource);
            await _context.SaveChangesAsync();
            return Created(productPhotoResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<ProductPhotoResource> productPhotoResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.ProductPhotoResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            productPhotoResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!productPhotoResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] ProductPhotoResource update)
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
                if (!productPhotoResourceExists(key))
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
            var productPhotoResource = await _context.ProductPhotoResources.FindAsync(key);
            if (productPhotoResource == null)
            {
                return NotFound();
            }
            _context.ProductPhotoResources.Remove(productPhotoResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool productPhotoResourceExists(int key)
        {
            return _context.ProductPhotoResources.Any(x => x.Id == key);
        }
    }
}
