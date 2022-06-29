using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.OperationMessageCenter
{
	/// <summary>
	/// Bővító metódusok az OperationMessageEntry típushoz
	/// </summary>
	public static class OperationMessageEntryExtensions
	{
		/// <summary>
		/// Hozzáad egy új adatelemet az üzenethez
		/// </summary>
		/// <param name="messageEntry">üzenet</param>
		/// <param name="key">adat kulcsa</param>
		/// <param name="value">adat értéke</param>
		/// <returns>Az üzenet objektum, az új adat hozzáadásával</returns>
		public static OperationMessageEntry AddData(this OperationMessageEntry messageEntry, string key, string value)
		{
			if (messageEntry != null)
			{
				messageEntry.AdditionalDatas.Add(new KeyValuePair<string, string>(key, value));
			}
			return messageEntry;
		}

		/// <summary>
		/// It adds (save the database) the operation message entry. It can do Sync (run and wait in caller tread) or assync (run in taskpool) mode, as the waitMe parameter provides.
		/// </summary>
		/// <param name="messageEntry">Operation message</param>
		/// <param name="waitMe">Wait (true) or not wait to save the message.</param>
		public static void SendMe(this OperationMessageEntry messageEntry, OperationMessageWriter writer, bool waitMe = false)
		{
			if (messageEntry != null)
			{
				writer.AddMessage(messageEntry, waitMe);
			}
		}
	}
}
