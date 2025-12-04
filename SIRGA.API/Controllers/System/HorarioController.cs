using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.Interfaces.Entities;
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
        private readonly IEstudianteRepository _estudianteRepository;

        public HorarioController(
            IHorarioEstudianteService horarioService,
            IEstudianteRepository estudianteRepository)
        {
            _horarioService = horarioService;
            _estudianteRepository = estudianteRepository;
        }

        [HttpGet("Mi-Horario")]
        public async Task<IActionResult> GetMiHorario()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            // Obtener el estudiante por ApplicationUserId
            var estudiante = await _estudianteRepository.GetByApplicationUserIdAsync(userId);

            if (estudiante == null)
            {
                return NotFound(new { message = "Estudiante no encontrado" });
            }

            var result = await _horarioService.GetHorarioByEstudianteIdAsync(estudiante.Id);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("Clases-Hoy")]
        public async Task<IActionResult> GetClasesHoy()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var estudiante = await _estudianteRepository.GetByApplicationUserIdAsync(userId);

            if (estudiante == null)
            {
                return NotFound(new { message = "Estudiante no encontrado" });
            }

            var diaActual = DateTime.Now.DayOfWeek;
            var result = await _horarioService.GetClasesDelDiaAsync(estudiante.Id, diaActual);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("Clases-Por-Dia/{dia}")]
        public async Task<IActionResult> GetClasesPorDia(string dia)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var estudiante = await _estudianteRepository.GetByApplicationUserIdAsync(userId);

            if (estudiante == null)
            {
                return NotFound(new { message = "Estudiante no encontrado" });
            }

            // Convertir string a DayOfWeek
            if (!Enum.TryParse<DayOfWeek>(dia, true, out var diaSemana))
            {
                return BadRequest(new { message = "Día inválido" });
            }

            var result = await _horarioService.GetClasesDelDiaAsync(estudiante.Id, diaSemana);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
