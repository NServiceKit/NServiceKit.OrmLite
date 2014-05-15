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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using NServiceKit.Logging;
using NServiceKit.Text;
using System.Linq;

namespace NServiceKit.OrmLite
{
    /// <summary>Gets quoted value delegate.</summary>
    /// <param name="value">    The value.</param>
    /// <param name="fieldType">Type of the field.</param>
    /// <returns>A string.</returns>
	public delegate string GetQuotedValueDelegate(object value, Type fieldType);

    /// <summary>Convert database value delegate.</summary>
    /// <param name="value">The value.</param>
    /// <param name="type"> The type.</param>
    /// <returns>An object.</returns>
	public delegate object ConvertDbValueDelegate(object value, Type type);

    /// <summary>Property setter delegate.</summary>
    /// <param name="instance">The instance.</param>
    /// <param name="value">   The value.</param>
	public delegate void PropertySetterDelegate(object instance, object value);

    /// <summary>Property getter delegate.</summary>
    /// <param name="instance">The instance.</param>
    /// <returns>An object.</returns>
	public delegate object PropertyGetterDelegate(object instance);

    /// <summary>Gets value delegate.</summary>
    /// <param name="i">Zero-based index of the.</param>
    /// <returns>An object.</returns>
	public delegate object GetValueDelegate(int i);

    /// <summary>An ORM lite read extensions.</summary>
	public static class OrmLiteReadExtensions
	{
        /// <summary>The log.</summary>
		private static readonly ILog Log = LogManager.GetLogger(typeof(OrmLiteReadExtensions));

        /// <summary>The use database connection extensions.</summary>
	    public const string UseDbConnectionExtensions = "Use IDbConnection Extensions instead";

        /// <summary>(Only available in DEBUG builds) logs a debug.</summary>
        /// <param name="fmt"> Describes the format to use.</param>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
		[Conditional("DEBUG")]
		private static void LogDebug(string fmt, params object[] args)
		{
			if (args.Length > 0)
				Log.DebugFormat(fmt, args);
			else
				Log.Debug(fmt);
		}

        /// <summary>An IDbCommand extension method that executes the reader operation.</summary>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <param name="sql">  The SQL.</param>
        /// <returns>An IDataReader.</returns>
		internal static IDataReader ExecReader(this IDbCommand dbCmd, string sql)
		{
			LogDebug(sql);
			dbCmd.CommandTimeout = OrmLiteConfig.CommandTimeout;
			dbCmd.CommandText = sql;
			return dbCmd.ExecuteReader();
		}

        /// <summary>An IDbCommand extension method that executes the reader operation.</summary>
        /// <param name="dbCmd">     The dbCmd to act on.</param>
        /// <param name="sql">       The SQL.</param>
        /// <param name="parameters">Options for controlling the operation.</param>
        /// <returns>An IDataReader.</returns>
        internal static IDataReader ExecReader(this IDbCommand dbCmd, string sql, IEnumerable<IDataParameter> parameters)
        {
            LogDebug(sql);
			dbCmd.CommandTimeout = OrmLiteConfig.CommandTimeout;
			dbCmd.CommandText = sql;
            dbCmd.Parameters.Clear();

            foreach (var param in parameters)
            {
                dbCmd.Parameters.Add(param);
            }

			return dbCmd.ExecuteReader();
        }

        /// <summary>Gets value function.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="reader">The reader to act on.</param>
        /// <returns>The value function.</returns>
		public static GetValueDelegate GetValueFn<T>(IDataRecord reader)
		{
			var type = typeof(T);

			if (type == typeof(string))
				return reader.GetString;

			if (type == typeof(short))
				return i => reader.GetInt16(i);

			if (type == typeof(int))
				return i => reader.GetInt32(i);

			if (type == typeof(long))
				return i => reader.GetInt64(i);

			if (type == typeof(bool))
				return i => reader.GetBoolean(i);

			if (type == typeof(DateTime))
				return i => reader.GetDateTime(i);

			if (type == typeof(Guid))
				return i => reader.GetGuid(i);

			if (type == typeof(float))
				return i => reader.GetFloat(i);

			if (type == typeof(double))
				return i => reader.GetDouble(i);

			if (type == typeof(decimal) || type == typeof(decimal?))
				return i => reader.GetDecimal(i);

			return reader.GetValue;
		}

        /// <summary>Query if this object is scalar.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>true if scalar, false if not.</returns>
        public static bool IsScalar<T>()
        {
            return typeof(T).IsValueType || typeof(T) == typeof(string);
        }

        /// <summary>An IDbCommand extension method that selects.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <returns>A List&lt;TModel&gt;</returns>
	    internal static List<T> Select<T>(this IDbCommand dbCmd)
		{
			return Select<T>(dbCmd, (string)null);
		}

        /// <summary>An IDbCommand extension method that selects.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">       The dbCmd to act on.</param>
        /// <param name="sqlFilter">   A filter specifying the SQL.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>A List&lt;TModel&gt;</returns>
	    internal static List<T> Select<T>(this IDbCommand dbCmd, string sqlFilter, params object[] filterParams)
		{
			using (var reader = dbCmd.ExecReader(
				OrmLiteConfig.DialectProvider.ToSelectStatement(typeof(T), sqlFilter, filterParams)))
			{
				return reader.ConvertToList<T>();
			}
		}

