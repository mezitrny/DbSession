using RoseByte.AdoSession.Internals;

namespace RoseByte.AdoSession.Sqlite
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