using api.Data;
using api.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using api.DTO;

using Microsoft.EntityFrameworkCore;
using api.Service;
using api.Interfaces;

namespace api.Controllers
{
    public class AccountController : BaseApiController
    {

        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext contect, ITokenService tokenService)
        {
            _context = contect;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUsers>> Register(RegisterDto registerDto)
        {
            if(await _context.Users.AnyAsync(x => x.Username == registerDto.UserName.ToLower()))
                return BadRequest("Username is exsited");
            using var hmac = new HMACSHA512();
            var user = new AppUsers
            {
                Username = registerDto.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == loginDto.UserName);
            if (user == null)
                return Unauthorized("Invalid UserName");
            var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for(int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid Password");
            }
            return new UserDto
            {
                UserName = user.Username,
                Token  = _tokenService.CreateToken(user)
            };
        }
    }
}