        /// <summary>An IDbCommand extension method that selects.</summary>
        /// <typeparam name="TModel">Type of the model.</typeparam>
        /// <param name="dbCmd">        The dbCmd to act on.</param>
        /// <param name="fromTableType">Type of from table.</param>
        /// <returns>A List&lt;TModel&gt;</returns>
	    internal static List<TModel> Select<TModel>(this IDbCommand dbCmd, Type fromTableType)
		{
			return Select<TModel>(dbCmd, fromTableType, null);
		}

        /// <summary>An IDbCommand extension method that selects.</summary>
        /// <typeparam name="TModel">Type of the model.</typeparam>
        /// <param name="dbCmd">        The dbCmd to act on.</param>
        /// <param name="fromTableType">Type of from table.</param>
        /// <param name="sqlFilter">    A filter specifying the SQL.</param>
        /// <param name="filterParams"> Options for controlling the filter.</param>
        /// <returns>A List&lt;TModel&gt;</returns>
	    internal static List<TModel> Select<TModel>(this IDbCommand dbCmd, Type fromTableType, string sqlFilter, params object[] filterParams)
		{
			var sql = new StringBuilder();
			var modelDef = ModelDefinition<TModel>.Definition;
		    sql.AppendFormat("SELECT {0} FROM {1}", OrmLiteConfig.DialectProvider.GetColumnNames( modelDef),
		                     OrmLiteConfig.DialectProvider.GetQuotedTableName(fromTableType.GetModelDefinition()));
            if (!string.IsNullOrEmpty(sqlFilter))
			{
				sqlFilter = sqlFilter.SqlFormat(filterParams);
				sql.Append(" WHERE ");
				sql.Append(sqlFilter);
			}

			using (var reader = dbCmd.ExecReader(sql.ToString()))
			{
				return reader.ConvertToList<TModel>();
			}
		}

        /// <summary>Enumerates each in this collection.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process each in this collection.
        /// </returns>
	    internal static IEnumerable<T> Each<T>(this IDbCommand dbCmd)
		{
			return Each<T>(dbCmd, null);
		}

        /// <summary>Enumerates each in this collection.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">       The dbCmd to act on.</param>
        /// <param name="filter">      Specifies the filter.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process each in this collection.
        /// </returns>
	    internal static IEnumerable<T> Each<T>(this IDbCommand dbCmd, string filter, params object[] filterParams)
		{
			var fieldDefs = ModelDefinition<T>.Definition.FieldDefinitionsArray;
			using (var reader = dbCmd.ExecReader(
				OrmLiteConfig.DialectProvider.ToSelectStatement(typeof(T),  filter, filterParams)))
			{
				var indexCache = reader.GetIndexFieldsCache(ModelDefinition<T>.Definition);
                while (reader.Read())
				{
                    var row = OrmLiteUtilExtensions.CreateInstance<T>();
                    row.PopulateWithSqlReader(reader, fieldDefs, indexCache);
					yield return row;
				}
			}
		}

        /// <summary>An IDbCommand extension method that firsts.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">       The dbCmd to act on.</param>
        /// <param name="filter">      Specifies the filter.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>A T.</returns>
	    internal static T First<T>(this IDbCommand dbCmd, string filter, params object[] filterParams)
		{
			return First<T>(dbCmd, filter.SqlFormat(filterParams));
		}

        /// <summary>An IDbCommand extension method that firsts.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd"> The dbCmd to act on.</param>
        /// <param name="filter">Specifies the filter.</param>
        /// <returns>A T.</returns>
	    internal static T First<T>(this IDbCommand dbCmd, string filter)
		{
			var result = FirstOrDefault<T>(dbCmd, filter);
			if (Equals(result, default(T)))
			{
				throw new ArgumentNullException(string.Format(
					"{0}: '{1}' does not exist", typeof(T).Name, filter));
			}
			return result;
		}

        /// <summary>An IDbCommand extension method that first or default.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">       The dbCmd to act on.</param>
        /// <param name="filter">      Specifies the filter.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>A T.</returns>
	    internal static T FirstOrDefault<T>(this IDbCommand dbCmd, string filter, params object[] filterParams)
		{
			return FirstOrDefault<T>(dbCmd, filter.SqlFormat(filterParams));
		}

        /// <summary>An IDbCommand extension method that first or default.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd"> The dbCmd to act on.</param>
        /// <param name="filter">Specifies the filter.</param>
        /// <returns>A T.</returns>
	    internal static T FirstOrDefault<T>(this IDbCommand dbCmd, string filter)
		{
			using (var dbReader = dbCmd.ExecReader(
				OrmLiteConfig.DialectProvider.ToSelectStatement(typeof(T),  filter)))
			{
				return dbReader.ConvertTo<T>();
			}
		}

