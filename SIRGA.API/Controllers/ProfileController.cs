using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Identity;
using SIRGA.Identity.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SIRGA.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        
        [HttpGet("My-Profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Usuario no autenticado" });

            var result = await _profileService.GetUserProfileAsync(userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// Obtiene el perfil de otro usuario 
        
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            // Solo admin puede ver perfiles de otros usuarios o el mismo usuario puede ver su propio perfil
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && currentUserId != userId)
                return Forbid();

            var result = await _profileService.GetUserProfileAsync(userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Usuario no autenticado" });

            var result = await _profileService.UpdateProfileAsync(userId, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("Update-Photo")]
        public async Task<IActionResult> UpdatePhoto([FromBody] UpdatePhotoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Usuario no autenticado" });

            var result = await _profileService.UpdateProfilePhotoAsync(userId, dto.PhotoBase64);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

       
        [HttpPost("Basic-Info")]
        public async Task<IActionResult> GetUsersBasicInfo([FromBody] List<string> userIds)
        {
            if (userIds == null || !userIds.Any())
                return BadRequest(new { message = "Debe proporcionar al menos un userId" });

            var result = await _profileService.GetUsersBasicInfoAsync(userIds);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }

    // DTO para actualizar foto
    public class UpdatePhotoDto
    {
        [Required]
        public string PhotoBase64 { get; set; }
    }
}
