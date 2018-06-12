using System.Collections.Generic;
using RoseByte.AdoSession.Interfaces;

namespace RoseByte.AdoSession
{
    public class ParameterSet : HashSet<IParameter>
    {
        public ParameterSet(){ }
        
        public ParameterSet(IEnumerable<IParameter> parameters)
        {
            foreach (var parameter in parameters)
            {
                Add(parameter);
            }
        }
    }
}