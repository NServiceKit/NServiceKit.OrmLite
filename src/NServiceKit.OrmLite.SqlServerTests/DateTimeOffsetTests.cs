#region Using directives

using System;
using System.Data;
using System.Linq;
using NUnit.Framework;
using NServiceKit.OrmLite.SqlServer;

#endregion


namespace NServiceKit.OrmLite.SqlServerTests
{
    /// <summary>A date time offset tests.</summary>
    internal class DateTimeOffsetTests : OrmLiteTestBase
    {
        /// <summary>Generic way to create our test tables.</summary>
        /// <typeparam name="TTable">.</typeparam>
        /// <typeparam name="TProp"> .</typeparam>
        /// <param name="db">   .</param>
        /// <param name="value">.</param>
        /// <returns>A TTable.</returns>
        private static TTable InsertAndSelectDateTimeOffset<TTable, TProp>(IDbConnection db, TProp value) where TTable : IDateTimeOffsetObject<TProp>, new()
        {
            db.DropAndCreateTable<TTable>();
            db.Insert(new TTable
            {
                Test = value
            });
            var result = db.Select<TTable>().First();
            return result;
        }

        /// <summary>Ensures that date time offset saves.</summary>
        [Test]
        public void EnsureDateTimeOffsetSaves()
        {
            var dbFactory = new OrmLiteConnectionFactory(base.ConnectionString, SqlServerOrmLiteDialectProvider.Instance);

            using (var db = dbFactory.OpenDbConnection())
            {
                var dateTime = new DateTimeOffset(2012, 1, 30, 1, 1, 1, new TimeSpan(5, 0, 0));
                var x = InsertAndSelectDateTimeOffset<DateTimeOffsetObject, DateTimeOffset>(db, dateTime);
                Assert.AreEqual(x.Test, dateTime);
            }
        }

        /// <summary>Ensures that nullable date time offset saves.</summary>
        [Test]
        public void EnsureNullableDateTimeOffsetSaves()
        {
            var dbFactory = new OrmLiteConnectionFactory(base.ConnectionString, SqlServerOrmLiteDialectProvider.Instance);

            using (var db = dbFactory.OpenDbConnection())
            {
                DateTimeOffset? dateTime = new DateTimeOffset(2012, 1, 30, 1, 1, 1, new TimeSpan(5, 0, 0));
                var x = InsertAndSelectDateTimeOffset<NullableDateTimeOffsetObject, DateTimeOffset?>(db, dateTime);
                Assert.AreEqual(x.Test, dateTime);
            }
        }

        /// <summary>A date time offset object.</summary>
        private class DateTimeOffsetObject : IDateTimeOffsetObject<DateTimeOffset>
        {
            /// <summary>Gets or sets the test.</summary>
            /// <value>The test.</value>
            public DateTimeOffset Test { get; set; }
        }

        /// <summary>A nullable date time offset object.</summary>
        private class NullableDateTimeOffsetObject : IDateTimeOffsetObject<DateTimeOffset?>
        {
            /// <summary>Gets or sets the test.</summary>
            /// <value>The test.</value>
            public DateTimeOffset? Test { get; set; }
        }

        /// <summary>Interface for date time offset object.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        private interface IDateTimeOffsetObject<T>
        {
             /// <summary>Gets or sets the test.</summary>
             /// <value>The test.</value>
             T Test { get; set; }
        }
    }
}