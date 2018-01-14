using System;
using System.Collections.Generic;
using DbSession.Parameters;
using DbSession.ValueSets;

namespace DbSession
{
    public interface ISession : IDisposable
    {
        /// <summary>
        /// Returns ValueSet for each row fetched by given SQL script
        /// </summary>
        /// <param name="sql">sql command fetching rows</param>
        /// <param name="parameters">parameters for command</param>
        /// <returns>IEnumerable interface of IValueSet</returns>
        IEnumerable<IValueSet> Select(string sql, SqlParameterSet parameters = null);

        /// <summary>
        /// Executes given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        void Execute(string sql, SqlParameterSet parameters = null);

        /// <summary>
        /// Executes given SQL script on transaction. Transaction is created by first call 
        /// and disposed by either Commit() or Rollback() command.
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        void ExecuteOnTransaction(string sql, SqlParameterSet parameters = null);

        /// <summary>
        /// Returns scalar value fetched by given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        /// <returns>object</returns>
        object GetScalar(string sql, SqlParameterSet parameters = null);

        /// <summary>
        /// Returns scalar value fetched by given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        /// <returns>promitive type of generic type given to the method</returns>
        T GetScalar<T>(string sql, SqlParameterSet parameters = null);

        /// <summary>
        /// Commits current transaction if any.
        /// </summary>
        void Commit();

        /// <summary>
        /// Cancels current transaction if any.
        /// </summary>
        void RollBack();

        /// <summary>
        /// Closes current connection. Connection instance will be disposed and renewed with
        /// first next call to database.
        /// </summary>
        void CloseConnection();
    }
}