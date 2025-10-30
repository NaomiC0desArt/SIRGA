using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Identity;
using SIRGA.Identity.Interfaces;
using SIRGA.Identity.Shared.Entities;
using SIRGA.Persistence.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Identity.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public async Task<ApiResponse<UserProfileDto>> GetUserProfileAsync(string userId)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new UserProfileDto
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Gender = u.Gender,
                        DateOfBirth = u.DateOfBirth,
                        Province = u.Province,
                        Sector = u.Sector,
                        Address = u.Address,
                        Photo = u.Photo,
                        DateOfEntry = u.DateOfEntry,
                        IsActive = u.IsActive,
                        MustCompleteProfile = u.MustCompleteProfile,
                        LastLogin = u.LastLogin
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return ApiResponse<UserProfileDto>.ErrorResponse("Usuario no encontrado");
                }

                return ApiResponse<UserProfileDto>.SuccessResponse(user);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserProfileDto>.ErrorResponse(
                    "Error al obtener el perfil",
                    new List<string> { ex.Message }
                );
            }
        }

        
        public async Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(
            string userId,
            UpdateProfileDto dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return ApiResponse<UserProfileDto>.ErrorResponse("Usuario no encontrado");
                }

                // Verificar contraseña actual para seguridad
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.CurrentPassword);
                if (!isPasswordValid)
                {
                    return ApiResponse<UserProfileDto>.ErrorResponse(
                        "La contraseña actual es incorrecta"
                    );
                }

                // Actualizar datos
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.PhoneNumber = dto.PhoneNumber;
                user.Province = dto.Province;
                user.Sector = dto.Sector;
                user.Address = dto.Address;

                // Si se proporciona nueva foto
                if (!string.IsNullOrEmpty(dto.Photo))
                {
                    user.Photo = dto.Photo;
                }

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<UserProfileDto>.ErrorResponse(
                        "Error al actualizar el perfil",
                        errors
                    );
                }

                // Retornar perfil actualizado
                var updatedProfile = new UserProfileDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    Province = user.Province,
                    Sector = user.Sector,
                    Address = user.Address,
                    Photo = user.Photo,
                    DateOfEntry = user.DateOfEntry,
                    IsActive = user.IsActive,
                    MustCompleteProfile = user.MustCompleteProfile,
                    LastLogin = user.LastLogin
                };

                return ApiResponse<UserProfileDto>.SuccessResponse(
                    updatedProfile,
                    "Perfil actualizado exitosamente"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<UserProfileDto>.ErrorResponse(
                    "Error al actualizar el perfil",
                    new List<string> { ex.Message }
                );
            }
        }

      
        public async Task<ApiResponse<string>> UpdateProfilePhotoAsync(
            string userId,
            string photoBase64)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return ApiResponse<string>.ErrorResponse("Usuario no encontrado");
                }

                user.Photo = photoBase64;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<string>.ErrorResponse(
                        "Error al actualizar la foto",
                        errors
                    );
                }

                return ApiResponse<string>.SuccessResponse(
                    photoBase64,
                    "Foto actualizada exitosamente"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse(
                    "Error al actualizar la foto",
                    new List<string> { ex.Message }
                );
            }
        }

       
        public async Task<ApiResponse<List<BasicUserInfoDto>>> GetUsersBasicInfoAsync(List<string> userIds)
        {
            try
            {
                var users = await _context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => new BasicUserInfoDto
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Photo = u.Photo
                    })
                    .AsNoTracking()
                    .ToListAsync();

                return ApiResponse<List<BasicUserInfoDto>>.SuccessResponse(users);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<BasicUserInfoDto>>.ErrorResponse(
                    "Error al obtener información de usuarios",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
