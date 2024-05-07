﻿using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.UserService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace DB_AngoraREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IUserService userService, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _logger = logger;
        }

        // todo: Du har liiige fået flyttet login fra AccoutController.. 

        //[HttpPost("Login")]
        //public async Task<IActionResult> Login(UserLoginDTO userLoginDto)
        //{
        //    // Validate the user's credentials
        //    var user = await _userService.Login(userLoginDto);

        //    // If the user's credentials are invalid, return 401
        //    if (user == null)
        //    {
        //        return Unauthorized();
        //    }

        //    // Create the claims
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //        new Claim("BreederRegNo", user.Id),
        //        // Add other claims as needed
        //    };

        //    // Create the claims identity
        //    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //    // Sign the user in
        //    await HttpContext.SignInAsync(
        //        CookieAuthenticationDefaults.AuthenticationScheme,
        //        new ClaimsPrincipal(claimsIdentity),
        //        new AuthenticationProperties());

        //    // Return a success response
        //    return Ok();
        //}


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetAllUsers")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


        [HttpGet("GetCurrentUserBreederRegNo")]
        [Authorize]
        public async Task<ActionResult<string>> GetCurrentUserBreederRegNo()
        {
            // Get the current user's BreederRegNo from the User property
            var breederRegNo = User.FindFirstValue("BreederRegNo");

            // Get the user from the database
            var user = await _userService.GetUserByBreederRegNoAsync(new User_KeyDTO { BreederRegNo = breederRegNo });

            // If the user is not found, return 404
            if (user == null)
            {
                return NotFound();
            }

            // Return the user's BreederRegNo
            return Ok(user.Id);
        }



    }
}
