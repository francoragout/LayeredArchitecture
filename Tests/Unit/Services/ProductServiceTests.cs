using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dtos.Product;
using Application.Interfaces;
using Application.Services.Product;
using Domain.Entities;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            var products = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "P1", SupplierID = 2, CategoryID = 3, UnitPrice = 10.5m, UnitsInStock = 5, Discontinued = false },
                new Product { ProductID = 2, ProductName = "P2", SupplierID = 4, CategoryID = 6, UnitPrice = 20m, UnitsInStock = 10, Discontinued = true }
            };

            var mock = new Mock<IProductRepository>();
            mock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

            var service = new ProductService(mock.Object);

            var result = (await service.GetAllAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].ProductID);
            Assert.Equal("P1", result[0].ProductName);
            Assert.Equal(2, result[0].SupplierID);
            Assert.Equal(3, result[0].CategoryID);
            Assert.Equal(10.5m, result[0].UnitPrice);
            Assert.Equal((short)5, result[0].UnitsInStock);
            Assert.False(result[0].Discontinued);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

            var service = new ProductService(mock.Object);

            var result = await service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Found_ReturnsDto()
        {
            var product = new Product { ProductID = 5, ProductName = "Prod5", SupplierID = 8, UnitPrice = 3.14m, Discontinued = true };
            var mock = new Mock<IProductRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);

            var service = new ProductService(mock.Object);

            var result = await service.GetByIdAsync(5);

            Assert.NotNull(result);
            Assert.Equal(5, result!.ProductID);
            Assert.Equal("Prod5", result.ProductName);
            Assert.Equal(8, result.SupplierID);
            Assert.Equal(3.14m, result.UnitPrice);
            Assert.True(result.Discontinued);
        }

        [Fact]
        public async Task CreateAsync_MapsDtoAndCallsRepository_ReturnsId()
        {
            var dto = new CreateProductDto
            {
                ProductName = "NewProd",
                SupplierID = 12,
                CategoryID = 34,
                QuantityPerUnit = "10 boxes",
                UnitPrice = 7.5m,
                UnitsInStock = 20,
                UnitsOnOrder = 0,
                ReorderLevel = 5,
                Discontinued = false
            };

            var mock = new Mock<IProductRepository>();
            mock.Setup(r => r.CreateAsync(It.IsAny<Product>())).ReturnsAsync(555);

            Product? captured = null;
            mock.Setup(r => r.CreateAsync(It.IsAny<Product>())).Callback<Product>(p => captured = p).ReturnsAsync(555);

            var service = new ProductService(mock.Object);

            var id = await service.CreateAsync(dto);

            Assert.Equal(555, id);
            Assert.NotNull(captured);
            Assert.Equal("NewProd", captured!.ProductName);
            Assert.Equal(12, captured.SupplierID);
            Assert.Equal(34, captured.CategoryID);
            Assert.Equal("10 boxes", captured.QuantityPerUnit);
            Assert.Equal(7.5m, captured.UnitPrice);
            Assert.Equal((short)20, captured.UnitsInStock);
            Assert.Equal((short)0, captured.UnitsOnOrder);
            Assert.Equal((short)5, captured.ReorderLevel);
            Assert.False(captured.Discontinued);
            mock.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NotFound_ReturnsFalse()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

            var service = new ProductService(mock.Object);

            var dto = new UpdateProductDto { ProductName = "X", Discontinued = true };

            var result = await service.UpdateAsync(1, dto);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_Success_UpdatesAndCallsRepository()
        {
            var original = new Product { ProductID = 10, ProductName = "Old", SupplierID = 2, UnitPrice = 1.1m, UnitsInStock = 1, Discontinued = false };
            Product? captured = null;

            var mock = new Mock<IProductRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(original);
            mock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Callback<Product>(p => captured = p).ReturnsAsync(true);

            var service = new ProductService(mock.Object);

            var dto = new UpdateProductDto { ProductName = "NewName", SupplierID = 99, UnitPrice = 12.34m, UnitsInStock = 50, UnitsOnOrder = 5, ReorderLevel = 2, Discontinued = true };

            var result = await service.UpdateAsync(10, dto);

            Assert.True(result);
            Assert.NotNull(captured);
            Assert.Equal(10, captured!.ProductID);
            Assert.Equal("NewName", captured.ProductName);
            Assert.Equal(99, captured.SupplierID);
            Assert.Equal(12.34m, captured.UnitPrice);
            Assert.Equal((short)50, captured.UnitsInStock);
            Assert.Equal((short)5, captured.UnitsOnOrder);
            Assert.Equal((short)2, captured.ReorderLevel);
            Assert.True(captured.Discontinued);
            mock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeletesAndReturnsResult()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);

            var service = new ProductService(mock.Object);

            var result = await service.DeleteAsync(7);

            Assert.True(result);
            mock.Verify(r => r.DeleteAsync(7), Times.Once);
        }
    }
}
