﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using RoseByte.AdoSession.Interfaces;
using RoseByte.AdoSession.Internals;

namespace RoseByte.AdoSession
{
    public class SqlServerSession : Session, ISqlServerSession
    {
        private readonly string _connectionString;
        private readonly IConnectionFactory _factory; 
        private IConnection _connection;
        private IConnection Connection => _connection ?? (_connection = _factory.Create(_connectionString));

        protected override (string, string) ParseConnectionString()
        {
            var csb = new SqlConnectionStringBuilder
            {
                ConnectionString = _connectionString
            };
            
            return (csb.InitialCatalog, csb.DataSource);
        }

        /// <summary>
        /// Executes given stored procedure
        /// </summary>
        /// <param name="name">Stored procedure name</param>
        /// <param name="parameters">parameters for stored procedure</param>
        /// <param name="timeout">command timeout in seconds and zero for infinity</param>
        public void ExecuteProcedure(string name, ParameterSet parameters = null, int timeout = 0)
        {
            Connection.Execute(name, parameters, CommandType.StoredProcedure, timeout);
        }

        /// <summary>
        /// Executes given stored procedure with multiple parameter sets as a batch
        /// </summary>
        /// <param name="name">Stored procedure name</param>
        /// <param name="parameterSets">parameters for stored procedure</param>
        public void ExecuteProcedureBatch(string name, IEnumerable<ParameterSet> parameterSets)
        {
            Connection.ExecuteBatch(name, parameterSets, CommandType.StoredProcedure);
        }

        /// <summary>
        /// Executes given stored procedure on transaction. Transaction is created by first call 
        /// and disposed by either Commit() or Rollback() command.
        /// </summary>
        /// <param name="name">Stored procedure name</param>
        /// <param name="parameters">parameters for stored procedure</param>
        public void ExecuteProcedureOnTransaction(string name, ParameterSet parameters = null)
        {
            Connection.ExecuteOnTransaction(name, parameters, CommandType.StoredProcedure);
        }

        /// <summary>
        /// Executes given stored procedure on transaction with multiple parameter sets as a batch.
        /// Transaction is created by first call and disposed by either Commit() or Rollback() command.
        /// </summary>
        /// <param name="name">Stored procedure name</param>
        /// <param name="parameterSets">parameters for stored procedure</param>
        public void ExecuteProcedureBatchOnTransaction(string name, IEnumerable<ParameterSet> parameterSets)
        {
            Connection.ExecuteBatchOnTransaction(name, parameterSets, CommandType.StoredProcedure);
        }

        /// <summary>
        /// Connection string in format "Data Source=..."
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlServerSession(string connectionString) : this(new ConnectionFactory(), connectionString)
        { }

        internal SqlServerSession(IConnectionFactory factory, string connectionString) 
            : base(factory, connectionString)
        {
            _factory = factory;
            _connectionString = connectionString;
        }
    }
}