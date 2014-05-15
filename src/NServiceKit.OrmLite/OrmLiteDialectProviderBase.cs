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
using System.Globalization;
using System.Text;
using NServiceKit.DataAnnotations;
using NServiceKit.Logging;
using NServiceKit.Text;
using System.Diagnostics;
using NServiceKit.Common;
using System.IO;
using System.Linq.Expressions;

namespace NServiceKit.OrmLite
{
    /// <summary>An ORM lite dialect provider base.</summary>
    /// <typeparam name="TDialect">Type of the dialect.</typeparam>
    public abstract class OrmLiteDialectProviderBase<TDialect>
        : IOrmLiteDialectProvider
        where TDialect : IOrmLiteDialectProvider
    {
        /// <summary>The log.</summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof(IOrmLiteDialectProvider));

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

        #region ADO.NET supported types
        /* ADO.NET UNDERSTOOD DATA TYPES:
			COUNTER	DbType.Int64
			AUTOINCREMENT	DbType.Int64
			IDENTITY	DbType.Int64
			LONG	DbType.Int64
			TINYINT	DbType.Byte
			INTEGER	DbType.Int64
			INT	DbType.Int32
			VARCHAR	DbType.String
			NVARCHAR	DbType.String
			CHAR	DbType.String
			NCHAR	DbType.String
			TEXT	DbType.String
			NTEXT	DbType.String
			STRING	DbType.String
			DOUBLE	DbType.Double
			FLOAT	DbType.Double
			REAL	DbType.Single
			BIT	DbType.Boolean
			YESNO	DbType.Boolean
			LOGICAL	DbType.Boolean
			BOOL	DbType.Boolean
			NUMERIC	DbType.Decimal
			DECIMAL	DbType.Decimal
			MONEY	DbType.Decimal
			CURRENCY	DbType.Decimal
			TIME	DbType.DateTime
			DATE	DbType.DateTime
			TIMESTAMP	DbType.DateTime
			DATETIME	DbType.DateTime
			BLOB	DbType.Binary
			BINARY	DbType.Binary
			VARBINARY	DbType.Binary
			IMAGE	DbType.Binary
			GENERAL	DbType.Binary
			OLEOBJECT	DbType.Binary
			GUID	DbType.Guid
			UNIQUEIDENTIFIER	DbType.Guid
			MEMO	DbType.String
			NOTE	DbType.String
			LONGTEXT	DbType.String
			LONGCHAR	DbType.String
			SMALLINT	DbType.Int16
			BIGINT	DbType.Int64
			LONGVARCHAR	DbType.String
			SMALLDATE	DbType.DateTime
			SMALLDATETIME	DbType.DateTime
		 */
        #endregion

        /// <summary>The log.</summary>
        private static ILog log = LogManager.GetLogger(typeof(OrmLiteDialectProviderBase<>));

        /// <summary>The string length non unicode column definition format.</summary>
        public string StringLengthNonUnicodeColumnDefinitionFormat = "VARCHAR({0})";

        /// <summary>The string length unicode column definition format.</summary>
        public string StringLengthUnicodeColumnDefinitionFormat = "NVARCHAR({0})";

        /// <summary>Set by Constructor and UpdateStringColumnDefinitions()</summary>
        public string StringColumnDefinition;

        /// <summary>The string length column definition format.</summary>
        public string StringLengthColumnDefinitionFormat;

        /// <summary>SqlServer express limit.</summary>
        public string AutoIncrementDefinition = "AUTOINCREMENT";

        /// <summary>The int column definition.</summary>
        public string IntColumnDefinition = "INTEGER";

        /// <summary>The long column definition.</summary>
        public string LongColumnDefinition = "BIGINT";

        /// <summary>The unique identifier column definition.</summary>
        public string GuidColumnDefinition = "GUID";

        /// <summary>The column definition.</summary>
        public string BoolColumnDefinition = "BOOL";

        /// <summary>The real column definition.</summary>
        public string RealColumnDefinition = "DOUBLE";

        /// <summary>The decimal column definition.</summary>
        public string DecimalColumnDefinition = "DECIMAL";

        /// <summary>The BLOB column definition.</summary>
        public string BlobColumnDefinition = "BLOB";

        /// <summary>The date time column definition.</summary>
        public string DateTimeColumnDefinition = "DATETIME";

        /// <summary>The time column definition.</summary>
        public string TimeColumnDefinition = "DATETIME";

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.OrmLiteDialectProviderBase&lt;
        /// TDialect&gt; class.
        /// </summary>
        protected OrmLiteDialectProviderBase()
        {
            UpdateStringColumnDefinitions();
        }

        /// <summary>The default decimal precision.</summary>
        private int defaultDecimalPrecision = 18;

        /// <summary>The default decimal scale.</summary>
        private int defaultDecimalScale = 12;

        /// <summary>Gets or sets the default decimal precision.</summary>
        /// <value>The default decimal precision.</value>
        public int DefaultDecimalPrecision
        {
            get { return defaultDecimalPrecision; }
            set { defaultDecimalPrecision = value; }
        }

        /// <summary>Gets or sets the default decimal scale.</summary>
        /// <value>The default decimal scale.</value>
        public int DefaultDecimalScale
        {
            get { return defaultDecimalScale; }
            set { defaultDecimalScale = value; }
        }

        /// <summary>SqlServer express limit.</summary>
        private int defaultStringLength = 8000;

        /// <summary>Gets or sets the default string length.</summary>
        /// <value>The default string length.</value>
        public int DefaultStringLength
        {
            get
            {
                return defaultStringLength;
            }
            set
            {
                defaultStringLength = value;
                UpdateStringColumnDefinitions();
            }
        }

        /// <summary>The parameter string.</summary>
        private string paramString = "@";

        /// <summary>Gets or sets the parameter string.</summary>
        /// <value>The parameter string.</value>
        public string ParamString
        {
            get { return paramString; }
            set { paramString = value; }
        }

        /// <summary>true to use unicode.</summary>
        protected bool useUnicode;

        /// <summary>Gets or sets a value indicating whether this object use unicode.</summary>
        /// <value>true if use unicode, false if not.</value>
        public virtual bool UseUnicode
        {
            get
            {
                return useUnicode;
            }
            set
            {
                useUnicode = value;
                UpdateStringColumnDefinitions();
            }
        }

        /// <summary>The naming strategy.</summary>
        private INamingStrategy namingStrategy = new OrmLiteNamingStrategyBase();

        /// <summary>Gets or sets the naming strategy.</summary>
        /// <value>The naming strategy.</value>
        public INamingStrategy NamingStrategy
        {
            get
            {
                return namingStrategy;
            }
            set
            {
                namingStrategy = value;
            }
        }

        /// <summary>Updates the string column definitions.</summary>
        private void UpdateStringColumnDefinitions()
        {
            this.StringLengthColumnDefinitionFormat = useUnicode
                ? StringLengthUnicodeColumnDefinitionFormat
                : StringLengthNonUnicodeColumnDefinitionFormat;

            this.StringColumnDefinition = string.Format(
                this.StringLengthColumnDefinitionFormat, DefaultStringLength);

        }

        /// <summary>The database type map.</summary>
        protected DbTypes<TDialect> DbTypeMap = new DbTypes<TDialect>();

        /// <summary>Initialises the column type map.</summary>
        protected void InitColumnTypeMap()
        {
            DbTypeMap.Set<string>(DbType.String, StringColumnDefinition);
            DbTypeMap.Set<char>(DbType.StringFixedLength, StringColumnDefinition);
            DbTypeMap.Set<char?>(DbType.StringFixedLength, StringColumnDefinition);
            DbTypeMap.Set<char[]>(DbType.String, StringColumnDefinition);
            DbTypeMap.Set<bool>(DbType.Boolean, BoolColumnDefinition);
            DbTypeMap.Set<bool?>(DbType.Boolean, BoolColumnDefinition);
            DbTypeMap.Set<Guid>(DbType.Guid, GuidColumnDefinition);
            DbTypeMap.Set<Guid?>(DbType.Guid, GuidColumnDefinition);
            DbTypeMap.Set<DateTime>(DbType.DateTime, DateTimeColumnDefinition);
            DbTypeMap.Set<DateTime?>(DbType.DateTime, DateTimeColumnDefinition);
            DbTypeMap.Set<TimeSpan>(DbType.Time, TimeColumnDefinition);
            DbTypeMap.Set<TimeSpan?>(DbType.Time, TimeColumnDefinition);
            DbTypeMap.Set<DateTimeOffset>(DbType.Time, TimeColumnDefinition);
            DbTypeMap.Set<DateTimeOffset?>(DbType.Time, TimeColumnDefinition);

            DbTypeMap.Set<byte>(DbType.Byte, IntColumnDefinition);
            DbTypeMap.Set<byte?>(DbType.Byte, IntColumnDefinition);
            DbTypeMap.Set<sbyte>(DbType.SByte, IntColumnDefinition);
            DbTypeMap.Set<sbyte?>(DbType.SByte, IntColumnDefinition);
            DbTypeMap.Set<short>(DbType.Int16, IntColumnDefinition);
            DbTypeMap.Set<short?>(DbType.Int16, IntColumnDefinition);
            DbTypeMap.Set<ushort>(DbType.UInt16, IntColumnDefinition);
            DbTypeMap.Set<ushort?>(DbType.UInt16, IntColumnDefinition);
            DbTypeMap.Set<int>(DbType.Int32, IntColumnDefinition);
            DbTypeMap.Set<int?>(DbType.Int32, IntColumnDefinition);
            DbTypeMap.Set<uint>(DbType.UInt32, IntColumnDefinition);
            DbTypeMap.Set<uint?>(DbType.UInt32, IntColumnDefinition);

            DbTypeMap.Set<long>(DbType.Int64, LongColumnDefinition);
            DbTypeMap.Set<long?>(DbType.Int64, LongColumnDefinition);
            DbTypeMap.Set<ulong>(DbType.UInt64, LongColumnDefinition);
            DbTypeMap.Set<ulong?>(DbType.UInt64, LongColumnDefinition);

            DbTypeMap.Set<float>(DbType.Single, RealColumnDefinition);
            DbTypeMap.Set<float?>(DbType.Single, RealColumnDefinition);
            DbTypeMap.Set<double>(DbType.Double, RealColumnDefinition);
            DbTypeMap.Set<double?>(DbType.Double, RealColumnDefinition);

            DbTypeMap.Set<decimal>(DbType.Decimal, DecimalColumnDefinition);
            DbTypeMap.Set<decimal?>(DbType.Decimal, DecimalColumnDefinition);

            DbTypeMap.Set<byte[]>(DbType.Binary, BlobColumnDefinition);

            DbTypeMap.Set<object>(DbType.Object, StringColumnDefinition);
        }

        /// <summary>The default value format.</summary>
        public string DefaultValueFormat = " DEFAULT ({0})";

        /// <summary>Determine if we should quote value.</summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public virtual bool ShouldQuoteValue(Type fieldType)
        {
            string fieldDefinition;
            if (!DbTypeMap.ColumnTypeMap.TryGetValue(fieldType, out fieldDefinition))
            {
                fieldDefinition = this.GetUndefinedColumnDefinition(fieldType, null);
            }

            return fieldDefinition != IntColumnDefinition
                   && fieldDefinition != LongColumnDefinition
                   && fieldDefinition != RealColumnDefinition
                   && fieldDefinition != DecimalColumnDefinition
                   && fieldDefinition != BoolColumnDefinition;
        }

        /// <summary>Convert database value.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="value">The value.</param>
        /// <param name="type"> The type.</param>
        /// <returns>The database converted value.</returns>
        public virtual object ConvertDbValue(object value, Type type)
        {
            if (value == null || value.GetType() == typeof(DBNull)) return null;

            if (value.GetType() == type)
            {
                if (type == typeof(byte[]))
                    return TypeSerializer.DeserializeFromStream<byte[]>(new MemoryStream((byte[])value));

                return value;
            }

            if (type.IsValueType)
            {
                if (type == typeof(float))
                    return value is double ? (float)((double)value) : (float)value;

                if (type == typeof(double))
                    return value is float ? (double)((float)value) : (double)value;

                if (type == typeof(decimal))
                    return (decimal)value;
            }

            if (type == typeof(string))
                return value;

            try
            {
                var convertedValue = TypeSerializer.DeserializeFromString(value.ToString(), type);
                return convertedValue;
            }
            catch (Exception)
            {
                log.ErrorFormat("Error ConvertDbValue trying to convert {0} into {1}",
                    value, type.Name);
                throw;
            }
        }

        /// <summary>Gets quoted value.</summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="value">    The value.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns>The quoted value.</returns>
        public virtual string GetQuotedValue(object value, Type fieldType)
        {
            if (value == null) return "NULL";

            if (!fieldType.UnderlyingSystemType.IsValueType && fieldType != typeof(string))
            {
                if (TypeSerializer.CanCreateFromString(fieldType))
                {
                    return OrmLiteConfig.DialectProvider.GetQuotedParam(TypeSerializer.SerializeToString(value));
                }

                throw new NotSupportedException(
                    string.Format("Property of type: {0} is not supported", fieldType.FullName));
            }

            if (fieldType == typeof(float))
                return ((float)value).ToString(CultureInfo.InvariantCulture);

            if (fieldType == typeof(double))
                return ((double)value).ToString(CultureInfo.InvariantCulture);

            if (fieldType == typeof(decimal))
                return ((decimal)value).ToString(CultureInfo.InvariantCulture);

            return ShouldQuoteValue(fieldType)
                    ? OrmLiteConfig.DialectProvider.GetQuotedParam(value.ToString())
                    : value.ToString();
        }

        /// <summary>Creates a connection.</summary>
        /// <param name="filePath">Full pathname of the file.</param>
        /// <param name="options"> Options for controlling the operation.</param>
        /// <returns>The new connection.</returns>
        public abstract IDbConnection CreateConnection(string filePath, Dictionary<string, string> options);

        /// <summary>
        /// Quote the string so that it can be used inside an SQL-expression Escape quotes inside the
        /// string.
        /// </summary>
        /// <param name="paramValue">.</param>
        /// <returns>The quoted parameter.</returns>
        public virtual string GetQuotedParam(string paramValue)
        {
            return "'" + paramValue.Replace("'", "''") + "'";
        }

        /// <summary>Gets quoted table name.</summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>The quoted table name.</returns>
        public virtual string GetQuotedTableName(ModelDefinition modelDef)
        {
            return GetQuotedTableName(modelDef.ModelName);
        }

        /// <summary>Gets quoted table name.</summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>The quoted table name.</returns>
        public virtual string GetQuotedTableName(string tableName)
        {
            return string.Format("\"{0}\"", namingStrategy.GetTableName(tableName));
        }

        /// <summary>Gets quoted column name.</summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The quoted column name.</returns>
        public virtual string GetQuotedColumnName(string columnName)
        {
            return string.Format("\"{0}\"", namingStrategy.GetColumnName(columnName));
        }

        /// <summary>Gets quoted name.</summary>
        /// <param name="name">Name of the column.</param>
        /// <returns>The quoted name.</returns>
        public virtual string GetQuotedName(string name)
        {
            return string.Format("\"{0}\"", name);
        }

        /// <summary>Gets undefined column definition.</summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="fieldType">  Type of the field.</param>
        /// <param name="fieldLength">Length of the field.</param>
        /// <returns>The undefined column definition.</returns>
        protected virtual string GetUndefinedColumnDefinition(Type fieldType, int? fieldLength)
        {
            if (TypeSerializer.CanCreateFromString(fieldType))
            {
                return string.Format(StringLengthColumnDefinitionFormat, fieldLength.GetValueOrDefault(DefaultStringLength));
            }

            throw new NotSupportedException(
                string.Format("Property of type: {0} is not supported", fieldType.FullName));
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
        public virtual string GetColumnDefinition(string fieldName, Type fieldType,
            bool isPrimaryKey, bool autoIncrement, bool isNullable,
            int? fieldLength, int? scale, string defaultValue)
        {
            string fieldDefinition;

            if (fieldType == typeof(string))
            {
                fieldDefinition = string.Format(StringLengthColumnDefinitionFormat, fieldLength.GetValueOrDefault(DefaultStringLength));
            }
            else
            {
                if (!DbTypeMap.ColumnTypeMap.TryGetValue(fieldType, out fieldDefinition))
                {
                    fieldDefinition = this.GetUndefinedColumnDefinition(fieldType, fieldLength);
                }
            }

            var sql = new StringBuilder();
            sql.AppendFormat("{0} {1}", GetQuotedColumnName(fieldName), fieldDefinition);

            if (isPrimaryKey)
            {
                sql.Append(" PRIMARY KEY");
                if (autoIncrement)
                {
                    sql.Append(" ").Append(AutoIncrementDefinition);
                }
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

        /// <summary>Gets or sets the select identity SQL.</summary>
        /// <value>The select identity SQL.</value>
        public virtual string SelectIdentitySql { get; set; }

        /// <summary>Gets the last insert identifier.</summary>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        /// <param name="dbCmd">The command.</param>
        /// <returns>The last insert identifier.</returns>
        public virtual long GetLastInsertId(IDbCommand dbCmd)
        {
            if (SelectIdentitySql == null)
                throw new NotImplementedException("Returning last inserted identity is not implemented on this DB Provider.");

            dbCmd.CommandText = SelectIdentitySql;
            return dbCmd.GetLongScalar();
        }

        /// <summary>Inserts an and get last insert identifier described by dbCmd.</summary>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <returns>A long.</returns>
        public virtual long InsertAndGetLastInsertId<T>(IDbCommand dbCmd)
        {
            if (SelectIdentitySql == null)
                throw new NotImplementedException("Returning last inserted identity is not implemented on this DB Provider.");
            
            dbCmd.CommandText += "; " + SelectIdentitySql;
            return dbCmd.GetLongScalar();
        }

        /// <summary>Converts this object to a count statement.</summary>
        /// <param name="fromTableType">Type of from table.</param>
        /// <param name="sqlFilter">    A filter specifying the SQL.</param>
        /// <param name="filterParams"> Options for controlling the filter.</param>
        /// <returns>The given data converted to a string.</returns>
        public virtual string ToCountStatement(Type fromTableType, string sqlFilter, params object[] filterParams)
        {
            var sql = new StringBuilder();
            const string SelectStatement = "SELECT ";
            var modelDef = fromTableType.GetModelDefinition();
            var isFullSelectStatement =
                !string.IsNullOrEmpty(sqlFilter)
                && sqlFilter.TrimStart().StartsWith(SelectStatement, StringComparison.OrdinalIgnoreCase);

            if (isFullSelectStatement) return (filterParams != null ? sqlFilter.SqlFormat(filterParams) : sqlFilter);

            sql.AppendFormat("SELECT {0} FROM {1}", "COUNT(*)",
                             GetQuotedTableName(modelDef));
            if (!string.IsNullOrEmpty(sqlFilter))
            {
                sqlFilter = filterParams != null ? sqlFilter.SqlFormat(filterParams) : sqlFilter;
                if ((!sqlFilter.StartsWith("ORDER ", StringComparison.InvariantCultureIgnoreCase)
                    && !sqlFilter.StartsWith("LIMIT ", StringComparison.InvariantCultureIgnoreCase))
                    && (!sqlFilter.StartsWith("WHERE ", StringComparison.InvariantCultureIgnoreCase)))
                {
                    sql.Append(" WHERE ");
                }
                sql.Append(" " + sqlFilter);
            }
            return sql.ToString();
        }

        /// <summary>Converts this object to a select statement.</summary>
        /// <param name="tableType">   Type of the table.</param>
        /// <param name="sqlFilter">   A filter specifying the SQL.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>The given data converted to a string.</returns>
        public virtual string ToSelectStatement(Type tableType, string sqlFilter, params object[] filterParams)
        {
            const string SelectStatement = "SELECT";
            var isFullSelectStatement =
                !string.IsNullOrEmpty(sqlFilter)
                && sqlFilter.TrimStart().StartsWith(SelectStatement, StringComparison.InvariantCultureIgnoreCase);

            if (isFullSelectStatement) 
                return sqlFilter.SqlFormat(filterParams);

            var modelDef = tableType.GetModelDefinition();
            var sql = new StringBuilder("SELECT " + tableType.GetColumnNames() + " FROM " + GetQuotedTableName(modelDef));

            if (!string.IsNullOrEmpty(sqlFilter))
            {
                sqlFilter = sqlFilter.SqlFormat(filterParams);
                if (!sqlFilter.StartsWith("ORDER ", StringComparison.InvariantCultureIgnoreCase)
                    && !sqlFilter.StartsWith("LIMIT ", StringComparison.InvariantCultureIgnoreCase))
                {
                    sql.Append(" WHERE ");
                }

                sql.Append(sqlFilter);
            }

            return sql.ToString();
        }

        /// <summary>Converts this object to an insert row statement.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="command">          The command.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="insertFields">     The insert fields.</param>
        /// <returns>The given data converted to a string.</returns>
        public virtual string ToInsertRowStatement(IDbCommand command, object objWithProperties, ICollection<string> insertFields = null)
        {
            if (insertFields == null) 
                insertFields = new List<string>();

            var sbColumnNames = new StringBuilder();
            var sbColumnValues = new StringBuilder();
            var modelDef = objWithProperties.GetType().GetModelDefinition();

            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (fieldDef.IsComputed) continue;
                if (fieldDef.AutoIncrement) continue;
                //insertFields contains Property "Name" of fields to insert ( that's how expressions work )
                if (insertFields.Count > 0 && !insertFields.Contains(fieldDef.Name)) continue;

                if (sbColumnNames.Length > 0) sbColumnNames.Append(",");
                if (sbColumnValues.Length > 0) sbColumnValues.Append(",");

                try
                {
                    sbColumnNames.Append(GetQuotedColumnName(fieldDef.FieldName));
                    sbColumnValues.Append(fieldDef.GetQuotedValue(objWithProperties));
                }
                catch (Exception ex)
                {
                    Log.Error("ERROR in ToInsertRowStatement(): " + ex.Message, ex);
                    throw;
                }
            }

            var sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                                    GetQuotedTableName(modelDef), sbColumnNames, sbColumnValues);

            return sql;
        }

        /// <summary>Creates parameterized insert statement.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="connection">       The connection.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="insertFields">     The insert fields.</param>
        /// <returns>The new parameterized insert statement.</returns>
        public virtual IDbCommand CreateParameterizedInsertStatement(IDbConnection connection, object objWithProperties, ICollection<string> insertFields = null)
        {
            if (insertFields == null) 
                insertFields = new List<string>();

            var sbColumnNames = new StringBuilder();
            var sbColumnValues = new StringBuilder();
            var modelDef = objWithProperties.GetType().GetModelDefinition();

            var cmd = connection.CreateCommand();
            cmd.CommandTimeout = OrmLiteConfig.CommandTimeout;

            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (fieldDef.IsComputed) continue;
                if (fieldDef.AutoIncrement)
                        continue;
                    
                //insertFields contains Property "Name" of fields to insert ( that's how expressions work )
                if (insertFields.Count > 0 && !insertFields.Contains(fieldDef.Name)) continue;

                if (sbColumnNames.Length > 0) sbColumnNames.Append(",");
                if (sbColumnValues.Length > 0) sbColumnValues.Append(",");

                try
                {
                    sbColumnNames.Append(GetQuotedColumnName(fieldDef.FieldName));
                    sbColumnValues.Append(ParamString)
                                  .Append(fieldDef.FieldName);

                    AddParameterForFieldToCommand(cmd, fieldDef, objWithProperties);
                }
                catch (Exception ex)
                {
                    Log.Error("ERROR in CreateParameterizedInsertStatement(): " + ex.Message, ex);
                    throw;
                }
            }

            cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                                            GetQuotedTableName(modelDef), sbColumnNames, sbColumnValues);

            return cmd;
        }

