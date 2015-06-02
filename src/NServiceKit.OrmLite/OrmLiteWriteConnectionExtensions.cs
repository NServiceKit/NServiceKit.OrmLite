using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace NServiceKit.OrmLite
{
    /// <summary>An ORM lite write connection extensions.</summary>
    public static class OrmLiteWriteConnectionExtensions
    {
        /// <summary>
        /// An IDbConnection extension method that queries if a given table exists.
        /// </summary>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool TableExists(this IDbConnection dbConn, string tableName)
        {
            return dbConn.GetDialectProvider().DoesTableExist(dbConn, tableName);
        }

        /// <summary>An IDbConnection extension method that creates the tables.</summary>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="overwrite"> true to overwrite, false to preserve.</param>
        /// <param name="tableTypes">List of types of the tables.</param>
        public static void CreateTables(this IDbConnection dbConn, bool overwrite, params Type[] tableTypes)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTables(overwrite, tableTypes));
        }

        /// <summary>An IDbConnection extension method that creates table if not exists.</summary>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="tableTypes">List of types of the tables.</param>
        public static void CreateTableIfNotExists(this IDbConnection dbConn, params Type[] tableTypes)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTables(false, tableTypes));
        }

        /// <summary>An IDbConnection extension method that drop and create tables.</summary>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="tableTypes">List of types of the tables.</param>
        public static void DropAndCreateTables(this IDbConnection dbConn, params Type[] tableTypes)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTables(true, tableTypes));
        }

        /// <summary>Alias for CreateTableIfNotExists.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        public static void CreateTable<T>(this IDbConnection dbConn)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable<T>());
        }

        /// <summary>Alias for CreateTableIfNotExists.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="overwrite">true to overwrite, false to preserve.</param>
        public static void CreateTable<T>(this IDbConnection dbConn, bool overwrite)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable<T>(overwrite));
        }

        /// <summary>An IDbConnection extension method that creates table if not exists.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        public static void CreateTableIfNotExists<T>(this IDbConnection dbConn)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable<T>(false));
        }

        /// <summary>An IDbConnection extension method that drop and create table.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        public static void DropAndCreateTable<T>(this IDbConnection dbConn)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable<T>(true));
        }

        /// <summary>An IDbConnection extension method that creates a table.</summary>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="overwrite">true to overwrite, false to preserve.</param>
        /// <param name="modelType">Type of the model.</param>
        public static void CreateTable(this IDbConnection dbConn, bool overwrite, Type modelType)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable(overwrite, modelType));
        }

        /// <summary>An IDbConnection extension method that creates table if not exists.</summary>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="modelType">Type of the model.</param>
        public static void CreateTableIfNotExists(this IDbConnection dbConn, Type modelType)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable(false, modelType));
        }

        /// <summary>An IDbConnection extension method that drop and create table.</summary>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="modelType">Type of the model.</param>
        public static void DropAndCreateTable(this IDbConnection dbConn, Type modelType)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable(true, modelType));
        }

        /// <summary>An IDbConnection extension method that drop tables.</summary>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="tableTypes">List of types of the tables.</param>
        public static void DropTables(this IDbConnection dbConn, params Type[] tableTypes)
        {
            dbConn.Exec(dbCmd => dbCmd.DropTables(tableTypes));
        }

        /// <summary>An IDbConnection extension method that drop table.</summary>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="modelType">Type of the model.</param>
        public static void DropTable(this IDbConnection dbConn, Type modelType)
        {
            dbConn.Exec(dbCmd => dbCmd.DropTable(modelType));
        }

        /// <summary>An IDbConnection extension method that drop table.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        public static void DropTable<T>(this IDbConnection dbConn)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.DropTable<T>());
        }

        /// <summary>An IDbConnection extension method that gets the last SQL.</summary>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <returns>The last SQL.</returns>
        public static string GetLastSql(this IDbConnection dbConn)
        {
            return ReadConnectionExtensions.LastCommandText;
        }

        /// <summary>An IDbConnection extension method that executes the SQL operation.</summary>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="sql">   The SQL.</param>
        /// <returns>An int.</returns>
        public static int ExecuteSql(this IDbConnection dbConn, string sql)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecuteSql(sql));
        }

        /// <summary>An IDbConnection extension method that updates this object.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="objs">  The objects.</param>
        public static void Update<T>(this IDbConnection dbConn, params T[] objs)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.Update(objs));
        }

        /// <summary>An IDbConnection extension method that updates all.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="objs">  The objects.</param>
        public static void UpdateAll<T>(this IDbConnection dbConn, IEnumerable<T> objs)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.UpdateAll(objs));
        }

        /// <summary>
        /// Performs an Update&lt;T&gt;() except arguments are passed as parameters to the generated SQL.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="obj">   The object.</param>
        public static void UpdateParam<T>(this IDbConnection dbConn, T obj) where T : new()
        {
            dbConn.Exec(dbCmd =>
            {
                using (var updateStmt = dbConn.CreateUpdateStatement(obj))
                {
                    dbCmd.CopyParameterizedStatementTo(updateStmt);
                }
                dbCmd.ExecuteNonQuery();
            });
        }

        /// <summary>An IDbConnection extension method that deletes this object.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="objs">  The objects.</param>
        public static void Delete<T>(this IDbConnection dbConn, params T[] objs)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.Delete(objs));
        }

        /// <summary>An IDbConnection extension method that deletes all described by dbConn.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="objs">  The objects.</param>
        public static void DeleteAll<T>(this IDbConnection dbConn, IEnumerable<T> objs)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.DeleteAll(objs));
        }

        /// <summary>An IDbConnection extension method that deletes the by identifier.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="id">    The identifier.</param>
        public static void DeleteById<T>(this IDbConnection dbConn, object id)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.DeleteById<T>(id));
        }

        /// <summary>
        /// Performs a DeleteById() except argument is passed as a parameter to the generated SQL.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="id">    The identifier.</param>
        public static void DeleteByIdParam<T>(this IDbConnection dbConn, object id) where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.DeleteByIdParam<T>(id));
        }

        /// <summary>An IDbConnection extension method that deletes the by identifiers.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">  The dbConn to act on.</param>
        /// <param name="idValues">The identifier values.</param>
        public static void DeleteByIds<T>(this IDbConnection dbConn, IEnumerable idValues)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.DeleteByIds<T>(idValues));
        }

        /// <summary>
        /// An IDbConnection extension method that deletes all described by dbConn.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        public static void DeleteAll<T>(this IDbConnection dbConn)
        {
            dbConn.Exec(dbCmd => dbCmd.DeleteAll<T>());
        }

        /// <summary>An IDbConnection extension method that deletes all.</summary>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="tableType">Type of the table.</param>
        public static void DeleteAll(this IDbConnection dbConn, Type tableType)
        {
            dbConn.Exec(dbCmd => dbCmd.DeleteAll(tableType));
        }

        /// <summary>An IDbConnection extension method that deletes this object.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">      The dbConn to act on.</param>
        /// <param name="sqlFilter">   A filter specifying the SQL.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        public static void Delete<T>(this IDbConnection dbConn, string sqlFilter, params object[] filterParams)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.Delete<T>(sqlFilter, filterParams));
        }

        /// <summary>An IDbConnection extension method that deletes this object.</summary>
        /// <param name="dbConn">      The dbConn to act on.</param>
        /// <param name="tableType">   Type of the table.</param>
        /// <param name="sqlFilter">   A filter specifying the SQL.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        public static void Delete(this IDbConnection dbConn, Type tableType, string sqlFilter, params object[] filterParams)
        {
            dbConn.Exec(dbCmd => dbCmd.Delete(tableType, sqlFilter, filterParams));
        }

        /// <summary>An IDbConnection extension method that saves.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="obj">   The object.</param>
        public static void Save<T>(this IDbConnection dbConn, T obj)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.Save(obj));
        }

        /// <summary>An IDbConnection extension method that inserts.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="objs">  The objects.</param>
        public static void Insert<T>(this IDbConnection dbConn, params T[] objs)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.Insert(objs));
        }


        /// <summary>An IDbConnection extension method that inserts where not exists.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="obj">  The object.</param>
        /// <param name="wherePredicate"> predicate to create where clause</param>
        public static void InsertWhereNotExists<T>(this IDbConnection dbConn, T obj, Expression<Func<T, bool>> wherePredicate)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.InsertWhereNotExists(obj,wherePredicate));
        }

        /// <summary>An IDbConnection extension method that inserts all.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="objs">  The objects.</param>
        public static void InsertAll<T>(this IDbConnection dbConn, IEnumerable<T> objs)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.InsertAll(objs));
        }

        /// <summary>
        /// Performs an Insert() except arguments are passed as parameters to the generated SQL.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">        The dbConn to act on.</param>
        /// <param name="obj">           The object.</param>
        /// <param name="selectIdentity">true to select, false to deselect the identity.</param>
        /// <returns>A long.</returns>
        public static long InsertParam<T>(this IDbConnection dbConn, T obj, bool selectIdentity = false)
            where T : new()
        {
            return dbConn.Exec(dbCmd =>
            {
                using (var insertStmt = dbConn.CreateInsertStatement(obj))
                {
                    dbCmd.CopyParameterizedStatementTo(insertStmt);
                }

                if (selectIdentity)
                    return OrmLiteConfig.DialectProvider.InsertAndGetLastInsertId<T>(dbCmd);

                dbCmd.ExecuteNonQuery();
                return -1;
            });
        }

        /// <summary>
        /// An IDbCommand extension method that copies the parameterized statement to.
        /// </summary>
        /// <param name="dbCmd">  The dbCmd to act on.</param>
        /// <param name="tmpStmt">The temporary statement.</param>
        private static void CopyParameterizedStatementTo(this IDbCommand dbCmd, IDbCommand tmpStmt)
        {
            dbCmd.CommandText = tmpStmt.CommandText;

            //Instead of creating new generic DbParameters, copy them from the "dummy" IDbCommand, 
            //to keep provider specific information. E.g: SqlServer "datetime2" dbtype
            //We must first create a temp list, as DbParam can't belong to two DbCommands
            var tmpParams = new List<IDbDataParameter>(tmpStmt.Parameters.Count);
            tmpParams.AddRange(tmpStmt.Parameters.Cast<IDbDataParameter>());

            tmpStmt.Parameters.Clear();

            tmpParams.ForEach(x => dbCmd.Parameters.Add(x));
        }

        /// <summary>An IDbConnection extension method that saves.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="objs">  The objects.</param>
        public static void Save<T>(this IDbConnection dbConn, params T[] objs)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.Save(objs));
        }

        /// <summary>An IDbConnection extension method that saves all.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="objs">  The objects.</param>
        public static void SaveAll<T>(this IDbConnection dbConn, IEnumerable<T> objs)
            where T : new()
        {
            dbConn.Exec(dbCmd => dbCmd.SaveAll(objs));
        }

        /// <summary>An IDbConnection extension method that begins a transaction.</summary>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <returns>An IDbTransaction.</returns>
        public static IDbTransaction BeginTransaction(this IDbConnection dbConn)
        {
            return dbConn.Exec(dbCmd => dbCmd.BeginTransaction());
        }

        /// <summary>An IDbConnection extension method that begins a transaction.</summary>
        /// <param name="dbConn">        The dbConn to act on.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>An IDbTransaction.</returns>
        public static IDbTransaction BeginTransaction(this IDbConnection dbConn, IsolationLevel isolationLevel)
        {
            return dbConn.Exec(dbCmd => dbCmd.BeginTransaction(isolationLevel));
        }

        /// <summary>Procedures.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="obj">   The object.</param>
        public static void ExecuteProcedure<T>(this IDbConnection dbConn, T obj)
        {
            dbConn.Exec(dbCmd => dbCmd.ExecuteProcedure(obj));
        }
    }
}