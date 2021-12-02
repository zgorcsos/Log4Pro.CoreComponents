using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vrh.DIServices.Settings;

namespace Vrh.Test.DIServices.Settings
{
	[Description(nameof(TestSettingClass))]
	[ModuleKey(MODULE_NAME)]
	class TestSettingClass
	{
		[Description(nameof(ReaderCheckInterval))]
		[SettingType(typeof(int))]
		[DefaultValue(READER_CHECK_INTERVAL_DEFAULTVALUE)]
		public class ReaderCheckInterval { }

		[Description(nameof(TargetSAP))]
		[SettingType(typeof(string))]
		public class TargetSAP 
		{
			public string GetDefaultValue()
			{
				return TARGET_SAP_DEFAULTVALUE;
			}
		}

		[Description(nameof(WarningForEmptyField))]
		[SettingType(typeof(bool))]
		[DefaultValue(WARNING_FOR_EMPTY_FIELD_DEFAULTVALUE)]
		public class WarningForEmptyField { }

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

		[Description(nameof(UseAuthentication))]
		[SettingType(typeof(bool))]
		[DefaultValue(USE_AUTHENTICATION_DEFAULTVALUE)]
		public class UseAuthentication
		{
			[Description(nameof(TechnicalUser))]
			[DefaultValue(TECHNICAL_USER_DEFAULTVALUE)]
			public class TechnicalUser { }

			[Description(nameof(Password))]
			[SettingType(typeof(string))]
			[DefaultValue(PASSWORD_DEFAULTVALUE)]
			[SensitiveData]
			public class Password { }
		}

		[Description(nameof(TypeOfReader))]
		[SettingType(typeof(ReaderType))]
		[SettingSelections(typeof(ReaderType))]
		public class TypeOfReader { }

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

	class WorkstationIdBinding
	{
		public string WorkstationLevelId { get; set; }

		public string SAPLevelId { get; set; }
	}

	[JsonConverter(typeof(StringEnumConverter))]
	enum ReaderType
	{
		[Description(nameof(ReaderType.Single))]
		[IsDefault]
		Single,
		[Description(nameof(ReaderType.Multiple))]
		Multiple,
	}
}
