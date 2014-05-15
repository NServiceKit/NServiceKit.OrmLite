using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.OrmLite.SqlServer;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.SqlServerTests.UseCase
{
    /// <summary>A simple use case.</summary>
    [TestFixture, NUnit.Framework.Ignore]
    public class SimpleUseCase
    {
        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //Inject your database provider here
            OrmLiteConfig.DialectProvider = new SqlServerOrmLiteDialectProvider();
        }

        /// <summary>An user.</summary>
        public class User
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public long Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            [Index]
            public string Name { get; set; }

            /// <summary>Gets or sets the created date.</summary>
            /// <value>The created date.</value>
            public DateTime CreatedDate { get; set; }

            /// <summary>Gets or sets a value indicating whether this object is admin.</summary>
            /// <value>true if this object is admin, false if not.</value>
            public bool IsAdmin { get; set; }
        }

        /// <summary>A dual.</summary>
        public class Dual
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement]
            public int Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }
        }

        /// <summary>Simple crud example.</summary>
        [Test]
        public void Simple_CRUD_example()
        {
            //using (IDbConnection db = ":memory:".OpenDbConnection())

            var connStr = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;
            var sqlServerFactory = new OrmLiteConnectionFactory(connStr, SqlServerOrmLiteDialectProvider.Instance);

            using (IDbConnection db = sqlServerFactory.OpenDbConnection())
            {
                db.CreateTable<Dual>(true);
                db.CreateTable<User>(true);

                db.Insert(new User { Id = 1, Name = "A", CreatedDate = DateTime.Now });
                db.Insert(new User { Id = 2, Name = "B", CreatedDate = DateTime.Now });
                db.Insert(new User { Id = 3, Name = "B", CreatedDate = DateTime.Now, IsAdmin = true});

                db.Insert(new Dual { Name = "Dual" });
                var lastInsertId = db.GetLastInsertId();
                Assert.That(lastInsertId, Is.GreaterThan(0));

                var rowsB = db.Select<User>("Name = {0}", "B");

                Assert.That(rowsB, Has.Count.EqualTo(2));

                var admin = db.Select<User>("IsAdmin = {0}", true);
                Assert.That(admin[0].Id, Is.EqualTo(3));

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