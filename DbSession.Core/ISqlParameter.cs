using System;

namespace DbSession.Core
{
    public interface ISqlParameter
    {
        string Name { get; }
        Type Type { get; }
        object Value { get; }
    }
}