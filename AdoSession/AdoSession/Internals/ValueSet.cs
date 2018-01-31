using System;
using System.Collections.Generic;
using System.Data;
using RoseByte.AdoSession.Interfaces;

namespace RoseByte.AdoSession.Internals
{
    public class ValueSet : IValueSet
    {
        private readonly Dictionary<string, object> _values;

        internal ValueSet(IDataRecord reader)
        {
            _values = new Dictionary<string, object>();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                _values.Add(reader.GetName(i), reader[i]);
            }
        }

        internal ValueSet(Dictionary<string, object> values)
        {
            _values = values;
        }

        public T Get<T>(string name)
        {
            return (T)Convert.ChangeType(_values[name], typeof(T));
        }

        public object this[string name] => _values[name];

        public IReadOnlyDictionary<string, object> Values => _values;
    }
}