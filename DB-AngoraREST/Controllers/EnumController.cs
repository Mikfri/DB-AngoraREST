using DB_AngoraLib;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DB_AngoraREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnumController : ControllerBase
    {
        [HttpGet("Races")]
        public IActionResult GetRaces()
        {
            var races = Enum.GetNames(typeof(Race));
            return Ok(races);
        }

        [HttpGet("Colors")]
        public IActionResult GetColors()
        {
            var colors = Enum.GetNames(typeof(Color));
            return Ok(colors);
        }

        [HttpGet("Genders")]
        public IActionResult GetGenders()
        {
            var genders = Enum.GetNames(typeof(Gender));
            return Ok(genders);
        }
    }

}
