using DB_AngoraLib;
using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.BreederBrandService;
using DB_AngoraLib.Services.SigninService;
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

        private readonly ISigninService _signinService;
        private readonly IAccountService _accountService;
        private readonly IBreederBrandService _bbService;

        public AccountController(ISigninService signinService, IAccountService accountService, IBreederBrandService bbService)
        {
            _signinService = signinService;
            _accountService = accountService;
            _bbService = bbService;
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


        //--------------------: GET :--------------------
        //-------: User profile
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("UserProfile/{userProfileId}")]
        [Authorize(Policy = "ReadUser")]
        public async Task<IActionResult> GetUserProfile(string userProfileId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userClaims = User.Claims.ToList();

            var userProfile = await _accountService.Get_User_Profile(userId, userProfileId, userClaims);

            if (userProfile != null)
            {
                return Ok(userProfile);
            }

            return NotFound("User profile not found or you do not have permission to view it.");
        }

        //-------: BreederBrand profile


        //-------: User collections

        /// <summary>
        /// Viser brugerens ICollection af RabbitsOwned med mulighed for at filtrere på en række parametre.
        /// DateOnly, fungerer pt ikke som den skal i Swagger UI
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("Rabbits_Owned")]
        [Authorize(Roles = "Admin, Breeder, Moderator")]
        public async Task<IActionResult> GetMyFilteredRabbits([FromQuery] Rabbit_FilteredRequestDTO filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rabbits = await _accountService.GetAll_RabbitsOwned_Filtered(userId, filter);
            return Ok(rabbits);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized, hvis brugeren har en ugyldig token (ikke logget ind eller tokenfejl)
        [ProducesResponseType(StatusCodes.Status403Forbidden)]    // Forbidden, hvis brugeren ikke har adgang til ressourcen
        [HttpGet("Rabbits_FromMyFold")]
        [Authorize(Roles = "Admin, Breeder, Moderator")]
        public async Task<IActionResult> GetRabbitsFromMyFold()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Getting rabbits for user with ID: {userId}");

            var rabbits = await _accountService.GetAll_Rabbits_FromMyFold(userId);

            Console.WriteLine($"Got {rabbits.Count} rabbits for user with ID: {userId}");

            return Ok(rabbits);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized, hvis brugeren har en ugyldig token (ikke logget ind eller tokenfejl)
        [ProducesResponseType(StatusCodes.Status403Forbidden)]    // Forbidden, hvis brugeren ikke har adgang til ressourcen
        [HttpGet("TransferRequests_Received")]
        [Authorize(Roles = "Admin, Breeder, Moderator")]
        public async Task<ActionResult<List<TransferRequest_ReceivedDTO>>> GetReceivedTransferRequests([FromQuery] TransferRequest_ReceivedFilterDTO filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("Bruger ID mangler eller er ugyldigt.");
            }

            var transferRequests = await _accountService.GetAll_TransferRequests_Received(userId, filter);
            return Ok(transferRequests);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized, hvis brugeren har en ugyldig token (ikke logget ind eller tokenfejl)
        [ProducesResponseType(StatusCodes.Status403Forbidden)]    // Forbidden, hvis brugeren ikke har adgang til ressourcen
        [HttpGet("TransferRequests_Issued")]
        [Authorize(Roles = "Admin, Breeder, Moderator")]
        public async Task<ActionResult<List<TransferRequest_SentDTO>>> GetSentTransferRequests([FromQuery] TransferRequest_SentFilterDTO filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("Bruger ID mangler eller er ugyldigt.");
            }

            var transferRequests = await _accountService.GetAll_TransferRequests_Sent(userId, filter);
            return Ok(transferRequests);
        }

        //-------: User(s)
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet("All")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _accountService.GetAll_Users();
            return Ok(users);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet("ByBreederRegNo/{breederRegNo}")]
        public async Task<IActionResult> GetUserByBreederRegNo(string breederRegNo)
        {
            var user = await _accountService.Get_UserByBreederRegNo(breederRegNo);
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
            var user = await _accountService.Get_UserByUserNameOrEmail(userNameOrEmail);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }
    }
}
