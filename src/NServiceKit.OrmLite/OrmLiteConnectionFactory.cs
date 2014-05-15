using System;
using System.Collections.Generic;
using System.Data;

namespace NServiceKit.OrmLite
{
    /// <summary>
    /// Allow for mocking and unit testing by providing non-disposing connection factory with
    /// injectable IDbCommand and IDbTransaction proxies.
    /// </summary>
    public class OrmLiteConnectionFactory : IDbConnectionFactory
    {
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.OrmLiteConnectionFactory class.
        /// </summary>
        public OrmLiteConnectionFactory()
            : this(null, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.OrmLiteConnectionFactory class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public OrmLiteConnectionFactory(string connectionString)
            : this(connectionString, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.OrmLiteConnectionFactory class.
        /// </summary>
        /// <param name="connectionString">     The connection string.</param>
        /// <param name="autoDisposeConnection">true to automatically dispose connection.</param>
        public OrmLiteConnectionFactory(string connectionString, bool autoDisposeConnection)
            : this(connectionString, autoDisposeConnection, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.OrmLiteConnectionFactory class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dialectProvider"> The dialect provider.</param>
        public OrmLiteConnectionFactory(string connectionString, IOrmLiteDialectProvider dialectProvider)
            : this(connectionString, true, dialectProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.OrmLiteConnectionFactory class.
        /// </summary>
        /// <param name="connectionString">     The connection string.</param>
        /// <param name="autoDisposeConnection">true to automatically dispose connection.</param>
        /// <param name="dialectProvider">      The dialect provider.</param>
        public OrmLiteConnectionFactory(string connectionString, bool autoDisposeConnection, IOrmLiteDialectProvider dialectProvider)
            : this(connectionString, autoDisposeConnection, dialectProvider, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.OrmLiteConnectionFactory class.
        /// </summary>
        /// <param name="connectionString">     The connection string.</param>
        /// <param name="autoDisposeConnection">true to automatically dispose connection.</param>
        /// <param name="dialectProvider">      The dialect provider.</param>
        /// <param name="setGlobalConnection">  true to set global connection.</param>
        public OrmLiteConnectionFactory(string connectionString, bool autoDisposeConnection, IOrmLiteDialectProvider dialectProvider, bool setGlobalConnection)
        {
            ConnectionString = connectionString;
            AutoDisposeConnection = autoDisposeConnection;
            this.DialectProvider = dialectProvider ?? OrmLiteConfig.DialectProvider;

            if (setGlobalConnection && dialectProvider != null)
            {
                OrmLiteConfig.DialectProvider = dialectProvider;
            }

            this.ConnectionFilter = x => x;
        }

        /// <summary>Gets or sets the dialect provider.</summary>
        /// <value>The dialect provider.</value>
        public IOrmLiteDialectProvider DialectProvider { get; set; }

        /// <summary>Gets or sets the connection string.</summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the automatic dispose connection.
        /// </summary>
        /// <value>true if automatic dispose connection, false if not.</value>
        public bool AutoDisposeConnection { get; set; }

        /// <summary>Gets or sets the connection filter.</summary>
        /// <value>The connection filter.</value>
        public Func<IDbConnection, IDbConnection> ConnectionFilter { get; set; }

        /// <summary>Force the IDbConnection to always return this IDbCommand.</summary>
        /// <value>The always return command.</value>
        public IDbCommand AlwaysReturnCommand { get; set; }

        /// <summary>Force the IDbConnection to always return this IDbTransaction.</summary>
        /// <value>The always return transaction.</value>
        public IDbTransaction AlwaysReturnTransaction { get; set; }

        /// <summary>Gets or sets the on dispose.</summary>
        /// <value>The on dispose.</value>
        public Action<OrmLiteConnection> OnDispose { get; set; }

        /// <summary>The ORM lite connection.</summary>
        private OrmLiteConnection ormLiteConnection;

        /// <summary>Gets the ORM lite connection.</summary>
        /// <value>The ORM lite connection.</value>
        private OrmLiteConnection OrmLiteConnection
        {
            get
            {
                if (ormLiteConnection == null)
                {
                    ormLiteConnection = new OrmLiteConnection(this);
                }
                return ormLiteConnection;
            }
        }

        /// <summary>Opens database connection.</summary>
        /// <returns>An IDbConnection.</returns>
        public IDbConnection OpenDbConnection()
        {
            var connection = CreateDbConnection();
            connection.Open();

            return connection;
        }

        /// <summary>Creates database connection.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <returns>The new database connection.</returns>
        public IDbConnection CreateDbConnection()
        {
            if (this.ConnectionString == null)
                throw new ArgumentNullException("ConnectionString", "ConnectionString must be set");

            var connection = AutoDisposeConnection
                ? new OrmLiteConnection(this)
                : OrmLiteConnection;

            //moved setting up the ConnectionFilter to OrmLiteConnection.Open
            //return ConnectionFilter(connection);
            return connection;
        }

        /// <summary>Opens database connection.</summary>
        /// <exception cref="KeyNotFoundException">Thrown when a Key Not Found error condition occurs.</exception>
        /// <param name="connectionKey">The connection key.</param>
        /// <returns>An IDbConnection.</returns>
        public IDbConnection OpenDbConnection(string connectionKey)
        {
            OrmLiteConnectionFactory factory;
            if (!NamedConnections.TryGetValue(connectionKey, out factory))
                throw new KeyNotFoundException("No factory registered is named " + connectionKey);

            IDbConnection connection = factory.AutoDisposeConnection
                ? new OrmLiteConnection(factory)
                : factory.OrmLiteConnection;

            //moved setting up the ConnectionFilter to OrmLiteConnection.Open
            //connection = factory.ConnectionFilter(connection);
            connection.Open();

            return connection;
        }

        /// <summary>The named connections.</summary>
        private static Dictionary<string, OrmLiteConnectionFactory> namedConnections;

        /// <summary>Gets the named connections.</summary>
        /// <value>The named connections.</value>
        public static Dictionary<string, OrmLiteConnectionFactory> NamedConnections
        {
            get
            {
                return namedConnections = namedConnections
                    ?? (namedConnections = new Dictionary<string, OrmLiteConnectionFactory>());
            }
        }

        /// <summary>Registers the connection.</summary>
        /// <param name="connectionKey">        The connection key.</param>
        /// <param name="connectionString">     The connection string.</param>
        /// <param name="dialectProvider">      The dialect provider.</param>
        /// <param name="autoDisposeConnection">true to automatically dispose connection.</param>
        public void RegisterConnection(string connectionKey, string connectionString, IOrmLiteDialectProvider dialectProvider, bool autoDisposeConnection = true)
        {
            RegisterConnection(connectionKey, new OrmLiteConnectionFactory(connectionString, autoDisposeConnection, dialectProvider, autoDisposeConnection));
        }

        /// <summary>Registers the connection.</summary>
        /// <param name="connectionKey">    The connection key.</param>
        /// <param name="connectionFactory">The connection factory.</param>
        public void RegisterConnection(string connectionKey, OrmLiteConnectionFactory connectionFactory)
        {
            NamedConnections[connectionKey] = connectionFactory;
        }
    }

    /// <summary>An ORM lite connection factory extensions.</summary>
    public static class OrmLiteConnectionFactoryExtensions
    {
        /// <summary>
        /// (This method is obsolete) an IDbConnectionFactory extension method that execs.
        /// </summary>
        /// <param name="connectionFactory">The connectionFactory to act on.</param>
        /// <param name="runDbCommandsFn">  The run database commands function.</param>
        [Obsolete("Use IDbConnectionFactory.Run(IDbConnection db => ...) extension method instead")]
        public static void Exec(this IDbConnectionFactory connectionFactory, Action<IDbCommand> runDbCommandsFn)
        {
            using (var dbConn = connectionFactory.OpenDbConnection())
            using (var dbCmd = dbConn.CreateCommand())
            {
                runDbCommandsFn(dbCmd);
            }
        }

        /// <summary>
        /// (This method is obsolete) an IDbConnectionFactory extension method that execs.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="connectionFactory">The connectionFactory to act on.</param>
        /// <param name="runDbCommandsFn">  The run database commands function.</param>
        /// <returns>A T.</returns>
        [Obsolete("Use IDbConnectionFactory.Run(IDbConnection db => ...) extension method instead")]
        public static T Exec<T>(this IDbConnectionFactory connectionFactory, Func<IDbCommand, T> runDbCommandsFn)
        {
            using (var dbConn = connectionFactory.OpenDbConnection())
            using (var dbCmd = dbConn.CreateCommand())
            {
                return runDbCommandsFn(dbCmd);
            }
        }

        /// <summary>An IDbConnectionFactory extension method that runs.</summary>
        /// <param name="connectionFactory">The connectionFactory to act on.</param>
        /// <param name="runDbCommandsFn">  The run database commands function.</param>
        public static void Run(this IDbConnectionFactory connectionFactory, Action<IDbConnection> runDbCommandsFn)
        {
            using (var dbConn = connectionFactory.OpenDbConnection())
            {
                runDbCommandsFn(dbConn);
            }
        }

        /// <summary>An IDbConnectionFactory extension method that runs.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="connectionFactory">The connectionFactory to act on.</param>
        /// <param name="runDbCommandsFn">  The run database commands function.</param>
        /// <returns>A T.</returns>
        public static T Run<T>(this IDbConnectionFactory connectionFactory, Func<IDbConnection, T> runDbCommandsFn)
        {
            using (var dbConn = connectionFactory.OpenDbConnection())
            {
                return runDbCommandsFn(dbConn);
            }
        }

        /// <summary>An IDbConnectionFactory extension method that opens.</summary>
        /// <param name="connectionFactory">The connectionFactory to act on.</param>
        /// <returns>An IDbConnection.</returns>
        public static IDbConnection Open(this IDbConnectionFactory connectionFactory)
        {
            return connectionFactory.OpenDbConnection();
        }

        /// <summary>An IDbConnectionFactory extension method that opens.</summary>
        /// <param name="connectionFactory">The connectionFactory to act on.</param>
        /// <param name="namedConnection">  The named connection.</param>
        /// <returns>An IDbConnection.</returns>
        public static IDbConnection Open(this IDbConnectionFactory connectionFactory, string namedConnection)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnection(namedConnection);
        }

        /// <summary>
        /// An IDbConnectionFactory extension method that opens database connection.
        /// </summary>
        /// <param name="connectionFactory">The connectionFactory to act on.</param>
        /// <param name="namedConnection">  The named connection.</param>
        /// <returns>An IDbConnection.</returns>
        public static IDbConnection OpenDbConnection(this IDbConnectionFactory connectionFactory, string namedConnection)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnection(namedConnection);
        }
    }
}