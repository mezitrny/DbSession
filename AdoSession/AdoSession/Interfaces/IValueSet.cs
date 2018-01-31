using System.Collections.Generic;

namespace RoseByte.AdoSession.Interfaces
{
    public interface IValueSet
    {
        /// <summary>
        /// returns typed value of the particular column by it's name
        /// </summary>
        /// <typeparam name="T">desired type of value (value will be casted from object type to this type)</typeparam>
        /// <param name="name">column's name</param>
        /// <returns></returns>
        T Get<T>(string name);

        /// <summary>
        /// returns value of the particular column by it's name
        /// </summary>
        /// <param name="name">column's name</param>
        /// <returns></returns>
        object this[string name] { get; }

        /// <summary>
        /// Returns all fetched columns as columnName-columnValue pairs as a dictionary
        /// </summary>
        IReadOnlyDictionary<string, object> Values { get; }
    }
}