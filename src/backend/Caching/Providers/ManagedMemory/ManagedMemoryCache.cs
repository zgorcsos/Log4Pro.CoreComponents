using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Log4Pro.CoreComponents.Caching.Providers.ManagedMemory
{
	/// <summary>
	/// A cache provider that stores the cache values in a high-speed acces, concurrent available storage structure in clr-managed memory.
	/// </summary>
	/// <seealso cref="ICacheProvider" />
	public class ManagedMemoryCache : ICacheProvider
	{
		/// <inheritdoc/>
		public void Publish<TStoredObject>(string dataId, TStoredObject value)
		{
			_cache[dataId] = value;
		}

		/// <inheritdoc/>
		public TStoredObject Read<TStoredObject>(string dataId)
		{
			if (!_cache.TryGetValue(dataId, out var value))
			{
				throw new NotFoundException(dataId);
			}
			return (TStoredObject)value;
		}

		/// <inheritdoc/>
		public void Remove(string dataId)
		{
			_cache.Remove(dataId, out var _);
		}

		private readonly ConcurrentDictionary<string, object> _cache = new();
	}
}
