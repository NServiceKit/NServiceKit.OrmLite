using System;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite.Firebird.DbSchema;

namespace NServiceKit.OrmLite.Firebird
{
    /// <summary>A column.</summary>
	public class Column : IColumn
	{
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.Firebird.Column class.
        /// </summary>
		public Column()
		{
		}

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		[Alias("FIELD_NAME")]
		public string Name { get; set; }

        /// <summary>Gets or sets the position.</summary>
        /// <value>The position.</value>
		[Alias("FIELD_POSITION")]
		public int Position { get; set; }

        /// <summary>Gets or sets the type of the database.</summary>
        /// <value>The type of the database.</value>
		[Alias("FIELD_TYPE")]
		public string DbType { get; set; }

        /// <summary>Gets or sets the length.</summary>
        /// <value>The length.</value>
		[Alias("FIELD_LENGTH")]
		public int Length { get; set; }

        /// <summary>Gets or sets the presicion.</summary>
        /// <value>The presicion.</value>
		[Alias("FIELD_PRECISION")]
		public int Presicion { get; set; }

        /// <summary>Gets or sets the scale.</summary>
        /// <value>The scale.</value>
		[Alias("FIELD_SCALE")]
		public int Scale { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is nullable.</summary>
        /// <value>true if nullable, false if not.</value>
		[Alias("NULLABLE")]
		public bool Nullable { get; set; }

        /// <summary>Gets or sets the description.</summary>
        /// <value>The description.</value>
		[Alias("DESCRIPTION")]
		public string Description { get; set; }

        /// <summary>Gets or sets the name of the table.</summary>
        /// <value>The name of the table.</value>
		[Alias("TABLENAME")]
		public string TableName { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is primary key.</summary>
        /// <value>true if this object is primary key, false if not.</value>
		[Alias("IS_PRIMARY_KEY")]
		public bool IsPrimaryKey { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is unique.</summary>
        /// <value>true if this object is unique, false if not.</value>
		[Alias("IS_UNIQUE")]
		public bool IsUnique { get; set; }

        /// <summary>Gets or sets the sequence.</summary>
        /// <value>The sequence.</value>
		[Alias("SEQUENCE_NAME")]
		public string Sequence { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is computed.</summary>
        /// <value>true if this object is computed, false if not.</value>
		[Alias("IS_COMPUTED")]
		public bool IsComputed { get; set; }

        /// <summary>Gets or sets a value indicating whether the automatic increment.</summary>
        /// <value>true if automatic increment, false if not.</value>
		[Ignore]
		public bool AutoIncrement
		{
			get { return !string.IsNullOrEmpty(Sequence); }
			set
			{
				;
			}
		}

        /// <summary>Gets the type of the net.</summary>
        /// <value>The type of the net.</value>
		[Ignore]
		public Type NetType
		{
			get
			{
				Type t;
				switch (DbType)
				{
					case "BIGINT":
						t = Nullable ? typeof(Int64?) : typeof(Int64);
						break;

					case "BLOB":
						t = Nullable ? typeof(Byte?[]) : typeof(Byte[]);
						break;

					case "CHAR":
						t = typeof(string);
						break;

					case "DATE":
						t = Nullable ? typeof(DateTime?) : typeof(DateTime);
						break;

					case "DECIMAL":
						t = Nullable ? typeof(Decimal?) : typeof(Decimal);
						break;

					case "DOUBLE PRECISION":
						t = Nullable ? typeof(Double?) : typeof(Double);
						break;

					case "FLOAT":
						t = Nullable ? typeof(float?) : typeof(float);
						break;

					case "NUMERIC":
						t = Nullable ? typeof(Decimal?) : typeof(Decimal);
						break;

					case "SMALLINT":
						t = Nullable ? typeof(Int16?) : typeof(Int16);
						break;

					case "TIME":
						t = Nullable ? typeof(DateTime?) : typeof(DateTime);
						break;

					case "TIMESTAMP":
						t = Nullable ? typeof(DateTime?) : typeof(DateTime);
						break;

					case "TEXT":
						t = typeof(string);
						break;

					case "INTEGER":
						t = Nullable ? typeof(Int32?) : typeof(Int32);
						break;

					case "VARCHAR":
						t = typeof(string);
						break;
					case "GUID":
						t = Nullable ? typeof(Guid?) : typeof(Guid);
						break;
					default:
						t = typeof(string);
						break;
				}

				return t;
			}
		}

	}
}

