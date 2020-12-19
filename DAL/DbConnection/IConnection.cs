using System.Data;

namespace DAL.DbConnection
{
    public interface IConnection
    {
        IDbConnection LContext { get; }
    }
}
