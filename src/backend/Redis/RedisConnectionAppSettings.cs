using System;

namespace Log4Pro.CoreComponents.Redis
{
	/// <summary>
	/// Represent an redis connection settings
	/// </summary>
	internal class RedisConnectionAppSettings
	{
		/// <summary>
		/// The connection as Stackexchange Redis connectionstring.
		/// <see href="https://stackexchange.github.io/StackExchange.Redis/Configuration.html#basic-configuration-strings"/>
		/// </summary>
		public string Connection { get; set; }

		/// <summary>
		/// <strong>The pool deepness. It defines how many ConectionMultiplexers provide for this connection.</strong>
		/// <para>Default value is 1.</para>
		/// <para>
		/// See the 6th point of <see href="https://gist.github.com/JonCole">@JonCole</see>'s guide
		/// <see href="https://gist.github.com/JonCole/925630df72be1351b21440625ff2671f#file-redis-bestpractices-stackexchange-redis-md">here. </see>
		/// </para>
		/// </summary>
		public int PoolDeep { get; set; }

		/// <summary>
		/// <strong>It specifies an time after which drop and recreat an unworking ConnectionMultiplexer object.</strong>
		/// <para>Default value is 30 sec.</para>		 
		/// <para>
		/// See the <see href="https://gist.github.com/JonCole">@JonCole</see>'s guide  
		/// <see href="https://gist.github.com/JonCole/925630df72be1351b21440625ff2671f#file-redis-bestpractices-stackexchange-redis-md">here. </see>
		/// Here is the relevant sentence: 
		/// </para>
		/// <para>
		/// "We have seen a few <see href="https://github.com/StackExchange/StackExchange.Redis/issues/559">rare cases </see> 
		/// where StackExchange.Redis fails to reconnect after a connection blip (for example, due to patching). 
		/// <strong>Creating a new ConnectionMultiplexer will fix the issue.</strong>"
		/// </para>
		/// </summary>
		public TimeSpan RecreateHaltedMultiplexerDelay { get; set; }
	}
}
