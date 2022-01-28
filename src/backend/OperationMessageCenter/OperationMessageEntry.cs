using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.OperationMessageCenter
{
	/// <summary>
	/// An operation message entry
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
		/// Module identifier
		/// </summary>
		public string Module { get; set; }

		/// <summary>
		/// Instance identifier
		/// </summary>
		public string Instance { get; set; }

		/// <summary>
		/// Other identifier
		/// </summary>
		public string OtherFilter { get; set; }

		/// <summary>
		/// Category of message
		/// </summary>
		public MessageCategory MessageCategory { get; set; }

		/// <summary>
		/// The message body (human readable text)
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The tread id brings together related operation messages.
		/// </summary>
		public string Thread { set; get; }

		/// <summary>
		/// More detailed informations as list of key value pairs
		/// </summary>
		public List<KeyValuePair<string, string>> AdditionalDatas { get; } = new List<KeyValuePair<string, string>>();
	}
}
