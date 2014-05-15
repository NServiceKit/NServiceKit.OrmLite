using System;
using System.Collections.Generic;
using Northwind.Perf;
using NServiceKit.Common.Utils;
using NServiceKit.OrmLite.Sqlite;

namespace NServiceKit.OrmLite.TestsPerf.Scenarios.OrmLite
{
    /// <summary>An ORM lite scenrio configuration.</summary>
	public class OrmLiteScenrioConfig
	{
        /// <summary>The data provider and connection strings.</summary>
		private static readonly Dictionary<IOrmLiteDialectProvider, List<string>> DataProviderAndConnectionStrings 
			= new Dictionary<IOrmLiteDialectProvider, List<string>> {
				{ 
					new SqliteOrmLiteDialectProvider(), 
					new List<string>
                  	{
                  		":memory:",
						"~/App_Data/perf.sqlite".MapAbsolutePath(),
                  	} 
				}
			};

        /// <summary>Enumerates data provider configuration runs in this collection.</summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process data provider configuration runs in
        /// this collection.
        /// </returns>
		public static IEnumerable<OrmLiteConfigRun> DataProviderConfigRuns()
		{
			foreach (var providerConnectionString in DataProviderAndConnectionStrings)
			{
				var dialectProvider = providerConnectionString.Key;
				var connectionStrings = providerConnectionString.Value;

				foreach (var connectionString in connectionStrings)
				{
					yield return new OrmLiteConfigRun {
						DialectProvider = dialectProvider,
						ConnectionString = connectionString,
					};
				}
			}
		}
	}

    /// <summary>An ORM lite configuration run.</summary>
	public class OrmLiteConfigRun
	{
        /// <summary>Gets or sets the dialect provider.</summary>
        /// <value>The dialect provider.</value>
		public IOrmLiteDialectProvider DialectProvider { get; set; }

        /// <summary>Gets or sets the connection string.</summary>
        /// <value>The connection string.</value>
		public string ConnectionString { get; set; }

        /// <summary>Gets or sets the property invoker.</summary>
        /// <value>The property invoker.</value>
		public IPropertyInvoker PropertyInvoker { get; set; }

        /// <summary>Initialises this object.</summary>
        /// <param name="scenarioBase">The scenario base.</param>
		public void Init(ScenarioBase scenarioBase)
		{
			var dbScenarioBase = scenarioBase as DatabaseScenarioBase;
			if (dbScenarioBase == null) return;

			OrmLiteConfig.DialectProvider = this.DialectProvider;

			OrmLiteConfig.ClearCache();
			//PropertyInvoker.ConvertValueFn = OrmLiteConfig.DialectProvider.ConvertDbValue;

			dbScenarioBase.ConnectionString = this.ConnectionString;
		}

	}

}