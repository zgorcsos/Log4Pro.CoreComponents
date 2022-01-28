using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Log4Pro.CoreComponents.Settings.Internals;

namespace Log4Pro.CoreComponents.Settings.DAL
{
#pragma warning disable CS1658 // Warning is overriding an error
#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
	/// <summary>
	/// Implements the IDesignTimeDbContextFactory interface for support the design time commands of EF migration in VS 
	/// </summary>
	/// <seealso cref="Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory&lt;DAL.SettingContext&gt;" />
	public class SettingContextFactorySQLite : IDesignTimeDbContextFactory<SettingContextSQLite>
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute
#pragma warning restore CS1658 // Warning is overriding an error
	{
		/// <summary>
		/// Creates a new instance of a derived context.
		/// </summary>
		/// <param name="args">Arguments provided by the design-time service.</param>
		/// <returns>
		/// An instance of SettingContext.
		/// </returns>
		public SettingContextSQLite CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<SettingContext>();
			builder.UseSqlite("Filename=:memory:", x =>
			{
				x.MigrationsAssembly(typeof(SettingContextSQLite).Assembly.FullName);
				x.MigrationsHistoryTable("__EFMigrationsHistory", SettingContextSQLite.DB_SCHEMA);
			});
			return new SettingContextSQLite(builder.Options);
		}
	}
}
