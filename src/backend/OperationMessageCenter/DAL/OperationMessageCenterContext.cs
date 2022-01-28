using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.OperationMessageCenter.DAL
{
    /// <summary>
    /// Database context of Operation message center
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public class OperationMessageCenterContext : DbContext
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationMessageCenterContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public OperationMessageCenterContext(
            DbContextOptions<OperationMessageCenterContext> options) : base(options)
        {
            Database.Migrate();
        }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
			{
				relationship.DeleteBehavior = DeleteBehavior.Restrict;
			}
			base.OnModelCreating(modelBuilder);
		}


		/// <summary>
		/// Operation messages
		/// </summary>
		internal DbSet<OperationMessage> OperationMessages { get; set; }

        /// <summary>
        /// Aditional data of operation message
        /// </summary>
        public DbSet<AdditionalMessageData> AdditionalMessageDatas { get; set; }

        /// <summary>
        /// Database schema name
        /// </summary>
        internal const string DB_SCHEMA = "omcenter";
    }
}
