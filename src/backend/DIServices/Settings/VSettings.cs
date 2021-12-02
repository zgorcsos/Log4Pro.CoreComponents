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
using Vrh.DIServices.Caching;
using Vrh.DIServices.Settings.DAL;
using Vrh.DIServices.Settings.Internals;
using Vrh.DIServices.UnitTesting;

namespace Vrh.DIServices.Settings
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
		/// <typeparam name="TSettingsValue">The type of the settings value.</typeparam>
		/// <typeparam name="TSettings">The type of the settings.</typeparam>
		/// <param name="instanceKey">The instance key.</param>
		/// <returns></returns>
		public TSettingsValue GetSettingValue<TSettingsValue, TSettings>(string instanceKey = null)
		{
			_lock.Wait();
			try
			{
				return GetSettingValue<TSettingsValue>(typeof(TSettings), instanceKey);
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

		#endregion Internal API

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
			if (typeDescriptionAttribute == null)
			{
				throw new ArgumentException($"{nameof(DescriptionAttribute)} is mandantory for all setting type!");
			}
			var settingTypeAttribute = nestedType.GetCustomAttributes<SettingTypeAttribute>().FirstOrDefault();
			if (settingTypeAttribute != null) // if null: this is a node only
			{
				// this is a settings
				var settingType = settingTypeAttribute.Type;
				var address = GetAddress(nestedType, instanceKey);
				var defaultValueAattribute = nestedType.GetCustomAttributes<DefaultValueAttribute>().FirstOrDefault();
				object defaultValue = null;
				string defaultValueAsJsonString = null;
				if (defaultValueAattribute != null)
				{
					defaultValueAsJsonString = JsonConvert.SerializeObject(defaultValueAattribute.DefaultValue);
				}
				else
				{
					if (defaultValue != null && defaultValue.GetType().IsEnum)
					{
						var enumValues = defaultValue.GetType().GetFields(BindingFlags.Public | BindingFlags.Static);
						foreach (var enumValue in enumValues)
						{
							var isDefaultAttribute = enumValue.GetCustomAttribute<IsDefaultAttribute>();
							if (isDefaultAttribute != null)
							{
								var ev = Enum.Parse(defaultValue.GetType(), enumValue.Name);
								defaultValueAsJsonString = JsonConvert.SerializeObject(ev);
							}
						}
						if (defaultValueAsJsonString == null)
						{
							defaultValueAsJsonString = JsonConvert.SerializeObject(Enum.ToObject(defaultValue.GetType(), 0));
						}
					}
					else
					{
						var getter = nestedType.GetMethod("GetDefaultValue");
						if (getter != null)
						{
							if (getter.IsStatic)
							{
								defaultValueAsJsonString = JsonConvert.SerializeObject(getter.Invoke(null, null));
							}
							else
							{
								defaultValueAsJsonString = JsonConvert.SerializeObject(getter.Invoke(Activator.CreateInstance(settingType, null), null));
							}
						}
						else
						{
							if (settingType.IsValueType)
							{
								defaultValueAsJsonString = JsonConvert.SerializeObject(Activator.CreateInstance(settingType));
							}
							else
							{
								defaultValueAsJsonString = JsonConvert.SerializeObject(null);
							}
						}
					}
				}
				var settingData = new SettingData()
				{
					SettingAddress = address,
					Description = nestedType.GetDescription(),
					DefaultValue = defaultValueAsJsonString,
					SettingSelections = settingType.GetSettingSelections(),
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
							existingSetting.Value = settingData.DefaultValue;
							existingSetting.DefaultValue = settingData.DefaultValue;
							existingSetting.Version = settingData.Version;
							existingSetting.Options = JsonConvert.SerializeObject(settingData.SettingSelections);
							var modify = new ModifySetting
							{
								ModuleKey = existingSetting.ModuleKey,
								InstanceOrUserKey = existingSetting.InstanceOrUserKey,
								SettingKey = existingSetting.Key,
								Changer = "SYS_INIT",
								From = existingSetting.Value,
								TimeStamp = DateTime.UtcNow,
								To = settingData.DefaultValue,
								Version = settingData.Version,
							};
							db.SettingHistories.Add(modify);
						}
						else
						{
							var dbSetting = new Setting()
							{
								DefaultValue = settingData.DefaultValue,
								Description = settingData.Description,
								ModuleKey = settingData.SettingAddress.ModuleKey,
								InstanceOrUserKey = settingData.SettingAddress.InstanceOrUserKey,
								Key = settingData.SettingAddress.Key,
								Options = JsonConvert.SerializeObject(settingData.SettingSelections),
								Value = settingData.DefaultValue,
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
				DefaultValue = JsonConvert.SerializeObject(settingDefinition.GetDefaultValue<TSettingValue>()),
				SettingSelections = settingDefinition.GetSettingSelections(),
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
						DefaultValue = settingData.DefaultValue,
						Description = settingData.Description,
						ModuleKey = settingData.SettingAddress.ModuleKey,
						InstanceOrUserKey = settingData.SettingAddress.InstanceOrUserKey,
						Key = settingData.SettingAddress.Key,
						Options = JsonConvert.SerializeObject(settingData.SettingSelections),
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