        /// <summary>An IDbCommand extension method that gets by identifier.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">  The dbCmd to act on.</param>
        /// <param name="idValue">The identifier value.</param>
        /// <returns>The by identifier.</returns>
	    internal static T GetById<T>(this IDbCommand dbCmd, object idValue)
		{
			return First<T>(dbCmd, OrmLiteConfig.DialectProvider.GetQuotedColumnName(ModelDefinition<T>.PrimaryKeyName) + " = {0}".SqlFormat(idValue));
		}

        /// <summary>Type of the last query.</summary>
		[ThreadStatic]
		private static Type lastQueryType;

        /// <summary>Sets a filter.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
		private static void SetFilter<T>(IDbCommand dbCmd, string name, object value)
		{
			dbCmd.Parameters.Clear();
			var p = dbCmd.CreateParameter();
			p.ParameterName = name;
			p.DbType = OrmLiteConfig.DialectProvider.GetColumnDbType(value.GetType());
			p.Direction = ParameterDirection.Input;
			dbCmd.Parameters.Add(p);
			dbCmd.CommandText = GetFilterSql<T>(dbCmd);
			lastQueryType = typeof(T);
		}

        /// <summary>An IDbCommand extension method that sets the filters.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">       The dbCmd to act on.</param>
        /// <param name="anonType">    Type of the anon.</param>
        /// <param name="excludeNulls">true to exclude, false to include the nulls.</param>
		public static void SetFilters<T>(this IDbCommand dbCmd, object anonType, bool excludeNulls)
		{
			SetParameters<T>(dbCmd, anonType, excludeNulls);

			dbCmd.CommandText = GetFilterSql<T>(dbCmd);
		}

        /// <summary>An IDbCommand extension method that sets the parameters.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">       The dbCmd to act on.</param>
        /// <param name="anonType">    Type of the anon.</param>
        /// <param name="excludeNulls">true to exclude, false to include the nulls.</param>
		private static void SetParameters<T>(this IDbCommand dbCmd, object anonType, bool excludeNulls)
		{
			dbCmd.Parameters.Clear();
			lastQueryType = null;
			if (anonType == null) return;

            var pis = anonType.GetType().GetSerializableProperties();
            var model = ModelDefinition<T>.Definition;

			foreach (var pi in pis)
			{
				var mi = pi.GetGetMethod();
				if (mi == null) continue;

				var value = mi.Invoke(anonType, new object[0]);
				if (excludeNulls && value == null) continue;


				var p = dbCmd.CreateParameter();

                var targetField = model != null ? model.FieldDefinitions.FirstOrDefault(f => string.Equals(f.Name, pi.Name)) : null;
                if (targetField != null && !string.IsNullOrEmpty(targetField.Alias))
                    p.ParameterName = targetField.Alias;
                else
                    p.ParameterName = pi.Name;

				p.DbType = OrmLiteConfig.DialectProvider.GetColumnDbType(pi.PropertyType);
				p.Direction = ParameterDirection.Input;
                p.Value = value ?? DBNull.Value;
				dbCmd.Parameters.Add(p);
			}
		}

        /// <summary>An IDbCommand extension method that sets the parameters.</summary>
        /// <param name="dbCmd">       The dbCmd to act on.</param>
        /// <param name="anonType">    Type of the anon.</param>
        /// <param name="excludeNulls">true to exclude, false to include the nulls.</param>
        private static void SetParameters(this IDbCommand dbCmd, object anonType, bool excludeNulls)
        {
            dbCmd.Parameters.Clear();
            lastQueryType = null;
            if (anonType == null)
                return;

            var pis = anonType.GetType().GetSerializableProperties();

            foreach (var pi in pis)
            {
                var mi = pi.GetGetMethod();
                if (mi == null)
                    continue;

                var value = mi.Invoke(anonType, new object[0]);
                if (excludeNulls && value == null)
                    continue;

                var p = dbCmd.CreateParameter();

                p.ParameterName = pi.Name;
                p.DbType = OrmLiteConfig.DialectProvider.GetColumnDbType(pi.PropertyType);
                p.Direction = ParameterDirection.Input;
                p.Value = value ?? DBNull.Value;
                dbCmd.Parameters.Add(p);
            }
        }	

        /// <summary>An IDbCommand extension method that sets the parameters.</summary>
        /// <param name="dbCmd">       The dbCmd to act on.</param>
        /// <param name="dict">        The dictionary.</param>
        /// <param name="excludeNulls">true to exclude, false to include the nulls.</param>
		private static void SetParameters(this IDbCommand dbCmd, Dictionary<string,object> dict, bool excludeNulls)
		{
			dbCmd.Parameters.Clear();
			lastQueryType = null;
			if (dict == null) return;

			foreach (var kvp in dict)
			{
				var value = dict[kvp.Key];
				if (excludeNulls && value == null) continue;
				var p = dbCmd.CreateParameter();
				p.ParameterName = kvp.Key;

                if (value != null)
                {
                    p.DbType = OrmLiteConfig.DialectProvider.GetColumnDbType(value.GetType());
                }
				
				p.Direction = ParameterDirection.Input;
                p.Value = value ?? DBNull.Value;
				dbCmd.Parameters.Add(p);
			}
		}

