using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using NServiceKit.DataAccess;

namespace NServiceKit.OrmLite
{
    /// <summary>Wrapper IDbConnection class to manage db connection events.</summary>
    public class OrmLiteConnectionWrapper
        : IDbConnection, IHasDbConnection
    {
        /// <summary>Gets the transaction.</summary>
        /// <value>The transaction.</value>
        public IDbTransaction Transaction { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the automatic dispose connection.
        /// </summary>
        /// <value>true if automatic dispose connection, false if not.</value>
        public bool AutoDisposeConnection { get; set; }

        /// <summary>Gets or sets the on dispose.</summary>
        /// <value>The on dispose.</value>
        public Action<OrmLiteConnectionWrapper> OnDispose { get; set; }

        /// <summary>Gets or sets the always return transaction.</summary>
        /// <value>The always return transaction.</value>
        public IDbTransaction AlwaysReturnTransaction { get; set; }

        /// <summary>Gets or sets the always return command.</summary>
        /// <value>The always return command.</value>
        public IDbCommand AlwaysReturnCommand { get; set; }

        /// <summary>Gets or sets the connection filter.</summary>
        /// <value>The connection filter.</value>
        public Func<IDbConnection, IDbConnection> ConnectionFilter { get; set; }

        /// <summary>Gets or sets the dialect provider.</summary>
        /// <value>The dialect provider.</value>
        public IOrmLiteDialectProvider DialectProvider { get; set; }

        /// <summary>true if this object is open.</summary>
        private bool isOpen;

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.OrmLiteConnectionWrapper class.
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        public OrmLiteConnectionWrapper(IDbConnection dbConn)
        {
            this.DbConnection = dbConn;
        }

        /// <summary>Gets or sets the database connection.</summary>
        /// <value>The database connection.</value>
        public IDbConnection DbConnection { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            if (OnDispose != null) OnDispose(this);
            if (!AutoDisposeConnection) return;

            DbConnection.Dispose();
            DbConnection = null;
            isOpen = false;
        }

        /// <summary>Begins a database transaction.</summary>
        /// <returns>An object representing the new transaction.</returns>
        public IDbTransaction BeginTransaction()
        {
            if (AlwaysReturnTransaction != null)
                return AlwaysReturnTransaction;

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
            if (AlwaysReturnTransaction != null)
                return AlwaysReturnTransaction;

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
            if (AlwaysReturnCommand != null)
                return AlwaysReturnCommand;

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
            if (ConnectionFilter != null) { DbConnection = ConnectionFilter(DbConnection); }
            isOpen = true;
        }

        /// <summary>Gets or sets the string used to open a database.</summary>
        /// <value>A string containing connection settings.</value>
        public string ConnectionString
        {
            get { return DbConnection.ConnectionString; }
            set { DbConnection.ConnectionString = value; }
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
        public static explicit operator SqlConnection(OrmLiteConnectionWrapper dbConn)
        {
            return (SqlConnection)dbConn.DbConnection;
        }

        /// <summary>DbConnection casting operator.</summary>
        /// <param name="dbConn">The database connection.</param>
        public static explicit operator DbConnection(OrmLiteConnectionWrapper dbConn)
        {
            return (DbConnection)dbConn.DbConnection;
        }
    }
}