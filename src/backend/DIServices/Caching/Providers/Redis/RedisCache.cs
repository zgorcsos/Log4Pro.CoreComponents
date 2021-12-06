using System;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Log4Pro.DIServices.Redis;

namespace Log4Pro.DIServices.Caching.Providers.Redis
{
	/// <summary>
	/// Redis cache provider for VRH caching
	/// </summary>
	/// <seealso cref="ICacheProvider" />
	public class RedisCache : ICacheProvider
	{
		private readonly IRedisConnectionMultiplexerStore _redis;
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Initializes a new instance of the <see cref="RedisCache"/> class.
		/// </summary>
		/// <param name="redis">The redis multiplexer store</param>
		/// <param name="configuration"></param>
		public RedisCache(IRedisConnectionMultiplexerStore redis, IConfiguration configuration)
		{
			_configuration = configuration;
			_redis = redis;
		}

		/// <inheritdoc/>
		public void Publish<TStoredObject>(string dataId, TStoredObject value)
		{
			var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
			var db = _redis.Multiplexer.GetDatabase(DatabaseNumber);
			if (!db.StringSet(dataId, JsonConvert.SerializeObject(value, settings)))
			{
				throw new Exception("Redis StringSet error!");
			}
		}

		/// <inheritdoc/>
		public TStoredObject Read<TStoredObject>(string dataId)
		{
			var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
			var db = _redis.Multiplexer.GetDatabase(DatabaseNumber);
			var value = db.StringGet(dataId);
			if (!value.HasValue)
			{
				throw new NotFoundException(dataId);
			}
			return JsonConvert.DeserializeObject<TStoredObject>(value, settings);
		}

		/// <inheritdoc/>
		public void Remove(string dataId)
		{
			var db = _redis.Multiplexer.GetDatabase(DatabaseNumber);
			db.KeyDelete(dataId);
		}

		/// <summary>
		/// No of Redis db, that uses by caching
		/// </summary>
		public int DatabaseNumber
		{
			get
			{
				try
				{
					var config = _configuration.GetSection(APPSETTINGS_SECTION_NAME).Get<RedisCacheAppSettings>();
					return config.UsedDatabase;
				}
				catch
				{
					return -1;
				}				
			}
		}

		private const string APPSETTINGS_SECTION_NAME = "RedisCache";
	}
}
