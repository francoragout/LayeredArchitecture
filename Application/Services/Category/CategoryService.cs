using Application.Dtos.Category;
using Application.Interfaces;

namespace Application.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ReadCategoryDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();

            return categories.Select(c => new ReadCategoryDto
            {
                CategoryID = c.CategoryID,
                CategoryName = c.CategoryName,
                Description = c.Description,
                Picture = c.Picture
            });
        }

        public async Task<ReadCategoryDto?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return null;

            return new ReadCategoryDto
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                Description = category.Description,
                Picture = category.Picture
            };
        }

        public async Task<int> CreateAsync(CreateCategoryDto dto)
        {
            // Evitar duplicados: si existe otra categoría con ese nombre => error
            var existingCategory = await _repository.GetByNameAsync(dto.CategoryName);
            if (existingCategory != null)
            {
                throw new InvalidOperationException("A category with the same name already exists.");
            }

            var category = new Domain.Entities.Category
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description,
                Picture = dto.Picture
            };

            return await _repository.CreateAsync(category);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return false;

            // Evitar duplicados: si existe otra categoría con ese nombre y distinto id => error
            var existingCategory = await _repository.GetByNameAsync(dto.CategoryName);
            if (existingCategory != null && existingCategory.CategoryID != id)
            {
                throw new InvalidOperationException("A category with the same name already exists.");
            }

            category.CategoryName = dto.CategoryName;
            category.Description = dto.Description;
            category.Picture = dto.Picture;

            return await _repository.UpdateAsync(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
