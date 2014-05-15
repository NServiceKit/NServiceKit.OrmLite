using System;
using System.IO;
using NUnit.Framework;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.OrmLite.FirebirdTests
{
    /// <summary>An ORM lite connection tests.</summary>
	[TestFixture]
	public class OrmLiteConnectionTests 
		: OrmLiteTestBase
	{
        /// <summary>Can create connection to blank database.</summary>
		[Test][Ignore]
		public void Can_create_connection_to_blank_database()
		{
			var connString ="User=SYSDBA;Password=masterkey;Database=ormlite-tests.fdb;DataSource=localhost;Dialect=3;charset=ISO8859_1;";
			using (var db = connString.OpenDbConnection())
			{
			}
		}

        /// <summary>Can create connection.</summary>
		[Test]
		public void Can_create_connection()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
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
		[Test][Ignore]
		public void Can_create_table_with_ReadOnly_connection()
		{
			using (var db = ConnectionString.OpenReadOnlyDbConnection())
			{
				try
				{
					db.CreateTable<ModelWithIdAndName>(true);
					db.Insert(new ModelWithIdAndName(0));
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
            var db = ConnectionString.OpenReadOnlyDbConnection();
            db.CreateTable<ModelWithIdAndName>(true);
            db.Insert(new ModelWithIdAndName(1));

            var dbReadOnly = ConnectionString.OpenReadOnlyDbConnection();
            dbReadOnly.Insert(new ModelWithIdAndName(2));
            var rows = dbReadOnly.Select<ModelWithIdAndName>();
            Assert.That(rows, Has.Count.EqualTo(2));

			dbReadOnly.Dispose();
			db.Dispose();
		}

	}
}