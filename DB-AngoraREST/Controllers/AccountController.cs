using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.SigninService;
//using DB_AngoraLib.Services.TokenService;
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
        private readonly IAccountService _accountService;
        private readonly IRabbitService _rabbitService;
        //private readonly TokenService _tokenService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ISigninService signinService, IAccountService accountService, IRabbitService rabbitService/*, TokenService tokenService*/)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _signinService = signinService;
            _accountService = accountService;
            _rabbitService = rabbitService;
            //_tokenService = tokenService;
        }


        //--------------------: ADD/POST :--------------------
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("Register_BasicUser")]
        public async Task<IActionResult> Register(Register_CreateBasicUserDTO newUserDto)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.Register_BasicUserAsync(newUserDto);

                if (response.IsSuccessful)
                {
                    return Ok(response);
                }

                // Hvis der er fejl, tilføj dem til ModelState
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            // Hvis vi er nået hertil, er der noget galt, vis formular igen
            return BadRequest(ModelState);
        }


        //--------------------: LOGIN :--------------------
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login_RequestDTO loginDTO)
        {
            var result = await _signinService.LoginAsync(loginDTO);
            if (!string.IsNullOrEmpty(result.Token))
            {
                return Ok(result);
            }
            return Unauthorized(new { error = "Invalid login attempt" });
        }



        //--------------------: GET :--------------------
        //-------: User collections
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized, hvis brugeren har en ugyldig token (ikke logget ind eller tokenfejl)
        [ProducesResponseType(StatusCodes.Status403Forbidden)]    // Forbidden, hvis brugeren ikke har adgang til ressourcen
        [HttpGet("MyRabbitCollection")]
        [Authorize(Roles = "Admin, Breeder, Moderator")]
        public async Task<IActionResult> GetMyRabbits()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Getting rabbits for user with ID: {userId}");

            var rabbits = await _accountService.GetMyRabbitCollection(userId);

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

            var rabbits = await _accountService.GetMyRabbitCollection_Filtered(userId, rightEarId, leftEarId, nickName, raceEnum, colorEnum, genderEnum);
            return Ok(rabbits);
        }

        //-------: User(s)
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[Authorize(Roles = "Admin, Moderator")]
        //[HttpGet("All")]
        //public async Task<IActionResult> GetAllUsers()
        //{
        //    var users = await _accountService.GetAllUsersAsync();
        //    return Ok(users);
        //}


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet("ByBreederRegNo/{breederRegNo}")]
        public async Task<IActionResult> GetUserByBreederRegNo(string breederRegNo)
        {
            var user = await _accountService.GetUserByBreederRegNoAsync(breederRegNo);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet("GetUserByUserNameOrEmail/{userNameOrEmail}")]
        public async Task<IActionResult> GetUserByUserNameOrEmail(string userNameOrEmail)
        {
            var user = await _accountService.GetUserByUserNameOrEmailAsync(userNameOrEmail);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }
    }
}
