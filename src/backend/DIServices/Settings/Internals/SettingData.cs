using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.DIServices.Settings.Internals
{
	/// <summary>
	/// Beállítás adatiat reprezentáló osztály
	/// </summary>
	internal class SettingData
	{
		/// <summary>
		/// Beállítás címzése
		/// </summary>
		public SettingAddress SettingAddress { get; set; }

		/// <summary>
		/// Leírás a beállításhoz
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alapértelmezett érték
		/// </summary>
		public string DefaultValue { get; set; }

		/// <summary>
		/// Ha a beáálítás diszkrét értékeket vehet fel, akkor ez képviseli a lehetséges értékek választéklistáját 
		/// </summary>
		public List<SettingSelection> SettingSelections { get; set; }

		/// <summary>
		/// The version of setting.
		/// </summary>
		public string Version { get; set; }
	}
}
