using System;
using System.Collections.Generic;
using System.Diagnostics;
using Northwind.Perf;
using NServiceKit.Logging;
using NServiceKit.OrmLite.TestsPerf.Scenarios.Northwind;
using NServiceKit.OrmLite.TestsPerf.Scenarios.OrmLite;

namespace NServiceKit.OrmLite.TestsPerf
{
    /// <summary>A program.</summary>
	class Program
	{
        /// <summary>The log.</summary>
		private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        /// <summary>The default iterations.</summary>
		const long DefaultIterations = 1;

        /// <summary>The batch iterations.</summary>
		static readonly List<long> BatchIterations = new List<long> { 100, 1000, 5000, 20000, /*100000, 250000, 1000000, 5000000*/ };
		//static readonly List<long> BatchIterations = new List<long> { 1, 10, 100 };

        /// <summary>Gets use cases.</summary>
        /// <returns>The use cases.</returns>
		static List<DatabaseScenarioBase> GetUseCases()
		{
			return new List<DatabaseScenarioBase>
			{
				//new InsertModelWithFieldsOfDifferentTypesPerfScenario(),
				//new InsertSampleOrderLineScenario(),
				//new SelectOneModelWithFieldsOfDifferentTypesPerfScenario(),
				//new SelectOneSampleOrderLineScenario(),
				//new SelectManyModelWithFieldsOfDifferentTypesPerfScenario(),
				//new SelectManySampleOrderLineScenario(),
				new InsertNorthwindDataScenario(),
			};
		}

        /// <summary>Main entry-point for this application.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="args">The arguments.</param>
		static void Main(string[] args)
		{
			try
			{
				foreach (var configRun in OrmLiteScenrioConfig.DataProviderConfigRuns())
				{
					Console.WriteLine("\n\nStarting config run {0}...", configRun);
					if (args.Length == 1 && args[0] == "csv")
						RunBatch(configRun);
					else
						RunInteractive(configRun, args);
				}

				Console.ReadKey();
			}
			catch (Exception ex)
			{
				Log.Error("Error running perfs", ex);
				throw;
			}
		}

        /// <summary>Executes the batch operation.</summary>
        /// <param name="configRun">The configuration run.</param>
		private static void RunBatch(OrmLiteConfigRun configRun)
		{
			Console.Write(";");
			var useCases = GetUseCases();

			useCases.ForEach(uc => Console.Write("{0};", uc.GetType().Name));
			Console.WriteLine();
			BatchIterations.ForEach(iterations => {
				Console.Write("{0};", iterations);
				useCases.ForEach(uc => {

					configRun.Init(uc);

					// warmup
					uc.Run();
					GC.Collect();
					Console.Write("{0};", Measure(uc.Run, iterations));
				});
				Console.WriteLine();
			});
		}

        /// <summary>Executes the interactive operation.</summary>
        /// <param name="configRun">The configuration run.</param>
        /// <param name="args">     The arguments.</param>
		private static void RunInteractive(OrmLiteConfigRun configRun, string[] args)
		{
			long iterations = DefaultIterations;

			if (args.Length != 0)
				iterations = long.Parse(args[0]);

			Console.WriteLine("Running {0} iterations for each use case.", iterations);

			var useCases = GetUseCases();
			useCases.ForEach(uc => {

				configRun.Init(uc);

				// warmup
				uc.Run();
				GC.Collect();
               	
				var avgMs = Measure(uc.Run, iterations);
				Console.WriteLine("{0}: Avg: {1}ms", uc.GetType().Name, avgMs);
			});
		}

        /// <summary>Measures.</summary>
        /// <param name="action">    The action.</param>
        /// <param name="iterations">The iterations.</param>
        /// <returns>A decimal.</returns>
		private static decimal Measure(Action action, decimal iterations)
		{
			GC.Collect();
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var begin = stopWatch.ElapsedMilliseconds;

			for (var i = 0; i < iterations; i++)
			{
				action();
			}

			var end = stopWatch.ElapsedMilliseconds;

			return (end - begin) / iterations;
		}
	}
}
