using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Xunit;
using Infrastructure.Database;
using Tests.Integration.Database;

namespace Tests.Integration.Repositories
{
    public class CategoryRepositoryTest
    {
        [Fact]
        public async Task Create_GetAll_GetById_Update_Delete_flow()
        {
            using var ctx = new DapperContextTest("catstest");

            using var conn = ctx.CreateConnection();
            await conn.OpenSafeAsync();

            // Create table compatible with repository queries
            var createTable = @"
                CREATE TABLE Categories (
                    CategoryID INTEGER PRIMARY KEY AUTOINCREMENT,
                    CategoryName TEXT NOT NULL,
                    Description TEXT,
                    Picture BLOB
                );
            ";

            await conn.ExecuteAsync(createTable);

            var repo = new Infrastructure.Repositories.CategoryRepository(ctx);

            // SQLite doesn't support SQL Server's OUTPUT clause used in the repository CreateAsync implementation.
            // Insert directly here and obtain last inserted id via last_insert_rowid().
            var insertQuery = @"
                INSERT INTO Categories (CategoryName, Description, Picture)
                VALUES (@CategoryName, @Description, @Picture);
                SELECT last_insert_rowid();
            ";

            var idLong = await conn.ExecuteScalarAsync<long>(insertQuery, new { CategoryName = "T1", Description = "d1", Picture = (byte[]?)null });
            var id = (int)idLong;
            id.Should().BeGreaterThan(0);

            // GetAll
            var all = (await repo.GetAllAsync()).ToList();
            all.Should().ContainSingle().Which.CategoryName.Should().Be("T1");

            // GetById
            var item = await repo.GetByIdAsync(id);
            item.Should().NotBeNull();
            item!.CategoryName.Should().Be("T1");

            // GetByName
            var byName = await repo.GetByNameAsync("T1");
            byName.Should().NotBeNull();
            byName!.CategoryID.Should().Be(id);

            // Update
            item.Description = "updated";
            var updated = await repo.UpdateAsync(item);
            updated.Should().BeTrue();

            var item2 = await repo.GetByIdAsync(id);
            item2!.Description.Should().Be("updated");

            // Delete
            var deleted = await repo.DeleteAsync(id);
            deleted.Should().BeTrue();

            var after = await repo.GetByIdAsync(id);
            after.Should().BeNull();
        }
    }
}
