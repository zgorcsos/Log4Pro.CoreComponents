using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Vrh.DIServices.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Vrh.Classes.UnitTestExtensions;
using Vrh.DIServices.UnitTesting;

namespace Vrh.Test.DIServices.UnitTesting
{
	public class UnitTestEnvironmentUnitTest : TestBaseClassWithServiceCollection
	{
		public UnitTestEnvironmentUnitTest(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		[Fact(DisplayName = "Unit test environment service is present from test base class.")]
		public void UnitTestEnvironmentServiceIsPresent()
		{
			var s = GetService<UnitTestEnvironment>();
			Assert.NotNull(s);
		}
	}
}
