namespace DbSession.Core
{
    internal interface IConnectionFactory
    {
        IConnection Create(string connectionSting);
    }
}