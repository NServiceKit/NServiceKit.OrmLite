using System;

namespace NServiceKit.OrmLite.Oracle.DbSchema
{
    /// <summary>Interface for procedure.</summary>
	public interface IProcedure
	{
        /// <summary>Gets or sets the name of the procedure.</summary>
        /// <value>The name of the procedure.</value>
        string ProcedureName { get; set; }

        /// <summary>Gets or sets the name of the function.</summary>
        /// <value>The name of the function.</value>
        string FunctionName { get; set; }

        /// <summary>Gets or sets the owner.</summary>
        /// <value>The owner.</value>
		string Owner { get; set; }

        /// <summary>Gets or sets the type of the object.</summary>
        /// <value>The type of the object.</value>
        string ObjectType { get; set; }
		
		//Int16 Inputs { get; set; }
		
		//Int16 Outputs { get; set; }

        /// <summary>Gets the type.</summary>
        /// <value>The type.</value>
		ProcedureType Type { get; }
	}
}

