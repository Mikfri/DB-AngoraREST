using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DB_AngoraREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpGet("RabbitCollection")]
        public async Task<IActionResult> GetCurrentUsersRabbitCollection_ByProperties(
            [FromQuery] User_KeyDTO userKeyDto, 
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
            try
            {
                var result = await _userService.GetCurrentUsersRabbitCollection_ByProperties(userKeyDto, rightEarId, leftEarId, nickName, race, color, gender, isPublic, isJuvenile, dateOfBirth, dateOfDeath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception and return a 500 error
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

    }
}
