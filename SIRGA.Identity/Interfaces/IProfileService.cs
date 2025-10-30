using SIRGA.Application.DTOs.Common;
using SIRGA.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Identity.Interfaces
{
    public interface IProfileService
    {
        Task<ApiResponse<UserProfileDto>> GetUserProfileAsync(string userId);
        Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task<ApiResponse<string>> UpdateProfilePhotoAsync(string userId, string photoBase64);
        Task<ApiResponse<List<BasicUserInfoDto>>> GetUsersBasicInfoAsync(List<string> userIds);
    }
}
