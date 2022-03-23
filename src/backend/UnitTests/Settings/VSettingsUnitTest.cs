using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Log4Pro.CoreComponents.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Log4Pro.CoreComponents.Settings;
using Log4Pro.CoreComponents.Settings.DAL;
using Log4Pro.CoreComponents.Classes.UnitTestExtensions;
using Log4Pro.CoreComponents;
using Log4Pro.CoreComponents.Caching;
using Log4Pro.CoreComponents.Settings.Internals;
using Newtonsoft.Json;

namespace Log4Pro.CoreComponents.Test.Settings
{
	public class VSettingsUnitTest : TestBaseClassWithServiceCollection
	{
		public VSettingsUnitTest(IServiceProvider serviceProvider, IServiceCollection sc) : base(serviceProvider)
		{
			var c2 = serviceProvider;
			var mockLogger = new Mock<ILogger<Log4Pro.CoreComponents.Settings.VSettings>>();
			AddService<ILogger<Log4Pro.CoreComponents.Settings.VSettings>, ILogger<Log4Pro.CoreComponents.Settings.VSettings>>(ServiceLifetime.Transient, mockLogger.Object);
			var s = _serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration));
			_serviceCollection.Remove(s);
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();
			_serviceCollection.AddSingleton<IConfiguration>(builder.Build());
			var c = GetService<IConfiguration>();
			_serviceCollection.AddVSettings((IConfiguration)_serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration)).ImplementationInstance);
		}

		[Fact(DisplayName = "AddVSettings IServiceCollection extension to add the service works well.")]
		public void ServiceRegisterExtensionWorksWell()
		{
			var s = GetService<VSettings>();
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(VSettings));
			Assert.NotNull(s);
			Assert.IsType<VSettings>(s);
		}

		[Fact(DisplayName = "AddVSettings IServiceCollection extension adds the needed service dependency too.")]
		public void ServiceRegisterExtensionWorksWell2()
		{
			var s = GetService<Cache>();
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(Cache));
			Assert.NotNull(s);
			Assert.IsType<Cache>(s);
			Assert.Same(s, GetService<Cache>());
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(SettingContextSQLite) && x.Lifetime == ServiceLifetime.Scoped);
		}

		[Fact(DisplayName = "AddVSettings IServiceCollection extension not duplicates the service, when happens multiple call to the extension method.")]
		public void ServiceRegisterExtensionWorksWell3()
		{
			_serviceCollection.AddVSettings((IConfiguration)_serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration)).ImplementationInstance);
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(VSettings));
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(Cache));
		}

		[Fact(DisplayName = "VSettings service is singletone.")]
		public void IsSingletone()
		{
			Assert.Single(_serviceCollection, x => x.ServiceType == typeof(VSettings) && x.Lifetime == ServiceLifetime.Singleton);
			var s1 = GetService<VSettings>();
			var s2 = GetService<VSettings>();
			Assert.Same(s1, s2);
		}


		[Fact(DisplayName = "SettingAddress ToString works well.")]
		public void SettingAddressWorksWell()
		{
			SettingAddress a = new()
			{
				ModuleKey = "M",
				InstanceOrUserKey = "I",
				Key = "K",
			};
			Assert.Equal("M:I:K", a.ToString());
		}

		[Fact(DisplayName = "GetSettingKey extension method (for Type) works well.")]
		public void GetSettingKeyExtensionWorksWell()
		{
			Assert.Equal($"{nameof(TestSettingClass.UseAuthentication)}:{nameof(TestSettingClass.UseAuthentication.TechnicalUser)}",
				typeof(TestSettingClass.UseAuthentication.TechnicalUser).GetSettingKey());
		}

		[Fact(DisplayName = "GetDescription extension method (for Type) works well.")]
		public void GetDescriptionExtensionWorksWell()
		{
			Assert.Equal($"{nameof(TestSettingClass.UseAuthentication.TechnicalUser)}",
				typeof(TestSettingClass.UseAuthentication.TechnicalUser).GetDescription());
		}

		[Fact(DisplayName = "GetTitle extension method (for Type) works well.")]
		public void GetTitleExtensionWorksWell()
		{
			Assert.Equal($"{nameof(TestSettingClass.UseAuthentication.TechnicalUser)}",
				typeof(TestSettingClass.UseAuthentication.TechnicalUser).GetTitle());
		}

		[Fact(DisplayName = "GetVersion extension method (for Type) works well.")]
		public void GetVersionExtensionWorksWell()
		{
			Assert.Equal(TestSettingClass.WORKSTATIONS_VERSION,
				typeof(TestSettingClass.Workstations).GetVersion());
			Assert.Null(typeof(TestSettingClass.ReaderCheckInterval).GetVersion());
		}

		[Fact(DisplayName = "GetSettingType extension method (for Type) works well.")]
		public void GetSettingTypeExtensionWorksWell()
		{
			Assert.Equal(typeof(string), typeof(TestSettingClass.TargetSAP).GetSettingType());
			Assert.Equal(typeof(string), typeof(TestSettingClass.UseAuthentication.TechnicalUser).GetSettingType());
			Assert.Equal(typeof(List<WorkstationIdBinding>), typeof(TestSettingClass.Workstations).GetSettingType());
			Assert.Equal(typeof(int), typeof(TestSettingClass.ReaderCheckInterval).GetSettingType());
			Assert.Equal(typeof(bool), typeof(TestSettingClass.WarningForEmptyField).GetSettingType());
			Assert.Equal(typeof(ReaderType), typeof(TestSettingClass.TypeOfReader).GetSettingType());
		}

		[Fact(DisplayName = "GetDefaultValue extension method (for Type) works well.")]
		public void GetDefaultValueExtensionWorksWell()
		{
			Assert.Equal(ReaderType.Single, typeof(TestSettingClass.TypeOfReader).GetDefaultValue<ReaderType>());
			Assert.Equal(TestSettingClass.READER_CHECK_INTERVAL_DEFAULTVALUE, typeof(TestSettingClass.ReaderCheckInterval).GetDefaultValue<int>());
			Assert.Same(TestSettingClass.WORKSTATIONS_DEFAULTVALUE, typeof(TestSettingClass.Workstations).GetDefaultValue<List<WorkstationIdBinding>>());
			Assert.Equal(TestSettingClass.TARGET_SAP_DEFAULTVALUE, typeof(TestSettingClass.TargetSAP).GetDefaultValue<string>());
		}

		[Fact(DisplayName = "GetSettingSelections extension method (for Type) works well.")]
		public void GetSettingSelectionsExtensionWorksWell()
		{
			List<SettingSelection> expected = new()
			{
				new()
				{
					Title = nameof(ReaderType.Single),
					Description = nameof(ReaderType.Single),
					IsDefault = true,
					Value = ReaderType.Single.ToString(),
				},
				new()
				{
					Title = nameof(ReaderType.Multiple),
					Description = nameof(ReaderType.Multiple),
					IsDefault = false,
					Value = ReaderType.Multiple.ToString(),
				},
			};
			var actual = typeof(TestSettingClass.TypeOfReader).GetSettingSelections();
			Assert.Equal(expected[0].Title, actual.ToArray()[0].Title);
			Assert.Equal(expected[0].Description, actual.ToArray()[0].Description);
			Assert.Equal(expected[0].IsDefault, actual.ToArray()[0].IsDefault);
			Assert.Equal(expected[0].Value, actual.ToArray()[0].Value);
			Assert.Equal(expected[1].Title, actual.ToArray()[1].Title);
			Assert.Equal(expected[1].Description, actual.ToArray()[1].Description);
			Assert.Equal(expected[1].IsDefault, actual.ToArray()[1].IsDefault);
			Assert.Equal(expected[1].Value, actual.ToArray()[1].Value);
		}

		[Fact(DisplayName = "GetModuleName extension method (for Type) works well.")]
		public void GetModuleNameExtensionWorksWell()
		{
			Assert.Equal(TestSettingClass.MODULE_NAME, typeof(TestSettingClass).GetModuleName());
			Assert.Equal(TestSettingClass.MODULE_NAME, typeof(TestSettingClass.UseAuthentication.Password).GetModuleName());
		}

		[Fact(DisplayName = "GetAllDefinedSettingType extension method (for Type) works well.")]
		public void GetAllDefinedSettingTypeExtensionWorksWell()
		{
			var actual = typeof(TestSettingClass).GetAllDefinedSettingType().ToList();
			List<SettingTypesWithKey> expected = new()
			{
				new()
				{
					Key = nameof(TestSettingClass.ReaderCheckInterval),
					SettingType = typeof(TestSettingClass.ReaderCheckInterval),
				},
				new()
				{
					Key = nameof(TestSettingClass.TargetSAP),
					SettingType = typeof(TestSettingClass.TargetSAP),
				},
				new()
				{
					Key = nameof(TestSettingClass.WarningForEmptyField),
					SettingType = typeof(TestSettingClass.WarningForEmptyField),
				},
				new()
				{
					Key = nameof(TestSettingClass.Workstations),
					SettingType = typeof(TestSettingClass.Workstations),
				},
				new()
				{
					Key = nameof(TestSettingClass.UseAuthentication),
					SettingType = typeof(TestSettingClass.UseAuthentication),
				},
				new()
				{
					Key = $"{nameof(TestSettingClass.UseAuthentication)}:{nameof(TestSettingClass.UseAuthentication.TechnicalUser)}",
					SettingType = typeof(TestSettingClass.UseAuthentication.TechnicalUser),
				},
				new()
				{
					Key = $"{nameof(TestSettingClass.UseAuthentication)}:{nameof(TestSettingClass.UseAuthentication.Password)}",
					SettingType = typeof(TestSettingClass.UseAuthentication.Password),
				},
				new()
				{
					Key = nameof(TestSettingClass.TypeOfReader),
					SettingType = typeof(TestSettingClass.TypeOfReader),
				},
			};
			Assert.Equal(expected[0].Key, actual[0].Key);
			Assert.Equal(expected[0].SettingType.FullName, actual[0].SettingType.FullName);
			Assert.Equal(expected[1].Key, actual[1].Key);
			Assert.Equal(expected[1].SettingType.FullName, actual[1].SettingType.FullName);
			Assert.Equal(expected[2].Key, actual[2].Key);
			Assert.Equal(expected[2].SettingType.FullName, actual[2].SettingType.FullName);
			Assert.Equal(expected[3].Key, actual[3].Key);
			Assert.Equal(expected[3].SettingType.FullName, actual[3].SettingType.FullName);
			Assert.Equal(expected[4].Key, actual[4].Key);
			Assert.Equal(expected[4].SettingType.FullName, actual[4].SettingType.FullName);
			Assert.Equal(expected[5].Key, actual[5].Key);
			Assert.Equal(expected[5].SettingType.FullName, actual[5].SettingType.FullName);
			Assert.Equal(expected[6].Key, actual[6].Key);
			Assert.Equal(expected[6].SettingType.FullName, actual[6].SettingType.FullName);
			Assert.Equal(expected[7].Key, actual[7].Key);
			Assert.Equal(expected[7].SettingType.FullName, actual[7].SettingType.FullName);
		}

		[Fact(DisplayName = "Sensitive extension method (for Type) works well.")]
		public void SensitiveExtensionWorksWell()
		{
			Assert.True(typeof(TestSettingClass.UseAuthentication.Password).Sensitive());
			Assert.False(typeof(TestSettingClass.UseAuthentication.TechnicalUser).Sensitive());
		}

		[Fact(DisplayName = "GetSettingValueFromCache throws NotFoundException, if the required setting does not store in cache.")]
		public void GetSettingValueFromCacheTrowsNotFoundException()
		{
			var s = GetService<VSettings>();
			Assert.Throws<Log4Pro.CoreComponents.Caching.NotFoundException>(() => s.GetSettingValueFromCache<int>(s.GetAddress(typeof(TestSettingClass.ReaderCheckInterval), _testInstanceId)));
		}

		[Fact(DisplayName = "GetSettingValueFromDb throws NotFoundException, if the required setting does not store in setting database.")]
		public void GetSettingValueFromDbTrowsNotFoundException()
		{
			var s = GetService<VSettings>();
			Assert.Throws<Log4Pro.CoreComponents.Settings.NotFoundException>(() => s.GetSettingValueFromDb<int>(s.GetAddress(typeof(TestSettingClass.ReaderCheckInterval), _testInstanceId)));
		}

		[Fact(DisplayName = "GetAppSettingConfiguration works well.")]
		public void GetAppSettingConfigurationWorksWellWithService()
		{
			var configuration = (IConfiguration)_serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration)).ImplementationInstance;
			var c = VSettings.GetAppSettingConfiguration(configuration);
			Assert.Equal("A", c.UsedConnectionString);
			configuration = new ConfigurationBuilder()
				 .SetBasePath(Directory.GetCurrentDirectory())
				 .AddJsonFile("appsettings.json", true)
				 .Build();
			c = VSettings.GetAppSettingConfiguration(configuration);
			Assert.Equal("A", c.UsedConnectionString);
			configuration = new ConfigurationBuilder()
				 .SetBasePath(Directory.GetCurrentDirectory())
				 .AddJsonFile("appsettings.error.json", true)
				 .Build();
			c = VSettings.GetAppSettingConfiguration(configuration);
			Assert.Equal(VSettings.DEFAULT_CONNECTIONSTRING, c.UsedConnectionString);
		}

		[Fact(DisplayName = "GetSettingValue provide default setting value (into db, into cache & return)")]
		public void GetSettingValueProvideDefultWhenSettingIsNotInitialized()
		{
			var s = GetService<VSettings>();
			var a = s.GetAddress(typeof(TestSettingClass.UseAuthentication.TechnicalUser), _testInstanceId);
			Assert.Throws<Log4Pro.CoreComponents.Caching.NotFoundException>(() => s.GetSettingValueFromCache<int>(s.GetAddress(typeof(TestSettingClass.UseAuthentication.TechnicalUser), _testInstanceId)));
			Assert.Throws<Log4Pro.CoreComponents.Settings.NotFoundException>(() => s.GetSettingValueFromDb<int>(s.GetAddress(typeof(TestSettingClass.UseAuthentication.TechnicalUser), _testInstanceId)));
			Assert.Equal(TestSettingClass.TECHNICAL_USER_DEFAULTVALUE, s.GetSettingValue<string, TestSettingClass.UseAuthentication.TechnicalUser>(_testInstanceId));
			Assert.Equal(TestSettingClass.TECHNICAL_USER_DEFAULTVALUE, s.GetSettingValueFromCache<string>(a));
			Assert.Equal(TestSettingClass.TECHNICAL_USER_DEFAULTVALUE, s.GetSettingValueFromDb<string>(a));
		}

		[Fact(DisplayName = "StoreSettingValue works well (set the setting value in db, in cache)")]
		public void StoreSettingValueWorksWell()
		{
			var value = "SAP1";
			var changer = "Test Code 1";
			var s = GetService<VSettings>();
			var a = s.GetAddress(typeof(TestSettingClass.TargetSAP), _testInstanceId);
			s.StoreSettingValue<string, TestSettingClass.TargetSAP>(value, _testInstanceId, changer);
			Assert.Equal(value, s.GetSettingValue<string, TestSettingClass.TargetSAP>(_testInstanceId));
			Assert.Equal(value, s.GetSettingValueFromCache<string>(a));
			Assert.Equal(value, s.GetSettingValueFromDb<string>(a));
			var value2 = "SAP2";
			var changer2 = "Test Code 2";
			s.StoreSettingValue<string, TestSettingClass.TargetSAP>(value2, _testInstanceId, changer2);
			using (var scope = GetService<IServiceScopeFactory>().CreateScope())
			{
				var db = s.GetDbContext(scope);
				var settingRecords = db.Settings.Where(x => x.ModuleKey == a.ModuleKey && x.InstanceOrUserKey == a.InstanceOrUserKey && x.Key == a.Key);
				Assert.Single(settingRecords);
				var settingRecord = settingRecords.FirstOrDefault();
				Assert.NotNull(settingRecord);
				Assert.Equal(value2, JsonConvert.DeserializeObject<string>(settingRecord.Value));
				var createRecords = db.Creates.Where(x => x.ModuleKey == a.ModuleKey && x.InstanceOrUserKey == a.InstanceOrUserKey && x.SettingKey == a.Key);
				Assert.Single(createRecords);
				var creatRecord = createRecords.FirstOrDefault();
				Assert.NotNull(creatRecord);
				Assert.Equal(value, JsonConvert.DeserializeObject<string>(creatRecord.To));
				Assert.Equal(changer, creatRecord.Changer);
				Assert.Equal(OperationType.Create, creatRecord.OperationType);
				var modifyRecords = db.Modifies.Where(x => x.ModuleKey == a.ModuleKey && x.InstanceOrUserKey == a.InstanceOrUserKey && x.SettingKey == a.Key);
				Assert.Single(modifyRecords);
				var modifyRecord = modifyRecords.FirstOrDefault();
				Assert.NotNull(modifyRecord);
				Assert.Equal(value, JsonConvert.DeserializeObject<string>(modifyRecord.From));
				Assert.Equal(value2, JsonConvert.DeserializeObject<string>(modifyRecord.To));
				Assert.Equal(changer2, modifyRecord.Changer);
				Assert.Equal(OperationType.Modify, modifyRecord.OperationType);
				var historyRecords = db.SettingHistories.Where(x => x.ModuleKey == a.ModuleKey && x.InstanceOrUserKey == a.InstanceOrUserKey && x.SettingKey == a.Key).OrderBy(x => x.Id);
				Assert.Equal(2, historyRecords.Count());
				var historyRecord = historyRecords.FirstOrDefault();
				Assert.Equal(OperationType.Create, historyRecord.OperationType);
				Assert.True(historyRecord is CreateSetting);				
			}
		}

		[Fact(DisplayName = "GetSettingValue works well (returns with setted value)")]
		public void GetSettingValueWorksWell()
		{
			var value = ReaderType.Multiple;
			var s = GetService<VSettings>();
			s.StoreSettingValue<ReaderType, TestSettingClass.TypeOfReader>(value, _testInstanceId);
			Assert.Equal(value, s.GetSettingValue<ReaderType, TestSettingClass.TypeOfReader>(_testInstanceId));
		}

		[Fact(DisplayName = "GetDbContext provide SQLiteContext in test environment, which is a SettingContext descendant.")]
		public void GetDbContextWorksWell()
		{
			var s = GetService<VSettings>();
			using (var scope = GetService<IServiceScopeFactory>().CreateScope())
			{
				Assert.IsType<SettingContextSQLite>(s.GetDbContext(scope));
				Assert.True(s.GetDbContext(scope) is SettingContext);
			}
		}

		[Fact(DisplayName = "Get all available setting defination class.")]
		public void GetAllSettingDefinationTypeWorkWell()
        {
			var s = GetService<VSettings>();
			var t = s.GetAllSettingDefinationType();
			Assert.Contains(t, x => x.FullName == typeof(TestSettingClass).FullName);
			Assert.Single(t);
		}

		[Fact(DisplayName = "Build full settingtree function works well.")]
		public void GetFullSettingTree()
		{
			var s = GetService<VSettings>();
			s.InitializeMe<TestSettingClass>();
			var t = s.BuildAllSettingTree();
			Assert.Single(t);
			Assert.Contains(t[0].Childrens, x => x.Title == nameof(TestSettingClass.DoubleNumber));
			Assert.Contains(t[0].Childrens, x => x.Title == nameof(TestSettingClass.ReaderCheckInterval));
			Assert.Contains(t[0].Childrens, x => x.Title == nameof(TestSettingClass.TargetSAP));
			Assert.Contains(t[0].Childrens, x => x.Title == nameof(TestSettingClass.TypeOfReader));
			Assert.Contains(t[0].Childrens, x => x.Title == nameof(TestSettingClass.UseAuthentication));
			Assert.Contains(t[0].Childrens, x => x.Title == nameof(TestSettingClass.Workstations));
			var nodeWithSubTree = t[0].Childrens.FirstOrDefault(x => x.Title == nameof(TestSettingClass.UseAuthentication));
			Assert.NotNull(nodeWithSubTree);
			Assert.NotEmpty(nodeWithSubTree.Childrens);
		}

		[Fact(DisplayName = "Store setting value function works well (both cache and persist side).")]
		public void StoreSettingValueFromUIWorksWell()
		{
			var s = GetService<VSettings>();
			s.InitializeMe<TestSettingClass>();
			s.StoreSettingValueFromUI("Log4Pro.CoreComponents.Test.Settings.TestSettingClass+DoubleNumber, Log4Pro.CoreComponents.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
				"System.Double, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "3.14", null, "TESTER");
			var a = s.GetAddress(typeof(TestSettingClass.DoubleNumber), null);
			Assert.Equal(3.14, s.GetSettingValueFromCache<double>(a));
			Assert.Equal(3.14, s.GetSettingValueFromDb<double>(a));
		}

		private readonly string _testInstanceId = Guid.NewGuid().ToString();
	}
}