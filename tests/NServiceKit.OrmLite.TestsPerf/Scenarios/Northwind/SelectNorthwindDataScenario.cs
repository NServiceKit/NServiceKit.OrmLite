using System;
using System.Collections.Generic;
using System.Data;
using Northwind.Common.DataModel;
using Northwind.Perf;
using NServiceKit.Common;

namespace NServiceKit.OrmLite.TestsPerf.Scenarios.Northwind
{
    /// <summary>A select northwind data scenario.</summary>
	public class SelectNorthwindDataScenario
		: DatabaseScenarioBase
	{
        /// <summary>Runs the given database.</summary>
        /// <param name="dbCmd">The database.</param>
        protected override void Run(IDbConnection dbCmd)
		{
			if (this.IsFirstRun)
			{
				new InsertNorthwindDataScenario().InsertData(dbCmd);
			}

			dbCmd.Select<Category>();
			dbCmd.Select<Customer>();
			dbCmd.Select<Employee>();
			dbCmd.Select<Shipper>();
			dbCmd.Select<Order>();
			dbCmd.Select<Product>();
			dbCmd.Select<OrderDetail>();
			dbCmd.Select<CustomerCustomerDemo>();
			dbCmd.Select<Region>();
			dbCmd.Select<Territory>();
			dbCmd.Select<EmployeeTerritory>();
		}
	}
}