using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
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
        public IEnumerable<IValueSet> Select(string sql, DbParameterSet parameters = null)
        {
            return Connection.Select(sql, parameters);
        }

        /// <summary>
        /// Executes given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        public void Execute(string sql, DbParameterSet parameters = null)
        {
            Connection.Execute(sql, parameters);
        }

        /// <summary>
        /// Executes given SQL script with multiple parameter sets as a batch
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameterSets">parameters for script</param>
        public void ExecuteBatch(string sql, IEnumerable<DbParameterSet> parameterSets)
        {
            Connection.ExecuteBatch(sql, parameterSets);
        }

        /// <summary>
        /// Executes given SQL script on transaction. Transaction is created by first call 
        /// and disposed by either Commit() or Rollback() command.
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        public void ExecuteOnTransaction(string sql, DbParameterSet parameters = null)
        {
            Connection.ExecuteOnTransaction(sql, parameters);
        }

        /// <summary>
        /// Executes given SQL script on transaction with multiple parameter sets as a batch. Transaction is created by first call 
        /// and disposed by either Commit() or Rollback() command.
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameterSets">parameters for script</param>
        public void ExecuteBatchOnTransaction(string sql, IEnumerable<DbParameterSet> parameterSets)
        {
            Connection.ExecuteBatchOnTransaction(sql, parameterSets);
        }

        /// <summary>
        /// Returns scalar value fetched by given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        /// <returns>object</returns>
        public object GetScalar(string sql, DbParameterSet parameters = null)
        {
            return Connection.GetScalar(sql, parameters);
        }

        /// <summary>
        /// Returns scalar value fetched by given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        /// <returns>promitive type of generic type given to the method</returns>
        public T GetScalar<T>(string sql, DbParameterSet parameters = null)
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
        /// Returns content of embedded resource as a string.
        /// </summary>
        /// <param name="path">namespace of embedded resource (remember add folders' names)</param>
        /// <returns>string</returns>
        public string ReadEmbedded(string path)
        {
            using (var stm = Assembly.GetCallingAssembly().GetManifestResourceStream(path))
            {
                if (stm == null)
                {
                    throw new ArgumentException($"Resource script '{path}' couldn't be found.");
                }

                return new StreamReader(stm).ReadToEnd();
            }
        }

        /// <summary>
        /// Returns content of resource item as a string.
        /// </summary>
        /// <param name="path">namespace of resource (remember add folders' names)</param>
        /// <param name="key">key in resource</param>
        /// <returns>string</returns>
        public string ReadResource(string path, string key)
        {
            try
            {
                var rm = new ResourceManager(path, Assembly.GetCallingAssembly());

                var item = rm.GetString(key);

                if (item == null)
                {
                    throw new ArgumentException($"Resource '{path}' file doesn't contain item '{key}'.");
                }

                return item;
            }
            catch (MissingManifestResourceException)
            {
                throw new ArgumentException($"Resource file '{path}' couldn't be found.");
            }
        }

        /// <summary>
        /// Returns content of file as a string.
        /// </summary>
        /// <param name="path">path to file</param>
        /// <returns>string</returns>
        public string ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException($"File '{path}' couldn't be found.");
            }

            return File.ReadAllText(path);
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