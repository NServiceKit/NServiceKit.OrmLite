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
using System.Linq.Expressions;

namespace NServiceKit.OrmLite
{
    /// <summary>Interface for ORM lite dialect provider.</summary>
    public interface IOrmLiteDialectProvider
    {
        /// <summary>Gets or sets the default string length.</summary>
        /// <value>The default string length.</value>
        int DefaultStringLength { get; set; }

        /// <summary>Gets or sets the parameter string.</summary>
        /// <value>The parameter string.</value>
        string ParamString { get; set; }

        /// <summary>Gets or sets a value indicating whether this object use unicode.</summary>
        /// <value>true if use unicode, false if not.</value>
        bool UseUnicode { get; set; }

        /// <summary>Gets or sets the naming strategy.</summary>
        /// <value>The naming strategy.</value>
        INamingStrategy NamingStrategy { get; set; }

        /// <summary>
        /// Quote the string so that it can be used inside an SQL-expression Escape quotes inside the
        /// string.
        /// </summary>
        /// <param name="paramValue">.</param>
        /// <returns>The quoted parameter.</returns>
        string GetQuotedParam(string paramValue);

        /// <summary>Convert database value.</summary>
        /// <param name="value">The value.</param>
        /// <param name="type"> The type.</param>
        /// <returns>The database converted value.</returns>
        object ConvertDbValue(object value, Type type);

        /// <summary>Gets quoted value.</summary>
        /// <param name="value">    The value.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns>The quoted value.</returns>
        string GetQuotedValue(object value, Type fieldType);

        /// <summary>Creates a connection.</summary>
        /// <param name="filePath">Full pathname of the file.</param>
        /// <param name="options"> Options for controlling the operation.</param>
        /// <returns>The new connection.</returns>
        IDbConnection CreateConnection(string filePath, Dictionary<string, string> options);

        /// <summary>Gets quoted table name.</summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>The quoted table name.</returns>
        string GetQuotedTableName(ModelDefinition modelDef);

        /// <summary>Gets quoted table name.</summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>The quoted table name.</returns>
        string GetQuotedTableName(string tableName);

        /// <summary>Gets quoted column name.</summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The quoted column name.</returns>
        string GetQuotedColumnName(string columnName);

        /// <summary>Gets quoted name.</summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The quoted name.</returns>
        string GetQuotedName(string columnName);

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
        string GetColumnDefinition(
            string fieldName, Type fieldType, bool isPrimaryKey, bool autoIncrement,
            bool isNullable, int? fieldLength,
            int? scale, string defaultValue);

        /// <summary>Gets the last insert identifier.</summary>
        /// <param name="command">The command.</param>
        /// <returns>The last insert identifier.</returns>
        long GetLastInsertId(IDbCommand command);

        /// <summary>Inserts an and get last insert identifier described by dbCmd.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <returns>A long.</returns>
        long InsertAndGetLastInsertId<T>(IDbCommand dbCmd);

        /// <summary>Converts this object to a select statement.</summary>
        /// <param name="tableType">   Type of the table.</param>
        /// <param name="sqlFilter">   A filter specifying the SQL.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>The given data converted to a string.</returns>
        string ToSelectStatement(Type tableType, string sqlFilter, params object[] filterParams);

        /// <summary>Converts this object to an insert row statement.</summary>
        /// <param name="command">          The command.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="InsertFields">     The insert fields.</param>
        /// <returns>The given data converted to a string.</returns>
        string ToInsertRowStatement(IDbCommand command, object objWithProperties, ICollection<string> InsertFields = null);

        /// <summary>Creates parameterized insert statement.</summary>
        /// <param name="connection">       The connection.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="insertFields">     The insert fields.</param>
        /// <returns>The new parameterized insert statement.</returns>
        IDbCommand CreateParameterizedInsertStatement(IDbConnection connection, object objWithProperties, ICollection<string> insertFields = null);

        /// <summary>Re parameterize insert statement.</summary>
        /// <param name="command">          The command.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="insertFields">     The insert fields.</param>
        void ReParameterizeInsertStatement(IDbCommand command, object objWithProperties, ICollection<string> insertFields = null);

        /// <summary>Converts this object to an update row statement.</summary>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="UpdateFields">     The update fields.</param>
        /// <returns>The given data converted to a string.</returns>
        string ToUpdateRowStatement(object objWithProperties, ICollection<string> UpdateFields = null);

        /// <summary>Creates parameterized update statement.</summary>
        /// <param name="connection">       The connection.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="updateFields">     The update fields.</param>
        /// <returns>The new parameterized update statement.</returns>
        IDbCommand CreateParameterizedUpdateStatement(IDbConnection connection, object objWithProperties, ICollection<string> updateFields = null);

        /// <summary>Converts the objWithProperties to a delete row statement.</summary>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <returns>objWithProperties as a string.</returns>
        string ToDeleteRowStatement(object objWithProperties);

        /// <summary>Converts this object to a delete statement.</summary>
        /// <param name="tableType">   Type of the table.</param>
        /// <param name="sqlFilter">   A filter specifying the SQL.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>The given data converted to a string.</returns>
        string ToDeleteStatement(Type tableType, string sqlFilter, params object[] filterParams);

        /// <summary>Creates parameterized delete statement.</summary>
        /// <param name="connection">       The connection.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <returns>The new parameterized delete statement.</returns>
        IDbCommand CreateParameterizedDeleteStatement(IDbConnection connection, object objWithProperties);

        /// <summary>Converts this object to an exist statement.</summary>
        /// <param name="fromTableType">    Type of from table.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="sqlFilter">        A filter specifying the SQL.</param>
        /// <param name="filterParams">     Options for controlling the filter.</param>
        /// <returns>The given data converted to a string.</returns>
        string ToExistStatement(Type fromTableType,
            object objWithProperties,
            string sqlFilter,
            params object[] filterParams);

