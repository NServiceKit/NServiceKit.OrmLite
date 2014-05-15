using Northwind.Common.ServiceModel;
using Northwind.Perf;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.TestsPerf.Scenarios.Text
{
    /// <summary>A northwind supplier to string scenario.</summary>
	public class NorthwindSupplierToStringScenario
		: ScenarioBase
	{
        /// <summary>The supplier.</summary>
		private readonly SupplierDto supplier = NorthwindDtoFactory.Supplier(
			1, "Exotic Liquids", "Charlotte Cooper", "Purchasing Manager", "49 Gilbert St.", "London", null, 
			"EC1 4SD", "UK", "(171) 555-2222", null, null);

        /// <summary>Runs this object.</summary>
		public override void Run()
		{
			TypeSerializer.SerializeToString(supplier);
		}
	}

}