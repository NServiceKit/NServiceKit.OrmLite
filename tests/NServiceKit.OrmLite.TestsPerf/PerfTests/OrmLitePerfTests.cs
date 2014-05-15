using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Northwind.Perf;
using NServiceKit.Common.Utils;
using NServiceKit.OrmLite.TestsPerf.Scenarios.OrmLite;

namespace NServiceKit.OrmLite.TestsPerf.PerfTests
{
    /// <summary>An ORM lite performance tests.</summary>
	public class OrmLitePerfTests
		: PerfTestBase
	{
        /// <summary>The connection strings.</summary>
		protected List<string> ConnectionStrings = new List<string>();

        /// <summary>Writes the log.</summary>
		public void WriteLog()
		{
			var fileName = string.Format("~/App_Data/PerfTests/{0:yyyy-MM-dd}.log", DateTime.Now).MapAbsolutePath();
			using (var writer = new StreamWriter(fileName, true))
			{
				writer.Write(SbLog);
			}
		}

        /// <summary>Executes the multiple times operation.</summary>
        /// <param name="scenarioBase">The scenario base.</param>
		protected void RunMultipleTimes(ScenarioBase scenarioBase)
		{
			foreach (var configRun in OrmLiteScenrioConfig.DataProviderConfigRuns())
			{
				configRun.Init(scenarioBase);

				RunMultipleTimes(scenarioBase.Run, scenarioBase.GetType().Name);
			}
		}

	}

}