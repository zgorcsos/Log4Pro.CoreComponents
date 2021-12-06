using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.DIServices.UnitTesting
{	
	public class UnitTestEnvironment
	{		
		public static void SetUnitTestEnvironment(IServiceCollection services)
		{
			services.AddSingleton(typeof(UnitTestEnvironment));
			services.AddSingleton(services);
		}
	}
}
