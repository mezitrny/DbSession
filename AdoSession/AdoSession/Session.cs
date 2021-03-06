﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using RoseByte.AdoSession.Interfaces;
using RoseByte.AdoSession.Internals;

namespace RoseByte.AdoSession
{
    public class Session : ISession
    {
        private readonly string _connectionString;
        private readonly IConnectionFactory _factory; 
        private IConnection _connection;
        private IConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = _factory.Create(_connectionString);
                    _connection.MessageReceived += OnMessageReceived;
                    _delegates.Add(OnMessageReceived);
                }

                return _connection;
            }
        }
        private string _database;
        private string _server;
        
        /// <summary>
        /// Messages sent by SQL server.
        /// </summary>
        public event Action<string> MessageReceived;
        
        /// <summary>
        /// Errors sent by SQL server.
        /// </summary>
        public event Action<string> ErrorReceived;
        
        /// <summary>
        /// Toggles between message and error sending. Set to false by every Close().
        /// </summary>
        public bool FireInfoMessageEventOnUserErrors
        {
            get => Connection.FireInfoMessageEventOnUserErrors;
            set => Connection.FireInfoMessageEventOnUserErrors = value;
        }

        private void OnMessageReceived(object sender, SqlInfoMessageEventArgs args)
        {
            foreach (SqlError error in args.Errors)
            {
                if (error.Class > 10)
                {
                    ErrorReceived?.Invoke(error.Message);
                }
                else
                {
                    MessageReceived?.Invoke(args.Message);
                }
            }
        }

        private readonly List<EventHandler<SqlInfoMessageEventArgs>> _delegates;

        /// <summary>
        /// Returns session database's name
        /// </summary>
        public virtual string Database {
            get
            {
                if (_database == null)
                {
                    (_database, _server) = ParseConnectionString();
                }

                return _database;
            }
            
        }
        
        /// <summary>
        /// Returns session servers's name
        /// </summary>
        public virtual string Server {
            get
            {
                if (_server == null)
                {
                    (_database, _server) = ParseConnectionString();
                }

                return _server;
            } 
        }
        
        /// <summary>
        /// Returns ValueSet for each row fetched by given SQL script
        /// </summary>
        /// <param name="sql">sql command fetching rows</param>
        /// <param name="parameters">parameters for command</param>
        /// <returns>IEnumerable interface of IValueSet</returns>
        public IEnumerable<IValueSet> Select(string sql, ParameterSet parameters = null)
        {
            return Connection.Select(sql, parameters).ToList();
        }

        /// <summary>
        /// Executes given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        /// <param name="timeout">command timeout in seconds and zero for infinity</param>
        public void Execute(string sql, ParameterSet parameters = null, int timeout = 0)
        {
            Connection.Execute(sql, parameters, CommandType.Text, timeout);
        }

        /// <summary>
        /// Executes given SQL script with multiple parameter sets as a batch
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameterSets">parameters for script</param>
        public void ExecuteBatch(string sql, IEnumerable<ParameterSet> parameterSets)
        {
            Connection.ExecuteBatch(sql, parameterSets);
        }

        /// <summary>
        /// Executes given SQL script on transaction. Transaction is created by first call 
        /// and disposed by either Commit() or Rollback() command.
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        public void ExecuteOnTransaction(string sql, ParameterSet parameters = null)
        {
            Connection.ExecuteOnTransaction(sql, parameters);
        }

        /// <summary>
        /// Executes given SQL script on transaction with multiple parameter sets as a batch. Transaction is created by first call 
        /// and disposed by either Commit() or Rollback() command.
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameterSets">parameters for script</param>
        public void ExecuteBatchOnTransaction(string sql, IEnumerable<ParameterSet> parameterSets)
        {
            Connection.ExecuteBatchOnTransaction(sql, parameterSets);
        }

        /// <summary>
        /// Returns scalar value fetched by given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        /// <returns>object</returns>
        public object GetScalar(string sql, ParameterSet parameters = null)
        {
            return Connection.GetScalar(sql, parameters);
        }

        /// <summary>
        /// Returns scalar value fetched by given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        /// <returns>promitive type of generic type given to the method</returns>
        public T GetScalar<T>(string sql, ParameterSet parameters = null)
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
            foreach (var deleg in _delegates)
            {
                _connection.MessageReceived -= deleg;
            }
            
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
            _delegates = new List<EventHandler<SqlInfoMessageEventArgs>>();
        }

        public void Dispose()
        {
            CloseConnection();
        }

        protected virtual (string, string) ParseConnectionString()
        {
            var csb = new DbConnectionStringBuilder { ConnectionString = _connectionString };
            return (csb["Initial Catalog"].ToString(), csb["Data Source"].ToString());
        }

    }
}