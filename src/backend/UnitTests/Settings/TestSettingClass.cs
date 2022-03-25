using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Log4Pro.CoreComponents.Settings;
using Log4Pro.CoreComponents.OperationMessageCenter;

namespace Log4Pro.CoreComponents.Test.Settings
{
    [Title(nameof(TestSettingClass))]
    [Description(nameof(TestSettingClass))]
    [ModuleKey(MODULE_NAME)]
    class TestSettingClass
    {
        [Title(nameof(ReaderCheckInterval))]
        [Description(nameof(ReaderCheckInterval))]
        [SettingType(typeof(int))]
        [DefaultValue(READER_CHECK_INTERVAL_DEFAULTVALUE)]
        public class ReaderCheckInterval { }

        [Title(nameof(TargetSAP))]
        [Description(nameof(TargetSAP))]
        [SettingType(typeof(string))]
        public class TargetSAP
        {
            public string GetDefaultValue()
            {
                return TARGET_SAP_DEFAULTVALUE;
            }
        }

        [Title(nameof(WarningForEmptyField))]
        [Description(nameof(WarningForEmptyField))]
        [SettingType(typeof(bool))]
        [DefaultValue(WARNING_FOR_EMPTY_FIELD_DEFAULTVALUE)]
        public class WarningForEmptyField { }

        [Title(nameof(Workstations))]
        [Description(nameof(Workstations))]
        [SettingType(typeof(List<WorkstationIdBinding>))]
        [Version(WORKSTATIONS_VERSION)]
        public class Workstations
        {
            public static List<WorkstationIdBinding> GetDefaultValue()
            {
                return WORKSTATIONS_DEFAULTVALUE;
            }
        }

        [Title(nameof(UseAuthentication))]
        [Description(nameof(UseAuthentication))]
        [SettingType(typeof(bool))]
        [DefaultValue(USE_AUTHENTICATION_DEFAULTVALUE)]
        public class UseAuthentication
        {
            [Title(nameof(TechnicalUser))]
            [Description(nameof(TechnicalUser))]
            [DefaultValue(TECHNICAL_USER_DEFAULTVALUE)]
            [SettingType(typeof(string))]
            public class TechnicalUser { }

            [Title(nameof(Password))]
            [Description(nameof(Password))]
            [SettingType(typeof(string))]
            [DefaultValue(PASSWORD_DEFAULTVALUE)]
            [SensitiveData]
            public class Password { }
        }

        [Title(nameof(TypeOfReader))]
        [Description(nameof(TypeOfReader))]
        [SettingType(typeof(ReaderType))]
        [SettingSelections(typeof(ReaderType))]
        public class TypeOfReader { }

        [Title(nameof(OperationMessageWriteLevel))]
        [Description(nameof(OperationMessageWriteLevel))]
        [SettingType(typeof(MessageCategory))]
        [DefaultValue(MessageCategory.Information)]
        [SettingSelections(typeof(MessageCategory))]
        public class OperationMessageWriteLevel { }

        [Title(nameof(DoubleNumber))]
        [Description(nameof(DoubleNumber))]
        [SettingType(typeof(double))]
        [DefaultValue(0.12345)]
        public class DoubleNumber { }

        [Title(nameof(ReaderCheckIntervalTimeSpan))]
        [Description(nameof(ReaderCheckIntervalTimeSpan))]
        [SettingType(typeof(TimeSpan))]
        public class ReaderCheckIntervalTimeSpan
        {
            public static TimeSpan GetDefaultValue()
            {
                return new TimeSpan(0, 0, 10);
            }
        }

        [Title(nameof(UserLevelSample))]
        [Description(nameof(UserLevelSample))]
        [SettingType(typeof(string))]
        [DefaultValue("ULS")]
        [UserLevel]
        public class UserLevelSample { }

        public const int READER_CHECK_INTERVAL_DEFAULTVALUE = 10;
        public const string TARGET_SAP_DEFAULTVALUE = "ERT";
        public const bool WARNING_FOR_EMPTY_FIELD_DEFAULTVALUE = false;
        public const bool USE_AUTHENTICATION_DEFAULTVALUE = true;
        public const string TECHNICAL_USER_DEFAULTVALUE = "TECH_USER";
        public const string PASSWORD_DEFAULTVALUE = "";
        public const string WORKSTATIONS_VERSION = "1.0.0";
        public const string MODULE_NAME = "Test";
        public static List<WorkstationIdBinding> WORKSTATIONS_DEFAULTVALUE = new()
        {
            new WorkstationIdBinding
            {
                WorkstationLevelId = "84",
                SAPLevelId = "NFAGG340",
            },
            new WorkstationIdBinding
            {
                WorkstationLevelId = "85",
                SAPLevelId = "NFAGG350",
            },
        };
    }

    [Title(nameof(UserLevelSettings))]
    [Description(nameof(UserLevelSettings))]
    [ModuleKey(USERLEVEL_MODUL)]
    [UserLevel]
    class UserLevelSettings
    {
        [Title(nameof(UserStyle))]
        [Description(nameof(UserStyle))]
        [SettingType(typeof(string))]
        [DefaultValue("default")]
        public class UserStyle { }

        public const string USERLEVEL_MODUL = "UserSettings";
    }

    class WorkstationIdBinding
    {
        public string WorkstationLevelId { get; set; }

        public string SAPLevelId { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ReaderType
    {
        [Title(nameof(ReaderType.Single))]
        [Description(nameof(ReaderType.Single))]
        [IsDefault]
        Single,
        [Title(nameof(ReaderType.Multiple))]
        [Description(nameof(ReaderType.Multiple))]
        Multiple,
    }
}
