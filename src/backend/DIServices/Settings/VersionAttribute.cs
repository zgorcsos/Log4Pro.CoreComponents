using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.DIServices.Settings
{
	/// <summary>
	/// Setting version
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
	public class VersionAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VersionAttribute"/> class.
		/// </summary>
		/// <param name="versionValue">The version value.</param>
		public VersionAttribute(string versionValue)
		{
			Version = versionValue;
		}

		/// <summary>
		/// Gets the version.
		/// </summary>
		/// <value>
		/// The version.
		/// </value>
		public string Version { get; private set; }
	}

}
