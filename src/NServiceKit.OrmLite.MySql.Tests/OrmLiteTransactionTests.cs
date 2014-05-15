using NUnit.Framework;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.OrmLite.MySql.Tests
{
    /// <summary>An ORM lite transaction tests.</summary>
	[TestFixture]
	public class OrmLiteTransactionTests 
		: OrmLiteTestBase
	{
        /// <summary>Transaction commit persists data to the database.</summary>
		[Test]
		public void Transaction_commit_persists_data_to_the_db()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithIdAndName>(true);
				db.Insert(new ModelWithIdAndName(1));

				using (var dbTrans = db.BeginTransaction())
				{
					db.Insert(new ModelWithIdAndName(2));
					db.Insert(new ModelWithIdAndName(3));

					var rowsInTrans = db.Select<ModelWithIdAndName>();
					Assert.That(rowsInTrans, Has.Count.EqualTo(3));

					dbTrans.Commit();
				}

				var rows = db.Select<ModelWithIdAndName>();
				Assert.That(rows, Has.Count.EqualTo(3));
			}
		}

        /// <summary>Transaction rollsback if not committed.</summary>
		[Test]
		public void Transaction_rollsback_if_not_committed()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithIdAndName>(true);
				db.Insert(new ModelWithIdAndName(1));

				using (var dbTrans = db.BeginTransaction())
				{
					db.Insert(new ModelWithIdAndName(2));
					db.Insert(new ModelWithIdAndName(3));

					var rowsInTrans = db.Select<ModelWithIdAndName>();
					Assert.That(rowsInTrans, Has.Count.EqualTo(3));
				}

				var rows = db.Select<ModelWithIdAndName>();
				Assert.That(rows, Has.Count.EqualTo(1));
			}
		}

        /// <summary>Transaction rollsback transactions to different tables.</summary>
		[Test]
		public void Transaction_rollsback_transactions_to_different_tables()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithIdAndName>(true);
				db.CreateTable<ModelWithFieldsOfDifferentTypes>(true);
				db.CreateTable<ModelWithOnlyStringFields>(true);

				db.Insert(new ModelWithIdAndName(1));

				using (var dbTrans = db.BeginTransaction())
				{
					db.Insert(new ModelWithIdAndName(2));
					db.Insert(ModelWithFieldsOfDifferentTypes.Create(3));
					db.Insert(ModelWithOnlyStringFields.Create("id3"));

					Assert.That(db.Select<ModelWithIdAndName>(), Has.Count.EqualTo(2));
					Assert.That(db.Select<ModelWithFieldsOfDifferentTypes>(), Has.Count.EqualTo(1));
					Assert.That(db.Select<ModelWithOnlyStringFields>(), Has.Count.EqualTo(1));
				}

				Assert.That(db.Select<ModelWithIdAndName>(), Has.Count.EqualTo(1));
				Assert.That(db.Select<ModelWithFieldsOfDifferentTypes>(), Has.Count.EqualTo(0));
				Assert.That(db.Select<ModelWithOnlyStringFields>(), Has.Count.EqualTo(0));
			}
		}

        /// <summary>Transaction commits inserts to different tables.</summary>
		[Test]
		public void Transaction_commits_inserts_to_different_tables()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTable<ModelWithIdAndName>(true);
				db.CreateTable<ModelWithFieldsOfDifferentTypes>(true);
				db.CreateTable<ModelWithOnlyStringFields>(true);

				db.Insert(new ModelWithIdAndName(1));

				using (var dbTrans = db.BeginTransaction())
				{
					db.Insert(new ModelWithIdAndName(2));
					db.Insert(ModelWithFieldsOfDifferentTypes.Create(3));
					db.Insert(ModelWithOnlyStringFields.Create("id3"));

					Assert.That(db.Select<ModelWithIdAndName>(), Has.Count.EqualTo(2));
					Assert.That(db.Select<ModelWithFieldsOfDifferentTypes>(), Has.Count.EqualTo(1));
					Assert.That(db.Select<ModelWithOnlyStringFields>(), Has.Count.EqualTo(1));

					dbTrans.Commit();
				}

				Assert.That(db.Select<ModelWithIdAndName>(), Has.Count.EqualTo(2));
				Assert.That(db.Select<ModelWithFieldsOfDifferentTypes>(), Has.Count.EqualTo(1));
				Assert.That(db.Select<ModelWithOnlyStringFields>(), Has.Count.EqualTo(1));
			}
		}


	}
}