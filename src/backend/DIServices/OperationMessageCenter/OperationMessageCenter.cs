using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Log4Pro.CoreComponents.DIServices.OperationMessageCenter.DAL;
using Log4Pro.CoreComponents.DIServices.OperationMessageCenter.Internals;
using Microsoft.Extensions.Configuration;

namespace Log4Pro.CoreComponents.DIServices.OperationMessageCenter
{
	/// <summary>
	/// Működési üzenet modul
	/// </summary>
	public class OperationMessageCenter
	{
		#region instance level parts


		#endregion instance level parts

		#region Static parts

		internal static OperationMessageCenterAppSettings GetAppSettingConfiguration(IConfiguration configuration)
		{
			var config = configuration.GetSection(OperationMessageCenter.APPSETTINGS_SECTION_NAME).Get<OperationMessageCenterAppSettings>();
			if (config == null)
			{
				config = new OperationMessageCenterAppSettings();
			}
			if (string.IsNullOrEmpty(config.UsedConnectionString))
			{
				config.UsedConnectionString = OperationMessageCenter.DEFAULT_CONNECTIONSTRING;
			}
			return config;
		}

		/// <summary>
		/// Hozzáad egy üzenetet az üzenetközponthoz
		/// </summary>
		/// <param name="entry">Üzenet</param>
		/// <returns>true, ha sikeres a művelet</returns>
		public static Task<bool> AddMessageAsync(OperationMessageEntry entry)
		{
			return Task.Run<bool>(() => AddMessage(entry));
		}

		/// <summary>
		/// Hozzáad egy üzenetet az üzenetközponthoz
		/// </summary>
		/// <param name="entry">Üzenet</param>
		/// <returns>true, ha sikeres a művelet</returns>
		internal static bool AddMessage(OperationMessageEntry entry)
		{
			//	try
			//	{
			//		using (var db = new OperationMessageCenterContext())
			//		{
			//			var messageEntity = new OperationMessage()
			//			{
			//				Instance = entry.Instance,
			//				Message = entry.Message,
			//				MessageCategory = entry.MessageCategory,
			//				Module = entry.Module,
			//				OtherFilter = string.IsNullOrEmpty(entry.OtherFilter) ? null : entry.OtherFilter,
			//				TimeStamp = DateTime.UtcNow,
			//				Thread = entry.Thread,
			//			};
			//			db.OperationMessages.Add(messageEntity);
			//			foreach (var additionalData in entry.AdditionalDatas)
			//			{
			//				var additionalDataEntity = new AdditionalMessageData()
			//				{
			//					DataKey = additionalData.Key,
			//					DataValue = additionalData.Value,
			//					OperationMessage = messageEntity,
			//				};
			//				db.AdditionalMessageDatas.Add(additionalDataEntity);						
			//			}
			//			db.SaveChanges();
			//			entry.DbId = messageEntity.Id;
			//			entry.TimeStamp = messageEntity.TimeStamp;
			//		}
			//		return true;
			//	}
			//	catch (Exception ex)
			//	{
			//		try
			//		{
			//		}
			//		catch (Exception)
			//		{
			//		}
			//		return false;
			//	}
			return false;
		}

		#endregion Static parts

		internal const string APPSETTINGS_SECTION_NAME = "OperationMessageCenter";
		internal const string DEFAULT_CONNECTIONSTRING = "DefaultConnection";
	}
}
