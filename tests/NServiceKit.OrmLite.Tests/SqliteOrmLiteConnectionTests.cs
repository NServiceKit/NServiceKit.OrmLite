using System;
using System.IO;
using NUnit.Framework;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>A sqlite ORM lite connection tests.</summary>
	[TestFixture]
	public class SqliteOrmLiteConnectionTests 
		: OrmLiteTestBase
	{
        /// <summary>Sets the up.</summary>
        [SetUp]
        public void SetUp()
        {
            OrmLiteConfig.DialectProvider = SqliteDialect.Provider;
            ConnectionString = "test.sqlite";
        }

        /// <summary>Can create connection.</summary>
		[Test]
		public void Can_create_connection()
		{
			using (var db = OpenDbConnection())
			{
			}
		}

        /// <summary>Can create read only connection.</summary>
		[Test]
		public void Can_create_ReadOnly_connection()
		{
			using (var db = ConnectionString.OpenReadOnlyDbConnection()) 
			{
			}
		}

        /// <summary>Can create table with read only connection.</summary>
		[Test]
		public void Can_create_table_with_ReadOnly_connection()
		{
			using (var db = ConnectionString.OpenReadOnlyDbConnection())
			{
				try
				{
					db.CreateTable<ModelWithIdAndName>(true);
					db.Insert(new ModelWithIdAndName(1));
				}
				catch (Exception ex)
				{
					Log(ex.Message);
					return;
				}
				Assert.Fail("Should not be able to create a table with a readonly connection");
			}
		}

        /// <summary>Can open two read only connections to same database.</summary>
		[Test]
		public void Can_open_two_ReadOnlyConnections_to_same_database()
		{
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<ModelWithIdAndName>();
                db.Insert(new ModelWithIdAndName(1));

                using (var dbReadOnly = OpenDbConnection())
                {
                    dbReadOnly.Insert(new ModelWithIdAndName(2));
                    var rows = dbReadOnly.Select<ModelWithIdAndName>();
                    Assert.That(rows, Has.Count.EqualTo(2));
                }
            }
		}

	}
}