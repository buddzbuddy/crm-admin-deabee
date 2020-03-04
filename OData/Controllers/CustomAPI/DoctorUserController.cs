using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OData.Models.Data;

namespace OData.Controllers.CustomAPI
{
    [ApiController]
    public class DoctorUserController : ControllerBase
    {
        private readonly ODataDbContext _context;

        public DoctorUserController(ODataDbContext context) => _context = context;

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public ActionResult<DoctorUserResource> Register([FromBody] DoctorUserResource user)
        {
            var existingUser = _context.UserResources.Find(user.Id);
            if(existingUser != null)
            {
                return BadRequest();
            }
            _context.DoctorUserResources.Add(user);
            _context.SaveChanges();
            return Ok(user);
        }

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public ActionResult<DoctorUserResource> Authenticate([FromBody]DoctorUserResource user)
        {
            var existingUser = _context.DoctorUserResources.FirstOrDefault(x => x.Username.ToLower() == user.Username.ToLower() && x.Password == user.Password);
            if (existingUser == null)
            {
                return Ok(new { result = false });
            }
            return Ok(new { result = true, user = existingUser });
        }

        [Route("api/[controller]/[action]")]
        [HttpGet]
        public ActionResult<DoctorUserResource[]> GetAll()
        {
            return Ok(_context.DoctorUserResources.ToList());
        }

        [Route("api/[controller]/[action]/{Id}")]
        [HttpPut]
        public ActionResult Update(int Id, [FromBody]DoctorUserResource user)
        {
            var existingUser = _context.DoctorUserResources.Find(Id);
            if (existingUser == null)
            {
                return BadRequest();
            }
            existingUser.Username = user.Username;
            existingUser.Address = user.Address;
            existingUser.Bio = user.Bio;
            existingUser.City = user.City;
            existingUser.Company = user.Company;
            existingUser.Country = user.Country;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;/*
            existingUser.Password = user.Password;
            existingUser.PositionName = user.PositionName;*/
            existingUser.PostalCode = user.PostalCode;

            _context.DoctorUserResources.Update(existingUser);
            _context.SaveChanges();
            return Ok();
        }
        [Route("api/[controller]/[action]/{id}")]
        [HttpDelete]
        public ActionResult GetAll(int id)
        {
            var obj = _context.DoctorUserResources.Find(id);
            _context.DoctorUserResources.Remove(obj);
            return Ok();
        }
    }
}