using SIRGA.Application.DTOs.Common;

namespace SIRGA.Application.Interfaces.Base
{
    public interface IBaseServices<TEntityDto, TResponse> 
    {
        Task<ApiResponse<TResponse>> CreateAsync(TEntityDto dto);
        Task<ApiResponse<TResponse>> GetByIdAsync(int id);
        Task<ApiResponse<List<TResponse>>> GetAllAsync();
        Task<ApiResponse<TResponse>> UpdateAsync(int id, TEntityDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
