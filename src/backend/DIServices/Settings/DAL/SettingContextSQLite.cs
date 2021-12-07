using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.DIServices.Settings.DAL
{
    /// <summary>
    /// Context for unit testing
    /// </summary>
    /// <seealso cref="Log4Pro.CoreComponents.DIServices.Settings.DAL.SettingContext" />
    public class SettingContextSQLite: SettingContext
	{
		public SettingContextSQLite(
			DbContextOptions options) : base(options)
		{
		}
	}
}
