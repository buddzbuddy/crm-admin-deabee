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
    public class QualityLevelResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public QualityLevelResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.QualityLevelResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.QualityLevelResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]QualityLevelResource qualityLevelResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.QualityLevelResources.Add(qualityLevelResource);
            await _context.SaveChangesAsync();
            return Created(qualityLevelResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<QualityLevelResource> qualityLevelResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.QualityLevelResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            qualityLevelResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!qualityLevelResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] QualityLevelResource update)
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
                if (!qualityLevelResourceExists(key))
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
            var qualityLevelResource = await _context.QualityLevelResources.FindAsync(key);
            if (qualityLevelResource == null)
            {
                return NotFound();
            }
            _context.QualityLevelResources.Remove(qualityLevelResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool qualityLevelResourceExists(int key)
        {
            return _context.QualityLevelResources.Any(x => x.Id == key);
        }
    }
}
