using System;
using System.Collections.Generic;

namespace DbSession.Core
{
    public class Session : ISession
    {
        private readonly string _connectionString;
        private readonly IConnectionFactory _factory; 
        private IConnection _connection;
        private IConnection Connection => _connection ?? (_connection = _factory.Create(_connectionString));

        public IEnumerable<ValueSet> Select(string sql, SqlParameterSet parameters = null)
        {
            return Connection.Select(sql, parameters);
        }

        public void Execute(string sql, SqlParameterSet parameters = null)
        {
            Connection.Execute(sql, parameters);
        }

        public void ExecuteOnTransaction(string sql, SqlParameterSet parameters = null)
        {
            Connection.ExecuteOnTransaction(sql, parameters);
        }

        public object GetScalar(string sql, SqlParameterSet parameters = null)
        {
            return Connection.GetScalar(sql, parameters);
        }

        public T GetScalar<T>(string sql, SqlParameterSet parameters = null)
        {
            return (T)Convert.ChangeType(Connection.GetScalar(sql, parameters), typeof(T));
        }

        public void Commit()
        {
            _connection?.Commit();
        }

        public void RollBack()
        {
            _connection?.RollBack();
        }

        public void CloseConnection()
        {
            _connection?.Dispose();
            _connection = null;
        }

        public Session(string connectionString) : this(new ConnectionFactory(), connectionString)
        { }

        internal Session(IConnectionFactory factory, string connectionString)
        {
            _factory = factory;
            _connectionString = connectionString;
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}