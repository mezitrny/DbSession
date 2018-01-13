using System;
using System.Collections.Generic;

namespace DbSession.Core
{
    public interface ISession : IDisposable
    {
        IEnumerable<ValueSet> Select(string sql, SqlParameterSet parameters = null);
        void Execute(string sql, SqlParameterSet parameters = null);
        void ExecuteOnTransaction(string sql, SqlParameterSet parameters = null);
        object GetScalar(string sql, SqlParameterSet parameters = null);
        T GetScalar<T>(string sql, SqlParameterSet parameters = null);
        void Commit();
        void RollBack();
        void CloseConnection();
    }
}