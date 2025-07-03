using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Application.Exceptions;

namespace Application.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            }).ToList();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };
        }

        public async Task AddAsync(CreateProductDto dto)
        {
            var exists = await _productRepository.GetByNameAsync(dto.Name);
            if (exists != null)
                throw new BusinessException("Ya existe un producto con ese nombre.");

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price
            };

            await _productRepository.AddAsync(product);
        }

        public async Task UpdateAsync(int id, CreateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) throw new BusinessException("Producto no encontrado.");

            product.Name = dto.Name;
            product.Price = dto.Price;

            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) throw new BusinessException("Producto no encontrado.");

            var isInOrder = await _productRepository.IsInAnyOrderAsync(id);
            if (isInOrder) throw new BusinessException("No se puede eliminar un producto que ya fue usado en una orden.");

            await _productRepository.DeleteAsync(product);
        }
    }
}
