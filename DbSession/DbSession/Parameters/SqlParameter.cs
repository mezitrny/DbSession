using System;

namespace DbSession.Parameters
{
    public class SqlParameter<T> : ISqlParameter
    {
        /// <summary>
        /// Parameter name - literal taking place after @, e.g. @Id
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// C# type of the parameter that will be translated into 
        /// appropriate SQL type by provider
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Parameter's value
        /// </summary>
        public object Value { get; }

        public SqlParameter(string name, object value)
        {
            Name = name;
            Type = typeof(T);
            Value = value;
        }
    }
}