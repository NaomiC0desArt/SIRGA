using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Interfaces.Usuarios;
using SIRGA.Domain.Interfaces;
using System.Security.Claims;

namespace SIRGA.API.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Estudiante")]
    public class HorarioController : ControllerBase
    {
        private readonly IHorarioEstudianteService _horarioService;
        private readonly IEstudianteService _estudianteService;

        public HorarioController(
            IHorarioEstudianteService horarioService,
            IEstudianteService estudianteService)
        {
            _horarioService = horarioService;
            _estudianteService = estudianteService;
        }

        [HttpGet("Mi-Horario")]
        public async Task<IActionResult> GetMiHorario()
        {
            var estudianteId = await GetEstudianteIdFromUser();
            if (estudianteId == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            var result = await _horarioService.GetHorarioByEstudianteIdAsync(estudianteId.Value);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("Clases-Hoy")]
        public async Task<IActionResult> GetClasesHoy()
        {
            var estudianteId = await GetEstudianteIdFromUser();
            if (estudianteId == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            var diaActual = DateTime.Now.DayOfWeek;
            var result = await _horarioService.GetClasesDelDiaAsync(estudianteId.Value, diaActual);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("Clases-Por-Dia/{dia}")]
        public async Task<IActionResult> GetClasesPorDia(string dia)
        {
            var estudianteId = await GetEstudianteIdFromUser();
            if (estudianteId == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            if (!Enum.TryParse<DayOfWeek>(dia, true, out var diaSemana))
                return BadRequest(new { message = "Día inválido" });

            var result = await _horarioService.GetClasesDelDiaAsync(estudianteId.Value, diaSemana);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        #region Helper Methods
        private async Task<int?> GetEstudianteIdFromUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return null;

            var result = await _estudianteService.GetEstudianteIdByUserIdAsync(userId);

            // extraer el valor del ApiResponse
            return result.Success ? result.Data : null;
        }
        #endregion
    }
}
