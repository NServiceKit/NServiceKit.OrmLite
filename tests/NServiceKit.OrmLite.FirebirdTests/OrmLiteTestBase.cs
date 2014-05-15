using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Configuration;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.OrmLite.Firebird;

namespace NServiceKit.OrmLite.FirebirdTests
{
    /// <summary>An ORM lite test base.</summary>
	public class OrmLiteTestBase
	{		
        /// <summary>Gets or sets the connection string.</summary>
        /// <value>The connection string.</value>
		protected virtual string ConnectionString { get; set; }

        /// <summary>Gets file connection string.</summary>
        /// <returns>The file connection string.</returns>
		protected string GetFileConnectionString()
		{
            return ConfigurationManager.ConnectionStrings["testDb"].ConnectionString;
        }

        /// <summary>Creates new database.</summary>
		protected void CreateNewDatabase()
		{
			ConnectionString = GetFileConnectionString();
		}

        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			LogManager.LogFactory = new ConsoleLogFactory();

			OrmLiteConfig.DialectProvider = FirebirdOrmLiteDialectProvider.Instance;
			ConnectionString = GetFileConnectionString();
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
