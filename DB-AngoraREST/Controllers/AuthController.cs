using DB_AngoraLib;
using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Services.SigninService;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DB_AngoraLib.Services.TokenService;

namespace DB_AngoraREST.Controllers
{
    [Route("api/[controller]")]
    //[Route("")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISigninService _signinService;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthController> _logger;


        public AuthController(ISigninService signinService, ITokenService tokenService, UserManager<User> userManager, ILogger<AuthController> logger)
        {
            _signinService = signinService;
            _tokenService = tokenService;
            _userManager = userManager;
            _logger = logger;
        }


        //-----------------------------------: LOGIN :-----------------------------------
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login_RequestDTO loginDTO)
        {
            // Hent brugerens IP-adresse
            var userIP = HttpContext.Connection.RemoteIpAddress?.ToString();

            var result = await _signinService.LoginAsync(userIP, loginDTO);
            if (result.AccessToken != null)
            {
                return Ok(result);
            }
            return Unauthorized(new { error = "Invalid login attempt" });
        }


        //--------------------------------: EXTERNAL LOGIN :--------------------------------

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("Login_Google")]
        public IActionResult LoginWithGoogle()
        {
            // Opret en challenge for Google authentication scheme, som vil omdirigere brugeren til Google's login side.
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleLoginCallback") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[HttpGet("Login_Google")]
        //public IActionResult LoginWithGoogle()
        //{
        //    // Opret en challenge for Google authentication scheme, som vil omdirigere brugeren til Google's login side.
        //    var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleLoginCallback") };
        //    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        //}


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("signin-google")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            _logger.LogInformation("Entered GoogleLoginCallback");

            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return BadRequest("Google authentication failed.");

            var googleUser = result.Principal;
            var email = googleUser.FindFirstValue(ClaimTypes.Email);
            var name = googleUser.FindFirstValue(ClaimTypes.Name);

            // Find eller opret bruger baseret på email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User { UserName = email, Email = email, FirstName = name };
                var createUserResult = await _userManager.CreateAsync(user);
                if (!createUserResult.Succeeded)
                {
                    return BadRequest("Failed to create user.");
                }
            }
            // Generer JWT token
            var token = await _tokenService.GenerateAccessToken(user);
            // Redirect til en side med token eller returner token direkte
            return Ok(new { Token = token });
        }

    }
}