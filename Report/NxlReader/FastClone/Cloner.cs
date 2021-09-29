using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NxlReader.FastClone.Internal;

namespace NxlReader.FastClone
{
    public static class Cloner
    {
        static readonly ConcurrentDictionary<Type, Func<object, Dictionary<object, object>, object>> TypeCloners =
            new();

        /// <summary>
        /// Creates a deep clone.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T Clone<T>(T original)
        {
            Func<object, Dictionary<object, object>, object> creator = GetTypeCloner(typeof(T));
            return (T)creator(original, new Dictionary<object, object>());
        }

        static Func<object, Dictionary<object, object>, object> GetTypeCloner(Type type)
        {
            return TypeCloners.GetOrAdd(type, t => new CloneExpressionBuilder(t).CreateTypeCloner());
        }
    }
}