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
using Vrh.DIServices.Caching;
using Vrh.Classes.UnitTestExtensions;
using Vrh.DIServices;
using Vrh.DIServices.Caching.Providers.Redis;

namespace Vrh.Test.DIServices.Caching.Providers.Redis
{
	public class RedisCacheProviderUnitTest : TestBaseClassWithServiceCollection
	{
		public RedisCacheProviderUnitTest(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);			
			_serviceCollection.AddSingleton<IConfiguration>(builder.Build());
			_serviceCollection.AddSingleton(typeof(IRedisConnectionMultiplexerStore), typeof(RedisConnectionMultiplexerStore));
			_serviceCollection.AddSingleton(typeof(ICacheProvider), typeof(RedisCache));
		}

		//private readonly DataId _testAddress = new() { ModuleKey = "M", InstanceOrUserKey = "I", Key = "K" };
		//private readonly DataId _jsontTestAddress = new() { ModuleKey = "M", InstanceOrUserKey = "I", Key = "J" };
		//private readonly DataId _removeTestAddress = new() { ModuleKey = "M", InstanceOrUserKey = "I", Key = "R" };

		[Fact(DisplayName = "Appsettings configuration works.")]
		public void ConfigurationWork()
		{
			ICacheProvider cp = GetService<ICacheProvider>();
			Assert.Equal(1, (cp as RedisCache).DatabaseNumber);
		}

		[Fact(DisplayName = "Store & Read functions work.")]
		public void ObjectStoreAndReadWork()
		{
			ICacheProvider cp = GetService<ICacheProvider>();
			int testValue = 1;
			cp.Publish("", testValue);
			var readed = cp.Read<int>("");
			Assert.Equal(testValue, readed);
		}

		[Fact(DisplayName = "Remove function works. The provider throws NotFoundException, if the data not exist by specified key.")]
		public void RemoveAndNotFoundExceptionWork()
		{
			ICacheProvider cp = GetService<ICacheProvider>();
			int testValue = 1;
			cp.Publish("", testValue);
			var readed = cp.Read<int>("");
			Assert.Equal(testValue, readed);
			cp.Remove("");
			Assert.Throws<NotFoundException>(() => cp.Read<int>(""));
		}

		[Fact(DisplayName = "Bad appsettig configuration value protection works.")]
		public void BadConfigProtectionWork()
		{
			var s = _serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration));
			_serviceCollection.Remove(s);
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.BadRedisCache.json", optional: true, reloadOnChange: true);
			_serviceCollection.AddSingleton<IConfiguration>(builder.Build());
			ICacheProvider cp = GetService<ICacheProvider>();
			Assert.Equal(-1, (cp as RedisCache).DatabaseNumber);
		}
	}
}
