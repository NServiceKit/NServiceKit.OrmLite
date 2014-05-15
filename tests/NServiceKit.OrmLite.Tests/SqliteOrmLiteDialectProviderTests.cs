using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>A sqlite ORM lite dialect provider tests.</summary>
    [TestFixture]
    public class SqliteOrmLiteDialectProviderTests
    {
        /// <summary>Tests fixture setup.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            OrmLiteConfig.DialectProvider = SqliteDialect.Provider;
        }

        /// <summary>The has date time offset memeber.</summary>
        public class HasDateTimeOffsetMemeber
        {
            /// <summary>Gets or sets the moment in time.</summary>
            /// <value>The moment in time.</value>
            public DateTimeOffset MomentInTime { get; set; }
        }

        /// <summary>The has nullable date time offset memeber.</summary>
        public class HasNullableDateTimeOffsetMemeber
        {
            /// <summary>Gets or sets the moment in time.</summary>
            /// <value>The moment in time.</value>
            public DateTimeOffset? MomentInTime { get; set; }
        }

        /// <summary>Can persist and retrieve date time offset.</summary>
        [Test]
        public void CanPersistAndRetrieveDateTimeOffset()
        {
            using (IDbConnection db = ":memory:".OpenDbConnection())
            {
                var dto = DateTimeOffset.Now;

                db.CreateTable<HasDateTimeOffsetMemeber>(false);
                db.Insert(new HasDateTimeOffsetMemeber() {MomentInTime = dto});

                List<HasDateTimeOffsetMemeber> list = db.Select<HasDateTimeOffsetMemeber>();

                Assert.That(list.Count == 1);
                Assert.That(list.First().MomentInTime.CompareTo(dto) == 0);
            }
        }

        /// <summary>Can persist and retrieve nullable date time offset.</summary>
        [Test]
        public void CanPersistAndRetrieveNullableDateTimeOffset()
        {
            using (IDbConnection db = ":memory:".OpenDbConnection())
            {
                var dto = DateTimeOffset.Now;

                db.CreateTable<HasNullableDateTimeOffsetMemeber>(false);
                db.Insert(new HasNullableDateTimeOffsetMemeber() { MomentInTime = dto });

                List<HasNullableDateTimeOffsetMemeber> list = db.Select<HasNullableDateTimeOffsetMemeber>();

                Assert.That(list.Count == 1);
                Assert.That(list.First().MomentInTime.HasValue);
                Assert.That(list.First().MomentInTime.Value.CompareTo(dto) == 0);
            }
        }
    }
}
