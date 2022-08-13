using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext context;
        public ITokenService TokenService { get; }
        public AccountController(DataContext context, ITokenService tokenService)
        {
            this.TokenService = tokenService;
            this.context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // Check if user exists
            if (await userExists(registerDto.Username)) return BadRequest("User already exists");
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            this.context.Users.Add(user);
            await this.context.SaveChangesAsync();
            return new UserDto
            {
                Username = user.UserName,
                Token = this.TokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
            if (user == null) return Unauthorized("Invalid username");
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Passwor");
            }

            // correct credentials
            return new UserDto
            {
                Username = user.UserName,
                Token = this.TokenService.CreateToken(user)
            };
        }

        private async Task<bool> userExists(string username)
        {
            return await this.context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}