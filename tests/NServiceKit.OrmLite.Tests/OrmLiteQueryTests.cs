using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NServiceKit.Common.Tests.Models;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>An ORM lite query tests.</summary>
	[TestFixture]
	public class OrmLiteQueryTests
		: OrmLiteTestBase
	{
        /// <summary>
        /// Can get by identifier int from model with fields of different types table.
        /// </summary>
		[Test]
		public void Can_GetById_int_from_ModelWithFieldsOfDifferentTypes_table()
		{
			using (var db = OpenDbConnection())
			{
                db.DropAndCreateTable<ModelWithFieldsOfDifferentTypes>();

				var rowIds = new List<int>(new[] { 1, 2, 3 });

				rowIds.ForEach(x => db.Insert(ModelWithFieldsOfDifferentTypes.Create(x)));

				var row = db.QueryById<ModelWithFieldsOfDifferentTypes>(1);

				Assert.That(row.Id, Is.EqualTo(1));
			}
		}

        /// <summary>Can get by identifier string from model with only string fields table.</summary>
		[Test]
		public void Can_GetById_string_from_ModelWithOnlyStringFields_table()
		{
			using (var db = OpenDbConnection())
			{
                db.DropAndCreateTable<ModelWithOnlyStringFields>();

				var rowIds = new List<string>(new[] { "id-1", "id-2", "id-3" });

				rowIds.ForEach(x => db.Insert(ModelWithOnlyStringFields.Create(x)));

				var row = db.QueryById<ModelWithOnlyStringFields>("id-1");

				Assert.That(row.Id, Is.EqualTo("id-1"));
			}
		}

        /// <summary>Can select with filter from model with only string fields table.</summary>
		[Test]
		public void Can_select_with_filter_from_ModelWithOnlyStringFields_table()
		{
			using (var db = OpenDbConnection())
			{
                db.DropAndCreateTable<ModelWithOnlyStringFields>();

				var rowIds = new List<string>(new[] { "id-1", "id-2", "id-3" });

				rowIds.ForEach(x => db.Insert(ModelWithOnlyStringFields.Create(x)));

				var filterRow = ModelWithOnlyStringFields.Create("id-4");
				filterRow.AlbumName = "FilteredName";

				db.Insert(filterRow);

				var rows = db.Where<ModelWithOnlyStringFields>(new { filterRow.AlbumName });
				var dbRowIds = rows.ConvertAll(x => x.Id);
				Assert.That(dbRowIds, Has.Count.EqualTo(1));
				Assert.That(dbRowIds[0], Is.EqualTo(filterRow.Id));

				rows = db.Where<ModelWithOnlyStringFields>(new { filterRow.AlbumName });
				dbRowIds = rows.ConvertAll(x => x.Id);
				Assert.That(dbRowIds, Has.Count.EqualTo(1));
				Assert.That(dbRowIds[0], Is.EqualTo(filterRow.Id));

				var queryByExample = new ModelWithOnlyStringFields { AlbumName = filterRow.AlbumName };
				rows = db.ByExampleWhere<ModelWithOnlyStringFields>(queryByExample);
				dbRowIds = rows.ConvertAll(x => x.Id);
				Assert.That(dbRowIds, Has.Count.EqualTo(1));
				Assert.That(dbRowIds[0], Is.EqualTo(filterRow.Id));

				rows = db.Query<ModelWithOnlyStringFields>(
					"SELECT * FROM ModelWithOnlyStringFields WHERE AlbumName = @AlbumName", new { filterRow.AlbumName });
				dbRowIds = rows.ConvertAll(x => x.Id);
				Assert.That(dbRowIds, Has.Count.EqualTo(1));
				Assert.That(dbRowIds[0], Is.EqualTo(filterRow.Id));
			}
		}

        /// <summary>Can loop each with filter from model with only string fields table.</summary>
		[Test]
		public void Can_loop_each_with_filter_from_ModelWithOnlyStringFields_table()
		{
			using (var db = OpenDbConnection())
			{
                db.DropAndCreateTable<ModelWithOnlyStringFields>();

				var rowIds = new List<string>(new[] { "id-1", "id-2", "id-3" });

				rowIds.ForEach(x => db.Insert(ModelWithOnlyStringFields.Create(x)));

				var filterRow = ModelWithOnlyStringFields.Create("id-4");
				filterRow.AlbumName = "FilteredName";

				db.Insert(filterRow);

				var dbRowIds = new List<string>();
				var rows = db.EachWhere<ModelWithOnlyStringFields>(new { filterRow.AlbumName });
				foreach (var row in rows)
				{
					dbRowIds.Add(row.Id);
				}

				Assert.That(dbRowIds, Has.Count.EqualTo(1));
				Assert.That(dbRowIds[0], Is.EqualTo(filterRow.Id));
			}
		}

        /// <summary>Can get single with filter from model with only string fields table.</summary>
        [Test]
        public void Can_GetSingle_with_filter_from_ModelWithOnlyStringFields_table()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<ModelWithOnlyStringFields>();

                var rowIds = new List<string>(new[] { "id-1", "id-2", "id-3" });

                rowIds.ForEach(x => db.Insert(ModelWithOnlyStringFields.Create(x)));

                var filterRow = ModelWithOnlyStringFields.Create("id-4");
                filterRow.AlbumName = "FilteredName";

                db.Insert(filterRow);

                var row = db.QuerySingle<ModelWithOnlyStringFields>(new { filterRow.AlbumName });
                Assert.That(row.Id, Is.EqualTo(filterRow.Id));

                row = db.QuerySingle<ModelWithOnlyStringFields>(new { filterRow.AlbumName });
                Assert.That(row.AlbumName, Is.EqualTo(filterRow.AlbumName));

                row = db.QuerySingle<ModelWithOnlyStringFields>(new { AlbumName = "Junk", Id = (object)null });
                Assert.That(row, Is.Null);
            }
        }

        /// <summary>A note.</summary>
        class Note
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement] // Creates Auto primary key
            public int Id { get; set; }

            /// <summary>Gets or sets URI of the schema.</summary>
            /// <value>The schema URI.</value>
            public string SchemaUri { get; set; }

            /// <summary>Gets or sets the note text.</summary>
            /// <value>The note text.</value>
            public string NoteText { get; set; }

            /// <summary>Gets or sets the Date/Time of the last updated.</summary>
            /// <value>The last updated.</value>
            public DateTime? LastUpdated { get; set; }

            /// <summary>Gets or sets who updated this object.</summary>
            /// <value>Describes who updated this object.</value>
            public string UpdatedBy { get; set; }
        }

        /// <summary>Can query where and select notes.</summary>
        [Test]
        public void Can_query_where_and_select_Notes()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<Note>();

                db.Insert(new Note
                    {
                        SchemaUri = "tcm:0-0-0",
                        NoteText = "Hello world 5",
                        LastUpdated = new DateTime(2013, 1, 5),
                        UpdatedBy = "RC"
                    });

                var notes = db.Where<Note>(new { SchemaUri = "tcm:0-0-0" });
                Assert.That(notes[0].Id, Is.EqualTo(1));
                Assert.That(notes[0].NoteText, Is.EqualTo("Hello world 5"));

                notes = db.Select<Note>("SchemaUri={0}", "tcm:0-0-0");
                Assert.That(notes[0].Id, Is.EqualTo(1));
                Assert.That(notes[0].NoteText, Is.EqualTo("Hello world 5"));

                notes = db.Query<Note>("SELECT * FROM Note WHERE SchemaUri=@schemaUri", new { schemaUri = "tcm:0-0-0" });
                Assert.That(notes[0].Id, Is.EqualTo(1));
                Assert.That(notes[0].NoteText, Is.EqualTo("Hello world 5"));

                notes = db.Query<Note>("SchemaUri=@schemaUri", new { schemaUri = "tcm:0-0-0" });
                Assert.That(notes[0].Id, Is.EqualTo(1));
                Assert.That(notes[0].NoteText, Is.EqualTo("Hello world 5"));
            }            
        }

        /// <summary>A note dto.</summary>
        class NoteDto
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Gets or sets URI of the schema.</summary>
            /// <value>The schema URI.</value>
            public string SchemaUri { get; set; }

            /// <summary>Gets or sets the note text.</summary>
            /// <value>The note text.</value>
            public string NoteText { get; set; }
        }

        /// <summary>Can select notes dto with pretty SQL.</summary>
        [Test]
        public void Can_select_NotesDto_with_pretty_sql()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<Note>();

                db.Insert(new Note
                {
                    SchemaUri = "tcm:0-0-0",
                    NoteText = "Hello world 5",
                    LastUpdated = new DateTime(2013, 1, 5),
                    UpdatedBy = "RC"
                });

                var sql = @"
