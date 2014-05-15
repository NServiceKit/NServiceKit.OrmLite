using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite.SqlServer;

namespace NServiceKit.OrmLite.SqlServerTests
{
    /// <summary>A datetime 2 tests.</summary>
	public class Datetime2Tests : SqlServerTests.OrmLiteTestBase
	{
        /// <summary>Datetime tests can use datetime 2.</summary>
		[Test]
		public void datetime_tests__can_use_datetime2()
		{
			var dbFactory = new OrmLiteConnectionFactory(base.ConnectionString, SqlServerOrmLiteDialectProvider.Instance);

			//change to datetime2 - check for higher range and precision
			(SqlServerOrmLiteDialectProvider.Instance as SqlServerOrmLiteDialectProvider).UseDatetime2(true);

			using(var conn = dbFactory.OpenDbConnection()) {
				var test_object_ValidForDatetime2 = Table_for_datetime2_tests.get_test_object_ValidForDatetime2();
				var test_object_ValidForNormalDatetime = Table_for_datetime2_tests.get_test_object_ValidForNormalDatetime();

				conn.CreateTable<Table_for_datetime2_tests>(true);

				//normal insert
				conn.Insert(test_object_ValidForDatetime2);
				var insertedId = (int)conn.GetLastInsertId();

				//read back, and verify precision
				var fromDb = conn.GetById<Table_for_datetime2_tests>(insertedId);
				Assert.AreEqual(test_object_ValidForDatetime2.ToVerifyPrecision, fromDb.ToVerifyPrecision);

				//update
				fromDb.ToVerifyPrecision = test_object_ValidForDatetime2.ToVerifyPrecision.Value.AddYears(1);
				conn.UpdateParam(fromDb);
				var fromDb2 = conn.GetById<Table_for_datetime2_tests>(insertedId);
				Assert.AreEqual(test_object_ValidForDatetime2.ToVerifyPrecision.Value.AddYears(1), fromDb2.ToVerifyPrecision);


				//check InsertParam
				conn.InsertParam(test_object_ValidForDatetime2);
			}
		}

        /// <summary>Datetime tests check default behaviour.</summary>
		[Test]
		public void datetime_tests__check_default_behaviour()
		{
			var dbFactory = new OrmLiteConnectionFactory(base.ConnectionString, SqlServerOrmLiteDialectProvider.Instance);

			//default behaviour: normal datetime can't hold DateTime values of year 1.
			(SqlServerOrmLiteDialectProvider.Instance as SqlServerOrmLiteDialectProvider).UseDatetime2(false);

			using(var conn = dbFactory.OpenDbConnection()) {
				var test_object_ValidForDatetime2 = Table_for_datetime2_tests.get_test_object_ValidForDatetime2();
				var test_object_ValidForNormalDatetime = Table_for_datetime2_tests.get_test_object_ValidForNormalDatetime();

				conn.CreateTable<Table_for_datetime2_tests>(true);

				//normal insert
				conn.Insert(test_object_ValidForNormalDatetime);
				var insertedId = conn.GetLastInsertId();

				//insert works, but can't regular datetime's precision is not great enough.
				var fromDb = conn.GetById<Table_for_datetime2_tests>(insertedId);
				Assert.AreNotEqual(test_object_ValidForNormalDatetime.ToVerifyPrecision, fromDb.ToVerifyPrecision);

				var thrown = Assert.Throws<SqlException>(() => {
					conn.Insert(test_object_ValidForDatetime2);
				});
				Assert.That(thrown.Message.Contains("The conversion of a varchar data type to a datetime data type resulted in an out-of-range value."));

				
				//check InsertParam
				conn.InsertParam(test_object_ValidForNormalDatetime);
				//InsertParam fails differently:
				var insertParamException = Assert.Throws<System.Data.SqlTypes.SqlTypeException>(() => {
					conn.InsertParam(test_object_ValidForDatetime2);
				});
				Assert.That(insertParamException.Message.Contains("SqlDateTime overflow"));
			}
		}

        /// <summary>A table for datetime 2 tests.</summary>
		private class Table_for_datetime2_tests
		{
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
			[AutoIncrement]
			public int Id { get; set; }

            /// <summary>Gets or sets some date time.</summary>
            /// <value>some date time.</value>
			public DateTime SomeDateTime { get; set; }

            /// <summary>Gets or sets the Date/Time of to verify precision.</summary>
            /// <value>to verify precision.</value>
			public DateTime? ToVerifyPrecision { get; set; }

            /// <summary>
            /// Gets or sets the Date/Time of the nullable date time leave iterator null.
            /// </summary>
            /// <value>The nullable date time leave iterator null.</value>
			public DateTime? NullableDateTimeLeaveItNull { get; set; }

            /// <summary>
            /// to check datetime(2)'s precision. A regular 'datetime' is not precise enough.
            /// </summary>
			public static readonly DateTime regular_datetime_field_cant_hold_this_exact_moment = new DateTime(2013, 3, 17, 21, 29, 1, 678);

            /// <summary>Gets test object valid for datetime 2.</summary>
            /// <returns>The test object valid for datetime 2.</returns>
			public static Table_for_datetime2_tests get_test_object_ValidForDatetime2() { return new Table_for_datetime2_tests { SomeDateTime = new DateTime(1, 1, 1), ToVerifyPrecision = regular_datetime_field_cant_hold_this_exact_moment }; }

            /// <summary>Gets test object valid for normal datetime.</summary>
            /// <returns>The test object valid for normal datetime.</returns>
			public static Table_for_datetime2_tests get_test_object_ValidForNormalDatetime() { return new Table_for_datetime2_tests { SomeDateTime = new DateTime(2001, 1, 1), ToVerifyPrecision = regular_datetime_field_cant_hold_this_exact_moment }; }

		}
	}
}
