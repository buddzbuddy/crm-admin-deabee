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
    public class MessagesController : ControllerBase
    {
        private readonly ODataDbContext _context;

        public MessagesController(ODataDbContext context) => _context = context;

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public ActionResult<TextMessageResource> SendToDoctor([FromBody] TextMessageResource obj)
        {
            obj.DoctorUserResourceId = _context.DoctorUserResources.First().Id;
            _context.TextMessageResources.Add(obj);
            _context.SaveChanges();
            return Ok(obj);
        }

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public ActionResult<TextMessageResource> SendToUser([FromBody] TextMessageResource obj)
        {
            _context.TextMessageResources.Add(obj);
            _context.SaveChanges();
            return Ok(obj);
        }

        [Route("api/[controller]/[action]/{userId}")]
        [HttpGet]
        public ActionResult GetList(int userId)
        {
            var msgList = _context.TextMessageResources.IncludeAll().Where(x => x.UserResourceId == userId).OrderByDescending(x => x.CreatedAt).ToList();

            foreach (var item in msgList)
            {
                item.IsRead = true;
                _context.TextMessageResources.Attach(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            _context.SaveChanges();

            return Ok(msgList.Select(x => new { sender = x.DoctorUserResource.FirstName + " " + x.DoctorUserResource.LastName, message = x.Message, createdAt = x.CreatedAt.ToString("yyyy-MM-dd") }));
        }
    }
}