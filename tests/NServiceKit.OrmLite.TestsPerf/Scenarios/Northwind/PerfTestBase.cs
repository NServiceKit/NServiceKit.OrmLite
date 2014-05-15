using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Northwind.Perf
{
    /// <summary>A performance test base.</summary>
	public class PerfTestBase
	{
        /// <summary>Gets or sets the default iterations.</summary>
        /// <value>The default iterations.</value>
		protected int DefaultIterations { get; set; }

        /// <summary>Gets or sets the multiple iterations.</summary>
        /// <value>The multiple iterations.</value>
		protected List<int> MultipleIterations { get; set; }

        /// <summary>Initializes a new instance of the Northwind.Perf.PerfTestBase class.</summary>
		public PerfTestBase()
		{
			this.DefaultIterations = 10000;
			this.MultipleIterations = new List<int> { 1000, 10000, 100000, 1000000 };
		}

        /// <summary>The log.</summary>
		protected StringBuilder SbLog = new StringBuilder();

        /// <summary>Logs.</summary>
        /// <param name="message">The message.</param>
		public virtual void Log(string message)
		{
			//#if DEBUG
			Console.WriteLine(message);
			//#endif
			SbLog.AppendLine(message);
		}

        /// <summary>Logs.</summary>
        /// <param name="message">The message.</param>
        /// <param name="args">   A variable-length parameters list containing arguments.</param>
		public virtual void Log(string message, params object[] args)
		{
			//#if DEBUG
			Console.WriteLine(message, args);
			//#endif
			SbLog.AppendFormat(message, args);
			SbLog.AppendLine();
		}

        /// <summary>Compare multiple runs.</summary>
        /// <param name="run1Name">  Name of the run 1.</param>
        /// <param name="run1Action">The run 1 action.</param>
        /// <param name="run2Name">  Name of the run 2.</param>
        /// <param name="run2Action">The run 2 action.</param>
		protected void CompareMultipleRuns(string run1Name, Action run1Action, string run2Name, Action run2Action)
		{
			WarmUp(run1Action, run2Action);
			foreach (var iteration in this.MultipleIterations)
			{
				Log("{0} times:", iteration);
				CompareRuns(iteration, run1Name, run1Action, run2Name, run2Action);
			}
		}

        /// <summary>Compare runs.</summary>
        /// <param name="run1Name">  Name of the run 1.</param>
        /// <param name="run1Action">The run 1 action.</param>
        /// <param name="run2Name">  Name of the run 2.</param>
        /// <param name="run2Action">The run 2 action.</param>
		protected void CompareRuns(string run1Name, Action run1Action, string run2Name, Action run2Action)
		{
			CompareRuns(DefaultIterations, run1Name, run1Action, run2Name, run2Action);
		}

        /// <summary>Compare runs.</summary>
        /// <param name="iterations">The iterations.</param>
        /// <param name="run1Name">  Name of the run 1.</param>
        /// <param name="run1Action">The run 1 action.</param>
        /// <param name="run2Name">  Name of the run 2.</param>
        /// <param name="run2Action">The run 2 action.</param>
		protected void CompareRuns(int iterations, string run1Name, Action run1Action, string run2Name, Action run2Action)
		{
			var run1 = RunAction(run1Action, DefaultIterations, run1Name);
			var run2 = RunAction(run2Action, DefaultIterations, run2Name);

			var runDiff = run1 - run2;
			var run1IsSlower = runDiff > 0;
			var slowerRun = run1IsSlower ? run1Name : run2Name;
			var fasterRun = run1IsSlower ? run2Name : run1Name;
			var runDiffTime = run1IsSlower ? runDiff : runDiff * -1;
			var runDiffAvg = run1IsSlower ? run1 / run2 : run2 / run1;

			Log("{0} was {1}ms or {2} times slower than {3}",
				slowerRun, runDiffTime, Math.Round(runDiffAvg, 2), fasterRun);
		}

        /// <summary>Warm up.</summary>
        /// <param name="actions">A variable-length parameters list containing actions.</param>
		protected void WarmUp(params Action[] actions)
		{
			foreach (var action in actions)
			{
				action();
				GC.Collect();
			}
		}

        /// <summary>Executes the multiple times operation.</summary>
        /// <param name="action">    The action.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns>A decimal.</returns>
		protected decimal RunMultipleTimes(Action action, string actionName)
		{
			Log("\n");
			WarmUp(action);

			var i = 0;
			var total = 0M;
			foreach (var iteration in this.MultipleIterations)
			{
				i += iteration;
				Log("{0} times:", iteration);
				total += RunAction(action, iteration, actionName ?? "Action");
			}

			return total / i;
		}

        /// <summary>Gets total ticks taken for all iterations.</summary>
        /// <param name="action">    The action.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns>The total ticks taken for all iterations.</returns>
		protected long GetTotalTicksTakenForAllIterations(Action action, string actionName)
		{
			Log("\n");
			try
			{
				WarmUp(action);

				var i = 0;
				var total = 0M;
				foreach (var iteration in this.MultipleIterations)
				{
					i += iteration;
					Log("{0} times:", iteration);
					total += RunAction(action, iteration, actionName ?? "Action");
				}
				return (long)total;
			}
			catch (Exception ex)
			{
				Log("Error in {0}: {1}", actionName, ex);
			}

			return -1;
		}

        /// <summary>Executes the action.</summary>
        /// <param name="action">    The action.</param>
        /// <param name="iterations">The iterations.</param>
        /// <returns>A decimal.</returns>
		protected decimal RunAction(Action action, int iterations)
		{
			return RunAction(action, iterations, null);
		}

        /// <summary>Executes the action.</summary>
        /// <param name="action">    The action.</param>
        /// <param name="iterations">The iterations.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns>A decimal.</returns>
		protected decimal RunAction(Action action, int iterations, string actionName)
		{
			actionName = actionName ?? action.GetType().Name;
			var ticksTaken = Measure(action, iterations);
			var timeSpan = TimeSpan.FromSeconds(ticksTaken * 1d / Stopwatch.Frequency);

			Log("{0} took {1}ms ({2} ticks), avg: {3} ticks", actionName, timeSpan.TotalMilliseconds, ticksTaken, (ticksTaken / iterations));

			return ticksTaken;
		}

        /// <summary>Measures.</summary>
        /// <param name="action">    The action.</param>
        /// <param name="iterations">The iterations.</param>
        /// <returns>A long.</returns>
		protected long Measure(Action action, decimal iterations)
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect(); 
			
			var begin = Stopwatch.GetTimestamp();

			for (var i = 0; i < iterations; i++)
			{
				action();
			}

			var end = Stopwatch.GetTimestamp();

			return (end - begin);
		}
	}
}