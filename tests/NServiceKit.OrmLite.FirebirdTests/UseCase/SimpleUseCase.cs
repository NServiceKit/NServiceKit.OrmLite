using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.OrmLite.Firebird;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.FirebirdTests
{
    /// <summary>A simple use case.</summary>
	[TestFixture]
	public class SimpleUseCase
	{
        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			//Inject your database provider here
			OrmLiteConfig.DialectProvider = new FirebirdOrmLiteDialectProvider();
		}

        /// <summary>An user.</summary>
		public class User
		{	
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
			public long Id { get; set; }

			[Index]

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
			public string Name { get; set; }	

            /// <summary>Gets or sets the created date.</summary>
            /// <value>The created date.</value>
			public DateTime CreatedDate { get; set; }
		}

        /// <summary>A unique identifier.</summary>
		public class GuidId
		{
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
			public Guid Id { get; set; }
		}

        /// <summary>Simple crud example.</summary>
		[Test]
		public void Simple_CRUD_example()
		{
			using (IDbConnection db = "User=SYSDBA;Password=masterkey;Database=ormlite-tests.fdb;DataSource=localhost;Dialect=3;charset=ISO8859_1;".OpenDbConnection())
			{
				db.CreateTable<User>(true);
				
				db.Insert(new User { Id = 1, Name = "A", CreatedDate = DateTime.Now });
				db.Insert(new User { Id = 2, Name = "B", CreatedDate = DateTime.Now });
				db.Insert(new User { Id = 3, Name = "B", CreatedDate = DateTime.Now });
				
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

				db.CreateTable<GuidId>(true);
				Guid g = Guid.NewGuid();
				db.Insert(new GuidId { Id = g });

				GuidId gid = db.First<GuidId>("Id = {0}", g);
				Assert.That(g == gid.Id);
			}
		}

	}
	
}
