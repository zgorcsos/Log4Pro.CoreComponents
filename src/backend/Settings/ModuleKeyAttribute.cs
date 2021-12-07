using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.Settings
{
	/// <summary>
	/// Attribute to specify a modul id for an setting class
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ModuleKeyAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleKeyAttribute"/> class.
		/// </summary>
		/// <param name="moduleKey">The module key.</param>
		public ModuleKeyAttribute(string moduleKey)
		{
			ModuleKey = moduleKey;
		}

		/// <summary>
		/// Gets the module key.
		/// </summary>
		/// <value>
		/// The module key.
		/// </value>
		public string ModuleKey { get; private set; }
	}
}
