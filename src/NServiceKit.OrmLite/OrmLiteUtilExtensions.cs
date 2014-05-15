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
using System.Text;
using NServiceKit.Text;

namespace NServiceKit.OrmLite
{
    /// <summary>An ORM lite utility extensions.</summary>
	public static class OrmLiteUtilExtensions
	{
        /// <summary>Creates the instance.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>The new instance.</returns>
        public static T CreateInstance<T>()
        {
            return (T)ReflectionExtensions.CreateInstance<T>();
        }

        /// <summary>An IDataReader extension method that convert to.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dataReader">The dataReader to act on.</param>
        /// <returns>to converted.</returns>
		public static T ConvertTo<T>(this IDataReader dataReader)
        {
			var fieldDefs = ModelDefinition<T>.Definition.AllFieldDefinitionsArray;

			using (dataReader)
			{
				if (dataReader.Read())
				{
                    var row = CreateInstance<T>();
					var indexCache = dataReader.GetIndexFieldsCache(ModelDefinition<T>.Definition);
					row.PopulateWithSqlReader(dataReader, fieldDefs, indexCache);
					return row;
				}
				return default(T);
			}
		}

        /// <summary>An IDataReader extension method that converts a dataReader to a list.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dataReader">The dataReader to act on.</param>
        /// <returns>The given data converted to a list.</returns>
		public static List<T> ConvertToList<T>(this IDataReader dataReader)
		{
            var fieldDefs = ModelDefinition<T>.Definition.AllFieldDefinitionsArray;

			var to = new List<T>();
			using (dataReader)
			{
				var indexCache = dataReader.GetIndexFieldsCache(ModelDefinition<T>.Definition);
				while (dataReader.Read())
				{
                    var row = CreateInstance<T>();
					row.PopulateWithSqlReader(dataReader, fieldDefs, indexCache);
					to.Add(row);
				}
			}
			return to;
		}

        /// <summary>A ModelDefinition extension method that gets column names.</summary>
        /// <param name="tableType">The tableType to act on.</param>
        /// <returns>The column names.</returns>
		internal static string GetColumnNames(this Type tableType)
		{
		    var modelDefinition = tableType.GetModelDefinition();
		    return GetColumnNames(modelDefinition);
		}

        /// <summary>A ModelDefinition extension method that gets column names.</summary>
        /// <param name="modelDef">The modelDef to act on.</param>
        /// <returns>The column names.</returns>
	    public static string GetColumnNames(this ModelDefinition modelDef)
	    {
            var sqlColumns = new StringBuilder();
	        modelDef.FieldDefinitions.ForEach(x => 
                sqlColumns.AppendFormat("{0}{1} ", sqlColumns.Length > 0 ? "," : "",
                  OrmLiteConfig.DialectProvider.GetQuotedColumnName(x.FieldName)));

	        return sqlColumns.ToString();
	    }

        /// <summary>An IEnumerable extension method that gets identifiers in SQL.</summary>
        /// <param name="idValues">The idValues to act on.</param>
        /// <returns>The identifiers in SQL.</returns>
	    internal static string GetIdsInSql(this IEnumerable idValues)
		{
			var sql = new StringBuilder();
			foreach (var idValue in idValues)
			{
				if (sql.Length > 0) sql.Append(",");
				sql.AppendFormat("{0}".SqlFormat(idValue));
			}
			return sql.Length == 0 ? null : sql.ToString();
		}

        /// <summary>A string extension method that parameters.</summary>
        /// <param name="sqlText">  The sqlText to act on.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>A string.</returns>
		public static string Params(this string sqlText, params object[] sqlParams)
		{
		    return SqlFormat(sqlText, sqlParams);
		}

        /// <summary>A string extension method that SQL format.</summary>
        /// <param name="sqlText">  The sqlText to act on.</param>
        /// <param name="sqlParams">Options for controlling the SQL.</param>
        /// <returns>A string.</returns>
		public static string SqlFormat(this string sqlText, params object[] sqlParams)
		{
			var escapedParams = new List<string>();
			foreach (var sqlParam in sqlParams)
			{
				if (sqlParam == null)
				{
					escapedParams.Add("NULL");
				}
				else
				{
					var sqlInValues = sqlParam as SqlInValues;
					if (sqlInValues != null)
					{
						escapedParams.Add(sqlInValues.ToSqlInString());
					}
					else
					{
						escapedParams.Add(OrmLiteConfig.DialectProvider.GetQuotedValue(sqlParam, sqlParam.GetType()));
					}
				}
			}
			return string.Format(sqlText, escapedParams.ToArray());
		}

        /// <summary>A List&lt;T&gt; extension method that SQL join.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="values">The values to act on.</param>
        /// <returns>A string.</returns>
		public static string SqlJoin<T>(this List<T> values)
		{
			var sb = new StringBuilder();
			foreach (var value in values)
			{
				if (sb.Length > 0) sb.Append(",");
				sb.Append(OrmLiteConfig.DialectProvider.GetQuotedValue(value, value.GetType()));
			}

			return sb.ToString();
		}

        /// <summary>SQL join.</summary>
        /// <param name="values">The values to act on.</param>
        /// <returns>A string.</returns>
		public static string SqlJoin(IEnumerable values)
		{
			var sb = new StringBuilder();
			foreach (var value in values)
			{
				if (sb.Length > 0) sb.Append(",");
				sb.Append(OrmLiteConfig.DialectProvider.GetQuotedValue(value, value.GetType()));
			}

			return sb.ToString();
		}

        /// <summary>A T[] extension method that SQL in values.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="values">The values to act on.</param>
        /// <returns>The SqlInValues.</returns>
		public static SqlInValues SqlInValues<T>(this List<T> values)
		{
			return new SqlInValues(values);
		}

        /// <summary>A T[] extension method that SQL in values.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="values">The values to act on.</param>
        /// <returns>The SqlInValues.</returns>
		public static SqlInValues SqlInValues<T>(this T[] values)
		{
			return new SqlInValues(values);
		}

        /// <summary>An IDataReader extension method that gets index fields cache.</summary>
        /// <param name="reader">         The reader to act on.</param>
        /// <param name="modelDefinition">The model definition.</param>
        /// <returns>The index fields cache.</returns>
        public static Dictionary<string, int> GetIndexFieldsCache(this IDataReader reader, ModelDefinition modelDefinition = null)
        {
            var cache = new Dictionary<string, int>();
            if (modelDefinition != null)
            {
                foreach (var field in modelDefinition.IgnoredFieldDefinitions)
                {
                    cache[field.FieldName] = -1;
                }
            }
            for (var i = 0; i < reader.FieldCount; i++)
            {
                cache[reader.GetName(i)] = i;
            }
            return cache;
        }

	}
}