using System.Collections.Generic;

namespace Gamegaard.RuntimeDebug
{
    public class EnumerableParser<T> : ITypeParser<IEnumerable<T>>
    {
        public IEnumerable<T> Parse(string value)
        {
            return new CollectionParser<T>().Parse(value);
        }
    }
}