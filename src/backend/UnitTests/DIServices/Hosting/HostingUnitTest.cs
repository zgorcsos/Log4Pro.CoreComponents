using System;
using Xunit;
using Log4Pro.CoreComponents.Hosting;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Log4Pro.CoreComponents.Classes.UnitTestExtensions;
using Log4Pro.CoreComponents;
using System.Linq;

namespace Log4Pro.CoreComponents.Test.Hosting
{
	public class HostingUnitTest : TestBaseClassWithServiceCollection
	{
		const string PATH_START = "C:\\";
		const string FILE_NAME = "test.txt";

		public HostingUnitTest(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			var mockEnvironment = new Mock<IWebHostEnvironment>();
			mockEnvironment.Setup(m => m.ApplicationName).Returns(nameof(HostingUnitTest));
			mockEnvironment.Setup(m => m.EnvironmentName).Returns(nameof(HostingUnitTest));
			mockEnvironment.Setup(m => m.ContentRootPath).Returns(PATH_START);
			mockEnvironment.Setup(m => m.WebRootPath).Returns(PATH_START);
			AddService<IWebHostEnvironment, IWebHostEnvironment>(ServiceLifetime.Singleton, mockEnvironment.Object);
			_serviceCollection = _serviceCollection.AddHostProvider<HostProvider>();
		}

		[Fact(DisplayName = "AddHostProvider IServiceCollection extension to add the service works well.")]
		public void RegisterExtensionWork()
		{
			Assert.NotNull(GetService<IHostProvider>());
			Assert.Single(_serviceCollection.Where(x => x.ServiceType == typeof(IHostProvider) && x.ImplementationType == typeof(HostProvider)));
		}

		[Fact(DisplayName = "AddHostProvider IServiceCollection extension not duplicates the service, when happens multiple call to the extension method.")]
		public void ProtectDuplicate()
		{
			Assert.NotNull(GetService<IHostProvider>());
			_serviceCollection = _serviceCollection.AddHostProvider<HostProvider>();
			Assert.Single(_serviceCollection.Where(x => x.ServiceType == typeof(IHostProvider) && x.ImplementationType == typeof(HostProvider)));
		}

		[Fact(DisplayName = "The registered service instance is singletone.")]
		public void IsSingleton()
		{
			Assert.Same(GetService<IHostProvider>(), GetService<IHostProvider>());
		}

		[Fact(DisplayName = "ApplicationName property works well.")]
		public void GetApplicationName()
		{
			Assert.Equal(nameof(HostingUnitTest), GetService<IHostProvider>().ApplicationName);			
		}

		[Fact(DisplayName = "UsedEnvironment property works well.")]
		public void GetUsedEnvironment()
		{
			Assert.Equal(nameof(HostingUnitTest), GetService<IHostProvider>().UsedEnvironment);
		}

		[Fact(DisplayName = "ContentAbsolutePath function works well.")]
		public void ContentPathWork()
		{
			Assert.Equal(PATH_START, GetService<IHostProvider>().ContentAbsolutePath(null));
			Assert.Equal(PATH_START, GetService<IHostProvider>().ContentAbsolutePath(""));
			Assert.Equal(Path.Combine(PATH_START, FILE_NAME), GetService<IHostProvider>().ContentAbsolutePath(FILE_NAME));
		}

		[Fact(DisplayName = "WebRootAbsolutePath function works well.")]
		public void WebPathWork()
		{
			Assert.Equal(PATH_START, GetService<IHostProvider>().WebRootAbsolutePath(null));
			Assert.Equal(PATH_START, GetService<IHostProvider>().WebRootAbsolutePath(""));
			Assert.Equal(Path.Combine(PATH_START, FILE_NAME), GetService<IHostProvider>().WebRootAbsolutePath(FILE_NAME));
		}
	}
}
