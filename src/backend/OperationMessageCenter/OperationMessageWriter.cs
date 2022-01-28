using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.OperationMessageCenter
{
    /// <summary>
    /// Provides a writer to write operation message to operation message center.
	/// This is a scoped DI service. 
    /// </summary>
    public class OperationMessageWriter
    {
		private readonly OperationMessageService _omCenter;

        public OperationMessageWriter(OperationMessageService omCenter)
        {
			_omCenter = omCenter;
        }

		/// <summary>
		/// Setups the writer working parameters.
		/// </summary>
		/// <param name="module">The module identifier.</param>
		/// <param name="instance">The instance identifier.</param>
		/// <param name="minimumWritedMessageCategory">The minimum writed message category.</param>
		public void Setup(string module, string instance, MessageCategory minimumWritedMessageCategory = MessageCategory.DetailInformation)
        {
			_module = module;
			_instance = instance;
			_minimumWritedMessageCategory = minimumWritedMessageCategory;
		}

		public bool AddMessage(OperationMessageEntry entry, bool waitMe)
        {
			if (waitMe)
			{
				return _omCenter.AddMessage(entry);
			}
			else
            {
				_omCenter.AddMessageAsync(entry);
				return true;
            }
        }

		/// <summary>
		/// Starts the new operation thread.
		/// The tread id brings together related operation messages.
		/// </summary>
		/// <param name="threadId">The thread identifier.</param>
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

        /// <summary>
        /// Drops the operation thread.
        /// </summary>
        public void DropThread()
		{
			Thread = null;
		}

        /// <summary>
        /// Creates a fatal message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="otherFilter">The other filter.</param>
        /// <returns></returns>
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
        /// Creates an Error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="otherFilter">The other filter.</param>
        /// <returns></returns>
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
        /// Creates a Warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="otherFilter">The other filter.</param>
        /// <returns></returns>
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
        /// Creates an Information message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="otherFilter">The other filter.</param>
        /// <returns></returns>
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
        /// Creates a DetailInformation message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="otherFilter">The other filter.</param>
        /// <returns></returns>
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
		/// Get/Set the Thread id 
		/// </summary>
		public string Thread { get; set; } = null;

		private OperationMessageEntry GetMessage(string message, MessageCategory messageCategory = MessageCategory.DetailInformation, string otherFilter = null)
		{
			if (_minimumWritedMessageCategory >= messageCategory)
			{
				return new OperationMessageEntry()
				{
					Module = _module,
					Instance = _instance,
					MessageCategory = messageCategory,
					Message = message,
					Thread = Thread,
					OtherFilter = otherFilter,
				};
			}
			else
            {
				return null;
            }
		}

		private string _instance;

		private string _module;

		private MessageCategory _minimumWritedMessageCategory = MessageCategory.DetailInformation;
	}
}
