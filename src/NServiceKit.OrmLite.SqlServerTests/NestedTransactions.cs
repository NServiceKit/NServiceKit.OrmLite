using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.SqlServerTests
{
    /// <summary>A nested transactions.</summary>
    public class NestedTransactions : OrmLiteTestBase
    {
        /// <summary>
        /// A demostration, that the ThreadStatic Transaction property can cause problems.
        /// </summary>
        [Test]
        public void Can_use_nested_transactions()
        {
            //must use the factory, because that returns an OrmLiteConnection, that can saves the current transaction
            var factory = new OrmLiteConnectionFactory(ConnectionString, SqlServerDialect.Provider);
            //using(var outerConn = OpenDbConnection()) {
            using(var outerConn = factory.OpenDbConnection()) {
                //(re)create tables
                outerConn.DropAndCreateTable<Can_use_nested_transactions_Table1>();
                outerConn.DropAndCreateTable<Can_use_nested_transactions_Table2>();

                //using(var innerConn = OpenDbConnection()) {//use the factory to get the connections
                using(var innerConn = factory.OpenDbConnection()) {

                    using(var outerTran = outerConn.OpenTransaction()) {
                        outerConn.Insert(new Can_use_nested_transactions_Table1 { Dummy = DateTime.Now });

                        using(var innerTran = innerConn.OpenTransaction()) {
                            //The other transaction inserts into table1, Table2 is not locked
                            innerConn.Insert(new Can_use_nested_transactions_Table2 { Dummy = DateTime.Now });

                            //fails here, because innerTran has overwritten the ThreadStatic OrmLiteConfig.CurrentTransaction
                            outerConn.Insert(new Can_use_nested_transactions_Table1 { Dummy = DateTime.Now });

                            outerConn.Insert(new Can_use_nested_transactions_Table1 { Dummy = DateTime.Now });
                        }
                    }
                }
            }
        }

        /// <summary>two separate tables, so they are not locked.</summary>
        public class Can_use_nested_transactions_Table1
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement]
            public int Id { get; set; }

            /// <summary>Gets or sets the Date/Time of the dummy.</summary>
            /// <value>The dummy.</value>
            public DateTime Dummy { get; set; }
        }

        /// <summary>A can use nested transactions table 2.</summary>
        public class Can_use_nested_transactions_Table2
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement]
            public int Id { get; set; }

            /// <summary>Gets or sets the Date/Time of the dummy.</summary>
            /// <value>The dummy.</value>
            public DateTime Dummy { get; set; }
        }
    }
}
