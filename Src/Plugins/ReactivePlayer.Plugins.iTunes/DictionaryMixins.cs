using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core
{
    public static class DictionaryMixins
    {
        public static TValue GetKeyValueIfExists<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            dictionary.TryGetValue(key, out TValue value);
            return value;
        }

        public static TRet GetValue<TKey, TValue, TRet>(this IDictionary<TKey, TValue> dictionary, TKey key, TRet fallbackValue)
        {
            try
            {
                TValue value = dictionary.GetKeyValueIfExists(key);
                return value != null ? (TRet)Convert.ChangeType(value, typeof(TRet)) : fallbackValue;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static TRet GetNullableValue<TKey, TValue, TRet>(this IDictionary<TKey, TValue> dictionary, TKey key, TRet fallbackValue)
        {
            try
            {
                TValue value = dictionary.GetKeyValueIfExists(key);
                var type = Nullable.GetUnderlyingType(typeof(TRet));
                return value != null ? (TRet)Convert.ChangeType(value, type) : fallbackValue;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}