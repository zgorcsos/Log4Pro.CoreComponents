using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.DIServices.OperationMessageCenter.DAL
{
    /// <summary>
    /// Context for unit testing
    /// </summary>
    /// <seealso cref="Log4Pro.CoreComponents.DIServices.OperationMessageCenter.DAL.OperationMessageCenterContext" />
    public class OperationMessageCenterContextSQLite : OperationMessageCenterContext
	{
		public OperationMessageCenterContextSQLite(
			DbContextOptions options) : base(options)
		{
		}
	}
}
