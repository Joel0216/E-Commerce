using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<(List<Product> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByNameAsync(string name);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<bool> IsInAnyOrderAsync(int productId);

        // 🔍 Métodos opcionales de mejora
        Task<bool> ExistsAsync(int id);         // Verificar si existe un producto por ID
        Task<IEnumerable<Product>> SearchAsync(string keyword); // Buscar productos por nombre parcial
    }
}
