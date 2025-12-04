using Application.Interfaces;
using Dapper;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDapperContext _context;

        public ProductRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var query = @"
                SELECT ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued 
                FROM Products
            ";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Product>(query);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var query = @"
                SELECT ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued FROM Products 
                WHERE ProductID = @Id
            ";

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Product>(query, new { Id = id });
        }

        public async Task<int> CreateAsync(Product product)
        {
            var query = @"
                INSERT INTO Products (ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued)
                OUTPUT INSERTED.ProductID
                VALUES (@ProductName, @SupplierID, @CategoryID, @QuantityPerUnit, @UnitPrice, @UnitsInStock, @UnitsOnOrder, @ReorderLevel, @Discontinued)
            ";

            using var connection = _context.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int>(query, product);
            return id;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var query = @"
                UPDATE Products
                SET ProductName = @ProductName,
                    SupplierID = @SupplierID,
                    CategoryID = @CategoryID,
                    QuantityPerUnit = @QuantityPerUnit,
                    UnitPrice = @UnitPrice,
                    UnitsInStock = @UnitsInStock,
                    UnitsOnOrder = @UnitsOnOrder,
                    ReorderLevel = @ReorderLevel,
                    Discontinued = @Discontinued
                WHERE ProductID = @ProductID
            ";

            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, product);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = @"
                DELETE FROM Products 
                WHERE ProductID = @Id
            ";

            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, new { Id = id });
            return rows > 0;
        }
    }
}
