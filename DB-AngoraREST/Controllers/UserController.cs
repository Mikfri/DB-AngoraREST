using DB_AngoraLib.DTOs;
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

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        [HttpGet("mycollection")]
        public async Task<ActionResult<IEnumerable<Rabbit_PreviewDTO>>> GetCurrentUsersRabbitCollection()
        {
            var breederRegNo = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (breederRegNo == null)
            {
                return Unauthorized();
            }

            var userKeyDto = new User_KeyDTO { BreederRegNo = breederRegNo };
            var rabbits = await _userService.GetCurrentUsersRabbitCollection_ByProperties(userKeyDto);
            if (rabbits == null || !rabbits.Any())
            {
                return NotFound();
            }
            return Ok(rabbits);
        }

    }
}
