using System;
using System.Collections.Generic;

namespace DbAnonymizer
{
    public static class Extensions
    {
        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<TKey, TValue> action)
        {
            foreach (var pair in dict)
            {
                action(pair.Key, pair.Value);
            }
        }
    }
}