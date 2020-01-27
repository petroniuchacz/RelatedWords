using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RelatedWordsAPI.Services;
using RelatedWordsAPI.Models;


namespace RelatedWordsAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]User userParam)
        {
            if (userParam?.Email == null || userParam.Password == null)
                return BadRequest(new { message = "Missing email or password" });

            var user = await _userService.Authenticate(userParam.Email, userParam.Password).ConfigureAwait(false);

            if (user == null)
                return Unauthorized(new { message = "Email or password is incorrect" });

            return Ok(Models.User.GenerateWithoutSensitive(user));
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll().ConfigureAwait(false);
            var usersWithoutSensitive = users.Select(u => Models.User.GenerateWithoutSensitive(u));
            return Ok(usersWithoutSensitive);
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetById(int UserId)
        {
            var user = await _userService.GetById(UserId).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            // only allow admins to access other user records
            var currentUserId = int.Parse(User.Identity.Name);
            if (UserId != currentUserId && !User.IsInRole(Role.Admin))
            {
                return Forbid();
            }

            return Ok(Models.User.GenerateWithoutSensitive(user));
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]User userParam)
        {
            if (userParam?.Email == null || userParam.Password == null || !Role.Roles().Contains(userParam.Role))
                return BadRequest(new { message = "Missing email, password or role." });

            var users = await _userService.GetAll().ConfigureAwait(false);
            var user = users.Where(u => u.Email == userParam.Email).FirstOrDefault();

            if (user != null)
                return BadRequest(new { message = "This email address is already in use." });

            user = new User(userParam.Email, userParam.Password, userParam.Role);
            await _userService.Register(user).ConfigureAwait(false);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("setupadmin")]
        public async Task<IActionResult> SetupAdmin([FromBody]User userParam)
        {
            if (userParam?.Email == null || userParam.Password == null)
                return BadRequest(new { message = "Missing email or password" });

            var users = await _userService.GetAll().ConfigureAwait(false);
            var admin = users.Where(u => u.Role == Role.Admin).FirstOrDefault();

            if (admin != null)
                return BadRequest(new { message = "An admin user already exists." });

            admin = new User(userParam.Email, userParam.Password, Role.Admin);
            await _userService.Register(admin);

            return Ok();
        }
    }
}