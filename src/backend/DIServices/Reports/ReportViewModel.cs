using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WkHtmlToPdfDotNet;

namespace Log4Pro.DIServices.Reports
{
	/// <summary>
	/// View model class for reports
	/// </summary>
	/// <typeparam name="TContent">The type of the content.</typeparam>
	public class ReportViewModel<TContent>
	{
		/// <summary>
		/// The used css.
		/// </summary>
		public string CSS { get; set; }

		/// <summary>
		/// Gets or sets the kind of the paper.
		/// </summary>
		/// <value>
		/// The kind of the paper.
		/// </value>
		public PaperKind PaperKind { get; set; }

		/// <summary>
		/// The concrate viewmodel content.
		/// </summary>
		public TContent Content { get; set; }
	}
}
