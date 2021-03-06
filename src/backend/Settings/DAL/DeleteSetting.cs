namespace Log4Pro.CoreComponents.Settings.DAL
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
        /// This was the original title of deleted setting
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// This was the value of deleted setting
        /// </summary>
        public string From { get; set; }        
    }
}
