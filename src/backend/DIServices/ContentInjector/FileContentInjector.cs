using System.Collections.Generic;
using System.IO;
using System.Text;
using Log4Pro.DIServices.Hosting;

namespace Log4Pro.DIServices.ContentInjector
{
	/// <summary>
	/// Content injector servie for useing in cshtmls.
	/// </summary>
	public class FileContentInjector
	{
		private readonly IHostProvider _hostEnvironment;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileContentInjector"/> class.
		/// Never create instance manually! Use .NET DI for correct dependency resolve!
		/// </summary>
		/// <param name="environment">The environment.</param>
		public FileContentInjector(IHostProvider environment)
		{
			_hostEnvironment = environment;
		}

		/// <summary>
		/// Gets and returns with the specified file content.
		/// </summary>
		/// <param name="contentFileRelativePath">The content file relative path.</param>
		/// <param name="dataInjections">The data injections. If dataInjections parameter is not null then the Inject method changes all occurrences of key to defined value.</param>
		/// <returns></returns>
		public string Inject(string contentFileRelativePath, Dictionary<string, string> dataInjections = null)
		{
			var content = new StringBuilder(File.ReadAllText(_hostEnvironment.ContentAbsolutePath(contentFileRelativePath), Encoding.UTF8));
			if (dataInjections != null)
			{
				foreach (var data in dataInjections)
				{
					content.Replace(data.Key, data.Value);
				}
			}
			return content.ToString();
		}
	}
}
