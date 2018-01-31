using System;
using RoseByte.AdoSession.Interfaces;

namespace RoseByte.AdoSession
{
    public class Parameter<T> : IParameter
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

        /// <param name="name">Parameter name - literal taking place after @, e.g. @Id</param>
        /// <param name="value">Parameter's value</param>
        public Parameter(string name, object value)
        {
            Name = name;
            Type = typeof(T);
            Value = value;
        }
    }

    public class Parameter : IParameter
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

        /// <param name="name">Parameter name - literal taking place after @, e.g. @Id</param>
        /// <param name="type">C# type of the parameter that will be translated into appropriate SQL type by provider</param>
        /// <param name="value">Parameter's value</param>
        public Parameter(string name, Type type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}