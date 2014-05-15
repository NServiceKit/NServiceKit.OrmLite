using System;

namespace NServiceKit.OrmLite.Firebird.DbSchema
{
    /// <summary>Interface for procedure.</summary>
	public interface IProcedure
	{
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		string Name { get; set; }

        /// <summary>Gets or sets the owner.</summary>
        /// <value>The owner.</value>
		string Owner { get; set; }

        /// <summary>Gets or sets the inputs.</summary>
        /// <value>The inputs.</value>
		Int16 Inputs { get; set; }

        /// <summary>Gets or sets the outputs.</summary>
        /// <value>The outputs.</value>
		Int16 Outputs { get; set; }

        /// <summary>Gets the type.</summary>
        /// <value>The type.</value>
		ProcedureType Type { get; }
	}
}

