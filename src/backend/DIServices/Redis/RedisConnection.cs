using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Log4Pro.CoreComponents.DIServices.Redis
{
	/// <summary>
	/// Represent an Redis connection with one or more Multiplexer stock (see PoolDeep parameter to)
	/// </summary>
	internal class RedisConnection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RedisConnection"/> class.
		/// </summary>
		/// <param name="settings">The settings.</param>
		public RedisConnection(RedisConnectionAppSettings settings)
		{
			_redisConnectionSettings = settings;
			for (int i = 0; i < _redisConnectionSettings.PoolDeep; i++)
			{
				_poolOfMultiplexers.Add(CreateMultiplexerHolder());
			}
		}

		private readonly RedisConnectionAppSettings _redisConnectionSettings;

		/// <summary>
		/// Gets the best connection.
		/// </summary>
		/// <value>
		/// The best connection.
		/// </value>
		public MultiplexerHolder BestConnection
		{
			get
			{
				if (_poolOfMultiplexers.Count == 1 || !_poolOfMultiplexers.Any(x => x.MultiplexerCreated))
				{
					return _poolOfMultiplexers.FirstOrDefault();
				}
				else
				{
					var winner = _poolOfMultiplexers.Where(x => x.MultiplexerCreated && !x.InError).OrderBy(x => x.TotalOutStanding).FirstOrDefault();
					if ((winner == null || winner.TotalOutStanding > 0) && _poolOfMultiplexers.Where(x => x.MultiplexerCreated).Count() < _poolOfMultiplexers.Count)
					{
						winner = _poolOfMultiplexers.Where(x => !x.MultiplexerCreated).FirstOrDefault();
						if (winner == null)
						{
							// winner posible null here!!!
							// (this is a non treadsafe section for less delay, so posible null, if created by other tread bettween linq queries!!!)
							winner = _poolOfMultiplexers.Where(x => x.MultiplexerCreated && !x.InError).OrderBy(x => x.TotalOutStanding).FirstOrDefault();
						}
					}
					// winner posible null here!!!
					// this is a non treadsafe section for less delay, so posible null, if changed the _poolOfMultiplexers ConcurentBag contents!!! 
					return winner ?? _poolOfMultiplexers.FirstOrDefault();
				}
			}
		}

		private MultiplexerHolder CreateMultiplexerHolder()
		{
			try
			{
				var config = ConfigurationOptions.Parse(_redisConnectionSettings.Connection);
				if (config.AbortOnConnectFail)
				{
					// For azure Redis or taskpool use AbortOnConnectFail be always False!!!
					config.AbortOnConnectFail = false;
				}
				return new MultiplexerHolder(config, _redisConnectionSettings.RecreateHaltedMultiplexerDelay);
			}
			catch (Exception ex)
			{
				throw new Exception("Bad configstring!", ex);
			}
		}
		
		private readonly ConcurrentBag<MultiplexerHolder> _poolOfMultiplexers = new();
	}
}
