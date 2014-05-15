using System;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Common.DataModel{

    /// <summary>A supplier.</summary>
	[Alias("Suppliers")]
	public class Supplier : IHasIntId, IHasId<int>
	{
        /// <summary>Gets or sets the address.</summary>
        /// <value>The address.</value>
		[StringLength(60)]
		public string Address
		{
			get;
			set;
		}

        /// <summary>Gets or sets the city.</summary>
        /// <value>The city.</value>
		[StringLength(15)]
		public string City
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name of the company.</summary>
        /// <value>The name of the company.</value>
		[StringLength(40)]
		[Required]
		[Index]
		public string CompanyName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name of the contact.</summary>
        /// <value>The name of the contact.</value>
		[StringLength(30)]
		public string ContactName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the contact title.</summary>
        /// <value>The contact title.</value>
		[StringLength(30)]
		public string ContactTitle
		{
			get;
			set;
		}

        /// <summary>Gets or sets the country.</summary>
        /// <value>The country.</value>
		[StringLength(15)]
		public string Country
		{
			get;
			set;
		}

        /// <summary>Gets or sets the fax.</summary>
        /// <value>The fax.</value>
		[StringLength(24)]
		public string Fax
		{
			get;
			set;
		}

        /// <summary>Gets or sets the home page.</summary>
        /// <value>The home page.</value>
		public string HomePage
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[AutoIncrement]
		[Alias("SupplierID")]
		public int Id
		{
			get;
			set;
		}

        /// <summary>Gets or sets the phone.</summary>
        /// <value>The phone.</value>
		[StringLength(24)]
		public string Phone
		{
			get;
			set;
		}

        /// <summary>Gets or sets the postal code.</summary>
        /// <value>The postal code.</value>
		[StringLength(10)]
		[Index]
		public string PostalCode
		{
			get;
			set;
		}

        /// <summary>Gets or sets the region.</summary>
        /// <value>The region.</value>
		[StringLength(15)]
		public string Region
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.Supplier class.
        /// </summary>
		public Supplier()
		{
		}
	}
}