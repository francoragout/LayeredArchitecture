using Application.Interfaces;
using Dapper;
using Domain.Entities;
using Infrastructure.Database;

namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDapperContext _context;

        public OrderRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var query = @"
                SELECT OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry 
                FROM Orders
            ";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Order>(query);
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            var query = @"
                SELECT OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry 
                FROM Orders 
                WHERE OrderID = @Id
            ";

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Order>(query, new { Id = id });
        }

        public async Task<int> CreateAsync(Order order)
        {
            using var connection = _context.CreateConnection();
            await connection.OpenSafeAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // 1️. Insertar orden
                var insertOrderQuery = @"
                    INSERT INTO Orders
                        (CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, 
                         ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry)
                    OUTPUT INSERTED.OrderID
                    VALUES
                        (@CustomerID, @EmployeeID, @OrderDate, @RequiredDate, @ShippedDate, @ShipVia, @Freight,
                         @ShipName, @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry);
                ";


                int orderId = await connection.ExecuteScalarAsync<int>(
                    insertOrderQuery,
                    order,
                    transaction
                );

                // 2️. Insertar detalles de la orden
                var insertDetailQuery = @"
                    INSERT INTO [Order Details] (OrderID, ProductID, UnitPrice, Quantity, Discount)
                    VALUES (@OrderID, @ProductID, @UnitPrice, @Quantity, @Discount);
                ";

                foreach (var detail in order.OrderDetails)
                {
                    detail.OrderID = orderId;

                    await connection.ExecuteAsync(
                        insertDetailQuery,
                        detail,
                        transaction
                    );
                }

                // 3️. Confirmar transacción
                transaction.Commit();

                return orderId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            var query = @"
                UPDATE Orders
                SET CustomerID = @CustomerID,
                    EmployeeID = @EmployeeID,
                    OrderDate = @OrderDate,
                    RequiredDate = @RequiredDate,
                    ShippedDate = @ShippedDate,
                    ShipVia = @ShipVia,
                    Freight = @Freight,
                    ShipName = @ShipName,
                    ShipAddress = @ShipAddress,
                    ShipCity = @ShipCity,
                    ShipRegion = @ShipRegion,
                    ShipPostalCode = @ShipPostalCode,
                    ShipCountry = @ShipCountry
                WHERE OrderID = @OrderID
            ";
            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, order);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int orderId)
        {
            using var connection = _context.CreateConnection();
            await connection.OpenSafeAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                // 1. Borrar los detalles de la orden
                var deleteDetailsQuery = @"
                    DELETE FROM [Order Details] 
                    WHERE OrderID = @OrderID
                ";

                await connection.ExecuteAsync(deleteDetailsQuery, new { OrderID = orderId }, transaction);

                // 2. Borrar la orden
                var deleteOrderQuery = @"
                    DELETE FROM Orders 
                    WHERE OrderID = @OrderID
                ";

                var rows = await connection.ExecuteAsync(deleteOrderQuery, new { OrderID = orderId }, transaction);

                // 3. Confirmar la transacción
                transaction.Commit();
                return rows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

    }
}
