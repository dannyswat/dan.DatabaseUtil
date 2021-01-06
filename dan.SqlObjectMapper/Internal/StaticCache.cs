using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlObjectMapper.Internal
{
	/// <summary>
	/// Utility class for static cache
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	internal class StaticCache<TKey, TValue> where TValue : class
	{
		Dictionary<TKey, TValue> cache = new Dictionary<TKey, TValue>();

		public TValue GetOrAdd(TKey key)
		{
			return Get(key) ?? AddOrUpdate(key);
		}

		public TValue Get(TKey key)
		{
			if (cache.ContainsKey(key))
				return cache[key];
			else
				return default;
		}

		public TValue AddOrUpdate(TKey key)
		{
			if (cache.ContainsKey(key))
				cache[key] = CreateInstance(key);
			else
				cache.Add(key, CreateInstance(key));

			return cache[key];
		}

		public void Remove(TKey key)
		{
			if (cache.ContainsKey(key))
				cache.Remove(key);
		}

		protected virtual TValue CreateInstance(TKey key)
		{
			return (TValue)Activator.CreateInstance(typeof(TValue), key);
		}
	}
}
