using System;
using System.Data;
using System.Linq.Expressions;

namespace NServiceKit.OrmLite
{
    /// <summary>A write connection extensions.</summary>
    public static class WriteConnectionExtensions
    {
        /// <summary>An IDbConnection extension method that updates the only.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="model">     The model.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <returns>An int.</returns>
        public static int UpdateOnly<T>(this IDbConnection dbConn, T model, Func<SqlExpressionVisitor<T>, SqlExpressionVisitor<T>> onlyFields)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnly(model, onlyFields));
        }

        /// <summary>An IDbConnection extension method that updates the only.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="model">     The model.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <returns>An int.</returns>
        public static int UpdateOnly<T>(this IDbConnection dbConn, T model, SqlExpressionVisitor<T> onlyFields)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnly(model, onlyFields));
        }

        /// <summary>An IDbConnection extension method that updates the only.</summary>
        /// <typeparam name="T">   Generic type parameter.</typeparam>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="obj">       The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        /// <param name="where">     The where.</param>
        /// <returns>An int.</returns>
        public static int UpdateOnly<T, TKey>(this IDbConnection dbConn, T obj,
            Expression<Func<T, TKey>> onlyFields = null,
            Expression<Func<T, bool>> where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnly(obj, onlyFields, where));
        }

        /// <summary>An IDbConnection extension method that updates the non defaults.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="item">  The item.</param>
        /// <param name="where"> The where.</param>
        /// <returns>An int.</returns>
        public static int UpdateNonDefaults<T>(this IDbConnection dbConn, T item, Expression<Func<T, bool>> where)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateNonDefaults(item, where));
        }

        /// <summary>An IDbConnection extension method that updates this object.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="item">  The item.</param>
        /// <param name="where"> The where.</param>
        /// <returns>An int.</returns>
        public static int Update<T>(this IDbConnection dbConn, T item, Expression<Func<T, bool>> where)
        {
            return dbConn.Exec(dbCmd => dbCmd.Update(item, where));
        }

        /// <summary>An IDbConnection extension method that updates this object.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="updateOnly">The update only.</param>
        /// <param name="where">     The where.</param>
        /// <returns>An int.</returns>
        public static int Update<T>(this IDbConnection dbConn, object updateOnly, Expression<Func<T, bool>> where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Update(updateOnly, where));
        }

        /// <summary>An IDbConnection extension method that updates this object.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="set">   The set.</param>
        /// <param name="where"> The where.</param>
        /// <returns>An int.</returns>
        public static int Update<T>(this IDbConnection dbConn, string set = null, string where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Update<T>(set, where));
        }

        /// <summary>An IDbConnection extension method that updates this object.</summary>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="table"> The table.</param>
        /// <param name="set">   The set.</param>
        /// <param name="where"> The where.</param>
        /// <returns>An int.</returns>
        public static int Update(this IDbConnection dbConn, string table = null, string set = null, string where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Update(table, set, where));
        }

        /// <summary>An IDbConnection extension method that inserts an only.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="obj">       The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        public static void InsertOnly<T>(this IDbConnection dbConn, T obj, Func<SqlExpressionVisitor<T>, SqlExpressionVisitor<T>> onlyFields)
        {
            dbConn.Exec(dbCmd => dbCmd.InsertOnly(obj, onlyFields));
        }

        /// <summary>An IDbConnection extension method that inserts an only.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="obj">       The object.</param>
        /// <param name="onlyFields">The only fields.</param>
        public static void InsertOnly<T>(this IDbConnection dbConn, T obj, SqlExpressionVisitor<T> onlyFields)
        {
            dbConn.Exec(dbCmd => dbCmd.InsertOnly(obj, onlyFields));
        }

        /// <summary>An IDbConnection extension method that deletes this object.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="where"> The where.</param>
        /// <returns>An int.</returns>
        public static int Delete<T>(this IDbConnection dbConn, Expression<Func<T, bool>> where)
        {
            return dbConn.Exec(dbCmd => dbCmd.Delete(where));
        }

        /// <summary>An IDbConnection extension method that deletes this object.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="where"> The where.</param>
        /// <returns>An int.</returns>
        public static int Delete<T>(this IDbConnection dbConn, Func<SqlExpressionVisitor<T>, SqlExpressionVisitor<T>> where)
        {
            return dbConn.Exec(dbCmd => dbCmd.Delete(where));
        }

        /// <summary>An IDbConnection extension method that deletes this object.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="where"> The where.</param>
        /// <returns>An int.</returns>
        public static int Delete<T>(this IDbConnection dbConn, SqlExpressionVisitor<T> where)
        {
            return dbConn.Exec(dbCmd => dbCmd.Delete(where));
        }

        /// <summary>An IDbConnection extension method that deletes this object.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="where"> The where.</param>
        /// <returns>An int.</returns>
        public static int Delete<T>(this IDbConnection dbConn, string where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Delete<T>(where));
        }

        /// <summary>An IDbConnection extension method that deletes this object.</summary>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="table"> The table.</param>
        /// <param name="where"> The where.</param>
        /// <returns>An int.</returns>
        public static int Delete(this IDbConnection dbConn, string table = null, string where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Delete(table, where));
        }         
    }
}