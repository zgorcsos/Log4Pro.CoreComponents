﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WkHtmlToPdfDotNet.Contracts;
using WkHtmlToPdfDotNet;
using Vrh.DIServices.Reports;
using Vrh.DIServices.Hosting;
using Vrh.DIServices.ContentInjector;
using Vrh.DIServices.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Vrh.DIServices.UnitTesting;
using Vrh.DIServices.Settings;
using Vrh.DIServices.Settings.DAL;
using Vrh.DIServices.Caching;
using Vrh.DIServices.Caching.Providers.ManagedMemory;
using Vrh.DIServices.Caching.Providers.Redis;
using Microsoft.Data.Sqlite;

namespace Vrh.DIServices
{
	/// <summary>
	/// Extension class for IServiceCollection for simplify our service registrations 
	/// </summary>
	public static class VrhServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the report provider. (Scoped lifetime)
		/// And adds the all necesary services too, if those not registered yet.
		/// So adds these dependency services:
		///		- PdfTools (see: https://github.com/HakanL/WkHtmlToPdf-DotNet)
		///		- HostProvider (Vrh.DIServices)
		///		- ContentInjector (Vrh.DIServices)
		/// </summary>
		/// <typeparam name="TImplementation">The type of the report implementation.</typeparam>
		/// <param name="services">The services.</param>
		/// <returns></returns>
		public static IServiceCollection AddReportProvider<TImplementation>(this IServiceCollection services)
			where TImplementation : IReport
		{
			if (typeof(TImplementation).Name == typeof(SimpleReport).Name)
			{
				// Dependencies
				services.AddContentInjector();
				if (!services.Any(x => x.ServiceType == typeof(IConverter) && x.ImplementationType == typeof(PdfTools) && x.Lifetime == ServiceLifetime.Singleton))
				{
					// https://github.com/HakanL/WkHtmlToPdf-DotNet
					services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
				}
			}
			if (!services.Any(x => x.ServiceType == typeof(IReport) && x.ImplementationType == typeof(TImplementation) && x.Lifetime == ServiceLifetime.Scoped))
			{
				services = services.AddScoped(typeof(IReport), typeof(TImplementation));
			}
			return services;
		}

		/// <summary>
		/// Adds the host provider. (Singleton lifetime)
		/// </summary>
		/// <typeparam name="TImplementation">The type of the implementation.</typeparam>
		/// <param name="services">The services.</param>
		/// <returns></returns>
		public static IServiceCollection AddHostProvider<TImplementation>(this IServiceCollection services)
			where TImplementation : IHostProvider
		{
			if (!services.Any(x => x.ServiceType == typeof(IHostProvider) && x.ImplementationType == typeof(TImplementation) && x.Lifetime == ServiceLifetime.Singleton))
			{
				services.AddSingleton(typeof(IHostProvider), typeof(TImplementation));
			}
			return services;
		}

		/// <summary>
		/// Adds the content injector service. (Singleton lifetime)
		/// And adds the all necesary services too, if those not regstered yet.
		/// So registers thise dependency service:
		///		- HostProvider (Vrh.DIServices)
		/// </summary>
		/// <param name="services">The services.</param>
		/// <returns></returns>
		public static IServiceCollection AddContentInjector(this IServiceCollection services)
		{
			// dependencies:
			services = services.AddHostProvider<HostProvider>();
			if (!services.Any(x => x.ServiceType == typeof(FileContentInjector) && x.ImplementationType == typeof(FileContentInjector) && x.Lifetime == ServiceLifetime.Singleton))
			{
				services.AddSingleton(typeof(FileContentInjector));
			}
			return services;
		}

		/// <summary>
		/// Adds the redis connection multiplexer store. (Singleton lifetime)
		/// </summary>
		/// <param name="services">The services.</param>
		/// <returns></returns>
		public static IServiceCollection AddRedisConnectionMultiplexerStore(this IServiceCollection services)
		{
			if (!services.Any(x => x.ServiceType == typeof(IRedisConnectionMultiplexerStore) && x.ImplementationType == typeof(RedisConnectionMultiplexerStore)
				&& x.Lifetime == ServiceLifetime.Singleton))
			{
				services.AddSingleton(typeof(IRedisConnectionMultiplexerStore), typeof(RedisConnectionMultiplexerStore));
			}
			return services;
		}

		/// <summary>
		/// Adds cache.
		/// </summary>
		/// <param name="services">The services.</param>
		/// <returns></returns>
		public static IServiceCollection AddCache<TCacheProvider>(this IServiceCollection services)
			where TCacheProvider : ICacheProvider
		{
			if (!services.Any(x => x.ServiceType == typeof(ICacheProvider)))
			{
				if (typeof(TCacheProvider) == typeof(ManagedMemoryCache))
				{
					services.AddSingleton(typeof(ICacheProvider), typeof(ManagedMemoryCache));
				}
				if (typeof(TCacheProvider) == typeof(RedisCache))
				{
					services.AddRedisConnectionMultiplexerStore();
					services.AddSingleton(typeof(ICacheProvider), typeof(RedisCache));
				}
			}
			if (!services.Any(x => x.ServiceType == typeof(Cache)))
			{
				services.AddSingleton(typeof(Cache));
			}
			return services;
		}

		/// <summary>
		/// Registers the vsettings service for DI.
		/// </summary>
		/// <param name="services">The services.</param>
		/// <returns></returns>
		public static IServiceCollection AddVSettings(this IServiceCollection services, IConfiguration configuration)
		{
			if (!services.Any(x => x.ImplementationType == typeof(VSettings) && x.Lifetime == ServiceLifetime.Singleton))
			{
				if (configuration == null)
				{
					configuration = (IConfiguration)services.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration)).ImplementationInstance;
				}
				var config = Settings.VSettings.GetAppSettingConfiguration(configuration);
				if (services.Any(x => x.ServiceType == typeof(UnitTestEnvironment)))
				{

					var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
					inMemorySqlite.Open();
					services.AddDbContext<SettingContextSQLite>(options => 
						options.UseSqlite(inMemorySqlite, x => x.MigrationsAssembly(typeof(SettingContext).Assembly.FullName)));
				}
				else
				{
					services.AddDbContext<SettingContext>(options =>
						options.UseSqlServer(configuration.GetConnectionString(config.UsedConnectionString)));
				}
				services.AddCache<ManagedMemoryCache>();
				services.AddSingleton(typeof(VSettings));
			}
			return services;
		}
	}
}
