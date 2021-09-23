using jwt_authentication.DataAccessLayer;
using jwt_authentication.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace jwt_authentication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly DataContext _context;

        public HomeController(DataContext context)
        {
            _context = context;
        }

        // GET: api/<HomeController>
        [HttpGet]
        public async Task<ActionResult<string>> GetDummy()
        {
            return "JWT Authentication";
        }

        // Testing to fetch user anonymously
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUsers(int id)
        {
            return await _context.Users.FindAsync(id);
        }


    }
}
