using StackExchange.Redis;

namespace Vrh.DIServices.Redis
{
	/// <summary>
	/// Stores and provides the reusable Redis Stackexchange ConnectionMultiplexer object
	/// </summary>
	public interface IRedisConnectionMultiplexerStore
	{
		/// <summary>
		/// Privide the multiplexer to use Redis.
		/// </summary>
		/// <value>
		/// The multiplexer.
		/// </value>
		ConnectionMultiplexer Multiplexer { get; }
	}
}
