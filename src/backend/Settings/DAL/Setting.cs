using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Log4Pro.CoreComponents.Settings.DAL
{
	/// <summary>
	/// Represent a setting
	/// </summary>
	[Index(nameof(ModuleKey))]
	[Index(nameof(InstanceOrUserKey))]
	[Index(nameof(Key))]
	[Index(nameof(Version))]
    [Index(nameof(UserLevelSettings))]
	[Index(nameof(ModuleKey), nameof(InstanceOrUserKey), nameof(Key), IsUnique = true)]
	[Table(nameof(SettingContext.Settings), Schema = SettingContext.DB_SCHEMA)]
    public class Setting
    {
        /// <summary>
        /// PK
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The module of this setting.
        /// </summary>
        [MaxLength(100)]
        public string ModuleKey { get; set; }

        /// <summary>
        /// The instance or user identifier of setting. (Use for instance or user levels sezttings.)
        /// </summary>
        [MaxLength(100)]
        public string InstanceOrUserKey { get; set; }

        /// <summary>
        ///  Identifier key of this setting.
        /// </summary>
        [Required]
        [MaxLength(1000)]
		public string Key { get; set; }

        /// <summary>
        /// The informative description of the setting.
        /// </summary>
        public string Description { get; set; }

        [MaxLength(1000)]
        public string Title { get; set; }

        /// <summary>
        /// Current value of the setting as JSON serialized object.
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [user level settings].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [user level settings]; otherwise, <c>false</c>.
        /// </value>        
        public bool UserLevelSettings { get; set; }

        /// <summary>
        /// The version of setting.
        /// </summary>
        [MaxLength(100)]
		public string Version { get; set; }
    }
}
