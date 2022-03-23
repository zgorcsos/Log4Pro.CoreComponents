using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.Settings
{
	/// <summary>
	/// Attribute to define a description
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
	public class DescriptionAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DescriptionAttribute"/> class.
		/// </summary>
		/// <param name="descriptionValue">The description value.</param>
		public DescriptionAttribute(string descriptionValue)
		{
			Description = descriptionValue;
		}

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		public string Description { get; private set; }
	}
}
