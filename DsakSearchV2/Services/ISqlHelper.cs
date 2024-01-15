using System.Data;

namespace DsakSearchV2.Services
{
    public interface ISqlHelper
    {
        IDbConnection CreateConnection();
    }
}
