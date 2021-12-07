using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.DIServices.OperationMessageCenter
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
			messageEntry.AdditionalDatas.Add(new KeyValuePair<string, string>(key, value));
			return messageEntry;
		}

		/// <summary>
		/// It adds (save the database) the operation message entry. It can do Sync (run and wait in caller tread) or assync (run in taskpool) mode, as the waitMe parameter provides.
		/// </summary>
		/// <param name="messageEntry">Operation message</param>
		/// <param name="waitMe">Wait (true) or not wait to save the message.</param>
		public static void SendMe(this OperationMessageEntry messageEntry, bool waitMe = false)
		{
			if (waitMe)
			{
				messageEntry.SendAndWaitMe();
			}
			else
			{
				OperationMessageCenter.AddMessageAsync(messageEntry);
			}
		}

		/// <summary>
		/// Szinkron módon hozzáadja az üzenetet az operationmessage modulhoz. Blokkolja  hívó szálat amíg a művelet végrehajtódik!
		/// Pl.: dispose ágakon ezt kell használni, hogy a főszáll ne szünjön meg amíg a taskpoolon lévő üzenetek bemenek.
		/// </summary>
		/// <param name="messageEntry">The message entry.</param>
		public static void SendAndWaitMe(this OperationMessageEntry messageEntry)
		{
			OperationMessageCenter.AddMessage(messageEntry);
		}
	}
}
