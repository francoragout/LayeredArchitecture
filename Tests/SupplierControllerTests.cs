using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dtos.Supplier;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests
{
    public class SupplierControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkWithSuppliers()
        {
            var items = new List<ReadSupplierDto>
            {
                new ReadSupplierDto { SupplierID = 1, CompanyName = "Comp1" },
                new ReadSupplierDto { SupplierID = 2, CompanyName = "Comp2" }
            };

            var mock = new Mock<Application.Services.Supplier.ISupplierService>();
            mock.Setup(s => s.GetAllAsync()).ReturnsAsync(items);

            var controller = new API.Controllers.SupplierController(mock.Object);

            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(items, ok.Value);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var mock = new Mock<Application.Services.Supplier.ISupplierService>();
            mock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ReadSupplierDto?)null);

            var controller = new API.Controllers.SupplierController(mock.Object);

            var result = await controller.GetById(5);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_Found_ReturnsOkWithItem()
        {
            var dto = new ReadSupplierDto { SupplierID = 7, CompanyName = "X" };
            var mock = new Mock<Application.Services.Supplier.ISupplierService>();
            mock.Setup(s => s.GetByIdAsync(7)).ReturnsAsync(dto);

            var controller = new API.Controllers.SupplierController(mock.Object);

            var result = await controller.GetById(7);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(dto, ok.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var dto = new CreateSupplierDto { CompanyName = "NewComp" };
            var mock = new Mock<Application.Services.Supplier.ISupplierService>();
            mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(123);

            var controller = new API.Controllers.SupplierController(mock.Object);

            var result = await controller.Create(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(API.Controllers.SupplierController.GetById), created.ActionName);
            Assert.NotNull(created.RouteValues);
            Assert.True(created.RouteValues.ContainsKey("id"));
            Assert.Equal(123, created.RouteValues!["id"]);
            Assert.Null(created.Value);
        }

        [Fact]
        public async Task Update_NotFound_ReturnsNotFound()
        {
            var dto = new UpdateSupplierDto { CompanyName = "X" };
            var mock = new Mock<Application.Services.Supplier.ISupplierService>();
            mock.Setup(s => s.UpdateAsync(5, dto)).ReturnsAsync(false);

            var controller = new API.Controllers.SupplierController(mock.Object);

            var result = await controller.Update(5, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_Success_ReturnsNoContent()
        {
            var dto = new UpdateSupplierDto { CompanyName = "Updated" };
            var mock = new Mock<Application.Services.Supplier.ISupplierService>();
            mock.Setup(s => s.UpdateAsync(8, dto)).ReturnsAsync(true);

            var controller = new API.Controllers.SupplierController(mock.Object);

            var result = await controller.Update(8, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_NotFound_ReturnsNotFound()
        {
            var mock = new Mock<Application.Services.Supplier.ISupplierService>();
            mock.Setup(s => s.DeleteAsync(9)).ReturnsAsync(false);

            var controller = new API.Controllers.SupplierController(mock.Object);

            var result = await controller.Delete(9);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Success_ReturnsNoContent()
        {
            var mock = new Mock<Application.Services.Supplier.ISupplierService>();
            mock.Setup(s => s.DeleteAsync(9)).ReturnsAsync(true);

            var controller = new API.Controllers.SupplierController(mock.Object);

            var result = await controller.Delete(9);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
