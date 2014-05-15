using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using System.ComponentModel.DataAnnotations;
using System;

namespace Northwind.Common.DataModel{

    /// <summary>A customer.</summary>
	[Alias("Customers")]
	public class Customer : IHasStringId, IHasId<string>
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
		[Index]
		[StringLength(15)]
		public string City
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name of the company.</summary>
        /// <value>The name of the company.</value>
		[Index]
		[Required]
		[StringLength(40)]
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

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[Alias("CustomerID")]
		[Required]
		[StringLength(5)]
		public string Id
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

        /// <summary>Gets or sets the picture.</summary>
        /// <value>The picture.</value>
		public byte[] Picture
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
		[Index]
		public string Region
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.Customer class.
        /// </summary>
		public Customer()
		{
		}
	}
}