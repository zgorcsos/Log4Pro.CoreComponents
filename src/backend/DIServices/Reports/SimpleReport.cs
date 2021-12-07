using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Log4Pro.CoreComponents.DIServices.Hosting;

namespace Log4Pro.CoreComponents.DIServices.Reports
{
	/// <summary>
	/// Simple class for pdf reporting in .net core applications.
	/// </summary>
	public class SimpleReport : IReport
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleReport"/> class.
		/// Never manually create an instance of this class because it must be parameterized manually! Use the great DI engine of the .net core!
		/// </summary>
		/// <param name="serviceProvider">The service provider.</param>
		/// <param name="options">The MvcView options.</param>
		/// <param name="tempDataProvider">The temporary data provider.</param>
		/// <param name="htmltoPDFConverter">The htmlto PDF converter.</param>
		public SimpleReport(IServiceProvider serviceProvider, IOptions<MvcViewOptions> options, ITempDataProvider tempDataProvider, 
			IConverter htmltoPDFConverter)
		{
			_serviceProvider = serviceProvider;
			_options = options;
			_tempDataProvider = tempDataProvider;
			_htmltoPDFConverter = htmltoPDFConverter;
			var webSettings = new WebSettings
			{
				UserStyleSheet = "",
				DefaultEncoding = "utf-8",
			};
			var headerSettings = new HeaderSettings
			{
				FontSize = 12,
				FontName = "Ariel",
				Left = "VRH",
				Center = "Log4Pro report",
				Right = "Page [page] of [toPage]",
				Line = true,
			};
			var footerSettings = new FooterSettings
			{
				FontSize = 10,
				FontName = "Ariel",
				Left = DateTime.Now.ToString($"{CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern} {CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern}"),
				Center = "from Log4Pro system by VRH",
				Line = true,
			};
			_htmlToPdfDocument = new HtmlToPdfDocument
			{
				GlobalSettings = new GlobalSettings
				{
					DocumentTitle = "Log4Pro report",
					PaperSize = PaperKind.A4,
					Margins = new MarginSettings { Top = 25, Bottom = 25 },
				},
				Objects =
				{
					new ObjectSettings
					{
						PagesCount = true,
						HeaderSettings = headerSettings,
						FooterSettings = footerSettings,
						WebSettings = webSettings,
					}
				},
			};
		}

		///<inheritdoc cref="IReport"/>
		public async Task<byte[]> GetPDFAsync<TContent>(string razorView, ReportViewModel<TContent> model)
		{
			_htmlToPdfDocument.Objects.FirstOrDefault().HtmlContent = await RenderViewAsyn<TContent>(razorView, model);
			return _htmltoPDFConverter.Convert(_htmlToPdfDocument);
		}

		///<inheritdoc cref="IReport"/>
		public GlobalSettings PDFGlobalSettings
		{
			get
			{
				return _htmlToPdfDocument.GlobalSettings;
			}
		}

		///<inheritdoc cref="IReport"/>
		public HeaderSettings PDFHeaderSettings
		{
			get
			{
				return _htmlToPdfDocument.Objects.FirstOrDefault()?.HeaderSettings;
			}
		}

		///<inheritdoc cref="IReport"/>
		public FooterSettings PDFFooterSettings
		{
			get
			{
				return _htmlToPdfDocument.Objects.FirstOrDefault()?.FooterSettings;
			}
		}

		///<inheritdoc cref="IReport"/>
		public WebSettings PDFWebSettings
		{
			get
			{
				return _htmlToPdfDocument.Objects.FirstOrDefault()?.WebSettings;
			}
		}

		/// <summary>
		/// Renders the razor view asynchronous.
		/// </summary>
		/// <typeparam name="TContent">The type of the enlosed data in ReportViewModel.</typeparam>
		/// <param name="razorView">The razor view.</param>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">If the specified cshtml view not found by Razor viewengine.</exception>
		private async Task<string> RenderViewAsyn<TContent>(string razorView, ReportViewModel<TContent> model)
		{
			var actionContext = new ActionContext(new DefaultHttpContext { RequestServices = _serviceProvider }, new RouteData(), new ActionDescriptor());
			var viewEngine = _options.Value.ViewEngines[0] as IRazorViewEngine;
			var getViewResult = viewEngine.GetView(executingFilePath: null, viewPath: razorView, isMainPage: true);
			IView view;
			if (getViewResult.Success)
			{
				view = getViewResult.View;
			}
			else
			{
				var findViewResult = viewEngine.FindView(actionContext, razorView, isMainPage: true);
				if (findViewResult.Success)
				{
					view = findViewResult.View;
				}
				else
				{
					var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
					var errorMessage = string.Join(
						Environment.NewLine,
						new[] { $"Unable to find view '{razorView}'. The following locations were searched:" }.Concat(searchedLocations)); ;
					throw new InvalidOperationException(errorMessage);
				}
			}
			var viewData = new ViewDataDictionary<ReportViewModel<TContent>>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
			{
				Model = model
			};
			using var output = new StringWriter();
			var viewContext = new ViewContext(
				actionContext,
				view,
				viewData,
				new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
				output,
				new HtmlHelperOptions()
			);
			await view.RenderAsync(viewContext);
			return output.ToString();
		}

		private readonly IServiceProvider _serviceProvider;
		private readonly IOptions<MvcViewOptions> _options;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly IConverter _htmltoPDFConverter;
		private readonly HtmlToPdfDocument _htmlToPdfDocument;
	}
}