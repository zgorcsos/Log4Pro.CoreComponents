using System;
using Xunit;
using Vrh.DIServices.Caching;
using Newtonsoft.Json;
using Moq;
using Microsoft.Extensions.Logging;
using Vrh.DIServices.Redis;
using Vrh.DIServices.Caching.Providers.Redis;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Vrh.DIServices.Caching.Providers.ManagedMemory;
using Vrh.Classes.UnitTestExtensions;
using Vrh.DIServices;

namespace Vrh.Test.DIServices.Caching
{
	public class CachingUnitTests : TestBaseClassWithServiceCollection
	{
		public CachingUnitTests(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			_serviceCollection = _serviceCollection.AddCache<ManagedMemoryCache>();
		}

		[Fact(DisplayName = "AddCache IServiceCollection extension to add the service works well.")]
		public void ServiceRegisterExtensionWorksWell()
		{
			var s = GetService<Cache>();
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(Cache));
			Assert.NotNull(s);
			Assert.IsType<Cache>(s);
		}

		[Fact(DisplayName = "AddCache IServiceCollection extension adds the needed service dependency too.")]
		public void ServiceRegisterExtensionWorksWell2()
		{
			var s = GetService<ICacheProvider>();
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(ICacheProvider));
			Assert.NotNull(s);
			Assert.IsType<ManagedMemoryCache>(s);
			Assert.Same(s, GetService<ICacheProvider>());
			IServiceCollection services = new ServiceCollection();
			services = services.AddCache<RedisCache>();
			Assert.Single(services, x => x.ServiceType == typeof(ICacheProvider) && x.ImplementationType == typeof(RedisCache) && x.Lifetime == ServiceLifetime.Singleton);
			Assert.Single(services, x => x.ServiceType == typeof(IRedisConnectionMultiplexerStore) && x.Lifetime == ServiceLifetime.Singleton);
		}

		[Fact(DisplayName = "AddCache IServiceCollection extension not duplicates the service, when happens multiple call to the extension method.")]
		public void ServiceRegisterExtensionWorksWell3()
		{
			_serviceCollection.AddCache<RedisCache>();
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(Cache));
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(ICacheProvider));
		}

		[Fact(DisplayName = "AddCache IServiceCollection extension works .")]
		public void ServiceRegisterExtensionWorksWell4()
		{
			_serviceCollection.AddCache<RedisCache>();
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(Cache));
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(ICacheProvider));
		}

		[Fact(DisplayName = "AddCache IServiceCollection extension works well by TCacheProvider type parameter.")]
		public void ServiceRegisterExtensionWorksWell5()
		{
			IServiceCollection services = new ServiceCollection();
			services = services.AddCache<RedisCache>();
			Assert.Single(services, x => x.ServiceType == typeof(ICacheProvider) && x.ImplementationType == typeof(RedisCache) && x.Lifetime == ServiceLifetime.Singleton);
			Assert.DoesNotContain(services, x => x.ServiceType == typeof(ICacheProvider) && x.ImplementationType == typeof(ManagedMemoryCache));
			Assert.Single(services, x => x.ServiceType == typeof(IRedisConnectionMultiplexerStore) && x.Lifetime == ServiceLifetime.Singleton);			
			services = new ServiceCollection();
			services = services.AddCache<ManagedMemoryCache>();
			Assert.Single(services, x => x.ServiceType == typeof(ICacheProvider) && x.ImplementationType == typeof(ManagedMemoryCache) && x.Lifetime == ServiceLifetime.Singleton);
			Assert.DoesNotContain(services, x => x.ServiceType == typeof(ICacheProvider) && x.ImplementationType == typeof(RedisCache));
			Assert.DoesNotContain(services, x => x.ServiceType == typeof(IRedisConnectionMultiplexerStore));
		}

		[Fact(DisplayName = "Cache service is singletone.")]
		public void IsSingletone()
		{
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(Cache) && x.Lifetime == ServiceLifetime.Singleton);
			var cp = GetService<Cache>();
			var cp2 = GetService<Cache>();
			Assert.Same(cp, cp2);
		}

		[Fact(DisplayName = "NotFoundException iplementation is great.")]
		public void NotFoundException()
		{
			var a = GetSampleAdressingInstance();
			var ex = new NotFoundException(a);
			Assert.True(ex is Exception);
			Assert.Equal(a, ex.DataId);
		}

		[Fact(DisplayName = "Publish & read functions work.")]
		public void CachePublishAndReadIsWork()
		{
			var a = GetSampleAdressingInstance();
			string serialized = JsonConvert.SerializeObject(a);
			var cache = GetService<Cache>();
			cache.Publish(a, a);
			Assert.Equal(serialized, JsonConvert.SerializeObject(cache.Read<string>(a)));
		}

		[Fact(DisplayName = "Remove function is works.")]
		public void RemoveIsWork()
		{
			var a = GetSampleAdressingInstance();
			string serialized = JsonConvert.SerializeObject(a);
			var cache = GetService<Cache>();
			cache.Publish(a, a);
			Assert.Equal(serialized, JsonConvert.SerializeObject(cache.Read<string>(a)));
			cache.RemoveFromCache(a);
			Assert.Throws<NotFoundException>(() => cache.Read<string>(a));
		}

		[Fact(DisplayName = "Read function throws NotFoundException, if the requested key is not present in cache.")]
		public void ThrowNotFound()
		{
			var a = GetSampleAdressingInstance();
			var cache = GetService<Cache>();
			Assert.Throws<NotFoundException>(() => cache.Read<string>(a));
		}

		[Fact(DisplayName = "AutoPublisher is work well, if setted.")]
		public void AutoPublisherIsWork()
		{
			var a = GetSampleAdressingInstance();
			var cache = GetService<Cache>();
			cache.RemoveFromCache(a);
			Assert.Throws<NotFoundException>(() => cache.Read<int>(a));
			cache.AutoPublisher = AutoPublisher;
			Assert.Equal(1, cache.Read<int>(a));
		}

		private string StringAutoPublisher(string a)
		{
			return "auto";
		}

		private object AutoPublisher(string key)
		{
			return 1;
		}

		private static string GetSampleAdressingInstance()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
