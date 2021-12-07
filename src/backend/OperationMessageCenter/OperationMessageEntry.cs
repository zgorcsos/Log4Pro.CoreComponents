using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.OperationMessageCenter
{
	/// <summary>
	/// Egy üzenet bejegyzés
	/// </summary>
	public class OperationMessageEntry
	{
		/// <summary>
		/// The database unique identifier (PK).
		/// </summary>
		public int DbId { get; set; }

		/// <summary>
		/// The time stamp.
		/// </summary>
		public DateTime TimeStamp { get; set; }

		/// <summary>
		/// Module, amihez tartozik
		/// </summary>
		public string Module { get; set; }

		/// <summary>
		/// Instance, amihez tartozik
		/// </summary>
		public string Instance { get; set; }

		/// <summary>
		/// Egyéb azonosító a megoldásfüggő kategorizáláshoz
		/// </summary>
		public string OtherFilter { get; set; }

		/// <summary>
		/// Üzenet ketegoriája
		/// </summary>
		public MessageCategory MessageCategory { get; set; }

		/// <summary>
		/// Üzenet szövege
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Üzeneteket összefogó azonosító
		/// </summary>
		public string Thread { set; get; }

		/// <summary>
		/// További az üzenethez tartozó adatok
		/// </summary>
		public List<KeyValuePair<string, string>> AdditionalDatas { get; } = new List<KeyValuePair<string, string>>();
	}
}
