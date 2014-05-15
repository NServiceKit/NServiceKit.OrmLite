using System;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite.Oracle.DbSchema;

namespace NServiceKit.OrmLite.Oracle
{
    /// <summary>A parameter.</summary>
	public class Parameter : IParameter
	{
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.Oracle.Parameter class.
        /// </summary>
		public Parameter()
		{
			Nullable = true;
		}

        /// <summary>Gets or sets the name of the procedure.</summary>
        /// <value>The name of the procedure.</value>
		[Alias("PROCEDURE_NAME")]
		public string ProcedureName { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		[Alias("PARAMETER_NAME")]
		public string Name { get; set; }

        /// <summary>Gets or sets the position.</summary>
        /// <value>The position.</value>
		[Alias("PARAMETER_NUMBER")]
		public Int16 Position { get; set; }

        /// <summary>Gets or sets the type.</summary>
        /// <value>The p type.</value>
		[Alias("PARAMETER_TYPE")]
		public Int16 PType { get; set; }

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
		[Ignore]
		public bool Nullable
		{
			get;
			set;
		}

        /// <summary>Gets the direction.</summary>
        /// <value>The direction.</value>
		[Ignore]
		public ParameterDirection Direction
		{
			get
			{
				return PType == 0 ? ParameterDirection.Input : ParameterDirection.Ouput;
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
					case "INT64":
						t = Nullable ? typeof(Int64?) : typeof(Int64);
						break;

					case "BLOB":
						t = Nullable ? typeof(Byte?[]) : typeof(Byte[]);
						break;

					case "DOUBLE PRECISION":
						t = Nullable ? typeof(Double?) : typeof(Double);
						break;

					case "FLOAT":
						t = Nullable ? typeof(float?) : typeof(float);
						break;

					case "DECIMAL":
					case "NUMERIC":
						t = Nullable ? typeof(Decimal?) : typeof(Decimal);
						break;

					case "SMALLINT":
					case "SHORT":
						t = Nullable ? typeof(Int16?) : typeof(Int16);
						break;

					case "DATE":
					case "TIME":
					case "TIMESTAMP":
						t = Nullable ? typeof(DateTime?) : typeof(DateTime);
						break;

					case "INTEGER":
					case "LONG":
						t = Nullable ? typeof(Int32?) : typeof(Int32);
						break;


					case "VARCHAR":
					case "CHAR":
					case "VARYING":
					case "TEXT":
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

