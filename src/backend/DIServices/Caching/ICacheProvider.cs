using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vrh.DIServices.Caching
{
    /// <summary>
    /// Defines a cache provider interface
    /// </summary>
    public interface ICacheProvider
    {
		/// <summary>
		/// Publishes a value into the cache under specified data identifier.
		/// </summary>
		/// <typeparam name="TStoredObject">The type of the stored object.</typeparam>
		/// <param name="dataId">The data identifier.</param>
		/// <param name="value">The value.</param>
		void Publish<TStoredObject>(string dataId, TStoredObject value);

		/// <summary>
		/// Reads a stored data from cache
		/// </summary>
		/// <typeparam name="TStoredObject">The type of the stored oject.</typeparam>
		/// <param name="dataId">The data identifier.</param>
		/// <exception cref="NotFoundException"></exception>
		/// <returns>The stored object from cache. (Or NotFoundException, if not found.)</returns>
		TStoredObject Read<TStoredObject>(string dataId);

		/// <summary>
		/// Removes the specified data from cache.
		/// </summary>
		/// <param name="dataId">The data identifier.</param>
		void Remove(string dataId);
    }
}
