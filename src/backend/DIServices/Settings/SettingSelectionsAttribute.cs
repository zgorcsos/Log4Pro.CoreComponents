using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.DIServices.Settings
{
	/// <summary>
	/// Mark an Enum as source of an options-type setting
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsage(AttributeTargets.Class)]
	public class SettingSelectionsAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SettingSelectionsAttribute"/> class.
		/// </summary>
		/// <param name="enumType">Type of the enum.</param>
		public SettingSelectionsAttribute(Type enumType)
		{
			this.enumType = enumType;
		}

		/// <summary>
		/// Gets the setting selections.
		/// </summary>
		/// <value>
		/// The setting selections.
		/// </value>
		public List<SettingSelection> SettingSelections
		{
			get
			{
				bool defaultSetted = false;
				var selections = new List<SettingSelection>();
				FieldInfo[] fields = enumType.GetFields();
				foreach (var field in fields)
				{
					if (field.Name.Equals("value__"))
					{
						continue;
					}
					var descriptionAttribute = field.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
					var settingSelection = new SettingSelection()
					{
						Value = field.Name,
						Description = descriptionAttribute != null ? descriptionAttribute.Description : string.Empty,
						IsDefault = false,
					};
					if (!defaultSetted)
					{
						settingSelection.IsDefault = field.GetCustomAttributes<IsDefaultAttribute>().FirstOrDefault() != null;
						defaultSetted = settingSelection.IsDefault;
					}
					selections.Add(settingSelection);
				}
				return selections;
			}
		}

		private readonly Type enumType;
	}
}
