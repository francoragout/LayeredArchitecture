using Application.Dtos.Category;

namespace Application.Services.Category
{
    public interface ICategoryService
    {
        Task<IEnumerable<ReadCategoryDto>> GetAllAsync();
        Task<ReadCategoryDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateCategoryDto dto);
        Task<bool> UpdateAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
