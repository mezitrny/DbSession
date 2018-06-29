using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RoseByte.AdoSession.Interfaces;
using RoseByte.AdoSession.Internals;

namespace RoseByte.AdoSession.SqlServer
{
    internal class SqlServerConnection : IConnection
    {
        private readonly SqlConnection _connection;
        private readonly object _lock = new object();
        private SqlTransaction _transaction;
        private bool _isDisposed;

        public SqlServerConnection(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public void ExecuteOnTransaction(string sql, ParameterSet parameters = null, 
            CommandType commandType = CommandType.Text)
        {
            lock (_lock)
            {
                if (_transaction == null)
                {
                    EnsureOpen();
                    _transaction = _connection.BeginTransaction();
                }

                if (_transaction.Connection.State == ConnectionState.Closed)
                {
                    _transaction.Connection.Open();
                }

                PrepareCommand(_transaction.Connection, sql, parameters, _transaction, commandType)
                    .ExecuteNonQuery();
            }
        }
        
        public void ExecuteBatchOnTransaction(string sql, IEnumerable<ParameterSet> parameterSets, 
            CommandType commandType = CommandType.Text)
        {
            if (parameterSets == null)
            {
                return;
            }

            lock (_lock)
            {
                if (_transaction == null)
                {
                    EnsureOpen();
                    _transaction = _connection.BeginTransaction();
                }
            }

            var enumerator = parameterSets.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                return;
            }

            EnsureOpen();

            var command = PrepareCommand(_connection, sql, enumerator.Current, _transaction, commandType);
            command.ExecuteNonQuery();

            while (enumerator.MoveNext())
            {
                ReuseCommand(command, enumerator.Current).ExecuteNonQuery();
            }

            enumerator.Dispose();
        }

        public void Commit()
        {
            lock (_lock)
            {
                _transaction?.Commit();
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public void RollBack()
        {
            lock (_lock)
            {
                _transaction?.Rollback();
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public IEnumerable<IValueSet> Select(string sql, ParameterSet parameters = null)
        {
            EnsureOpen();
            var reader = PrepareCommand(_connection, sql, parameters, null, CommandType.Text).ExecuteReader();
            while (reader.Read())
            {
                yield return new ValueSet(reader);
            }
            reader.Close();
            reader.Dispose();
        }

        public object GetScalar(string sql, ParameterSet parameters = null)
        {
            EnsureOpen();
            return PrepareCommand(_connection, sql, parameters, null, CommandType.Text)
                .ExecuteScalar();
        }

        public void Execute(string sql, ParameterSet parameters = null, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            EnsureOpen();
            PrepareCommand(_connection, sql, parameters, null, commandType, timeout)
                .ExecuteNonQuery();
        }

        public void ExecuteBatch(string sql, IEnumerable<ParameterSet> parameterSets, CommandType commandType = CommandType.Text)
        {
            if (parameterSets == null)
            {
                return;
            }

            var enumerator = parameterSets.GetEnumerator();
            
            if (!enumerator.MoveNext())
            {
                return;
            }

            EnsureOpen();
            var transaction = _connection.BeginTransaction();
            var command = PrepareCommand(_connection, sql, enumerator.Current, transaction, commandType);
            command.ExecuteNonQuery();

            while (enumerator.MoveNext())
            {
                ReuseCommand(command, enumerator.Current).ExecuteNonQuery();
            }

            transaction.Commit();
            enumerator.Dispose();
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_isDisposed)
                {
                    return;
                }

                RollBack();

                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }

                _connection.Dispose();
                _isDisposed = true;
            }
        }

        private void EnsureOpen()
        {
            lock (_lock)
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
            }
        }

        private static SqlCommand ReuseCommand(SqlCommand command, ParameterSet parameters)
        {
            if (parameters == null)
            {
                return command;
            }

            foreach (var parameter in parameters)
            {
                command.Parameters[parameter.Name].Value = parameter.Value;
            }

            return command;
        }

        private static SqlCommand PrepareCommand(SqlConnection connection, string sql, ParameterSet parameters, 
            SqlTransaction transaction, CommandType commandType, int timeout = 0)
        {
            var command = connection.CreateCommand();
            command.CommandType = commandType;
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.CommandText = sql;
            command.CommandTimeout = timeout;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(
                        new SqlParameter(parameter.Name, Types[parameter.Type])
                        {
                            Value = parameter.Value
                        });
                }
            }

            return command;
        }

        private static readonly Dictionary<Type, SqlDbType> Types = new Dictionary<Type, SqlDbType>
        {
            {typeof(string), SqlDbType.NVarChar },
            {typeof(int), SqlDbType.Int },
            {typeof(int?), SqlDbType.Int },
            {typeof(bool), SqlDbType.Bit },
            {typeof(bool?), SqlDbType.Bit },
            {typeof(DateTime), SqlDbType.DateTime },
            {typeof(DateTime?), SqlDbType.DateTime },
            {typeof(char), SqlDbType.NChar },
            {typeof(char?), SqlDbType.NChar },
            {typeof(decimal), SqlDbType.Decimal },
            {typeof(decimal?), SqlDbType.Decimal },
            {typeof(Guid), SqlDbType.Timestamp },
            {typeof(Guid?), SqlDbType.Timestamp },
            {typeof(long), SqlDbType.BigInt },
            {typeof(long?), SqlDbType.BigInt },
            {typeof(object), SqlDbType.Variant }
        };
    }
}