using System;
using System.Collections.Generic;
using DbSession.Connections;
using DbSession.Parameters;
using DbSession.ValueSets;

namespace DbSession
{
    public class Session : ISession
    {
        private readonly string _connectionString;
        private readonly IConnectionFactory _factory; 
        private IConnection _connection;
        private IConnection Connection => _connection ?? (_connection = _factory.Create(_connectionString));

        /// <summary>
        /// Returns ValueSet for each row fetched by given SQL script
        /// </summary>
        /// <param name="sql">sql command fetching rows</param>
        /// <param name="parameters">parameters for command</param>
        /// <returns>IEnumerable interface of IValueSet</returns>
        public IEnumerable<IValueSet> Select(string sql, SqlParameterSet parameters = null)
        {
            return Connection.Select(sql, parameters);
        }

        /// <summary>
        /// Executes given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        public void Execute(string sql, SqlParameterSet parameters = null)
        {
            Connection.Execute(sql, parameters);
        }

        /// <summary>
        /// Executes given SQL script on transaction. Transaction is created by first call 
        /// and disposed by either Commit() or Rollback() command.
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        public void ExecuteOnTransaction(string sql, SqlParameterSet parameters = null)
        {
            Connection.ExecuteOnTransaction(sql, parameters);
        }

        /// <summary>
        /// Returns scalar value fetched by given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        /// <returns>object</returns>
        public object GetScalar(string sql, SqlParameterSet parameters = null)
        {
            return Connection.GetScalar(sql, parameters);
        }

        /// <summary>
        /// Returns scalar value fetched by given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        /// <returns>promitive type of generic type given to the method</returns>
        public T GetScalar<T>(string sql, SqlParameterSet parameters = null)
        {
            return (T)Convert.ChangeType(Connection.GetScalar(sql, parameters), typeof(T));
        }

        /// <summary>
        /// Commits current transaction if any.
        /// </summary>
        public void Commit()
        {
            _connection?.Commit();
        }

        /// <summary>
        /// Cancels current transaction if any.
        /// </summary>
        public void RollBack()
        {
            _connection?.RollBack();
        }

        /// <summary>
        /// Closes current connection. Connection instance will be disposed and renewed with
        /// first next call to database.
        /// </summary>
        public void CloseConnection()
        {
            _connection?.Dispose();
            _connection = null;
        }

        /// <summary>
        /// Connection string in format "Data Source=..."
        /// </summary>
        /// <param name="connectionString"></param>
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