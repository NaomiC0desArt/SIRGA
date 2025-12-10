using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.DTOs.ResponseDto;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services.Base;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services
{
    public class CursoAcademicoService : BaseService<CursoAcademico, CreateCursoAcademicoDto, CursoAcademicoDto>, ICursoAcademicoService
    {
        private readonly ICursoAcademicoRepository _cursoAcademicoRepository;

        public CursoAcademicoService(
            ICursoAcademicoRepository cursoAcademicoRepository,
            ILogger<CursoAcademicoService> logger)
            : base(cursoAcademicoRepository, logger)
        {
            _cursoAcademicoRepository = cursoAcademicoRepository;
        }

        protected override string EntityName => "Curso Académico";

        protected override CursoAcademico MapDtoToEntity(CreateCursoAcademicoDto dto)
        {
            return new CursoAcademico
            {
                IdGrado = dto.IdGrado,
                IdSeccion = dto.IdSeccion,
                IdAnioEscolar = dto.IdAnioEscolar,
                IdAulaBase = dto.IdAulaBase
            };
        }

        protected override CursoAcademicoDto MapEntityToResponse(CursoAcademico entity)
        {
            return new CursoAcademicoDto
            {
                Id = entity.Id,
                IdGrado = entity.IdGrado,
                IdSeccion = entity.IdSeccion,
                IdAnioEscolar = entity.IdAnioEscolar,
                IdAulaBase = entity.IdAulaBase,
                Grado = entity.Grado != null ? new GradoDto
                {
                    Id = entity.Grado.Id,
                    GradeName = entity.Grado.GradeName,
                    Nivel = entity.Grado.Nivel.ToString()
                } : null,
                Seccion = entity.Seccion != null ? new SeccionDto
                {
                    Id = entity.Seccion.Id,
                    Nombre = entity.Seccion.Nombre,
                    CapacidadMaxima = entity.Seccion.CapacidadMaxima
                } : null,
                AnioEscolar = entity.AnioEscolar != null ? new AnioEscolarDto
                {
                    Id = entity.AnioEscolar.Id,
                    AnioInicio = entity.AnioEscolar.AnioInicio,
                    AnioFin = entity.AnioEscolar.AnioFin,
                    Activo = entity.AnioEscolar.Activo,
                    Periodo = entity.AnioEscolar.Periodo
                } : null,
                AulaBase = entity.AulaBase != null ? new AulaDto
                {
                    Id = entity.AulaBase.Id,
                    Nombre = entity.AulaBase.Nombre,
                    Codigo = entity.AulaBase.Codigo,
                    Tipo = entity.AulaBase.Tipo.ToString(),
                    Capacidad = entity.AulaBase.Capacidad,
                    EstaDisponible = entity.AulaBase.EstaDisponible,
                    NombreCompleto = entity.AulaBase.NombreCompleto
                } : null,
                NombreCompleto = entity.NombreCompleto
            };
        }

        protected override void UpdateEntityFromDto(CursoAcademico entity, CreateCursoAcademicoDto dto)
        {
            entity.IdGrado = dto.IdGrado;
            entity.IdSeccion = dto.IdSeccion;
            entity.IdAnioEscolar = dto.IdAnioEscolar;
            entity.IdAulaBase = dto.IdAulaBase;
        }

        protected override async Task<ApiResponse<CursoAcademicoDto>> ValidateCreateAsync(CreateCursoAcademicoDto dto)
        {
            var existe = await _cursoAcademicoRepository.ExisteCursoAsync(
                dto.IdGrado,
                dto.IdSeccion,
                dto.IdAnioEscolar);

            if (existe)
            {
                return ApiResponse<CursoAcademicoDto>.ErrorResponse(
                    "Ya existe un curso académico con esta combinación de grado, sección y año escolar");
            }

            return null;
        }

        protected override async Task<ApiResponse<CursoAcademicoDto>> ValidateUpdateAsync(int id, CreateCursoAcademicoDto dto)
        {
            var existe = await _cursoAcademicoRepository.ExisteCursoAsync(
                dto.IdGrado,
                dto.IdSeccion,
                dto.IdAnioEscolar,
                excludeId: id);

            if (existe)
            {
                return ApiResponse<CursoAcademicoDto>.ErrorResponse(
                    "Ya existe otro curso académico con esta combinación de grado, sección y año escolar");
            }

            return null;
        }

        // Recargar con navegación después de crear/actualizar
        protected override async Task<CursoAcademico> AfterCreateAsync(CursoAcademico entity)
        {
            return await _cursoAcademicoRepository.GetByIdWithDetailsAsync(entity.Id);
        }

        protected override async Task<CursoAcademico> AfterUpdateAsync(CursoAcademico entity)
        {
            return await _cursoAcademicoRepository.GetByIdWithDetailsAsync(entity.Id);
        }

        // Override para GetAll - incluir detalles
        public override async Task<ApiResponse<List<CursoAcademicoDto>>> GetAllAsync()
        {
            try
            {
                var cursos = await _cursoAcademicoRepository.GetAllWithDetailsAsync();
                var responses = cursos.Select(MapEntityToResponse).ToList();

                return ApiResponse<List<CursoAcademicoDto>>.SuccessResponse(
                    responses,
                    $"{EntityName}s obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener {EntityName}s");
                return ApiResponse<List<CursoAcademicoDto>>.ErrorResponse(
                    $"Error al obtener {EntityName}s",
                    new List<string> { ex.Message });
            }
        }

        // Override para GetById - incluir detalles
        public override async Task<ApiResponse<CursoAcademicoDto>> GetByIdAsync(int id)
        {
            try
            {
                var curso = await _cursoAcademicoRepository.GetByIdWithDetailsAsync(id);

                if (curso == null)
                    return ApiResponse<CursoAcademicoDto>.ErrorResponse($"{EntityName} no encontrado");

                var response = MapEntityToResponse(curso);
                return ApiResponse<CursoAcademicoDto>.SuccessResponse(
                    response,
                    $"{EntityName} obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener {EntityName}");
                return ApiResponse<CursoAcademicoDto>.ErrorResponse(
                    $"Error al obtener {EntityName}",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<SeccionDto>>> GetSeccionesDisponiblesAsync(int idGrado, int idAnioEscolar)
        {
            try
            {
                // Obtener todos los cursos para ese grado y año escolar
                var cursosExistentes = await _cursoAcademicoRepository.GetByAnioEscolarAsync(idAnioEscolar);
                var seccionesUsadas = cursosExistentes
                    .Where(c => c.IdGrado == idGrado)
                    .Select(c => c.IdSeccion)
                    .ToList();

                // Aquí necesitarías acceso al repositorio de Secciones
                // Por ahora, retorno una lista vacía o puedes inyectar ISeccionRepository

                _logger.LogInformation($"Consultando secciones disponibles para Grado {idGrado} y Año Escolar {idAnioEscolar}");

                // Opción temporal: retornar lista vacía
                return ApiResponse<List<SeccionDto>>.SuccessResponse(
                    new List<SeccionDto>(),
                    "Consulta realizada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener secciones disponibles");
                return ApiResponse<List<SeccionDto>>.ErrorResponse(
                    "Error al obtener secciones disponibles",
                    new List<string> { ex.Message });
            }
        }
    }
}

