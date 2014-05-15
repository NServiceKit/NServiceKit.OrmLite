namespace NServiceKit.OrmLite.Tests.UseCase
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;

    using NUnit.Framework;

    using NServiceKit.Common.Utils;
    using NServiceKit.DataAnnotations;
    using NServiceKit.OrmLite.Sqlite;

    /// <summary>A password use case.</summary>
    [TestFixture]
    public class PasswordUseCase {

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            //Inject your database provider here
            //OrmLiteConfig.DialectProvider = new SqliteOrmLiteDialectProvider();
        }

        /// <summary>An user.</summary>
        public class User {

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
        }

        /// <summary>A user 2.</summary>
        [Alias("Users")]
        public class User2 {

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement]
            public long Id { get; set; }

            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            public long Value { get; set; }
        }

        /// <summary>Simple crud example.</summary>
        [Test]
        public void Simple_CRUD_example() {
            var path = Config.SqliteFileDb;
            if (File.Exists(path))
                File.Delete(path);
            var connectionFactory = new OrmLiteConnectionFactory(path, SqliteOrmLiteDialectProvider.Instance.WithPassword("bob"));
            using (var db = connectionFactory.OpenDbConnection())
            {
                db.CreateTable<User>(true);

                db.Insert(new User { Id = 1, Name = "A", CreatedDate = DateTime.Now });
                db.Insert(new User { Id = 2, Name = "B", CreatedDate = DateTime.Now });
                db.Insert(new User { Id = 3, Name = "B", CreatedDate = DateTime.Now });

                var rowsB = db.Select<User>("Name = {0}", "B");
                var rowsB1 = db.Select<User>(user => user.Name == "B");

                Assert.That(rowsB, Has.Count.EqualTo(2));
                Assert.That(rowsB1, Has.Count.EqualTo(2));

                var rowIds = rowsB.ConvertAll(x => x.Id);
                Assert.That(rowIds, Is.EquivalentTo(new List<long> { 2, 3 }));

                rowsB.ForEach(x => db.Delete(x));

                rowsB = db.Select<User>("Name = {0}", "B");
                Assert.That(rowsB, Has.Count.EqualTo(0));

                var rowsLeft = db.Select<User>();
                Assert.That(rowsLeft, Has.Count.EqualTo(1));

                Assert.That(rowsLeft[0].Name, Is.EqualTo("A"));
            }
            File.Delete(path);

        }
    }
}