        /// <summary>An IDbCommand extension method that sets the filters.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="anonType">Type of the anon.</param>
		public static void SetFilters<T>(this IDbCommand dbCmd, object anonType)
		{
            dbCmd.SetFilters<T>(anonType, excludeNulls: false);
		}

        /// <summary>
        /// An IDbCommand extension method that clears the filters described by dbCmd.
        /// </summary>
        /// <param name="dbCmd">The dbCmd to act on.</param>
		public static void ClearFilters(this IDbCommand dbCmd)
		{
			dbCmd.Parameters.Clear();
		}

        /// <summary>Gets filter SQL.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <returns>The filter SQL.</returns>
		private static string GetFilterSql<T>(IDbCommand dbCmd)
		{
			var sb = new StringBuilder();
			for (var i = 0; i < dbCmd.Parameters.Count; i++)
			{
				sb.Append(i == 0 ? " " : " AND ");
				var p = (IDbDataParameter)dbCmd.Parameters[i];
				sb.AppendFormat("{0} = {1}{2}",
								OrmLiteConfig.DialectProvider.GetQuotedColumnName(p.ParameterName),
								OrmLiteConfig.DialectProvider.ParamString,
								p.ParameterName);
			}
			return OrmLiteConfig.DialectProvider.ToSelectStatement(typeof(T), sb.ToString());
		}

        /// <summary>An IDbCommand extension method that queries by identifier.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <param name="value">The value.</param>
        /// <returns>The by identifier.</returns>
        internal static T QueryById<T>(this IDbCommand dbCmd, object value)
		{
			if (dbCmd.Parameters.Count != 1
				|| ((IDbDataParameter)dbCmd.Parameters[0]).ParameterName != ModelDefinition<T>.PrimaryKeyName
				|| lastQueryType != typeof(T))
				SetFilter<T>(dbCmd, ModelDefinition<T>.PrimaryKeyName, value);

			((IDbDataParameter)dbCmd.Parameters[0]).Value = value;

			using (var dbReader = dbCmd.ExecuteReader())
				return dbReader.ConvertTo<T>();
		}

        /// <summary>An IDbCommand extension method that single where.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>A T.</returns>
        internal static T SingleWhere<T>(this IDbCommand dbCmd, string name, object value)
		{
			if (dbCmd.Parameters.Count != 1 || ((IDbDataParameter)dbCmd.Parameters[0]).ParameterName != name
				|| lastQueryType != typeof(T))
				SetFilter<T>(dbCmd, name, value);

			((IDbDataParameter)dbCmd.Parameters[0]).Value = value;

			using (var dbReader = dbCmd.ExecuteReader())
				return dbReader.ConvertTo<T>();
		}

        /// <summary>An IDbCommand extension method that queries a single.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>The single.</returns>
        internal static T QuerySingle<T>(this IDbCommand dbCmd, object anonType)
		{
			if (IsScalar<T>()) return QueryScalar<T>(dbCmd, anonType);

            dbCmd.SetFilters<T>(anonType, excludeNulls: false);

			using (var dbReader = dbCmd.ExecuteReader())
				return dbReader.ConvertTo<T>();
		}

        /// <summary>An IDbCommand extension method that queries a single.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>The single.</returns>
        internal static T QuerySingle<T>(this IDbCommand dbCmd, string sql, object anonType)
		{
			if (IsScalar<T>()) return QueryScalar<T>(dbCmd, sql, anonType);

            dbCmd.SetParameters<T>(anonType, excludeNulls: false);
            dbCmd.CommandText = OrmLiteConfig.DialectProvider.ToSelectStatement(typeof(T), sql);

			using (var dbReader = dbCmd.ExecuteReader())
				return dbReader.ConvertTo<T>();
		}

        /// <summary>An IDbCommand extension method that wheres.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>A List&lt;T&gt;</returns>
        internal static List<T> Where<T>(this IDbCommand dbCmd, string name, object value)
		{
			if (dbCmd.Parameters.Count != 1 || ((IDbDataParameter)dbCmd.Parameters[0]).ParameterName != name
				|| lastQueryType != typeof(T))
				SetFilter<T>(dbCmd, name, value);

			((IDbDataParameter)dbCmd.Parameters[0]).Value = value;

			using (var dbReader = dbCmd.ExecuteReader())
				return dbReader.ConvertToList<T>();
		}

        /// <summary>An IDbCommand extension method that wheres.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>A List&lt;T&gt;</returns>
        internal static List<T> Where<T>(this IDbCommand dbCmd, object anonType)
		{
			dbCmd.SetFilters<T>(anonType);

			using (var dbReader = dbCmd.ExecuteReader())
				return IsScalar<T>()
					? dbReader.GetFirstColumn<T>()
					: dbReader.ConvertToList<T>();
		}

        /// <summary>An IDbCommand extension method that queries.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>A List&lt;T&gt;</returns>
        internal static List<T> Query<T>(this IDbCommand dbCmd, string sql, object anonType = null)
		{
            if (anonType != null) dbCmd.SetParameters<T>(anonType, excludeNulls: false);
            dbCmd.CommandText = OrmLiteConfig.DialectProvider.ToSelectStatement(typeof(T), sql);

			using (var dbReader = dbCmd.ExecuteReader())
				return IsScalar<T>()
					? dbReader.GetFirstColumn<T>()
					: dbReader.ConvertToList<T>();
		}

