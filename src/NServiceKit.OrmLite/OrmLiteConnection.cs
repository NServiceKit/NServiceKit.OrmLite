using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using NServiceKit.DataAccess;

namespace NServiceKit.OrmLite
{
    /// <summary>Wrapper IDbConnection class to allow for connection sharing, mocking, etc.</summary>
	public class OrmLiteConnection
		: IDbConnection, IHasDbConnection 
	{
        /// <summary>The factory.</summary>
	    public readonly OrmLiteConnectionFactory Factory;

        /// <summary>Gets the transaction.</summary>
        /// <value>The transaction.</value>
        public IDbTransaction Transaction { get; internal set; }

        /// <summary>The database connection.</summary>
		private IDbConnection dbConnection;

        /// <summary>true if this object is open.</summary>
		private bool isOpen;

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.OrmLiteConnection class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public OrmLiteConnection(OrmLiteConnectionFactory factory)
        {
            this.Factory = factory;
        }

        /// <summary>Gets the database connection.</summary>
        /// <value>The database connection.</value>
        public IDbConnection DbConnection
		{
			get
			{
				if (dbConnection == null)
				{
					dbConnection = Factory.ConnectionString.ToDbConnection(Factory.DialectProvider);
				}
				return dbConnection;
			}
		}

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
		public void Dispose()
		{
            if (Factory.OnDispose != null) Factory.OnDispose(this);
            if (!Factory.AutoDisposeConnection) return;

			DbConnection.Dispose();
			dbConnection = null;
			isOpen = false;
        }

        /// <summary>Begins a database transaction.</summary>
        /// <returns>An object representing the new transaction.</returns>
		public IDbTransaction BeginTransaction()
		{
			if (Factory.AlwaysReturnTransaction != null)
				return Factory.AlwaysReturnTransaction;

            Transaction = DbConnection.BeginTransaction();
            return Transaction;
		}

        /// <summary>
        /// Begins a database transaction with the specified <see cref="T:System.Data.IsolationLevel" />
        /// value.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>An object representing the new transaction.</returns>
        /// ### <param name="il">One of the <see cref="T:System.Data.IsolationLevel" /> values.</param>
		public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			if (Factory.AlwaysReturnTransaction != null)
				return Factory.AlwaysReturnTransaction;

			Transaction = DbConnection.BeginTransaction(isolationLevel);
            return Transaction;
		}

        /// <summary>Closes the connection to the database.</summary>
		public void Close()
		{
            DbConnection.Close();
        }

        /// <summary>Changes the current database for an open Connection object.</summary>
        /// <param name="databaseName">The name of the database to use in place of the current database.</param>
		public void ChangeDatabase(string databaseName)
		{
			DbConnection.ChangeDatabase(databaseName);
		}

        /// <summary>Creates and returns a Command object associated with the connection.</summary>
        /// <returns>A Command object associated with the connection.</returns>
		public IDbCommand CreateCommand()
		{
			if (Factory.AlwaysReturnCommand != null)
				return Factory.AlwaysReturnCommand;

			var cmd = DbConnection.CreateCommand();
            if(Transaction != null) { cmd.Transaction = Transaction; }
			cmd.CommandTimeout = OrmLiteConfig.CommandTimeout;
            return cmd;
		}

        /// <summary>
        /// Opens a database connection with the settings specified by the ConnectionString property of
        /// the provider-specific Connection object.
        /// </summary>
		public void Open()
		{
			if (isOpen) return;
			
			DbConnection.Open();
            //so the internal connection is wrapped for example by miniprofiler
            if(Factory.ConnectionFilter != null) { dbConnection = Factory.ConnectionFilter(dbConnection); }
			isOpen = true;
		}

        /// <summary>Gets or sets the string used to open a database.</summary>
        /// <value>A string containing connection settings.</value>
		public string ConnectionString
		{
			get { return Factory.ConnectionString; }
			set { Factory.ConnectionString = value; }
		}

        /// <summary>
        /// Gets the time to wait while trying to establish a connection before terminating the attempt
        /// and generating an error.
        /// </summary>
        /// <value>
        /// The time (in seconds) to wait for a connection to open. The default value is 15 seconds.
        /// </value>
		public int ConnectionTimeout
		{
			get { return DbConnection.ConnectionTimeout; }
		}

        /// <summary>
        /// Gets the name of the current database or the database to be used after a connection is opened.
        /// </summary>
        /// <value>
        /// The name of the current database or the name of the database to be used once a connection is
        /// open. The default value is an empty string.
        /// </value>
		public string Database
		{
			get { return DbConnection.Database; }
		}

        /// <summary>Gets the current state of the connection.</summary>
        /// <value>One of the <see cref="T:System.Data.ConnectionState" /> values.</value>
		public ConnectionState State
		{
			get { return DbConnection.State; }
		}

        /// <summary>SqlConnection casting operator.</summary>
        /// <param name="dbConn">The database connection.</param>
		public static explicit operator SqlConnection(OrmLiteConnection dbConn)
		{
			return (SqlConnection)dbConn.DbConnection;
		}

        /// <summary>DbConnection casting operator.</summary>
        /// <param name="dbConn">The database connection.</param>
		public static explicit operator DbConnection(OrmLiteConnection dbConn)
		{
			return (DbConnection)dbConn.DbConnection;
		}
	}
}