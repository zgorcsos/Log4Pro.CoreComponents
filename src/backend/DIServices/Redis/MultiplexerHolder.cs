using StackExchange.Redis;
using System;

namespace Log4Pro.DIServices.Redis
{
	/// <summary>
	/// This class encapsulates an Multiplexer instance, and provide auto reconnect an halted multiplexer.
	/// </summary>
	internal class MultiplexerHolder
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MultiplexerHolder"/> class.
		/// </summary>
		/// <param name="config">The configuration.</param>
		/// <param name="recreateDelay">The recreate delay.</param>
		public MultiplexerHolder(ConfigurationOptions config, TimeSpan recreateDelay)
		{
			_recreateDelay = recreateDelay;
			_config = config;
			_connectionMultiplexer = new Lazy<ConnectionMultiplexer>(MultiplexerFactory);
		}

		/// <summary>
		/// Gets the total out standing.
		/// </summary>
		/// <value>
		/// The total out standing.
		/// </value>
		public long TotalOutStanding
		{
			get
			{
				return _connectionMultiplexer.IsValueCreated ? _connectionMultiplexer.Value.GetCounters().TotalOutstanding : -1;
			}
		}

		/// <summary>
		/// Gets a value indicating whether [multiplexer created].
		/// </summary>
		/// <value>
		///   <c>true</c> if [multiplexer created]; otherwise, <c>false</c>.
		/// </value>
		public bool MultiplexerCreated
		{
			get
			{
				return _connectionMultiplexer.IsValueCreated;
			}
		}

		/// <summary>
		/// Gets a value indicating whether [in error].
		/// </summary>
		/// <value>
		///   <c>true</c> if [in error]; otherwise, <c>false</c>.
		/// </value>
		public bool InError
		{
			get
			{
				return _fixTheMultiplexerTimer != null;
			}
		}

		/// <summary>
		/// Gets the multiplexer.
		/// </summary>
		/// <value>
		/// The multiplexer.
		/// </value>
		public ConnectionMultiplexer Multiplexer
		{
			get
			{
				return _connectionMultiplexer.Value;
			}
		}

		private readonly ConfigurationOptions _config;

		private readonly TimeSpan _recreateDelay;

		private ConnectionMultiplexer MultiplexerFactory()
		{			
			Id = Guid.NewGuid();
			var multiplexer = ConnectionMultiplexer.Connect(_config);
			multiplexer.ConnectionFailed += ConnectionFailed;
			multiplexer.ConnectionRestored += ConnectionRestored;
			return multiplexer;
		}

		private Lazy<ConnectionMultiplexer> _connectionMultiplexer;

		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		public Guid Id { get; private set; }

		private void ConnectionFailed(object sender, ConnectionFailedEventArgs e)
		{
			// Connectionfailed callad: SocketFailure, Interactive
			// Set the reconnect task this!
			if (_fixTheMultiplexerTimer == null)
			{
				_fixTheMultiplexerTimer = new System.Threading.Timer(RecreateThisMultiplexerCallBack, null, TimeSpan.Zero, _recreateDelay);
			}
		}

		private void RecreateThisMultiplexerCallBack(object state)
		{
			// drop here the halted Multiplexer and create a new by Lazy 
			_connectionMultiplexer.Value.Close();
			_connectionMultiplexer.Value.Dispose();
			_connectionMultiplexer = new Lazy<ConnectionMultiplexer>(MultiplexerFactory);
			_fixTheMultiplexerTimer?.Dispose();
			_fixTheMultiplexerTimer = null;
		}

		private void ConnectionRestored(object sender, ConnectionFailedEventArgs e)
		{
			// ConnectionRestored called: None, Interactive,
			// Reset the reconnect task this!
			_fixTheMultiplexerTimer?.Dispose();
			_fixTheMultiplexerTimer = null;
		}

		private System.Threading.Timer _fixTheMultiplexerTimer = null;
	}
}
