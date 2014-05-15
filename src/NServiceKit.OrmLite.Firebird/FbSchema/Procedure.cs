using System;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite.Firebird.DbSchema;

namespace NServiceKit.OrmLite.Firebird
{
    /// <summary>A procedure.</summary>
	public class Procedure : IProcedure
	{
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.Firebird.Procedure class.
        /// </summary>
		public Procedure()
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

        /// <summary>Gets or sets the inputs.</summary>
        /// <value>The inputs.</value>
		[Alias("INPUTS")]
		public Int16 Inputs { get; set; }

        /// <summary>Gets or sets the outputs.</summary>
        /// <value>The outputs.</value>
		[Alias("OUTPUTS")]
		public Int16 Outputs { get; set; }

        /// <summary>Gets the type.</summary>
        /// <value>The type.</value>
		[Ignore]
		public ProcedureType Type
		{
			get
			{
				return Outputs == 0 ? ProcedureType.Executable : ProcedureType.Selectable;
			}
		}
	}
}

