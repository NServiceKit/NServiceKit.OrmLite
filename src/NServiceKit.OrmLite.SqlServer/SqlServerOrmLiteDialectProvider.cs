using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.SqlServer 
{
    /// <summary>A SQL server ORM lite dialect provider.</summary>
    public class SqlServerOrmLiteDialectProvider : OrmLiteDialectProviderBase<SqlServerOrmLiteDialectProvider>
	{
        /// <summary>The instance.</summary>
		public static SqlServerOrmLiteDialectProvider Instance = new SqlServerOrmLiteDialectProvider();

        /// <summary>The time span offset.</summary>
		private static readonly DateTime timeSpanOffset = new DateTime(1900,01,01);

        /// <summary>The date time offset column definition.</summary>
        private const string DateTimeOffsetColumnDefinition = "DATETIMEOFFSET";

        /// <summary>
        /// Initializes a new instance of the
        /// NServiceKit.OrmLite.SqlServer.SqlServerOrmLiteDialectProvider class.
        /// </summary>
		public SqlServerOrmLiteDialectProvider()
		{
			base.AutoIncrementDefinition = "IDENTITY(1,1)";
			StringColumnDefinition = UseUnicode ?  "NVARCHAR(4000)" : "VARCHAR(8000)";
			base.GuidColumnDefinition = "UniqueIdentifier";
			base.RealColumnDefinition = "FLOAT";
		    base.BoolColumnDefinition = "BIT";
			base.DecimalColumnDefinition = "DECIMAL(38,6)";
			base.TimeColumnDefinition = "TIME"; //SQLSERVER 2008+
		    base.BlobColumnDefinition = "VARBINARY(MAX)";
		    base.SelectIdentitySql = "SELECT SCOPE_IDENTITY()";

			base.InitColumnTypeMap();

            // add support for DateTimeOffset
            DbTypeMap.Set<DateTimeOffset>(DbType.DateTimeOffset, DateTimeOffsetColumnDefinition);
            DbTypeMap.Set<DateTimeOffset?>(DbType.DateTimeOffset, DateTimeOffsetColumnDefinition);
		}

        /// <summary>Gets quoted parameter.</summary>
        /// <param name="paramValue">The parameter value.</param>
        /// <returns>The quoted parameter.</returns>
        public override string GetQuotedParam(string paramValue)
        {
            return (UseUnicode ? "N'" : "'") + paramValue.Replace("'", "''") + "'";
        }

        /// <summary>Creates a connection.</summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="options">         Options for controlling the operation.</param>
        /// <returns>The new connection.</returns>
		public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
		{
			var isFullConnectionString = connectionString.Contains(";");

			if (!isFullConnectionString)
			{
				var filePath = connectionString;

				var filePathWithExt = filePath.ToLower().EndsWith(".mdf")
					? filePath 
					: filePath + ".mdf";

				var fileName = Path.GetFileName(filePathWithExt);
				var dbName = fileName.Substring(0, fileName.Length - ".mdf".Length);

				connectionString = string.Format(
				@"Data Source=.\SQLEXPRESS;AttachDbFilename={0};Initial Catalog={1};Integrated Security=True;User Instance=True;",
					filePathWithExt, dbName);
			}

			if (options != null)
			{
				foreach (var option in options)
				{
					if (option.Key.ToLower() == "read only")
					{
						if (option.Value.ToLower() == "true")
						{
							connectionString += "Mode = Read Only;";
						}
						continue;
					}
					connectionString += option.Key + "=" + option.Value + ";";
				}
			}

			return new SqlConnection(connectionString);
		}

        /// <summary>Gets quoted table name.</summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>The quoted table name.</returns>
        public override string GetQuotedTableName(ModelDefinition modelDef)
        {
            if (!modelDef.IsInSchema)
                return base.GetQuotedTableName(modelDef);

            var escapedSchema = modelDef.Schema.Replace(".", "\".\"");
            return string.Format("\"{0}\".\"{1}\"", escapedSchema, NamingStrategy.GetTableName(modelDef.ModelName));
        }

        /// <summary>Convert database value.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="value">The value.</param>
        /// <param name="type"> The type.</param>
        /// <returns>The database converted value.</returns>
		public override object ConvertDbValue(object value, Type type)
		{
			try
			{
				if (value == null || value is DBNull) return null;

				if (type == typeof(bool) && !(value is bool))
				{
					var intVal = Convert.ToInt32(value.ToString());
					return intVal != 0;
				}

				if (type == typeof(TimeSpan) && value is DateTime)
				{
					var dateTimeValue = (DateTime)value;
					return dateTimeValue - timeSpanOffset;
				}

                if (_ensureUtc && type == typeof (DateTime))
                {
                    var result = base.ConvertDbValue(value, type);
                    if(result is DateTime)
                        return DateTime.SpecifyKind((DateTime)result, DateTimeKind.Utc);
                    return result;
                }

                if (type == typeof(byte[]))
                    return value;

				return base.ConvertDbValue(value, type);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

        /// <summary>Gets quoted value.</summary>
        /// <param name="value">    The value.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns>The quoted value.</returns>
		public override string  GetQuotedValue(object value, Type fieldType)
		{
			if (value == null) return "NULL";

			if (fieldType == typeof(Guid))
			{
				var guidValue = (Guid)value;
				return string.Format("CAST('{0}' AS UNIQUEIDENTIFIER)", guidValue);
			}
			if (fieldType == typeof(DateTime))
			{
                var dateValue = (DateTime)value;
			    if (_ensureUtc && dateValue.Kind == DateTimeKind.Local)
			        dateValue = dateValue.ToUniversalTime(); 
				const string iso8601Format = "yyyyMMdd HH:mm:ss.fff";
				return base.GetQuotedValue(dateValue.ToString(iso8601Format,CultureInfo.InvariantCulture) , typeof(string));
			}
            if (fieldType == typeof(DateTimeOffset))
            {
                var dateValue = (DateTimeOffset)value;
                const string iso8601Format = "yyyyMMdd HH:mm:ss.fff zzz";
                return base.GetQuotedValue(dateValue.ToString(iso8601Format, CultureInfo.InvariantCulture), typeof(string));
            }
			if (fieldType == typeof(bool))
			{
				var boolValue = (bool)value;
				return base.GetQuotedValue(boolValue ? 1 : 0, typeof(int));
			}
			if(fieldType == typeof(string)) {
                return GetQuotedParam(value.ToString());
			}

            if (fieldType == typeof(byte[]))
            {
                return "0x" + BitConverter.ToString((byte[])value).Replace("-", "");
            }

			return base.GetQuotedValue(value, fieldType);


		}

        /// <summary>true to use date time 2.</summary>
		protected bool _useDateTime2;

        /// <summary>Use datetime 2.</summary>
        /// <param name="shouldUseDatetime2">true if should use datetime 2.</param>
		public void UseDatetime2(bool shouldUseDatetime2)
		{
			_useDateTime2 = shouldUseDatetime2;
			DateTimeColumnDefinition = shouldUseDatetime2 ? "datetime2" : "datetime";
			base.DbTypeMap.Set<DateTime>(shouldUseDatetime2 ? DbType.DateTime2 : DbType.DateTime, DateTimeColumnDefinition);
			base.DbTypeMap.Set<DateTime?>(shouldUseDatetime2 ? DbType.DateTime2 : DbType.DateTime, DateTimeColumnDefinition);
		}

        /// <summary>true to ensure UTC.</summary>
		protected bool _ensureUtc;

        /// <summary>Ensures that UTC.</summary>
        /// <param name="shouldEnsureUtc">true if should ensure UTC.</param>
		public void EnsureUtc(bool shouldEnsureUtc)
		{
		    _ensureUtc = shouldEnsureUtc;
		}

        /// <summary>Expression visitor.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
		public override SqlExpressionVisitor<T> ExpressionVisitor<T>()
		{
			return new SqlServerExpressionVisitor<T>();
		}

        /// <summary>Query if 'dbCmd' does table exist.</summary>
        /// <param name="dbCmd">    The database command.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
		public override bool DoesTableExist(IDbCommand dbCmd, string tableName)
		{
			var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}"
				.SqlFormat(tableName);

			//if (!string.IsNullOrEmpty(schemaName))
			//    sql += " AND TABLE_SCHEMA = {0}".SqlFormat(schemaName);

			dbCmd.CommandText = sql;
			var result = dbCmd.GetLongScalar();
			
			return result > 0;
		}

        /// <summary>Gets or sets a value indicating whether this object use unicode.</summary>
        /// <value>true if use unicode, false if not.</value>
        public override bool UseUnicode
        {
            get
            {
                return useUnicode;
            }
            set
            {
                useUnicode = value;
                if (useUnicode && this.DefaultStringLength > 4000)
                {
                    this.DefaultStringLength = 4000;
                }

                // UpdateStringColumnDefinitions(); is called by changing DefaultStringLength 
            }
        }

        /// <summary>Gets foreign key on delete clause.</summary>
        /// <param name="foreignKey">The foreign key.</param>
        /// <returns>The foreign key on delete clause.</returns>
        public override string GetForeignKeyOnDeleteClause(ForeignKeyConstraint foreignKey)
        {
            return "RESTRICT" == (foreignKey.OnDelete ?? "").ToUpper()
                ? ""
                : base.GetForeignKeyOnDeleteClause(foreignKey);
        }

        /// <summary>Gets foreign key on update clause.</summary>
        /// <param name="foreignKey">The foreign key.</param>
        /// <returns>The foreign key on update clause.</returns>
        public override string GetForeignKeyOnUpdateClause(ForeignKeyConstraint foreignKey)
        {
            return "RESTRICT" == (foreignKey.OnDelete ?? "").ToUpper()
                ? ""
                : base.GetForeignKeyOnUpdateClause(foreignKey);
        }

        /// <summary>Gets drop foreign key constraints.</summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>The drop foreign key constraints.</returns>
        public override string GetDropForeignKeyConstraints(ModelDefinition modelDef)
        {
            //TODO: find out if this should go in base class?

            var sb = new StringBuilder();
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (fieldDef.ForeignKey != null)
                {
                    var foreignKeyName = fieldDef.ForeignKey.GetForeignKeyName(
                        modelDef,
                        GetModelDefinition(fieldDef.ForeignKey.ReferenceType),
                        NamingStrategy,
                        fieldDef);

                    var tableName = GetQuotedTableName(modelDef);
                    sb.AppendLine("IF EXISTS (SELECT name FROM sys.foreign_keys WHERE name = '{0}')".Fmt(foreignKeyName));
                    sb.AppendLine("BEGIN");
                    sb.AppendLine("  ALTER TABLE {0} DROP {1};".Fmt(tableName, foreignKeyName));
                    sb.AppendLine("END");
                }
            }

            return sb.ToString();
        }

        /// <summary>Converts this object to an add column statement.</summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef"> The field definition.</param>
        /// <returns>The given data converted to a string.</returns>
        public override string ToAddColumnStatement(Type modelType, FieldDefinition fieldDef)
        {
            var column = GetColumnDefinition(fieldDef.FieldName,
                                             fieldDef.FieldType,
                                             fieldDef.IsPrimaryKey,
                                             fieldDef.AutoIncrement,
                                             fieldDef.IsNullable,
                                             fieldDef.FieldLength,
                                             fieldDef.Scale,
                                             fieldDef.DefaultValue);

            return string.Format("ALTER TABLE {0} ADD {1};",
                                 GetQuotedTableName(GetModel(modelType).ModelName),
                                 column);
        }

        /// <summary>Converts this object to an alter column statement.</summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef"> The field definition.</param>
        /// <returns>The given data converted to a string.</returns>
        public override string ToAlterColumnStatement(Type modelType, FieldDefinition fieldDef)
        {
            var column = GetColumnDefinition(fieldDef.FieldName,
                                             fieldDef.FieldType,
                                             fieldDef.IsPrimaryKey,
                                             fieldDef.AutoIncrement,
                                             fieldDef.IsNullable,
                                             fieldDef.FieldLength,
                                             fieldDef.Scale,
                                             fieldDef.DefaultValue);

            return string.Format("ALTER TABLE {0} ALTER COLUMN {1};",
                                 GetQuotedTableName(GetModel(modelType).ModelName),
                                 column);
        }

        /// <summary>Converts this object to a change column name statement.</summary>
        /// <param name="modelType">    Type of the model.</param>
        /// <param name="fieldDef">     The field definition.</param>
        /// <param name="oldColumnName">Name of the old column.</param>
        /// <returns>The given data converted to a string.</returns>
        public override string ToChangeColumnNameStatement(Type modelType, FieldDefinition fieldDef, string oldColumnName)
        {
            var objectName = string.Format("{0}.{1}",
                NamingStrategy.GetTableName(GetModel(modelType).ModelName),
                oldColumnName);

            return string.Format("EXEC sp_rename {0}, {1}, {2};",
                                 GetQuotedParam(objectName),
                                 GetQuotedParam(fieldDef.FieldName),
                                 GetQuotedParam("COLUMN"));
        }
    }
}
