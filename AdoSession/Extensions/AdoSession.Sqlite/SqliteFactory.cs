using RoseByte.AdoSession.Internals;

namespace RoseByte.AdoSession.Sqlite
{
    internal class SqliteFactory : IConnectionFactory
    {
        public IConnection Create(string connectionSting)
        {
            return new SqliteConnection(connectionSting);
        }
    }
}