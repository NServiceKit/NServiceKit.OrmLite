using NUnit.Framework;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.OrmLite.MySql.Tests
{
    /// <summary>An ORM lite update tests.</summary>
	[TestFixture]
	public class OrmLiteUpdateTests
		: OrmLiteTestBase
	{
        /// <summary>Can update model with fields of different types table.</summary>
		[Test]
		public void Can_update_ModelWithFieldsOfDifferentTypes_table()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithFieldsOfDifferentTypes>(true);

				var row = ModelWithFieldsOfDifferentTypes.Create(1);

				db.Insert(row);

				row.Name = "UpdatedName";

				db.Update(row);

				var dbRow = db.GetById<ModelWithFieldsOfDifferentTypes>(1);

				ModelWithFieldsOfDifferentTypes.AssertIsEqual(dbRow, row);
			}
		}

	}
}