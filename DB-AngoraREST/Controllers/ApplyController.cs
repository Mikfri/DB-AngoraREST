using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DB_AngoraREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplyController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IAccountService _accountService;


        public ApplyController(IApplicationService applicationService, IAccountService accountService)
        {
            _applicationService = applicationService;
            _accountService = accountService;
        }

        // POST metode til at ansøge om avlerrolle
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //NB! AUTHORIZE er nødvendig for at kunne hente userId fra Claims - 3+ hours spent on this -.-'
        [Authorize]
        [HttpPost("Breeder")]
        public async Task<IActionResult> ApplyForBreederRole([FromBody] Application_BreederDTO applicationDto)
        {
            try
            {
                // Henter userId fra den autentificerede brugers Claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Bruger ikke fundet");
                }

                await _applicationService.ApplyForBreederRoleAsync(userId, applicationDto);
                return Ok("Ansøgning modtaget");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST metode til at godkende en ansøgning
        [HttpPost("Breeder/Approve/{applicationId}")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> ApproveApplication(int applicationId)
        {
            try
            {
                await _applicationService.ApproveApplicationAsync(applicationId);
                return Ok("Ansøgning godkendt");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST metode til at afvise en ansøgning
        [HttpPost("Breeder/Reject/{applicationId}")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> RejectApplication(int applicationId, [FromBody] string rejectionReason)
        {
            try
            {
                await _applicationService.RejectApplicationAsync(applicationId, rejectionReason);
                return Ok("Ansøgning afvist");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET metode til at hente alle afventende ansøgninger
        [HttpGet("Pending")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<ActionResult<IEnumerable<BreederApplication>>> GetPendingApplications()
        {
            var applications = await _applicationService.GetPendingApplicationsAsync();
            return Ok(applications);
        }
    }
}
