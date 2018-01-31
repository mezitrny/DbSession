using RoseByte.AdoSession.SqlServer;

namespace RoseByte.AdoSession.Internals
{
    internal class ConnectionFactory : IConnectionFactory
    {
        public IConnection Create(string connectionSting)
        {
            return new SqlServerConnection(connectionSting);
        }
    }
}