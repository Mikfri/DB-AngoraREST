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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllRabbits")]
        public async Task<ActionResult<IEnumerable<Rabbit>>> GetAllRabbits()
        {
            var rabbits = await _rabbitService.GetAllRabbitsAsync();
            return Ok(rabbits);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("GetRabbitByEarTags/{rightEarId}-{leftEarId}")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<ActionResult<Rabbit>> GetRabbitByEarTags(string rightEarId, string leftEarId)
        {
            var rabbit = await _rabbitService.GetRabbitByEarTagsAsync(rightEarId, leftEarId);
            if (rabbit == null)
            {
                return NotFound();
            }
            return Ok(rabbit);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("GetRabbitByUserId/{breederRegNo}")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<ActionResult<IEnumerable<Rabbit>>> GetRabbitsByBreeder(string breederRegNo)
        {
            var rabbits = await _rabbitService.GetAllRabbits_ByBreederRegAsync(breederRegNo);
            if (rabbits == null || !rabbits.Any())
            {
                return NotFound();
            }
            return Ok(rabbits);
        }

        //[ProducesResponseType(StatusCodes.Status200OK)]    // TODO: Får vi 201 Created tilbage?
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[HttpPost("AddRabbit_ToMyCollection")]
        //[Authorize(Roles = "Admin, Breeder, Moderator")]
        //public async Task<IActionResult> AddRabbit([FromBody] RabbitDTO newRabbitDto)
        //{
        //    // Get the current user's ID from the User property
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    // Pass the userId and newRabbitDto to your service method
        //    await _rabbitService.AddRabbit_ToCurrentUserAsync(userId, newRabbitDto);

        //    return Ok();
        //}

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost("AddRabbit_ToMyCollection")]
        [Authorize(Roles = "Admin, Breeder, Moderator")]
        public async Task<IActionResult> AddRabbit([FromBody] RabbitDTO newRabbitDto)
        {
            // Get the current user's ID from the User property
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                // Pass the userId and newRabbitDto to your service method
                var createdRabbit = await _rabbitService.AddRabbit_ToMyCollectionAsync(userId, newRabbitDto);

                // Use CreatedAtAction with GetRabbitByEarTags
                return CreatedAtAction(nameof(GetRabbitByEarTags), new { rightEarId = createdRabbit.RightEarId, leftEarId = createdRabbit.LeftEarId }, createdRabbit);
            }
            catch (InvalidOperationException ex)    // ved at have catch på vil RabbitService fejl beskeden kunne sendes tilbage til klienten
            {
                // Log the error here if needed
                return BadRequest(new { message = ex.Message });
            }
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin, Moderator, Breeder")]
        [HttpPut("UpdateMyRabbit/{rightEarId}-{leftEarId}")]
        public async Task<IActionResult> UpdateMyRabbit(string rightEarId, string leftEarId, [FromBody] Rabbit_UpdateDTO updatedRabbit)
        {
            // Get the current user's ID from the User property
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the current user's claims
            var userClaims = User.Claims.ToList();

            // Get the current user
            var currentUser = await _userService.GetUserByIdAsync(userId);

            try
            {
                // Pass the currentUser, rightEarId, leftEarId, updatedRabbit, and userClaims to your service method
                await _rabbitService.UpdateRabbit_RBAC_Async(currentUser, rightEarId, leftEarId, updatedRabbit, userClaims);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin, Moderator, Breeder")]
        [HttpDelete("DeleteMyRabbit/{rightEarId}-{leftEarId}")]
        public async Task<IActionResult> DeleteMyRabbit(string rightEarId, string leftEarId)
        {
            // Get the current user's ID from the User property
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the current user's claims
            var userClaims = User.Claims.ToList();

            // Get the current user
            var currentUser = await _userService.GetUserByIdAsync(userId);

            try
            {
                // Pass the currentUser, rightEarId, leftEarId, and userClaims to your service method
                await _rabbitService.DeleteRabbit_RBAC_Async(currentUser, rightEarId, leftEarId, userClaims);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok();
        }

    }
}
