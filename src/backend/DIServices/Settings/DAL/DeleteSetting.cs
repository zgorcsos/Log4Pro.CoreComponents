namespace Vrh.DIServices.Settings.DAL
{
	/// <summary>
	/// Represents a setting deletion
	/// </summary>
	/// <seealso cref="SettingHistory" />
	public class DeleteSetting : SettingHistory
    {
        /// <summary>
        /// This was the original description of deleted setting
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// This was the value of deleted setting
        /// </summary>
        public string From { get; set; }        
    }
}
