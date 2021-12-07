using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Log4Pro.CoreComponents.DIServices.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Log4Pro.CoreComponents.Classes.UnitTestExtensions;
using Log4Pro.CoreComponents.DIServices;
using Microsoft.Extensions.Logging.Abstractions;

namespace Log4Pro.CoreComponents.Test.DIServices.Redis
{
	public class RedisConnectionMultiplexerStoreUnitTest : TestBaseClassWithServiceCollection
	{
		public RedisConnectionMultiplexerStoreUnitTest(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			_serviceCollection = _serviceCollection.AddRedisConnectionMultiplexerStore();
		}

		[Fact(DisplayName = "AddRedisConnectionMultiplexerStore IServiceCollection extension to add the service works well.")]
		public void RegisterExtensionWork()
		{
			Assert.NotNull(GetService<IRedisConnectionMultiplexerStore>());
			Assert.Single(_serviceCollection.Where(x => x.ServiceType == typeof(IRedisConnectionMultiplexerStore) && x.ImplementationType == typeof(RedisConnectionMultiplexerStore)));
		}

		[Fact(DisplayName = "AddRedisConnectionMultiplexerStore IServiceCollection extension not duplicates the service, when happens multiple call to the extension method.")]
		public void ProtectDuplicate()
		{
			Assert.NotNull(GetService<IRedisConnectionMultiplexerStore>());
			_serviceCollection = _serviceCollection.AddRedisConnectionMultiplexerStore();
			Assert.Single(_serviceCollection.Where(x => x.ServiceType == typeof(IRedisConnectionMultiplexerStore) && x.ImplementationType == typeof(RedisConnectionMultiplexerStore)));
		}

		[Fact(DisplayName = "The registered service instance is singletone.")]
		public void IsSingleton()
		{
			Assert.Same(GetService<IRedisConnectionMultiplexerStore>(), GetService<IRedisConnectionMultiplexerStore>());
		}

		[Fact(DisplayName = "Get Redis multiplexer works.")]
		public void GetMultiplexer()
		{
			var s = _serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration));
			_serviceCollection.Remove(s);
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);			
			_serviceCollection.AddSingleton<IConfiguration>(builder.Build());
			// WARNING this test succes only if redis is avilable on localhost defult port
			var mp = GetService<IRedisConnectionMultiplexerStore>().Multiplexer;
			Assert.NotNull(mp);
		}

		[Fact(DisplayName = "Appsettings configuration works.")]
		public void ConfigurationWork()
		{
			var s = _serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration));
			_serviceCollection.Remove(s);
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.error.json", optional: true, reloadOnChange: true);
			_serviceCollection.AddSingleton<IConfiguration>(builder.Build());
			Assert.Throws<Exception>(() => GetService<IRedisConnectionMultiplexerStore>().Multiplexer);
		}

		[Fact(DisplayName = "Defaults is works, if the defined appsettings section is not exist.")]
		public void DefaultsWork()
		{
			var s = _serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration));
			_serviceCollection.Remove(s);
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.notexist.json", optional: true, reloadOnChange: true);
			_serviceCollection.AddSingleton<IConfiguration>(builder.Build());
			// WARNING this test succes only if redis is avilable on localhost defult port
			var mp = GetService<IRedisConnectionMultiplexerStore>().Multiplexer;
			Assert.NotNull(mp);
		}
	}
}
