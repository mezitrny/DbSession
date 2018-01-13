using System.Collections.Generic;

namespace DbSession.Core
{
    public interface IValueSet
    {
        T Get<T>(string name);
        object this[string name] { get; }
        IReadOnlyDictionary<string, object> Values { get; }
    }
}