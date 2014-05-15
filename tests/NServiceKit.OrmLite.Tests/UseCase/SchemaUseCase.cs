using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.Tests.UseCase
{
    /// <summary>A schema use case.</summary>
    [TestFixture]
    public class SchemaUseCase
    {
        /// <summary>An user.</summary>
        [Alias("Users")]
        [Schema("Security")]
        public class User
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement]
            public int Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            [Index]
            public string Name { get; set; }

            /// <summary>Gets or sets the created date.</summary>
            /// <value>The created date.</value>
            public DateTime CreatedDate { get; set; }
        }

        /// <summary>Can create tables with schema in sqlite.</summary>
        [Test]
        public void Can_Create_Tables_With_Schema_In_Sqlite()
        {
            OrmLiteConfig.DialectProvider = SqliteDialect.Provider;

            using (IDbConnection db = ":memory:".OpenDbConnection())
            {
                db.CreateTable<User>(true);

                var tables =
                    db.GetFirstColumn<string>
                        (@"SELECT name FROM sqlite_master WHERE type='table';");

                //sqlite dialect should just concatenate the schema and table name to create a unique table name
                Assert.That(tables.Contains("Security_Users"));
            }
        }

        /// <summary>Creates schema if not exists.</summary>
        /// <param name="db">The database.</param>
        private void CreateSchemaIfNotExists(IDbConnection db)
        {
            //in Sql2008, CREATE SCHEMA must be the first statement in a batch
            const string createSchemaSQL = @"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Security')
                                        BEGIN
                                        EXEC( 'CREATE SCHEMA Security' );
                                        END";
            db.ExecuteSql(createSchemaSQL);
        }

        /// <summary>Can create tables with schema in SQL server.</summary>
        [Test]
        public void Can_Create_Tables_With_Schema_in_SqlServer()
        {
            var dbFactory = OrmLiteTestBase.CreateSqlServerDbFactory();
            using (IDbConnection db = dbFactory.OpenDbConnection())
            {
                CreateSchemaIfNotExists(db);
                db.DropAndCreateTable<User>();

                var tables = db.GetFirstColumn<string>
                    (@"SELECT '['+SCHEMA_NAME(schema_id)+'].['+name+']' AS SchemaTable FROM sys.tables");

                //sql server dialect should create the table in the schema
                Assert.That(tables.Contains("[Security].[Users]"));
            }
        }

        /// <summary>Can perform crud operations on table with schema.</summary>
        [Test]
        public void Can_Perform_CRUD_Operations_On_Table_With_Schema()
        {
            var dbFactory = OrmLiteTestBase.CreateSqlServerDbFactory();
            using (IDbConnection db = dbFactory.OpenDbConnection())
            {
                CreateSchemaIfNotExists(db);
                db.CreateTable<User>(true);

				db.Insert(new User { Id = 1, Name = "A", CreatedDate = DateTime.Now });
                db.Insert(new User { Id = 2, Name = "B", CreatedDate = DateTime.Now });
                db.Insert(new User { Id = 3, Name = "B", CreatedDate = DateTime.Now });

                var lastInsertId = db.GetLastInsertId();
                Assert.That(lastInsertId, Is.GreaterThan(0));

                var rowsB = db.Select<User>("Name = {0}", "B");
                Assert.That(rowsB, Has.Count.EqualTo(2));

                var rowIds = rowsB.ConvertAll(x => x.Id);
                Assert.That(rowIds, Is.EquivalentTo(new List<long> { 2, 3 }));

                rowsB.ForEach(x => db.Delete(x));

                rowsB = db.Select<User>("Name = {0}", "B");
                Assert.That(rowsB, Has.Count.EqualTo(0));

                var rowsLeft = db.Select<User>();
                Assert.That(rowsLeft, Has.Count.EqualTo(1));

                Assert.That(rowsLeft[0].Name, Is.EqualTo("A"));
            }
        }
    }
}