using System;
using NUnit.Framework;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>An ORM lite create table with namig strategy tests.</summary>
	[TestFixture]
	public class OrmLiteCreateTableWithNamigStrategyTests 
		: OrmLiteTestBase
	{
        /// <summary>Can create table with namig strategy table prefix.</summary>
		[Test]
		public void Can_create_TableWithNamigStrategy_table_prefix()
		{
			OrmLiteConfig.DialectProvider.NamingStrategy = new PrefixNamingStrategy
			{
				 TablePrefix ="tab_",
				 ColumnPrefix = "col_",
			};
			
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithOnlyStringFields>(true);
			}
			
			OrmLiteConfig.DialectProvider.NamingStrategy= new OrmLiteNamingStrategyBase();
		}

        /// <summary>Can create table with namig strategy table lowered.</summary>
		[Test]
		public void Can_create_TableWithNamigStrategy_table_lowered()
		{
			OrmLiteConfig.DialectProvider.NamingStrategy = new LowercaseNamingStrategy();

			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithOnlyStringFields>(true);
			}
			
			OrmLiteConfig.DialectProvider.NamingStrategy= new OrmLiteNamingStrategyBase();
		}

        /// <summary>Can create table with namig strategy table name underscore coumpound.</summary>
		[Test]
		public void Can_create_TableWithNamigStrategy_table_nameUnderscoreCoumpound()
		{
			OrmLiteConfig.DialectProvider.NamingStrategy = new UnderscoreSeparatedCompoundNamingStrategy();

			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithOnlyStringFields>(true);
			}
			
			OrmLiteConfig.DialectProvider.NamingStrategy= new OrmLiteNamingStrategyBase();
		}

        /// <summary>Can get data from table with namig strategy with get by identifier.</summary>
		[Test]
		public void Can_get_data_from_TableWithNamigStrategy_with_GetById()
		{
			OrmLiteConfig.DialectProvider.NamingStrategy = OrmLiteConfig.DialectProvider.NamingStrategy = new PrefixNamingStrategy
			{
				TablePrefix = "tab_",
				ColumnPrefix = "col_",
			};

			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithOnlyStringFields>(true);
				var m = new ModelWithOnlyStringFields { Id= "999", AlbumId = "112", AlbumName="ElectroShip", Name = "MyNameIsBatman"};

				db.Save(m);
				var modelFromDb =  db.GetById<ModelWithOnlyStringFields>("999");

				Assert.AreEqual(m.Name, modelFromDb.Name);
			}
			
			OrmLiteConfig.DialectProvider.NamingStrategy= new OrmLiteNamingStrategyBase();
		}

        /// <summary>Can get data from table with namig strategy with query by example.</summary>
		[Test]
		public void Can_get_data_from_TableWithNamigStrategy_with_query_by_example()
		{
			OrmLiteConfig.DialectProvider.NamingStrategy = OrmLiteConfig.DialectProvider.NamingStrategy = new PrefixNamingStrategy
			{
				TablePrefix = "tab_",
				ColumnPrefix = "col_",
			};

			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithOnlyStringFields>(true);
				var m = new ModelWithOnlyStringFields { Id = "998", AlbumId = "112", AlbumName = "ElectroShip", Name = "QueryByExample" };

				db.Save(m);
				var modelFromDb = db.Where<ModelWithOnlyStringFields>(new { Name = "QueryByExample" })[0];

				Assert.AreEqual(m.Name, modelFromDb.Name);
			}
			
			OrmLiteConfig.DialectProvider.NamingStrategy= new OrmLiteNamingStrategyBase();
		}

        /// <summary>
        /// Can get data from table with underscore separated compound naming strategy with read
        /// connection extension.
        /// </summary>
		[Test]
		public void Can_get_data_from_TableWithUnderscoreSeparatedCompoundNamingStrategy_with_ReadConnectionExtension()
		{
			OrmLiteConfig.DialectProvider.NamingStrategy = new UnderscoreSeparatedCompoundNamingStrategy();

			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithOnlyStringFields>(true);
				var m = new ModelWithOnlyStringFields
							{
								Id = "997",
								AlbumId = "112",
								AlbumName = "ElectroShip",
								Name = "ReadConnectionExtensionFirst"
							};
				db.Save(m);
				var modelFromDb = db.First<ModelWithOnlyStringFields>(x => x.Name == "ReadConnectionExtensionFirst");
				Assert.AreEqual(m.AlbumName, modelFromDb.AlbumName);
			}

            OrmLiteConfig.DialectProvider.NamingStrategy = new OrmLiteNamingStrategyBase();
        }

        /// <summary>
        /// Can get data from table with namig strategy after changing naming strategy.
        /// </summary>
		[Test]
		public void Can_get_data_from_TableWithNamigStrategy_AfterChangingNamingStrategy()
		{			
			OrmLiteConfig.DialectProvider.NamingStrategy = OrmLiteConfig.DialectProvider.NamingStrategy = new PrefixNamingStrategy
			{
				TablePrefix = "tab_",
				ColumnPrefix = "col_",
			};

			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithOnlyStringFields>(true);
				var m = new ModelWithOnlyStringFields() { Id = "998", AlbumId = "112", AlbumName = "ElectroShip", Name = "QueryByExample" };

				db.Save(m);
				var modelFromDb = db.Where<ModelWithOnlyStringFields>(new { Name = "QueryByExample" })[0];

				Assert.AreEqual(m.Name, modelFromDb.Name);
				
				modelFromDb =  db.GetById<ModelWithOnlyStringFields>("998");
				Assert.AreEqual(m.Name, modelFromDb.Name);
				
			}
			
			OrmLiteConfig.DialectProvider.NamingStrategy= new OrmLiteNamingStrategyBase();
			
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithOnlyStringFields>(true);
				var m = new ModelWithOnlyStringFields() { Id = "998", AlbumId = "112", AlbumName = "ElectroShip", Name = "QueryByExample" };

				db.Save(m);
				var modelFromDb = db.Where<ModelWithOnlyStringFields>(new { Name = "QueryByExample" })[0];

				Assert.AreEqual(m.Name, modelFromDb.Name);
				
				modelFromDb =  db.GetById<ModelWithOnlyStringFields>("998");
				Assert.AreEqual(m.Name, modelFromDb.Name);	
			}
			
			OrmLiteConfig.DialectProvider.NamingStrategy = OrmLiteConfig.DialectProvider.NamingStrategy = new PrefixNamingStrategy
			{
				TablePrefix = "tab_",
				ColumnPrefix = "col_",
			};

			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithOnlyStringFields>(true);
				var m = new ModelWithOnlyStringFields() { Id = "998", AlbumId = "112", AlbumName = "ElectroShip", Name = "QueryByExample" };

				db.Save(m);
				var modelFromDb = db.Where<ModelWithOnlyStringFields>(new { Name = "QueryByExample" })[0];

				Assert.AreEqual(m.Name, modelFromDb.Name);
				
				modelFromDb =  db.GetById<ModelWithOnlyStringFields>("998");
				Assert.AreEqual(m.Name, modelFromDb.Name);
			}
			
			OrmLiteConfig.DialectProvider.NamingStrategy = new OrmLiteNamingStrategyBase();
		}

	}

    /// <summary>A prefix naming strategy.</summary>
	public class PrefixNamingStrategy : OrmLiteNamingStrategyBase
	{
        /// <summary>Gets or sets the table prefix.</summary>
        /// <value>The table prefix.</value>
		public string TablePrefix { get; set; }

        /// <summary>Gets or sets the column prefix.</summary>
        /// <value>The column prefix.</value>
		public string ColumnPrefix { get; set; }

        /// <summary>Gets table name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The table name.</returns>
		public override string GetTableName(string name)
		{
			return TablePrefix + name;
		}

        /// <summary>Gets column name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The column name.</returns>
		public override string GetColumnName(string name)
		{
			return ColumnPrefix + name;
		}

	}

    /// <summary>A lowercase naming strategy.</summary>
	public class LowercaseNamingStrategy : OrmLiteNamingStrategyBase
	{
        /// <summary>Gets table name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The table name.</returns>
		public override string GetTableName(string name)
		{
			return name.ToLower();
		}

        /// <summary>Gets column name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The column name.</returns>
		public override string GetColumnName(string name)
		{
			return name.ToLower();
		}

	}

    /// <summary>An underscore separated compound naming strategy.</summary>
	public class UnderscoreSeparatedCompoundNamingStrategy : OrmLiteNamingStrategyBase
	{
        /// <summary>Gets table name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The table name.</returns>
		public override string GetTableName(string name)
		{
			return toUnderscoreSeparatedCompound(name);
		}

        /// <summary>Gets column name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The column name.</returns>
		public override string GetColumnName(string name)
		{
			return toUnderscoreSeparatedCompound(name);
		}

        /// <summary>Converts a name to an underscore separated compound.</summary>
        /// <param name="name">The name.</param>
        /// <returns>name as a string.</returns>
		string toUnderscoreSeparatedCompound(string name)
		{

			string r = char.ToLower(name[0]).ToString();

			for (int i = 1; i < name.Length; i++)
			{
				char c = name[i];
				if (char.IsUpper(name[i]))
				{
					r += "_";
					r += char.ToLower(name[i]);
				}
				else
				{
					r += name[i];
				}
			}
			return r;
		}

	}

}