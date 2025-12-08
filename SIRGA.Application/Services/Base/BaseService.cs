

using Microsoft.Extensions.Logging;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.Interfaces.Base;
using SIRGA.Domain.Interfaces;

namespace SIRGA.Application.Services.Base
{
    public abstract class BaseService<TEntity, TEntityDto, TResponse> : IBaseServices<TEntityDto, TResponse>
        where TEntity : class
        where TResponse : class
    {
        protected readonly IGenericRepository<TEntity> _repository;
        protected readonly ILogger _logger;

        protected BaseService(IGenericRepository<TEntity> repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        #region Métodos Abstractos - Para personalizar en servicios hijos
        protected abstract TEntity MapDtoToEntity(TEntityDto dto);
        protected abstract TResponse MapEntityToResponse(TEntity entity);
        protected abstract void UpdateEntityFromDto(TEntity entity, TEntityDto dto);
        protected abstract string EntityName { get; }
        #endregion

        #region Métodos Virtuales - Validaciones opcionales
        protected virtual Task<ApiResponse<TResponse>> ValidateCreateAsync(TEntityDto dto)
        {
            return Task.FromResult<ApiResponse<TResponse>>(null);
        }

        protected virtual Task<ApiResponse<TResponse>> ValidateUpdateAsync(int id, TEntityDto dto)
        {
            return Task.FromResult<ApiResponse<TResponse>>(null);
        }

        protected virtual async Task<TEntity> AfterCreateAsync(TEntity entity)
        {
            return entity;
        }

        protected virtual async Task<TEntity> AfterUpdateAsync(TEntity entity)
        {
            return entity;
        }
        #endregion

        #region CRUD Genérico
        public virtual async Task<ApiResponse<TResponse>> CreateAsync(TEntityDto dto)
        {
            try
            {
                // Validación personalizada
                var validationResult = await ValidateCreateAsync(dto);
                if (validationResult != null)
                    return validationResult;

                // Mapeo y creación
                var entity = MapDtoToEntity(dto);
                var createdEntity = await _repository.AddAsync(entity);

                // Hook post-creación
                createdEntity = await AfterCreateAsync(createdEntity);

                // Respuesta
                var response = MapEntityToResponse(createdEntity);
                return ApiResponse<TResponse>.SuccessResponse(
                    response,
                    $"{EntityName} creado exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al crear {EntityName}");
                return ApiResponse<TResponse>.ErrorResponse(
                    $"Error al crear {EntityName}",
                    new List<string> { ex.Message }
                );
            }
        }

        public virtual async Task<ApiResponse<TResponse>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponse<TResponse>.ErrorResponse($"{EntityName} no encontrado");

                var response = MapEntityToResponse(entity);
                return ApiResponse<TResponse>.SuccessResponse(
                    response,
                    $"{EntityName} obtenido exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener {EntityName}");
                return ApiResponse<TResponse>.ErrorResponse(
                    $"Error al obtener {EntityName}",
                    new List<string> { ex.Message }
                );
            }
        }

        public virtual async Task<ApiResponse<List<TResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _repository.GetAllAsync();
                _logger.LogInformation($"{EntityName}s obtenidos: {entities.Count}");

                var responses = entities.Select(MapEntityToResponse).ToList();

                return ApiResponse<List<TResponse>>.SuccessResponse(
                    responses,
                    $"{EntityName}s obtenidos exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener {EntityName}s");
                return ApiResponse<List<TResponse>>.ErrorResponse(
                    $"Error al obtener {EntityName}s",
                    new List<string> { ex.Message }
                );
            }
        }

        public virtual async Task<ApiResponse<TResponse>> UpdateAsync(int id, TEntityDto dto)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponse<TResponse>.ErrorResponse($"{EntityName} no encontrado");

                // Validación personalizada
                var validationResult = await ValidateUpdateAsync(id, dto);
                if (validationResult != null)
                    return validationResult;

                // Actualización
                UpdateEntityFromDto(entity, dto);
                var updatedEntity = await _repository.UpdateAsync(entity);

                // Hook post-actualización
                updatedEntity = await AfterUpdateAsync(updatedEntity);

                // Respuesta
                var response = MapEntityToResponse(updatedEntity);
                return ApiResponse<TResponse>.SuccessResponse(
                    response,
                    $"{EntityName} actualizado exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar {EntityName}");
                return ApiResponse<TResponse>.ErrorResponse(
                    $"Error al actualizar {EntityName}",
                    new List<string> { ex.Message }
                );
            }
        }

        public virtual async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);

                if (entity == null)
                    return ApiResponse<bool>.ErrorResponse($"{EntityName} no encontrado");

                var deleted = await _repository.DeleteAsync(id);

                return ApiResponse<bool>.SuccessResponse(
                    deleted,
                    $"{EntityName} eliminado exitosamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar {EntityName}");
                return ApiResponse<bool>.ErrorResponse(
                    $"Error al eliminar {EntityName}",
                    new List<string> { ex.Message }
                );
            }
        }
        #endregion
    }
}

