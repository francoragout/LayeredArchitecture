using Application.Dtos.Product;

namespace Application.Services.Product
{
    public interface IProductService
    {
        Task<IEnumerable<ReadProductDto>> GetAllAsync();
        Task<ReadProductDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateProductDto productDto);
        Task<bool> UpdateAsync(int id, UpdateProductDto productDto);
        Task<bool> DeleteAsync(int id);
    }
}