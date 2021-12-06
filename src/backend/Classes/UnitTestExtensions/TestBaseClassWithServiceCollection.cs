using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.Classes.UnitTestExtensions
{
	/// <summary>
	/// Base class for easy injectable in unit tests 
	/// </summary>
	public class TestBaseClassWithServiceCollection : IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TestBaseClassWithServiceCollection"/> class.
		/// </summary>
		/// <param name="serviceProvider">The service provider.</param>
		public TestBaseClassWithServiceCollection(IServiceProvider serviceProvider)
		{
			var sp = serviceProvider;
			foreach (var service in sp.GetRequiredService<IServiceCollection>())
			{
				if (service.ServiceType != typeof(IServiceCollection))
				{
					lock (_locker)
					{
						_serviceCollection.Add(service);
					}
				}
			}
		}

		/// <summary>
		/// Adds the service.
		/// </summary>
		/// <typeparam name="TServiceType">The type of the service type.</typeparam>
		/// <typeparam name="TImplementationType">The type of the implementation type.</typeparam>
		/// <param name="lifeTime">The life time.</param>
		/// <param name="implementationInstance">The implementation instance.</param>
		protected void AddService<TServiceType, TImplementationType>(ServiceLifetime lifeTime = ServiceLifetime.Singleton, TImplementationType implementationInstance = null)
			where TServiceType : class
			where TImplementationType : class, TServiceType
		{
			lock (_locker)
			{
				switch (lifeTime)
				{
					case ServiceLifetime.Singleton:
						if (implementationInstance != null)
						{
							_serviceCollection.AddSingleton(implementationInstance);
						}
						else
						{
							_serviceCollection.AddSingleton<TServiceType, TImplementationType>();
						}						  
						break;
					case ServiceLifetime.Scoped:
						if (implementationInstance != null)
						{
							Func<IServiceProvider, TImplementationType> func = (e) => implementationInstance; 
							_serviceCollection.AddScoped<TServiceType, TImplementationType>(func);
						}
						else
						{
							_serviceCollection.AddScoped<TServiceType, TImplementationType>();
						}
						break;
					case ServiceLifetime.Transient:
						if (implementationInstance != null)
						{
							Func<IServiceProvider, TImplementationType> func = (e) => implementationInstance;
							_serviceCollection.AddTransient<TServiceType, TImplementationType>(func);
						}
						else
						{
							_serviceCollection.AddTransient<TServiceType, TImplementationType>();
						}
						break;
					default:
						break;
				}
			}
		}

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <typeparam name="TService">The type of the service.</typeparam>
		/// <returns></returns>
		protected TService GetService<TService>()
		{
			lock (_locker)
			{
				try
				{
					return _serviceProvider.GetRequiredService<TService>();
				}
				catch
				{
					_serviceProvider = _serviceCollection.BuildServiceProvider(true);
					return _serviceProvider.GetRequiredService<TService>();
				}
			}
		}

		/// <summary>
		/// The service collection
		/// </summary>
		protected IServiceCollection _serviceCollection = new ServiceCollection();

		private IServiceProvider _serviceProvider;

		private bool disposedValue;
		private readonly object _locker = new();

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// dispose managed state (managed objects)
					foreach (var disposableService in _serviceCollection.Where(x => x is IDisposable))
					{
						(disposableService as IDisposable).Dispose();
					}
				}

				// - free unmanaged resources (unmanaged objects) and override finalizer
				// - set large fields to null
				disposedValue = true;
			}
		}

		// // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~TestClassWithServiceCollection()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
