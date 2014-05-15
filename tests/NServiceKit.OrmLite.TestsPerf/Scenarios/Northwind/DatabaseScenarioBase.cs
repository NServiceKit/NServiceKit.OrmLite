using System;
using System.Data;
using NServiceKit.OrmLite;

namespace Northwind.Perf
{
    /// <summary>A database scenario base.</summary>
	public abstract class DatabaseScenarioBase
		: ScenarioBase, IDisposable
	{
        /// <summary>Gets or sets the connection string.</summary>
        /// <value>The connection string.</value>
		public string ConnectionString { get; set; }

        /// <summary>The iteration.</summary>
		protected int Iteration;

        /// <summary>Gets a value indicating whether this object is first run.</summary>
        /// <value>true if this object is first run, false if not.</value>
		public bool IsFirstRun
		{
			get
			{
				return this.Iteration == 0;
			}
		}

        /// <summary>The database.</summary>
        private IDbConnection db;

        /// <summary>Gets the database.</summary>
        /// <value>The database.</value>
        protected IDbConnection Db
		{
			get
			{
				if (db == null)
				{
				    var connStr = ConnectionString;
                    db = connStr.OpenDbConnection();
				}
				return db;
			}
		}

        /// <summary>Runs the given database.</summary>
		public override void Run()
		{
			Run(Db);
			this.Iteration++;
		}

        /// <summary>Runs the given database.</summary>
        /// <param name="db">The database.</param>
        protected abstract void Run(IDbConnection db);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
		public void Dispose()
		{
			if (this.db == null) return;

			try
			{
				this.db.Dispose();
				this.db = null;
			}
			finally
			{
			}
		}
	}
}