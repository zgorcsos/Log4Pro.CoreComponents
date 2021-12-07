using Log4Pro.CoreComponents.DIServices.OperationMessageCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.CoreComponents.DIServices.OperationMessageCenter
{
    public class OperationMessage
    {
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="module">module azonosító a példány által küldött üzenetekben (Message methodokkal lekért üzenetek)</param>
		/// <param name="instance">instance azonosító a példány által küldött üzenetekben</param>
		/// <param name="otherFilter">egyéb azonosító a példány által küldött üzenetekben</param>
		public OperationMessage(string module, string instance, string otherFilter = null)
		{
			_module = module;
			_instance = instance;
			_otherFilter = otherFilter;
		}

		public void StartNewThread(string threadId = null)
		{
			if (string.IsNullOrEmpty(threadId))
			{
				Thread = Guid.NewGuid().ToString();
			}
			else
			{
				Thread = threadId;
			}
		}

		public void DropThread()
		{
			Thread = null;
		}

		/// <summary>
		/// Visszaad egy új üzenet objektumot a megadott paraméterek alapján
		/// </summary>
		/// <param name="message">üzenet szövege</param>
		/// <param name="messageCategory">üzenet kategóriája</param>
		/// <param name="otherFilter">Egyéb szűrő az üzenet kategorzálásához</param>
		/// <returns>Üzenet objektum</returns>
		public OperationMessageEntry GetMessage(string message, MessageCategory messageCategory = MessageCategory.DetailInformation, string otherFilter = null)
		{
			return new OperationMessageEntry()
			{
				Module = _module,
				Instance = _instance,
				MessageCategory = messageCategory,
				Message = message,
				Thread = Thread,
				OtherFilter = otherFilter != null ? otherFilter : _otherFilter,
			};
		}

		/// <summary>
		/// Visszaad egy új Fatal (végzetes hiba) üzenet objektumot
		/// </summary>
		/// <param name="message">üzenet szövege</param>
		/// <param name="otherFilter">Egyéb szűrő az üzenet kategorzálásához</param>
		/// <returns>Üzenet objektum</returns>
		public OperationMessageEntry FatalMessage(string message, string otherFilter = null)
		{
			return GetMessage(message, MessageCategory.Fatal, otherFilter);
		}

		/// <summary>
		/// Returns a new Fatal message with enum specified filter.
		/// </summary>
		/// <typeparam name="TFilterEnum">The type of the filter enum.</typeparam>
		/// <param name="message">The message.</param>
		/// <param name="otherFilter">The other filter.</param>
		/// <returns></returns>
		public OperationMessageEntry FatalMessage<TFilterEnum>(string message, TFilterEnum otherFilter)
			where TFilterEnum : System.Enum
		{
			return GetMessage(message, MessageCategory.Fatal, otherFilter.ToString());
		}

		/// <summary>
		/// Visszaad egy új Error üzenet objektumot
		/// </summary>
		/// <param name="message">üzenet szövege</param>
		/// <param name="otherFilter">Egyéb szűrő az üzenet kategorzálásához</param>
		/// <returns>Üzenet objektum</returns>
		public OperationMessageEntry ErrorMessage(string message, string otherFilter = null)
		{
			return GetMessage(message, MessageCategory.Error, otherFilter);
		}

		/// <summary>
		/// Returns a new Error message with enum specified filter.
		/// </summary>
		/// <typeparam name="TFilterEnum">The type of the filter enum.</typeparam>
		/// <param name="message">The message.</param>
		/// <param name="otherFilter">The other filter.</param>
		/// <returns></returns>
		public OperationMessageEntry ErrorMessage<TFilterEnum>(string message, TFilterEnum otherFilter)
			where TFilterEnum : System.Enum
		{
			return GetMessage(message, MessageCategory.Error, otherFilter.ToString());
		}

		/// <summary>
		/// Visszaad egy új Warning üzenet objektumot
		/// </summary>
		/// <param name="message">üzenet szövege</param>
		/// <param name="otherFilter">Egyéb szűrő az üzenet kategorzálásához</param>
		/// <returns>Üzenet objektum</returns>
		public OperationMessageEntry WarningMessage(string message, string otherFilter = null)
		{
			return GetMessage(message, MessageCategory.Warning, otherFilter);
		}

		/// <summary>
		/// Returns a new Warning message with enum specified filter.
		/// </summary>
		/// <typeparam name="TFilterEnum">The type of the filter enum.</typeparam>
		/// <param name="message">The message.</param>
		/// <param name="otherFilter">The other filter.</param>
		/// <returns></returns>
		public OperationMessageEntry WarningMessage<TFilterEnum>(string message, TFilterEnum otherFilter)
			where TFilterEnum : System.Enum
		{
			return GetMessage(message, MessageCategory.Warning, otherFilter.ToString());
		}

		/// <summary>
		/// Visszaad egy új Information üzenet objektumot
		/// </summary>
		/// <param name="message">üzenet szövege</param>
		/// <param name="otherFilter">Egyéb szűrő az üzenet kategorzálásához</param>
		/// <returns>Üzenet objektum</returns>
		public OperationMessageEntry InformationMessage(string message, string otherFilter = null)
		{
			return GetMessage(message, MessageCategory.Information, otherFilter);
		}

		/// <summary>
		/// Returns a new information message with enum specified filter.
		/// </summary>
		/// <typeparam name="TFilterEnum">The type of the filter enum.</typeparam>
		/// <param name="message">The message.</param>
		/// <param name="otherFilter">The other filter.</param>
		/// <returns></returns>
		public OperationMessageEntry InformationMessage<TFilterEnum>(string message, TFilterEnum otherFilter)
			where TFilterEnum : System.Enum
		{
			return GetMessage(message, MessageCategory.Information, otherFilter.ToString());
		}

		/// <summary>
		/// Visszaad egy új DetailInformation üzenet objektumot
		/// </summary>
		/// <param name="message">üzenet szövege</param>
		/// <param name="otherFilter">Egyéb szűrő az üzenet kategorzálásához</param>
		/// <returns>Üzenet objektum</returns>
		public OperationMessageEntry DetailInformationMessage(string message, string otherFilter = null)
		{
			return GetMessage(message, MessageCategory.DetailInformation, otherFilter);
		}

		/// <summary>
		/// Returns a new detailinformation message with enum specified filter.
		/// </summary>
		/// <typeparam name="TFilterEnum">The type of the filter enum.</typeparam>
		/// <param name="message">The message.</param>
		/// <param name="otherFilter">The other filter.</param>
		/// <returns></returns>
		public OperationMessageEntry DetailInformationMessage<TFilterEnum>(string message, TFilterEnum otherFilter)
			where TFilterEnum : System.Enum
		{
			return GetMessage(message, MessageCategory.DetailInformation, otherFilter.ToString());
		}

		/// <summary>
		/// Üzeneteket összefogó szürő
		/// </summary>
		public string Thread { get; set; } = null;

		/// <summary>
		/// A használt Instance Id
		/// </summary>
		private readonly string _instance;

		/// <summary>
		/// A használt modul Id
		/// </summary>
		private readonly string _module;

		/// <summary>
		/// Egyéb szürő
		/// </summary>
		private readonly string _otherFilter;

	}
}
