using DB_AngoraLib.Models;
using DB_AngoraREST.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace DB_AngoraREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(string email, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Bruger ikke fundet
                return NotFound();
            }

            // Fjern den gamle adgangskode
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                // Fejl ved fjernelse af adgangskode
                return BadRequest(removePasswordResult.Errors);
            }

            // Tilføj den nye adgangskode
            var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
            if (!addPasswordResult.Succeeded)
            {
                // Fejl ved tilføjelse af adgangskode
                return BadRequest(addPasswordResult.Errors);
            }

            return Ok();
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO regDTO)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = regDTO.BreederRegNo,
                    Email = regDTO.Email,
                    PhoneNumber = regDTO.PhoneNum,
                    FirstName = regDTO.FirstName,
                    LastName = regDTO.LastName,
                    City = regDTO.City,
                    RoadNameAndNo = regDTO.RoadNameAndNo,
                    ZipCode = regDTO.ZipCode,
                };

                var result = await _userManager.CreateAsync(user, regDTO.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok();
                }

                // Hvis der er fejl, tilføj dem til ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Hvis vi er nået hertil, er der noget galt, vis formular igen
            return BadRequest(ModelState);
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return Ok();
                }

                if (result.RequiresTwoFactor)
                {
                    // Håndter to-faktor godkendelse her, hvis det er aktiveret
                }

                if (result.IsLockedOut)
                {
                    // Håndter låste konti her
                }

                // Hvis vi er nået hertil, er der noget galt, vis formular igen
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                // Udskriv detaljer om SignInResult
                Console.WriteLine($"Login failed: {result.ToString()}");

                return BadRequest(ModelState);
            }

            // Hvis vi er nået hertil, er der noget galt, vis formular igen
            return BadRequest(ModelState);
        }


    }
}
