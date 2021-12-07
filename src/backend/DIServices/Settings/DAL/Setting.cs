using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Log4Pro.CoreComponents.DIServices.Settings.DAL
{
	/// <summary>
	/// Represent a setting
	/// </summary>
	[Index(nameof(ModuleKey))]
	[Index(nameof(InstanceOrUserKey))]
	[Index(nameof(Key))]
	[Index(nameof(Version))]
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

        /// <summary>
        /// Current value of the setting as JSON serialized object.
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Default value of the setting as JSON serialized object. (Defined by code.) 
        /// </summary>
        [Required]
        public string DefaultValue { get; set; }

        /// <summary>
        /// The posible options, as JSON serialized array object, if posible value of this setting is finite discrate values.
        /// </summary>
        public string Options { get; set; }

		/// <summary>
		/// The version of setting.
		/// </summary>
		[MaxLength(100)]
		public string Version { get; set; }
    }
}
