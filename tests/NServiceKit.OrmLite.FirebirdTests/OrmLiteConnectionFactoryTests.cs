using System;
using Northwind.Common.DataModel;
using NUnit.Framework;
using NServiceKit.OrmLite.Firebird;

namespace NServiceKit.OrmLite.FirebirdTests
{
    /// <summary>An ORM lite connection factory tests.</summary>
    [TestFixture]
    [Ignore]
    public class OrmLiteConnectionFactoryTests
    {
        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            OrmLiteConfig.DialectProvider = FirebirdOrmLiteDialectProvider.Instance; //use Pooling=false ?
        }

        /// <summary>Automatic dispose connection factory disposes connection.</summary>
        [Test]
        public void AutoDispose_ConnectionFactory_disposes_connection()
        {
            var factory = new OrmLiteConnectionFactory("User=SYSDBA;Password=masterkey;Database=ormlite-tests.fdb;DataSource=localhost;Dialect=3;charset=ISO8859_1;", true);

            using (var db = factory.OpenDbConnection())
            {
                db.CreateTable<Shipper>(true);
                db.Insert(new Shipper { CompanyName = "I am shipper 1" });
            }

            using (var db = factory.OpenDbConnection())
            {
                db.CreateTable<Shipper>(false);
                Assert.That(db.Select<Shipper>(), Has.Count.EqualTo(1));

                db.DeleteAll<Shipper>();
                Assert.That(db.Select<Shipper>(), Has.Count.EqualTo(0));
            }
        }

        /// <summary>Non automatic dispose connection factory reuses connection.</summary>
        [Test]
        public void NonAutoDispose_ConnectionFactory_reuses_connection()
        {
            var factory = new OrmLiteConnectionFactory("User=SYSDBA;Password=masterkey;Database=ormlite-tests.fdb;DataSource=localhost;Dialect=3;charset=ISO8859_1;", false);

            using (var db = factory.OpenDbConnection())
            {
                db.CreateTable<Shipper>(false);
                db.Insert(new Shipper { CompanyName = "I am shipper 2" });
            }

            using (var db = factory.OpenDbConnection())
            {
                db.CreateTable<Shipper>(false);
                Assert.That(db.Select<Shipper>(), Has.Count.EqualTo(1));

                db.DeleteAll<Shipper>();
                Assert.That(db.Select<Shipper>(), Has.Count.EqualTo(0));
            }
        }

        /// <summary>Non automatic dispose connection factory delete and drop.</summary>
        [Test]
        public void NonAutoDispose_ConnectionFactory_delete_and_drop()
        {
            var factory = new OrmLiteConnectionFactory("User=SYSDBA;Password=masterkey;Database=ormlite-tests.fdb;DataSource=localhost;Dialect=3;charset=ISO8859_1;", false);

            using (var db = factory.OpenDbConnection())
            {
                db.DeleteAll<Shipper>();
                Assert.That(db.Select<Shipper>(), Has.Count.EqualTo(0));
            }

            using (var db = factory.OpenDbConnection())
            {
                db.DropTable<Shipper>();
                var schema = new Schema {
                    Connection = db,
                };

                Assert.That(schema.GetTable("Shippers".ToUpper()) == null);
            }
        }

    }
}
