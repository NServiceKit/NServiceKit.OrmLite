using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NServiceKit.OrmLite;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>An ORM lite utility extensions tests.</summary>
    public class OrmLiteUtilExtensionsTests : OrmLiteTestBase
    {
        /// <summary>Can create string in statement.</summary>
        [Test]
        public void CanCreateStringInStatement()
        {
            var list = new string[] { "A", "B", "C" };

            var sql = "IN ({0})".Params(list.SqlInValues());

            Assert.AreEqual("IN ('A','B','C')", sql);
        }

        /// <summary>Can create int in statement.</summary>
        [Test]
        public void CanCreateIntInStatement()
        {
            var list = new int[] { 1, 2, 3 };

            var sql = "IN ({0})".Params(list.SqlInValues());

            Assert.AreEqual("IN (1,2,3)", sql);
        }

        /// <summary>Can create null in statement from empty list.</summary>
        [Test]
        public void CanCreateNullInStatementFromEmptyList()
        {
            var list = new string[] {};

            var sql = "IN ({0})".Params(list.SqlInValues());

            Assert.AreEqual("IN (NULL)", sql);
        }
    }
}
