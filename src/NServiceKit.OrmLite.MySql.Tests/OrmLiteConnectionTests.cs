using NUnit.Framework;

namespace NServiceKit.OrmLite.MySql.Tests
{
    /// <summary>An ORM lite connection tests.</summary>
	[TestFixture]
	public class OrmLiteConnectionTests 
		: OrmLiteTestBase
	{
        /// <summary>Can create connection to blank database.</summary>
		[Test][Ignore]
		public void Can_create_connection_to_blank_database()
		{
			var connString = @"C:\Projects\PoToPe\trunk\website\src\Mflow.Intranet\Mflow.Intranet\App_Data\Exports\2009-10\MonthlySnapshot.mdf";
			using (var db = connString.OpenDbConnection())
			using (var dbCmd = db.CreateCommand())
			{
			}
		}

        /// <summary>Can create connection.</summary>
		[Test]
		public void Can_create_connection()
		{
			using (var db = OpenDbConnection())
			using (var dbCmd = db.CreateCommand())
			{
			}
		}

	}
}