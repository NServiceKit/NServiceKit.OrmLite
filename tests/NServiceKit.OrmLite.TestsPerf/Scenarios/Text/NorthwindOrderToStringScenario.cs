using System;
using Northwind.Common.ServiceModel;
using Northwind.Perf;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.TestsPerf.Scenarios.Text
{
    /// <summary>A northwind order to string scenario.</summary>
	public class NorthwindOrderToStringScenario
		: ScenarioBase
	{
        /// <summary>The order.</summary>
		readonly OrderDto order = NorthwindDtoFactory.Order(
			1, "VINET", 5, new DateTime(1996, 7, 4), new DateTime(1996, 1, 8), new DateTime(1996, 7, 16), 
			3, 32.38m, "Vins et alcools Chevalier", "59 rue de l'Abbaye", "Reims", null, "51100", "France");

        /// <summary>Runs this object.</summary>
		public override void Run()
		{
			TypeSerializer.SerializeToString(order);
		}
	}
}