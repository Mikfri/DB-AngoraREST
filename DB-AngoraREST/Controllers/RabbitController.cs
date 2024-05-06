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
    public class RabbitController : ControllerBase
    {
        private readonly IRabbitService _rabbitService;
        private readonly IUserService _userService;

        public RabbitController(IRabbitService rabbitService, IUserService userService)
        {
            _rabbitService = rabbitService;
            _userService = userService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetAllRabbits")]
        public async Task<ActionResult<IEnumerable<Rabbit>>> GetAllRabbits()
        {
            var rabbits = await _rabbitService.GetAllRabbitsAsync();
            return Ok(rabbits);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetRabbitByBreederRegNo/{breederRegNo}")]
        public async Task<ActionResult<IEnumerable<Rabbit>>> GetRabbitsByBreeder(string breederRegNo)
        {
            var rabbits = await _rabbitService.GetAllRabbits_ByBreederRegAsync(breederRegNo);
            if (rabbits == null || !rabbits.Any())
            {
                return NotFound();
            }
            return Ok(rabbits);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddRabbit([FromBody] Rabbit_BasicsDTO newRabbitDto)
        {
            // Get the current user's ID from the User property
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Create a new User_KeyDTO with the user's ID
            var userKeyDto = new User_KeyDTO { BreederRegNo = userId };

            // Pass the User_KeyDTO and Rabbit_BasicsDTO to your service method
            await _rabbitService.AddRabbit_ToCurrentUserAsync(userKeyDto, newRabbitDto);

            return Ok();
        }
    }
}
