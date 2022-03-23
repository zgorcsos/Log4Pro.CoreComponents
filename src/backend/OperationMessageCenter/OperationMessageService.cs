using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Log4Pro.CoreComponents.Caching;
using Log4Pro.CoreComponents.OperationMessageCenter.DAL;
using Log4Pro.CoreComponents.OperationMessageCenter.Internals;
using Log4Pro.CoreComponents.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Log4Pro.CoreComponents.OperationMessageCenter
{
    /// <summary>
    /// Operation message center.
    /// This is an Singletone DI service.
    /// </summary>
    public class OperationMessageService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly Cache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationMessageCenter"/> class.
        /// This is an Singletone DI service. Never instantiate it manually!
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="scopeFactory">The scope factory.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="cache">The cache.</param>
        public OperationMessageService(IConfiguration configuration, IServiceScopeFactory scopeFactory, IServiceProvider serviceProvider, Cache cache)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _serviceProvider = serviceProvider;
            _cache = cache;
            using (var scope = _scopeFactory.CreateScope())
            {
                OperationMessageCenterContext db = GetDbContext(scope);
            }
        }

        #region Internal API

        /// <summary>
        /// Adds a message to operation message center (storage, cache, and subscribe layer)
        /// 
        /// </summary>
        /// <param name="entry">Operation message entry</param>
        /// <returns>true, ha sikeres a művelet</returns>
        internal Task<bool> AddMessageAsync(OperationMessageEntry entry)
        {
            return Task.Run<bool>(() => AddMessage(entry));
        }

        /// <summary>
        /// Hozzáad egy üzenetet az üzenetközponthoz
        /// </summary>
        /// <param name="entry">Üzenet</param>
        /// <returns>true, ha sikeres a művelet</returns>
        internal bool AddMessage(OperationMessageEntry entry)
        {
            _lock.Wait();
            try
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        OperationMessageCenterContext db = GetDbContext(scope);
                        var messageEntity = new OperationMessage()
                        {
                            Instance = entry.Instance,
                            Message = entry.Message,
                            MessageCategory = entry.MessageCategory,
                            Module = entry.Module,
                            OtherFilter = string.IsNullOrEmpty(entry.OtherFilter) ? null : entry.OtherFilter,
                            TimeStamp = DateTime.UtcNow,
                            Thread = entry.Thread,
                        };
                        db.OperationMessages.Add(messageEntity);
                        foreach (var additionalData in entry.AdditionalDatas)
                        {
                            var additionalDataEntity = new AdditionalMessageData()
                            {
                                DataKey = additionalData.Key,
                                DataValue = additionalData.Value,
                                OperationMessage = messageEntity,
                            };
                            db.AdditionalMessageDatas.Add(additionalDataEntity);
                        }
                        db.SaveChanges();
                    }
                    return true;
                }
                catch (Exception)
                {
                    try
                    {
                    }
                    catch (Exception)
                    {
                    }
                    return false;
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Gets the database context.
		/// In unit test environments provides the SQLite memory db.
		/// In NOT unit test environment provides SQL server db.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        internal OperationMessageCenterContext GetDbContext(IServiceScope scope)
        {
            if (_serviceProvider.GetService(typeof(UnitTestEnvironment)) != null)
            {
                return scope.ServiceProvider.GetRequiredService<OperationMessageCenterContextSQLite>();
            }
            else
            {
                return scope.ServiceProvider.GetRequiredService<OperationMessageCenterContext>();
            }
        }

        #endregion Internal API

        #region Static parts

        /// <summary>
        /// Gets the application setting configuration.
		/// This is a static part, 'cause use by ContextFactory.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>Application setting of modul</returns>
        internal static OperationMessageCenterAppSettings GetAppSettingConfiguration(IConfiguration configuration)
        {
            var config = configuration.GetSection(OperationMessageService.APPSETTINGS_SECTION_NAME).Get<OperationMessageCenterAppSettings>();
            if (config == null)
            {
                config = new OperationMessageCenterAppSettings();
            }
            if (string.IsNullOrEmpty(config.UsedConnectionString))
            {
                config.UsedConnectionString = OperationMessageService.DEFAULT_CONNECTIONSTRING;
            }
            return config;
        }

        #endregion Static parts

        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        internal const string APPSETTINGS_SECTION_NAME = nameof(OperationMessageCenter);
        internal const string DEFAULT_CONNECTIONSTRING = "DefaultConnection";
    }
}
