using Application.Interfaces;
using Dapper;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly IDapperContext _context;

        public SupplierRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            var query = @"
                SELECT SupplierID, CompanyName, ContactName, ContactTitle, Address, City, Region, PostalCode, Country, Phone, Fax, HomePage 
                FROM Suppliers
            ";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Supplier>(query);
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            var query = @"
                SELECT SupplierID, CompanyName, ContactName, ContactTitle, Address, City, Region, PostalCode, Country, Phone, Fax, HomePage 
                FROM Suppliers 
                WHERE SupplierID = @Id
            ";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Supplier>(query, new { Id = id });
        }

        public async Task<int> CreateAsync(Supplier supplier)
        {
            var query = @"
                INSERT INTO Suppliers (CompanyName, ContactName, ContactTitle, Address, City, Region, PostalCode, Country, Phone, Fax, HomePage)
                OUTPUT INSERTED.SupplierID
                VALUES (@CompanyName, @ContactName, @ContactTitle, @Address, @City, @Region, @PostalCode, @Country, @Phone, @Fax, @HomePage)
            ";

            using var connection = _context.CreateConnection();
            var id = await connection.QuerySingleAsync<int>(query, supplier);
            return id;
        }

        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            var query = @"
                UPDATE Suppliers
                SET CompanyName = @CompanyName,
                    ContactName = @ContactName,
                    ContactTitle = @ContactTitle,
                    Address = @Address,
                    City = @City,
                    Region = @Region,
                    PostalCode = @PostalCode,
                    Country = @Country,
                    Phone = @Phone,
                    Fax = @Fax,
                    HomePage = @HomePage
                WHERE SupplierID = @SupplierID
            ";

            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, supplier);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = @"
                DELETE FROM Suppliers 
                WHERE SupplierID = @Id
            ";

            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, new { Id = id });
            return rows > 0;
        }
    }
}
