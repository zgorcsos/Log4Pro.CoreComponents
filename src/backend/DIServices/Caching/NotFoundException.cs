using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vrh.DIServices.Caching
{
    /// <summary>
    /// Not found in cache Exception. 
	/// All provider implementations must throw this exception when the readed value not found in cache.
    /// </summary>
    public class NotFoundException : Exception
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="NotFoundException"/> class.
		/// </summary>
		/// <param name="dataId">The data identifier.</param>
		public NotFoundException(string dataId)
            : base($"This data is not found in cache: {dataId}")
        {
            DataId = dataId;
        }

		/// <summary>
		/// Gets the data identifier.
		/// </summary>
		/// <value>
		/// The data identifier.
		/// </value>
		public string DataId { get; private set; }
    }
}