        /// <summary>Re parameterize insert statement.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="command">          The command.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="insertFields">     The insert fields.</param>
        public void ReParameterizeInsertStatement(IDbCommand command, object objWithProperties, ICollection<string> insertFields = null)
        {
            if (insertFields == null) 
                insertFields = new List<string>();

            var modelDef = objWithProperties.GetType().GetModelDefinition();
            
            command.Parameters.Clear();

            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (fieldDef.IsComputed) continue;
                if (fieldDef.AutoIncrement) continue;
                //insertFields contains Property "Name" of fields to insert ( that's how expressions work )
                if (insertFields.Count > 0 && !insertFields.Contains(fieldDef.Name)) continue;

                try
                {
                    AddParameterForFieldToCommand(command, fieldDef, objWithProperties);
                }
                catch (Exception ex)
                {
                    Log.Error("ERROR in ReParameterizeInsertStatement(): " + ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>Adds a parameter for field to command.</summary>
        /// <param name="command">          The command.</param>
        /// <param name="fieldDef">         The field definition.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        protected virtual void AddParameterForFieldToCommand(IDbCommand command, FieldDefinition fieldDef, object objWithProperties)
        {
            var p = command.CreateParameter();
            p.ParameterName = string.Format("{0}{1}", ParamString, fieldDef.FieldName);

            if (DbTypeMap.ColumnDbTypeMap.ContainsKey(fieldDef.FieldType))
            {
                p.DbType = DbTypeMap.ColumnDbTypeMap[fieldDef.FieldType];
                p.Value = GetValueOrDbNull(fieldDef, objWithProperties);
            }
            else
            {
                p.DbType = DbType.String;
                p.Value = GetQuotedValueOrDbNull(fieldDef, objWithProperties);
            }

            command.Parameters.Add(p);
        }

        /// <summary>Gets value or database null.</summary>
        /// <param name="fieldDef">         The field definition.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <returns>The value or database null.</returns>
        protected object GetValueOrDbNull(FieldDefinition fieldDef, object objWithProperties)
        {
            return fieldDef.GetValue(objWithProperties) ?? DBNull.Value;
        }

        /// <summary>Gets quoted value or database null.</summary>
        /// <param name="fieldDef">         The field definition.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <returns>The quoted value or database null.</returns>
        protected object GetQuotedValueOrDbNull(FieldDefinition fieldDef, object objWithProperties)
        {
            var value = fieldDef.GetValue(objWithProperties);

            if (value == null)
                return DBNull.Value;

            var unquotedVal = OrmLiteConfig.DialectProvider.GetQuotedValue(value, fieldDef.FieldType)
                .TrimStart('\'').TrimEnd('\''); ;

            if (string.IsNullOrEmpty(unquotedVal))
                return DBNull.Value;

            return unquotedVal;
        }

        /// <summary>Converts this object to an update row statement.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="updateFields">     The update fields.</param>
        /// <returns>The given data converted to a string.</returns>
        public virtual string ToUpdateRowStatement(object objWithProperties, ICollection<string> updateFields = null)
        {
            if (updateFields == null) 
                updateFields = new List<string>();

            var sqlFilter = new StringBuilder();
            var sql = new StringBuilder();
            var modelDef = objWithProperties.GetType().GetModelDefinition();

            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (fieldDef.IsComputed) continue;

                try
                {
                    if (fieldDef.IsPrimaryKey && updateFields.Count == 0)
                    {
                        if (sqlFilter.Length > 0) sqlFilter.Append(" AND ");

                        sqlFilter.AppendFormat("{0} = {1}", GetQuotedColumnName(fieldDef.FieldName), fieldDef.GetQuotedValue(objWithProperties));

                        continue;
                    }

                    if (updateFields.Count > 0 && !updateFields.Contains(fieldDef.Name) || fieldDef.AutoIncrement) continue;
                    if (sql.Length > 0) sql.Append(",");
                    sql.AppendFormat("{0} = {1}", GetQuotedColumnName(fieldDef.FieldName), fieldDef.GetQuotedValue(objWithProperties));
                }
                catch (Exception ex)
                {
                    Log.Error("ERROR in ToUpdateRowStatement(): " + ex.Message, ex);
                }
            }

            var updateSql = string.Format("UPDATE {0} SET {1}{2}",
                GetQuotedTableName(modelDef), sql, (sqlFilter.Length > 0 ? " WHERE " + sqlFilter : ""));

            if (sql.Length == 0)
                throw new Exception("No valid update properties provided (e.g. p => p.FirstName): " + updateSql);
            
            return updateSql;
        }

        /// <summary>Creates parameterized update statement.</summary>
        /// <param name="connection">       The connection.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="updateFields">     The update fields.</param>
        /// <returns>The new parameterized update statement.</returns>
        public virtual IDbCommand CreateParameterizedUpdateStatement(IDbConnection connection, object objWithProperties, ICollection<string> updateFields = null)
        {
            if (updateFields == null) 
                updateFields = new List<string>();

            var sqlFilter = new StringBuilder();
            var sql = new StringBuilder();
            var modelDef = objWithProperties.GetType().GetModelDefinition();

            var command = connection.CreateCommand();
            command.CommandTimeout = OrmLiteConfig.CommandTimeout;
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (fieldDef.IsComputed) continue;
                try
                {
                    if (fieldDef.IsPrimaryKey && updateFields.Count == 0)
                    {
                        if (sqlFilter.Length > 0) sqlFilter.Append(" AND ");

                        sqlFilter.AppendFormat("{0} = {1}", GetQuotedColumnName(fieldDef.FieldName), String.Concat(ParamString, fieldDef.FieldName));
                        AddParameterForFieldToCommand(command, fieldDef, objWithProperties);

                        continue;
                    }

                    if (updateFields.Count > 0 && !updateFields.Contains(fieldDef.Name)) continue;
                    if (sql.Length > 0) sql.Append(",");
                    sql.AppendFormat("{0} = {1}", GetQuotedColumnName(fieldDef.FieldName), String.Concat(ParamString, fieldDef.FieldName));

                    AddParameterForFieldToCommand(command, fieldDef, objWithProperties);
                }
                catch (Exception ex)
                {
                    Log.Error("ERROR in CreateParameterizedUpdateStatement(): " + ex.Message, ex);
                }
            }

            command.CommandText = string.Format("UPDATE {0} SET {1} {2}", GetQuotedTableName(modelDef), sql, (sqlFilter.Length > 0 ? "WHERE " + sqlFilter : ""));
            return command;
        }

        /// <summary>Converts the objWithProperties to a delete row statement.</summary>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <returns>objWithProperties as a string.</returns>
        public virtual string ToDeleteRowStatement(object objWithProperties)
        {
            var sqlFilter = new StringBuilder();
            var modelDef = objWithProperties.GetType().GetModelDefinition();

            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                try
                {
                    if (fieldDef.IsPrimaryKey)
                    {
                        if (sqlFilter.Length > 0) sqlFilter.Append(" AND ");

                        sqlFilter.AppendFormat("{0} = {1}", GetQuotedColumnName(fieldDef.FieldName), fieldDef.GetQuotedValue(objWithProperties));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("ERROR in ToDeleteRowStatement(): " + ex.Message, ex);
                }
            }

            var deleteSql = string.Format("DELETE FROM {0} WHERE {1}",
                GetQuotedTableName(modelDef), sqlFilter);

            return deleteSql;
        }

        /// <summary>Converts this object to a delete statement.</summary>
        /// <param name="tableType">   Type of the table.</param>
        /// <param name="sqlFilter">   A filter specifying the SQL.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>The given data converted to a string.</returns>
        public virtual string ToDeleteStatement(Type tableType, string sqlFilter, params object[] filterParams)
        {
            var sql = new StringBuilder();
            const string deleteStatement = "DELETE ";

            var isFullDeleteStatement =
                !string.IsNullOrEmpty(sqlFilter)
                && sqlFilter.Length > deleteStatement.Length
                && sqlFilter.Substring(0, deleteStatement.Length).ToUpper().Equals(deleteStatement);

            if (isFullDeleteStatement) return sqlFilter.SqlFormat(filterParams);

            var modelDef = tableType.GetModelDefinition();
            sql.AppendFormat("DELETE FROM {0}", GetQuotedTableName(modelDef));
            if (!string.IsNullOrEmpty(sqlFilter))
            {
                sqlFilter = sqlFilter.SqlFormat(filterParams);
                sql.Append(" WHERE ");
                sql.Append(sqlFilter);
            }

            return sql.ToString();
        }

        /// <summary>Converts a tableType to a create table statement.</summary>
        /// <param name="tableType">Type of the table.</param>
        /// <returns>tableType as a string.</returns>
        public virtual string ToCreateTableStatement(Type tableType)
        {
            var sbColumns = new StringBuilder();
            var sbConstraints = new StringBuilder();

            var modelDef = tableType.GetModelDefinition();
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (sbColumns.Length != 0) sbColumns.Append(", \n  ");

                var columnDefinition = GetColumnDefinition(
                    fieldDef.FieldName,
                    fieldDef.FieldType,
                    fieldDef.IsPrimaryKey,
                    fieldDef.AutoIncrement,
                    fieldDef.IsNullable,
                    fieldDef.FieldLength,
                    null,
                    fieldDef.DefaultValue);

                sbColumns.Append(columnDefinition);

                if (fieldDef.ForeignKey == null) continue;

                var refModelDef = fieldDef.ForeignKey.ReferenceType.GetModelDefinition();
                sbConstraints.AppendFormat(
                    ", \n\n  CONSTRAINT {0} FOREIGN KEY ({1}) REFERENCES {2} ({3})",
                    GetQuotedName(fieldDef.ForeignKey.GetForeignKeyName(modelDef, refModelDef, NamingStrategy, fieldDef)),
                    GetQuotedColumnName(fieldDef.FieldName),
                    GetQuotedTableName(refModelDef),
                    GetQuotedColumnName(refModelDef.PrimaryKey.FieldName));

                sbConstraints.Append(GetForeignKeyOnDeleteClause(fieldDef.ForeignKey));
                sbConstraints.Append(GetForeignKeyOnUpdateClause(fieldDef.ForeignKey));
            }
            var sql = new StringBuilder(string.Format(
                "CREATE TABLE {0} \n(\n  {1}{2} \n); \n", GetQuotedTableName(modelDef), sbColumns, sbConstraints));

            return sql.ToString();
        }

