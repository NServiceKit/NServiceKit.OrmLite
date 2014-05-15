using NUnit.Framework;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>An ORM lite drop table tests.</summary>
    [TestFixture]
    public class OrmLiteDropTableTests
        : OrmLiteTestBase
    {
        /// <summary>Can drop existing table.</summary>
        [Test]
        public void Can_drop_existing_table()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable(typeof(ModelWithIdOnly));
                db.DropAndCreateTable<ModelWithIdAndName>();

                Assert.That(
                    db.TableExists(typeof(ModelWithIdOnly).Name),
                    Is.True);
                Assert.That(
                    db.TableExists(typeof(ModelWithIdAndName).Name),
                    Is.True);

                db.DropTable<ModelWithIdOnly>();
                db.DropTable(typeof(ModelWithIdAndName));

                Assert.That(
                    db.TableExists(typeof(ModelWithIdOnly).Name),
                    Is.False);
                Assert.That(
                    db.TableExists(typeof(ModelWithIdAndName).Name),
                    Is.False);
            }
        }

        /// <summary>Can drop multiple tables.</summary>
        [Test]
        public void Can_drop_multiple_tables()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTables(typeof(ModelWithIdOnly), typeof(ModelWithIdAndName));

                Assert.That(
                    db.TableExists(typeof(ModelWithIdOnly).Name),
                    Is.True);
                Assert.That(
                    db.TableExists(typeof(ModelWithIdAndName).Name),
                    Is.True);

                db.DropTables(typeof(ModelWithIdOnly), typeof(ModelWithIdAndName));

                Assert.That(
                    db.TableExists(typeof(ModelWithIdOnly).Name),
                    Is.False);
                Assert.That(
                    db.TableExists(typeof(ModelWithIdAndName).Name),
                    Is.False);
            }
        }
    }
}
