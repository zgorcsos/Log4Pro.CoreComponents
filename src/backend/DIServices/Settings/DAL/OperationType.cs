using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.DIServices.Settings.DAL
{
	/// <summary>
	/// The type of operation
	/// </summary>
	public enum OperationType
	{
		/// <summary>
		/// Create a setting
		/// </summary>
		Create,
		/// <summary>
		/// Modify a value of setting
		/// </summary>
		Modify,
		/// <summary>
		/// Delete a setting
		/// </summary>
		Delete,
	}
}
