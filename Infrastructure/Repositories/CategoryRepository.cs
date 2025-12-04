using Application.Interfaces;
using Dapper;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDapperContext _context;

        public CategoryRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            var query = @"
                SELECT CategoryID, CategoryName, Description, Picture 
                FROM Categories
            ";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Category>(query);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            var query = @"
                SELECT CategoryID, CategoryName, Description, Picture 
                FROM Categories 
                WHERE CategoryID = @Id
            ";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Category>(query, new { Id = id });
        }

        public async Task<int> CreateAsync(Category category)
        {
            var query = @"
                INSERT INTO Categories (CategoryName, Description, Picture)
                OUTPUT INSERTED.CategoryID
                VALUES (@CategoryName, @Description, @Picture)
            ";

            using var connection = _context.CreateConnection();
            var id = await connection.QuerySingleAsync<int>(query, category);
            return id;
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            var query = @"
                UPDATE Categories
                SET CategoryName = @CategoryName,
                    Description = @Description,
                    Picture = @Picture
                WHERE CategoryID = @CategoryID
            ";

            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, category);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = @"
                DELETE FROM Categories 
                WHERE CategoryID = @Id
            ";

            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, new { Id = id });
            return rows > 0;
        }

        // Metodos adicionales
        public async Task<Category?> GetByNameAsync(string categoryName)
        {
            var query = @"
                SELECT CategoryID, CategoryName, Description, Picture
                FROM Categories
                WHERE LOWER(CategoryName) = LOWER(@CategoryName)
            ";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Category>(query, new { CategoryName = categoryName });
        }
    }
}
