using System;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Common.DataModel{

    /// <summary>An order.</summary>
	[Alias("Orders")]
	public class Order : IHasIntId, IHasId<int>
	{
        /// <summary>Gets or sets the identifier of the customer.</summary>
        /// <value>The identifier of the customer.</value>
		[Alias("CustomerID")]
		[Index]
		[StringLength(5)]
		[References(typeof(Customer))]
		public string CustomerId
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier of the employee.</summary>
        /// <value>The identifier of the employee.</value>
		[References(typeof(Customer))]
		[Alias("EmployeeID")]
		[Index]
		public int EmployeeId
		{
			get;
			set;
		}

        /// <summary>Gets or sets the freight.</summary>
        /// <value>The freight.</value>
		public decimal Freight
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[AutoIncrement]
		[Alias("OrderID")]
		public int Id
		{
			get;
			set;
		}

        /// <summary>Gets or sets the order date.</summary>
        /// <value>The order date.</value>
		[Index]
		public DateTime? OrderDate
		{
			get;
			set;
		}

        /// <summary>Gets or sets the required date.</summary>
        /// <value>The required date.</value>
		public DateTime? RequiredDate
		{
			get;
			set;
		}

        /// <summary>Gets or sets the ship address.</summary>
        /// <value>The ship address.</value>
		[StringLength(60)]
		public string ShipAddress
		{
			get;
			set;
		}

        /// <summary>Gets or sets the ship city.</summary>
        /// <value>The ship city.</value>
		[StringLength(15)]
		public string ShipCity
		{
			get;
			set;
		}

        /// <summary>Gets or sets the ship country.</summary>
        /// <value>The ship country.</value>
		[StringLength(15)]
		public string ShipCountry
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name of the ship.</summary>
        /// <value>The name of the ship.</value>
		[StringLength(40)]
		public string ShipName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the shipped date.</summary>
        /// <value>The shipped date.</value>
		[Index]
		public DateTime? ShippedDate
		{
			get;
			set;
		}

        /// <summary>Gets or sets the ship postal code.</summary>
        /// <value>The ship postal code.</value>
		[StringLength(10)]
		[Index]
		public string ShipPostalCode
		{
			get;
			set;
		}

        /// <summary>Gets or sets the ship region.</summary>
        /// <value>The ship region.</value>
		[StringLength(15)]
		public string ShipRegion
		{
			get;
			set;
		}

        /// <summary>Gets or sets the ship via.</summary>
        /// <value>The ship via.</value>
		[Index]
		[References(typeof(Shipper))]
		public int? ShipVia
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.Order class.
        /// </summary>
		public Order()
		{
		}
	}
}