using DB_AngoraLib;
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
        [HttpPost("ApplicationBreeder")]
        public async Task<IActionResult> ApplyForBreederRole([FromBody] ApplicationBreeder_CreateDTO applicationDto)
        {
            try
            {
                // Henter userId fra den autentificerede brugers Claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Bruger ikke fundet");
                }

                await _applicationService.Apply_ApplicationBreeder(userId, applicationDto);
                return Ok("Ansøgning modtaget");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

        // GET metode til at hente alle afventende ansøgninger
        [HttpGet("Get/ApplicationBreeder/Pending")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<ActionResult<IEnumerable<ApplicationBreeder>>> GetPendingApplications()
        {
            var applications = await _applicationService.GetAll_ApplicationBreeder_Pending();
            return Ok(applications);
        }
    }
}