        /// <summary>An IDbCommand extension method that queries.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <param name="sql">  The SQL.</param>
        /// <param name="dict"> The dictionary.</param>
        /// <returns>A List&lt;T&gt;</returns>
        internal static List<T> Query<T>(this IDbCommand dbCmd, string sql, Dictionary<string, object> dict)
		{
            if (dict != null) dbCmd.SetParameters(dict, excludeNulls: false);
            dbCmd.CommandText = OrmLiteConfig.DialectProvider.ToSelectStatement(typeof(T), sql);

			using (var dbReader = dbCmd.ExecuteReader())
				return IsScalar<T>()
					? dbReader.GetFirstColumn<T>()
					: dbReader.ConvertToList<T>();
		}

        /// <summary>An IDbCommand extension method that executes the non query operation.</summary>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>An int.</returns>
        internal static int ExecuteNonQuery(this IDbCommand dbCmd, string sql, object anonType = null)
        {
            if (anonType != null)
                dbCmd.SetParameters(anonType, excludeNulls: false);
            dbCmd.CommandText = sql;

            return dbCmd.ExecuteNonQuery();
        }

        /// <summary>An IDbCommand extension method that executes the non query operation.</summary>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <param name="sql">  The SQL.</param>
        /// <param name="dict"> The dictionary.</param>
        /// <returns>An int.</returns>
        internal static int ExecuteNonQuery(this IDbCommand dbCmd, string sql, Dictionary<string, object> dict)
        {
            if (dict != null)
                dbCmd.SetParameters(dict, excludeNulls: false);
            dbCmd.CommandText = sql;

            return dbCmd.ExecuteNonQuery();
        }

        /// <summary>An IDbCommand extension method that queries a scalar.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>The scalar.</returns>
	    internal static T QueryScalar<T>(this IDbCommand dbCmd, object anonType)
		{
            dbCmd.SetFilters<T>(anonType, excludeNulls: false);

			using (var dbReader = dbCmd.ExecuteReader())
				return GetScalar<T>(dbReader);
		}

        /// <summary>An IDbCommand extension method that queries a scalar.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>The scalar.</returns>
	    internal static T QueryScalar<T>(this IDbCommand dbCmd, string sql, object anonType = null)
		{
            if (anonType != null) dbCmd.SetParameters<T>(anonType, excludeNulls: false);
            dbCmd.CommandText = OrmLiteConfig.DialectProvider.ToSelectStatement(typeof(T), sql);

			using (var dbReader = dbCmd.ExecuteReader())
				return GetScalar<T>(dbReader);
		}

        /// <summary>An IDbCommand extension method that SQL list.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>A List&lt;T&gt;</returns>
        internal static List<T> SqlList<T>(this IDbCommand dbCmd, string sql, object anonType = null)
        {
            if (anonType != null) dbCmd.SetParameters<T>(anonType, excludeNulls: false);
            dbCmd.CommandText = sql;

            using (var dbReader = dbCmd.ExecuteReader())
                return IsScalar<T>()
                    ? dbReader.GetFirstColumn<T>()
                    : dbReader.ConvertToList<T>();
        }

        /// <summary>An IDbCommand extension method that SQL list.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <param name="sql">  The SQL.</param>
        /// <param name="dict"> The dictionary.</param>
        /// <returns>A List&lt;T&gt;</returns>
        internal static List<T> SqlList<T>(this IDbCommand dbCmd, string sql, Dictionary<string, object> dict)
        {
            if (dict != null) dbCmd.SetParameters(dict, excludeNulls: false);
            dbCmd.CommandText = sql;

            using (var dbReader = dbCmd.ExecuteReader())
                return IsScalar<T>()
                    ? dbReader.GetFirstColumn<T>()
                    : dbReader.ConvertToList<T>();
        }

        /// <summary>An IDbCommand extension method that SQL scalar.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>A T.</returns>
	    internal static T SqlScalar<T>(this IDbCommand dbCmd, string sql, object anonType = null)
        {
            if (anonType != null) dbCmd.SetParameters<T>(anonType, excludeNulls: false);
            dbCmd.CommandText = sql;

            using (var dbReader = dbCmd.ExecuteReader())
                return GetScalar<T>(dbReader);
        }

        /// <summary>An IDbCommand extension method that SQL scalar.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <param name="sql">  The SQL.</param>
        /// <param name="dict"> The dictionary.</param>
        /// <returns>A T.</returns>
        internal static T SqlScalar<T>(this IDbCommand dbCmd, string sql, Dictionary<string, object> dict)
        {
            if (dict != null) dbCmd.SetParameters(dict, excludeNulls: false);
            dbCmd.CommandText = sql;

            using (var dbReader = dbCmd.ExecuteReader())
                return GetScalar<T>(dbReader);
        }

        /// <summary>An IDbCommand extension method that by example where.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>A List&lt;T&gt;</returns>
        internal static List<T> ByExampleWhere<T>(this IDbCommand dbCmd, object anonType)
        {
            return ByExampleWhere<T>(dbCmd, anonType, true);
        }

