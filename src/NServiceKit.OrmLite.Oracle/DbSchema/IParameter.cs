using System;

namespace NServiceKit.OrmLite.Oracle.DbSchema
{
    /// <summary>Interface for parameter.</summary>
	public interface IParameter
	{
        /// <summary>Gets or sets the name of the procedure.</summary>
        /// <value>The name of the procedure.</value>
		string ProcedureName { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		string Name { get; set; }

        /// <summary>Gets or sets the position.</summary>
        /// <value>The position.</value>
		Int16 Position { get; set; }

        /// <summary>Gets or sets the type.</summary>
        /// <value>The p type.</value>
		Int16 PType { get; set; }

        /// <summary>Gets or sets the type of the database.</summary>
        /// <value>The type of the database.</value>
		string DbType { get; set; }

        /// <summary>Gets or sets the length.</summary>
        /// <value>The length.</value>
		Int32 Length { get; set; }

        /// <summary>Gets or sets the presicion.</summary>
        /// <value>The presicion.</value>
		Int32 Presicion { get; set; }

        /// <summary>Gets or sets the scale.</summary>
        /// <value>The scale.</value>
		Int32 Scale { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is nullable.</summary>
        /// <value>true if nullable, false if not.</value>
		bool Nullable { get; set; }

        /// <summary>Gets the direction.</summary>
        /// <value>The direction.</value>
		ParameterDirection Direction { get; }
	}
}

