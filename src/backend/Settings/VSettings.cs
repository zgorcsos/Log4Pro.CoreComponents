using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Log4Pro.CoreComponents.Caching;
using Log4Pro.CoreComponents.Settings.DAL;
using Log4Pro.CoreComponents.Settings.Internals;
using Log4Pro.CoreComponents.UnitTesting;

namespace Log4Pro.CoreComponents.Settings
{
    /// <summary>
    /// Setting implementation
    /// </summary>
    public class VSettings
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly Cache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="VSettings"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="scopeFactory">The scope factory.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="cache">The cache.</param>
        public VSettings(IConfiguration configuration, ILogger<VSettings> logger, IServiceScopeFactory scopeFactory, IServiceProvider serviceProvider, Cache cache)
        {
            _configuration = configuration;
            _logger = logger ?? NullLogger<VSettings>.Instance;
            _scopeFactory = scopeFactory;
            _serviceProvider = serviceProvider;
            _cache = cache;
        }

        #region API

        /// <summary>
        /// Gets the setting.
        /// </summary>
        /// <typeparam name="TSettingValue">The type of the settings value.</typeparam>
        /// <typeparam name="TSetting">The type of the settings.</typeparam>
        /// <param name="instanceKey">The instance key.</param>
        /// <returns></returns>
        public TSettingValue GetSettingValue<TSettingValue, TSetting>(string instanceKey = null)
        {
            _lock.Wait();
            try
            {
                return GetSettingValue<TSettingValue>(typeof(TSetting), instanceKey);
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Stores the setting value.
        /// </summary>
        /// <typeparam name="TSettingValue">The type of the setting value.</typeparam>
        /// <typeparam name="TSetting">The type of the setting.</typeparam>
        /// <param name="settingValue">The setting value.</param>
        /// <param name="instanceKey">The instance key.</param>
        /// <param name="changer">The changer.</param>
        public void StoreSettingValue<TSettingValue, TSetting>(TSettingValue settingValue, string instanceKey = null, string changer = null)
        {
            _lock.Wait();
            try
            {
                StoreSettingValue(typeof(TSetting), settingValue, instanceKey, changer);
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Store the setting value from an ui request.
        /// </summary>
        /// <param name="definingTypeId">This type defines the setting</param>
        /// <param name="settingTypeId">This type defines the type of setting value</param>
        /// <param name="jsonValue">This json string represents the setted value</param>
        /// <param name="instance">Instance id</param>
        /// <param name="changer">Setting changer</param>
        public void StoreSettingValueFromUI(string definingTypeId, string settingTypeId, string jsonValue, string instance, string changer)
        {
            var definingType = Type.GetType(definingTypeId);
            var settingType = Type.GetType(settingTypeId);
            MethodInfo jsonConvertMethod = typeof(JsonConvert).GetMethod(nameof(JsonConvert.DeserializeObject), 1, new Type[] { typeof(string) });
            MethodInfo jsonConvertGeneric = jsonConvertMethod.MakeGenericMethod(settingType);
            var value = jsonConvertGeneric.Invoke(null, new object[] { jsonValue });
            MethodInfo storeMethod = this.GetType().GetMethod(nameof(StoreSettingValue), BindingFlags.Public | BindingFlags.Instance);
            MethodInfo storeGeneric = storeMethod.MakeGenericMethod(settingType, definingType);
            storeGeneric.Invoke(this, new object[] { value, instance, changer });
        }

        /// <summary>
        /// Initializes me.
        /// </summary>
        /// <typeparam name="TSettingHolderType">The type of the setting holder type.</typeparam>
        /// <param name="instanceKey">The instance key.</param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void InitializeMe<TSettingHolderType>(string instanceKey = null)
        {
            _lock.Wait();
            try
            {
                var moduleKeyAttribute = typeof(TSettingHolderType).GetCustomAttributes<ModuleKeyAttribute>().FirstOrDefault();
                if (moduleKeyAttribute == null)
                {
                    throw new ArgumentException($"{nameof(ModuleKeyAttribute)} is mandantory for setting holder type!");
                }
                var holderDescriptionAttribute = typeof(TSettingHolderType).GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
                if (holderDescriptionAttribute == null)
                {
                    throw new ArgumentException($"{nameof(DescriptionAttribute)} is mandantory for setting holder type!");
                }
                foreach (var item in typeof(TSettingHolderType).GetNestedTypes())
                {
                    InitNestedType(item, instanceKey);
                }
                DeleteAllRemovedSettings(typeof(TSettingHolderType), moduleKeyAttribute.ModuleKey, instanceKey);
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Get the full setting tree from present senting defination for UI
        /// </summary>
        /// <param name="wantedModules">Only this moduls's settings</param>
        /// <param name="wantedInstances">Only this instances</param>
        /// <returns>Full tree of settings</returns>
        public List<SettingNode> BuildAllSettingTree(List<string> wantedModules = null, List<string> wantedInstances = null)
        {
            var tree = new List<SettingNode>();
            var allPresentSettingType = GetAllSettingDefinationType();
            if (wantedModules != null)
            {
                allPresentSettingType = allPresentSettingType.Where(x => wantedModules.Contains(x.GetModuleName()));
            }
            foreach (var settingDeclaringType in allPresentSettingType.OrderBy(x => x.GetTitle()))
            {
                var typeStartNode = new SettingNode { Title = settingDeclaringType.GetTitle(), Description = settingDeclaringType.GetDescription(), Childrens = new List<SettingNode>() };
                var instances = GetMyInstances(settingDeclaringType.GetModuleName());
                if (instances.Count() > 0)
                {
                    if (wantedInstances != null)
                    {
                        instances = instances.Where(x => wantedInstances.Contains(x));
                    }
                    foreach (var instance in instances.OrderBy(x => x))
                    {
                        var instanceNode = new SettingNode { Title = instance, Childrens = new List<SettingNode>() };
                        instanceNode.Childrens = BuildMySettingTree(settingDeclaringType, instance).OrderBy(x => x.Title).ToList();
                        typeStartNode.Childrens.Add(instanceNode);
                    }
                }
                else
                {
                    typeStartNode.Childrens = BuildMySettingTree(settingDeclaringType).OrderBy(x => x.Title).ToList();
                }
                tree.Add(typeStartNode);
            }
            return tree;
        }

        #endregion API

        #region Internal API

        internal SettingContext GetDbContext(IServiceScope scope)
        {
            if (_serviceProvider.GetService(typeof(UnitTestEnvironment)) != null)
            {
                return scope.ServiceProvider.GetRequiredService<SettingContextSQLite>();
            }
            else
            {
                return scope.ServiceProvider.GetRequiredService<SettingContext>();
            }
        }

        internal static VSettingsAppSettings GetAppSettingConfiguration(IConfiguration configuration)
        {
            var config = configuration.GetSection(VSettings.APPSETTINGS_SECTION_NAME).Get<VSettingsAppSettings>();
            if (config == null)
            {
                config = new VSettingsAppSettings();
            }
            if (string.IsNullOrEmpty(config.UsedConnectionString))
            {
                config.UsedConnectionString = VSettings.DEFAULT_CONNECTIONSTRING;
            }
            return config;
        }

        internal TSettingValue GetSettingValueFromCache<TSettingValue>(SettingAddress address)
        {
            return _cache.Read<TSettingValue>($"{nameof(VSettings)}:{address.ToString()}");
        }

        internal SettingAddress GetAddress(Type setting, string instanceOrUserKey)
        {
            SettingAddress address = new()
            {
                ModuleKey = setting.GetModuleName(),
                Key = setting.GetSettingKey(),
                InstanceOrUserKey = instanceOrUserKey,
            };
            return address;
        }

        internal TSettingValue GetSettingValueFromDb<TSettingValue>(SettingAddress address)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                SettingContext db = GetDbContext(scope);
                return JsonConvert.DeserializeObject<TSettingValue>(GetSettingFromDb(address, db).Value);
            }
        }

        internal IEnumerable<Type> GetAllSettingDefinationType()
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where Attribute.IsDefined(type, typeof(ModuleKeyAttribute))
                    select type).ToList();
        }

        internal IEnumerable<string> GetMyInstances(string module)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                SettingContext db = GetDbContext(scope);
                return db.Settings.Where(x => x.ModuleKey == module && !x.UserLevelSettings && x.InstanceOrUserKey != null)
                    .GroupBy(x => x.InstanceOrUserKey).Select(x => x.Key).ToList();
            }
        }

