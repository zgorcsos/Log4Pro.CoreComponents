using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.DIServices.Settings
{
	/// <summary>
	/// Attribute to define a default value a setting.
	///  (Compile time constant only! To more complexity use the GetDefaultValue metod!!!)
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DefaultValueAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultValueAttribute"/> class.
		/// </summary>
		/// <param name="defaultValue">The default value.</param>
		public DefaultValueAttribute(object defaultValue)
		{
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Gets the default value.
		/// </summary>
		/// <value>
		/// The default value.
		/// </value>
		public object DefaultValue { get; private set; }
	}
}
