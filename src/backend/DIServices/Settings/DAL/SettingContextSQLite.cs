using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vrh.DIServices.Settings.DAL
{
	public class SettingContextSQLite: SettingContext
	{
		public SettingContextSQLite(
			DbContextOptions options) : base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			//options.UseSqlite("Filename=:memory:", x => x.MigrationsAssembly(typeof(SettingContext).Assembly.FullName));
		}
	}
}
