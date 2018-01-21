using System;
using System.Collections.Generic;
using DbSession.Parameters;
using DbSession.ValueSets;

namespace DbSession.Connections
{
    internal interface IConnection : IDisposable
    {
        IEnumerable<ValueSet> Select(string sql, DbParameterSet parameters = null);
        void Execute(string sql, DbParameterSet parameters = null);
        void ExecuteBatch(string sql, IEnumerable<DbParameterSet> parameterSets);
        void ExecuteOnTransaction(string sql, DbParameterSet parameters = null);
        void ExecuteBatchOnTransaction(string sql, IEnumerable<DbParameterSet> parameterSets);
        object GetScalar(string sql, DbParameterSet parameters = null);
        void Commit();
        void RollBack();
    }
}