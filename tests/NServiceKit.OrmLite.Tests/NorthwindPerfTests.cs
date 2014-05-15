using System;
using System.Data;
using System.Diagnostics;
using Northwind.Common.DataModel;
using NUnit.Framework;
using NServiceKit.DataAccess;
using NServiceKit.OrmLite.Sqlite;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>A northwind performance tests.</summary>
	[Ignore("Perf test")]
	[TestFixture]
	public class NorthwindPerfTests
	{
        /// <summary>Loads northwind database with ORM lite sqlite memory database.</summary>
		[Test]
		public void Load_Northwind_database_with_OrmLite_sqlite_memory_db()
		{
			OrmLiteConfig.DialectProvider = new SqliteOrmLiteDialectProvider();

			NorthwindData.LoadData(false);
			GC.Collect();

			var stopWatch = new Stopwatch();
			stopWatch.Start();

			using (var db = ":memory:".OpenDbConnection())
			{
				using (var client = new OrmLitePersistenceProvider(db))
				{
					OrmLiteNorthwindTests.CreateNorthwindTables(db);
					LoadNorthwindData(client);
				}
			}

			Console.WriteLine("stopWatch.ElapsedMilliseconds: " + stopWatch.ElapsedMilliseconds);
		}

        /// <summary>Loads northwind data.</summary>
        /// <param name="persistenceProvider">The persistence provider.</param>
		private static void LoadNorthwindData(IBasicPersistenceProvider persistenceProvider)
		{
			persistenceProvider.StoreAll(NorthwindData.Categories);
			persistenceProvider.StoreAll(NorthwindData.Customers);
			persistenceProvider.StoreAll(NorthwindData.Employees);
			persistenceProvider.StoreAll(NorthwindData.Shippers);
            persistenceProvider.StoreAll(NorthwindData.Suppliers);
			persistenceProvider.StoreAll(NorthwindData.Orders);
			persistenceProvider.StoreAll(NorthwindData.Products);
			persistenceProvider.StoreAll(NorthwindData.OrderDetails);
			persistenceProvider.StoreAll(NorthwindData.CustomerCustomerDemos);
			persistenceProvider.StoreAll(NorthwindData.Regions);
			persistenceProvider.StoreAll(NorthwindData.Territories);
			persistenceProvider.StoreAll(NorthwindData.EmployeeTerritories);
		}
	}
}