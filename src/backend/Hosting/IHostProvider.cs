using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.Hosting
{
	/// <summary>
	/// Host provider interface
	/// </summary>
	public interface IHostProvider
	{
		/// <summary>
		/// Gets absolute path from relative path under the current host content.
		/// </summary>
		/// <param name="relativePath">The relative path.</param>
		/// <returns>The absolute path in current hosting environment.</returns>
		string ContentAbsolutePath(string relativePath);

		/// <summary>
		/// Gets absolute path from relative path under the wwwroot.
		/// </summary>
		/// <param name="relativePath">The relative path.</param>
		/// <returns></returns>
		string WebRootAbsolutePath(string relativePath);

		/// <summary>
		/// Gets the used environment in current host.
		/// </summary>
		/// <value>
		/// The used environment.
		/// </value>
		string UsedEnvironment { get; }

		/// <summary>
		/// Gets the name of the application in current host.
		/// </summary>
		/// <value>
		/// The name of the application.
		/// </value>
		string ApplicationName { get; }
	}
}
