using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Xunit;
using Infrastructure.Database;
using Tests.Integration.Database;

namespace Tests.Integration.Repositories
{
    public class OrderRepositoryTest
    {
        [Fact]
        public async Task Create_GetAll_GetById_Update_Delete_flow()
        {
            using var ctx = new DapperContextTest("ordertest");

            using var conn = ctx.CreateConnection();
            await conn.OpenSafeAsync();

            var createOrders = @"
                CREATE TABLE Orders (
                    OrderID INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerID TEXT,
                    EmployeeID INTEGER,
                    OrderDate TEXT,
                    RequiredDate TEXT,
                    ShippedDate TEXT,
                    ShipVia INTEGER,
                    Freight REAL,
                    ShipName TEXT,
                    ShipAddress TEXT,
                    ShipCity TEXT,
                    ShipRegion TEXT,
                    ShipPostalCode TEXT,
                    ShipCountry TEXT
                );
            ";

            var createOrderDetails = @"
                CREATE TABLE [Order Details] (
                    OrderID INTEGER,
                    ProductID INTEGER,
                    UnitPrice REAL,
                    Quantity INTEGER,
                    Discount REAL
                );
            ";

            await conn.ExecuteAsync(createOrders);
            await conn.ExecuteAsync(createOrderDetails);

            var repo = new Infrastructure.Repositories.OrderRepository(ctx);

            // Insert order directly (SQLite doesn't support OUTPUT)
            var insertOrder = @"
                INSERT INTO Orders (CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry)
                VALUES (@CustomerID, @EmployeeID, @OrderDate, @RequiredDate, @ShippedDate, @ShipVia, @Freight, @ShipName, @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry);
                SELECT last_insert_rowid();
            ";

            var idLong = await conn.ExecuteScalarAsync<long>(insertOrder, new
            {
                CustomerID = "C1",
                EmployeeID = 2,
                OrderDate = "2020-01-01",
                RequiredDate = (string?)null,
                ShippedDate = (string?)null,
                ShipVia = (int?)null,
                Freight = 10.5m,
                ShipName = "S1",
                ShipAddress = "Addr",
                ShipCity = "CityX",
                ShipRegion = (string?)null,
                ShipPostalCode = (string?)null,
                ShipCountry = "CountryA"
            });

            var orderId = (int)idLong;
            orderId.Should().BeGreaterThan(0);

            // Insert a detail
            var insertDetail = @"
                INSERT INTO [Order Details] (OrderID, ProductID, UnitPrice, Quantity, Discount)
                VALUES (@OrderID, @ProductID, @UnitPrice, @Quantity, @Discount);
            ";

            await conn.ExecuteAsync(insertDetail, new { OrderID = orderId, ProductID = 1, UnitPrice = 5.5m, Quantity = 2, Discount = 0m });

            // GetAll
            var all = (await repo.GetAllAsync()).ToList();
            all.Should().ContainSingle().Which.OrderID.Should().Be(orderId);

            // GetById
            var item = await repo.GetByIdAsync(orderId);
            item.Should().NotBeNull();
            item!.CustomerID.Should().Be("C1");

            // Update
            item.ShipCity = "CityY";
            var updated = await repo.UpdateAsync(item);
            updated.Should().BeTrue();

            var item2 = await repo.GetByIdAsync(orderId);
            item2!.ShipCity.Should().Be("CityY");

            // Delete
            var deleted = await repo.DeleteAsync(orderId);
            deleted.Should().BeTrue();

            var after = await repo.GetByIdAsync(orderId);
            after.Should().BeNull();
        }
    }
}
