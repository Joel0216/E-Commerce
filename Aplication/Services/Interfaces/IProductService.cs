using Application.DTOs;

namespace Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedResponseDto<ProductDto>> GetAllAsync(int page = 1, int pageSize = 25);
        Task<ProductDto?> GetByIdAsync(int id);
        Task AddAsync(CreateProductDto dto);
        Task UpdateAsync(int id, CreateProductDto dto);
        Task DeleteAsync(int id);
        Task<bool> HasEnoughStockAsync(int productId, int quantity);
        Task UpdateStockAsync(int productId, int quantity);
    }
}
