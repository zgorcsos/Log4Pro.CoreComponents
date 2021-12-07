using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.OperationMessageCenter.DAL
{
	/// <summary>
	/// Operation Message Center DAL servicees as extension methodes
	/// </summary>
	public static class OperationMessageCenterContextExtensions
	{
		#region ErrorCounts

		/// <summary>
		/// Gets unhandled error count in module/instance.
		/// </summary>
		/// <param name="db">The database.</param>
		/// <param name="module">The module.</param>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public static int MyUnhandledErrorCount(this OperationMessageCenterContext db, string module, string instance)
		{
			return db.OperationMessages.Count(x => x.Module == module && x.Instance == instance 
												&& (x.MessageCategory == MessageCategory.Error || x.MessageCategory == MessageCategory.Fatal)
												&& !x.Handled);
		}


		public static int MyErrorCountSince(this OperationMessageCenterContext db, string module, string instance, int daysAgo)
		{
			DateTime since = DateTime.UtcNow.AddDays(-daysAgo);
			return db.OperationMessages.Count(x => x.Module == module && x.Instance == instance
												&& (x.MessageCategory == MessageCategory.Error || x.MessageCategory == MessageCategory.Fatal)
												&& x.TimeStamp >= since);
		}

		public static int AllUnhandledErrorCount(this OperationMessageCenterContext db)
		{
			return db.OperationMessages.Count(x => (x.MessageCategory == MessageCategory.Error || x.MessageCategory == MessageCategory.Fatal)
												&& !x.Handled);
		}

		public static int AllErrorCountSince(this OperationMessageCenterContext db, int daysAgo)
		{
			DateTime since = DateTime.UtcNow.AddDays(-daysAgo);
			return db.OperationMessages.Count(x => (x.MessageCategory == MessageCategory.Error || x.MessageCategory == MessageCategory.Fatal)
												&& x.TimeStamp >= since);
		}

		#endregion ErrorCounts

		#region WarningCounts

		public static int MyUnhandledWarningCount(this OperationMessageCenterContext db, string module, string instance)
		{
			return db.OperationMessages.Count(x => x.Module == module && x.Instance == instance
												&& x.MessageCategory == MessageCategory.Warning
												&& !x.Handled);
		}

		public static int MyWarningCountSince(this OperationMessageCenterContext db, string module, string instance, int daysAgo)
		{
			DateTime since = DateTime.UtcNow.AddDays(-daysAgo);
			return db.OperationMessages.Count(x => x.Module == module && x.Instance == instance
												&& x.MessageCategory == MessageCategory.Warning
												&& x.TimeStamp >= since);
		}

		public static int AllUnhandledWarningCount(this OperationMessageCenterContext db)
		{
			return db.OperationMessages.Count(x => x.MessageCategory == MessageCategory.Warning
												&& !x.Handled);
		}

		public static int AllWarningCountSince(this OperationMessageCenterContext db, int daysAgo)
		{
			DateTime since = DateTime.UtcNow.AddDays(-daysAgo);
			return db.OperationMessages.Count(x => x.MessageCategory == MessageCategory.Warning
												&& x.TimeStamp >= since);
		}

		#endregion WarningCounts

		#region OtherFilterCounts

		/// <summary>
		/// Gets counts of message with the specified filter.
		/// </summary>
		/// <typeparam name="TFilterEnum">The type of the filter enum.</typeparam>
		/// <param name="db">The database.</param>
		/// <param name="module">The module.</param>
		/// <param name="instance">The instance.</param>
		/// <param name="filter">The filter.</param>
		/// <returns></returns>
		public static int CountThisFilter<TFilterEnum>(this OperationMessageCenterContext db, string module, string instance, TFilterEnum filter)
			where TFilterEnum : System.Enum
		{
			return db.OperationMessages.Count(x => x.Module == module && x.Instance == instance && x.OtherFilter == filter.ToString());
		}

		public static int MyCountSince(this OperationMessageCenterContext db, string module, string instance, string filter, int daysAgo)
		{
			DateTime since = DateTime.UtcNow.AddDays(-daysAgo);
			return db.OperationMessages.Count(x => x.Module == module && x.Instance == instance
												&& x.OtherFilter == filter
												&& x.TimeStamp >= since);
		}

		public static int DaysCountByFilter(this OperationMessageCenterContext db, string module, string instance, string filter, int daysAgo)
		{
			DateTime since = DateTime.UtcNow.AddDays(-daysAgo);
			return db.OperationMessages.Count(x => x.Module == module && x.Instance == instance
												&& x.OtherFilter == filter
												&& x.TimeStamp >= since);
		}

		public static List<int> HoursStatisticByFilter(this OperationMessageCenterContext db, string module, string instance, string filter, int hours)
		{
			DateTime now = DateTime.UtcNow;
			List<int> statistic = new List<int>();
			for (int i = hours; i > 0; i--)
			{
				DateTime start = now.AddHours(-(i));
				DateTime end = now.AddHours(-(i-1));
				int count = db.OperationMessages.Count(x => x.Module == module && x.Instance == instance
													&& x.OtherFilter == filter
													&& x.TimeStamp >= start && x.TimeStamp < end);
				statistic.Add(count);
			}
			return statistic;
		}

		public static List<int> DaysStatisticByFilter(this OperationMessageCenterContext db, string module, string instance, string filter, int days)
		{
			DateTime now = DateTime.UtcNow;
			List<int> statistic = new List<int>();
			for (int i = days; i > 0; i--)
			{
				DateTime start = now.AddDays(-(i));
				DateTime end = now.AddDays(-(i - 1));
				int count = db.OperationMessages.Count(x => x.Module == module && x.Instance == instance
													&& x.OtherFilter == filter
													&& x.TimeStamp >= start && x.TimeStamp < end);
				statistic.Add(count);
			}
			return statistic;
		}

		#endregion OtherFilterCounts
	}
}
