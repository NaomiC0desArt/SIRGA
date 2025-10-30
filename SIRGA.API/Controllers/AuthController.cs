using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SIRGA.Application.DTOs.Identity;
using SIRGA.Identity.Interfaces;
using System.Security.Claims;
using System.Text;

namespace SIRGA.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.AuthenticateAsync(request);

            if (!result.Success)
                return Unauthorized(result);


            return Ok(result);
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _accountService.SignOutAsync();
            return Ok(result);
        }

        /// verifica si el usuario está autenticado y retorna sus datos
        [Authorize]
        [HttpGet("Check")]
        public IActionResult CheckAuth()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var firstName = User.FindFirstValue("FirstName");
            var lastName = User.FindFirstValue("LastName");
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Ok(new
            {
                success = true,
                data = new
                {
                    authenticated = true,
                    userId = userId,
                    email = email,
                    firstName = firstName,
                    lastName = lastName,
                    roles = roles
                }
            });
        }

        
        [Authorize]
        [HttpPost("Change-Password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            request.UserId = userId;

            var result = await _accountService.ChangePasswordAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // usar el origin que viene del frontend en el request
            var origin = request.Origin;

            var result = await _accountService.ForgotPasswordAsync(request, origin);

            return Ok(result);
        }

        
        [AllowAnonymous]
        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.ResetPasswordAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        
        [AllowAnonymous]
        [HttpGet("Confirm-Email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest(new { message = "UserId y Token son requeridos" });

            var result = await _accountService.ConfirmEmailAsync(userId, token);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        
        [AllowAnonymous]
        [HttpGet("User-Exists/{email}")]
        public async Task<IActionResult> UserExists(string email)
        {
            var result = await _accountService.UserExistsAsync(email);
            return Ok(result);
        }
    }
}
