using DB_AngoraLib;
using DB_AngoraLib.DTOs;
using DB_AngoraLib.Services.TransferService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DB_AngoraREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferRequestController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransferRequestController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        //--------------------: POST :--------------------

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost("Create")]
        [Authorize(Roles = "Admin, Breeder, Moderator")]
        public async Task<ActionResult<TransferRequest_ContractDTO>> Create_TransferRequest([FromBody] TransferRequest_CreateDTO createTransferDTO)
        {
            try
            {
                var issuerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (issuerId == null)
                {
                    return Unauthorized("Issuer ID is missing or invalid.");
                }

                var result = await _transferService.CreateTransferRequest(issuerId, createTransferDTO);
                //return Ok(result);
                return CreatedAtAction(nameof(Get_TransferContract), new { transferId = result.Id }, result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("Respond/{transferId}")]
        [Authorize(Roles = "Admin, Breeder, Moderator")]
        public async Task<IActionResult> RespondToTransferRequest(int transferId, [FromBody] TransferRequest_ResponseDTO responseDTO)
        {
            try
            {
                var recipientId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (recipientId == null)
                {
                    return Unauthorized("Bruger ID mangler eller er ugyldigt.");
                }

                var result = await _transferService.Response_TransferRequest(recipientId, transferId, responseDTO);

                if (result == null)
                {
                    return responseDTO.Accept ? NotFound("Overførselsanmodning ikke fundet.") : Ok("Anmodning afvist.");
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Forbid(ex.Message); // Bruger har ikke tilladelse
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Der opstod en fejl under behandlingen af din anmodning.");
            }
        }

        //--------------------: GET :--------------------

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("Get/{transferId}")]
        [Authorize(Roles = "Admin, Breeder, Moderator")]
        public async Task<ActionResult<TransferRequest_ContractDTO>> Get_TransferContract(int transferId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized("Bruger ID mangler eller er ugyldigt.");
                }

                var result = await _transferService.Get_RabbitTransfer_Contract(userId, transferId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message); 
            }
            catch (Exception ex)
            {
                // Log fejlen
                return StatusCode(StatusCodes.Status500InternalServerError, "Der opstod en fejl under behandlingen af din anmodning.");
            }
        }

        //--------------------: DELETE :--------------------
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpDelete("Delete/{transferRequestId}")]
        [Authorize(Roles = "Admin, Breeder, Moderator")]
        public async Task<ActionResult<TransferRequest_PreviewDTO>> Delete_TransferRequest(int transferRequestId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized("Bruger ID mangler eller er ugyldigt.");
                }

                var result = await _transferService.DeleteTransferRequest(userId, transferRequestId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message); // Bruger har ikke tilladelse
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message); // Bruger har ikke tilladelse
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Operationen er ikke tilladt
            }
            catch (Exception ex)
            {
                // Log fejlen
                return StatusCode(StatusCodes.Status500InternalServerError, "Der opstod en fejl under behandlingen af din anmodning.");
            }
        }


    }
}
