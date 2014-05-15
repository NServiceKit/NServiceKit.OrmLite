﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Npgsql;

namespace NServiceKit.OrmLite.PostgreSQL
{
    /// <summary>A postgre SQL dialect provider.</summary>
    public class PostgreSQLDialectProvider : OrmLiteDialectProviderBase<PostgreSQLDialectProvider>
	{
        /// <summary>The instance.</summary>
		public static PostgreSQLDialectProvider Instance = new PostgreSQLDialectProvider();

        /// <summary>The text column definition.</summary>
        const string textColumnDefinition = "text";

        /// <summary>
        /// Prevents a default instance of the NServiceKit.OrmLite.PostgreSQL.PostgreSQLDialectProvider
        /// class from being created.
        /// </summary>
		private PostgreSQLDialectProvider()
		{
			base.AutoIncrementDefinition = "";
			base.IntColumnDefinition = "integer";
			base.BoolColumnDefinition = "boolean";
			base.TimeColumnDefinition = "time";
			base.DateTimeColumnDefinition = "timestamp";
			base.DecimalColumnDefinition = "numeric(38,6)";
			base.GuidColumnDefinition = "uuid";
			base.ParamString = ":";
			base.BlobColumnDefinition = "bytea";
			base.RealColumnDefinition = "double precision";
            base.StringLengthColumnDefinitionFormat = textColumnDefinition;
            //there is no "n"varchar in postgres. All strings are either unicode or non-unicode, inherited from the database.
            base.StringLengthUnicodeColumnDefinitionFormat = "character varying({0})";
            base.StringLengthNonUnicodeColumnDefinitionFormat = "character varying({0})"; 
			base.InitColumnTypeMap();
		    base.SelectIdentitySql = "SELECT LASTVAL()";
		    this.NamingStrategy = new PostgreSqlNamingStrategy();

            DbTypeMap.Set<TimeSpan>(DbType.Time, "Interval");
            DbTypeMap.Set<TimeSpan?>(DbType.Time, "Interval");
		}

        /// <summary>Gets column definition.</summary>
        /// <param name="fieldName">    Name of the field.</param>
        /// <param name="fieldType">    Type of the field.</param>
        /// <param name="isPrimaryKey"> true if this object is primary key.</param>
        /// <param name="autoIncrement">true to automatically increment.</param>
        /// <param name="isNullable">   true if this object is nullable.</param>
        /// <param name="fieldLength">  Length of the field.</param>
        /// <param name="scale">        The scale.</param>
        /// <param name="defaultValue"> The default value.</param>
        /// <returns>The column definition.</returns>
		public override string GetColumnDefinition(
			string fieldName,
			Type fieldType,
			bool isPrimaryKey,
			bool autoIncrement,
			bool isNullable,
			int? fieldLength,
			int? scale,
			string defaultValue)
		{
			string fieldDefinition = null;
			if (fieldType == typeof(string))
			{
				if (fieldLength != null)
				{
					fieldDefinition = string.Format(base.StringLengthColumnDefinitionFormat, fieldLength);
				}
				else
				{
                    fieldDefinition = textColumnDefinition;
				}
			}
			else
			{
				if (autoIncrement)
				{
					if (fieldType == typeof(long))
						fieldDefinition = "bigserial";
					else if (fieldType == typeof(int))
						fieldDefinition = "serial";
				}
				else
				{
					fieldDefinition = GetColumnTypeDefinition(fieldType);
				}
			}

			var sql = new StringBuilder();
			sql.AppendFormat("{0} {1}", GetQuotedColumnName(fieldName), fieldDefinition);

			if (isPrimaryKey)
			{
				sql.Append(" PRIMARY KEY");
			}
			else
			{
				if (isNullable)
				{
					sql.Append(" NULL");
				}
				else
				{
					sql.Append(" NOT NULL");
				}
			}

			if (!string.IsNullOrEmpty(defaultValue))
			{
				sql.AppendFormat(DefaultValueFormat, defaultValue);
			}

			return sql.ToString();
		}		

        /// <summary>Gets quoted parameter.</summary>
        /// <param name="paramValue">The parameter value.</param>
        /// <returns>The quoted parameter.</returns>
        public override string GetQuotedParam(string paramValue)
        {
            return "'" + paramValue.Replace("'", @"''") + "'";
        }

        /// <summary>Creates a connection.</summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="options">         Options for controlling the operation.</param>
        /// <returns>The new connection.</returns>
		public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
		{
			return new NpgsqlConnection(connectionString);
		}