        /// <summary>An IDbCommand extension method that by example where.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">       The dbCmd to act on.</param>
        /// <param name="anonType">    Type of the anon.</param>
        /// <param name="excludeNulls">true to exclude, false to include the nulls.</param>
        /// <returns>A List&lt;T&gt;</returns>
        internal static List<T> ByExampleWhere<T>(this IDbCommand dbCmd, object anonType, bool excludeNulls)
		{
            dbCmd.SetFilters<T>(anonType, excludeNulls);

			using (var dbReader = dbCmd.ExecuteReader())
				return dbReader.ConvertToList<T>();
		}

        /// <summary>An IDbCommand extension method that queries by example.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>The by example.</returns>
	    internal static List<T> QueryByExample<T>(this IDbCommand dbCmd, string sql, object anonType = null)
		{
            if (anonType != null) dbCmd.SetParameters<T>(anonType, excludeNulls: false);
            dbCmd.CommandText = OrmLiteConfig.DialectProvider.ToSelectStatement(typeof(T), sql);

			using (var dbReader = dbCmd.ExecuteReader())
				return dbReader.ConvertToList<T>();
		}

        /// <summary>Enumerates query each in this collection.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="sql">     The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process query each in this collection.
        /// </returns>
	    internal static IEnumerable<T> QueryEach<T>(this IDbCommand dbCmd, string sql, object anonType = null)
		{
            if (anonType != null) dbCmd.SetFilters<T>(anonType);

			var fieldDefs = ModelDefinition<T>.Definition.FieldDefinitionsArray;
			using (var reader = dbCmd.ExecuteReader())
			{
				var indexCache = reader.GetIndexFieldsCache(ModelDefinition<T>.Definition);
                while (reader.Read())
				{
                    var row = OrmLiteUtilExtensions.CreateInstance<T>();
					row.PopulateWithSqlReader(reader, fieldDefs, indexCache);
					yield return row;
				}
			}
		}

        /// <summary>Enumerates each where in this collection.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process each where in this collection.
        /// </returns>
	    internal static IEnumerable<T> EachWhere<T>(this IDbCommand dbCmd, object anonType)
		{
			dbCmd.SetFilters<T>(anonType);

			var fieldDefs = ModelDefinition<T>.Definition.FieldDefinitionsArray;
			using (var reader = dbCmd.ExecuteReader())
			{
				var indexCache = reader.GetIndexFieldsCache(ModelDefinition<T>.Definition);
                while (reader.Read())
				{
                    var row = OrmLiteUtilExtensions.CreateInstance<T>();
                    row.PopulateWithSqlReader(reader, fieldDefs, indexCache);
					yield return row;
				}
			}
		}

        /// <summary>An IDbCommand extension method that gets by identifier or default.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">  The dbCmd to act on.</param>
        /// <param name="idValue">The identifier value.</param>
        /// <returns>The by identifier or default.</returns>
	    internal static T GetByIdOrDefault<T>(this IDbCommand dbCmd, object idValue)
		{
			return FirstOrDefault<T>(dbCmd, OrmLiteConfig.DialectProvider.GetQuotedColumnName(ModelDefinition<T>.PrimaryKeyName) + " = {0}".SqlFormat(idValue));
		}

        /// <summary>An IDbCommand extension method that gets by identifiers.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">   The dbCmd to act on.</param>
        /// <param name="idValues">The identifier values.</param>
        /// <returns>The by identifiers.</returns>
	    internal static List<T> GetByIds<T>(this IDbCommand dbCmd, IEnumerable idValues)
		{
			var sql = idValues.GetIdsInSql();
			return sql == null
				? new List<T>()
				: Select<T>(dbCmd, OrmLiteConfig.DialectProvider.GetQuotedColumnName(ModelDefinition<T>.PrimaryKeyName) + " IN (" + sql + ")");
		}

        /// <summary>An IDbCommand extension method that gets by identifier parameter.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <param name="id">   The identifier.</param>
        /// <returns>The by identifier parameter.</returns>
	    internal static T GetByIdParam<T>(this IDbCommand dbCmd, object id)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var idParamString = OrmLiteConfig.DialectProvider.ParamString + "0";

            var sql = string.Format("{0} WHERE {1} = {2}",
                OrmLiteConfig.DialectProvider.ToSelectStatement(typeof(T), "", null),
                OrmLiteConfig.DialectProvider.GetQuotedColumnName(modelDef.PrimaryKey.FieldName),
                idParamString);

            var idParam = dbCmd.CreateParameter();
            idParam.ParameterName = idParamString;
            idParam.Value = id;
            List<IDataParameter> paramsToInsert = new List<IDataParameter>();
            paramsToInsert.Add(idParam);

            return dbCmd.ExecReader(sql, paramsToInsert).ConvertTo<T>();
        }

        /// <summary>An IDataReader extension method that gets a scalar.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">    The dbCmd to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The scalar.</returns>
	    internal static T GetScalar<T>(this IDbCommand dbCmd, string sql, params object[] sqlParams)
		{
			using (var reader = dbCmd.ExecReader(sql.SqlFormat(sqlParams)))
				return GetScalar<T>(reader);
		}

