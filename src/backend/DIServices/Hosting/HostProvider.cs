using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.DIServices.Hosting
{
	/// <summary>
	/// Host provider impelmentation.
	/// </summary>
	/// <seealso cref="Log4Pro.AspNetServices.Hosting.IHostProvider" />
	public class HostProvider : IHostProvider
	{
		private readonly IWebHostEnvironment _webHostEnvironment;

		/// <summary>
		/// Initializes a new instance of the <see cref="HostProvider"/> class.
		/// Never create instance manually! Use .NET DI for correct dependency resolve!
		/// </summary>
		/// <param name="environment">The environment.</param>
		public HostProvider(IWebHostEnvironment environment)
		{
			_webHostEnvironment = environment;
		}

		///<inheritdoc cref="IHostProvider"/>
		public string UsedEnvironment => _webHostEnvironment.EnvironmentName;

		///<inheritdoc cref="IHostProvider"/>
		public string ApplicationName => _webHostEnvironment.ApplicationName;

		///<inheritdoc cref="IHostProvider"/>
		public string ContentAbsolutePath(string relativePath = null) => string.IsNullOrEmpty(relativePath)
				? _webHostEnvironment.ContentRootPath
				: Path.Combine(_webHostEnvironment.ContentRootPath, relativePath);

		///<inheritdoc cref="IHostProvider"/>
		public string WebRootAbsolutePath(string relativePath = null) => string.IsNullOrEmpty(relativePath)
				? _webHostEnvironment.WebRootPath
				: Path.Combine(_webHostEnvironment.WebRootPath, relativePath);
	}
}