        internal IEnumerable<SettingNode> BuildMySettingTree(Type settingType, string instance = null, SettingNode me = null)
        {
            foreach (var nestedType in settingType.GetNestedTypes())
            {
                var node = new SettingNode()
                {
                    Title = nestedType.GetTitle(),
                    Description = nestedType.GetDescription(),
                };
                if (nestedType.GetSettingType() != null)
                {
                    var settingValueType = nestedType.GetSettingType();
                    var settingAddress = GetAddress(nestedType, instance);
                    node.Me = new SettingViewModel()
                    {
                        UniqueId = settingAddress.ToString(),
                        Instance = settingAddress.InstanceOrUserKey,
                        DataType = settingValueType.Name,
                        TypeId = settingValueType.AssemblyQualifiedName,
                        DefiningTypeId = nestedType.AssemblyQualifiedName,
                        SensitiveData = nestedType.Sensitive(),
                    };
                    if (!node.Me.SensitiveData)
                    {
                        // Cache side value
                        try
                        {
                            MethodInfo method = this.GetType().GetMethod(nameof(GetSettingValueFromCache), BindingFlags.NonPublic | BindingFlags.Instance);
                            MethodInfo generic = method.MakeGenericMethod(nestedType.GetSettingType());
                            node.Me.CachedValue = generic.Invoke(this, new object[] { settingAddress });
                        }
                        catch (Exception)
                        {
                            node.Me.CachedValue = null;
                        }
                        // Db side value
                        try
                        {
                            MethodInfo method = this.GetType().GetMethod(nameof(GetSettingValueFromDb), BindingFlags.NonPublic | BindingFlags.Instance);
                            MethodInfo generic = method.MakeGenericMethod(nestedType.GetSettingType());
                            node.Me.PersistedValue = generic.Invoke(this, new object[] { settingAddress });
                        }
                        catch (Exception)
                        {
                            node.Me.PersistedValue = null;
                        }
                        // default value
                        MethodInfo defaultMethod = this.GetType().GetMethod(nameof(GetDefaultValue), BindingFlags.NonPublic | BindingFlags.Instance);
                        MethodInfo defaultGeneric = defaultMethod.MakeGenericMethod(nestedType.GetSettingType());
                        node.Me.DefaultValue = defaultGeneric.Invoke(this, new object[] { nestedType });
                        node.Me.Options = nestedType.GetSettingSelections();
                    }
                    else
                    {
                        Random rnd = new Random();
                        var sensitiveDataValue = new string('*', rnd.Next(7, 13));
                        node.Me.PersistedValue = sensitiveDataValue;
                        node.Me.CachedValue = sensitiveDataValue;
                        node.Me.DefaultValue = sensitiveDataValue;
                    }
                }
                node.Childrens = BuildMySettingTree(nestedType, instance, node).OrderBy(x => x.Title).ToList();
                yield return node;
            }
        }

