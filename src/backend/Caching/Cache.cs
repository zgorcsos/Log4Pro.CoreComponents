using System;
using Newtonsoft.Json;

namespace Log4Pro.CoreComponents.Caching
{
	/// <summary>
	/// Object cache implementation
	/// </summary>
	public class Cache
    {
		// Injected parts
		private readonly ICacheProvider _provider;

		/// <summary>
		/// Initializes a new instance of the <see cref="Cache"/> class.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public Cache(ICacheProvider provider)
        {
			_provider = provider;
        }

		/// <summary>
		/// Publishes an object (value) into cache under the specified data identifier.
		/// </summary>
		/// <typeparam name="TStoredObject">The type of the stored object.</typeparam>
		/// <param name="dataId">The data identifier.</param>
		/// <param name="value">The value.</param>
		public void Publish<TStoredObject>(string dataId, TStoredObject value)
        {
            _provider.Publish(dataId, value);
        }

		/// <summary>
		/// Reads the specified value (by data identifier) from cache.
		/// </summary>
		/// <typeparam name="TStoredObject">The type of the stored object.</typeparam>
		/// <param name="dataId">The data identifier.</param>
		/// <exception cref="NotFoundException"></exception>
		/// <returns></returns>
		public TStoredObject Read<TStoredObject>(string dataId)
        {
            try
            {
                return _provider.Read<TStoredObject>(dataId);
            }
            catch(NotFoundException)
            {
                if (AutoPublisher != null)
                {
                    var autoValue = AutoPublisher.Invoke(dataId);                    
                    Publish(dataId, (TStoredObject)autoValue);
                    return _provider.Read<TStoredObject>(dataId);
                }
                else
                {
					throw;
				}
            }
        }

		/// <summary>
		/// Removes an object (value) from cache under the specified data identifier.
		/// </summary>
		/// <param name="dataId">The data identifier.</param>
		public void RemoveFromCache(string dataId)
        {
            _provider.Remove(dataId);
        }

		/// <summary>
		/// The automatic publisher action.
		/// You can set an "AutoPublisher" action. This action able to gives back the value based on data-identifier. 
		/// When you set this no more necesary preload the cache value by call "Publish" method. 
		/// Simply set this property and implement an working publisher action logic. 
		/// When you call the "Read" method the first time, the "AutoPublisher" load the value by this data-identifier into cache.
		/// </summary>
		public Func<string, object> AutoPublisher { get; set; }
    }
}
