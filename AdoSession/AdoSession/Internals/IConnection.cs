using System;
using System.Collections.Generic;
using System.Data;
using RoseByte.AdoSession.Interfaces;

namespace RoseByte.AdoSession.Internals
{
    internal interface IConnection : IDisposable
    {
        IEnumerable<IValueSet> Select(string sql, ParameterSet parameters = null);
        void Execute(string sql, ParameterSet parameters = null, CommandType commandType = CommandType.Text);
        void ExecuteBatch(string sql, IEnumerable<ParameterSet> parameterSets, CommandType commandType = CommandType.Text);
        void ExecuteOnTransaction(string sql, ParameterSet parameters = null, CommandType commandType = CommandType.Text);
        void ExecuteBatchOnTransaction(string sql, IEnumerable<ParameterSet> parameterSets, CommandType commandType = CommandType.Text);
        object GetScalar(string sql, ParameterSet parameters = null);
        void Commit();
        void RollBack();
    }
}