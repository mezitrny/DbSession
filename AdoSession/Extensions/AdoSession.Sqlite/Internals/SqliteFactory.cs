using DbSession.Connections;

namespace Mezitrny.DbSession.Sqlite.Internals
{
    internal class SqliteFactory : IConnectionFactory
    {
        public IConnection Create(string connectionSting)
        {
            return new SqliteConnection(connectionSting);
        }
    }
}