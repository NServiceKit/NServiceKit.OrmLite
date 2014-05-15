using System.Collections.Generic;
using NUnit.Framework;
using NServiceKit.OrmLite.TestsPerf.Scenarios.Northwind;

namespace NServiceKit.OrmLite.TestsPerf.PerfTests
{
    /// <summary>A northwind performance tests.</summary>
	[Ignore]
	[TestFixture]
	public class NorthwindPerfTests
		: OrmLitePerfTests
	{
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.TestsPerf.PerfTests.NorthwindPerfTests
        /// class.
        /// </summary>
		public NorthwindPerfTests()
		{
			//this.MultipleIterations = new List<int> { 1000, 10000, 100000 };
			this.MultipleIterations = new List<int> { 10 };
		}

        /// <summary>Tests fixture tear down.</summary>
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			WriteLog();
		}

        /// <summary>Executes the insert northwind order scenario operation.</summary>
		[Test]
		public void Run_InsertNorthwindOrderScenario()
		{
			RunMultipleTimes(new InsertNorthwindOrderScenario());
		}

        /// <summary>Executes the insert northwind customer scenario operation.</summary>
		[Test]
		public void Run_InsertNorthwindCustomerScenario()
		{
			RunMultipleTimes(new InsertNorthwindCustomerScenario());
		}

        /// <summary>Executes the insert northwind supplier scenario operation.</summary>
		[Test]
		public void Run_InsertNorthwindSupplierScenario()
		{
			RunMultipleTimes(new InsertNorthwindSupplierScenario());
		}

        /// <summary>Executes the select northwind supplier scenario operation.</summary>
		[Test]
		public void Run_SelectNorthwindSupplierScenario()
		{
			RunMultipleTimes(new SelectNorthwindSupplierScenario());
		}

        /// <summary>Executes the insert northwind data scenario operation.</summary>
		[Test]
		public void Run_InsertNorthwindDataScenario()
		{
			RunMultipleTimes(new InsertNorthwindDataScenario());
		}

        /// <summary>Executes the select northwind data scenario operation.</summary>
		[Test]
		public void Run_SelectNorthwindDataScenario()
		{
			RunMultipleTimes(new SelectNorthwindDataScenario());
		}

	}

}