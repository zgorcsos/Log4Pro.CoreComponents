using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Log4Pro.CoreComponents.ContentInjector;
using Log4Pro.CoreComponents.Hosting;
using Log4Pro.CoreComponents.Classes.UnitTestExtensions;
using Log4Pro.CoreComponents;
using Xunit;

namespace Log4Pro.CoreComponents.Test.ContentInjector
{
	public class ContentInjectorUnitTest : TestBaseClassWithServiceCollection
	{
		public ContentInjectorUnitTest(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			var mockEnvironment = new Mock<IWebHostEnvironment>();
			mockEnvironment.Setup(m => m.ContentRootPath).Returns("");
			AddService<IWebHostEnvironment, IWebHostEnvironment>(ServiceLifetime.Transient, mockEnvironment.Object);
			_serviceCollection = _serviceCollection.AddContentInjector();
		}

		[Fact(DisplayName = "AddContentInjector IServiceCollection extension to add the service works well.")]
		public void RegisterExtensionWork()
		{
			Assert.NotNull(GetService<FileContentInjector>());
			Assert.Single(_serviceCollection.Where(x => x.ServiceType == typeof(FileContentInjector) && x.ImplementationType == typeof(FileContentInjector)));
		}

		[Fact(DisplayName = "AddContentInjector IServiceCollection extension adds the needed service dependency too.")]
		public void RegisterExtensionRegistersDependenciesToo()
		{
			Assert.NotNull(GetService<IHostProvider>());
		}

		[Fact(DisplayName = "AddContentInjector IServiceCollection extension not duplicates the service, when happens multiple call to the extension method.")]
		public void ProtectDuplicate()
		{
			Assert.NotNull(GetService<FileContentInjector>());
			_serviceCollection = _serviceCollection.AddContentInjector();
			Assert.Single(_serviceCollection.Where(x => x.ServiceType == typeof(FileContentInjector) && x.ImplementationType == typeof(FileContentInjector)));
		}

		[Fact(DisplayName = "The registered service instance is singletone.")]
		public void IsSingleton()
		{
			Assert.Same(GetService<FileContentInjector>(), GetService<FileContentInjector>());
		}

		[Fact(DisplayName = "Inject method reads and provides the file content well.")]
		public void GetFileContentWork()
		{
			Assert.Equal("{key1}:{key2}", GetService<FileContentInjector>().Inject("Test.txt"));
		}

		[Fact(DisplayName = "The built in key-value changer solution in Inject method works well.")]
		public void GetFileContentAndChangeKeysWork()
		{
			var d = new Dictionary<string, string> { { "{key1}", "1" }, { "{key2}", "2" } };
			Assert.Equal("1:2", GetService<FileContentInjector>().Inject("Test.txt", d));
		}
	}
}
