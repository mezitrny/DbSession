namespace DbSession.Core
{
    internal class ConnectionFactory : IConnectionFactory
    {
        public IConnection Create(string connectionSting)
        {
            return new Connection(connectionSting);
        }
    }
}