using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.DIServices.Settings
{
	/// <summary>
	/// Full representation of a setting
	/// </summary>
	public class SettingRepresentation
	{
		/// <summary>
		/// The database identifier in background permanent storage database.
		/// </summary>
		public int DbId { get; set; }

		/// <summary>
		/// This setting is hosted in this module.
		/// </summary>
		public string HosterModule { get; set; }

		/// <summary>
		/// The instance or user identifier which belongs this setting.
		/// </summary>
		public string HosterInstance { get; set; }

		/// <summary>
		/// The setting module which define this setting.
		/// </summary>
		public string SettingModule { get; set; }

		/// <summary>
		/// The setting instance name.
		/// </summary>
		public string SettingInstance { get; set; }

		/// <summary>
		/// The settig unique key identifier 
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// The description of setting.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// The original description of setting which defined by developer in setting declaration type.
		/// </summary>
		public string DescriptionDefault { get; set; }

		/// <summary>
		/// The database side value.
		/// </summary>
		public string DbSideValue { get; set; }

		/// <summary>
		/// The cache side value.
		/// </summary>
		public string CacheSideValue { get; set; }

		/// <summary>
		/// The default value for this setting defined by developer in setting declaration type.
		/// </summary>
		public string DefaultValue { get; set; }

		/// <summary>
		/// Gets or sets the type of the data.
		/// </summary>
		/// <value>
		/// The type of the data.
		/// </value>
		public DataType DataType { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [pretty format].
		/// </summary>
		/// <value>
		///   <c>true</c> if [pretty format]; otherwise, <c>false</c>.
		/// </value>
		public bool PrettyFormat { get; set; }

		/// <summary>
		/// The parent setting (for special setting tree structure)
		/// </summary>
		public string ParentSetting { get; set; } = null;

		/// <summary>
		/// The depends (for special setting tree structure)
		/// </summary>
		public List<string> Depends { get; set; } = new List<string>();

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SettingRepresentation"/> is disabled.
		/// (for special setting tree structure only: disabled 'cause the parent boolean type declarator type current value (in permanent storage) is false)
		/// </summary>
		/// <value>
		///   <c>true</c> if disabled; otherwise, <c>false</c>.
		/// </value>
		public bool Disabled { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [sensitive data].
		/// </summary>
		/// <value>
		///   <c>true</c> if [sensitive data]; otherwise, <c>false</c>.
		/// </value>
		public bool SensitiveData { get; set; }
	}

	/// <summary>
	/// Posible data types (for dashboard UI)
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum DataType
	{
		/// <summary>
		/// The numeric (represent an input in UI side)
		/// </summary>
		Numeric,
		/// <summary>
		/// The boolean (represent a checkbox in UI side)
		/// </summary>
		Boolean,
		/// <summary>
		/// The string (represent an input in UI side)
		/// </summary>
		String,
		/// <summary>
		/// The complex (represent a textarea with json string in UI side)
		/// </summary>
		Complex,
	}
}
