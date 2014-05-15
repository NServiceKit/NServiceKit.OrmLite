using System;
using System.Configuration;
using System.Data;
using System.IO;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.OrmLite.Oracle;


namespace NServiceKit.OrmLite.Oracle.Tests
{
    /// <summary>An oracle test base.</summary>
    public class OracleTestBase
	{
        /// <summary>Gets or sets the connection string.</summary>
        /// <value>The connection string.</value>
		protected virtual string ConnectionString { get; set; }

        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			LogManager.LogFactory = new ConsoleLogFactory();

            OrmLiteConfig.DialectProvider = OracleOrmLiteDialectProvider.Instance;
			OrmLiteConfig.ClearCache();
		    ConnectionString = ConfigurationManager.ConnectionStrings["testDb"].ConnectionString;
		}

        /// <summary>Opens database connection.</summary>
        /// <param name="connString">The connection string.</param>
        /// <returns>An IDbConnection.</returns>
        public IDbConnection OpenDbConnection(string connString = null)
        {
            connString = connString ?? ConnectionString;
            return connString.OpenDbConnection();
        }
    }
}