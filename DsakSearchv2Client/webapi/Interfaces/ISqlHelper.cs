using System.Data;

namespace webapi.Interfaces
{
    public interface ISqlHelper
    {
        IDbConnection CreateConnection();
    }
}
