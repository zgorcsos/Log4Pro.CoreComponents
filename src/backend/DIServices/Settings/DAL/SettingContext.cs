using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using Log4Pro.CoreComponents.DIServices.Settings.Internals;

namespace Log4Pro.CoreComponents.DIServices.Settings.DAL
{
	/// <summary>
	/// Database context of Settings module.
	/// </summary>
	/// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
	public class SettingContext : DbContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SettingContext"/> class.
		/// </summary>
		/// <param name="options">The options for this context.</param>
		public SettingContext(
			DbContextOptions options) : base(options)
		{
			Database.Migrate();
		}

		/// <summary>
		/// Override this method to further configure the model that was discovered by convention from the entity types
		/// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
		/// and re-used for subsequent instances of your derived context.
		/// </summary>
		/// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
		/// define extension methods on this object that allow you to configure aspects of the model that are specific
		/// to a given database.</param>
		/// <remarks>
		/// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
		/// then this method will not be run.
		/// </remarks>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
			{
				relationship.DeleteBehavior = DeleteBehavior.Restrict;
			}
			modelBuilder.Entity<CreateSetting>()
						.Property(x => x.To)
						.HasColumnName(nameof(CreateSetting.To));
			modelBuilder.Entity<ModifySetting>()
						.Property(x => x.To)
						.HasColumnName(nameof(ModifySetting.To));
			modelBuilder.Entity<ModifySetting>()
						.Property(x => x.From)
						.HasColumnName(nameof(ModifySetting.From));
			modelBuilder.Entity<DeleteSetting>()
						.Property(x => x.From)
						.HasColumnName(nameof(DeleteSetting.From));
			modelBuilder.Entity<SettingHistory>()
					.HasDiscriminator<OperationType>(x => x.OperationType)
					.HasValue<CreateSetting>(OperationType.Create)
					.HasValue<ModifySetting>(OperationType.Modify)
					.HasValue<DeleteSetting>(OperationType.Delete);
			modelBuilder.Entity<SettingHistory>()
				.Property(x => x.OperationType)
				.HasColumnName(nameof(SettingHistory.OperationType));
			base.OnModelCreating(modelBuilder);
		}

		/// <summary>
		/// Settings
		/// </summary>
		public DbSet<Setting> Settings { get; set; }

		/// <summary>
		/// Collection of all events which created a setting.
		/// </summary>
		public DbSet<CreateSetting> Creates { get; set; }

		/// <summary>
		/// Collection of all events which modified a setting.
		/// </summary>
		public DbSet<ModifySetting> Modifies { get; set; }

		/// <summary>
		/// Collection of all events which deleted a setting.
		/// </summary>
		public DbSet<DeleteSetting> Deletes { get; set; }

		/// <summary>
		/// Collection of all events which manipulated the settings.
		/// </summary>
		public DbSet<SettingHistory> SettingHistories { get; set; }

		/// <summary>
		/// Storage database schema of the settings.
		/// </summary>
		internal const string DB_SCHEMA = "settings";
	}
}
