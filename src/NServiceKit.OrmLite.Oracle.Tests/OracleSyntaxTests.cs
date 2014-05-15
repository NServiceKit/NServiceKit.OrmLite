using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.Oracle.Tests
{
    /// <summary>An oracle syntax tests.</summary>
    [TestFixture]
    public class OracleSyntaxTests : OracleTestBase
    {
        /// <summary>Can generate correct paging if first column must be quoted.</summary>
        [Test]
        public void can_generate_correct_paging_if_first_column_must_be_quoted()
        {
            using(var db = OpenDbConnection()) {
                db.CreateTable<FirstColMustBeQuoted>(true);

                var noRows = db.Select<FirstColMustBeQuoted>(ev => ev.Limit(100));

                Assert.AreEqual(0, noRows.Count());

                for(int i = 0; i < 150; i++) {
                    db.Insert<FirstColMustBeQuoted>(new FirstColMustBeQuoted { COMMENT = "row #" + i });
                }

                var hundredRows = db.Select<FirstColMustBeQuoted>(ev => ev.Limit(100));
                Assert.AreEqual(100, hundredRows.Count());
            }
        }

        /// <summary>Can generate correct paging if first column dont have to be quoted.</summary>
        [Test]
        public void can_generate_correct_paging_if_first_column_dont_have_to_be_quoted()
        {
            using(var db = OpenDbConnection()) {
                db.CreateTable<FirstColNoQuotes>(true);

                var noRows = db.Select<FirstColNoQuotes>(ev => ev.Limit(100));

                Assert.AreEqual(0, noRows.Count());

                for(int i = 0; i < 150; i++) {
                    db.Insert<FirstColNoQuotes>(new FirstColNoQuotes { COMMENT = "row #" + i });
                }

                var hundredRows = db.Select<FirstColNoQuotes>(ev => ev.Limit(100));
                Assert.AreEqual(100, hundredRows.Count());
            }
        }

        /// <summary>
        /// Can generate correct paging if table name must be quoted and first column have to be
        /// quoted.
        /// </summary>
        [Test]
        public void can_generate_correct_paging_if_TABLE_NAME_must_be_quoted_and_first_column_have_to_be_quoted()
        {
            using(var db = OpenDbConnection()) {
                db.CreateTable<COMMENT_first>(true);

                var noRows = db.Select<COMMENT_first>(ev => ev.Limit(100));

                Assert.AreEqual(0, noRows.Count());

                for(int i = 0; i < 150; i++) {
                    db.Insert(new COMMENT_first { COMMENT = "COMMENT row #" + i });
                }

                var hundredRows = db.Select<COMMENT_first>(ev => ev.Limit(100));
                Assert.AreEqual(100, hundredRows.Count());
            }
        }

        /// <summary>
        /// Can generate correct paging if table name must be quoted and first column dont have to be
        /// quoted.
        /// </summary>
        [Test]
        public void can_generate_correct_paging_if_TABLE_NAME_must_be_quoted_and_first_column_dont_have_to_be_quoted()
        {
            using(var db = OpenDbConnection()) {
                db.CreateTable<COMMENT_other>(true);

                var noRows = db.Select<COMMENT_other>(ev => ev.Limit(100));

                Assert.AreEqual(0, noRows.Count());

                for(int i = 0; i < 150; i++) {
                    db.Insert(new COMMENT_other { COMMENT = "COMMENT row #" + i });
                }

                var hundredRows = db.Select<COMMENT_other>(ev => ev.Limit(100));
                Assert.AreEqual(100, hundredRows.Count());
            }
        }

        /// <summary>A comment first.</summary>
        [Alias("COMMENT")]
        private class COMMENT_first
        {
            /// <summary>Gets or sets the comment.</summary>
            /// <value>The comment.</value>
            public string COMMENT { get; set; }

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement, PrimaryKey]
            public int Id { get; set; }
        }

        /// <summary>A comment other.</summary>
        [Alias("COMMENT")]
        private class COMMENT_other
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement, PrimaryKey]
            public int Id { get; set; }

            /// <summary>Gets or sets the comment.</summary>
            /// <value>The comment.</value>
            public string COMMENT { get; set; }
        }

        /// <summary>A first col must be quoted.</summary>
        private class FirstColMustBeQuoted
        {
            /// <summary>Gets or sets the comment.</summary>
            /// <value>The comment.</value>
            public string COMMENT { get; set; }

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement, PrimaryKey]
            public int Id { get; set; }
        }

        /// <summary>A first col no quotes.</summary>
        private class FirstColNoQuotes//Oracle: max 30 characters...
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement, PrimaryKey]
            public int Id { get; set; }

            /// <summary>Gets or sets the comment.</summary>
            /// <value>The comment.</value>
            public string COMMENT { get; set; }
        }
    }
}
