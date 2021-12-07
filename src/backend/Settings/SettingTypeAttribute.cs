using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.Settings
{
	/// <summary>
	/// Type of setting
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsage(AttributeTargets.Class)]
	public class SettingTypeAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SettingTypeAttribute"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		public SettingTypeAttribute(Type type)
		{
			Type = type;
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		public Type Type { get; private set; }
	}
}
