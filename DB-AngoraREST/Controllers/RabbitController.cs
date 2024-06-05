using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.RabbitService;
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
        private readonly IAccountService _userService;

        public RabbitController(IRabbitService rabbitService, IAccountService userService)
        {
            _rabbitService = rabbitService;
            _userService = userService;
        }

        //-------------------------------: POST

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost("Create")]
        [Authorize(Roles = "Admin, Breeder, Moderator")]
        public async Task<ActionResult<Rabbit_ProfileDTO>> AddRabbit([FromBody] Rabbit_CreateDTO newRabbitDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                // Pass the userId and newRabbitDto to your service method
                var createdRabbit = await _rabbitService.AddRabbit_ToMyCollectionAsync(userId, newRabbitDto);

                // Use CreatedAtAction with GetRabbit_ProfileByEarTags
                return CreatedAtAction(nameof(GetRabbit_ProfileByEarTags), new { earCombId = createdRabbit.EarCombId }, createdRabbit);
            }
            catch (InvalidOperationException ex)    // ved at have catch på vil RabbitService fejl beskeden kunne sendes tilbage til klienten
            {
                // Log the error here if needed
                return BadRequest(new { message = ex.Message });
            }
        }



        //-------------------------------: GET

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Rabbit>>> GetAllRabbits()
        {
            var rabbits = await _rabbitService.GetAllRabbitsAsync();
            return Ok(rabbits);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("Profile/{earCombId}")]
        [Authorize(Roles = "Admin, Moderator, Breeder")]
        public async Task<ActionResult<Rabbit_ProfileDTO>> GetRabbit_ProfileByEarTags(string earCombId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userClaims = User.Claims.ToList();
            var rabbitProfile = await _rabbitService.GetRabbit_ProfileAsync(currentUserId, earCombId, userClaims);

            if (rabbitProfile == null)
            {
                return NotFound();
            }

            return Ok(rabbitProfile);
        }



        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("ByBreeder/{breederRegNo}")]
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("Forsale")]
        public async Task<ActionResult<List<Rabbit_PreviewDTO>>> GetAllRabbits_OpenProfile_Filtered(
        [FromQuery] string rightEarId = null,
        [FromQuery] string nickName = null,
        [FromQuery] string race = null,
        [FromQuery] string color = null,
        [FromQuery] string gender = null,
        [FromQuery] bool? isJuvenile = null,
        [FromQuery] bool? approvedRaceColorCombination = null)
        {
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

            var filter = new Rabbit_ForsaleFilterDTO
            {
                RightEarId = rightEarId,
                NickName = nickName,
                Race = raceEnum,
                Color = colorEnum,
                Gender = genderEnum,
                IsJuvenile = isJuvenile,
                ApprovedRaceColorCombination = approvedRaceColorCombination
            };

            try
            {
                var filteredRabbits = await _rabbitService.GetAllRabbits_Forsale_Filtered(filter);
                return Ok(filteredRabbits);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        //-------------------------------: PUT
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Policy = "UpdateRabbit")]
        [HttpPut("Update/{earCombId}")]
        public async Task<ActionResult<Rabbit_ProfileDTO>> UpdateRabbit(string earCombId, [FromBody] Rabbit_UpdateDTO updatedRabbit)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userClaims = User.Claims.ToList();

            try
            {
                var updatedRabbitDTO = await _rabbitService.UpdateRabbit_RBAC_Async(userId, earCombId, updatedRabbit, userClaims);
                if (updatedRabbitDTO == null)
                {
                    return NotFound();
                }
                return Ok(updatedRabbitDTO);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //-------------------------------: DELETE

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Policy = "DeleteRabbit")]
        [HttpDelete("Delete/{earCombId}")]
        public async Task<ActionResult<Rabbit_PreviewDTO>> DeleteRabbit(string earCombId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userClaims = User.Claims.ToList();

            try
            {
                var rabbitPreviewDTO = await _rabbitService.DeleteRabbit_RBAC_Async(userId, earCombId, userClaims);
                return Ok(rabbitPreviewDTO);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



    }
}
