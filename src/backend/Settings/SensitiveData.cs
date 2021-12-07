using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.Settings
{
	/// <summary>
	/// Indicates that this is sensitive data
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsage(AttributeTargets.Class)]
	public class SensitiveData : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SensitiveData"/> class.
		/// </summary>
		public SensitiveData()
		{
		}
	}
}
