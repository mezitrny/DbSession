using System;

namespace DbSession.Parameters
{
    public interface IDbParameter
    {
        /// <summary>
        /// Parameter name - literal taking place after @, e.g. @Id
        /// </summary>
        string Name { get; }

        /// <summary>
        /// C# type of the parameter that will be translated into 
        /// appropriate SQL type by provider
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Parameter's value
        /// </summary>
        object Value { get; }
    }
}