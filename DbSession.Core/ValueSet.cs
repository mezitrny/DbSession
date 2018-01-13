using System;
using System.Collections.Generic;
using System.Data;

namespace DbSession.Core
{
    public class ValueSet : IValueSet
    {
        private readonly Dictionary<string, object> _values;

        public ValueSet(IDataRecord reader)
        {
            _values = new Dictionary<string, object>();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                _values.Add(reader.GetName(i), reader[i]);
            }
        }

        public ValueSet(Dictionary<string, object> values)
        {
            _values = values;
        }

        public TT Get<TT>(string name)
        {
            return (TT)Convert.ChangeType(_values[name], typeof(TT));
        }

        public object this[string name] => _values[name];
        public IReadOnlyDictionary<string, object> Values => _values;
    }
}