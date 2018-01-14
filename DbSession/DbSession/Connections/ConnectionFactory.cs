namespace DbSession.Connections
{
    internal class ConnectionFactory : IConnectionFactory
    {
        public IConnection Create(string connectionSting)
        {
            return new Connection(connectionSting);
        }
    }
}