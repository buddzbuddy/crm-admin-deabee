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
    public class UserManagerController : ControllerBase
    {
        private readonly ODataDbContext _context;

        public UserManagerController(ODataDbContext context) => _context = context;

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public ActionResult<int> Create([FromBody] UserResource user)
        {
            var existingUser = _context.UserResources.FirstOrDefault(x => x.Name.Trim().ToUpper() == user.Name.Trim().ToUpper());
            if(existingUser != null)
            {
                return Ok(new { existingUser.Id });
            }
            _context.UserResources.Add(user);
            _context.SaveChanges();

            NotificationService.NewUser(_context, user);

            return Ok(new { user.Id });
        }

        [Route("api/[controller]/[action]/{pin}")]
        [HttpGet]
        public ActionResult CreateByPIN(string pin)
        {
            var user = _context.UserResources.FirstOrDefault(x => x.Name.Trim().ToUpper() == pin.Trim().ToUpper());
            if (user != null)
            {
                return Ok(user.Id);
            }
            user = new UserResource
            {
                Name = pin
            };
            _context.UserResources.Add(user);
            _context.SaveChanges();

            NotificationService.NewUser(_context, user);

            return Ok(user.Id);
        }

        [Route("api/[controller]/[action]/{Id}")]
        [HttpPut]
        public ActionResult Update(int Id, [FromBody]UserResource user)
        {
            var existingUser = _context.UserResources.Find(Id);
            if (existingUser == null)
            {
                return BadRequest();
            }
            existingUser.Age = user.Age;
            existingUser.Custom_range_max = user.Custom_range_max;
            existingUser.Custom_range_min = user.Custom_range_min;
            existingUser.D_type = user.D_type;
            existingUser.Fullname = user.Fullname;
            existingUser.InsulinName = user.InsulinName;
            existingUser.InsulinCompany = user.InsulinCompany;
            existingUser.Name = user.Name;
            existingUser.Preferred_language = user.Preferred_language;
            existingUser.Preferred_range = user.Preferred_range;
            existingUser.Preferred_unit = user.Preferred_unit;
            existingUser.Preferred_unit_a1c = user.Preferred_unit_a1c;
            existingUser.Preferred_unit_weight = user.Preferred_unit_weight;

            _context.UserResources.Update(existingUser);
            _context.SaveChanges();
            return Ok();
        }

        [Route("api/[controller]/[action]/{username}")]
        [HttpPut]
        public ActionResult UpdateByName(string username, [FromBody]UserResource user)
        {
            var existingUser = _context.UserResources.FirstOrDefault(x => x.Name.ToLower() == username.ToLower());
            if (existingUser == null)
            {
                return BadRequest();
            }
            existingUser.Age = user.Age;
            existingUser.Custom_range_max = user.Custom_range_max;
            existingUser.Custom_range_min = user.Custom_range_min;
            existingUser.D_type = user.D_type;
            existingUser.Fullname = user.Fullname;
            existingUser.InsulinName = user.InsulinName;
            existingUser.InsulinCompany = user.InsulinCompany;
            existingUser.Name = user.Name;
            existingUser.Preferred_language = user.Preferred_language;
            existingUser.Preferred_range = user.Preferred_range;
            existingUser.Preferred_unit = user.Preferred_unit;
            existingUser.Preferred_unit_a1c = user.Preferred_unit_a1c;
            existingUser.Preferred_unit_weight = user.Preferred_unit_weight;

            _context.UserResources.Attach(existingUser);
            _context.UserResources.Update(existingUser);
            _context.SaveChanges();
            return Ok();
        }

        [Route("api/[controller]/[action]")]
        [HttpGet]
        public ActionResult<UserResource[]> GetAll()
        {
            return Ok(_context.UserResources.ToList());
        }
    }
}