using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Entities.Asistencia;
using SIRGA.Application.Interfaces.Entities;
using System.Security.Claims;

namespace SIRGA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsistenciaController : ControllerBase
    {
        private readonly IAsistenciaService _asistenciaService;
        private readonly ILogger<AsistenciaController> _logger;

        public AsistenciaController(
            IAsistenciaService asistenciaService,
            ILogger<AsistenciaController> logger)
        {
            _asistenciaService = asistenciaService;
            _logger = logger;
        }

        // ============ ENDPOINTS PARA PROFESORES ============

        /// <summary>
        /// Obtiene las clases del día para un profesor
        /// </summary>
        [Authorize(Roles = "Profesor,Admin")]
        [HttpGet("Profesor/{idProfesor}/Clases-del-Dia")]
        public async Task<IActionResult> GetClasesDelDia(int idProfesor, [FromQuery] DateTime? fecha = null)
        {
            var fechaConsulta = fecha ?? DateTime.Today;
            var result = await _asistenciaService.GetClasesDelDiaAsync(idProfesor, fechaConsulta);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Obtiene la lista de estudiantes de una clase para tomar asistencia
        /// </summary>
        [Authorize(Roles = "Profesor,Admin")]
        [HttpGet("Clase/{idClaseProgramada}/Estudiantes")]
        public async Task<IActionResult> GetEstudiantesPorClase(
            int idClaseProgramada,
            [FromQuery] DateTime? fecha = null)
        {
            var fechaConsulta = fecha ?? DateTime.Today;
            var result = await _asistenciaService.GetEstudiantesPorClaseAsync(idClaseProgramada, fechaConsulta);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Registra asistencia individual
        /// </summary>
        [Authorize(Roles = "Profesor,Admin")]
        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarAsistencia([FromBody] RegistrarAsistenciaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _asistenciaService.RegistrarAsistenciaAsync(dto, userId);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtRoute("GetAsistenciaById", new { id = result.Data.Id }, result);
        }

        /// <summary>
        /// Registra asistencia masiva (toda la clase)
        /// </summary>
        [Authorize(Roles = "Profesor,Admin")]
        [HttpPost("Registrar-Masiva")]
        public async Task<IActionResult> RegistrarAsistenciaMasiva([FromBody] RegistrarAsistenciaMasivaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _asistenciaService.RegistrarAsistenciaMasivaAsync(dto, userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // ============ ENDPOINTS PARA ADMIN ============

        /// <summary>
        /// Actualiza una asistencia (solo Admin)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> ActualizarAsistencia(int id, [FromBody] ActualizarAsistenciaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _asistenciaService.ActualizarAsistenciaAsync(id, dto, userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Justifica una asistencia (Profesor y Admin)
        /// </summary>
        [Authorize(Roles = "Profesor,Admin")]
        [HttpPatch("{id:int}/Justificar")]
        public async Task<IActionResult> JustificarAsistencia(int id, [FromBody] JustificarAsistenciaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _asistenciaService.JustificarAsistenciaAsync(id, dto, userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Obtiene asistencias que requieren justificación
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("Requieren-Justificacion")]
        public async Task<IActionResult> GetAsistenciasRequierenJustificacion()
        {
            var result = await _asistenciaService.GetAsistenciasRequierenJustificacionAsync();
            return Ok(result);
        }

        /// <summary>
        /// Elimina una asistencia (solo Admin)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> EliminarAsistencia(int id)
        {
            var result = await _asistenciaService.EliminarAsistenciaAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // ============ ENDPOINTS DE CONSULTA ============

        /// <summary>
        /// Obtiene una asistencia por ID
        /// </summary>
        [Authorize(Roles = "Profesor,Admin")]
        [HttpGet("{id:int}", Name = "GetAsistenciaById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _asistenciaService.GetAsistenciaByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Obtiene el historial de asistencia de un estudiante
        /// </summary>
        [Authorize(Roles = "Profesor,Admin,Estudiante")]
        [HttpGet("Estudiante/{idEstudiante}/Historial")]
        public async Task<IActionResult> GetHistorialEstudiante(
            int idEstudiante,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            var result = await _asistenciaService.GetHistorialAsistenciaEstudianteAsync(
                idEstudiante, fechaInicio, fechaFin);

            return Ok(result);
        }

        /// <summary>
        /// Obtiene el historial de asistencia de una clase
        /// </summary>
        [Authorize(Roles = "Profesor,Admin")]
        [HttpGet("Clase/{idClaseProgramada}/Historial")]
        public async Task<IActionResult> GetHistorialClase(
            int idClaseProgramada,
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            var result = await _asistenciaService.GetHistorialAsistenciaClaseAsync(
                idClaseProgramada, fechaInicio, fechaFin);

            return Ok(result);
        }

        /// <summary>
        /// Obtiene estadísticas de asistencia de un estudiante
        /// </summary>
        [Authorize(Roles = "Profesor,Admin,Estudiante")]
        [HttpGet("Estudiante/{idEstudiante}/Estadisticas")]
        public async Task<IActionResult> GetEstadisticasEstudiante(
            int idEstudiante,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            var result = await _asistenciaService.GetEstadisticasEstudianteAsync(
                idEstudiante, fechaInicio, fechaFin);

            return Ok(result);
        }
    }
}
