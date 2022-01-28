using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Log4Pro.CoreComponents.OperationMessageCenter.DAL
{
#pragma warning disable CS1658 // Warning is overriding an error
#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
	/// <summary>
	/// Implements the IDesignTimeDbContextFactory interface for support the design time commands of EF migration in VS 
	/// </summary>
	/// <seealso cref="Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory&lt;OperationMessageCenterContextSQLite&gt;" />
	public class OperationMessageCenterContextFactorySQLite : IDesignTimeDbContextFactory<OperationMessageCenterContextSQLite>
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
		public OperationMessageCenterContextSQLite CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<OperationMessageCenterContext>();
			builder.UseSqlite("Filename=:memory:", x => x.MigrationsAssembly(typeof(OperationMessageCenterContextSQLite).Assembly.FullName));
			return new OperationMessageCenterContextSQLite(builder.Options);
		}
	}
}
