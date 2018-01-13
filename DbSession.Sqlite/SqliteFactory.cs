using DbSession.Core;

namespace DbSession.Sqlite
{
    internal class SqliteFactory : IConnectionFactory
    {
        public IConnection Create(string connectionSting)
        {
            return new SqliteConnection(connectionSting);
        }
    }
}