        /// <summary>An IDataReader extension method that gets a scalar.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="reader">The reader to act on.</param>
        /// <returns>The scalar.</returns>
	    internal static T GetScalar<T>(this IDataReader reader)
		{
			while (reader.Read()){
				Type t = typeof(T);

				object oValue = reader.GetValue(0);
				if (oValue == DBNull.Value) return default(T);
	
				if (t== typeof(DateTime) || t== typeof(DateTime?)) 
					return(T)(object) DateTime.Parse(oValue.ToString(), System.Globalization.CultureInfo.CurrentCulture);	
						
				if (t== typeof(decimal) || t== typeof(decimal?)) 
					return(T)(object)decimal.Parse(oValue.ToString(), System.Globalization.CultureInfo.CurrentCulture);	
						
				if (t== typeof(double) || t== typeof(double?)) 
					return(T)(object)double.Parse(oValue.ToString(), System.Globalization.CultureInfo.CurrentCulture);
						
				if (t== typeof(float) || t== typeof(float?))
					return(T)(object)float.Parse(oValue.ToString(), System.Globalization.CultureInfo.CurrentCulture);
						
				object o = OrmLiteConfig.DialectProvider.ConvertDbValue(oValue, t);
				return o == null ? default(T) : (T)o;
			}
			return default(T);
		}

        /// <summary>An IDbCommand extension method that gets the last insert identifier.</summary>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <returns>The last insert identifier.</returns>
	    internal static long GetLastInsertId(this IDbCommand dbCmd)
		{
			return OrmLiteConfig.DialectProvider.GetLastInsertId(dbCmd);
		}

        /// <summary>An IDataReader extension method that gets the first column.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">    The dbCmd to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The first column.</returns>
	    internal static List<T> GetFirstColumn<T>(this IDbCommand dbCmd, string sql, params object[] sqlParams)
		{
			using (var dbReader = dbCmd.ExecReader(sql.SqlFormat(sqlParams)))
				return GetFirstColumn<T>(dbReader);
		}

        /// <summary>An IDataReader extension method that gets the first column.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="reader">The reader to act on.</param>
        /// <returns>The first column.</returns>
	    internal static List<T> GetFirstColumn<T>(this IDataReader reader)
		{
			var columValues = new List<T>();
			var getValueFn = GetValueFn<T>(reader);
			while (reader.Read())
			{
				var value = getValueFn(0);
                if (value == DBNull.Value)
                    value = default(T);

				columValues.Add((T)value);
			}
			return columValues;
		}

        /// <summary>Alias for GetFirstColumn. Returns the first selected column in a List.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">    The dbCmd to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The list.</returns>
	    internal static List<T> GetList<T>(this IDbCommand dbCmd, string sql, params object[] sqlParams)
        {
            return dbCmd.GetFirstColumn<T>(sql, sqlParams);
	    }

        /// <summary>An IDataReader extension method that gets the first column distinct.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">    The dbCmd to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The first column distinct.</returns>
	    internal static HashSet<T> GetFirstColumnDistinct<T>(this IDbCommand dbCmd, string sql, params object[] sqlParams)
        {
            using (var dbReader = dbCmd.ExecReader(sql.SqlFormat(sqlParams)))
                return GetFirstColumnDistinct<T>(dbReader);
        }

        /// <summary>
        /// Alias for GetFirstColumnDistinct. Returns the first selected column in a HashSet.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">    The dbCmd to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The hash set.</returns>
        public static HashSet<T> GetHashSet<T>(this IDbCommand dbCmd, string sql, params object[] sqlParams)
        {
            return dbCmd.GetFirstColumnDistinct<T>(sql, sqlParams);
        }

        /// <summary>An IDataReader extension method that gets the first column distinct.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="reader">The reader to act on.</param>
        /// <returns>The first column distinct.</returns>
	    internal static HashSet<T> GetFirstColumnDistinct<T>(this IDataReader reader)
		{
			var columValues = new HashSet<T>();
			var getValueFn = GetValueFn<T>(reader);
			while (reader.Read())
			{
				var value = getValueFn(0);
                if (value == DBNull.Value)
                    value = default(T);

				columValues.Add((T)value);
			}
			return columValues;
		}

        /// <summary>An IDataReader extension method that gets a lookup.</summary>
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="dbCmd">    The dbCmd to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The lookup.</returns>
	    internal static Dictionary<K, List<V>> GetLookup<K, V>(this IDbCommand dbCmd, string sql, params object[] sqlParams)
		{
			using (var dbReader = dbCmd.ExecReader(sql.SqlFormat(sqlParams)))
				return GetLookup<K, V>(dbReader);
		}

        /// <summary>An IDataReader extension method that gets a lookup.</summary>
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="reader">The reader to act on.</param>
        /// <returns>The lookup.</returns>
	    internal static Dictionary<K, List<V>> GetLookup<K, V>(this IDataReader reader)
		{
			var lookup = new Dictionary<K, List<V>>();

			var getKeyFn = GetValueFn<K>(reader);
			var getValueFn = GetValueFn<V>(reader);
			while (reader.Read())
			{
				var key = (K)getKeyFn(0);
				var value = (V)getValueFn(1);

				List<V> values;
				if (!lookup.TryGetValue(key, out values))
				{
					values = new List<V>();
					lookup[key] = values;
				}
				values.Add(value);
			}

			return lookup;
		}

