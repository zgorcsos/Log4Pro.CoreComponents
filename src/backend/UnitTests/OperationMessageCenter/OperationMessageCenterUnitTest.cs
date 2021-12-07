using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Log4Pro.CoreComponents.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Log4Pro.CoreComponents.Classes.UnitTestExtensions;
using Log4Pro.CoreComponents;
using Log4Pro.CoreComponents.Caching;
using Log4Pro.CoreComponents.Settings.Internals;
using Newtonsoft.Json;
using Log4Pro.CoreComponents.OperationMessageCenter;
using Log4Pro.CoreComponents.OperationMessageCenter.DAL;
using Log4Pro.CoreComponents.OperationMessageCenter.Internals;

namespace Log4Pro.CoreComponents.Test.Settings
{
	public class OperationMessageCenterUnitTest : TestBaseClassWithServiceCollection
	{
		public OperationMessageCenterUnitTest(IServiceProvider serviceProvider, IServiceCollection sc) : base(serviceProvider)
		{
			var s = _serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration));
			_serviceCollection.Remove(s);
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();
			_serviceCollection.AddSingleton<IConfiguration>(builder.Build());
			var c = GetService<IConfiguration>();
			_serviceCollection.AddOperationMessageCenter((IConfiguration)_serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration)).ImplementationInstance);
		}

		[Fact(DisplayName = "AddOperationMessageCenter IServiceCollection extension to add the service works well.")]
		public void ServiceRegisterExtensionWorksWell()
		{
			var s = GetService<OperationMessageServer>();
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageServer));
			Assert.NotNull(s);
			Assert.IsType<OperationMessageServer>(s);
			var w = GetService<OperationMessageWriter>();
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageWriter));
			Assert.NotNull(w);
			Assert.IsType<OperationMessageWriter>(w);
		}

		[Fact(DisplayName = "AddOperationMessageCenter IServiceCollection extension adds the needed service dependency too.")]
		public void ServiceRegisterExtensionWorksWell2()
		{
			var s = GetService<Cache>();
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(Cache));
			Assert.NotNull(s);
			Assert.IsType<Cache>(s);
			Assert.Same(s, GetService<Cache>());
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageCenterContextSQLite) && x.Lifetime == ServiceLifetime.Scoped);
		}

		[Fact(DisplayName = "AddOperationMessageCenter IServiceCollection extension not duplicates the service, when happens multiple call to the extension method.")]
		public void ServiceRegisterExtensionWorksWell3()
		{
			_serviceCollection.AddOperationMessageCenter((IConfiguration)_serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration)).ImplementationInstance);
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageServer));
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageWriter));
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(Cache));
		}

		[Fact(DisplayName = "OperationMessageServer service is singletone.")]
		public void IsSingletone()
		{
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageServer) && x.Lifetime == ServiceLifetime.Singleton);
			var s1 = GetService<OperationMessageServer>();
			var s2 = GetService<OperationMessageServer>();
			Assert.Same(s1, s2);
		}

		[Fact(DisplayName = "OperationMessageWriter service is transient.")]
		public void IsTransient()
		{
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageWriter) && x.Lifetime == ServiceLifetime.Transient);
			var s1 = GetService<OperationMessageWriter>();
			var s2 = GetService<OperationMessageWriter>();
			Assert.NotSame(s1, s2);
		}

		[Fact(DisplayName = "GetDbContext provide SQLiteContext in test environment, which is a OperationMessageCenterContext descendant.")]
		public void GetDbContextWorksWell()
		{
			var s = GetService<OperationMessageServer>();
			using (var scope = GetService<IServiceScopeFactory>().CreateScope())
			{
				Assert.IsType<OperationMessageCenterContextSQLite>(s.GetDbContext(scope));
				Assert.True(s.GetDbContext(scope) is OperationMessageCenterContext);
			}
		}

	}
}