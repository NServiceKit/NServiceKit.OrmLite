using System;
using System.Collections.Generic;
using Northwind.Common.DataModel;
using NUnit.Framework;
using NServiceKit.Common.Tests.Models;
using NServiceKit.DataAnnotations;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.FirebirdTests
{
    /// <summary>An ORM lite insert tests.</summary>
	[TestFixture]
	public class OrmLiteInsertTests
		: OrmLiteTestBase
	{
        /// <summary>Can insert into model with fields of different types table.</summary>
		[Test]
		public void Can_insert_into_ModelWithFieldsOfDifferentTypes_table()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<ModelWithFieldsOfDifferentTypes>(true);

				var row = ModelWithFieldsOfDifferentTypes.Create(1);

				db.Insert(row);
			}
		}

        /// <summary>Can insert and select from model with fields of different types table.</summary>
		[Test]
		public void Can_insert_and_select_from_ModelWithFieldsOfDifferentTypes_table()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<ModelWithFieldsOfDifferentTypes>(true);

				var row = ModelWithFieldsOfDifferentTypes.Create(1);

				db.Insert(row);

				var rows = db.Select<ModelWithFieldsOfDifferentTypes>();

				Assert.That(rows, Has.Count.EqualTo(1));

				ModelWithFieldsOfDifferentTypes.AssertIsEqual(rows[0], row);
			}
		}

        /// <summary>Can insert and select from model with fields of nullable types table.</summary>
		[Test]
		public void Can_insert_and_select_from_ModelWithFieldsOfNullableTypes_table()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<ModelWithFieldsOfNullableTypes>(true);

				var row = ModelWithFieldsOfNullableTypes.Create(1);

				db.Insert(row);

				var rows = db.Select<ModelWithFieldsOfNullableTypes>();

				Assert.That(rows, Has.Count.EqualTo(1));

				ModelWithFieldsOfNullableTypes.AssertIsEqual(rows[0], row);
			}
		}

        /// <summary>
        /// Can insert and select from model with fields of different and nullable types table default
        /// unique identifier.
        /// </summary>
        [Test]
        public void Can_insert_and_select_from_ModelWithFieldsOfDifferentAndNullableTypes_table_default_GUID()
        {
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
                Can_insert_and_select_from_ModelWithFieldsOfDifferentAndNullableTypes_table_impl(db);
        }

        /// <summary>
        /// Can insert and select from model with fields of different and nullable types table compact
        /// unique identifier.
        /// </summary>
        [Test]
		public void Can_insert_and_select_from_ModelWithFieldsOfDifferentAndNullableTypes_table_compact_GUID()
        {
            Firebird.FirebirdOrmLiteDialectProvider dialect = new Firebird.FirebirdOrmLiteDialectProvider(true);
            OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(ConnectionString, dialect);
            using (var db = factory.CreateDbConnection())
            {
                db.Open();
                Can_insert_and_select_from_ModelWithFieldsOfDifferentAndNullableTypes_table_impl(db);
            }
        }

        /// <summary>
        /// Can insert and select from model with fields of different and nullable types table
        /// implementation.
        /// </summary>
        /// <param name="db">The database.</param>
		private void Can_insert_and_select_from_ModelWithFieldsOfDifferentAndNullableTypes_table_impl(System.Data.IDbConnection db)
		{
			{
				db.CreateTable<ModelWithFieldsOfDifferentAndNullableTypes>(true);

				var row = ModelWithFieldsOfDifferentAndNullableTypes.Create(1);
				
				Console.WriteLine(OrmLiteConfig.DialectProvider.ToInsertRowStatement(null, row));
				db.Insert(row);

				var rows = db.Select<ModelWithFieldsOfDifferentAndNullableTypes>();

				Assert.That(rows, Has.Count.EqualTo(1));

				ModelWithFieldsOfDifferentAndNullableTypes.AssertIsEqual(rows[0], row);
			}
		}

        /// <summary>Can insert table with null fields.</summary>
		[Test]
		public void Can_insert_table_with_null_fields()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<ModelWithIdAndName>(true);
				db.DeleteAll<ModelWithIdAndName>();
				var row = ModelWithIdAndName.Create(0);
				row.Name = null;

				db.Insert(row);

				var rows = db.Select<ModelWithIdAndName>();

				Assert.That(rows, Has.Count.EqualTo(1));

				ModelWithIdAndName.AssertIsEqual(rows[0], row);
			}
		}

        /// <summary>Can retrieve last insert identifier from inserted table.</summary>
		[Test]
		public void Can_retrieve_LastInsertId_from_inserted_table()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<ModelWithIdAndName>(true);

				var row1 = ModelWithIdAndName.Create(5);
				var row2 = ModelWithIdAndName.Create(6);

				db.Insert(row1);
				var row1LastInsertId = db.GetLastInsertId();

				db.Insert(row2);
				var row2LastInsertId = db.GetLastInsertId();

				var insertedRow1 = db.GetById<ModelWithIdAndName>(row1LastInsertId);
				var insertedRow2 = db.GetById<ModelWithIdAndName>(row2LastInsertId);

				Assert.That(insertedRow1.Name, Is.EqualTo(row1.Name));
				Assert.That(insertedRow2.Name, Is.EqualTo(row2.Name));
			}
		}

        /// <summary>Can insert task queue table.</summary>
		[Test]
		public void Can_insert_TaskQueue_table()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<TaskQueue>(true);

				var row = TaskQueue.Create(1);

				db.Insert(row);

				var rows = db.Select<TaskQueue>();

				Assert.That(rows, Has.Count.EqualTo(1));

				//Update the auto-increment id
				row.Id = rows[0].Id;

				TaskQueue.AssertIsEqual(rows[0], row);
			}
		}

        /// <summary>Can insert table with blobs.</summary>
		[Test]
		public void Can_insert_table_with_blobs()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				var dsl= OrmLiteConfig.DialectProvider.DefaultStringLength;
				OrmLiteConfig.DialectProvider.DefaultStringLength=1024;
				
				db.CreateTable<OrderBlob>(true);
				OrmLiteConfig.DialectProvider.DefaultStringLength=dsl;

				var row = OrderBlob.Create(1);

				db.Insert(row);

				var rows = db.Select<OrderBlob>();

				Assert.That(rows, Has.Count.EqualTo(1));

				var newRow = rows[0];

				Assert.That(newRow.Id, Is.EqualTo(row.Id));
				Assert.That(newRow.Customer.Id, Is.EqualTo(row.Customer.Id));
				Assert.That(newRow.Employee.Id, Is.EqualTo(row.Employee.Id));
				Assert.That(newRow.IntIds, Is.EquivalentTo(row.IntIds));
				Assert.That(newRow.CharMap, Is.EquivalentTo(row.CharMap));
				Assert.That(newRow.OrderDetails.Count, Is.EqualTo(row.OrderDetails.Count));
				Assert.That(newRow.OrderDetails[0].ProductId, Is.EqualTo(row.OrderDetails[0].ProductId));
				Assert.That(newRow.OrderDetails[1].ProductId, Is.EqualTo(row.OrderDetails[1].ProductId));
				Assert.That(newRow.OrderDetails[2].ProductId, Is.EqualTo(row.OrderDetails[2].ProductId));
			}
		}

        /// <summary>A user authentication.</summary>
		public class UserAuth
		{
            /// <summary>
            /// Initializes a new instance of the
            /// NServiceKit.OrmLite.FirebirdTests.OrmLiteInsertTests.UserAuth class.
            /// </summary>
			public UserAuth()
			{
				this.Roles = new List<string>();
				this.Permissions = new List<string>();
			}

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
			[AutoIncrement]
			public virtual int Id { get; set; }

            /// <summary>Gets or sets the name of the user.</summary>
            /// <value>The name of the user.</value>
			public virtual string UserName { get; set; }

            /// <summary>Gets or sets the email.</summary>
            /// <value>The email.</value>
			public virtual string Email { get; set; }

            /// <summary>Gets or sets the primary email.</summary>
            /// <value>The primary email.</value>
			public virtual string PrimaryEmail { get; set; }

            /// <summary>Gets or sets the person's first name.</summary>
            /// <value>The name of the first.</value>
			public virtual string FirstName { get; set; }

            /// <summary>Gets or sets the person's last name.</summary>
            /// <value>The name of the last.</value>
			public virtual string LastName { get; set; }

            /// <summary>Gets or sets the name of the display.</summary>
            /// <value>The name of the display.</value>
			public virtual string DisplayName { get; set; }

            /// <summary>Gets or sets the salt.</summary>
            /// <value>The salt.</value>
			public virtual string Salt { get; set; }

            /// <summary>Gets or sets the password hash.</summary>
            /// <value>The password hash.</value>
			public virtual string PasswordHash { get; set; }

            /// <summary>Gets or sets the roles.</summary>
            /// <value>The roles.</value>
			public virtual List<string> Roles { get; set; }

            /// <summary>Gets or sets the permissions.</summary>
            /// <value>The permissions.</value>
			public virtual List<string> Permissions { get; set; }

            /// <summary>Gets or sets the created date.</summary>
            /// <value>The created date.</value>
			public virtual DateTime CreatedDate { get; set; }

            /// <summary>Gets or sets the modified date.</summary>
            /// <value>The modified date.</value>
			public virtual DateTime ModifiedDate { get; set; }

            /// <summary>Gets or sets the meta.</summary>
            /// <value>The meta.</value>
			public virtual Dictionary<string, string> Meta { get; set; }
		}

        /// <summary>Can insert table with user authentication.</summary>
		[Test]
		public void Can_insert_table_with_UserAuth()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<UserAuth>(true);
				
				var jsv = "{Id:0,UserName:UserName,Email:as@if.com,PrimaryEmail:as@if.com,FirstName:FirstName,LastName:LastName,DisplayName:DisplayName,Salt:WMQi/g==,PasswordHash:oGdE40yKOprIgbXQzEMSYZe3vRCRlKGuqX2i045vx50=,Roles:[],Permissions:[],CreatedDate:2012-03-20T07:53:48.8720739Z,ModifiedDate:2012-03-20T07:53:48.8720739Z}";
				var userAuth = jsv.To<UserAuth>();

				db.Insert(userAuth);

				var rows = db.Select<UserAuth>(q => q.UserName == "UserName");

				Console.WriteLine(rows[0].Dump());

				Assert.That(rows[0].UserName, Is.EqualTo(userAuth.UserName));
			}
		}
		
		
	}

}