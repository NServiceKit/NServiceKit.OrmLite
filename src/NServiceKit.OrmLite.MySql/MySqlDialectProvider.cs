﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using NServiceKit.OrmLite.MySql.DataAnnotations;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.MySql
{
    /// <summary>my SQL dialect provider.</summary>
    public class MySqlDialectProvider : OrmLiteDialectProviderBase<MySqlDialectProvider>
    {
        /// <summary>The instance.</summary>
        public static MySqlDialectProvider Instance = new MySqlDialectProvider();

        /// <summary>The text column definition.</summary>
    	private const string TextColumnDefinition = "TEXT";

        /// <summary>
        /// Prevents a default instance of the NServiceKit.OrmLite.MySql.MySqlDialectProvider class from
        /// being created.
        /// </summary>
    	private MySqlDialectProvider()
        {
            base.AutoIncrementDefinition = "AUTO_INCREMENT";
            base.IntColumnDefinition = "int(11)";
            base.BoolColumnDefinition = "tinyint(1)";
            base.TimeColumnDefinition = "time";
            base.DecimalColumnDefinition = "decimal(38,6)";
            base.GuidColumnDefinition = "char(32)";
            base.DefaultStringLength = 255;
            base.InitColumnTypeMap();
    	    base.DefaultValueFormat = " DEFAULT '{0}'";
    	    base.SelectIdentitySql = "SELECT LAST_INSERT_ID()";
        }

        /// <summary>Gets quoted parameter.</summary>
        /// <param name="paramValue">The parameter value.</param>
        /// <returns>The quoted parameter.</returns>
        public override string GetQuotedParam(string paramValue)
        {
            return "'" + paramValue.Replace("\\", "\\\\").Replace("'", @"\'") + "'";
        }

        /// <summary>Creates a connection.</summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="options">         Options for controlling the operation.</param>
        /// <returns>The new connection.</returns>
        public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
        {
            return new MySqlConnection(connectionString);
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
                /*
                 * ms not contained in format. MySql ignores ms part anyway
                 * 
                 * for more details see: http://dev.mysql.com/doc/refman/5.1/en/datetime.html
                 */
                const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss"; 

                return base.GetQuotedValue(dateValue.ToString(dateTimeFormat), typeof(string));
            }
            if (fieldType == typeof(Guid))
            {
                var guidValue = (Guid)value;
                return base.GetQuotedValue(guidValue.ToString("N"), typeof(string));
            }

            if (fieldType == typeof(byte[]))
            {
                return "0x" + BitConverter.ToString((byte[])value).Replace("-", "");
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

            if (type == typeof(bool))
            {
                return
                    value is bool
                        ? value
                        : (int.Parse(value.ToString()) != 0); //backward compatibility (prev version mapped bool as bit(1))
            }

            if (type == typeof(byte[]))
                return value;

            return base.ConvertDbValue(value, type);
        }

        /// <summary>Gets quoted table name.</summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>The quoted table name.</returns>
        public override string GetQuotedTableName(ModelDefinition modelDef)
        {
            return string.Format("`{0}`", NamingStrategy.GetTableName(modelDef.ModelName));
        }

        /// <summary>Gets quoted table name.</summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>The quoted table name.</returns>
        public override string GetQuotedTableName(string tableName)
        {
            return string.Format("`{0}`", NamingStrategy.GetTableName(tableName));
        }

        /// <summary>Gets quoted column name.</summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The quoted column name.</returns>
		public override string GetQuotedColumnName(string columnName)
		{
			return string.Format("`{0}`", NamingStrategy.GetColumnName(columnName));
		}

        /// <summary>Gets quoted name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The quoted name.</returns>
        public override string GetQuotedName(string name)
        {
			return string.Format("`{0}`", name);
        }

        /// <summary>Expression visitor.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public override SqlExpressionVisitor<T> ExpressionVisitor<T> ()
		{
			return new MySqlExpressionVisitor<T>();
		}

        /// <summary>Query if 'dbCmd' does table exist.</summary>
        /// <param name="dbCmd">    The database command.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
		public override bool DoesTableExist(IDbCommand dbCmd, string tableName)
		{
			//Same as SQL Server apparently?
			var sql = ("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
				"WHERE TABLE_NAME = {0} AND " +
				"TABLE_SCHEMA = {1}")
				.SqlFormat(tableName, dbCmd.Connection.Database);

			//if (!string.IsNullOrEmpty(schemaName))
			//    sql += " AND TABLE_SCHEMA = {0}".SqlFormat(schemaName);

			dbCmd.CommandText = sql;
			var result = dbCmd.GetLongScalar();

			return result > 0;
		}

        /// <summary>Converts a tableType to a create table statement.</summary>
        /// <param name="tableType">Type of the table.</param>
        /// <returns>tableType as a string.</returns>
        public override string ToCreateTableStatement(Type tableType)
        {
            var sbColumns = new StringBuilder();
            var sbConstraints = new StringBuilder();

            var modelDef = GetModel(tableType);
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (sbColumns.Length != 0) sbColumns.Append(", \n  ");

                sbColumns.Append(GetColumnDefinition(fieldDef));

                if (fieldDef.ForeignKey == null) continue;

                var refModelDef = GetModel(fieldDef.ForeignKey.ReferenceType);
                sbConstraints.AppendFormat(
                    ", \n\n  CONSTRAINT {0} FOREIGN KEY ({1}) REFERENCES {2} ({3})",
                    GetQuotedName(fieldDef.ForeignKey.GetForeignKeyName(modelDef, refModelDef, NamingStrategy, fieldDef)),
                    GetQuotedColumnName(fieldDef.FieldName),
                    GetQuotedTableName(refModelDef),
                    GetQuotedColumnName(refModelDef.PrimaryKey.FieldName));

                if (!string.IsNullOrEmpty(fieldDef.ForeignKey.OnDelete))
                    sbConstraints.AppendFormat(" ON DELETE {0}", fieldDef.ForeignKey.OnDelete);

                if (!string.IsNullOrEmpty(fieldDef.ForeignKey.OnUpdate))
                    sbConstraints.AppendFormat(" ON UPDATE {0}", fieldDef.ForeignKey.OnUpdate);
            }
            var sql = new StringBuilder(string.Format(
                "CREATE TABLE {0} \n(\n  {1}{2} \n); \n", GetQuotedTableName(modelDef), sbColumns, sbConstraints));

            return sql.ToString();
        }

        /// <summary>Gets column definition.</summary>
        /// <param name="fieldDefinition">The field definition.</param>
        /// <returns>The column definition.</returns>
        public string GetColumnDefinition(FieldDefinition fieldDefinition)
        {
            if (fieldDefinition.PropertyInfo.FirstAttribute<TextAttribute>() != null)
            {
                var sql = new StringBuilder();
                sql.AppendFormat("{0} {1}", GetQuotedColumnName(fieldDefinition.FieldName), TextColumnDefinition);
                sql.Append(fieldDefinition.IsNullable ? " NULL" : " NOT NULL");
                return sql.ToString();
            }

            return base.GetColumnDefinition(
                fieldDefinition.FieldName, 
                fieldDefinition.FieldType,
                fieldDefinition.IsPrimaryKey, 
                fieldDefinition.AutoIncrement, 
                fieldDefinition.IsNullable, 
                fieldDefinition.FieldLength, 
                null, 
                fieldDefinition.DefaultValue);
        }
	}
}
