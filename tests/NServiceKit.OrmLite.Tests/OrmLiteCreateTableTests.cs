using System;
using NUnit.Framework;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>An ORM lite create table tests.</summary>
	[TestFixture]
	public class OrmLiteCreateTableTests 
		: OrmLiteTestBase
	{
        /// <summary>Queries if a given does table exists.</summary>
		[Test]
		public void Does_table_Exists()
		{
			using (var db = OpenDbConnection())
			{
				db.DropTable<ModelWithIdOnly>();

				Assert.That(
                    db.TableExists(typeof(ModelWithIdOnly).Name),
					Is.False);
				
				db.CreateTable<ModelWithIdOnly>(true);

				Assert.That(
                    db.TableExists(typeof(ModelWithIdOnly).Name),
					Is.True);
			}
		}

        /// <summary>Can create model with identifier only table.</summary>
		[Test]
		public void Can_create_ModelWithIdOnly_table()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithIdOnly>(true);
			}
		}

        /// <summary>Can create model with only string fields table.</summary>
		[Test]
		public void Can_create_ModelWithOnlyStringFields_table()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithOnlyStringFields>(true);
			}
		}

        /// <summary>Can create model with long identifier and string fields table.</summary>
		[Test]
		public void Can_create_ModelWithLongIdAndStringFields_table()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithLongIdAndStringFields>(true);
			}
		}

        /// <summary>Can create model with fields of different types table.</summary>
		[Test]
		public void Can_create_ModelWithFieldsOfDifferentTypes_table()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithFieldsOfDifferentTypes>(true);
			}
		}

        /// <summary>Can preserve model with identifier only table.</summary>
		[Test]
		public void Can_preserve_ModelWithIdOnly_table()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithIdOnly>(true);

				db.Insert(new ModelWithIdOnly(1));
				db.Insert(new ModelWithIdOnly(2));

				db.CreateTable<ModelWithIdOnly>(false);

				var rows = db.Select<ModelWithIdOnly>();

				Assert.That(rows, Has.Count.EqualTo(2));
			}
		}

        /// <summary>Can preserve model with identifier and name table.</summary>
		[Test]
		public void Can_preserve_ModelWithIdAndName_table()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithIdAndName>(true);

				db.Insert(new ModelWithIdAndName(1));
				db.Insert(new ModelWithIdAndName(2));

				db.CreateTable<ModelWithIdAndName>(false);

				var rows = db.Select<ModelWithIdAndName>();

				Assert.That(rows, Has.Count.EqualTo(2));
			}
		}

        /// <summary>Can overwrite model with identifier only table.</summary>
		[Test]
		public void Can_overwrite_ModelWithIdOnly_table()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithIdOnly>(true);

				db.Insert(new ModelWithIdOnly(1));
				db.Insert(new ModelWithIdOnly(2));

				db.CreateTable<ModelWithIdOnly>(true);

				var rows = db.Select<ModelWithIdOnly>();

				Assert.That(rows, Has.Count.EqualTo(0));
			}
		}

        /// <summary>Can create multiple tables.</summary>
		[Test]
		public void Can_create_multiple_tables()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTables(true, typeof(ModelWithIdOnly), typeof(ModelWithIdAndName));

				db.Insert(new ModelWithIdOnly(1));
				db.Insert(new ModelWithIdOnly(2));

				db.Insert(new ModelWithIdAndName(1));
				db.Insert(new ModelWithIdAndName(2));

				var rows1 = db.Select<ModelWithIdOnly>();
				var rows2 = db.Select<ModelWithIdOnly>();

				Assert.That(rows1, Has.Count.EqualTo(2));
				Assert.That(rows2, Has.Count.EqualTo(2));
			}
		}

        /// <summary>
        /// Can create model with identifier and name table with specified default string length.
        /// </summary>
		[Test]
		public void Can_create_ModelWithIdAndName_table_with_specified_DefaultStringLength()
		{
			OrmLiteConfig.DialectProvider.DefaultStringLength = 255;
			var createTableSql =  OrmLiteConfig.DialectProvider.ToCreateTableStatement(typeof(ModelWithIdAndName));

			Console.WriteLine("createTableSql: " + createTableSql);
			Assert.That(createTableSql.Contains("VARCHAR(255)"), Is.True);
		}

	}
}