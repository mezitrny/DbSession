using DbSession.Connections;

namespace DbSession.Sqlite
{
    public class SqliteSession : Session
    {
        public SqliteSession(string connectionString)
            : this(new SqliteFactory(), connectionString)
        { }

        internal SqliteSession(IConnectionFactory factory, string connectionString) 
            : base(factory, connectionString)
        { }
    }
}