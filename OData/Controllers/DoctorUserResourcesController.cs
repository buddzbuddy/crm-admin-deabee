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
    public class DoctorUserResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public DoctorUserResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.DoctorUserResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.DoctorUserResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]DoctorUserResource doctorUserResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DoctorUserResources.Add(doctorUserResource);
            await _context.SaveChangesAsync();
            return Created(doctorUserResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<DoctorUserResource> doctorUserResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.DoctorUserResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            doctorUserResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!doctorUserResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] DoctorUserResource update)
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
                if (!doctorUserResourceExists(key))
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
            var doctorUserResource = await _context.DoctorUserResources.FindAsync(key);
            if (doctorUserResource == null)
            {
                return NotFound();
            }
            _context.DoctorUserResources.Remove(doctorUserResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool doctorUserResourceExists(int key)
        {
            return _context.DoctorUserResources.Any(x => x.Id == key);
        }
    }
}
