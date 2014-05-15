using System;
using System.Data;
using System.Diagnostics;
using Northwind.Common.DataModel;
using NUnit.Framework;
using NServiceKit.DataAccess;
using NServiceKit.OrmLite.PostgreSQL;

namespace NServiceKit.OrmLite.Tests
{
	//[Ignore("Perf test")]

    /// <summary>A northwind performance tests.</summary>
	[TestFixture]
	public class NorthwindPerfTests : OrmLiteTestBase
	{
        /// <summary>Loads northwind database with ormlite postgresql.</summary>
		[Test]
		public void Load_Northwind_database_with_ormlite_postgresql()
		{
		    OrmLiteConfig.DialectProvider =  PostgreSQLDialectProvider.Instance;

			NorthwindData.LoadData(false);
			GC.Collect();

			var stopWatch = new Stopwatch();
			stopWatch.Start();

            using (var db = OpenDbConnection())
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
			persistenceProvider.StoreAll(NorthwindData.Orders);
			persistenceProvider.StoreAll(NorthwindData.Suppliers);
			persistenceProvider.StoreAll(NorthwindData.Products);
			persistenceProvider.StoreAll(NorthwindData.OrderDetails);
			persistenceProvider.StoreAll(NorthwindData.CustomerCustomerDemos);
			persistenceProvider.StoreAll(NorthwindData.Regions);
			persistenceProvider.StoreAll(NorthwindData.Territories);
			persistenceProvider.StoreAll(NorthwindData.EmployeeTerritories);
		}
	}
}