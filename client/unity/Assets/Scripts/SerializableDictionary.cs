using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KG
{
    [Serializable]
    public class SerializableDictionary<K, V> : Dictionary<K,V>, ISerializationCallbackReceiver
    {
        [Serializable]
        public struct SerializableKeyValuePair
        {
            [SerializeField]
            public K _key;
            [SerializeField]
            public V _value;

            public SerializableKeyValuePair(K key, V value)
            {
                _key = key;
                _value = value;
            }
        }
        
        [SerializeField]
        private List<SerializableKeyValuePair> _pairs = new List<SerializableKeyValuePair>();

        public void OnBeforeSerialize()
        {
            _pairs.Clear();

            foreach (KeyValuePair<K, V> pair in this)
            {
                _pairs.Add(new SerializableKeyValuePair(pair.Key, pair.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();
            for (int i = 0, icount = _pairs.Count; i < icount; ++i)
            {
                this[_pairs[i]._key] = _pairs[i]._value;
            }
        }
    }

    public static class UnityDictionaryHelper
    {
        public static SerializableDictionary<TKey, TElement> ToUnityDictionary<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            var dict = new SerializableDictionary<TKey, TElement>();
            var enumer = source.GetEnumerator();
            while (enumer.MoveNext())
            {
                dict.Add(keySelector.Invoke(enumer.Current), elementSelector.Invoke(enumer.Current));
            }
            enumer.Dispose();

            return dict;
        }
    }
}