        /// <summary>Converts this object to a select from procedure statement.</summary>
        /// <param name="fromObjWithProperties">from object with properties.</param>
        /// <param name="outputModelType">      Type of the output model.</param>
        /// <param name="sqlFilter">            A filter specifying the SQL.</param>
        /// <param name="filterParams">         Options for controlling the filter.</param>
        /// <returns>The given data converted to a string.</returns>
        string ToSelectFromProcedureStatement(object fromObjWithProperties,
            Type outputModelType,
            string sqlFilter,
            params object[] filterParams);

        /// <summary>Converts this object to a count statement.</summary>
        /// <param name="fromTableType">Type of from table.</param>
        /// <param name="sqlFilter">    A filter specifying the SQL.</param>
        /// <param name="filterParams"> Options for controlling the filter.</param>
        /// <returns>The given data converted to a string.</returns>
        string ToCountStatement(Type fromTableType, string sqlFilter, params object[] filterParams);

        /// <summary>Converts the objWithProperties to an execute procedure statement.</summary>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <returns>objWithProperties as a string.</returns>
        string ToExecuteProcedureStatement(object objWithProperties);

        /// <summary>Converts a tableType to a create table statement.</summary>
        /// <param name="tableType">Type of the table.</param>
        /// <returns>tableType as a string.</returns>
        string ToCreateTableStatement(Type tableType);

        /// <summary>Converts a tableType to a create index statements.</summary>
        /// <param name="tableType">Type of the table.</param>
        /// <returns>tableType as a List&lt;string&gt;</returns>
        List<string> ToCreateIndexStatements(Type tableType);

        /// <summary>Converts a tableType to a create sequence statements.</summary>
        /// <param name="tableType">Type of the table.</param>
        /// <returns>tableType as a List&lt;string&gt;</returns>
        List<string> ToCreateSequenceStatements(Type tableType);        

        /// <summary>Converts this object to a create sequence statement.</summary>
        /// <param name="tableType">   Type of the table.</param>
        /// <param name="sequenceName">Name of the sequence.</param>
        /// <returns>The given data converted to a string.</returns>
        string ToCreateSequenceStatement(Type tableType, string sequenceName);

        /// <summary>Sequence list.</summary>
        /// <param name="tableType">Type of the table.</param>
        /// <returns>A List&lt;string&gt;</returns>
        List<string> SequenceList(Type tableType);

        /// <summary>Query if 'dbCmd' does table exist.</summary>
        /// <param name="db">       The database.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool DoesTableExist(IDbConnection db, string tableName);

        /// <summary>Query if 'dbCmd' does table exist.</summary>
        /// <param name="dbCmd">    The database command.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool DoesTableExist(IDbCommand dbCmd, string tableName);

        /// <summary>Query if 'dbCmd' does sequence exist.</summary>
        /// <param name="dbCmd">      The database command.</param>
        /// <param name="sequencName">Name of the sequenc.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool DoesSequenceExist(IDbCommand dbCmd, string sequencName);

        /// <summary>Gets column names.</summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>The column names.</returns>
        string GetColumnNames(ModelDefinition modelDef);

        /// <summary>Expression visitor.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        SqlExpressionVisitor<T> ExpressionVisitor<T>();

        /// <summary>Gets column database type.</summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>The column database type.</returns>
        DbType GetColumnDbType(Type valueType);

        /// <summary>Gets column type definition.</summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns>The column type definition.</returns>
        string GetColumnTypeDefinition(Type fieldType);

        /// <summary>Gets drop foreign key constraints.</summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>The drop foreign key constraints.</returns>
        string GetDropForeignKeyConstraints(ModelDefinition modelDef);

		#region DDL 
        /// <summary>Converts this object to an add column statement.</summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef"> The field definition.</param>
        /// <returns>The given data converted to a string.</returns>
		string ToAddColumnStatement (Type modelType, FieldDefinition fieldDef);

        /// <summary>Converts this object to an alter column statement.</summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef"> The field definition.</param>
        /// <returns>The given data converted to a string.</returns>
		string ToAlterColumnStatement(Type modelType, FieldDefinition fieldDef);

        /// <summary>Converts this object to a change column name statement.</summary>
        /// <param name="modelType">    Type of the model.</param>
        /// <param name="fieldDef">     The field definition.</param>
        /// <param name="oldColumnName">Name of the old column.</param>
        /// <returns>The given data converted to a string.</returns>
		string ToChangeColumnNameStatement(Type modelType, FieldDefinition fieldDef, string oldColumnName);

        /// <summary>Converts this object to an add foreign key statement.</summary>
        /// <typeparam name="T">       Generic type parameter.</typeparam>
        /// <typeparam name="TForeign">Type of the foreign.</typeparam>
        /// <param name="field">         The field.</param>
        /// <param name="foreignField">  The foreign field.</param>
        /// <param name="onUpdate">      The on update.</param>
        /// <param name="onDelete">      The on delete.</param>
        /// <param name="foreignKeyName">Name of the foreign key.</param>
        /// <returns>The given data converted to a string.</returns>
		string ToAddForeignKeyStatement<T,TForeign>(Expression<Func<T,object>> field,
		                                             Expression<Func<TForeign,object>> foreignField,
		                                             OnFkOption onUpdate,
		                                             OnFkOption onDelete,
		                                             string foreignKeyName=null);

        /// <summary>Converts this object to a create index statement.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="field">    The field.</param>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="unique">   true to unique.</param>
        /// <returns>The given data converted to a string.</returns>
		string ToCreateIndexStatement<T>(Expression<Func<T,object>> field,
		                                 string indexName=null, bool unique=false);
		#endregion DDL

    }
}