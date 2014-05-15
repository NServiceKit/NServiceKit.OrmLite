using System;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.OrmLite.FirebirdTests
{
    /// <summary>An ORM lite create table with indexes tests.</summary>
	[TestFixture]
	public class OrmLiteCreateTableWithIndexesTests 
		: OrmLiteTestBase
	{
        /// <summary>Can create model with index fields table.</summary>
		[Test]
		public void Can_create_ModelWithIndexFields_table()
		{
			OrmLiteConfig.DialectProvider.DefaultStringLength=128;
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<ModelWithIndexFields>(true);

				var sql = OrmLiteConfig.DialectProvider.ToCreateIndexStatements( typeof (ModelWithIndexFields)).Join();

				Assert.IsTrue(sql.Contains("idx_modelwif_name"));
				Assert.IsTrue(sql.Contains("uidx_modelwif_uniquename"));
			}
		}

        /// <summary>Can create model with composite index fields table.</summary>
		[Test]
		public void Can_create_ModelWithCompositeIndexFields_table()
		{
			OrmLiteConfig.DialectProvider.DefaultStringLength=128;
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<ModelWithCompositeIndexFields>(true);

				var sql = OrmLiteConfig.DialectProvider.ToCreateIndexStatements(typeof(ModelWithCompositeIndexFields)).Join();

				Assert.IsTrue(sql.Contains("idx_modelwcif_name"));
				Assert.IsTrue(sql.Contains("idx_modelwcif_comp1_comp2"));
			}
		}


	}
}