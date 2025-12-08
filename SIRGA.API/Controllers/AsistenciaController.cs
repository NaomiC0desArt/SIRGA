using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Asistencia;
using SIRGA.Application.DTOs.ResponseDto;
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

        #region Endpoints para Profesores - Toma de Asistencia

        /// Obtiene las clases del día para un profesor
        [Authorize(Roles = "Profesor,Admin")]
        [HttpGet("Profesor/{idProfesor}/Clases-del-Dia")]
        public async Task<IActionResult> GetClasesDelDia(
            int idProfesor,
            [FromQuery] DateTime? fecha = null)
        {
            var result = await _asistenciaService.GetClasesDelDiaAsync(
                idProfesor,
                fecha ?? DateTime.Today);

            return result.Success ? Ok(result) : NotFound(result);
        }

        /// Obtiene la lista de estudiantes de una clase para tomar asistencia
        [Authorize(Roles = "Profesor,Admin")]
        [HttpGet("Clase/{idClaseProgramada}/Estudiantes")]
        public async Task<IActionResult> GetEstudiantesPorClase(
            int idClaseProgramada,
            [FromQuery] DateTime? fecha = null)
        {
            var result = await _asistenciaService.GetEstudiantesPorClaseAsync(
                idClaseProgramada,
                fecha ?? DateTime.Today);

            return result.Success ? Ok(result) : NotFound(result);
        }

        /// Registra asistencia individual
        [Authorize(Roles = "Profesor,Admin")]
        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarAsistencia([FromBody] RegistrarAsistenciaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _asistenciaService.RegistrarAsistenciaAsync(
                dto,
                GetUserId());

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtRoute(
                "GetAsistenciaById",
                new { id = result.Data.Id },
                result);
        }

        /// Registra asistencia masiva (toda la clase)
        [Authorize(Roles = "Profesor,Admin")]
        [HttpPost("Registrar-Masiva")]
        public async Task<IActionResult> RegistrarAsistenciaMasiva([FromBody] RegistrarAsistenciaMasivaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _asistenciaService.RegistrarAsistenciaMasivaAsync(
                dto,
                GetUserId());

            return result.Success ? Ok(result) : BadRequest(result);
        }

        #endregion

        #region Endpoints de Administración

        /// Actualiza una asistencia (solo Admin)
        [Authorize(Roles = "Admin")]
        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> ActualizarAsistencia(
            int id,
            [FromBody] ActualizarAsistenciaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _asistenciaService.ActualizarAsistenciaAsync(
                id,
                dto,
                GetUserId());

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// Justifica una asistencia (Profesor y Admin)
        [Authorize(Roles = "Profesor,Admin")]
        [HttpPatch("{id:int}/Justificar")]
        public async Task<IActionResult> JustificarAsistencia(
            int id,
            [FromBody] JustificarAsistenciaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _asistenciaService.JustificarAsistenciaAsync(
                id,
                dto,
                GetUserId());

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// Obtiene asistencias que requieren justificación
        [Authorize(Roles = "Admin")]
        [HttpGet("Requieren-Justificacion")]
        public async Task<IActionResult> GetAsistenciasRequierenJustificacion()
        {
            var result = await _asistenciaService.GetAsistenciasRequierenJustificacionAsync();
            return Ok(result);
        }

        /// Elimina una asistencia (solo Admin)
        [Authorize(Roles = "Admin")]
        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> EliminarAsistencia(int id)
        {
            var result = await _asistenciaService.EliminarAsistenciaAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        #endregion

        #region Endpoints de Consulta

        /// Obtiene una asistencia por ID
        [Authorize(Roles = "Profesor,Admin")]
        [HttpGet("{id:int}", Name = "GetAsistenciaById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _asistenciaService.GetAsistenciaByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// Obtiene el historial de asistencia de un estudiante
        [Authorize(Roles = "Profesor,Admin,Estudiante")]
        [HttpGet("Estudiante/{idEstudiante}/Historial")]
        public async Task<IActionResult> GetHistorialEstudiante(
            int idEstudiante,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            var result = await _asistenciaService.GetHistorialAsistenciaEstudianteAsync(
                idEstudiante,
                fechaInicio,
                fechaFin);

            return Ok(result);
        }

        /// Obtiene el historial de asistencia de una clase
        [Authorize(Roles = "Profesor,Admin")]
        [HttpGet("Clase/{idClaseProgramada}/Historial")]
        public async Task<IActionResult> GetHistorialClase(
            int idClaseProgramada,
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            var result = await _asistenciaService.GetHistorialAsistenciaClaseAsync(
                idClaseProgramada,
                fechaInicio,
                fechaFin);

            LogHistorialInfo(result);

            return Ok(result);
        }

        /// Obtiene estadísticas de asistencia de un estudiante
        [Authorize(Roles = "Profesor,Admin,Estudiante")]
        [HttpGet("Estudiante/{idEstudiante}/Estadisticas")]
        public async Task<IActionResult> GetEstadisticasEstudiante(
            int idEstudiante,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            var result = await _asistenciaService.GetEstadisticasEstudianteAsync(
                idEstudiante,
                fechaInicio,
                fechaFin);

            return Ok(result);
        }

        #endregion

        #region Helper Methods

        /// Obtiene el ID del usuario autenticado
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Usuario no autenticado");
        }

        /// Registra información del historial (para debugging)
        private void LogHistorialInfo(ApiResponse<List<AsistenciaResponseDto>> result)
        {
            if (!result.Success || result.Data == null)
                return;

            _logger.LogInformation("Historial obtenido - Total: {Count}", result.Data.Count);

            if (result.Data.Any())
            {
                var primera = result.Data.First();
                _logger.LogInformation(
                    "Primera asistencia - Estado: {Estado}, Justificación: {Justificacion}",
                    primera.Estado,
                    primera.Justificacion ?? "NULL");
            }
        }

        #endregion
    }
}
