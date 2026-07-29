using CubeServer.Data;
using CubeServer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        // GET: api/<TokenController>
        [HttpGet]
        public ActionResult<Token> Get(string userId, string password)
        {
            if (userId == null || password == null)
            {
                return BadRequest();
            }

            User user = Global.db.GetUser(userId);
            if (user == null) return NotFound();

            if (user.privilege != (int)UserPrivilege.WebApi) return BadRequest("Bad user privilege");

            if (user.password != Util.EncryptPassword(password, user.privilege)) return BadRequest();

            Token token = Global.db.CreateToken(userId);

            return Ok(token);
        }
#if ZERO
        // GET api/<TokenController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TokenController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TokenController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TokenController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
#endif
    }
}
