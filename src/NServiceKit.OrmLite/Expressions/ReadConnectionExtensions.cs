using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace NServiceKit.OrmLite
{
    /// <summary>A read connection extensions.</summary>
    public static class ReadConnectionExtensions
    {
        /// <summary>The last command text.</summary>
        [ThreadStatic]
        internal static string LastCommandText;

        /// <summary>Creates the expression.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <returns>The new expression.</returns>
        public static SqlExpressionVisitor<T> CreateExpression<T>(this IDbConnection dbConn) 
        {
            return dbConn.GetDialectProvider().ExpressionVisitor<T>();
        }

        /// <summary>An IDbConnection extension method that execs.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="filter">Specifies the filter.</param>
        /// <returns>A T.</returns>
        public static T Exec<T>(this IDbConnection dbConn, Func<IDbCommand, T> filter)
        {
            var holdProvider = OrmLiteConfig.TSDialectProvider;
            try
            {
                var ormLiteDbConn = dbConn as OrmLiteConnection;
                if (ormLiteDbConn != null)
                    OrmLiteConfig.TSDialectProvider = ormLiteDbConn.Factory.DialectProvider;

                using (var dbCmd = dbConn.CreateCommand())
                {
                    dbCmd.Transaction = (ormLiteDbConn != null) ? ormLiteDbConn.Transaction : OrmLiteConfig.TSTransaction;
                    dbCmd.CommandTimeout = OrmLiteConfig.CommandTimeout;
                    var ret = filter(dbCmd);
                    LastCommandText = dbCmd.CommandText;
                    return ret;
                }
            }
            finally
            {
                OrmLiteConfig.TSDialectProvider = holdProvider;
            }
        }

        /// <summary>An IDbConnection extension method that execs.</summary>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="filter">Specifies the filter.</param>
        public static void Exec(this IDbConnection dbConn, Action<IDbCommand> filter)
        {
            var dialectProvider = OrmLiteConfig.DialectProvider;
            try
            {
                var ormLiteDbConn = dbConn as OrmLiteConnection;
                if (ormLiteDbConn != null)
                    OrmLiteConfig.DialectProvider = ormLiteDbConn.Factory.DialectProvider;

                using (var dbCmd = dbConn.CreateCommand())
                {
                    dbCmd.Transaction = (ormLiteDbConn != null) ? ormLiteDbConn.Transaction : OrmLiteConfig.TSTransaction;
                    dbCmd.CommandTimeout = OrmLiteConfig.CommandTimeout;

                    filter(dbCmd);
                    LastCommandText = dbCmd.CommandText;
                }
            }
            finally
            {
                OrmLiteConfig.DialectProvider = dialectProvider;
            }
        }

        /// <summary>Enumerates execute lazy in this collection.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="filter">Specifies the filter.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process execute lazy in this collection.
        /// </returns>
        public static IEnumerable<T> ExecLazy<T>(this IDbConnection dbConn, Func<IDbCommand, IEnumerable<T>> filter)
        {
            var dialectProvider = OrmLiteConfig.DialectProvider;
            try
            {
                var ormLiteDbConn = dbConn as OrmLiteConnection;
                if (ormLiteDbConn != null)
                    OrmLiteConfig.DialectProvider = ormLiteDbConn.Factory.DialectProvider;

                using (var dbCmd = dbConn.CreateCommand())
                {
                    dbCmd.Transaction = (ormLiteDbConn != null) ? ormLiteDbConn.Transaction : OrmLiteConfig.TSTransaction;
                    dbCmd.CommandTimeout = OrmLiteConfig.CommandTimeout;

                    var results = filter(dbCmd);
                    LastCommandText = dbCmd.CommandText;
                    foreach (var item in results)
                    {
                        yield return item;
                    }
                }
            }
            finally
            {
                OrmLiteConfig.DialectProvider = dialectProvider;
            }
        }

        /// <summary>An IDbConnection extension method that opens a transaction.</summary>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <returns>An IDbTransaction.</returns>
        public static IDbTransaction OpenTransaction(this IDbConnection dbConn)
        {
            return new OrmLiteTransaction(dbConn, dbConn.BeginTransaction());
        }

        /// <summary>An IDbConnection extension method that opens a transaction.</summary>
        /// <param name="dbConn">        The dbConn to act on.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>An IDbTransaction.</returns>
        public static IDbTransaction OpenTransaction(this IDbConnection dbConn, IsolationLevel isolationLevel)
        {
            return new OrmLiteTransaction(dbConn, dbConn.BeginTransaction(isolationLevel));
        }

        /// <summary>An IDbConnection extension method that gets dialect provider.</summary>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <returns>The dialect provider.</returns>
        public static IOrmLiteDialectProvider GetDialectProvider(this IDbConnection dbConn)
        {
            var ormLiteDbConn = dbConn as OrmLiteConnection;
            return ormLiteDbConn != null 
                ? ormLiteDbConn.Factory.DialectProvider 
                : OrmLiteConfig.DialectProvider;
        }

        /// <summary>Creates the expression.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>The new expression.</returns>
        public static SqlExpressionVisitor<T> CreateExpression<T>()
        {
            return OrmLiteConfig.DialectProvider.ExpressionVisitor<T>();
        }

        /// <summary>An IDbConnection extension method that selects.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> Select<T>(this IDbConnection dbConn, Expression<Func<T, bool>> predicate)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select(predicate));
        }

        /// <summary>An IDbConnection extension method that selects.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> Select<T>(this IDbConnection dbConn, Func<SqlExpressionVisitor<T>, SqlExpressionVisitor<T>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select(expression));
        }

        /// <summary>An IDbConnection extension method that selects.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> Select<T>(this IDbConnection dbConn, SqlExpressionVisitor<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select(expression));
        }

        /// <summary>
        /// Performs the same function as Select() except arguments are passed as parameters to the
        /// generated SQL. Currently does not support complex SQL.## ,  .StartsWith(), EndsWith() and
        /// Contains() operators.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> SelectParam<T>(this IDbConnection dbConn, Expression<Func<T, bool>> predicate)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectParam(predicate));
        }

        /// <summary>An IDbConnection extension method that firsts.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A T.</returns>
        public static T First<T>(this IDbConnection dbConn, Expression<Func<T, bool>> predicate)
        {
            return dbConn.Exec(dbCmd => dbCmd.First(predicate));
        }

        /// <summary>An IDbConnection extension method that firsts.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A T.</returns>
        public static T First<T>(this IDbConnection dbConn, SqlExpressionVisitor<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.First(expression));
        }

        /// <summary>An IDbConnection extension method that first or default.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A T.</returns>
        public static T FirstOrDefault<T>(this IDbConnection dbConn, Expression<Func<T, bool>> predicate)
        {
            return dbConn.Exec(dbCmd => dbCmd.FirstOrDefault(predicate));
        }

        /// <summary>An IDbConnection extension method that first or default.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A T.</returns>
        public static T FirstOrDefault<T>(this IDbConnection dbConn, SqlExpressionVisitor<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.FirstOrDefault(expression));
        }

        /// <summary>An IDbConnection extension method that gets a scalar.</summary>
        /// <typeparam name="T">   Generic type parameter.</typeparam>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="field"> The field.</param>
        /// <returns>The scalar.</returns>
        public static TKey GetScalar<T, TKey>(this IDbConnection dbConn, Expression<Func<T, TKey>> field)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetScalar(field));
        }

        /// <summary>An IDbConnection extension method that gets a scalar.</summary>
        /// <typeparam name="T">   Generic type parameter.</typeparam>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="field">    The field.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The scalar.</returns>
        public static TKey GetScalar<T, TKey>(this IDbConnection dbConn, Expression<Func<T, TKey>> field,
                                             Expression<Func<T, bool>> predicate)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetScalar(field, predicate));
        }

        /// <summary>
        /// An IDbConnection extension method that counts the given database connection.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A long.</returns>
        public static long Count<T>(this IDbConnection dbConn, SqlExpressionVisitor<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Count(expression));
        }

        /// <summary>
        /// An IDbConnection extension method that counts the given database connection.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A long.</returns>
        public static long Count<T>(this IDbConnection dbConn, Expression<Func<T, bool>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Count(expression));
        }

        /// <summary>
        /// An IDbConnection extension method that counts the given database connection.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <returns>A long.</returns>
        public static long Count<T>(this IDbConnection dbConn)
        {
            SqlExpressionVisitor<T> expression = OrmLiteConfig.DialectProvider.ExpressionVisitor<T>();
            return dbConn.Exec(dbCmd => dbCmd.Count(expression));
        }
    }
}