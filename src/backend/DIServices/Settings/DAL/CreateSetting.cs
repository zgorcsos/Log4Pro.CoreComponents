namespace Log4Pro.CoreComponents.DIServices.Settings.DAL
{
	/// <summary>
	/// Represents a setting creation
	/// </summary>
	/// <seealso cref="SettingHistory" />
	public class CreateSetting : SettingHistory
    {
		/// <summary>
		/// Created with this value
		/// </summary>
		public string To { get; set; }
    }
}
