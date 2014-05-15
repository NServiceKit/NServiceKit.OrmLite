using System.Data;
using System.Collections.Generic;

namespace NServiceKit.OrmLite.Oracle.DbSchema
{
    /// <summary>Interface for schema.</summary>
    /// <typeparam name="TTable">    Type of the table.</typeparam>
    /// <typeparam name="TColumn">   Type of the column.</typeparam>
    /// <typeparam name="TProcedure">Type of the procedure.</typeparam>
    /// <typeparam name="TParameter">Type of the parameter.</typeparam>
	public interface ISchema<TTable, TColumn, TProcedure, TParameter>
		where TTable : ITable, new()
		where TColumn : IColumn, new()
		where TProcedure : IProcedure, new()
		where TParameter : IParameter, new()
	{
        /// <summary>Sets the connection.</summary>
        /// <value>The connection.</value>
		IDbConnection Connection { set; }

        /// <summary>Gets the tables.</summary>
        /// <value>The tables.</value>
		List<TTable> Tables { get; }

        /// <summary>Gets a table.</summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>The table.</returns>
		TTable GetTable(string tableName);

        /// <summary>Gets the columns.</summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>The columns.</returns>
		List<TColumn> GetColumns(string tableName);

        /// <summary>Gets the columns.</summary>
        /// <param name="table">The table.</param>
        /// <returns>The columns.</returns>
		List<TColumn> GetColumns(TTable table);

        /// <summary>Gets a procedure.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The procedure.</returns>
		TProcedure GetProcedure(string name);

        /// <summary>Gets the parameters.</summary>
        /// <param name="procedure">The procedure.</param>
        /// <returns>The parameters.</returns>
		List<TParameter> GetParameters(TProcedure procedure);

        /// <summary>Gets the parameters.</summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>The parameters.</returns>
		List<TParameter> GetParameters(string procedureName);
	}
}
