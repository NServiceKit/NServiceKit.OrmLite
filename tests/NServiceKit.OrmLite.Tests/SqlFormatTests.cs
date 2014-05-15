using System.Collections.Generic;
using NUnit.Framework;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>A SQL format tests.</summary>
	[TestFixture]
	public class SqlFormatTests
		: OrmLiteTestBase
	{
        /// <summary>SQL join joins int identifiers.</summary>
		[Test]
		public void SqlJoin_joins_int_ids()
		{
			var ids = new List<int> { 1, 2, 3 };
			Assert.That(ids.SqlJoin(), Is.EqualTo("1,2,3"));
		}

        /// <summary>SQL join joins string identifiers.</summary>
		[Test]
		public void SqlJoin_joins_string_ids()
		{
			var ids = new List<string> { "1", "2", "3" };
			Assert.That(ids.SqlJoin(), Is.EqualTo("'1','2','3'"));
		}

        /// <summary>SQL format can handle null arguments.</summary>
		[Test]
		public void SqlFormat_can_handle_null_args()
		{
			const string sql = "SELECT Id FROM FOO WHERE Bar = {0}";
			var sqlFormat = sql.SqlFormat(1, null);

			Assert.That(sqlFormat, Is.EqualTo("SELECT Id FROM FOO WHERE Bar = 1"));
		}

	}
}