SELECT
Id, SchemaUri, NoteText
FROM Note
WHERE SchemaUri=@schemaUri
";

                var notes = db.Query<NoteDto>(sql, new { schemaUri = "tcm:0-0-0" });
                Assert.That(notes[0].Id, Is.EqualTo(1));
                Assert.That(notes[0].NoteText, Is.EqualTo("Hello world 5"));
            }
        }

        /// <summary>A customer dto.</summary>
        class CustomerDto
        {
            /// <summary>Gets or sets the identifier of the customer.</summary>
            /// <value>The identifier of the customer.</value>
            public int CustomerId { get; set; }

            /// <summary>Gets or sets the name of the customer.</summary>
            /// <value>The name of the customer.</value>
            public string @CustomerName { get; set; }

            /// <summary>Gets or sets the customer birth date.</summary>
            /// <value>The customer birth date.</value>
            public DateTime Customer_Birth_Date { get; set; }
        }

        /// <summary>
        /// Can query customer dto and map database fields not identical by guessing the mapping.
        /// </summary>
        /// <param name="field1Name">Name of the field 1.</param>
        /// <param name="field2Name">Name of the field 2.</param>
        /// <param name="field3Name">Name of the field 3.</param>
        [Test]
        [TestCase("customer_id", "customer_name", "customer_birth_date")]
        [TestCase("customerid%", "@customername", "customer_b^irth_date")]
        [TestCase("customerid_%", "@customer_name", "customer$_birth_#date")]
        [TestCase("c!u@s#t$o%m^e&r*i(d_%", "__cus_tomer__nam_e__", "~cus`tomer$_birth_#date")]
        [TestCase("t030CustomerId", "t030CustomerName", "t030Customer_birth_date")]
        [TestCase("t030_customer_id", "t030_customer_name", "t130_customer_birth_date")]
        [TestCase("t030#Customer_I#d", "t030CustomerNa$^me", "t030Cust^omer_birth_date")]
        public void Can_query_CustomerDto_and_map_db_fields_not_identical_by_guessing_the_mapping(string field1Name, string field2Name, string field3Name)
        {
            using (var db = OpenDbConnection())
            {
                var sql = string.Format(@"
                    SELECT 1 AS [{0}], 'John' AS [{1}], '1970-01-01' AS [{2}]
                    UNION ALL
                    SELECT 2 AS [{0}], 'Jane' AS [{1}], '1980-01-01' AS [{2}]",
                                        field1Name, field2Name, field3Name);

                var customers = db.Query<CustomerDto>(sql);

                Assert.That(customers.Count, Is.EqualTo(2));

                Assert.That(customers[0].CustomerId, Is.EqualTo(1));
                Assert.That(customers[0].CustomerName, Is.EqualTo("John"));
                Assert.That(customers[0].Customer_Birth_Date, Is.EqualTo(new DateTime(1970, 01, 01)));

                Assert.That(customers[1].CustomerId, Is.EqualTo(2));
                Assert.That(customers[1].CustomerName, Is.EqualTo("Jane"));
                Assert.That(customers[1].Customer_Birth_Date, Is.EqualTo(new DateTime(1980, 01, 01)));
            }
        }

	    [Test]
	    public void QueryAndQueryEachEachMustExecuteTheQueryPassedToTheMethod()
	    {
		    using (var db = OpenDbConnection())
		    {
			    const string query = @"SELECT CustomerId,
						                        CustomerName
						                 FROM CustomerDto
						                    WHERE CustomerName IS NOT NULL
						                    AND CustomerName <> ''";
				db.CreateTableIfNotExists<CustomerDto>();
				DateTime birthDate = DateTime.Now;

				db.Insert(new CustomerDto {CustomerId = 1, CustomerName = "", Customer_Birth_Date = birthDate});
				db.Insert(new CustomerDto {CustomerId = 2, CustomerName = "Test", Customer_Birth_Date = birthDate});
				IList<CustomerDto> customers = db.Query<CustomerDto>(query);
				Assert.AreEqual(1, customers.Count);
				CustomerDto customer = customers.First();
				Assert.AreEqual(2, customer.CustomerId);
				Assert.AreEqual("Test", customer.CustomerName);
				Assert.AreNotEqual(birthDate, customer.Customer_Birth_Date);
				Assert.AreEqual(DateTime.MinValue, customer.Customer_Birth_Date);

			    Assert.AreEqual(query, db.GetLastSql());
				
				customers = db.QueryEach<CustomerDto>(query).ToList();
				Assert.AreEqual(1, customers.Count);
				customer = customers.First();
				Assert.AreEqual(2, customer.CustomerId);
				Assert.AreEqual("Test", customer.CustomerName);
				Assert.AreNotEqual(birthDate, customer.Customer_Birth_Date);
				Assert.AreEqual(DateTime.MinValue, customer.Customer_Birth_Date);

			    Assert.AreEqual(query, db.GetLastSql());
		    }
	    }
	}
}