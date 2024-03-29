using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        public DataContext Context { get; }
        public UsersController(DataContext context)
        {
            this.Context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await this.Context.Users.ToListAsync();
            return users;
        }


        [HttpGet("{id}")]
        public ActionResult<AppUser> GetUser(int id)
        {
            var user = this.Context.Users.Find(id);
            return user;
        }
    }
}