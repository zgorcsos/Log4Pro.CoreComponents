using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.OperationMessageCenter.DAL
{
	public class OperationMessageCenterContext : DbContext
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationMessageCenterContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public OperationMessageCenterContext(
            DbContextOptions options) : base(options)
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
		/// Üzenetek
		/// </summary>
		internal DbSet<OperationMessage> OperationMessages { get; set; }

        /// <summary>
        /// Üzenetek további adatai
        /// </summary>
        public DbSet<AdditionalMessageData> AdditionalMessageDatas { get; set; }

        /// <summary>
        /// Database schema name
        /// </summary>
        internal const string DB_SCHEMA = "omcenter";
    }
}