        #endregion Internal API

        internal TSettingValue GetDefaultValue<TSettingValue>(Type setting)
        {
            return setting.GetDefaultValue<TSettingValue>();
        }

        private TSettingValue GetSettingValue<TSettingValue>(Type settingDefinition, string instanceKey = null)
        {
            TSettingValue value = default;
            var address = GetAddress(settingDefinition, instanceKey);
            try
            {
                try
                {
                    value = GetSettingValueFromCache<TSettingValue>(address);
                }
                catch (Caching.NotFoundException)
                {
                    value = GetSettingValueFromDb<TSettingValue>(address);
                }
            }
            catch (NotFoundException)
            {
                value = settingDefinition.GetDefaultValue<TSettingValue>();
                StoreSettingValue(settingDefinition, value, instanceKey, "byC0DE:FirstUse");
            }
            return value;
        }

        private Setting GetSettingFromDb(SettingAddress address, SettingContext db)
        {
            var setting = db.Settings.FirstOrDefault(x => x.ModuleKey == address.ModuleKey
                                                        && x.InstanceOrUserKey == address.InstanceOrUserKey
                                                        && x.Key == address.Key);
            if (setting == null)
            {
                throw new NotFoundException(address.ToString());
            }
            return setting;
        }

        private void InitNestedType(Type nestedType, string instanceKey)
        {
            var typeDescriptionAttribute = nestedType.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
            var typeTitleAttribute = nestedType.GetCustomAttributes<TitleAttribute>().FirstOrDefault();
            if (typeDescriptionAttribute == null || typeTitleAttribute == null)
            {
                throw new ArgumentException($"{nameof(DescriptionAttribute)} or/and {nameof(TitleAttribute)} is mandantory for all setting type!");
            }
            var settingTypeAttribute = nestedType.GetCustomAttributes<SettingTypeAttribute>().FirstOrDefault();
            if (settingTypeAttribute != null) // if null: this is a node only
            {
                // this is a settings
                var settingType = settingTypeAttribute.Type;
                var address = GetAddress(nestedType, instanceKey);
                object defaultValue = null;
                MethodInfo defaultMethod = this.GetType().GetMethod(nameof(GetDefaultValue), BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo defaultGeneric = defaultMethod.MakeGenericMethod(nestedType.GetSettingType());
                defaultValue = defaultGeneric.Invoke(this, new object[] { nestedType });
                string defaultValueAsJsonString = JsonConvert.SerializeObject(defaultValue);
                var settingData = new SettingData()
                {
                    SettingAddress = address,
                    Title = nestedType.GetTitle(),
                    Description = nestedType.GetDescription(),
                    DefaultValue = defaultValue,
                    DefaultValueAsJsonString = defaultValueAsJsonString,
                    Version = nestedType.GetVersion(),
                };
                using (var scope = _scopeFactory.CreateScope())
                {
                    SettingContext db = GetDbContext(scope);
                    var existingSetting = db.Settings.FirstOrDefault(x => x.ModuleKey == address.ModuleKey && x.InstanceOrUserKey == address.InstanceOrUserKey && x.Key == address.Key);
                    if (existingSetting == null || existingSetting.Version != settingData.Version)
                    {
                        if (existingSetting != null)
                        {
                            existingSetting.Description = settingData.Description;
                            existingSetting.Title = settingData.Title;
                            existingSetting.Value = settingData.DefaultValueAsJsonString;
                            existingSetting.Version = settingData.Version;
                            var modify = new ModifySetting
                            {
                                ModuleKey = existingSetting.ModuleKey,
                                InstanceOrUserKey = existingSetting.InstanceOrUserKey,
                                SettingKey = existingSetting.Key,
                                Changer = "SYS_INIT",
                                From = existingSetting.Value,
                                TimeStamp = DateTime.UtcNow,
                                To = settingData.DefaultValueAsJsonString,
                                Version = settingData.Version,
                            };
                            db.SettingHistories.Add(modify);
                        }
                        else
                        {
                            var dbSetting = new Setting()
                            {
                                Description = settingData.Description,
                                Title = settingData.Title,
                                ModuleKey = settingData.SettingAddress.ModuleKey,
                                InstanceOrUserKey = settingData.SettingAddress.InstanceOrUserKey,
                                Key = settingData.SettingAddress.Key,
                                Value = settingData.DefaultValueAsJsonString,
                                Version = settingData.Version,
                            };
                            db.Settings.Add(dbSetting);
                            var create = new CreateSetting()
                            {
                                ModuleKey = settingData.SettingAddress.ModuleKey,
                                InstanceOrUserKey = settingData.SettingAddress.InstanceOrUserKey,
                                SettingKey = settingData.SettingAddress.Key,
                                TimeStamp = DateTime.UtcNow,
                                To = dbSetting.Value,
                                Changer = "SYS_INIT",
                                Version = settingData.Version,
                            };
                            db.SettingHistories.Add(create);
                        }
                        db.SaveChanges();
                        _cache.Publish($"{nameof(VSettings)}:{address.ToString()}", settingData.DefaultValue);
                    }
                }
            }
            foreach (var item in nestedType.GetNestedTypes())
            {
                InitNestedType(item, instanceKey);
            }
        }

        private void DeleteAllRemovedSettings(Type settingHoderType, string moduleKey, string instanceKey)
        {
            var existingSettingsUnderHolderType = GetMySettingsName(settingHoderType);
            var deletes = new List<DeleteSetting>();
            using (var scope = _scopeFactory.CreateScope())
            {
                SettingContext db = GetDbContext(scope);
                var removedSettings = db.Settings.Where(x => x.ModuleKey == moduleKey && x.InstanceOrUserKey == instanceKey
                                                                && !existingSettingsUnderHolderType.Contains(x.Key));
                foreach (var removed in removedSettings)
                {
                    deletes.Add(new DeleteSetting()
                    {
                        Changer = "SYS_INIT",
                        Description = removed.Description,
                        Title = removed.Title,
                        From = removed.Value,
                        InstanceOrUserKey = removed.InstanceOrUserKey,
                        ModuleKey = removed.ModuleKey,
                        SettingKey = removed.Key,
                        TimeStamp = DateTime.UtcNow,
                        Version = removed.Version,
                    });
                    db.SettingHistories.Where(x => x.ModuleKey == removed.ModuleKey && x.InstanceOrUserKey == removed.InstanceOrUserKey && x.SettingKey == removed.Key).DeleteFromQuery();
                    db.Settings.Where(x => x.Id == removed.Id).DeleteFromQuery();
                    var a = new SettingAddress()
                    {
                        ModuleKey = removed.ModuleKey,
                        InstanceOrUserKey = removed.InstanceOrUserKey,
                        Key = removed.Key,
                    };
                    _cache.RemoveFromCache(a.ToString());
                }
                db.SettingHistories.AddRange(deletes);
                db.SaveChanges();
            }
        }

        private IEnumerable<string> GetMySettingsName(Type settingType, string treePrefix = "")
        {
            var names = new List<string>();
            foreach (var type in settingType.GetNestedTypes())
            {
                if (type.GetCustomAttributes<SettingTypeAttribute>().FirstOrDefault() != null)
                {
                    names.Add(string.IsNullOrEmpty(treePrefix) ? type.Name : $"{treePrefix}:{type.Name}");
                }
                names.AddRange(GetMySettingsName(type, string.IsNullOrEmpty(treePrefix) ? type.Name : $"{treePrefix}:{type.Name}"));
            }
            return names;
        }

        private void StoreSettingValue<TSettingValue>(Type settingDefinition, TSettingValue settingValue, string instanceKey, string changer)
        {
            var address = GetAddress(settingDefinition, instanceKey);
            var settingData = new SettingData()
            {
                SettingAddress = address,
                Description = settingDefinition.GetDescription(),
                Title = settingDefinition.GetTitle(),
                DefaultValue = settingDefinition.GetDefaultValue<TSettingValue>(),
                DefaultValueAsJsonString = JsonConvert.SerializeObject(settingDefinition.GetDefaultValue<TSettingValue>()),
                Version = settingDefinition.GetVersion(),
            };
            using (var scope = _scopeFactory.CreateScope())
            {
                SettingContext db = GetDbContext(scope);
                Setting dbSetting = null;
                try
                {
                    dbSetting = GetSettingFromDb(address, db);
                    var modify = new ModifySetting()
                    {
                        ModuleKey = settingData.SettingAddress.ModuleKey,
                        InstanceOrUserKey = settingData.SettingAddress.InstanceOrUserKey,
                        SettingKey = settingData.SettingAddress.Key,
                        Changer = changer,
                        From = dbSetting.Value,
                        TimeStamp = DateTime.UtcNow,
                        To = JsonConvert.SerializeObject(settingValue),
                        Version = settingData.Version,
                    };
                    dbSetting.Value = JsonConvert.SerializeObject(settingValue);
                    dbSetting.Version = settingData.Version;
                    db.SettingHistories.Add(modify);
                }
                catch (NotFoundException)
                {
                    dbSetting = new Setting()
                    {
                        Description = settingData.Description,
                        Title = settingData.Title,
                        ModuleKey = settingData.SettingAddress.ModuleKey,
                        InstanceOrUserKey = settingData.SettingAddress.InstanceOrUserKey,
                        Key = settingData.SettingAddress.Key,
                        Value = JsonConvert.SerializeObject(settingValue),
                        Version = settingData.Version,
                    };
                    db.Settings.Add(dbSetting);
                    var create = new CreateSetting()
                    {
                        ModuleKey = settingData.SettingAddress.ModuleKey,
                        InstanceOrUserKey = settingData.SettingAddress.InstanceOrUserKey,
                        SettingKey = settingData.SettingAddress.Key,
                        TimeStamp = DateTime.UtcNow,
                        To = dbSetting.Value,
                        Changer = changer,
                        Version = settingData.Version,
                    };
                    db.SettingHistories.Add(create);
                }
                db.SaveChanges();
            }
            _cache.Publish($"{nameof(VSettings)}:{address.ToString()}", settingValue);
        }

        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        internal const string APPSETTINGS_SECTION_NAME = "VSettings";
        internal const string DEFAULT_CONNECTIONSTRING = "DefaultConnection";
    }
}
