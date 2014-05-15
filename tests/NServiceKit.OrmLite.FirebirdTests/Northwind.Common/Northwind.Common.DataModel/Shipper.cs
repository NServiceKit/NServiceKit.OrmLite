using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using System.ComponentModel.DataAnnotations;
using System;

namespace Northwind.Common.DataModel{

    /// <summary>A shipper.</summary>
	[Alias("Shippers")]
	public class Shipper : IHasIntId, IHasId<int>
	{
        /// <summary>Gets or sets the name of the company.</summary>
        /// <value>The name of the company.</value>
		[Required]
		[Index(Unique=true)]
		[StringLength(40)]
		public string CompanyName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[Alias("ShipperID")]
		[AutoIncrement]
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

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.Shipper class.
        /// </summary>
		public Shipper()
		{
		}
	}
}