using System;
using System.Data;
using System.IO;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.DataAccess;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>A configuration.</summary>
    public class Config
    {
        /// <summary>The sqlite memory database.</summary>
        public static string SqliteMemoryDb = ":memory:";

        /// <summary>The sqlite file dir.</summary>
        public static string SqliteFileDir = "~/App_Data/".MapAbsolutePath();

        /// <summary>The sqlite file database.</summary>
        public static string SqliteFileDb = "~/App_Data/db.sqlite".MapAbsolutePath();

        /// <summary>The SQL server database.</summary>
        public static string SqlServerDb = "~/App_Data/Database1.mdf".MapAbsolutePath();

        /// <summary>.</summary>
        public static string SqlServerBuildDb = "Server=pc;Database=test;User Id=pc;Password=pc;";
        //public static string SqlServerBuildDb = "Data Source=localhost;Initial Catalog=TestDb;Integrated Security=SSPI;Connect Timeout=120;MultipleActiveResultSets=True";

        /// <summary>The default provider.</summary>
        public static IOrmLiteDialectProvider DefaultProvider = SqlServerDialect.Provider;

        /// <summary>The default connection.</summary>
        public static string DefaultConnection = SqlServerBuildDb;

        /// <summary>Gets default connection.</summary>
        /// <returns>The default connection.</returns>
        public static string GetDefaultConnection()
        {
            OrmLiteConfig.DialectProvider = DefaultProvider;
            return DefaultConnection;
        }

        /// <summary>Opens database connection.</summary>
        /// <returns>An IDbConnection.</returns>
        public static IDbConnection OpenDbConnection()
        {
            return GetDefaultConnection().OpenDbConnection();
        }
    }

    /// <summary>An ORM lite test base.</summary>
	public class OrmLiteTestBase
	{
        /// <summary>Gets or sets the connection string.</summary>
        /// <value>The connection string.</value>
	    protected virtual string ConnectionString { get; set; }

        /// <summary>Gets connection string.</summary>
        /// <returns>The connection string.</returns>
		protected string GetConnectionString()
		{
			return GetFileConnectionString();
		}

        /// <summary>Creates SQL server database factory.</summary>
        /// <returns>The new SQL server database factory.</returns>
	    public static OrmLiteConnectionFactory CreateSqlServerDbFactory()
	    {
            var dbFactory = new OrmLiteConnectionFactory(Config.SqlServerBuildDb, SqlServerDialect.Provider);
	        return dbFactory;
	    }

        /// <summary>Gets file connection string.</summary>
        /// <returns>The file connection string.</returns>
	    protected virtual string GetFileConnectionString()
		{
            var connectionString = Config.SqliteFileDb;
			if (File.Exists(connectionString))
				File.Delete(connectionString);

			return connectionString;
		}

        /// <summary>Creates new database.</summary>
		protected void CreateNewDatabase()
		{
			if (ConnectionString.Contains(".sqlite"))
				ConnectionString = GetFileConnectionString();
		}

        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			LogManager.LogFactory = new ConsoleLogFactory();

            OrmLiteConfig.DialectProvider = SqliteDialect.Provider;
            ConnectionString = GetFileConnectionString();
            ConnectionString = ":memory:";
            return;

            //OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            //ConnectionString = Config.SqlServerBuildDb;
            //ConnectionString = "~/App_Data/Database1.mdf".MapAbsolutePath();			

		    ConnectionString = Config.GetDefaultConnection();
		}

        /// <summary>Logs.</summary>
        /// <param name="text">The text.</param>
		public void Log(string text)
		{
			Console.WriteLine(text);
		}

        /// <summary>Gets or sets the in memory database connection.</summary>
        /// <value>The in memory database connection.</value>
        public IDbConnection InMemoryDbConnection { get; set; }

        /// <summary>Opens database connection.</summary>
        /// <param name="connString">The connection string.</param>
        /// <returns>An IDbConnection.</returns>
        public IDbConnection OpenDbConnection(string connString = null)
        {
            connString = connString ?? ConnectionString;
            if (connString == ":memory:")
            {
                if (InMemoryDbConnection == null)
                {
                    var dbConn = connString.OpenDbConnection();
                    InMemoryDbConnection = new OrmLiteConnectionWrapper(dbConn)
                    {
                        DialectProvider = OrmLiteConfig.DialectProvider,
                        AutoDisposeConnection = false,
                    };                    
                }

                return InMemoryDbConnection;
            }

            return connString.OpenDbConnection();            
        }
	}
}