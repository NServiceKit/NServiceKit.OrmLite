using System;
using System.Configuration;
using System.Data;
using System.IO;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.OrmLite.PostgreSQL;


namespace NServiceKit.OrmLite.Tests
{
    /// <summary>An ORM lite test base.</summary>
	public class OrmLiteTestBase
	{
        /// <summary>Gets or sets the connection string.</summary>
        /// <value>The connection string.</value>
		protected virtual string ConnectionString { get; set; }

        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			LogManager.LogFactory = new ConsoleLogFactory();

		    OrmLiteConfig.DialectProvider =  PostgreSQLDialectProvider.Instance;
			OrmLiteConfig.DialectProvider.NamingStrategy = new OrmLiteNamingStrategyBase();
			OrmLiteConfig.ClearCache();
		    ConnectionString = ConfigurationManager.ConnectionStrings["testDb"].ConnectionString;
		}

        /// <summary>Logs.</summary>
        /// <param name="text">The text.</param>
		public void Log(string text)
		{
			Console.WriteLine(text);
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