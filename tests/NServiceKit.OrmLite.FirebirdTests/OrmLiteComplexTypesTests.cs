using System;
using System.Linq;
using NUnit.Framework;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.OrmLite.FirebirdTests
{
    /// <summary>An ORM lite complex types tests.</summary>
	[TestFixture]
	public class OrmLiteComplexTypesTests
		: OrmLiteTestBase
	{
        /// <summary>Can insert into model with complex types table.</summary>
		[Ignore("Endless recursion, need to fix")]
		[Test]
		public void Can_insert_into_ModelWithComplexTypes_table()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<ModelWithComplexTypes>(true);

				var row = ModelWithComplexTypes.Create(1);

				db.Insert(row);
			}
		}

        /// <summary>Can insert and select from model with complex types table.</summary>
		[Ignore("Endless recursion, need to fix")]
		[Test]
		public void Can_insert_and_select_from_ModelWithComplexTypes_table()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<ModelWithComplexTypes>(true);

				var row = ModelWithComplexTypes.Create(1);

				db.Insert(row);

				var rows = db.Select<ModelWithComplexTypes>();

				Assert.That(rows, Has.Count.EqualTo(1));

				ModelWithComplexTypes.AssertIsEqual(rows[0], row);
			}
		}

        /// <summary>Can insert and select from order line data.</summary>
		[Test]
		public void Can_insert_and_select_from_OrderLineData()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<SampleOrderLine>(true);

				var orderIds = new[] { 1, 2, 3, 4, 5 }.ToList();

				orderIds.ForEach(x => db.Insert(
					SampleOrderLine.Create(Guid.NewGuid(), x, 1)));

				var rows = db.Select<SampleOrderLine>();
				Assert.That(rows, Has.Count.EqualTo(orderIds.Count));
			}
		}

	}
}
