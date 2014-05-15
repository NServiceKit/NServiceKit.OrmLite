using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite.SqlServer;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>A SQL builder tests.</summary>
    [TestFixture]
    public class SqlBuilderTests
    {
        /// <summary>An user.</summary>
        [Alias("Users")]
        public class User 
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement]
            public int Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>Gets or sets the age.</summary>
            /// <value>The age.</value>
            public int Age { get; set; }
        }

        /// <summary>The database.</summary>
        private IDbConnection db;

        /// <summary>Sets the up.</summary>
        [SetUp]
        public void SetUp()
        {
            db = Config.OpenDbConnection();
            db.DropAndCreateTable<User>();
        }

        /// <summary>Tear down.</summary>
        [TearDown]
        public void TearDown()
        {
        }

        /// <summary>Builder select clause.</summary>
        [Test]
        public void BuilderSelectClause()
        {
            var rand = new Random(8675309);
            var data = new List<User>();
            for (var i = 0; i < 100; i++)
            {
                var nU = new User { Age = rand.Next(70), Id = i, Name = Guid.NewGuid().ToString() };
                data.Add(nU);
                db.Insert(nU);
                nU.Id = (int)db.GetLastInsertId();
            }

            var builder = new SqlBuilder();
            var justId = builder.AddTemplate("SELECT /**select**/ FROM Users");
            var all = builder.AddTemplate("SELECT /**select**/, Name, Age FROM Users");

            builder.Select("Id");

            var ids = db.Query<int>(justId.RawSql, justId.Parameters);
            var users = db.Query<User>(all.RawSql, all.Parameters);

            foreach (var u in data)
            {
                Assert.That(ids.Any(i => u.Id == i), "Missing ids in select");
                Assert.That(users.Any(a => a.Id == u.Id && a.Name == u.Name && a.Age == u.Age), "Missing users in select");
            }
        }

        /// <summary>Builder template wo composition.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        [Test]
        public void BuilderTemplateWOComposition()
        {
            var builder = new SqlBuilder();
            var template = builder.AddTemplate("SELECT COUNT(*) FROM Users WHERE Age = @age", new { age = 5 });

            if (template.RawSql == null) throw new Exception("RawSql null");
            if (template.Parameters == null) throw new Exception("Parameters null");

            db.Insert(new User { Age = 5, Name = "Testy McTestington" });

            Assert.That(db.QueryScalar<int>(template.RawSql, template.Parameters), Is.EqualTo(1));
        }         
    }
}