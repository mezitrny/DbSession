namespace DbSession.Connections
{
    internal interface IConnectionFactory
    {
        IConnection Create(string connectionSting);
    }
}