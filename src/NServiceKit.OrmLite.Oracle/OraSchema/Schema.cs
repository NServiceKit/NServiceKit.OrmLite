using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite.Oracle.DbSchema;

namespace NServiceKit.OrmLite.Oracle
{
    /// <summary>A schema.</summary>
	public class Schema : ISchema<Table, Column, Procedure, Parameter>
	{
        /// <summary>The SQL tables.</summary>
		private string sqlTables;

        /// <summary>The SQL columns.</summary>
		private StringBuilder sqlColumns = new StringBuilder();

        /// <summary>The SQL field generator.</summary>
		private StringBuilder sqlFieldGenerator = new StringBuilder();

        /// <summary>The SQL generator.</summary>
		private StringBuilder sqlGenerator = new StringBuilder();

        /// <summary>The SQL procedures.</summary>
		private StringBuilder sqlProcedures = new StringBuilder();

        /// <summary>The SQL col constrains.</summary>
        private StringBuilder sqlColConstrains = new StringBuilder();        

        /// <summary>Options for controlling the SQL.</summary>
		private StringBuilder  sqlParameters = new StringBuilder();

        /// <summary>Sets the connection.</summary>
        /// <value>The connection.</value>
		public IDbConnection Connection { private get; set; }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.Oracle.Schema class.
        /// </summary>
		public Schema()
		{
			Init();
		}

        /// <summary>Gets the tables.</summary>
        /// <value>The tables.</value>
		public List<Table> Tables
		{
			get
			{
                return Connection.Select<Table>(sqlTables);
			}
		}

        /// <summary>Gets a table.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The table.</returns>
		public Table GetTable(string name)
		{
			string sql = sqlTables + string.Format("    WHERE TABLE_NAME ='{0}' ", name);

            var query = Connection.Select<Table>(sql);
            return query.FirstOrDefault();
		}

        /// <summary>Gets the columns.</summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>The columns.</returns>
		public List<Column> GetColumns(string tableName)
		{

			string sql = string.Format(sqlColumns.ToString(),string.IsNullOrEmpty(tableName) ? "\'\'" : string.Format("\'{0}\'", tableName));

            List<Column> columns = Connection.Select<Column>(sql);

            List<Generador> gens = Connection.Select<Generador>(sqlGenerator.ToString());

            foreach (var record in columns)
            {
                record.IsPrimaryKey = (Connection.GetScalar<int>(string.Format(sqlColConstrains.ToString(), tableName, record.Name, "P")) > 0);
                record.IsUnique = (Connection.GetScalar<int>(string.Format(sqlColConstrains.ToString(), tableName, record.Name, "U")) > 0);
                string g = (from gen in gens
                            where gen.Name == tableName + "_" + record.Name + "_GEN"
                            select gen.Name).FirstOrDefault();

                if (!string.IsNullOrEmpty(g)) record.Sequence = g.Trim();
            }
            return columns;
        }

        /// <summary>Gets the columns.</summary>
        /// <param name="table">The table.</param>
        /// <returns>The columns.</returns>
		public List<Column> GetColumns(Table table)
		{
			return GetColumns(table.Name);
		}

        /// <summary>Gets a procedure.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The procedure.</returns>
		public Procedure GetProcedure(string name)
		{
			string sql=  string.Format(" sqlProcedures.ToString() ", name);
            var query = Connection.Select<Procedure>(sql);
            return query.FirstOrDefault();
        }

        /// <summary>Gets the procedures.</summary>
        /// <value>The procedures.</value>
		public List<Procedure> Procedures
		{
			get
			{
                return Connection.Select<Procedure>(sqlProcedures.ToString());
			}
		}

        /// <summary>Gets the parameters.</summary>
        /// <param name="procedure">The procedure.</param>
        /// <returns>The parameters.</returns>
		public List<Parameter> GetParameters(Procedure procedure)
		{
			return GetParameters(procedure.ProcedureName);
		}

        /// <summary>Gets the parameters.</summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>The parameters.</returns>
		public List<Parameter> GetParameters(string procedureName)
		{
			string sql = string.Format(sqlParameters.ToString(), string.IsNullOrEmpty(procedureName) ? "" :procedureName);
            return Connection.Select<Parameter>(sql);
		}

        /// <summary>Initialises this object.</summary>
		private void Init()
		{
            sqlTables = "select TABLE_NAME, USER TABLE_SCHEMA  from USER_TABLES ";

			sqlColumns.Append(" select * \n");
			sqlColumns.Append(" from USER_TAB_COLS utc  \n");
			sqlColumns.Append(" where table_name = {0} \n");
            sqlColumns.Append(" AND hidden_column =  \'NO\' \n");
            
			sqlColumns.Append(" order by column_id \n");

            sqlProcedures.Append("SELECT * FROM ALL_PROCEDURES WHERE OBJECT_TYPE = \'PROCEDURE\'  OR  OBJECT_TYPE = \'FUNCTION\' \n");
			sqlParameters.Append("select * from user_arguments WHERE OBJECT_NAME = \'{0}\' ORDER BY Position asc\n");
            sqlGenerator.Append("SELECT TABLE_NAME AS \"Name\" FROM ALL_CATALOG WHERE Table_Type = \'SEQUENCE\' ");
            
            sqlColConstrains.Append(" SELECT Count(cols.position) \n");
            sqlColConstrains.Append(" FROM all_constraints cons, all_cons_columns cols \n");
            sqlColConstrains.Append(" WHERE cols.table_name = \'{0}\' \n");
            sqlColConstrains.Append(" AND cons.constraint_type = \'{2}\' \n");
            sqlColConstrains.Append(" AND cons.constraint_name = cols.constraint_name \n");
            sqlColConstrains.Append(" AND cons.owner = cols.owner \n");
            sqlColConstrains.Append(" AND cols.column_name = \'{1}\' \n");            
            sqlColConstrains.Append(" ORDER BY cols.table_name, cols.position \n");
		}		

        /// <summary>A generador.</summary>
		private class Generador
        {
            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
			public string Name { get; set; }
		}
	}

}