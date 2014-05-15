using System;
using NUnit.Framework;
using NServiceKit.OrmLite;
using NServiceKit.OrmLite.Firebird;
using NServiceKit.OrmLite.MySql;
using NServiceKit.OrmLite.SqlServer;
using System.Collections.Generic;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using System.ComponentModel.DataAnnotations;

namespace NServiceKit.OrmLite.DDLTest
{
    /// <summary>A test.</summary>
	[TestFixture()]
	public class Test
	{
        /// <summary>The dialects.</summary>
		List<Dialect> dialects = new List<Dialect>();

        /// <summary>Tests fixture setup.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			dialects.Add ( new Dialect{
				Provider= FirebirdOrmLiteDialectProvider.Instance, 
				AddColumnString="ALTER TABLE Model ADD Column1 VARCHAR(128) ;",
				AlterColumnString="ALTER TABLE Model ALTER Column2 VARCHAR(50) ;",
				ChangeColumnNameString="ALTER TABLE Model ALTER OldColumn3 TO Column3 ;",
				AddFKString="ALTER TABLE Child ADD CONSTRAINT JustOneFK FOREIGN KEY (IdModel) REFERENCES Model (Id) ON DELETE CASCADE ON UPDATE NO ACTION;",
				AddFKRestrictString="ALTER TABLE Child ADD CONSTRAINT JustOneMoreFK FOREIGN KEY (IdModel) REFERENCES Model (Id) ON UPDATE NO ACTION;",
				CreateIndexString="CREATE UNIQUE INDEX JustIndexOnColumn3 ON Model(Column3);"

			});
			dialects.Add ( new Dialect{
				Provider= MySqlDialectProvider.Instance,
				AddColumnString="ALTER TABLE `Model` ADD COLUMN `Column1` VARCHAR(255) NULL;",
				AlterColumnString="ALTER TABLE `Model` MODIFY COLUMN `Column2` VARCHAR(50) NULL;",
				ChangeColumnNameString="ALTER TABLE `Model` CHANGE COLUMN `OldColumn3` `Column3` VARCHAR(255) NULL;",
				AddFKString="ALTER TABLE `Child` ADD CONSTRAINT `JustOneFK` FOREIGN KEY (`IdModel`) REFERENCES `Model` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION;",
				AddFKRestrictString="ALTER TABLE `Child` ADD CONSTRAINT `JustOneMoreFK` FOREIGN KEY (`IdModel`) REFERENCES `Model` (`Id`) ON DELETE RESTRICT ON UPDATE NO ACTION;",
				CreateIndexString="CREATE UNIQUE INDEX `JustIndexOnColumn3` ON `Model`(`Column3`);"
			});

			dialects.Add ( new Dialect{
				Provider= SqlServerOrmLiteDialectProvider.Instance,
				AddColumnString = @"ALTER TABLE ""Model"" ADD ""Column1"" VARCHAR(8000) NULL;",
				AlterColumnString = @"ALTER TABLE ""Model"" ALTER COLUMN ""Column2"" VARCHAR(50) NULL;",
				ChangeColumnNameString = @"EXEC sp_rename 'Model.OldColumn3', 'Column3', 'COLUMN';",
				AddFKString = @"ALTER TABLE ""Child"" ADD CONSTRAINT ""JustOneFK"" FOREIGN KEY (""IdModel"") REFERENCES ""Model"" (""Id"") ON DELETE CASCADE ON UPDATE NO ACTION;",
				AddFKRestrictString = @"ALTER TABLE ""Child"" ADD CONSTRAINT ""JustOneMoreFK"" FOREIGN KEY (""IdModel"") REFERENCES ""Model"" (""Id"") ON UPDATE NO ACTION;",
				CreateIndexString = @"CREATE UNIQUE INDEX ""JustIndexOnColumn3"" ON ""Model""(""Column3"");"
			});

			LogManager.LogFactory = new ConsoleLogFactory();
		}

        /// <summary>Can add column.</summary>
		[Test()]
		public void CanAddColumn ()
		{
			var model = typeof(Model);

			foreach (var d in dialects) 
			{
				OrmLiteConfig.DialectProvider=d.Provider;
				var fielDef = ModelDefinition<Model>.Definition.GetFieldDefinition<Model> (f => f.Column1);
				Assert.AreEqual(d.AddColumnString, (d.Provider.ToAddColumnStatement(model, fielDef)));
			}
		}

        /// <summary>Can an alter column.</summary>
		[Test()]
		public void CanAAlterColumn ()
		{
			var model = typeof(Model);
			
			foreach (var d in dialects) 
			{
				OrmLiteConfig.DialectProvider=d.Provider;
				var fielDef = ModelDefinition<Model>.Definition.GetFieldDefinition<Model> (f => f.Column2);
				Assert.AreEqual(d.AlterColumnString, (d.Provider.ToAlterColumnStatement(model, fielDef)));
			}
			
		}

        /// <summary>Can change column name.</summary>
		[Test()]
		public void CanChangeColumnName ()
		{
			var model = typeof(Model);
			
			foreach (var d in dialects) 
			{
				OrmLiteConfig.DialectProvider=d.Provider;
				var fielDef = ModelDefinition<Model>.Definition.GetFieldDefinition<Model> (f => f.Column3);
				Assert.AreEqual(d.ChangeColumnNameString,(d.Provider.ToChangeColumnNameStatement(model, fielDef,"OldColumn3")));

			}
			
		}

        /// <summary>Can add foreign key.</summary>
		[Test()]
		public void CanAddForeignKey ()
		{
			
			foreach (var d in dialects) 
			{
				OrmLiteConfig.DialectProvider=d.Provider;		
				Assert.AreEqual(d.AddFKString,
				                d.Provider.ToAddForeignKeyStatement<Child,Model>(f=>f.IdModel,
				                                                 fk=>fk.Id,OnFkOption.NoAction,OnFkOption.Cascade, "JustOneFK"));
			}
		}

        /// <summary>Can add foreign key restrict.</summary>
		[Test()]
		public void CanAddForeignKeyRestrict ()
		{
			
			foreach (var d in dialects) 
			{
				OrmLiteConfig.DialectProvider=d.Provider;		
				Assert.AreEqual(d.AddFKRestrictString,
				                d.Provider.ToAddForeignKeyStatement<Child,Model>(f=>f.IdModel,
				                                                 fk=>fk.Id,OnFkOption.NoAction,OnFkOption.Restrict, "JustOneMoreFK"));
			}
		}

        /// <summary>Can create index.</summary>
		[Test()]
		public void CanCreateIndex ()
		{

			foreach (var d in dialects) 
			{
				OrmLiteConfig.DialectProvider=d.Provider;
				Assert.AreEqual(d.CreateIndexString, d.Provider.ToCreateIndexStatement<Model>(f=>f.Column3, "JustIndexOnColumn3", true) );
				
			}
			
		}


	}

    /// <summary>A model.</summary>
	public class Model
	{
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.DDLTest.Model class.
        /// </summary>
		public Model()
		{
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the column 1.</summary>
        /// <value>The column 1.</value>
		public string Column1{ get; set; }

        /// <summary>Gets or sets the column 2.</summary>
        /// <value>The column 2.</value>
		[StringLength(50)]
		public string Column2{ get; set; }

        /// <summary>Gets or sets the column 3.</summary>
        /// <value>The column 3.</value>
		public string Column3{ get; set; }

	}

    /// <summary>A child.</summary>
	public class Child
	{
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.DDLTest.Child class.
        /// </summary>
		public Child(){}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		public int Id{ get; set;}

        /// <summary>Gets or sets the identifier model.</summary>
        /// <value>The identifier model.</value>
		public int IdModel{ get; set;}

	}

    /// <summary>A dialect.</summary>
	public class Dialect
	{
        /// <summary>Gets or sets the provider.</summary>
        /// <value>The provider.</value>
		public IOrmLiteDialectProvider Provider { get; set; }

        /// <summary>Gets or sets the add column string.</summary>
        /// <value>The add column string.</value>
		public string AddColumnString { get; set; }

        /// <summary>Gets or sets the alter column string.</summary>
        /// <value>The alter column string.</value>
		public string AlterColumnString { get; set; }

        /// <summary>Gets or sets the change column name string.</summary>
        /// <value>The change column name string.</value>
		public string ChangeColumnNameString { get; set; }

        /// <summary>Gets or sets the add fk string.</summary>
        /// <value>The add fk string.</value>
		public string AddFKString { get; set; }

        /// <summary>Gets or sets the add fk restrict string.</summary>
        /// <value>The add fk restrict string.</value>
		public string AddFKRestrictString { get; set; }

        /// <summary>Gets or sets the create index string.</summary>
        /// <value>The create index string.</value>
		public string CreateIndexString { get; set; }

	}
}