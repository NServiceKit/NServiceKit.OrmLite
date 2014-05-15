using System.Data;
using Northwind.Common.DataModel;
using Northwind.Perf;

namespace NServiceKit.OrmLite.TestsPerf.Scenarios.Northwind
{
    /// <summary>A select northwind supplier scenario.</summary>
	public class SelectNorthwindSupplierScenario
		: DatabaseScenarioBase
	{
        /// <summary>Identifier for the supplier.</summary>
		private const int SupplierId = 1;

        /// <summary>Runs the given database.</summary>
        /// <param name="db">The database.</param>
        protected override void Run(IDbConnection db)
		{
			if (this.IsFirstRun)
			{
				db.CreateTable<Supplier>(true);

				db.Insert(NorthwindFactory.Supplier(SupplierId, "Exotic Liquids", "Charlotte Cooper", "Purchasing Manager", "49 Gilbert St.", "London", null, "EC1 4SD", "UK", "(171) 555-2222", null, null));
			}

			db.GetById<Supplier>(SupplierId);
		}
	}
}