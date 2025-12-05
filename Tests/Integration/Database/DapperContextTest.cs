using System;
using System.Data;
using Microsoft.Data.Sqlite;
using Application.Interfaces;

namespace Tests.Integration.Database
{
    // Lightweight IDapperContext for tests using SQLite in-memory DB.
    public sealed class DapperContextTest : IDapperContext, IDisposable
    {
        private readonly string _connectionString;
        private readonly SqliteConnection _keepAlive;

        public DapperContextTest(string dbName = "testdb")
        {
            // Use shared cache so multiple connections see the same in-memory DB.
            _connectionString = $"Data Source=file:{dbName}?mode=memory&cache=shared";
            _keepAlive = new SqliteConnection(_connectionString);
            _keepAlive.Open(); // keep DB alive
        }

        public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);

        public void Dispose()
        {
            _keepAlive?.Dispose();
        }
    }
}
