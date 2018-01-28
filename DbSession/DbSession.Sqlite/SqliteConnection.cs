using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DbSession.Connections;
using DbSession.Parameters;
using DbSession.ValueSets;

namespace DbSession.Sqlite
{
    public class SqliteConnection : IConnection
    {
        private readonly SQLiteConnection _connection;
        private readonly object _lock = new object();
        private SQLiteTransaction _transaction;
        private bool _isDisposed;

        public SqliteConnection(string connectionString)
        {
            _connection = new SQLiteConnection(connectionString);
        }

        public void ExecuteBatchOnTransaction(string sql, IEnumerable<DbParameterSet> parameterSets)
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

            var command = PrepareCommand(_connection, sql, enumerator.Current, _transaction);
            command.ExecuteNonQuery();

            while (enumerator.MoveNext())
            {
                ReuseCommand(command, enumerator.Current).ExecuteNonQuery();
            }

            enumerator.Dispose();
        }

        public void ExecuteOnTransaction(string sql, DbParameterSet parameters = null)
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

                PrepareCommand(_transaction.Connection, sql, parameters, _transaction)
                    .ExecuteNonQuery();
            }
        }

        public void ExecuteBatch(string sql, IEnumerable<DbParameterSet> parameterSets)
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
            var command = PrepareCommand(_connection, sql, enumerator.Current, transaction);
            command.ExecuteNonQuery();

            while (enumerator.MoveNext())
            {
                ReuseCommand(command, enumerator.Current).ExecuteNonQuery();
            }

            transaction.Commit();
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

        public IEnumerable<ValueSet> Select(string sql, DbParameterSet parameters = null)
        {
            EnsureOpen();
            var reader = PrepareCommand(_connection, sql, parameters).ExecuteReader();
            while (reader.Read())
            {
                yield return new ValueSet(reader);
            }
        }

        public object GetScalar(string sql, DbParameterSet parameters = null)
        {
            EnsureOpen();
            return PrepareCommand(_connection, sql, parameters)
                .ExecuteScalar();
        }

        public void Execute(string sql, DbParameterSet parameters = null)
        {
            EnsureOpen();
            PrepareCommand(_connection, sql, parameters)
                .ExecuteNonQuery();
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

        private static SQLiteCommand PrepareCommand(SQLiteConnection connection, string sql, DbParameterSet parameters, SQLiteTransaction transaction = null)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.CommandText = sql;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(
                        new SQLiteParameter(parameter.Name, Types[parameter.Type])
                        {
                            Value = parameter.Value
                        });
                }
            }

            return command;
        }

        private static readonly Dictionary<Type, DbType> Types = new Dictionary<Type, DbType>
        {
            {typeof(string), DbType.AnsiString },
            {typeof(int), DbType.Int32 },
            {typeof(int?), DbType.Int32 },
            {typeof(bool), DbType.Boolean },
            {typeof(bool?), DbType.Boolean },
            {typeof(DateTime), DbType.DateTime },
            {typeof(DateTime?), DbType.DateTime },
            {typeof(char), DbType.AnsiString },
            {typeof(char?), DbType.AnsiString },
            {typeof(decimal), DbType.Decimal },
            {typeof(decimal?), DbType.Decimal },
            {typeof(Guid), DbType.Guid },
            {typeof(Guid?), DbType.Guid },
            {typeof(long), DbType.Int64 },
            {typeof(long?), DbType.Int64 },
            {typeof(object), DbType.Object }
        };

        private static SQLiteCommand ReuseCommand(SQLiteCommand command, DbParameterSet parameters)
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
    }
}