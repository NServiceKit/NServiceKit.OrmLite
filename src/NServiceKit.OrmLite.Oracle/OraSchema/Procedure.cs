using System;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite.Oracle.DbSchema;

namespace NServiceKit.OrmLite.Oracle
{
    /// <summary>A procedure.</summary>
	public class Procedure : IProcedure
	{
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.Oracle.Procedure class.
        /// </summary>
		public Procedure()
		{
		}

        /// <summary>Gets or sets the name of the procedure.</summary>
        /// <value>The name of the procedure.</value>
		[Alias("PROCEDURE_NAME")]
		public string ProcedureName { get; set; }

        /// <summary>Gets or sets the name of the function.</summary>
        /// <value>The name of the function.</value>
        [Alias("OBJECT_NAME")]
        public string FunctionName { get; set; }

        /// <summary>Gets or sets the owner.</summary>
        /// <value>The owner.</value>
		[Alias("OWNER")]
		public string Owner { get; set; }

        /// <summary>Gets or sets the type of the object.</summary>
        /// <value>The type of the object.</value>
        [Alias("OBJECT_TYPE")]
        public string ObjectType { get; set; }

		//[Alias("INPUTS")]
		//public Int16 Inputs { get; set; }

		//[Alias("OUTPUTS")]
		//public Int16 Outputs { get; set; }

        /// <summary>Gets the type.</summary>
        /// <value>The type.</value>
		[Ignore]
		public ProcedureType Type
		{
			get
			{
                return ProcedureType.Executable;
				//return Outputs == 0 ? ProcedureType.Executable : ProcedureType.Selectable;
			}
		}
	}
}

