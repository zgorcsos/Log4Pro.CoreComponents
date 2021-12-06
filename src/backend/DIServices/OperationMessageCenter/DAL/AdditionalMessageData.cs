using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.DIServices.OperationMessageCenter.DAL
{
	/// <summary>
	/// További, az üzenethez tartozó adatok
	/// </summary>
	[Index(nameof(DataKey))]
	public class AdditionalMessageData
	{
		/// <summary>
		/// PK
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// Adat azonosítója
		/// </summary>
		[Required]
		[MaxLength(100)]
		public string DataKey { get; set; }

		/// <summary>
		/// Adat értéke
		/// </summary>
		public string DataValue { get; set; }

		/// <summary>
		/// FK: Az üzenet Id-ja, amelyhez ez az adat tartozik
		/// </summary>
		public int OperationMessageId { get; set; }

		/// <summary>
		/// NP: Az üzenet, amelyhez ez az adat tartozik
		/// </summary>
		public virtual OperationMessage OperationMessage { get; set; }
	}
}
