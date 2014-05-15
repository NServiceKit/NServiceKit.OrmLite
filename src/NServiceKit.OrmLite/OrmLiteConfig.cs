//
// ServiceStack.OrmLite: Light-weight POCO ORM for .NET and Mono
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2010 Liquidbit Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

using System;
using System.Collections.Generic;
using System.Data;

namespace NServiceKit.OrmLite
{
    /// <summary>An ORM lite configuration.</summary>
    public static class OrmLiteConfig
    {
        /// <summary>The identifier field.</summary>
        public const string IdField = "Id";

        /// <summary>The default command timeout.</summary>
        private const int defaultCommandTimeout = 30;

        /// <summary>The command timeout.</summary>
        private static int? commandTimeout;

        /// <summary>The ts command timeout.</summary>
        [ThreadStatic]
        public static int? TSCommandTimeout;

        /// <summary>Gets or sets the command timeout.</summary>
        /// <value>The command timeout.</value>
        public static int CommandTimeout
        {
            get
            {
                if (TSCommandTimeout != null)
                    return TSCommandTimeout.Value;
                if (commandTimeout != null)
                    return commandTimeout.Value;
                return defaultCommandTimeout;
            }
            set
            {
                commandTimeout = value;
            }
        }

        /// <summary>The ts dialect provider.</summary>
        [ThreadStatic]
        public static IOrmLiteDialectProvider TSDialectProvider;

        /// <summary>The ts transaction.</summary>
        [ThreadStatic]
        public static IDbTransaction TSTransaction;

        /// <summary>The dialect provider.</summary>
        private static IOrmLiteDialectProvider dialectProvider;

        /// <summary>Gets the dialect provider.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <value>The dialect provider.</value>
        public static IOrmLiteDialectProvider DialectProvider
        {
            get
            {
                if (dialectProvider == null)
                {
                    throw new ArgumentNullException("DialectProvider",
                        "You must set the singleton 'OrmLiteConfig.DialectProvider' to use the OrmLiteWriteExtensions");
                }
                return TSDialectProvider ?? dialectProvider;
            }
            set
            {
                dialectProvider = value;
            }
        }

        /// <summary>
        /// A string extension method that converts this object to a database connection.
        /// </summary>
        /// <param name="dbConnectionStringOrFilePath">The dbConnectionStringOrFilePath to act on.</param>
        /// <returns>The given data converted to an IDbConnection.</returns>
        public static IDbConnection ToDbConnection(this string dbConnectionStringOrFilePath)
        {
            return dbConnectionStringOrFilePath.ToDbConnection(DialectProvider);
        }

        /// <summary>A string extension method that opens database connection.</summary>
        /// <param name="dbConnectionStringOrFilePath">The dbConnectionStringOrFilePath to act on.</param>
        /// <returns>An IDbConnection.</returns>
        public static IDbConnection OpenDbConnection(this string dbConnectionStringOrFilePath)
        {
            var sqlConn = dbConnectionStringOrFilePath.ToDbConnection(DialectProvider);
            sqlConn.Open();
            return sqlConn;
        }

        /// <summary>A string extension method that opens read only database connection.</summary>
        /// <param name="dbConnectionStringOrFilePath">The dbConnectionStringOrFilePath to act on.</param>
        /// <returns>An IDbConnection.</returns>
        public static IDbConnection OpenReadOnlyDbConnection(this string dbConnectionStringOrFilePath)
        {
            var options = new Dictionary<string, string> { { "Read Only", "True" } };

            var dbConn = DialectProvider.CreateConnection(dbConnectionStringOrFilePath, options);
            dbConn.Open();
            return dbConn;
        }

        /// <summary>Clears the cache.</summary>
        public static void ClearCache()
        {
            OrmLiteConfigExtensions.ClearCache();
        }

        /// <summary>
        /// A string extension method that converts this object to a database connection.
        /// </summary>
        /// <param name="dbConnectionStringOrFilePath">The dbConnectionStringOrFilePath to act on.</param>
        /// <param name="dialectProvider">             The dialect provider.</param>
        /// <returns>The given data converted to an IDbConnection.</returns>
        public static IDbConnection ToDbConnection(this string dbConnectionStringOrFilePath, IOrmLiteDialectProvider dialectProvider)
        {
            var dbConn = dialectProvider.CreateConnection(dbConnectionStringOrFilePath, options: null);
            return dbConn;
        }
    }
}