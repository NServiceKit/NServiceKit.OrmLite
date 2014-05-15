using System.Linq;
using NUnit.Framework;
using NServiceKit.OrmLite.Sqlite;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>An expression tests.</summary>
    [TestFixture]
    public class ExpressionTests
    {
        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            OrmLiteConfig.DialectProvider = new SqliteOrmLiteDialectProvider();
        }

        /// <summary>Gets the expression.</summary>
        /// <returns>A SqliteExpressionVisitor&lt;Person&gt;</returns>
        public static SqliteExpressionVisitor<Person> expr()
        {
            return new SqliteExpressionVisitor<Person>();
        }

        /// <summary>Does support SQL in on int collections.</summary>
        [Test]
        public void Does_support_Sql_In_on_int_collections()
        {
            var ids = new[] { 1, 2, 3 };

            Assert.That(expr().Where(q => Sql.In(q.Id, 1, 2, 3)).WhereExpression,
                Is.EqualTo("WHERE \"Id\" In (1,2,3)"));

            Assert.That(expr().Where(q => Sql.In(q.Id, ids)).WhereExpression,
                Is.EqualTo("WHERE \"Id\" In (1,2,3)"));

            Assert.That(expr().Where(q => Sql.In(q.Id, ids.ToList())).WhereExpression,
                Is.EqualTo("WHERE \"Id\" In (1,2,3)"));

            Assert.That(expr().Where(q => Sql.In(q.Id, ids.ToList().Cast<object>())).WhereExpression,
                Is.EqualTo("WHERE \"Id\" In (1,2,3)"));
        }

        /// <summary>Does support SQL in on string collections.</summary>
        [Test]
        public void Does_support_Sql_In_on_string_collections()
        {
            var ids = new[] { "A", "B", "C" };

            Assert.That(expr().Where(q => Sql.In(q.FirstName, "A", "B", "C")).WhereExpression,
                Is.EqualTo("WHERE \"FirstName\" In ('A','B','C')"));

            Assert.That(expr().Where(q => Sql.In(q.FirstName, ids)).WhereExpression,
                Is.EqualTo("WHERE \"FirstName\" In ('A','B','C')"));

            Assert.That(expr().Where(q => Sql.In(q.FirstName, ids.ToList())).WhereExpression,
                Is.EqualTo("WHERE \"FirstName\" In ('A','B','C')"));

            Assert.That(expr().Where(q => Sql.In(q.FirstName, ids.ToList().Cast<object>())).WhereExpression,
                Is.EqualTo("WHERE \"FirstName\" In ('A','B','C')"));
        }
    }
}