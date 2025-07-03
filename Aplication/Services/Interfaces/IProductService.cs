using Application.DTOs;

namespace Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task AddAsync(CreateProductDto dto);
        Task UpdateAsync(int id, CreateProductDto dto);
        Task DeleteAsync(int id);
    }
}
