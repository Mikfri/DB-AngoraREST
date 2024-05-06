using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.UserService;
using DB_AngoraREST.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DB_AngoraREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;
        private readonly IRabbitService _rabbitService;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IUserService userService, IRabbitService rabbitService, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _rabbitService = rabbitService;
            _configuration = configuration;
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequestDTO regDTO)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Id = regDTO.BreederRegNo,
                    UserName = regDTO.Email,
                    Email = regDTO.Email,
                    PhoneNumber = regDTO.PhoneNum,
                    FirstName = regDTO.FirstName,
                    LastName = regDTO.LastName,
                    City = regDTO.City,
                    RoadNameAndNo = regDTO.RoadNameAndNo,
                    ZipCode = regDTO.ZipCode,
                };

                var result = await _userManager.CreateAsync(user, regDTO.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok();
                }

                // Hvis der er fejl, tilføj dem til ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Hvis vi er nået hertil, er der noget galt, vis formular igen
            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id), // added
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpGet("UserRabbitCollection")]
        [Authorize]
        public async Task<IActionResult> GetMyRabbits()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Getting rabbits for user with ID: {userId}");

            var userKeyDto = new User_KeyDTO { BreederRegNo = userId };
            var rabbits = await _userService.GetCurrentUsersRabbitCollection_ByProperties(userKeyDto);

            Console.WriteLine($"Got {rabbits.Count} rabbits for user with ID: {userId}");

            return Ok(rabbits);
        }

        

        [HttpGet("UserRabbitCollection2")]
        [Authorize]
        public async Task<IActionResult> GetMyRabbits(
            [FromQuery] string rightEarId = null,
            [FromQuery] string leftEarId = null,
            [FromQuery] string nickName = null,
            [FromQuery] Race? race = null,
            [FromQuery] Color? color = null,
            [FromQuery] Gender? gender = null,
            [FromQuery] IsPublic? isPublic = null,
            [FromQuery] bool? isJuvenile = null,
            [FromQuery] DateOnly? dateOfBirth = null,
            [FromQuery] DateOnly? dateOfDeath = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userKeyDto = new User_KeyDTO { BreederRegNo = userId };

            var rabbits = await _userService.GetCurrentUsersRabbitCollection_ByProperties(userKeyDto, rightEarId, leftEarId, nickName, race, color, gender, isPublic, isJuvenile, dateOfBirth, dateOfDeath);
            return Ok(rabbits);
        }

    }
}
