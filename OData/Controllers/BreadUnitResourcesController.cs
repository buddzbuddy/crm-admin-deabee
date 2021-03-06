﻿using System;
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
    public class BreadUnitResourcesController : ODataController
    {
        private readonly ODataDbContext _context;

        public BreadUnitResourcesController(ODataDbContext context) => _context = context;

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.BreadUnitResources);
        }
        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_context.BreadUnitResources.IncludeAll().FirstOrDefault(x => x.Id == key));
        }

        public async Task<IActionResult> Post([FromBody]BreadUnitResource breadUnitResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.BreadUnitResources.Add(breadUnitResource);
            await _context.SaveChangesAsync();
            return Created(breadUnitResource);
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<BreadUnitResource> breadUnitResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _context.BreadUnitResources.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            breadUnitResource.Patch(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!breadUnitResourceExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] BreadUnitResource update)
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
                if (!breadUnitResourceExists(key))
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
            var breadUnitResource = await _context.BreadUnitResources.FindAsync(key);
            if (breadUnitResource == null)
            {
                return NotFound();
            }
            _context.BreadUnitResources.Remove(breadUnitResource);
            await _context.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool breadUnitResourceExists(int key)
        {
            return _context.BreadUnitResources.Any(x => x.Id == key);
        }
    }
}