        /// <summary>An IDataReader extension method that gets a dictionary.</summary>
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="dbCmd">    The dbCmd to act on.</param>
        /// <param name="sql">      The SQL.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>The dictionary.</returns>
	    internal static Dictionary<K, V> GetDictionary<K, V>(this IDbCommand dbCmd, string sql, params object[] sqlParams)
		{
			using (var dbReader = dbCmd.ExecReader(sql.SqlFormat(sqlParams)))
				return GetDictionary<K, V>(dbReader);
		}

        /// <summary>An IDataReader extension method that gets a dictionary.</summary>
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="reader">The reader to act on.</param>
        /// <returns>The dictionary.</returns>
	    internal static Dictionary<K, V> GetDictionary<K, V>(this IDataReader reader)
		{
			var map = new Dictionary<K, V>();

			var getKeyFn = GetValueFn<K>(reader);
			var getValueFn = GetValueFn<V>(reader);
			while (reader.Read())
			{
				var key = (K)getKeyFn(0);
				var value = (V)getValueFn(1);

				map.Add(key, value);
			}

			return map;
		}

        /// <summary>somo aditional methods.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd"> The dbCmd to act on.</param>
        /// <param name="record">The record.</param>
        /// <returns>true if children, false if not.</returns>
	    internal static bool HasChildren<T>(this IDbCommand dbCmd, object record)
		{
			return HasChildren<T>(dbCmd, record, string.Empty);
		}

        /// <summary>An IDbCommand extension method that query if this object has children.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">       The dbCmd to act on.</param>
        /// <param name="record">      The record.</param>
        /// <param name="sqlFilter">   A filter specifying the SQL.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>true if children, false if not.</returns>
		private static bool HasChildren<T>(this IDbCommand dbCmd, object record, string sqlFilter, params object[] filterParams)
		{
			var fromTableType = typeof(T);			
			var sql = OrmLiteConfig.DialectProvider.ToExistStatement(fromTableType, record,sqlFilter, filterParams);
			dbCmd.CommandText = sql;
			var result =  dbCmd.ExecuteScalar();
			return result != null;
		}

        /// <summary>An IDbCommand extension method that exists.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">       The dbCmd to act on.</param>
        /// <param name="sqlFilter">   A filter specifying the SQL.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
	    internal static bool Exists<T>(this IDbCommand dbCmd, string sqlFilter, params object[] filterParams)
		{
			return HasChildren<T>(dbCmd, null, sqlFilter, filterParams);
		}

        /// <summary>An IDbCommand extension method that exists.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd"> The dbCmd to act on.</param>
        /// <param name="record">The record.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
	    internal static bool Exists<T>(this IDbCommand dbCmd, object record)
		{
			return HasChildren<T>(dbCmd, record, string.Empty);
		}

        /// <summary>procedures ...</summary>
        /// <typeparam name="TOutputModel">Type of the output model.</typeparam>
        /// <param name="dbCommand">            The dbCommand to act on.</param>
        /// <param name="fromObjWithProperties">from object with properties.</param>
        /// <returns>A List&lt;TOutputModel&gt;</returns>
	    internal static List<TOutputModel> SelectFromProcedure<TOutputModel>(this IDbCommand dbCommand,
			object fromObjWithProperties)
		{
			return SelectFromProcedure<TOutputModel>(dbCommand, fromObjWithProperties,string.Empty);
		}

        /// <summary>An IDbCommand extension method that select from procedure.</summary>
        /// <typeparam name="TOutputModel">Type of the output model.</typeparam>
        /// <param name="dbCommand">            The dbCommand to act on.</param>
        /// <param name="fromObjWithProperties">from object with properties.</param>
        /// <param name="sqlFilter">            A filter specifying the SQL.</param>
        /// <param name="filterParams">         Options for controlling the filter.</param>
        /// <returns>A List&lt;TOutputModel&gt;</returns>
	    internal static List<TOutputModel> SelectFromProcedure<TOutputModel>(this IDbCommand dbCommand,
			object fromObjWithProperties,
			string sqlFilter, 
			params object[] filterParams)
		{
			var modelType = typeof(TOutputModel);	
			
			string sql = OrmLiteConfig.DialectProvider.ToSelectFromProcedureStatement(
				fromObjWithProperties,modelType, sqlFilter, filterParams);
			
			using (var reader = dbCommand.ExecReader(sql))
			{
				return reader.ConvertToList<TOutputModel>();
			}
		}

        /// <summary>An IDbCommand extension method that gets long scalar.</summary>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <returns>The long scalar.</returns>
	    public static long GetLongScalar(this IDbCommand dbCmd)
		{
			var result = dbCmd.ExecuteScalar();
			if (result is DBNull) return default(long);
			if (result is int) return (int)result;
			if (result is decimal) return Convert.ToInt64((decimal)result);
			if (result is ulong) return (long)Convert.ToUInt64(result);
			return (long)result;
		}			
	}
}