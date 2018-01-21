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
        IEnumerable<IValueSet> Select(string sql, DbParameterSet parameters = null);

        /// <summary>
        /// Executes given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        void Execute(string sql, DbParameterSet parameters = null);

        /// <summary>
        /// Executes given SQL script with multiple parameter sets as a batch
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameterSets">parameters for script</param>
        void Execute(string sql, IEnumerable<DbParameterSet> parameterSets);

        /// <summary>
        /// Executes given SQL script on transaction. Transaction is created by first call 
        /// and disposed by either Commit() or Rollback() command.
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        void ExecuteOnTransaction(string sql, DbParameterSet parameters = null);

        /// <summary>
        /// Executes given SQL script on transaction with multiple parameter sets as a batch. Transaction is created by first call 
        /// and disposed by either Commit() or Rollback() command.
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameterSets">parameters for script</param>
        void ExecuteOnTransaction(string sql, IEnumerable<DbParameterSet> parameterSets);

        /// <summary>
        /// Returns scalar value fetched by given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        /// <returns>object</returns>
        object GetScalar(string sql, DbParameterSet parameters = null);

        /// <summary>
        /// Returns scalar value fetched by given SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">parameters for script</param>
        /// <returns>promitive type of generic type given to the method</returns>
        T GetScalar<T>(string sql, DbParameterSet parameters = null);

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

        /// <summary>
        /// Returns content of embedded resource as a string.
        /// </summary>
        /// <param name="path">namespace of embedded resource (remember add folders' names)</param>
        /// <returns>string</returns>
        string ReadEmbedded(string path);

        /// <summary>
        /// Returns content of resource item as a string.
        /// </summary>
        /// <param name="path">namespace of resource (remember add folders' names)</param>
        /// <param name="key">key in resource</param>
        /// <returns>string</returns>
        string ReadResource(string path, string key);

        /// <summary>
        /// Returns content of file as a string.
        /// </summary>
        /// <param name="path">path to file</param>
        /// <returns>string</returns>
        string ReadFile(string path);
    }
}