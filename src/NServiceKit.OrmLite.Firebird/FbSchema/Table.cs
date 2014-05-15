using System;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite.Firebird.DbSchema;

namespace NServiceKit.OrmLite.Firebird
{
    /// <summary>A table.</summary>
	public class Table : ITable
	{
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.Firebird.Table class.
        /// </summary>
		public Table()
		{
		}

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		[Alias("NAME")]
		public string Name { get; set; }

        /// <summary>Gets or sets the owner.</summary>
        /// <value>The owner.</value>
		[Alias("OWNER")]
		public string Owner { get; set; }
	}
}

