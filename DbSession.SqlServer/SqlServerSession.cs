using DbSession.Core;

namespace DbSession.Sqlite
{
    public class SqliteSession : Session
    {
        public SqliteSession(string connectionString) : base(connectionString)
        { }

        protected override IConnection CreateConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
    }
}