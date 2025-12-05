using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dtos.Category;
using Application.Interfaces;
using Application.Services.Category;
using Domain.Entities;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
    public class CategoryServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            var categories = new List<Category>
            {
                new Category { CategoryID = 1, CategoryName = "Beverages", Description = "Drinks", Picture = new byte[] {1,2,3} },
                new Category { CategoryID = 2, CategoryName = "Condiments", Description = "Sauces", Picture = new byte[] {4,5,6} }
            };

            var mock = new Mock<ICategoryRepository>();
            mock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var service = new CategoryService(mock.Object);

            var result = (await service.GetAllAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].CategoryID);
            Assert.Equal("Beverages", result[0].CategoryName);
            Assert.Equal("Drinks", result[0].Description);
            Assert.Equal(new byte[] {1,2,3}, result[0].Picture);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            var mock = new Mock<ICategoryRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Category?)null);

            var service = new CategoryService(mock.Object);

            var result = await service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Found_ReturnsDto()
        {
            var category = new Category { CategoryID = 5, CategoryName = "Seafood", Description = "Fish", Picture = new byte[] {9} };
            var mock = new Mock<ICategoryRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(category);

            var service = new CategoryService(mock.Object);

            var result = await service.GetByIdAsync(5);

            Assert.NotNull(result);
            Assert.Equal(5, result!.CategoryID);
            Assert.Equal("Seafood", result.CategoryName);
            Assert.Equal("Fish", result.Description);
            Assert.Equal(new byte[] {9}, result.Picture);
        }

        [Fact]
        public async Task CreateAsync_DuplicateName_ThrowsInvalidOperationException()
        {
            var existing = new Category { CategoryID = 2, CategoryName = "Beverages" };
            var mock = new Mock<ICategoryRepository>();
            mock.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(existing);

            var service = new CategoryService(mock.Object);

            var dto = new CreateCategoryDto { CategoryName = "Beverages", Description = "x" };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_Success_ReturnsId()
        {
            var mock = new Mock<ICategoryRepository>();
            mock.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync((Category?)null);
            mock.Setup(r => r.CreateAsync(It.IsAny<Category>())).ReturnsAsync(42);

            var service = new CategoryService(mock.Object);

            var dto = new CreateCategoryDto { CategoryName = "NewCat", Description = "desc" };

            var id = await service.CreateAsync(dto);

            Assert.Equal(42, id);
            mock.Verify(r => r.CreateAsync(It.Is<Category>(c => c.CategoryName == "NewCat")), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NotFound_ReturnsFalse()
        {
            var mock = new Mock<ICategoryRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Category?)null);

            var service = new CategoryService(mock.Object);

            var dto = new UpdateCategoryDto { CategoryName = "Whatever" };

            var result = await service.UpdateAsync(1, dto);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_DuplicateNameDifferentId_ThrowsInvalidOperationException()
        {
            var original = new Category { CategoryID = 1, CategoryName = "Old" };
            var existingWithName = new Category { CategoryID = 2, CategoryName = "NewName" };

            var mock = new Mock<ICategoryRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(original);
            mock.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(existingWithName);

            var service = new CategoryService(mock.Object);

            var dto = new UpdateCategoryDto { CategoryName = "NewName" };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAsync(1, dto));
        }

        [Fact]
        public async Task UpdateAsync_Success_CallsRepositoryAndReturnsResult()
        {
            var original = new Category { CategoryID = 10, CategoryName = "Old", Description = "old desc", Picture = new byte[] {1} };

            var mock = new Mock<ICategoryRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(original);
            mock.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync((Category?)null);

            Category? captured = null;
            mock.Setup(r => r.UpdateAsync(It.IsAny<Category>())).Callback<Category>(c => captured = c).ReturnsAsync(true);

            var service = new CategoryService(mock.Object);

            var dto = new UpdateCategoryDto { CategoryName = "UpdatedName", Description = "new desc", Picture = new byte[] {2,3} };

            var result = await service.UpdateAsync(10, dto);

            Assert.True(result);
            Assert.NotNull(captured);
            Assert.Equal(10, captured!.CategoryID);
            Assert.Equal("UpdatedName", captured.CategoryName);
            Assert.Equal("new desc", captured.Description);
            Assert.Equal(new byte[] {2,3}, captured.Picture);
            mock.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeletesAndReturnsResult()
        {
            var mock = new Mock<ICategoryRepository>();
            mock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);

            var service = new CategoryService(mock.Object);

            var result = await service.DeleteAsync(7);

            Assert.True(result);
            mock.Verify(r => r.DeleteAsync(7), Times.Once);
        }
    }
}
