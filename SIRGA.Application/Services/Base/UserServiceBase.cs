using Microsoft.AspNetCore.Identity;
using SIRGA.Application.DTOs.Common;
using SIRGA.Domain.Interfaces;
using SIRGA.Identity.Shared.Entities;

namespace SIRGA.Application.Services.Base
{
    public abstract class UserServiceBase<TEntity, TRepository>
        where TEntity : class
        where TRepository : IGenericRepository<TEntity>
    {
        protected readonly TRepository _repository;
        protected readonly UserManager<ApplicationUser> _userManager;

        protected UserServiceBase(
            TRepository repository,
            UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public virtual async Task<ApiResponse<bool>> ActivateAsync(int id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Entidad no encontrada");
                }

                // Obtener el ApplicationUserId de la entidad
                var userId = GetApplicationUserId(entity);
                if (string.IsNullOrEmpty(userId))
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario asociado no encontrado");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario no encontrado");
                }

                user.IsActive = true;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Error al activar el usuario",
                        result.Errors.Select(e => e.Description).ToList()
                    );
                }

                return ApiResponse<bool>.SuccessResponse(true, "Usuario activado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al activar el usuario",
                    new List<string> { ex.Message }
                );
            }
        }

        public virtual async Task<ApiResponse<bool>> DeactivateAsync(int id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Entidad no encontrada");
                }

                var userId = GetApplicationUserId(entity);
                if (string.IsNullOrEmpty(userId))
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario asociado no encontrado");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario no encontrado");
                }

                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Error al desactivar el usuario",
                        result.Errors.Select(e => e.Description).ToList()
                    );
                }

                return ApiResponse<bool>.SuccessResponse(true, "Usuario desactivado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Error al desactivar el usuario",
                    new List<string> { ex.Message }
                );
            }
        }

        protected abstract string GetApplicationUserId(TEntity entity);
    }
}
