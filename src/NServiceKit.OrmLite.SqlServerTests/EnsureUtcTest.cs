using System;
using System.Data;
using System.Linq;
using NUnit.Framework;
using NServiceKit.OrmLite.SqlServer;

namespace NServiceKit.OrmLite.SqlServerTests
{
    /// <summary>An ensure UTC tests.</summary>
    internal class EnsureUtcTests : OrmLiteTestBase
    {
        /// <summary>Saves the date time to database.</summary>
        [Test]
        public void SaveDateTimeToDatabase()
        {
            var dbFactory = new OrmLiteConnectionFactory(base.ConnectionString, SqlServerOrmLiteDialectProvider.Instance);
            SqlServerOrmLiteDialectProvider.Instance.EnsureUtc(true);

            using (var db = dbFactory.OpenDbConnection())
            {
                var dateTime = new DateTime(2012, 1, 1, 1, 1, 1, DateTimeKind.Local);
                var x = InsertAndSelectDateTime(db, dateTime);
                Assert.AreEqual(DateTimeKind.Utc, x.Test.Kind);
                Assert.AreEqual(x.Test.ToUniversalTime(), dateTime.ToUniversalTime());
                Assert.AreEqual(x.Test.ToLocalTime(), dateTime.ToLocalTime());

                dateTime = new DateTime(2012, 1, 1, 1, 1, 1, DateTimeKind.Utc);
                x = InsertAndSelectDateTime(db, dateTime);
                Assert.AreEqual(DateTimeKind.Utc, x.Test.Kind);
                Assert.AreEqual(x.Test.ToUniversalTime(), dateTime.ToUniversalTime());
                Assert.AreEqual(x.Test.ToLocalTime(), dateTime.ToLocalTime());

                dateTime = new DateTime(2012, 1, 1, 1, 1, 1, DateTimeKind.Unspecified);
                x = InsertAndSelectDateTime(db, dateTime);
                Assert.AreEqual(DateTimeKind.Utc, x.Test.Kind);
                Assert.AreEqual(x.Test.ToUniversalTime(), dateTime);
                Assert.AreEqual(x.Test.ToLocalTime(), dateTime.ToLocalTime());
            }
        }

        /// <summary>Inserts an and select date time.</summary>
        /// <param name="db">      The database.</param>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A DateTimeObject.</returns>
        private static DateTimeObject InsertAndSelectDateTime(IDbConnection db, DateTime dateTime)
        {
            db.DropAndCreateTable<DateTimeObject>();
            db.Insert(new DateTimeObject {Test = dateTime});
            var x = db.Select<DateTimeObject>().First();
            return x;
        }

        /// <summary>A date time object.</summary>
        private class DateTimeObject
        {
            /// <summary>Gets or sets the Date/Time of the test.</summary>
            /// <value>The test.</value>
            public DateTime Test { get; set; }
        }
    }
}
