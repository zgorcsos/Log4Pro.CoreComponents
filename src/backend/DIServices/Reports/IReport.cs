using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WkHtmlToPdfDotNet;

namespace Log4Pro.CoreComponents.DIServices.Reports
{
	/// <summary>
	/// Report interface: declaring a injected report service 
	/// </summary>
	public interface IReport
	{
		/// <summary>
		/// Gets the PDF asynchronous.
		/// </summary>
		/// <typeparam name="TContent">The concrate enclosed type in the report view model.</typeparam>
		/// <param name="razorView">The used razor view as cshtml file link with relative path.</param>
		/// <param name="model">The used view model instance to rendering view.</param>
		/// <returns>Task with byte array treturn type, when the returned byte array is the generated PDF file as byte array</returns>
		public Task<byte[]> GetPDFAsync<TContent>(string razorView, ReportViewModel<TContent> model);

		/// <summary>
		/// Gets the PDF global settings.
		/// </summary>
		/// <value>
		/// Represents the PDF global settings.
		/// </value>
		public GlobalSettings PDFGlobalSettings { get;  }

		/// <summary>
		/// Gets the PDF header settings.
		/// </summary>
		/// <value>
		/// Represents the PDF header settings.
		/// </value>
		public HeaderSettings PDFHeaderSettings { get; }

		/// <summary>
		/// Gets the PDF footer settings.
		/// </summary>
		/// <value>
		/// Represents the PDF footer settings.
		/// </value>
		public FooterSettings PDFFooterSettings { get; }

		/// <summary>
		/// Gets the PDF web (html) settings.
		/// </summary>
		/// <value>
		/// REpresent the PDF web (html) settings.
		/// </value>
		public WebSettings PDFWebSettings { get; }
	}
}
