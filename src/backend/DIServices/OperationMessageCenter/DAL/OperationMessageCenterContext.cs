using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Log4Pro.ConnectionStringStore;

namespace Log4Pro.OperationMessageCenter.Core.DAL
{
	public class OperationMessageCenterContext : DbContext
	{
        /// <summary>
        /// Constructor
        /// </summary>
        public OperationMessageCenterContext()
            : base(VRHConnectionStringStore.GetSQLConnectionString("OperationMessageCenter", true))
        {
			((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 180;
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<OperationMessageCenterContext, Migrations.Configuration>());
        }

        /// <summary>
        /// OnModelCreating override
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DB_SCHEMA);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Üzenetek
        /// </summary>
        public DbSet<OperationMessage> OperationMessages { get; set; }

        /// <summary>
        /// Üzenetek további adatai
        /// </summary>
        public DbSet<AdditionalMessageData> AdditionalMessageDatas { get; set; }

        /// <summary>
        /// Adatbázis séma név
        /// </summary>
        public const string DB_SCHEMA = "omcenter";
    }
}
