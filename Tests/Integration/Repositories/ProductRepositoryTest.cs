using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Xunit;
using Infrastructure.Database;
using Tests.Integration.Database;

namespace Tests.Integration.Repositories
{
    public class ProductRepositoryTest
    {
        [Fact]
        public async Task Create_GetAll_GetById_Update_Delete_flow()
        {
            using var ctx = new DapperContextTest("producttest");

            using var conn = ctx.CreateConnection();
            await conn.OpenSafeAsync();

            var createProducts = @"
                CREATE TABLE Products (
                    ProductID INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductName TEXT NOT NULL,
                    SupplierID INTEGER,
                    CategoryID INTEGER,
                    QuantityPerUnit TEXT,
                    UnitPrice REAL,
                    UnitsInStock INTEGER,
                    UnitsOnOrder INTEGER,
                    ReorderLevel INTEGER,
                    Discontinued INTEGER
                );
            ";

            await conn.ExecuteAsync(createProducts);

            var repo = new Infrastructure.Repositories.ProductRepository(ctx);

            // Insert product directly (SQLite doesn't support OUTPUT)
            var insertProduct = @"
                INSERT INTO Products (ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued)
                VALUES (@ProductName, @SupplierID, @CategoryID, @QuantityPerUnit, @UnitPrice, @UnitsInStock, @UnitsOnOrder, @ReorderLevel, @Discontinued);
                SELECT last_insert_rowid();
            ";

            var idLong = await conn.ExecuteScalarAsync<long>(insertProduct, new
            {
                ProductName = "P1",
                SupplierID = 1,
                CategoryID = 2,
                QuantityPerUnit = "1 box",
                UnitPrice = 9.99m,
                UnitsInStock = 10,
                UnitsOnOrder = 0,
                ReorderLevel = 5,
                Discontinued = 0
            });

            var productId = (int)idLong;
            productId.Should().BeGreaterThan(0);

            // GetAll
            var all = (await repo.GetAllAsync()).ToList();
            all.Should().ContainSingle().Which.ProductName.Should().Be("P1");

            // GetById
            var item = await repo.GetByIdAsync(productId);
            item.Should().NotBeNull();
            item!.ProductName.Should().Be("P1");

            // Update
            item.UnitsInStock = 20;
            var updated = await repo.UpdateAsync(item);
            updated.Should().BeTrue();

            var item2 = await repo.GetByIdAsync(productId);
            item2!.UnitsInStock.Should().Be((short)20);

            // Delete
            var deleted = await repo.DeleteAsync(productId);
            deleted.Should().BeTrue();

            var after = await repo.GetByIdAsync(productId);
            after.Should().BeNull();
        }
    }
}
