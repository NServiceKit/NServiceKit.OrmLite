using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace NServiceKit.OrmLite
{
    /// <summary>An ORM lite read connection extensions.</summary>
    public static class OrmLiteReadConnectionExtensions
    {
        /// <summary>An IDbConnection extension method that selects.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <returns>A List&lt;TModel&gt;</returns>
        public static List<T> Select<T>(this IDbConnection dbConn) 
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<T>());
        }

        /// <summary>An IDbConnection extension method that selects.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">      The dbConn to act on.</param>
        /// <param name="sqlFilter">   A filter specifying the SQL.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>A List&lt;TModel&gt;</returns>
        public static List<T> Select<T>(this IDbConnection dbConn, string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<T>(sqlFilter, filterParams));
        }

        /// <summary>An IDbConnection extension method that selects.</summary>
        /// <typeparam name="TModel">Type of the model.</typeparam>
        /// <param name="dbConn">       The dbConn to act on.</param>
        /// <param name="fromTableType">Type of from table.</param>
        /// <returns>A List&lt;TModel&gt;</returns>
        public static List<TModel> Select<TModel>(this IDbConnection dbConn, Type fromTableType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<TModel>(fromTableType));
        }

        /// <summary>An IDbConnection extension method that selects.</summary>
        /// <typeparam name="TModel">Type of the model.</typeparam>
        /// <param name="dbConn">       The dbConn to act on.</param>
        /// <param name="fromTableType">Type of from table.</param>
        /// <param name="sqlFilter">    A filter specifying the SQL.</param>
        /// <param name="filterParams"> Options for controlling the filter.</param>
        /// <returns>A List&lt;TModel&gt;</returns>
        public static List<TModel> Select<TModel>(this IDbConnection dbConn, Type fromTableType, string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<TModel>(fromTableType, sqlFilter, filterParams));
        }

        /// <summary>Enumerates each in this collection.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process each in this collection.
        /// </returns>
        public static IEnumerable<T> Each<T>(this IDbConnection dbConn)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.Each<T>(null));
        }

        /// <summary>Enumerates each in this collection.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">      The dbConn to act on.</param>
        /// <param name="filter">      Specifies the filter.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process each in this collection.
        /// </returns>
        public static IEnumerable<T> Each<T>(this IDbConnection dbConn, string filter, params object[] filterParams)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.Each<T>(filter, filterParams));
        }

        /// <summary>An IDbConnection extension method that firsts.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">      The dbConn to act on.</param>
        /// <param name="filter">      Specifies the filter.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>A T.</returns>
        public static T First<T>(this IDbConnection dbConn, string filter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.First<T>(filter, filterParams));
        }

        /// <summary>Alias for First.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">      The dbConn to act on.</param>
        /// <param name="filter">      Specifies the filter.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>A T.</returns>
        public static T Single<T>(this IDbConnection dbConn, string filter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.First<T>(filter, filterParams));
        }

        /// <summary>An IDbConnection extension method that firsts.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="filter">Specifies the filter.</param>
        /// <returns>A T.</returns>
        public static T First<T>(this IDbConnection dbConn, string filter)
        {
            return dbConn.Exec(dbCmd => dbCmd.First<T>(filter));
        }

        /// <summary>Alias for First.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="filter">Specifies the filter.</param>
        /// <returns>A T.</returns>
        public static T Single<T>(this IDbConnection dbConn, string filter)
        {
            return dbConn.Exec(dbCmd => dbCmd.First<T>(filter));
        }

        /// <summary>An IDbConnection extension method that first or default.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">      The dbConn to act on.</param>
        /// <param name="filter">      Specifies the filter.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>A T.</returns>
        public static T FirstOrDefault<T>(this IDbConnection dbConn, string filter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.FirstOrDefault<T>(filter, filterParams));
        }

        /// <summary>Alias for FirstOrDefault.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">      The dbConn to act on.</param>
        /// <param name="filter">      Specifies the filter.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>A T.</returns>
        public static T SingleOrDefault<T>(this IDbConnection dbConn, string filter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.FirstOrDefault<T>(filter, filterParams));
        }

        /// <summary>An IDbConnection extension method that first or default.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="filter">Specifies the filter.</param>
        /// <returns>A T.</returns>
        public static T FirstOrDefault<T>(this IDbConnection dbConn, string filter)
        {
            return dbConn.Exec(dbCmd => dbCmd.FirstOrDefault<T>(filter));
        }

        /// <summary>Alias for FirstOrDefault.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="filter">Specifies the filter.</param>
        /// <returns>A T.</returns>
        public static T SingleOrDefault<T>(this IDbConnection dbConn, string filter)
        {
            return dbConn.Exec(dbCmd => dbCmd.FirstOrDefault<T>(filter));
        }

        /// <summary>An IDbConnection extension method that gets by identifier.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn"> The dbConn to act on.</param>
        /// <param name="idValue">The identifier value.</param>
        /// <returns>The by identifier.</returns>
        public static T GetById<T>(this IDbConnection dbConn, object idValue)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetById<T>(idValue));
        }

        /// <summary>
        /// Performs an GetById() except argument is passed as a parameter to the generated SQL.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn"> The dbConn to act on.</param>
        /// <param name="idValue">The identifier value.</param>
        /// <returns>The by identifier parameter.</returns>
        public static T GetByIdParam<T>(this IDbConnection dbConn, object idValue)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetByIdParam<T>(idValue));
        }

        /// <summary>Alias for GetById.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn"> The dbConn to act on.</param>
        /// <param name="idValue">The identifier value.</param>
        /// <returns>A T.</returns>
        public static T Id<T>(this IDbConnection dbConn, object idValue)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetById<T>(idValue));
        }

        /// <summary>An IDbConnection extension method that queries by identifier.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="value"> The value.</param>
        /// <returns>The by identifier.</returns>
        public static T QueryById<T>(this IDbConnection dbConn, object value) 
        {
            return dbConn.Exec(dbCmd => dbCmd.QueryById<T>(value));
        }

        /// <summary>An IDbConnection extension method that single where.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="name">  The name.</param>
        /// <param name="value"> The value.</param>
        /// <returns>A T.</returns>
        public static T SingleWhere<T>(this IDbConnection dbConn, string name, object value)
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleWhere<T>(name, value));
        }

        /// <summary>An IDbConnection extension method that queries a single.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>The single.</returns>
        public static T QuerySingle<T>(this IDbConnection dbConn, object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.QuerySingle<T>(anonType));
        }

        /// <summary>An IDbConnection extension method that queries a single.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>The single.</returns>
        public static T QuerySingle<T>(this IDbConnection dbConn, string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.QuerySingle<T>(sql, anonType));
        }

        /// <summary>An IDbConnection extension method that wheres.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="name">  The name.</param>
        /// <param name="value"> The value.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> Where<T>(this IDbConnection dbConn, string name, object value)
        {
            return dbConn.Exec(dbCmd => dbCmd.Where<T>(name, value));
        }

        /// <summary>An IDbConnection extension method that wheres.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> Where<T>(this IDbConnection dbConn, object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Where<T>(anonType));
        }

        /// <summary>An IDbConnection extension method that wheres.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="Predicate">The predicate.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> Where<T>(this IDbConnection dbConn, System.Linq.Expressions.Expression<Func<T,bool>> Predicate)
        {
            return (List<T>) dbConn.Select<T>(Predicate);
        }

        /// <summary>An IDbConnection extension method that queries.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="sql">   The SQL.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> Query<T>(this IDbConnection dbConn, string sql)
        {
            return dbConn.Exec(dbCmd => dbCmd.Query<T>(sql));
        }

        /// <summary>An IDbConnection extension method that queries.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> Query<T>(this IDbConnection dbConn, string sql, object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Query<T>(sql, anonType));
        }

        /// <summary>An IDbConnection extension method that queries.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="sql">   The SQL.</param>
        /// <param name="dict">  The dictionary.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> Query<T>(this IDbConnection dbConn, string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.Query<T>(sql, dict));
        }

        /// <summary>An IDbConnection extension method that executes the non query operation.</summary>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="sql">   The SQL.</param>
        /// <returns>An int.</returns>
        public static int ExecuteNonQuery(this IDbConnection dbConn, string sql)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecuteNonQuery(sql));
        }

        /// <summary>An IDbConnection extension method that executes the non query operation.</summary>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>An int.</returns>
        public static int ExecuteNonQuery(this IDbConnection dbConn, string sql, object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecuteNonQuery(sql, anonType));
        }

        /// <summary>
        /// An IDbConnection extension method that executes the non query operation.
        /// </summary>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="sql">   The SQL.</param>
        /// <param name="dict">  The dictionary.</param>
        /// <returns>An int.</returns>
        public static int ExecuteNonQuery(this IDbConnection dbConn, string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecuteNonQuery(sql, dict));
        }

        /// <summary>An IDbConnection extension method that queries a scalar.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>The scalar.</returns>
        public static T QueryScalar<T>(this IDbConnection dbConn, object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.QueryScalar<T>(anonType));
        }

        /// <summary>An IDbConnection extension method that queries a scalar.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>The scalar.</returns>
        public static T QueryScalar<T>(this IDbConnection dbConn, string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.QueryScalar<T>(sql, anonType));
        }

        /// <summary>An IDbConnection extension method that SQL list.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="sql">   The SQL.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> SqlList<T>(this IDbConnection dbConn, string sql)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(sql));
        }

        /// <summary>An IDbConnection extension method that SQL list.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> SqlList<T>(this IDbConnection dbConn, string sql, object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(sql, anonType));
        }

        /// <summary>An IDbConnection extension method that SQL list.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="sql">   The SQL.</param>
        /// <param name="dict">  The dictionary.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> SqlList<T>(this IDbConnection dbConn, string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(sql, dict));
        }

        /// <summary>An IDbConnection extension method that SQL scalar.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>A T.</returns>
        public static T SqlScalar<T>(this IDbConnection dbConn, string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalar<T>(sql, anonType));
        }

        /// <summary>An IDbConnection extension method that SQL scalar.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="sql">   The SQL.</param>
        /// <param name="dict">  The dictionary.</param>
        /// <returns>A T.</returns>
        public static T SqlScalar<T>(this IDbConnection dbConn, string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalar<T>(sql, dict));
        }

        /// <summary>An IDbConnection extension method that by example where.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> ByExampleWhere<T>(this IDbConnection dbConn, object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.ByExampleWhere<T>(anonType));
        }

        /// <summary>An IDbConnection extension method that by example where.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">      The dbConn to act on.</param>
        /// <param name="anonType">    Type of the anon.</param>
        /// <param name="excludeNulls">true to exclude, false to include the nulls.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> ByExampleWhere<T>(this IDbConnection dbConn, object anonType, bool excludeNulls)
        {
            return dbConn.Exec(dbCmd => dbCmd.ByExampleWhere<T>(anonType, excludeNulls));
        }

        /// <summary>An IDbConnection extension method that queries by example.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>The by example.</returns>
        public static List<T> QueryByExample<T>(this IDbConnection dbConn, string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.QueryByExample<T>(sql, anonType));
        }

        /// <summary>Enumerates query each in this collection.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process query each in this collection.
        /// </returns>
        public static IEnumerable<T> QueryEach<T>(this IDbConnection dbConn, string sql, object anonType = null)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.QueryEach<T>(sql, anonType));
        }

        /// <summary>Enumerates each where in this collection.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process each where in this collection.
        /// </returns>
        public static IEnumerable<T> EachWhere<T>(this IDbConnection dbConn, object anonType)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.EachWhere<T>(anonType));
        }

        /// <summary>An IDbConnection extension method that gets by identifier or default.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn"> The dbConn to act on.</param>
        /// <param name="idValue">The identifier value.</param>
        /// <returns>The by identifier or default.</returns>
        public static T GetByIdOrDefault<T>(this IDbConnection dbConn, object idValue)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetByIdOrDefault<T>(idValue));
        }

        /// <summary>Alias for GetByIds.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn"> The dbConn to act on.</param>
        /// <param name="idValue">The identifier value.</param>
        /// <returns>A T.</returns>
        public static T IdOrDefault<T>(this IDbConnection dbConn, object idValue)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetByIdOrDefault<T>(idValue));
        }

        /// <summary>An IDbConnection extension method that gets by identifiers.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="idValues">The identifier values.</param>
        /// <returns>The by identifiers.</returns>
        public static List<T> GetByIds<T>(this IDbConnection dbConn, IEnumerable idValues)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetByIds<T>(idValues));
        }

        /// <summary>Alias for GetByIds.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="idValues">The identifier values.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> Ids<T>(this IDbConnection dbConn, IEnumerable idValues)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetByIds<T>(idValues));
        }

        /// <summary>An IDbConnection extension method that gets a scalar.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The scalar.</returns>
        public static T GetScalar<T>(this IDbConnection dbConn, string sql, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetScalar<T>(sql, sqlParams));
        }

        /// <summary>Alias for Scalar.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>A T.</returns>
        public static T Scalar<T>(this IDbConnection dbConn, string sql, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetScalar<T>(sql, sqlParams));
        }

        /// <summary>
        /// An IDbConnection extension method that gets the last insert identifier.
        /// </summary>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <returns>The last insert identifier.</returns>
        public static long GetLastInsertId(this IDbConnection dbConn)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetLastInsertId());
        }

        /// <summary>An IDbConnection extension method that gets the first column.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The first column.</returns>
        public static List<T> GetFirstColumn<T>(this IDbConnection dbConn, string sql, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetFirstColumn<T>(sql, sqlParams));
        }

        /// <summary>An IDbConnection extension method that gets a list.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The list.</returns>
        public static List<T> GetList<T>(this IDbConnection dbConn, string sql, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetList<T>(sql, sqlParams));
        }

        /// <summary>Alias for GetList.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> List<T>(this IDbConnection dbConn, string sql, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetList<T>(sql, sqlParams));
        }

        /// <summary>An IDbConnection extension method that gets the first column distinct.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The first column distinct.</returns>
        public static HashSet<T> GetFirstColumnDistinct<T>(this IDbConnection dbConn, string sql, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetFirstColumnDistinct<T>(sql, sqlParams));
        }

        /// <summary>An IDbConnection extension method that gets hash set.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The hash set.</returns>
        public static HashSet<T> GetHashSet<T>(this IDbConnection dbConn, string sql, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetHashSet<T>(sql, sqlParams));
        }

        /// <summary>Alias for GetHashSet.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>A HashSet&lt;T&gt;</returns>
        public static HashSet<T> HashSet<T>(this IDbConnection dbConn, string sql, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetHashSet<T>(sql, sqlParams));
        }

        /// <summary>An IDbConnection extension method that gets a lookup.</summary>
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The lookup.</returns>
        public static Dictionary<K, List<V>> GetLookup<K, V>(this IDbConnection dbConn, string sql, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetLookup<K, V>(sql, sqlParams));
        }

        /// <summary>Alias for GetLookup.</summary>
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>A list of.</returns>
        public static Dictionary<K, List<V>> Lookup<K, V>(this IDbConnection dbConn, string sql, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetLookup<K, V>(sql, sqlParams));
        }

        /// <summary>An IDbConnection extension method that gets a dictionary.</summary>
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The dictionary.</returns>
        public static Dictionary<K, V> GetDictionary<K, V>(this IDbConnection dbConn, string sql, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetDictionary<K, V>(sql, sqlParams));
        }

        /// <summary>Alias for GetDictionary.</summary>
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>A Dictionary&lt;K,V&gt;</returns>
        public static Dictionary<K, V> Dictionary<K, V>(this IDbConnection dbConn, string sql, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetDictionary<K, V>(sql, sqlParams));
        }

        /// <summary>somo aditional methods.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="record">The record.</param>
        /// <returns>true if children, false if not.</returns>
        public static bool HasChildren<T>(this IDbConnection dbConn, object record)
        {
            return dbConn.Exec(dbCmd => dbCmd.HasChildren<T>(record));
        }

        /// <summary>An IDbConnection extension method that exists.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">      The dbConn to act on.</param>
        /// <param name="sqlFilter">   A filter specifying the SQL.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool Exists<T>(this IDbConnection dbConn, string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.Exists<T>(sqlFilter, filterParams));
        }

        /// <summary>An IDbConnection extension method that exists.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="record">The record.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool Exists<T>(this IDbConnection dbConn, object record)
        {
            return dbConn.Exec(dbCmd => dbCmd.Exists<T>(record));
        }

        /// <summary>procedures ...</summary>
        /// <typeparam name="TOutputModel">Type of the output model.</typeparam>
        /// <param name="dbConn">               The dbConn to act on.</param>
        /// <param name="fromObjWithProperties">from object with properties.</param>
        /// <returns>A List&lt;TOutputModel&gt;</returns>
        public static List<TOutputModel> SelectFromProcedure<TOutputModel>(this IDbConnection dbConn,
            object fromObjWithProperties)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectFromProcedure<TOutputModel>(fromObjWithProperties));
        }

        /// <summary>An IDbConnection extension method that select from procedure.</summary>
        /// <typeparam name="TOutputModel">Type of the output model.</typeparam>
        /// <param name="dbConn">               The dbConn to act on.</param>
        /// <param name="fromObjWithProperties">from object with properties.</param>
        /// <param name="sqlFilter">            A filter specifying the SQL.</param>
        /// <param name="filterParams">         Options for controlling the filter.</param>
        /// <returns>A List&lt;TOutputModel&gt;</returns>
        public static List<TOutputModel> SelectFromProcedure<TOutputModel>(this IDbConnection dbConn,
            object fromObjWithProperties,
            string sqlFilter,
            params object[] filterParams)
            where TOutputModel : new()
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectFromProcedure<TOutputModel>(
                fromObjWithProperties, sqlFilter, filterParams));
        }

        /// <summary>An IDbConnection extension method that gets long scalar.</summary>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <returns>The long scalar.</returns>
        public static long GetLongScalar(this IDbConnection dbConn)
        {
            return dbConn.Exec(dbCmd => dbCmd.GetLongScalar());
        }			
    }
}
