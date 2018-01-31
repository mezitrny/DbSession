using System;
using System.Collections.Generic;
using RoseByte.AdoSession.Interfaces;

namespace RoseByte.AdoSession.Internals
{
    internal interface IConnection : IDisposable
    {
        IEnumerable<IValueSet> Select(string sql, ParameterSet parameters = null);
        void Execute(string sql, ParameterSet parameters = null);
        void ExecuteBatch(string sql, IEnumerable<ParameterSet> parameterSets);
        void ExecuteOnTransaction(string sql, ParameterSet parameters = null);
        void ExecuteBatchOnTransaction(string sql, IEnumerable<ParameterSet> parameterSets);
        object GetScalar(string sql, ParameterSet parameters = null);
        void Commit();
        void RollBack();
    }
}