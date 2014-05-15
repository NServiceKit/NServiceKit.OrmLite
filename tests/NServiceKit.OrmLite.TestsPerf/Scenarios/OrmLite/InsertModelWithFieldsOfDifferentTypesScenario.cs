using System.Data;
using Northwind.Perf;
using NServiceKit.Common;
using NServiceKit.OrmLite.TestsPerf.Model;

namespace NServiceKit.OrmLite.TestsPerf.Scenarios.OrmLite
{
    /// <summary>An insert model with fields of different types performance scenario.</summary>
	public class InsertModelWithFieldsOfDifferentTypesPerfScenario
		: DatabaseScenarioBase
	{
        /// <summary>Runs the given database.</summary>
        /// <param name="db">The database.</param>
		protected override void Run(IDbConnection db)
		{
			if (this.IsFirstRun)
			{
				db.CreateTable<ModelWithFieldsOfDifferentTypesPerf>(true);
			}

			db.Insert(ModelWithFieldsOfDifferentTypesPerf.Create(this.Iteration));
		}
	}

    /// <summary>
    /// A select one model with fields of different types performance scenario.
    /// </summary>
	public class SelectOneModelWithFieldsOfDifferentTypesPerfScenario
		: DatabaseScenarioBase
	{
        /// <summary>Runs the given database.</summary>
        /// <param name="db">The database.</param>
        protected override void Run(IDbConnection db)
		{
			if (this.IsFirstRun)
			{
				db.CreateTable<ModelWithFieldsOfDifferentTypesPerf>(true);
				db.Insert(ModelWithFieldsOfDifferentTypesPerf.Create(this.Iteration));
			}

			var row = db.Select<ModelWithFieldsOfDifferentTypesPerf>();
		}
	}

    /// <summary>
    /// A select many model with fields of different types performance scenario.
    /// </summary>
	public class SelectManyModelWithFieldsOfDifferentTypesPerfScenario
		: DatabaseScenarioBase
	{
        /// <summary>Runs the given database.</summary>
        /// <param name="db">The database.</param>
        protected override void Run(IDbConnection db)
		{
			if (this.IsFirstRun)
			{
				db.CreateTable<ModelWithFieldsOfDifferentTypesPerf>(true);
				20.Times(i => db.Insert(ModelWithFieldsOfDifferentTypesPerf.Create(i)));
			}

			var rows = db.Select<ModelWithFieldsOfDifferentTypesPerf>();
		}
	}

}