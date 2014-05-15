using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;

namespace NServiceKit.OrmLite.MySql.Tests
{
    /// <summary>A date time column test.</summary>
    [TestFixture]
    public class DateTimeColumnTest
        : OrmLiteTestBase
    {
        /// <summary>Can create table containing date time column.</summary>
        [Test]
        public void Can_create_table_containing_DateTime_column()
        {
            using (var db = OpenDbConnection())
            {
                db.CreateTable<Analyze>(true);
            }
        }

        /// <summary>Can store date time value.</summary>
        [Test]
        public void Can_store_DateTime_Value()
        {
            using (var db = OpenDbConnection())
            {
                db.CreateTable<Analyze>(true);

                var obj = new Analyze {
                    Id = 1,
                    Date = DateTime.Now,
                    Url = "http://www.google.com"
                };

                db.Save(obj);
            }
        }

        /// <summary>Can store and retrieve date time value.</summary>
        [Test]
        public void Can_store_and_retrieve_DateTime_Value()
        {
            using (var db = OpenDbConnection())
            {
                db.CreateTable<Analyze>(true);

                var obj = new Analyze {
                    Id = 1,
                    Date = DateTime.Now,
                    Url = "http://www.google.com"
                };

                db.Save(obj);

                var id = (int)db.GetLastInsertId();
                var target = db.QueryById<Analyze>(id);

                Assert.IsNotNull(target);
                Assert.AreEqual(id, target.Id);
                Assert.AreEqual(obj.Date.ToString("yyyy-MM-dd HH:mm:ss"), target.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                Assert.AreEqual(obj.Url, target.Url);
            }
        }

        /// <summary>
        /// Provided by RyogoNA in issue #38
        /// https://github.com/ServiceStack/ServiceStack.OrmLite/issues/38#issuecomment-4625178.
        /// </summary>
        [Alias("Analyzes")]
        public class Analyze : IHasId<int>
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement]
            [PrimaryKey]
            public int Id
            {
                get;
                set;
            }

            /// <summary>Gets or sets the Date/Time of the date.</summary>
            /// <value>The date.</value>
            [Alias("AnalyzeDate")]
            public DateTime Date
            {
                get;
                set;
            }

            /// <summary>Gets or sets URL of the document.</summary>
            /// <value>The URL.</value>
            public string Url
            {
                get;
                set;
            }
        }
    }
}
