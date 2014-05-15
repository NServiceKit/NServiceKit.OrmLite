using Northwind.Common.ServiceModel;
using Northwind.Perf;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.TestsPerf.Scenarios.Text
{
    /// <summary>A northwind customer to string scenario.</summary>
	public class NorthwindCustomerToStringScenario
		: ScenarioBase
	{
        /// <summary>The customer.</summary>
		private readonly CustomerDto customer = NorthwindDtoFactory.Customer(
			1.ToString("x"), "Alfreds Futterkiste", "Maria Anders", "Sales Representative", "Obere Str. 57", 
			"Berlin", null, "12209", "Germany", "030-0074321", "030-0076545", null);

        /// <summary>Runs this object.</summary>
		public override void Run()
		{
			TypeSerializer.SerializeToString(customer);
		}
	}
}