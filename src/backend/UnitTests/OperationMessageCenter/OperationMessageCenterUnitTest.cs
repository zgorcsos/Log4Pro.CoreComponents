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
using Log4Pro.CoreComponents.Classes.UnitTestExtensions;
using Log4Pro.CoreComponents;
using Log4Pro.CoreComponents.Caching;
using Log4Pro.CoreComponents.Settings.Internals;
using Newtonsoft.Json;
using Log4Pro.CoreComponents.OperationMessageCenter;
using Log4Pro.CoreComponents.OperationMessageCenter.DAL;
using Log4Pro.CoreComponents.OperationMessageCenter.Internals;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Log4Pro.CoreComponents.Test.Settings
{
    public class OperationMessageCenterUnitTest : TestBaseClassWithServiceCollection
    {
        public OperationMessageCenterUnitTest(IServiceProvider serviceProvider, IServiceCollection sc) : base(serviceProvider)
        {
            var s = _serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration));
            _serviceCollection.Remove(s);
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            _serviceCollection.AddSingleton<IConfiguration>(builder.Build());
            var c = GetService<IConfiguration>();
            _serviceCollection.AddOperationMessageCenter((IConfiguration)_serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration)).ImplementationInstance);
        }

        [Fact(DisplayName = "AddOperationMessageCenter IServiceCollection extension to add the service works well.")]
        public void ServiceRegisterExtensionWorksWell()
        {
            var s = GetService<OperationMessageService>();
            Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageService));
            Assert.NotNull(s);
            Assert.IsType<OperationMessageService>(s);
            var w = GetService<OperationMessageWriter>();
            Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageWriter));
            Assert.NotNull(w);
            Assert.IsType<OperationMessageWriter>(w);
        }

        [Fact(DisplayName = "AddOperationMessageCenter IServiceCollection extension adds the needed service dependency too.")]
        public void ServiceRegisterExtensionWorksWell2()
        {
            var s = GetService<Cache>();
            Assert.Single(_serviceCollection, x => x.ServiceType == typeof(Cache));
            Assert.NotNull(s);
            Assert.IsType<Cache>(s);
            Assert.Same(s, GetService<Cache>());
            Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageCenterContextSQLite) && x.Lifetime == ServiceLifetime.Scoped);
        }

        [Fact(DisplayName = "AddOperationMessageCenter IServiceCollection extension not duplicates the service, when happens multiple call to the extension method.")]
        public void ServiceRegisterExtensionWorksWell3()
        {
            _serviceCollection.AddOperationMessageCenter((IConfiguration)_serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration)).ImplementationInstance);
            Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageService));
            Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageWriter));
            Assert.Single(_serviceCollection, x => x.ServiceType == typeof(Cache));
        }

        [Fact(DisplayName = "OperationMessageServer service is singletone.")]
        public void IsSingletone()
        {
            Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageService) && x.Lifetime == ServiceLifetime.Singleton);
            var s1 = GetService<OperationMessageService>();
            var s2 = GetService<OperationMessageService>();
            Assert.Same(s1, s2);
        }

        [Fact(DisplayName = "OperationMessageWriter service is transient.")]
        public void IsTransient()
        {
            Assert.Single(_serviceCollection, x => x.ServiceType == typeof(OperationMessageWriter) && x.Lifetime == ServiceLifetime.Transient);
            var s1 = GetService<OperationMessageWriter>();
            var s2 = GetService<OperationMessageWriter>();
            Assert.NotSame(s1, s2);
        }

        [Fact(DisplayName = "GetDbContext provide SQLiteContext in test environment, which is a OperationMessageCenterContext descendant.")]
        public void GetDbContextWorksWell()
        {
            var s = GetService<OperationMessageService>();
            using (var scope = GetService<IServiceScopeFactory>().CreateScope())
            {
                Assert.IsType<OperationMessageCenterContextSQLite>(s.GetDbContext(scope));
                Assert.True(s.GetDbContext(scope) is OperationMessageCenterContext);
            }
        }

        [Fact(DisplayName = "Setup is work. (It sets up both module and instance.)")]
        public void SetupIsWork()
        {
            var w = GetService<OperationMessageWriter>();
            w.Setup("M", "I");
            var m = w.FatalMessage("");
            Assert.Equal("M", m.Module);
            Assert.Equal("I", m.Instance);
        }

        [Fact(DisplayName = "All FatalMessage method overloads provide a right Fatal message.")]
        public void FatalMessageWorks()
        {
            var w = GetService<OperationMessageWriter>();
            var m = w.FatalMessage("M", "F");
            Assert.Equal("M", m.Message);
            Assert.Equal("F", m.OtherFilter);
            Assert.Equal(MessageCategory.Fatal, m.MessageCategory);
            m = w.FatalMessage("M");
            Assert.Null(m.OtherFilter);
            Assert.Equal(MessageCategory.Fatal, m.MessageCategory);
            m = w.FatalMessage("M", MessageCategory.Fatal);
            Assert.Equal("M", m.Message);
            Assert.Equal(MessageCategory.Fatal.ToString(), m.OtherFilter);
            Assert.Equal(MessageCategory.Fatal, m.MessageCategory);
        }

        [Fact(DisplayName = "All ErrorMessage method overloads provide a right Error message.")]
        public void ErrorMessageWorks()
        {
            var w = GetService<OperationMessageWriter>();
            var m = w.ErrorMessage("M", "F");
            Assert.Equal("M", m.Message);
            Assert.Equal("F", m.OtherFilter);
            Assert.Equal(MessageCategory.Error, m.MessageCategory);
            m = w.ErrorMessage("M");
            Assert.Null(m.OtherFilter);
            Assert.Equal(MessageCategory.Error, m.MessageCategory);
            m = w.ErrorMessage("M", MessageCategory.Error);
            Assert.Equal("M", m.Message);
            Assert.Equal(MessageCategory.Error.ToString(), m.OtherFilter);
            Assert.Equal(MessageCategory.Error, m.MessageCategory);
        }

        [Fact(DisplayName = "All WarningMessage method overloads provide a right Warning message.")]
        public void WarningMessageWorks()
        {
            var w = GetService<OperationMessageWriter>();
            var m = w.WarningMessage("M", "F");
            Assert.Equal("M", m.Message);
            Assert.Equal("F", m.OtherFilter);
            Assert.Equal(MessageCategory.Warning, m.MessageCategory);
            m = w.WarningMessage("M");
            Assert.Null(m.OtherFilter);
            Assert.Equal(MessageCategory.Warning, m.MessageCategory);
            m = w.WarningMessage("M", MessageCategory.Warning);
            Assert.Equal("M", m.Message);
            Assert.Equal(MessageCategory.Warning.ToString(), m.OtherFilter);
            Assert.Equal(MessageCategory.Warning, m.MessageCategory);
        }

        [Fact(DisplayName = "All InformationMessage method overloads provide a right Information message.")]
        public void InformationMessageWorks()
        {
            var w = GetService<OperationMessageWriter>();
            var m = w.InformationMessage("M", "F");
            Assert.Equal("M", m.Message);
            Assert.Equal("F", m.OtherFilter);
            Assert.Equal(MessageCategory.Information, m.MessageCategory);
            m = w.InformationMessage("M");
            Assert.Null(m.OtherFilter);
            Assert.Equal(MessageCategory.Information, m.MessageCategory);
            m = w.InformationMessage("M", MessageCategory.Information);
            Assert.Equal("M", m.Message);
            Assert.Equal(MessageCategory.Information.ToString(), m.OtherFilter);
            Assert.Equal(MessageCategory.Information, m.MessageCategory);
        }

        [Fact(DisplayName = "All DetailInformationMessage method overloads provide a right DetailInformation message.")]
        public void DetailInformationMessageWorks()
        {
            var w = GetService<OperationMessageWriter>();
            var m = w.DetailInformationMessage("M", "F");
            Assert.Equal("M", m.Message);
            Assert.Equal("F", m.OtherFilter);
            Assert.Equal(MessageCategory.DetailInformation, m.MessageCategory);
            m = w.DetailInformationMessage("M");
            Assert.Null(m.OtherFilter);
            Assert.Equal(MessageCategory.DetailInformation, m.MessageCategory);
            m = w.DetailInformationMessage("M", MessageCategory.DetailInformation);
            Assert.Equal("M", m.Message);
            Assert.Equal(MessageCategory.DetailInformation.ToString(), m.OtherFilter);
            Assert.Equal(MessageCategory.DetailInformation, m.MessageCategory);
        }

        [Fact(DisplayName = "StartNewThread method works rigth.")]
        public void StartNewThreadWorks()
        {
            var w = GetService<OperationMessageWriter>();
            Assert.Null(w.Thread);
            w.StartNewThread();
            var t1 = w.Thread;
            Assert.NotNull(t1);
            w.StartNewThread();
            var t2 = w.Thread;
            Assert.NotSame(t1, t2);
            w.StartNewThread("other_thread_id");
            Assert.Equal("other_thread_id", w.Thread);
        }

        [Fact(DisplayName = "DropThread method works rigth.")]
        public void DropThreadWorks()
        {
            var w = GetService<OperationMessageWriter>();
            w.StartNewThread();
            Assert.NotNull(w.Thread);
            w.DropThread();
            Assert.Null(w.Thread);
        }

        [Fact(DisplayName = "AddData OperationMessageEntry extension method works rigth.")]
        public void AddDataExtensionWorks()
        {
            var w = GetService<OperationMessageWriter>();
            var m = w.InformationMessage("")
                .AddData("1", "V1")
                .AddData("2", "V2");
            Assert.Equal(2, m.AdditionalDatas.Count());
            Assert.Equal("V1", m.AdditionalDatas.FirstOrDefault(x => x.Key == "1").Value);
            Assert.Equal("V2", m.AdditionalDatas.FirstOrDefault(x => x.Key == "2").Value);
        }

        [Fact(DisplayName = "GetAppSettingConfiguration works well.")]
        public void GetAppSettingConfigurationWorksWell()
        {
            var configuration = (IConfiguration)_serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration)).ImplementationInstance;
            var c = OperationMessageService.GetAppSettingConfiguration(configuration);
            Assert.Equal("B", c.UsedConnectionString);
            configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.error.json", true)
                 .Build();
            c = OperationMessageService.GetAppSettingConfiguration(configuration);
            Assert.Equal(OperationMessageService.DEFAULT_CONNECTIONSTRING, c.UsedConnectionString);
        }

        [Fact(DisplayName = "AddMessage OperationMessageEntry extension method and Service method works well both.")]
        public void AddMessageExtensionAndServiceMethodWorksWell()
        {
            const string MODUL = "M";
            const string INSTANCE = "I";
            const string MESSAGE = "Test";
            const string THREAD_ID = "T1";
            const string DATA_KEY_1 = "K1";
            const string DATA_VALUE_1 = "V1";
            const string DATA_KEY_2 = "K2";
            const string DATA_VALUE_2 = "V2";
            var w = GetService<OperationMessageWriter>();
            w.Setup(MODUL, INSTANCE);
            w.StartNewThread(THREAD_ID);
            w.InformationMessage(MESSAGE, MessageCategory.Information)
                .AddData(DATA_KEY_1, DATA_VALUE_1)
                .AddData(DATA_KEY_2, DATA_VALUE_2)
                .SendMe(w, true);
            var s = GetService<OperationMessageService>();
            using (var scope = GetService<IServiceScopeFactory>().CreateScope())
            {
                var db = s.GetDbContext(scope);
                Assert.Single(db.OperationMessages);
                Assert.Equal(2, db.AdditionalMessageDatas.Count());
                var mEntry = db.OperationMessages.Include(x => x.AdditionalMessageDatas).FirstOrDefault();
                Assert.NotNull(mEntry);
                Assert.False(mEntry.Handled);
                Assert.Null(mEntry.HandledBy);
                Assert.Null(mEntry.HandledTimeStamp);
                Assert.Equal(1, mEntry.Id);
                Assert.Equal(INSTANCE, mEntry.Instance);
                Assert.Equal(MESSAGE, mEntry.Message);
                Assert.Equal(MessageCategory.Information, mEntry.MessageCategory);
                Assert.Equal(MODUL, mEntry.Module);
                Assert.Equal(MessageCategory.Information.ToString(), mEntry.OtherFilter);
                Assert.Equal(THREAD_ID, mEntry.Thread);
                Assert.NotEqual(DateTime.Now, mEntry.TimeStamp);
                var data = mEntry.AdditionalMessageDatas.OrderBy(x => x.Id).FirstOrDefault();
                Assert.Equal(DATA_KEY_1, data.DataKey);
                Assert.Equal(DATA_VALUE_1, data.DataValue);
                data = mEntry.AdditionalMessageDatas.OrderBy(x => x.Id).Skip(1).FirstOrDefault();
                Assert.Equal(DATA_KEY_2, data.DataKey);
                Assert.Equal(DATA_VALUE_2, data.DataValue);
            }
        }
    }
}