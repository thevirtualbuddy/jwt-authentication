
using jwt_authentication.DataAccessLayer;
using jwt_authentication.DTO;
using jwt_authentication.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace jwt_authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("test")]
        public string Test()
        {
            return "Hello";
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterUserDTO registerUser)
        {
            //Check if user exists
            //If we use ActionResult we are able to return different http status codes as response
            if (await (UserExists(registerUser.Username))) return BadRequest("Username already exists");
            
            // This is going to provide the hashing algorithm
            // With using we make sure that as soon as we're finished with the class, it's disposed correctly
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerUser.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerUser.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginUserDTO loginDTO)
        {
            var user = await _context.Users.
                SingleOrDefaultAsync(u => u.UserName == loginDTO.Username);

            if (user == null) return Unauthorized("Invalid Username");

            // Using the same salt that was used as key at the time of register 
            using var hmac = new HMACSHA512(user.PasswordSalt);

            // Compute the hash and check if the hash is same as stored in the db
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }
            return user;
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}
