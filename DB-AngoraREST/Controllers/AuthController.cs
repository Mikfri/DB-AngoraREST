using DB_AngoraLib.DTOs;
using DB_AngoraLib.Services.SigninService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DB_AngoraREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISigninService _signinService;

        public AuthController(ISigninService signinService)
        {
            _signinService = signinService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login_RequestDTO loginDTO)
        {
            // Hent brugerens IP-adresse
            var userIP = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Videregiv både loginDTO og userIP til LoginAsync metoden
            var result = await _signinService.LoginAsync(userIP, loginDTO);
            if (result.AccessToken != null) // Opdater denne linje til at tjekke for AccessToken i stedet for Token
            {
                return Ok(result);
            }
            return Unauthorized(new { error = "Invalid login attempt" });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("signin-google")]
        public async Task<IActionResult> GoogleLoginCallback(string returnUrl = null, string remoteError = null)
        {
            // Din logik her, f.eks. kalde ExternalLoginCallback metoden
            var token = await _signinService.ExternalLoginCallback(returnUrl, remoteError);
            if (!string.IsNullOrEmpty(token))
            {
                // Succes, returner token eller omdiriger brugeren
                return Ok(new { Token = token });
            }
            else
            {
                return BadRequest("Unable to process external login.");
            }
        }
    }
}