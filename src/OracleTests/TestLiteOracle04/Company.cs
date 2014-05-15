using System;
using System.ComponentModel.DataAnnotations;
using NServiceKit.Common;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using NServiceKit.OrmLite;

namespace Database.Records
{
    /// <summary>A company.</summary>
	[Alias("COMPANY")]
	public partial class Company:IHasId<System.Int32>{

        /// <summary>Initializes a new instance of the Database.Records.Company class.</summary>
		public Company(){}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[Alias("ID")]
		[Sequence("COMPANY_ID_GEN")]
		[PrimaryKey]
		[AutoIncrement]
		public System.Int32 Id { get; set;} 

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		[Alias("NAME")]
		public System.String Name { get; set;} 

        /// <summary>Gets or sets the turnover.</summary>
        /// <value>The turnover.</value>
		[Alias("TURNOVER")]
		public System.Single? Turnover { get; set;} 

        /// <summary>Gets or sets the started.</summary>
        /// <value>The started.</value>
		[Alias("STARTED")]
		public System.DateTime? Started { get; set;} 

        /// <summary>Gets or sets the employees.</summary>
        /// <value>The employees.</value>
		[Alias("EMPLOYEES")]
		public System.Int32? Employees { get; set;} 

        /// <summary>Gets or sets the created date.</summary>
        /// <value>The created date.</value>
		[Alias("CREATED_DATE")]
		public System.DateTime? CreatedDate { get; set;} 

        /// <summary>Gets or sets a unique identifier.</summary>
        /// <value>The identifier of the unique.</value>
		[Alias("GUID")]
		public System.Guid? Guid { get; set;} 

        /// <summary>Gets or sets some double.</summary>
        /// <value>some double.</value>
		[Alias("SOMEDOUBLE")]
		public Double SomeDouble { get; set;} 

        /// <summary>Gets or sets a value indicating whether some boolean.</summary>
        /// <value>true if some boolean, false if not.</value>
		[Alias("SOMEBOOL")]
		public bool SomeBoolean { get; set;} 

        /// <summary>A me.</summary>
		public static class Me {

            /// <summary>Gets the name of the table.</summary>
            /// <value>The name of the table.</value>
			public static string TableName { get { return "COMPANY"; }}

            /// <summary>Gets the identifier.</summary>
            /// <value>The identifier.</value>
			public static string Id { get { return "ID"; }}

            /// <summary>Gets the name.</summary>
            /// <value>The name.</value>
			public static string Name { get { return "NAME"; }}

            /// <summary>Gets the turnover.</summary>
            /// <value>The turnover.</value>
			public static string Turnover { get { return "TURNOVER"; }}

            /// <summary>Gets the started.</summary>
            /// <value>The started.</value>
			public static string Started { get { return "STARTED"; }}

            /// <summary>Gets the employees.</summary>
            /// <value>The employees.</value>
			public static string Employees { get { return "EMPLOYEES"; }}

            /// <summary>Gets the created date.</summary>
            /// <value>The created date.</value>
			public static string CreatedDate { get { return "CREATED_DATE"; }}

            /// <summary>Gets a unique identifier.</summary>
            /// <value>The identifier of the unique.</value>
			public static string Guid { get { return "GUID"; }}

		}
	}
}