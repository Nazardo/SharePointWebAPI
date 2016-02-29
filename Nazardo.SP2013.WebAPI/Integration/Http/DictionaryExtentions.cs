using System;
using System.Collections.Generic;

namespace Nazardo.SP2013.WebAPI.Integration.Http
{
    internal static class DictionaryExtensions
    {
        internal static IEnumerable<KeyValuePair<string, TValue>> FindKeysWithPrefix<TValue>(this IDictionary<string, TValue> dictionary, string prefix)
        {
            TValue tValue;
            if (dictionary.TryGetValue(prefix, out tValue))
            {
                yield return new KeyValuePair<string, TValue>(prefix, tValue);
            }
            foreach (KeyValuePair<string, TValue> keyValuePair in dictionary)
            {
                string str = keyValuePair.Key;
                if (str.Length <= prefix.Length || !str.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if (prefix.Length != 0)
                {
                    char chr = str[prefix.Length];
                    if (chr != '.' && chr != '[')
                    {
                        continue;
                    }
                    yield return keyValuePair;
                }
                else
                {
                    yield return keyValuePair;
                }
            }
        }

        public static void RemoveFromDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<KeyValuePair<TKey, TValue>, bool> removeCondition)
        {
            dictionary.RemoveFromDictionary<TKey, TValue, Func<KeyValuePair<TKey, TValue>, bool>>((KeyValuePair<TKey, TValue> entry, Func<KeyValuePair<TKey, TValue>, bool> innerCondition) => innerCondition(entry), removeCondition);
        }

        public static void RemoveFromDictionary<TKey, TValue, TState>(this IDictionary<TKey, TValue> dictionary, Func<KeyValuePair<TKey, TValue>, TState, bool> removeCondition, TState state)
        {
            int num = 0;
            TKey[] key = new TKey[dictionary.Count];
            foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
            {
                if (!removeCondition(keyValuePair, state))
                {
                    continue;
                }
                key[num] = keyValuePair.Key;
                num++;
            }
            for (int i = 0; i < num; i++)
            {
                dictionary.Remove(key[i]);
            }
        }

        public static bool TryGetValue<T>(this IDictionary<string, object> collection, string key, out T value)
        {
            object obj;
            if (!collection.TryGetValue(key, out obj) || !(obj is T))
            {
                value = default(T);
                return false;
            }
            value = (T)obj;
            return true;
        }
    }

}
