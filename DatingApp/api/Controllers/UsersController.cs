using System;
using System.Collections.Generic;
using System.Linq;
using api.Data;
using api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<IEnumerable<AppUsers>> GetUsers() 
        {
            var users = _context.Users.ToList();
            return users;
        }

        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<AppUsers> GetUser(int id)
        {
            var user = _context.Users.Find(id);
            return user;
        }
    }
}