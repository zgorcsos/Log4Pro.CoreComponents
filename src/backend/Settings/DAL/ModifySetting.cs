using System.ComponentModel.DataAnnotations.Schema;

namespace Log4Pro.CoreComponents.Settings.DAL
{
	/// <summary>
	/// Represents a setting modification
	/// </summary>
	/// <seealso cref="SettingHistory" />
	public class ModifySetting : SettingHistory
    {
        /// <summary>
        /// This was the original value of setting
        /// </summary>
        public string From { get; set; }

		/// <summary>
		/// This became the new value of setting
		/// </summary>
		public string To { get; set; }
    }
}
