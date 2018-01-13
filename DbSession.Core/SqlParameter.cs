using System;

namespace DbSession.Core
{
    public class SqlParameter<T> : ISqlParameter
    {
        public string Name { get; }
        public Type Type { get; }
        public object Value { get; }

        public SqlParameter(string name, object value)
        {
            Name = name;
            Type = typeof(T);
            Value = value;
        }
    }
}