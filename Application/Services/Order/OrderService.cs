using Application.Dtos.Order;
using Application.Interfaces;

namespace Application.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ReadOrderDto>> GetAllAsync()
        {
            var orders = await _repository.GetAllAsync();
            return orders.Select(o => new ReadOrderDto
            {
                OrderID = o.OrderID,
                CustomerID = o.CustomerID,
                EmployeeID = o.EmployeeID,
                OrderDate = o.OrderDate,
                RequiredDate = o.RequiredDate,
                ShippedDate = o.ShippedDate,
                ShipVia = o.ShipVia,
                Freight = o.Freight,
                ShipName = o.ShipName,
                ShipAddress = o.ShipAddress,
                ShipCity = o.ShipCity,
                ShipRegion = o.ShipRegion,
                ShipPostalCode = o.ShipPostalCode,
                ShipCountry = o.ShipCountry
            });
        }

        public async Task<ReadOrderDto?> GetByIdAsync(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null) return null;
            return new ReadOrderDto
            {
                OrderID = order.OrderID,
                CustomerID = order.CustomerID,
                EmployeeID = order.EmployeeID,
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
                ShipVia = order.ShipVia,
                Freight = order.Freight,
                ShipName = order.ShipName,
                ShipAddress = order.ShipAddress,
                ShipCity = order.ShipCity,
                ShipRegion = order.ShipRegion,
                ShipPostalCode = order.ShipPostalCode,
                ShipCountry = order.ShipCountry
            };
        }

        public async Task<int> CreateAsync(CreateOrderDto dto)
        {
            var order = new Domain.Entities.Order
            {
                CustomerID = dto.CustomerID,
                EmployeeID = dto.EmployeeID,
                OrderDate = dto.OrderDate,
                RequiredDate = dto.RequiredDate,
                ShippedDate = dto.ShippedDate,
                ShipVia = dto.ShipVia,
                Freight = dto.Freight,
                ShipName = dto.ShipName,
                ShipAddress = dto.ShipAddress,
                ShipCity = dto.ShipCity,
                ShipRegion = dto.ShipRegion,
                ShipPostalCode = dto.ShipPostalCode,
                ShipCountry = dto.ShipCountry,
             
                OrderDetails = dto.OrderDetails.Select(d => new Domain.Entities.OrderDetail
                {
                    ProductID = d.ProductID,
                    UnitPrice = d.UnitPrice,
                    Quantity = d.Quantity,
                    Discount = d.Discount
                }).ToList()
            };

            return await _repository.CreateAsync(order);
        }


        public async Task<bool> UpdateAsync(int id, UpdateOrderDto dto)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null) return false;

            order.CustomerID = dto.CustomerID;
            order.EmployeeID = dto.EmployeeID;
            order.OrderDate = dto.OrderDate;
            order.RequiredDate = dto.RequiredDate;
            order.ShippedDate = dto.ShippedDate;
            order.ShipVia = dto.ShipVia;
            order.Freight = dto.Freight;
            order.ShipName = dto.ShipName;
            order.ShipAddress = dto.ShipAddress;
            order.ShipCity = dto.ShipCity;
            order.ShipRegion = dto.ShipRegion;
            order.ShipPostalCode = dto.ShipPostalCode;
            order.ShipCountry = dto.ShipCountry;
            return await _repository.UpdateAsync(order);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}