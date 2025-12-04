using System.Data;
using System.Data.Common;


namespace Infrastructure.Database
{
    public static class DbConnectionExtensions
    {
        // Esta extensión permite abrir conexiones de manera asíncrona si el proveedor lo soporta. Ej: SqlConnection.
        public static Task OpenSafeAsync(this IDbConnection connection)
        {
            if (connection is DbConnection dbConnection)
                return dbConnection.OpenAsync();

            connection.Open();
            return Task.CompletedTask;
        }
    }
}
