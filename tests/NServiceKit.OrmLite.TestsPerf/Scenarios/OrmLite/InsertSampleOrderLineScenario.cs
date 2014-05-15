using System;
using System.Data;
using Northwind.Perf;
using NServiceKit.Common;
using NServiceKit.OrmLite.TestsPerf.Model;

namespace NServiceKit.OrmLite.TestsPerf.Scenarios.OrmLite
{
    /// <summary>An insert sample order line scenario.</summary>
	public class InsertSampleOrderLineScenario
		: DatabaseScenarioBase
	{
        /// <summary>Identifier for the user.</summary>
		private readonly Guid userId = Guid.NewGuid();

        /// <summary>Runs the given database.</summary>
        /// <param name="db">The database.</param>
        protected override void Run(IDbConnection db)
		{
			if (this.IsFirstRun)
			{
				db.CreateTable<SampleOrderLine>(true);
			}

			db.Insert(SampleOrderLine.Create(userId, this.Iteration, 1));
		}
	}

    /// <summary>A select one sample order line scenario.</summary>
	public class SelectOneSampleOrderLineScenario
		: DatabaseScenarioBase
	{
        /// <summary>Identifier for the user.</summary>
		private readonly Guid userId = Guid.NewGuid();

        /// <summary>Runs the given database.</summary>
        /// <param name="db">The database.</param>
        protected override void Run(IDbConnection db)
		{
			if (this.IsFirstRun)
			{
				db.CreateTable<SampleOrderLine>(true);
				db.Insert(SampleOrderLine.Create(userId, this.Iteration, 1));
			}

			var row = db.Select<SampleOrderLine>();
		}
	}

    /// <summary>A select many sample order line scenario.</summary>
	public class SelectManySampleOrderLineScenario
		: DatabaseScenarioBase
	{
        /// <summary>Identifier for the user.</summary>
		private readonly Guid userId = Guid.NewGuid();

        /// <summary>Runs the given database.</summary>
        /// <param name="db">The database.</param>
        protected override void Run(IDbConnection db)
		{
			if (this.IsFirstRun)
			{
				db.CreateTable<SampleOrderLine>(true);
				20.Times(i => db.Insert(SampleOrderLine.Create(userId, i, 1)));
			}

			var rows = db.Select<SampleOrderLine>();
		}
	}
}