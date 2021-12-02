using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vrh.DIServices.Settings.Internals
{
	internal class SettingAddress
	{
		/// <summary>
		/// Id of module to which this setting
		/// </summary>
		public string ModuleKey { get; set; }

		/// <summary>
		/// Id of instance (or user) to which this setting 
		/// </summary>
		public string InstanceOrUserKey { get; set; }

		/// <summary>
		/// Id of this setting
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Converts to string.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(ModuleKey);
			if (!string.IsNullOrEmpty(InstanceOrUserKey))
			{
				sb.Append($":{InstanceOrUserKey}");
			}
			sb.Append($":{Key}");
			return sb.ToString();
		}
	}
}
