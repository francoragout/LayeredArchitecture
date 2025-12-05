using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dtos.Product;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
    public class ProductControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkWithProducts()
        {
            var items = new List<ReadProductDto>
            {
                new ReadProductDto { ProductID = 1, ProductName = "P1" },
                new ReadProductDto { ProductID = 2, ProductName = "P2" }
            };

            var mock = new Mock<Application.Services.Product.IProductService>();
            mock.Setup(s => s.GetAllAsync()).ReturnsAsync(items);

            var controller = new API.Controllers.ProductController(mock.Object);

            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(items, ok.Value);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var mock = new Mock<Application.Services.Product.IProductService>();
            mock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ReadProductDto?)null);

            var controller = new API.Controllers.ProductController(mock.Object);

            var result = await controller.GetById(5);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_Found_ReturnsOkWithItem()
        {
            var dto = new ReadProductDto { ProductID = 7, ProductName = "X" };
            var mock = new Mock<Application.Services.Product.IProductService>();
            mock.Setup(s => s.GetByIdAsync(7)).ReturnsAsync(dto);

            var controller = new API.Controllers.ProductController(mock.Object);

            var result = await controller.GetById(7);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(dto, ok.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var dto = new CreateProductDto { ProductName = "NewProd" };
            var mock = new Mock<Application.Services.Product.IProductService>();
            mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(123);

            var controller = new API.Controllers.ProductController(mock.Object);

            var result = await controller.Create(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(API.Controllers.ProductController.GetById), created.ActionName);
            Assert.NotNull(created.RouteValues);
            Assert.True(created.RouteValues.ContainsKey("id"));
            Assert.Equal(123, created.RouteValues!["id"]);
            Assert.Null(created.Value);
        }

        [Fact]
        public async Task Update_NotFound_ReturnsNotFound()
        {
            var dto = new UpdateProductDto { ProductName = "X" };
            var mock = new Mock<Application.Services.Product.IProductService>();
            mock.Setup(s => s.UpdateAsync(5, dto)).ReturnsAsync(false);

            var controller = new API.Controllers.ProductController(mock.Object);

            var result = await controller.Update(5, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_Success_ReturnsNoContent()
        {
            var dto = new UpdateProductDto { ProductName = "Updated" };
            var mock = new Mock<Application.Services.Product.IProductService>();
            mock.Setup(s => s.UpdateAsync(8, dto)).ReturnsAsync(true);

            var controller = new API.Controllers.ProductController(mock.Object);

            var result = await controller.Update(8, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_NotFound_ReturnsNotFound()
        {
            var mock = new Mock<Application.Services.Product.IProductService>();
            mock.Setup(s => s.DeleteAsync(9)).ReturnsAsync(false);

            var controller = new API.Controllers.ProductController(mock.Object);

            var result = await controller.Delete(9);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Success_ReturnsNoContent()
        {
            var mock = new Mock<Application.Services.Product.IProductService>();
            mock.Setup(s => s.DeleteAsync(9)).ReturnsAsync(true);

            var controller = new API.Controllers.ProductController(mock.Object);

            var result = await controller.Delete(9);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
