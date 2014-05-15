using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite.Tests;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.PostgreSQL.Tests
{
    /// <summary>A create postgre SQL tables tests.</summary>
    [TestFixture]
    public class CreatePostgreSQLTablesTests : OrmLiteTestBase
    {
        /// <summary>Drop and create table drops table and creates table.</summary>
		[Test]
		public void DropAndCreateTable_DropsTableAndCreatesTable()
		{
			using (var db = OpenDbConnection())
			{
				db.DropTable<TestData>();
				db.CreateTable<TestData>();
				db.InsertParam<TestData>(new TestData { Id = Guid.NewGuid() });
				db.DropAndCreateTable<TestData>();
				db.InsertParam<TestData>(new TestData { Id = Guid.NewGuid() });
			}
		}

        /// <summary>Can create tables after use unicode or default string lenght changed.</summary>
        [Test]
        public void can_create_tables_after_UseUnicode_or_DefaultStringLenght_changed()
        {
            //first one passes
            _reCreateTheTable();

            //all of these pass now:
            OrmLiteConfig.DialectProvider.UseUnicode = true;
            _reCreateTheTable();

            OrmLiteConfig.DialectProvider.UseUnicode = false;
            _reCreateTheTable();

            OrmLiteConfig.DialectProvider.DefaultStringLength = 98765;

            _reCreateTheTable();
        }

        /// <summary>Re create the table.</summary>
        private void _reCreateTheTable()
        {
            using(var db = OpenDbConnection()) {
                db.CreateTable<CreatePostgreSQLTablesTests_dummy_table>(true);
            }
        }

        /// <summary>A create postgre SQL tables tests dummy table.</summary>
        private class CreatePostgreSQLTablesTests_dummy_table
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement]
            public int Id { get; set; }

            /// <summary>Gets or sets the length of the string no explicit.</summary>
            /// <value>The length of the string no explicit.</value>
            public String StringNoExplicitLength { get; set; }

            /// <summary>Gets or sets the string 100 characters.</summary>
            /// <value>The string 100 characters.</value>
            [StringLength(100)]
            public String String100Characters { get; set; }
        }

        /// <summary>
        /// Can create same table in multiple schemas based on connection string search path.
        /// </summary>
        [Test]
        public void can_create_same_table_in_multiple_schemas_based_on_conn_string_search_path()
        {
            var builder = new Npgsql.NpgsqlConnectionStringBuilder(ConnectionString);
            var schema1 = "schema_1";
            var schema2 = "schema_2";
            using (var db = OpenDbConnection())
            {
                CreateSchemaIfNotExists(db, schema1);
                CreateSchemaIfNotExists(db, schema2);
            }

            builder.SearchPath = schema1;
            using (var dbS1 = builder.ToString().OpenDbConnection())
            {
                dbS1.DropTable<CreatePostgreSQLTablesTests_dummy_table>();
                dbS1.CreateTable<CreatePostgreSQLTablesTests_dummy_table>();
                Assert.That(dbS1.Count<CreatePostgreSQLTablesTests_dummy_table>(), Is.EqualTo(0));
            }
            builder.SearchPath = schema2;

            using (var dbS2 = builder.ToString().OpenDbConnection())
            {
                dbS2.DropTable<CreatePostgreSQLTablesTests_dummy_table>();
                dbS2.CreateTable<CreatePostgreSQLTablesTests_dummy_table>();
                Assert.That(dbS2.Count<CreatePostgreSQLTablesTests_dummy_table>(), Is.EqualTo(0));
            }

        }

        /// <summary>A test data.</summary>
		public class TestData
		{
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
			[PrimaryKey]
			public Guid Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
			public string Name { get; set; }

            /// <summary>Gets or sets the person's surname.</summary>
            /// <value>The surname.</value>
			public string Surname { get; set; }
		}

        /// <summary>Creates schema if not exists.</summary>
        /// <param name="db">  The database.</param>
        /// <param name="name">The name.</param>
        private void CreateSchemaIfNotExists(System.Data.IDbConnection db, string name)
        {
            string createSchemaSQL = @"DO $$
BEGIN

    IF NOT EXISTS(
        SELECT schema_name
          FROM information_schema.schemata
          WHERE schema_name = '{0}'
      )
    THEN
      EXECUTE 'CREATE SCHEMA ""{0}""';
    END IF;

END
$$;"
                .Fmt(name);
            db.ExecuteSql(createSchemaSQL);
        }
    }
}
