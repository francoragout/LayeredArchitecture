using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dtos.Order;
using Application.Interfaces;
using Application.Services.Order;
using Domain.Entities;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            var orders = new List<Order>
            {
                new Order { OrderID = 1, CustomerID = "C1", EmployeeID = 2, OrderDate = new DateTime(2020,1,1), ShipCity = "CityA" },
                new Order { OrderID = 2, CustomerID = "C2", EmployeeID = 3, OrderDate = new DateTime(2020,2,1), ShipCity = "CityB" }
            };

            var mock = new Mock<IOrderRepository>();
            mock.Setup(r => r.GetAllAsync()).ReturnsAsync(orders);

            var service = new OrderService(mock.Object);

            var result = (await service.GetAllAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].OrderID);
            Assert.Equal("C1", result[0].CustomerID);
            Assert.Equal(2, result[0].EmployeeID);
            Assert.Equal(new DateTime(2020,1,1), result[0].OrderDate);
            Assert.Equal("CityA", result[0].ShipCity);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            var mock = new Mock<IOrderRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Order?)null);

            var service = new OrderService(mock.Object);

            var result = await service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Found_ReturnsDto()
        {
            var order = new Order { OrderID = 5, CustomerID = "C5", EmployeeID = 7, ShipCountry = "CountryX", Freight = 12.34m };
            var mock = new Mock<IOrderRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(order);

            var service = new OrderService(mock.Object);

            var result = await service.GetByIdAsync(5);

            Assert.NotNull(result);
            Assert.Equal(5, result!.OrderID);
            Assert.Equal("C5", result.CustomerID);
            Assert.Equal(7, result.EmployeeID);
            Assert.Equal("CountryX", result.ShipCountry);
            Assert.Equal(12.34m, result.Freight);
        }

        [Fact]
        public async Task CreateAsync_MapsDtoAndCallsRepository_ReturnsId()
        {
            var dto = new CreateOrderDto
            {
                CustomerID = "C1",
                EmployeeID = 4,
                OrderDate = new DateTime(2021,1,2),
                ShipCity = "CityX",
                Freight = 5.5m,
                OrderDetails = new List<CreateOrderDetailDto>
                {
                    new CreateOrderDetailDto { ProductID = 10, UnitPrice = 1.5m, Quantity = 2, Discount = 0f }
                }
            };

            var mock = new Mock<IOrderRepository>();
            mock.Setup(r => r.CreateAsync(It.IsAny<Order>())).ReturnsAsync(123);

            Order? captured = null;
            mock.Setup(r => r.CreateAsync(It.IsAny<Order>())).Callback<Order>(o => captured = o).ReturnsAsync(123);

            var service = new OrderService(mock.Object);

            var id = await service.CreateAsync(dto);

            Assert.Equal(123, id);
            Assert.NotNull(captured);
            Assert.Equal("C1", captured!.CustomerID);
            Assert.Equal(4, captured.EmployeeID);
            Assert.Equal(new DateTime(2021,1,2), captured.OrderDate);
            Assert.Equal("CityX", captured.ShipCity);
            Assert.Equal(5.5m, captured.Freight);
            Assert.Single(captured.OrderDetails);
            Assert.Equal(10, captured.OrderDetails[0].ProductID);
            mock.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NotFound_ReturnsFalse()
        {
            var mock = new Mock<IOrderRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Order?)null);

            var service = new OrderService(mock.Object);

            var dto = new UpdateOrderDto { ShipCity = "X" };

            var result = await service.UpdateAsync(1, dto);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_Success_UpdatesAndCallsRepository()
        {
            var original = new Order { OrderID = 10, CustomerID = "Old", ShipCity = "OldCity", Freight = 1.1m };
            Order? captured = null;

            var mock = new Mock<IOrderRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(original);
            mock.Setup(r => r.UpdateAsync(It.IsAny<Order>())).Callback<Order>(o => captured = o).ReturnsAsync(true);

            var service = new OrderService(mock.Object);

            var dto = new UpdateOrderDto { CustomerID = "NewC", ShipCity = "NewCity", Freight = 9.9m };

            var result = await service.UpdateAsync(10, dto);

            Assert.True(result);
            Assert.NotNull(captured);
            Assert.Equal(10, captured!.OrderID);
            Assert.Equal("NewC", captured.CustomerID);
            Assert.Equal("NewCity", captured.ShipCity);
            Assert.Equal(9.9m, captured.Freight);
            mock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeletesAndReturnsResult()
        {
            var mock = new Mock<IOrderRepository>();
            mock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);

            var service = new OrderService(mock.Object);

            var result = await service.DeleteAsync(7);

            Assert.True(result);
            mock.Verify(r => r.DeleteAsync(7), Times.Once);
        }
    }
}
