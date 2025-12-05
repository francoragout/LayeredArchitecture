using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dtos.Supplier;
using Application.Interfaces;
using Application.Services.Supplier;
using Domain.Entities;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
    public class SupplierServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            var suppliers = new List<Supplier>
            {
                new Supplier { SupplierID = 1, CompanyName = "Comp1", ContactName = "C1", City = "CityA" },
                new Supplier { SupplierID = 2, CompanyName = "Comp2", ContactName = "C2", City = "CityB" }
            };

            var mock = new Mock<ISupplierRepository>();
            mock.Setup(r => r.GetAllAsync()).ReturnsAsync(suppliers);

            var service = new SupplierService(mock.Object);

            var result = (await service.GetAllAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].SupplierID);
            Assert.Equal("Comp1", result[0].CompanyName);
            Assert.Equal("C1", result[0].ContactName);
            Assert.Equal("CityA", result[0].City);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            var mock = new Mock<ISupplierRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Supplier?)null);

            var service = new SupplierService(mock.Object);

            var result = await service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Found_ReturnsDto()
        {
            var supplier = new Supplier { SupplierID = 5, CompanyName = "Comp5", ContactName = "CN", Phone = "123" };
            var mock = new Mock<ISupplierRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(supplier);

            var service = new SupplierService(mock.Object);

            var result = await service.GetByIdAsync(5);

            Assert.NotNull(result);
            Assert.Equal(5, result!.SupplierID);
            Assert.Equal("Comp5", result.CompanyName);
            Assert.Equal("CN", result.ContactName);
            Assert.Equal("123", result.Phone);
        }

        [Fact]
        public async Task CreateAsync_MapsDtoAndCallsRepository_ReturnsId()
        {
            var dto = new CreateSupplierDto
            {
                CompanyName = "NewComp",
                ContactName = "ABC",
                ContactTitle = "CEO",
                Address = "Addr",
                City = "CityX",
                Region = "R",
                PostalCode = "11111",
                Country = "Country",
                Phone = "555",
                Fax = "666",
                HomePage = "hp"
            };

            var mock = new Mock<ISupplierRepository>();
            mock.Setup(r => r.CreateAsync(It.IsAny<Supplier>())).ReturnsAsync(777);

            Supplier? captured = null;
            mock.Setup(r => r.CreateAsync(It.IsAny<Supplier>())).Callback<Supplier>(s => captured = s).ReturnsAsync(777);

            var service = new SupplierService(mock.Object);

            var id = await service.CreateAsync(dto);

            Assert.Equal(777, id);
            Assert.NotNull(captured);
            Assert.Equal("NewComp", captured!.CompanyName);
            Assert.Equal("ABC", captured.ContactName);
            Assert.Equal("CEO", captured.ContactTitle);
            Assert.Equal("Addr", captured.Address);
            Assert.Equal("CityX", captured.City);
            Assert.Equal("R", captured.Region);
            Assert.Equal("11111", captured.PostalCode);
            Assert.Equal("Country", captured.Country);
            Assert.Equal("555", captured.Phone);
            Assert.Equal("666", captured.Fax);
            Assert.Equal("hp", captured.HomePage);
            mock.Verify(r => r.CreateAsync(It.IsAny<Supplier>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NotFound_ReturnsFalse()
        {
            var mock = new Mock<ISupplierRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Supplier?)null);

            var service = new SupplierService(mock.Object);

            var dto = new UpdateSupplierDto { CompanyName = "X" };

            var result = await service.UpdateAsync(1, dto);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_Success_UpdatesAndCallsRepository()
        {
            var original = new Supplier { SupplierID = 10, CompanyName = "OldCo", ContactName = "Old", City = "OldCity" };
            Supplier? captured = null;

            var mock = new Mock<ISupplierRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(original);
            mock.Setup(r => r.UpdateAsync(It.IsAny<Supplier>())).Callback<Supplier>(s => captured = s).ReturnsAsync(true);

            var service = new SupplierService(mock.Object);

            var dto = new UpdateSupplierDto { CompanyName = "NewCo", ContactName = "New", City = "NewCity", Phone = "999" };

            var result = await service.UpdateAsync(10, dto);

            Assert.True(result);
            Assert.NotNull(captured);
            Assert.Equal(10, captured!.SupplierID);
            Assert.Equal("NewCo", captured.CompanyName);
            Assert.Equal("New", captured.ContactName);
            Assert.Equal("NewCity", captured.City);
            Assert.Equal("999", captured.Phone);
            mock.Verify(r => r.UpdateAsync(It.IsAny<Supplier>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeletesAndReturnsResult()
        {
            var mock = new Mock<ISupplierRepository>();
            mock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);

            var service = new SupplierService(mock.Object);

            var result = await service.DeleteAsync(7);

            Assert.True(result);
            mock.Verify(r => r.DeleteAsync(7), Times.Once);
        }
    }
}
