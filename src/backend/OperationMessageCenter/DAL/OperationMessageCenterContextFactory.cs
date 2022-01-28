using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Log4Pro.CoreComponents.OperationMessageCenter.DAL
{
#pragma warning disable CS1658 // Warning is overriding an error
#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
	/// <summary>
	/// Implements the IDesignTimeDbContextFactory interface for support the design time commands of EF migration in VS 
	/// </summary>
	/// <seealso cref="Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory&lt;OperationMessageCenterContext&gt;" />
	public class OperationMessageCenterContextFactory : IDesignTimeDbContextFactory<OperationMessageCenterContext>
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
		public OperationMessageCenterContext CreateDbContext(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				 .SetBasePath(Directory.GetCurrentDirectory())
				 .AddJsonFile("appsettings.json", true)
				 .Build();
			var config = OperationMessageService.GetAppSettingConfiguration(configuration);
			var connectionString = configuration
						.GetConnectionString(config.UsedConnectionString);
			var builder = new DbContextOptionsBuilder<OperationMessageCenterContext>();
			builder.UseSqlServer(connectionString, x =>
			{
				x.MigrationsAssembly(typeof(OperationMessageCenterContext).Assembly.FullName);
				x.MigrationsHistoryTable("__EFMigrationsHistory", OperationMessageCenterContext.DB_SCHEMA);
			});
			return new OperationMessageCenterContext(builder.Options);
		}
	}
}
