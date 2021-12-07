using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.Settings
{
	/// <summary>
	/// Atribute to mark an enum value as default.
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsage(AttributeTargets.Field)]
	public class IsDefaultAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IsDefaultAttribute"/> class.
		/// </summary>
		public IsDefaultAttribute()
		{
		}
	}
}
