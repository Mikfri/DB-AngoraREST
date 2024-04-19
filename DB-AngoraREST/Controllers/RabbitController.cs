using DB_AngoraLib.Models;
using DB_AngoraLib.Services.RabbitService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DB_AngoraREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitController : ControllerBase
    {
        private readonly IRabbitService _rabbitService;

        public RabbitController(IRabbitService rabbitService)
        {
            _rabbitService = rabbitService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rabbit>>> GetRabbits()
        {
            var rabbits = await _rabbitService.GetAllRabbitsAsync();
            return Ok(rabbits);
        }

    }
}
