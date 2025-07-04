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

        public async Task<PaginatedResponseDto<ProductDto>> GetAllAsync(int page = 1, int pageSize = 25)
        {
            // Validar parámetros de paginación
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 25;
            if (pageSize > 25) pageSize = 25; // Máximo 25 elementos

            var (products, totalCount) = await _productRepository.GetAllAsync(page, pageSize);
            
            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            var pagination = new PaginationDto
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = totalPages,
                HasNextPage = page < totalPages,
                HasPreviousPage = page > 1
            };

            return new PaginatedResponseDto<ProductDto>
            {
                Items = productDtos,
                Pagination = pagination
            };
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock
            };
        }

        public async Task AddAsync(CreateProductDto dto)
        {
            var exists = await _productRepository.GetByNameAsync(dto.Name);
            if (exists != null)
                throw new BusinessException("Ya existe un producto con ese nombre.");

            if (dto.Stock < 0)
                throw new BusinessException("El stock no puede ser menor a 0.");

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock
            };

            await _productRepository.AddAsync(product);
        }

        public async Task UpdateAsync(int id, CreateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) throw new BusinessException("Producto no encontrado.");

            if (dto.Stock < 0)
                throw new BusinessException("El stock no puede ser menor a 0.");

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Stock = dto.Stock;

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

        public async Task<bool> HasEnoughStockAsync(int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return false;
            
            return product.Stock >= quantity;
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) throw new BusinessException("Producto no encontrado.");
            
            if (product.Stock < quantity)
                throw new BusinessException($"Stock insuficiente. Disponible: {product.Stock}, Solicitado: {quantity}");
            
            product.Stock -= quantity;
            await _productRepository.UpdateAsync(product);
        }
    }
}
