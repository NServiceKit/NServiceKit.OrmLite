using System;

namespace NServiceKit.OrmLite.Oracle.DbSchema
{
    /// <summary>Interface for column.</summary>
	public interface IColumn
	{
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		string Name { get; set; }

        /// <summary>Gets or sets the position.</summary>
        /// <value>The position.</value>
		int Position { get; set; }

        /// <summary>Gets or sets the type of the database.</summary>
        /// <value>The type of the database.</value>
		string DbType { get; set; }

        /// <summary>Gets or sets the length.</summary>
        /// <value>The length.</value>
		int Length { get; set; }

        /// <summary>Gets or sets the presicion.</summary>
        /// <value>The presicion.</value>
		int Presicion { get; set; }

        /// <summary>Gets or sets the scale.</summary>
        /// <value>The scale.</value>
		int Scale { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is nullable.</summary>
        /// <value>true if nullable, false if not.</value>
		bool Nullable { get; set; }

		//string Description { get; set; }

        /// <summary>Gets or sets the name of the table.</summary>
        /// <value>The name of the table.</value>
		string TableName { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is primary key.</summary>
        /// <value>true if this object is primary key, false if not.</value>
		bool IsPrimaryKey { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is unique.</summary>
        /// <value>true if this object is unique, false if not.</value>
		bool IsUnique { get; set; }

        /// <summary>Gets or sets the sequence.</summary>
        /// <value>The sequence.</value>
		string Sequence { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is computed.</summary>
        /// <value>true if this object is computed, false if not.</value>
		bool IsComputed { get; set; }

        /// <summary>Gets or sets a value indicating whether the automatic increment.</summary>
        /// <value>true if automatic increment, false if not.</value>
		bool AutoIncrement { get; set; }

        /// <summary>Gets the type of the net.</summary>
        /// <value>The type of the net.</value>
		Type NetType { get; }
	}
}

