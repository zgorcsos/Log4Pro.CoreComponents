using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Vrh.DIServices.Settings.DAL
{
    /// <summary>
    /// Egy beállítás értékének változása
    /// </summary>
	[Index(nameof(Changer))]
	[Index(nameof(TimeStamp))]
	[Index(nameof(Version))]
	[Index(nameof(ModuleKey))]
	[Index(nameof(InstanceOrUserKey))]
	[Index(nameof(SettingKey))]
	[Table(nameof(SettingContext.SettingHistories), Schema = SettingContext.DB_SCHEMA)]
	public abstract class SettingHistory
    {
		/// <summary>
		/// PK
		/// </summary>
		[Key]
        public int Id { get; set; }

		/// <summary>
		/// User friendly name of user or module, who/which changed this setting.
		/// </summary>
		[MaxLength(200)]
        public string Changer { get; set; }

        /// <summary>
        /// The timestamp of the setting change.
        /// </summary>
        public DateTime TimeStamp { get; set; }

		/// <summary>
		/// The module of this setting.
		/// </summary>
		[MaxLength(100)]
		public string ModuleKey { get; set; }

		/// <summary>
		/// Beállítás példány azonosító (pl. objektum instance-okhoz, vagy user level beállításokhoz való kötéshez)
		/// </summary>
		[MaxLength(100)]
		public string InstanceOrUserKey { get; set; }

		/// <summary>
		///  A beállítás azonosítója
		/// </summary>
		[Required]
		[MaxLength(1000)]
		public string SettingKey { get; set; }

		/// <summary>
		/// The version of setting.
		/// </summary>
		[MaxLength(100)]
		public string Version { get; set; }

		/// <summary>
		/// Gets or sets the type of the operation.
		/// TPH discriminator column.
		/// </summary>
		/// <value>
		/// The type of the operation.
		/// </value>
		public OperationType OperationType { get; set; }
	}
}
