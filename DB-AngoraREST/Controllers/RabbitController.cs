using DB_AngoraLib.Models;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rabbit>>> GetRabbits()
        {
            var rabbits = await _rabbitService.GetAllRabbitsAsync();
            return Ok(rabbits);
        }
    }
}
