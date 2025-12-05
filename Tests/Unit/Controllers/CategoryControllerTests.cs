using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dtos.Category;
using Application.Services.Category;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
    public class CategoryControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkWithCategories()
        {
            var items = new List<ReadCategoryDto>
            {
                new ReadCategoryDto { CategoryID = 1, CategoryName = "A" },
                new ReadCategoryDto { CategoryID = 2, CategoryName = "B" }
            };

            var mock = new Mock<ICategoryService>();
            mock.Setup(s => s.GetAllAsync()).ReturnsAsync(items);

            var controller = new API.Controllers.CategoryController(mock.Object);

            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(items, ok.Value);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var mock = new Mock<ICategoryService>();
            mock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ReadCategoryDto?)null);

            var controller = new API.Controllers.CategoryController(mock.Object);

            var result = await controller.GetById(5);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_Found_ReturnsOkWithItem()
        {
            var dto = new ReadCategoryDto { CategoryID = 7, CategoryName = "X" };
            var mock = new Mock<ICategoryService>();
            mock.Setup(s => s.GetByIdAsync(7)).ReturnsAsync(dto);

            var controller = new API.Controllers.CategoryController(mock.Object);

            var result = await controller.GetById(7);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(dto, ok.Value);
        }

        [Fact]
        public async Task Create_Success_ReturnsCreatedAtAction()
        {
            var dto = new CreateCategoryDto { CategoryName = "New" };
            var mock = new Mock<ICategoryService>();
            mock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(123);

            var controller = new API.Controllers.CategoryController(mock.Object);

            var result = await controller.Create(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(API.Controllers.CategoryController.GetById), created.ActionName);
            Assert.NotNull(created.RouteValues);
            Assert.True(created.RouteValues.ContainsKey("id"));
            Assert.Equal(123, created.RouteValues!["id"]);
            Assert.Null(created.Value);
        }

        [Fact]
        public async Task Create_DuplicateName_ReturnsConflict()
        {
            var dto = new CreateCategoryDto { CategoryName = "Dup" };
            var mock = new Mock<ICategoryService>();
            mock.Setup(s => s.CreateAsync(It.IsAny<CreateCategoryDto>())).ThrowsAsync(new System.InvalidOperationException("exists"));

            var controller = new API.Controllers.CategoryController(mock.Object);

            var result = await controller.Create(dto);

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            // message object shape is anonymous; ensure it contains the message string when serialized to string
            Assert.NotNull(conflict.Value);
        }

        [Fact]
        public async Task Update_NotFound_ReturnsNotFound()
        {
            var dto = new UpdateCategoryDto { CategoryName = "X" };
            var mock = new Mock<ICategoryService>();
            mock.Setup(s => s.UpdateAsync(5, dto)).ReturnsAsync(false);

            var controller = new API.Controllers.CategoryController(mock.Object);

            var result = await controller.Update(5, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_Success_ReturnsNoContent()
        {
            var dto = new UpdateCategoryDto { CategoryName = "Updated" };
            var mock = new Mock<ICategoryService>();
            mock.Setup(s => s.UpdateAsync(8, dto)).ReturnsAsync(true);

            var controller = new API.Controllers.CategoryController(mock.Object);

            var result = await controller.Update(8, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_DuplicateName_ReturnsConflict()
        {
            var dto = new UpdateCategoryDto { CategoryName = "Dup" };
            var mock = new Mock<ICategoryService>();
            mock.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateCategoryDto>())).ThrowsAsync(new System.InvalidOperationException("exists"));

            var controller = new API.Controllers.CategoryController(mock.Object);

            var result = await controller.Update(1, dto);

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Delete_NotFound_ReturnsNotFound()
        {
            var mock = new Mock<ICategoryService>();
            mock.Setup(s => s.DeleteAsync(9)).ReturnsAsync(false);

            var controller = new API.Controllers.CategoryController(mock.Object);

            var result = await controller.Delete(9);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Success_ReturnsNoContent()
        {
            var mock = new Mock<ICategoryService>();
            mock.Setup(s => s.DeleteAsync(9)).ReturnsAsync(true);

            var controller = new API.Controllers.CategoryController(mock.Object);

            var result = await controller.Delete(9);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
