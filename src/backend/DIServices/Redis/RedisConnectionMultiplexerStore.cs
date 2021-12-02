using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Vrh.DIServices.Redis
{
	/// <summary>
	/// RedisConnectionMultiplexer store implementation
	/// </summary>
	/// <seealso cref="Vrh.DIServices.Redis.IRedisConnectionMultiplexerStore" />
	public class RedisConnectionMultiplexerStore : IRedisConnectionMultiplexerStore
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<RedisConnectionMultiplexerStore> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="RedisConnectionMultiplexerStore"/> class.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		public RedisConnectionMultiplexerStore(IConfiguration configuration, ILogger<RedisConnectionMultiplexerStore> logger)
		{
			_configuration = configuration;
			_logger = logger;
			bool configFound = true;
			var config = _configuration.GetSection(APPSETTINGS_SECTION_NAME).Get<RedisConnectionAppSettings>();
			if (config == null)
			{
				configFound = false;
				config = new RedisConnectionAppSettings();
			}
			if (string.IsNullOrEmpty(config.Connection))
			{
				config.Connection = DEFAULT_REDIS_CONNECTIONSTRING;
			}
			if (config.PoolDeep == 0)
			{
				config.PoolDeep = 1;
			}
			if (config.RecreateHaltedMultiplexerDelay == default)
			{
				config.RecreateHaltedMultiplexerDelay = new TimeSpan(0, 0, 30);
			}
			_redisConnection = new RedisConnection(config);
			using (_logger.BeginScope(new Dictionary<string, object>
			{
				{"APPSETTINGS_SECTION_NAME", APPSETTINGS_SECTION_NAME},
				{"ConfigurationIsFound", configFound},
				{"RedisConnectionString", config.Connection },
				{"PoolDeep", config.PoolDeep },
				{"RecreateHaltedMultiplexerDelay", config.RecreateHaltedMultiplexerDelay },
			}))
			{
				_logger.LogDebug("Create RedisConnectionMultiplexerStore instance.");
			}
		}

		/// <summary>
		/// Privide the multiplexer to use Redis.
		/// </summary>
		/// <value>
		/// The multiplexer.
		/// </value>
		public ConnectionMultiplexer Multiplexer
		{
			get
			{
				_lock.Wait();
				try
				{
					var inHolder = _redisConnection.BestConnection;
					var cm = inHolder.Multiplexer;
					using (_logger.BeginScope(new Dictionary<string, object>
					{
						{"Multiplexer Id", inHolder.Id},
						{"Current TotalOutStanding", inHolder.TotalOutStanding},
						{"MultiplexerCreated", inHolder.MultiplexerCreated },
						{"In error state", inHolder.InError},
					}))
					{
						_logger.LogDebug("Redis multiplexer instance retreive succesfully.");
					}
					return cm;
				}
				finally
				{
					_lock.Release();
				}
			}
		}

		private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
		private readonly RedisConnection _redisConnection;
		private const string APPSETTINGS_SECTION_NAME = "RedisConnection";
		private const string DEFAULT_REDIS_CONNECTIONSTRING = "localhost";
	}
}
