using System.Data;

namespace Application.Interfaces
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
    }
}