        /// <summary>Gets foreign key on delete clause.</summary>
        /// <param name="foreignKey">The foreign key.</param>
        /// <returns>The foreign key on delete clause.</returns>
        public virtual string GetForeignKeyOnDeleteClause(ForeignKeyConstraint foreignKey)
        {
            return !string.IsNullOrEmpty(foreignKey.OnDelete) ? " ON DELETE " + foreignKey.OnDelete : "";
        }

        /// <summary>Gets foreign key on update clause.</summary>
        /// <param name="foreignKey">The foreign key.</param>
        /// <returns>The foreign key on update clause.</returns>
        public virtual string GetForeignKeyOnUpdateClause(ForeignKeyConstraint foreignKey)
        {
            return !string.IsNullOrEmpty(foreignKey.OnUpdate) ? " ON UPDATE " + foreignKey.OnUpdate : "";
        }

        /// <summary>Converts a tableType to a create index statements.</summary>
        /// <param name="tableType">Type of the table.</param>
        /// <returns>tableType as a List&lt;string&gt;</returns>
        public virtual List<string> ToCreateIndexStatements(Type tableType)
        {
            var sqlIndexes = new List<string>();

            var modelDef = tableType.GetModelDefinition();
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (!fieldDef.IsIndexed) continue;

                var indexName = GetIndexName(fieldDef.IsUnique, modelDef.ModelName.SafeVarName(), fieldDef.FieldName);

                sqlIndexes.Add(
                    ToCreateIndexStatement(fieldDef.IsUnique, indexName, modelDef, fieldDef.FieldName));
            }

