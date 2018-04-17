using System;
using System.Collections.Generic;

namespace RoseByte.AdoSession.Interfaces
{
    public interface ISqlServerSession : ISession
    {
        /// <summary>
        /// Executes given stored procedure
        /// </summary>
        /// <param name="name">Stored procedure name</param>
        /// <param name="parameters">parameters for stored procedure</param>
        void ExecuteProcedure(string name, ParameterSet parameters = null);

        /// <summary>
        /// Executes given stored procedure with multiple parameter sets as a batch
        /// </summary>
        /// <param name="name">Stored procedure name</param>
        /// <param name="parameterSets">parameters for stored procedure</param>
        void ExecuteProcedureBatch(string name, IEnumerable<ParameterSet> parameterSets);

        /// <summary>
        /// Executes given stored procedure on transaction. Transaction is created by first call 
        /// and disposed by either Commit() or Rollback() command.
        /// </summary>
        /// <param name="name">Stored procedure name</param>
        /// <param name="parameters">parameters for stored procedure</param>
        void ExecuteProcedureOnTransaction(string name, ParameterSet parameters = null);

        /// <summary>
        /// Executes given stored procedure on transaction with multiple parameter sets as a batch.
        /// Transaction is created by first call and disposed by either Commit() or Rollback() command.
        /// </summary>
        /// <param name="name">Stored procedure name</param>
        /// <param name="parameterSets">parameters for stored procedure</param>
        void ExecuteProcedureBatchOnTransaction(string name, IEnumerable<ParameterSet> parameterSets);
    }
}