using System;
using System.Data;
using System.Linq.Expressions;

namespace NServiceKit.OrmLite
{
    /// <summary>Values that represent OnFkOption.</summary>
	public enum  OnFkOption{

        /// <summary>An enum constant representing the cascade option.</summary>
		Cascade,

        /// <summary>An enum constant representing the set null option.</summary>
		SetNull,

        /// <summary>An enum constant representing the no action option.</summary>
		NoAction,

        /// <summary>An enum constant representing the set default option.</summary>
		SetDefault,

        /// <summary>An enum constant representing the restrict option.</summary>
		Restrict
	}

    /// <summary>An ORM lite ddl extensions.</summary>
	public static class OrmLiteDDLExtensions
	{
        /// <summary>An IDbConnection extension method that alter table.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn"> The dbConn to act on.</param>
        /// <param name="command">The command.</param>
		public static void AlterTable<T>(this IDbConnection dbConn, string command)
		{
			AlterTable(dbConn, typeof(T),command);
		}

        /// <summary>An IDbConnection extension method that alter table.</summary>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="command">  The command.</param>
		public static void AlterTable(this IDbConnection dbConn, Type modelType, string command)
		{
			string sql = string.Format("ALTER TABLE {0} {1};", 
			                           OrmLiteConfig.DialectProvider.GetQuotedTableName( modelType.GetModelDefinition()),
			                           command);
			dbConn.ExecuteSql(sql);
		}

        /// <summary>An IDbConnection extension method that adds a column to 'field'.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="field"> The field.</param>
		public static void AddColumn<T>(this IDbConnection dbConn,
		                                Expression<Func<T,object>> field)
		{
			var modelDef = ModelDefinition<T>.Definition;
			var fieldDef = modelDef.GetFieldDefinition<T>(field);
			dbConn.AddColumn(typeof(T), fieldDef);
		}

        /// <summary>An IDbConnection extension method that adds a column.</summary>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef"> The field definition.</param>
		public static void AddColumn(this IDbConnection dbConn, Type modelType, FieldDefinition fieldDef)
		{
			var command = OrmLiteConfig.DialectProvider.ToAddColumnStatement(modelType, fieldDef);
			dbConn.ExecuteSql(command);
		}

        /// <summary>An IDbConnection extension method that alter column.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">The dbConn to act on.</param>
        /// <param name="field"> The field.</param>
		public static void AlterColumn<T>(this IDbConnection dbConn,  Expression<Func<T,object>> field)
		{
			var modelDef = ModelDefinition<T>.Definition;
			var fieldDef = modelDef.GetFieldDefinition<T>(field);
			dbConn.AlterColumn(typeof(T), fieldDef);
		}

        /// <summary>An IDbConnection extension method that alter column.</summary>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef"> The field definition.</param>
		public static void AlterColumn(this IDbConnection dbConn, Type modelType, FieldDefinition fieldDef)
		{
			var command = OrmLiteConfig.DialectProvider.ToAlterColumnStatement(modelType, fieldDef);
			dbConn.ExecuteSql(command);
		}

        /// <summary>An IDbConnection extension method that change column name.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">       The dbConn to act on.</param>
        /// <param name="field">        The field.</param>
        /// <param name="oldColumnName">Name of the old column.</param>
		public static void ChangeColumnName<T>(this IDbConnection dbConn,
		                                       Expression<Func<T,object>> field,
		                                       string oldColumnName)
		{
			var modelDef = ModelDefinition<T>.Definition;
			var fieldDef = modelDef.GetFieldDefinition<T>(field);
			dbConn.ChangeColumnName(typeof(T), fieldDef, oldColumnName);	
		}

        /// <summary>An IDbConnection extension method that change column name.</summary>
        /// <param name="dbConn">       The dbConn to act on.</param>
        /// <param name="modelType">    Type of the model.</param>
        /// <param name="fieldDef">     The field definition.</param>
        /// <param name="oldColumnName">Name of the old column.</param>
		public static void ChangeColumnName(this IDbConnection dbConn,
		                                    Type modelType,
		                                    FieldDefinition fieldDef,
		                                    string oldColumnName)
		{
			var command = OrmLiteConfig.DialectProvider.ToChangeColumnNameStatement(modelType, fieldDef, oldColumnName);
			dbConn.ExecuteSql(command);
		}

