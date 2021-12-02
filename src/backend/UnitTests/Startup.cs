using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vrh.Classes.UnitTestExtensions;
using Vrh.DIServices.UnitTesting;

namespace Vrh.Test
{
	public class Startup
	{
		public void ConfigureHost(IHostBuilder hostBuilder)
		{
		}

		public void ConfigureServices(IServiceCollection services, HostBuilderContext context)
		{
			UnitTestEnvironment.SetUnitTestEnvironment(services);			
		}
	}
}
