using System;
using System.Data;
using NUnit.Framework;
using NServiceKit.Common.Tests.Models;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>An ORM lite update tests.</summary>
    [TestFixture]
    public class OrmLiteUpdateTests
        : OrmLiteTestBase
    {
        /// <summary>The database.</summary>
        private IDbConnection db;

        /// <summary>Sets the up.</summary>
        [SetUp]
        public void SetUp()
        {
            db = OpenDbConnection();
        }

        /// <summary>Tear down.</summary>
        [TearDown]
        public void TearDown()
        {
            db.Dispose();
        }

        /// <summary>Creates model with fields of different types.</summary>
        /// <returns>The new model with fields of different types.</returns>
        private ModelWithFieldsOfDifferentTypes CreateModelWithFieldsOfDifferentTypes()
        {
            db.DropAndCreateTable<ModelWithFieldsOfDifferentTypes>();

            var row = ModelWithFieldsOfDifferentTypes.Create(1);
            return row;
        }

        /// <summary>Can update model with fields of different types table.</summary>
        [Test]
        public void Can_update_ModelWithFieldsOfDifferentTypes_table()
        {
            var row = CreateModelWithFieldsOfDifferentTypes();

            db.Insert(row);

            row.Name = "UpdatedName";

            db.Update(row);

            var dbRow = db.GetById<ModelWithFieldsOfDifferentTypes>(1);

            ModelWithFieldsOfDifferentTypes.AssertIsEqual(dbRow, row);
        }

        /// <summary>Can update model with fields of different types table with filter.</summary>
        [Test]
        public void Can_update_ModelWithFieldsOfDifferentTypes_table_with_filter()
        {
            var row = CreateModelWithFieldsOfDifferentTypes();

            db.Insert(row);

            row.Name = "UpdatedName";

            db.Update(row, x => x.LongId <= row.LongId);

            var dbRow = db.GetById<ModelWithFieldsOfDifferentTypes>(1);

            ModelWithFieldsOfDifferentTypes.AssertIsEqual(dbRow, row);
        }

        /// <summary>Can update with anonymous type and expression filter.</summary>
        [Test]
        public void Can_update_with_anonymousType_and_expr_filter()
        {
            var row = CreateModelWithFieldsOfDifferentTypes();

            db.Insert(row);
            row.DateTime = DateTime.Now;
            row.Name = "UpdatedName";

            db.Update<ModelWithFieldsOfDifferentTypes>(new { row.Name, row.DateTime },
                x => x.LongId >= row.LongId && x.LongId <= row.LongId);

            var dbRow = db.GetById<ModelWithFieldsOfDifferentTypes>(row.Id);
            Console.WriteLine(dbRow.Dump());
            ModelWithFieldsOfDifferentTypes.AssertIsEqual(dbRow, row);
        }

        /// <summary>Can update with optional string parameters.</summary>
        [Test]
        public void Can_update_with_optional_string_params()
        {
            var row = CreateModelWithFieldsOfDifferentTypes();

            db.Insert(row);
            row.Name = "UpdatedName";

            db.Update<ModelWithFieldsOfDifferentTypes>(set: "NAME = {0}".SqlFormat(row.Name), where: "LongId <= {0}".SqlFormat(row.LongId));

            var dbRow = db.GetById<ModelWithFieldsOfDifferentTypes>(row.Id);
            Console.WriteLine(dbRow.Dump());
            ModelWithFieldsOfDifferentTypes.AssertIsEqual(dbRow, row);
        }

        /// <summary>Can update with table name and optional string parameters.</summary>
        [Test]
        public void Can_update_with_tableName_and_optional_string_params()
        {
            var row = CreateModelWithFieldsOfDifferentTypes();

            db.Insert(row);
            row.Name = "UpdatedName";

            db.Update(table: "ModelWithFieldsOfDifferentTypes",
                set: "NAME = {0}".SqlFormat(row.Name), where: "LongId <= {0}".SqlFormat(row.LongId));

            var dbRow = db.GetById<ModelWithFieldsOfDifferentTypes>(row.Id);
            Console.WriteLine(dbRow.Dump());
            ModelWithFieldsOfDifferentTypes.AssertIsEqual(dbRow, row);
        }

    }
}