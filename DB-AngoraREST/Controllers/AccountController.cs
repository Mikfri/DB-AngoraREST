using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.SigninService;
using DB_AngoraLib.Services.UserService;
using DB_AngoraREST.DTOs;
using DB_AngoraREST.Services;
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
        private readonly ISigninService _signinService;
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly IRabbitService _rabbitService;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ISigninService signinService, IUserService userService, IAccountService accountService, IRabbitService rabbitService, IConfiguration configuration, TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _signinService = signinService;
            _userService = userService;
            _accountService = accountService;
            _rabbitService = rabbitService;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("Register_BasicUser")]
        public async Task<IActionResult> Register(User_CreateBasicDTO newUserDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.Register_BasicUserAsync(newUserDto);

                if (result.Succeeded)
                {
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginDTO)
        {
            var result = await _signinService.LoginAsync(loginDTO.Email, loginDTO.Password, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(loginDTO.Email);
                var (token, expiration) = await _tokenService.GenerateToken(user);

                return Ok(new
                {
                    token,
                    expiration
                });
            }
            return Unauthorized();
        }

        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[HttpPost("Register")]
        //public async Task<IActionResult> Register(RegisterRequestDTO regDTO)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new User
        //        {
        //            Id = regDTO.BreederRegNo,
        //            UserName = regDTO.Email,
        //            Email = regDTO.Email,
        //            PhoneNumber = regDTO.PhoneNum,
        //            FirstName = regDTO.FirstName,
        //            LastName = regDTO.LastName,
        //            City = regDTO.City,
        //            RoadNameAndNo = regDTO.RoadNameAndNo,
        //            ZipCode = regDTO.ZipCode,
        //        };

        //        var result = await _userManager.CreateAsync(user, regDTO.Password);

        //        if (result.Succeeded)
        //        {
        //            await _signInManager.SignInAsync(user, isPersistent: false);
        //            return Ok();
        //        }

        //        // Hvis der er fejl, tilføj dem til ModelState
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //    }

        //    // Hvis vi er nået hertil, er der noget galt, vis formular igen
        //    return BadRequest(ModelState);
        //}

        //[HttpPost("Login2")]
        //public async Task<IActionResult> Login2(LoginRequestDTO loginDTO)
        //{
        //    var user = await _userManager.FindByEmailAsync(loginDTO.Email);
        //    if (user != null && await _userManager.CheckPasswordAsync(user, loginDTO.Password))
        //    {
        //        var userRoles = await _userManager.GetRolesAsync(user);

        //        var authClaims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, user.UserName),
        //            new Claim(ClaimTypes.NameIdentifier, user.Id), // added
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        };

        //        foreach (var userRole in userRoles)
        //        {
        //            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        //        }

        //        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        //        var token = new JwtSecurityToken(
        //            issuer: _configuration["Jwt:Issuer"],
        //            audience: _configuration["Jwt:Audience"],
        //            expires: DateTime.Now.AddHours(3),
        //            claims: authClaims,
        //            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //            );

        //        return Ok(new
        //        {
        //            token = new JwtSecurityTokenHandler().WriteToken(token),
        //            expiration = token.ValidTo
        //        });
        //    }
        //    return Unauthorized();
        //}

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized, hvis brugeren har en ugyldig token (ikke logget ind eller tokenfejl)
        [ProducesResponseType(StatusCodes.Status403Forbidden)]    // Forbidden, hvis brugeren ikke har adgang til ressourcen
        [HttpGet("MyRabbitCollection")]
        [Authorize(Roles = "Admin, Breeder, Moderator")]
        public async Task<IActionResult> GetMyRabbits()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Getting rabbits for user with ID: {userId}");

            var rabbits = await _userService.GetMyRabbitCollection(userId);

            Console.WriteLine($"Got {rabbits.Count} rabbits for user with ID: {userId}");

            return Ok(rabbits);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("MyRabbitCollection_Filtered")]
        [Authorize(Roles = "Admin, Breeder, Moderator")]
        public async Task<IActionResult> GetMyFilteredRabbits(
            [FromQuery] string rightEarId = null,
            [FromQuery] string leftEarId = null,
            [FromQuery] string nickName = null,
            [FromQuery] string race = null,
            [FromQuery] string color = null,
            [FromQuery] string gender = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //------: TRY PARSE ENUMS:
            // Hvis der bare benytte Enum.Parse (ikke Enum.TryParse) åbner vi op for der kan kastes exceptions, som gør systemet mere sårbart, for DDoS.
            Race? raceEnum = null;
            if (!string.IsNullOrEmpty(race))
            {
                Enum.TryParse(race, out Race parsedRace);
                raceEnum = parsedRace;
            }

            Color? colorEnum = null;
            if (!string.IsNullOrEmpty(color))
            {
                Enum.TryParse(color, out Color parsedColor);
                colorEnum = parsedColor;
            }

            Gender? genderEnum = null;
            if (!string.IsNullOrEmpty(gender))
            {
                Enum.TryParse(gender, out Gender parsedGender);
                genderEnum = parsedGender;
            }

            var rabbits = await _userService.GetMyRabbitCollection_Filtered(userId, rightEarId, leftEarId, nickName, raceEnum, colorEnum, genderEnum);
            return Ok(rabbits);
        }

    }
}
