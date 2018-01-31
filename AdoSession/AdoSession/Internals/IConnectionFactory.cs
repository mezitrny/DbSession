namespace RoseByte.AdoSession.Internals
{
    internal interface IConnectionFactory
    {
        IConnection Create(string connectionSting);
    }
}