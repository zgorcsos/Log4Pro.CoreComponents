using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.Settings
{
	/// <summary>
	/// Not found in setting database Exception. 
	/// </summary>
	/// <seealso cref="System.Exception" />
	public class NotFoundException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NotFoundException"/> class.
		/// </summary>
		/// <param name="dataId">The data identifier.</param>
		public NotFoundException(string dataId)
			: base($"This data is not found in setting database: {dataId}")
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
