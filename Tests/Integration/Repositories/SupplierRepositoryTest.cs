using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Xunit;
using Infrastructure.Database;
using Tests.Integration.Database;

namespace Tests.Integration.Repositories
{
    public class SupplierRepositoryTest
    {
        [Fact]
        public async Task Create_GetAll_GetById_Update_Delete_flow()
        {
            using var ctx = new DapperContextTest("suppliertest");

            using var conn = ctx.CreateConnection();
            await conn.OpenSafeAsync();

            var createSuppliers = @"
                CREATE TABLE Suppliers (
                    SupplierID INTEGER PRIMARY KEY AUTOINCREMENT,
                    CompanyName TEXT,
                    ContactName TEXT,
                    ContactTitle TEXT,
                    Address TEXT,
                    City TEXT,
                    Region TEXT,
                    PostalCode TEXT,
                    Country TEXT,
                    Phone TEXT,
                    Fax TEXT,
                    HomePage TEXT
                );
            ";

            await conn.ExecuteAsync(createSuppliers);

            var repo = new Infrastructure.Repositories.SupplierRepository(ctx);

            // Insert supplier directly (SQLite doesn't support OUTPUT)
            var insertSupplier = @"
                INSERT INTO Suppliers (CompanyName, ContactName, ContactTitle, Address, City, Region, PostalCode, Country, Phone, Fax, HomePage)
                VALUES (@CompanyName, @ContactName, @ContactTitle, @Address, @City, @Region, @PostalCode, @Country, @Phone, @Fax, @HomePage);
                SELECT last_insert_rowid();
            ";

            var idLong = await conn.ExecuteScalarAsync<long>(insertSupplier, new
            {
                CompanyName = "Comp1",
                ContactName = "Contact1",
                ContactTitle = "Owner",
                Address = "Addr1",
                City = "City1",
                Region = (string?)null,
                PostalCode = (string?)null,
                Country = "Country1",
                Phone = "123",
                Fax = (string?)null,
                HomePage = (string?)null
            });

            var supplierId = (int)idLong;
            supplierId.Should().BeGreaterThan(0);

            // GetAll
            var all = (await repo.GetAllAsync()).ToList();
            all.Should().ContainSingle().Which.CompanyName.Should().Be("Comp1");

            // GetById
            var item = await repo.GetByIdAsync(supplierId);
            item.Should().NotBeNull();
            item!.CompanyName.Should().Be("Comp1");

            // Update
            item.City = "City2";
            var updated = await repo.UpdateAsync(item);
            updated.Should().BeTrue();

            var item2 = await repo.GetByIdAsync(supplierId);
            item2!.City.Should().Be("City2");

            // Delete
            var deleted = await repo.DeleteAsync(supplierId);
            deleted.Should().BeTrue();

            var after = await repo.GetByIdAsync(supplierId);
            after.Should().BeNull();
        }
    }
}
