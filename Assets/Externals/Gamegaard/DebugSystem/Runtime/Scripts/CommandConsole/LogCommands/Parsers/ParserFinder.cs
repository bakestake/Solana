using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Gamegaard.RuntimeDebug
{
    public static class ParserFinder
    {
        private static readonly ConcurrentDictionary<Type, object> _parserCache = new ConcurrentDictionary<Type, object>();

        static ParserFinder()
        {
            InitializeParsers();
        }

        private static void InitializeParsers()
        {
            var parserTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITypeParser<>)) && !t.IsInterface && !t.IsAbstract)
                .ToList();

            foreach (var type in parserTypes)
            {
                var interfaceType = type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITypeParser<>));

                if (interfaceType != null)
                {
                    var genericType = interfaceType.GetGenericArguments()[0];
                    var parserInstance = Activator.CreateInstance(type);
                    _parserCache.TryAdd(genericType, parserInstance);
                }
            }
        }

        public static ITypeParser<T> GetParser<T>()
        {
            if (_parserCache.TryGetValue(typeof(T), out var parser))
            {
                return (ITypeParser<T>)parser;
            }

            return null;
        }
    }
}