using System;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite.Oracle.DbSchema;

namespace NServiceKit.OrmLite.Oracle
{
    /// <summary>A table.</summary>
	public class Table : ITable
	{
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.Oracle.Table class.
        /// </summary>
		public Table()
		{
		}

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        [Alias("TABLE_NAME")]
		public string Name { get; set; }

        /// <summary>Gets or sets the owner.</summary>
        /// <value>The owner.</value>
        [Alias("TABLE_SCHEMA")]
		public string Owner { get; set; }
	}
}