            foreach (var compositeIndex in modelDef.CompositeIndexes)
            {
                var indexName = GetCompositeIndexName(compositeIndex, modelDef);
                var indexNames = string.Join(" ASC, ",
                                             compositeIndex.FieldNames.ConvertAll(
                                                 n => GetQuotedName(n)).ToArray());

                sqlIndexes.Add(
                    ToCreateIndexStatement(compositeIndex.Unique, indexName, modelDef, indexNames, true));
            }

            return sqlIndexes;
        }

        /// <summary>Gets column database type.</summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>The column database type.</returns>
        public virtual DbType GetColumnDbType(Type valueType)
        {
            if (valueType.IsEnum)
                return DbTypeMap.ColumnDbTypeMap[typeof(string)];

            return DbTypeMap.ColumnDbTypeMap[valueType];
        }

        /// <summary>Gets column type definition.</summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns>The column type definition.</returns>
        public virtual string GetColumnTypeDefinition(Type fieldType)
        {
            string fieldDefinition;
            DbTypeMap.ColumnTypeMap.TryGetValue(fieldType, out fieldDefinition);
            return fieldDefinition ?? GetUndefinedColumnDefinition(fieldType, null);
        }

        /// <summary>Query if 'dbCmd' does table exist.</summary>
        /// <param name="db">       The database.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public virtual bool DoesTableExist(IDbConnection db, string tableName)
        {
            return db.Exec(dbCmd => DoesTableExist(dbCmd, tableName));
        }

        /// <summary>Query if 'dbCmd' does table exist.</summary>
        /// <param name="dbCmd">    The database command.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public virtual bool DoesTableExist(IDbCommand dbCmd, string tableName)
        {
            return false;
        }

        /// <summary>Query if 'dbCmd' does sequence exist.</summary>
        /// <param name="dbCmd">       The database command.</param>
        /// <param name="sequenceName">Name of the sequenc.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public virtual bool DoesSequenceExist(IDbCommand dbCmd, string sequenceName)
        {
            return true;
        }

        /// <summary>Gets index name.</summary>
        /// <param name="isUnique"> true if this object is unique.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The index name.</returns>
        protected virtual string GetIndexName(bool isUnique, string modelName, string fieldName)
        {
            return string.Format("{0}idx_{1}_{2}", isUnique ? "u" : "", modelName, fieldName).ToLower();
        }

        /// <summary>Gets composite index name.</summary>
        /// <param name="compositeIndex">Zero-based index of the composite.</param>
        /// <param name="modelDef">      The model definition.</param>
        /// <returns>The composite index name.</returns>
        protected virtual string GetCompositeIndexName(CompositeIndexAttribute compositeIndex, ModelDefinition modelDef)
        {
            return compositeIndex.Name ?? GetIndexName(compositeIndex.Unique, modelDef.ModelName.SafeVarName(),
                                                       string.Join("_", compositeIndex.FieldNames.ToArray()));
        }

        /// <summary>Gets composite index name with schema.</summary>
        /// <param name="compositeIndex">Zero-based index of the composite.</param>
        /// <param name="modelDef">      The model definition.</param>
        /// <returns>The composite index name with schema.</returns>
        protected virtual string GetCompositeIndexNameWithSchema(CompositeIndexAttribute compositeIndex, ModelDefinition modelDef)
        {
            return compositeIndex.Name ?? GetIndexName(compositeIndex.Unique,
                    (modelDef.IsInSchema ?
                        modelDef.Schema + "_" + GetQuotedTableName(modelDef) :
                        GetQuotedTableName(modelDef)).SafeVarName(),
                    string.Join("_", compositeIndex.FieldNames.ToArray()));
        }

        /// <summary>Converts this object to a create index statement.</summary>
        /// <param name="isUnique">  true if this object is unique.</param>
        /// <param name="indexName"> Name of the index.</param>
        /// <param name="modelDef">  The model definition.</param>
        /// <param name="fieldName"> Name of the field.</param>
        /// <param name="isCombined">true if this object is combined.</param>
        /// <returns>The given data converted to a string.</returns>
        protected virtual string ToCreateIndexStatement(bool isUnique, string indexName, ModelDefinition modelDef, string fieldName, bool isCombined = false)
        {
            return string.Format("CREATE {0} INDEX {1} ON {2} ({3} ASC); \n",
                                 isUnique ? "UNIQUE" : "", indexName,
                                 GetQuotedTableName(modelDef),
                                 (isCombined) ? fieldName : GetQuotedColumnName(fieldName));
        }

        /// <summary>Gets column names.</summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>The column names.</returns>
        public virtual string GetColumnNames(ModelDefinition modelDef)
        {
            return modelDef.GetColumnNames();
        }

        /// <summary>Converts a tableType to a create sequence statements.</summary>
        /// <param name="tableType">Type of the table.</param>
        /// <returns>tableType as a List&lt;string&gt;</returns>
        public virtual List<string> ToCreateSequenceStatements(Type tableType)
        {
            return new List<string>();
        }

        /// <summary>Converts this object to a create sequence statement.</summary>
        /// <param name="tableType">   Type of the table.</param>
        /// <param name="sequenceName">Name of the sequence.</param>
        /// <returns>The given data converted to a string.</returns>
        public virtual string ToCreateSequenceStatement(Type tableType, string sequenceName)
        {
            return "";
        }        

        /// <summary>Sequence list.</summary>
        /// <param name="tableType">Type of the table.</param>
        /// <returns>A List&lt;string&gt;</returns>
        public virtual List<string> SequenceList(Type tableType)
        {
            return new List<string>();
        }

        /// <summary>TODO : make abstract  ??</summary>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        /// <param name="fromTableType">    Type of from table.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="sqlFilter">        A filter specifying the SQL.</param>
        /// <param name="filterParams">     Options for controlling the filter.</param>
        /// <returns>The given data converted to a string.</returns>
        public virtual string ToExistStatement(Type fromTableType,
            object objWithProperties,
            string sqlFilter,
            params object[] filterParams)
        {
            throw new NotImplementedException();
        }

        /// <summary>TODO : make abstract  ??</summary>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        /// <param name="fromObjWithProperties">from object with properties.</param>
        /// <param name="outputModelType">      Type of the output model.</param>
        /// <param name="sqlFilter">            A filter specifying the SQL.</param>
        /// <param name="filterParams">         Options for controlling the filter.</param>
        /// <returns>The given data converted to a string.</returns>
        public virtual string ToSelectFromProcedureStatement(
            object fromObjWithProperties,
            Type outputModelType,
            string sqlFilter,
            params object[] filterParams)
        {
            throw new NotImplementedException();
        }

        /// <summary>TODO : make abstract  ??</summary>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <returns>objWithProperties as a string.</returns>
        public virtual string ToExecuteProcedureStatement(object objWithProperties)
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets a model.</summary>
        /// <param name="modelType">Type of the model.</param>
        /// <returns>The model.</returns>
        protected static ModelDefinition GetModel(Type modelType)
        {
            return modelType.GetModelDefinition();
        }

        /// <summary>Expression visitor.</summary>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> ExpressionVisitor<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>Creates parameterized delete statement.</summary>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        /// <param name="connection">       The connection.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <returns>The new parameterized delete statement.</returns>
        public IDbCommand CreateParameterizedDeleteStatement(IDbConnection connection, object objWithProperties)
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets drop foreign key constraints.</summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>The drop foreign key constraints.</returns>
        public virtual string GetDropForeignKeyConstraints(ModelDefinition modelDef)
        {
            return null;
        }

        /// <summary>Gets model definition.</summary>
        /// <param name="modelType">Type of the model.</param>
        /// <returns>The model definition.</returns>
        public static ModelDefinition GetModelDefinition(Type modelType)
        {
            return modelType.GetModelDefinition();
        }

		#region DDL
        /// <summary>Converts this object to an add column statement.</summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef"> The field definition.</param>
        /// <returns>The given data converted to a string.</returns>
		public virtual string ToAddColumnStatement(Type modelType, FieldDefinition fieldDef)
		{

			var column = GetColumnDefinition(fieldDef.FieldName,
			                                 fieldDef.FieldType,
			                                 fieldDef.IsPrimaryKey,
			                                 fieldDef.AutoIncrement,
			                                 fieldDef.IsNullable,
			                                 fieldDef.FieldLength,
			                                 fieldDef.Scale,
			                                 fieldDef.DefaultValue);
			return string.Format("ALTER TABLE {0} ADD COLUMN {1};",
			                     GetQuotedTableName(modelType.GetModelDefinition().ModelName),
			                     column);
		}

        /// <summary>Converts this object to an alter column statement.</summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef"> The field definition.</param>
        /// <returns>The given data converted to a string.</returns>
		public virtual string ToAlterColumnStatement(Type modelType, FieldDefinition fieldDef)
		{		
			var column = GetColumnDefinition(fieldDef.FieldName,
			                                 fieldDef.FieldType,
			                                 fieldDef.IsPrimaryKey,
			                                 fieldDef.AutoIncrement,
			                                 fieldDef.IsNullable,
			                                 fieldDef.FieldLength,
			                                 fieldDef.Scale,
			                                 fieldDef.DefaultValue);
			return string.Format("ALTER TABLE {0} MODIFY COLUMN {1};",
			                     GetQuotedTableName(modelType.GetModelDefinition().ModelName),
			                     column);
		}

        /// <summary>Converts this object to a change column name statement.</summary>
        /// <param name="modelType">    Type of the model.</param>
        /// <param name="fieldDef">     The field definition.</param>
        /// <param name="oldColumnName">Name of the old column.</param>
        /// <returns>The given data converted to a string.</returns>
		public virtual string ToChangeColumnNameStatement(Type modelType,
		                                                  FieldDefinition fieldDef,
		                                                  string oldColumnName)
		{
			var column = GetColumnDefinition(fieldDef.FieldName,
			                                 fieldDef.FieldType,
			                                 fieldDef.IsPrimaryKey,
			                                 fieldDef.AutoIncrement,
			                                 fieldDef.IsNullable,
			                                 fieldDef.FieldLength,
			                                 fieldDef.Scale,
			                                 fieldDef.DefaultValue);
			return string.Format("ALTER TABLE {0} CHANGE COLUMN {1} {2};",
			                     GetQuotedTableName(modelType.GetModelDefinition().ModelName),
			                     GetQuotedColumnName(oldColumnName),
			                     column);
		}

        /// <summary>Converts this object to an add foreign key statement.</summary>
        /// <typeparam name="T">       Generic type parameter.</typeparam>
        /// <typeparam name="TForeign">Type of the foreign.</typeparam>
        /// <param name="field">         The field.</param>
        /// <param name="foreignField">  The foreign field.</param>
        /// <param name="onUpdate">      The on update.</param>
        /// <param name="onDelete">      The on delete.</param>
        /// <param name="foreignKeyName">Name of the foreign key.</param>
        /// <returns>The given data converted to a string.</returns>
		public virtual string  ToAddForeignKeyStatement<T,TForeign>(Expression<Func<T,object>> field,
		                                                            Expression<Func<TForeign,object>> foreignField,
		                                                            OnFkOption onUpdate,
		                                                            OnFkOption onDelete,
		                                                            string foreignKeyName=null){
			var sourceMD = ModelDefinition<T>.Definition;
			var fieldName = sourceMD.GetFieldDefinition (field).FieldName; 
						
			var referenceMD=ModelDefinition<TForeign>.Definition;
			var referenceFieldName= referenceMD.GetFieldDefinition(foreignField).FieldName;
			
			string name = GetQuotedName(foreignKeyName.IsNullOrEmpty()?
			                            "fk_"+sourceMD.ModelName+"_"+ fieldName+"_"+referenceFieldName:
			                            foreignKeyName);
			
			return string.Format("ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ({2}) REFERENCES {3} ({4}){5}{6};",
			                     GetQuotedTableName(sourceMD.ModelName),
			                     name,
			                     GetQuotedColumnName(fieldName),
			                     GetQuotedTableName(referenceMD.ModelName),
			                     GetQuotedColumnName(referenceFieldName),
			                     GetForeignKeyOnDeleteClause(new ForeignKeyConstraint(typeof(T), onDelete: FkOptionToString( onDelete))),
			                     GetForeignKeyOnUpdateClause(new ForeignKeyConstraint(typeof(T), onUpdate: FkOptionToString(onUpdate))));	
		}

        /// <summary>Converts this object to a create index statement.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="field">    The field.</param>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="unique">   true to unique.</param>
        /// <returns>The given data converted to a string.</returns>
		public virtual string ToCreateIndexStatement<T>(Expression<Func<T,object>> field,
		                                                string indexName=null, bool unique=false)
		{
			
			var sourceMD = ModelDefinition<T>.Definition;
			var fieldName = sourceMD.GetFieldDefinition (field).FieldName;
			
			string name =GetQuotedName(indexName.IsNullOrEmpty()?
			                           (unique?"uidx":"idx") +"_"+sourceMD.ModelName+"_"+fieldName:
			                           indexName);
			
			string command = string.Format("CREATE{0}INDEX {1} ON {2}({3});",
			                               unique?" UNIQUE ": " ",
			                               name,
			                               GetQuotedTableName(sourceMD.ModelName),
			                               GetQuotedColumnName(fieldName)
			                               );
			return command;
		}

        /// <summary>Fk option to string.</summary>
        /// <param name="option">The option.</param>
        /// <returns>A string.</returns>
		protected virtual string FkOptionToString(OnFkOption option){
			switch(option){
			case OnFkOption.Cascade: return "CASCADE";
			case OnFkOption.NoAction: return "NO ACTION"; 
			case OnFkOption.SetNull: return "SET NULL"; 
			case OnFkOption.SetDefault: return "SET DEFAULT";
			case OnFkOption.Restrict:
			default: return "RESTRICT";
			}
		}

		#endregion DDL

    }
}