        /// <summary>Gets quoted value.</summary>
        /// <param name="value">    The value.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns>The quoted value.</returns>
		public override string GetQuotedValue(object value, Type fieldType)
		{
			if (value == null) return "NULL";

			if (fieldType == typeof(DateTime))
			{
				var dateValue = (DateTime)value;
				const string iso8601Format = "yyyy-MM-dd HH:mm:ss.fff";
				return base.GetQuotedValue(dateValue.ToString(iso8601Format), typeof(string));
			}
			if (fieldType == typeof(Guid))
			{
				var guidValue = (Guid)value;
				return base.GetQuotedValue(guidValue.ToString("N"), typeof(string));
			}
			if(fieldType == typeof(byte[]))
			{
				return "E'" + ToBinary(value) + "'";
			}
			if (fieldType.IsArray && typeof(string).IsAssignableFrom(fieldType.GetElementType()))
			{
				var stringArray = (string[]) value;
				return ToArray(stringArray);
			}
			if (fieldType.IsArray && typeof(int).IsAssignableFrom(fieldType.GetElementType()))
			{
				var integerArray = (int[]) value;
				return ToArray(integerArray);
			}
			if (fieldType.IsArray && typeof(long).IsAssignableFrom(fieldType.GetElementType()))
			{
				var longArray = (long[]) value;
				return ToArray(longArray);
			}

			return base.GetQuotedValue(value, fieldType);
		}

        /// <summary>Convert database value.</summary>
        /// <param name="value">The value.</param>
        /// <param name="type"> The type.</param>
        /// <returns>The database converted value.</returns>
		public override object ConvertDbValue(object value, Type type)
		{
			if (value == null || value is DBNull) return null;
			
			if (type == typeof(byte[])) { return value; }

			return base.ConvertDbValue(value, type);
		}

        /// <summary>Expression visitor.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
		public override SqlExpressionVisitor<T> ExpressionVisitor<T>()
		{
			return new PostgreSQLExpressionVisitor<T>();
		}

        /// <summary>Query if 'dbCmd' does table exist.</summary>
        /// <param name="dbCmd">    The database command.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
		public override bool DoesTableExist(IDbCommand dbCmd, string tableName)
		{
			var sql = "SELECT COUNT(*) FROM pg_class WHERE relname = {0}"
				.SqlFormat(tableName);
		    var conn = dbCmd.Connection;
            if (conn != null)
            {
                var builder = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
                // If a search path (schema) is specified, and there is only one, then assume the CREATE TABLE directive should apply to that schema.
                if (!String.IsNullOrEmpty(builder.SearchPath) && !builder.SearchPath.Contains(","))
                    sql = "SELECT COUNT(*) FROM pg_class JOIN pg_catalog.pg_namespace n ON n.oid = pg_class.relnamespace WHERE relname = {0} AND nspname = {1}"
                          .SqlFormat(tableName, builder.SearchPath);
            }     
			dbCmd.CommandText = sql;
			var result = dbCmd.GetLongScalar();

			return result > 0;
		}

        /// <summary>Converts the objWithProperties to an execute procedure statement.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <returns>objWithProperties as a string.</returns>
		public override string ToExecuteProcedureStatement(object objWithProperties)
		{
			var sbColumnValues = new StringBuilder();

			var tableType = objWithProperties.GetType();
			var modelDef = GetModel(tableType);

			foreach (var fieldDef in modelDef.FieldDefinitions)
			{
				if (sbColumnValues.Length > 0) sbColumnValues.Append(",");
				try
				{
					sbColumnValues.Append(fieldDef.GetQuotedValue(objWithProperties));
				}
				catch (Exception)
				{
					throw;
				}
			}

			var sql = string.Format("{0} {1}{2}{3};",
				GetQuotedTableName(modelDef),
				sbColumnValues.Length > 0 ? "(" : "",
				sbColumnValues,
				sbColumnValues.Length > 0 ? ")" : "");

			return sql;
		}

        /// <summary>Gets quoted table name.</summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>The quoted table name.</returns>
        public override string GetQuotedTableName(ModelDefinition modelDef)
        {
            if (!modelDef.IsInSchema)
            {
                return base.GetQuotedTableName(modelDef);
            }
            string escapedSchema = modelDef.Schema.Replace(".", "\".\"");
            return string.Format("\"{0}\".\"{1}\"", escapedSchema, base.NamingStrategy.GetTableName(modelDef.ModelName));
        }

        /// <summary>
        /// based on Npgsql2's source: Npgsql2\src\NpgsqlTypes\NpgsqlTypeConverters.cs.
        /// </summary>
        /// <param name="NativeData">.</param>
        /// <returns>A binary represenation of this object.</returns>
        /// ### <param name="TypeInfo">        .</param>
        /// ### <param name="ForExtendedQuery">.</param>
		internal static String ToBinary(Object NativeData)
		{
			Byte[] byteArray = (Byte[])NativeData;
			StringBuilder res = new StringBuilder(byteArray.Length * 5);
			foreach(byte b in byteArray)
				if(b >= 0x20 && b < 0x7F && b != 0x27 && b != 0x5C)
					res.Append((char)b);
				else
					res.Append("\\\\")
						.Append((char)('0' + (7 & (b >> 6))))
						.Append((char)('0' + (7 & (b >> 3))))
						.Append((char)('0' + (7 & b)));
			return res.ToString();
		}

        /// <summary>Convert this object into an array representation.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="source">Source for the.</param>
        /// <returns>source as a string.</returns>
		internal string ToArray<T>(T[] source)
		{
			var values = new StringBuilder();
			foreach (var value in source)
			{
				if (values.Length > 0) values.Append(",");
				values.Append(base.GetQuotedValue(value, typeof(T)));
			}
			return "ARRAY[" + values + "]";
		}
	}
}
