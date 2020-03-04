using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OData.Models.Data;
using OData.Utils;

namespace OData.Controllers.CustomAPI
{
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ODataDbContext _context;

        public NotificationsController(ODataDbContext context) => _context = context;

        [Route("api/[controller]/[action]/{id}")]
        [HttpGet]
        public ActionResult SetRead(int id)
        {
            var obj = _context.NotificationResources.Find(id);
            if(obj != null)
            {
                obj.HasRead = 1;
                _context.NotificationResources.Update(obj);
                _context.SaveChanges();
            }
            return Ok();
        }
    }
}