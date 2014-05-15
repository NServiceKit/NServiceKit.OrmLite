using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.OrmLite.FirebirdTests
{
    /// <summary>An ORM lite delete tests.</summary>
	[TestFixture]
	public class OrmLiteDeleteTests
		: OrmLiteTestBase
	{
        /// <summary>Sets the up.</summary>
		[SetUp]
		public void SetUp()
		{
			CreateNewDatabase();
		}

        /// <summary>Can delete from model with fields of different types table.</summary>
		[Test]
		public void Can_Delete_from_ModelWithFieldsOfDifferentTypes_table()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<ModelWithFieldsOfDifferentTypes>(true);

				var rowIds = new List<int>(new[] { 1, 2, 3 });
				rowIds.ForEach(x => db.Insert(ModelWithFieldsOfDifferentTypes.Create(x)));

				var rows = db.Select<ModelWithFieldsOfDifferentTypes>();
				var row2 = rows.First(x => x.Id == 2);

				db.Delete(row2);

				rows = db.GetByIds<ModelWithFieldsOfDifferentTypes>(rowIds);
				var dbRowIds = rows.ConvertAll(x => x.Id);

				Assert.That(dbRowIds, Is.EquivalentTo(new[] { 1, 3 }));
			}
		}

        /// <summary>
        /// Can delete by identifier from model with fields of different types table.
        /// </summary>
		[Test]
		public void Can_DeleteById_from_ModelWithFieldsOfDifferentTypes_table()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<ModelWithFieldsOfDifferentTypes>(true);

				var rowIds = new List<int>(new[] { 1, 2, 3 });
				rowIds.ForEach(x => db.Insert(ModelWithFieldsOfDifferentTypes.Create(x)));

				db.DeleteById<ModelWithFieldsOfDifferentTypes>(2);

				var rows = db.GetByIds<ModelWithFieldsOfDifferentTypes>(rowIds);
				var dbRowIds = rows.ConvertAll(x => x.Id);

				Assert.That(dbRowIds, Is.EquivalentTo(new[] { 1, 3 }));
			}
		}

        /// <summary>
        /// Can delete by identifiers from model with fields of different types table.
        /// </summary>
		[Test]
		public void Can_DeleteByIds_from_ModelWithFieldsOfDifferentTypes_table()
		{
            using (var db = new OrmLiteConnectionFactory(ConnectionString, FirebirdDialect.Provider).Open())
			{
				db.CreateTable<ModelWithFieldsOfDifferentTypes>(true);

				var rowIds = new List<int>(new[] { 1, 2, 3 });
				rowIds.ForEach(x => db.Insert(ModelWithFieldsOfDifferentTypes.Create(x)));

				db.DeleteByIds<ModelWithFieldsOfDifferentTypes>(new[] { 1, 3 });

				var rows = db.GetByIds<ModelWithFieldsOfDifferentTypes>(rowIds);
				var dbRowIds = rows.ConvertAll(x => x.Id);

				Assert.That(dbRowIds, Is.EquivalentTo(new[] { 2 }));
			}
		}

	}
}