        /// <summary>An IDbConnection extension method that drop column.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="columnName">Name of the column.</param>
		public static void DropColumn<T>(this IDbConnection dbConn,string columnName)
		{
			dbConn.DropColumn(typeof(T), columnName);
		}

        /// <summary>An IDbConnection extension method that drop column.</summary>
        /// <param name="dbConn">    The dbConn to act on.</param>
        /// <param name="modelType"> Type of the model.</param>
        /// <param name="columnName">Name of the column.</param>
		public static void DropColumn(this IDbConnection dbConn,Type modelType, string columnName)
		{
			string command = string.Format("ALTER TABLE {0} DROP {1};",
			                               OrmLiteConfig.DialectProvider.GetQuotedTableName(modelType.GetModelDefinition().ModelName),
			                               OrmLiteConfig.DialectProvider.GetQuotedName(columnName));
			
			dbConn.ExecuteSql(command);
		}

        /// <summary>An IDbConnection extension method that adds a foreign key.</summary>
        /// <typeparam name="T">       Generic type parameter.</typeparam>
        /// <typeparam name="TForeign">Type of the foreign.</typeparam>
        /// <param name="dbConn">        The dbConn to act on.</param>
        /// <param name="field">         The field.</param>
        /// <param name="foreignField">  The foreign field.</param>
        /// <param name="onUpdate">      The on update.</param>
        /// <param name="onDelete">      The on delete.</param>
        /// <param name="foreignKeyName">Name of the foreign key.</param>
		public static void AddForeignKey<T,TForeign>(this IDbConnection dbConn,
		                                             Expression<Func<T,object>> field,
		                                             Expression<Func<TForeign,object>> foreignField,
		                                             OnFkOption onUpdate,
		                                             OnFkOption onDelete,
		                                             string foreignKeyName=null)
		{
			string command = OrmLiteConfig.DialectProvider.ToAddForeignKeyStatement(field,
			                                                                        foreignField,
			                                                                        onUpdate,
			                                                                        onDelete,
			                                                                        foreignKeyName);
			dbConn.ExecuteSql (command);	
		}

        /// <summary>An IDbConnection extension method that drop foreign key.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">        The dbConn to act on.</param>
        /// <param name="foreignKeyName">Name of the foreign key.</param>
		public static void DropForeignKey<T>(this IDbConnection dbConn,string foreignKeyName)
		{
			string command = string.Format("ALTER TABLE {0} DROP FOREIGN KEY {1};",
			                               OrmLiteConfig.DialectProvider.GetQuotedTableName(ModelDefinition<T>.Definition.ModelName),
			                               OrmLiteConfig.DialectProvider.GetQuotedName(foreignKeyName));	
			dbConn.ExecuteSql(command);
		}

        /// <summary>An IDbConnection extension method that creates an index.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="field">    The field.</param>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="unique">   true to unique.</param>
		public static void CreateIndex<T>(this IDbConnection dbConn,Expression<Func<T,object>> field,
		                                  string indexName=null, bool unique=false)
		{
			var command = OrmLiteConfig.DialectProvider.ToCreateIndexStatement(field, indexName, unique);
			dbConn.ExecuteSql(command);
		}

        /// <summary>An IDbConnection extension method that drop index.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbConn">   The dbConn to act on.</param>
        /// <param name="indexName">Name of the index.</param>
		public static void DropIndex<T>(this IDbConnection dbConn, string indexName)
		{
			string command = string.Format("ALTER TABLE {0} DROP INDEX  {1};",
			                               OrmLiteConfig.DialectProvider.GetQuotedTableName(ModelDefinition<T>.Definition.ModelName),
			                               OrmLiteConfig.DialectProvider.GetQuotedName(indexName));
			dbConn.ExecuteSql(command);
		}

	}
}
