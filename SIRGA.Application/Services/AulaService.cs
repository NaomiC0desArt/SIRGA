using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Entities.Grado;
using SIRGA.Application.Interfaces.Entities;
using SIRGA.Application.Services.Base;
using SIRGA.Domain.Entities;
using SIRGA.Domain.Enum;
using SIRGA.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.Services
{
    public class AulaService : BaseService<Aula, CreateAulaDto, AulaDto>, IAulaService
    {
        private readonly IAulaRepository _aulaRepository;

        public AulaService(
            IAulaRepository aulaRepository,
            ILogger<AulaService> logger)
            : base(aulaRepository, logger)
        {
            _aulaRepository = aulaRepository;
        }

        protected override string EntityName => "Aula";

        protected override Aula MapDtoToEntity(CreateAulaDto dto)
        {
            return new Aula
            {
                Codigo = dto.Codigo.ToUpper(),
                Nombre = dto.Nombre,
                Tipo = (TipoEspacio)dto.Tipo,
                Capacidad = dto.Capacidad,
                EstaDisponible = dto.EstaDisponible
            };
        }

        protected override AulaDto MapEntityToResponse(Aula entity)
        {
            return new AulaDto
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                Nombre = entity.Nombre,
                Tipo = entity.Tipo.ToString(),
                Capacidad = entity.Capacidad,
                EstaDisponible = entity.EstaDisponible,
                NombreCompleto = entity.NombreCompleto
            };
        }

        protected override void UpdateEntityFromDto(Aula entity, CreateAulaDto dto)
        {
            entity.Codigo = dto.Codigo.ToUpper();
            entity.Nombre = dto.Nombre;
            entity.Tipo = (TipoEspacio)dto.Tipo;
            entity.Capacidad = dto.Capacidad;
            entity.EstaDisponible = dto.EstaDisponible;
        }

        protected override async Task<ApiResponse<AulaDto>> ValidateCreateAsync(CreateAulaDto dto)
        {
            var existe = await _aulaRepository.GetByCodigoAsync(dto.Codigo);
            if (existe != null)
            {
                return ApiResponse<AulaDto>.ErrorResponse(
                    $"Ya existe un aula con el código '{dto.Codigo}'");
            }

            return null;
        }

        // Método Update específico para usar UpdateAulaDto
        public async Task<ApiResponse<AulaDto>> UpdateAsync(int id, UpdateAulaDto dto)
        {
            try
            {
                var aula = await _aulaRepository.GetByIdAsync(id);

                if (aula == null)
                    return ApiResponse<AulaDto>.ErrorResponse("Aula no encontrada");

                var existeCodigo = await _aulaRepository.GetByCodigoAsync(dto.Codigo);
                if (existeCodigo != null && existeCodigo.Id != id)
                {
                    return ApiResponse<AulaDto>.ErrorResponse(
                        $"Ya existe otra aula con el código '{dto.Codigo}'");
                }

                aula.Codigo = dto.Codigo.ToUpper();
                aula.Nombre = dto.Nombre;
                aula.Tipo = (TipoEspacio)dto.Tipo;
                aula.Capacidad = dto.Capacidad;
                aula.EstaDisponible = dto.EstaDisponible;

                var updated = await _aulaRepository.UpdateAsync(aula);
                var response = MapEntityToResponse(updated);

                return ApiResponse<AulaDto>.SuccessResponse(
                    response,
                    "Aula actualizada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar aula");
                return ApiResponse<AulaDto>.ErrorResponse(
                    "Error al actualizar el aula",
                    new List<string> { ex.Message });
            }
        }

        // Método específico de Aula
        public async Task<ApiResponse<List<AulaDto>>> GetAulasDisponiblesAsync()
        {
            try
            {
                var aulas = await _aulaRepository.GetAulasDisponiblesAsync();
                var response = aulas.Select(MapEntityToResponse).ToList();

                return ApiResponse<List<AulaDto>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener aulas disponibles");
                return ApiResponse<List<AulaDto>>.ErrorResponse(
                    "Error al obtener aulas disponibles",
                    new List<string> { ex.Message });
            }
        }

        #region Generador de codigo

        public async Task<ApiResponse<string>> GenerarCodigoAsync(TipoEspacio tipo, string nombre)
        {
            try
            {
                // Obtener todas las aulas del mismo tipo
                var aulasDelTipo = (await _aulaRepository.GetAllAsync())
                    .Where(a => a.Tipo == tipo)
                    .ToList();

                // Prefijo según tipo
                string prefijo = tipo switch
                {
                    TipoEspacio.Aula => "A",
                    TipoEspacio.Laboratorio => "LAB",
                    TipoEspacio.Biblioteca => "BIB",
                    TipoEspacio.Cancha => "CAN",
                    _ => "ESP"
                };

                // Si hay nombre, intentar extraer número
                int numeroSugerido = 1;
                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    // Extraer números del nombre (ej: "Aula 101" -> 101)
                    var numeros = new string(nombre.Where(char.IsDigit).ToArray());
                    if (!string.IsNullOrEmpty(numeros) && int.TryParse(numeros, out int num))
                    {
                        numeroSugerido = num;
                    }
                }

                // Generar código
                string codigoGenerado;
                int contador = numeroSugerido;

                do
                {
                    if (tipo == TipoEspacio.Laboratorio)
                    {
                        // Para laboratorios: LAB-QUI-01, LAB-FIS-01, etc.
                        string subtipo = nombre?.ToUpper().Contains("QUIM") == true ? "QUI" :
                                        nombre?.ToUpper().Contains("FIS") == true ? "FIS" :
                                        nombre?.ToUpper().Contains("COMP") == true ? "COMP" :
                                        nombre?.ToUpper().Contains("BIOL") == true ? "BIO" : "GEN";

                        codigoGenerado = $"{prefijo}-{subtipo}-{contador:D2}";
                    }
                    else
                    {
                        // Para otros: A-101, BIB-01, CAN-01
                        codigoGenerado = $"{prefijo}-{contador:D3}";
                    }

                    // Verificar si el código ya existe
                    var existe = aulasDelTipo.Any(a => a.Codigo == codigoGenerado);
                    if (!existe)
                        break;

                    contador++;
                } while (contador < 1000); // Límite de seguridad

                return ApiResponse<string>.SuccessResponse(codigoGenerado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar código de aula");
                return ApiResponse<string>.ErrorResponse("Error al generar código");
            }
        }
        #endregion

    }
}
