using System;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Common.DataModel{

    /// <summary>An order detail.</summary>
	[Alias("OrderDetails")]
	public class OrderDetail : IHasStringId, IHasId<string>
	{
        /// <summary>Gets or sets the discount.</summary>
        /// <value>The discount.</value>
		public double Discount
		{
			get;
			set;
		}

        /// <summary>Gets the identifier.</summary>
        /// <value>The identifier.</value>
		public string Id
		{
			get
			{
				return string.Concat(this.OrderId, "/", this.ProductId);
			}
		}

        /// <summary>Gets or sets the identifier of the order.</summary>
        /// <value>The identifier of the order.</value>
		[Index]
		[References(typeof(Order))]
		[Alias("OrderID")]
		public int OrderId
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier of the product.</summary>
        /// <value>The identifier of the product.</value>
		[References(typeof(Product))]
		[Alias("ProductID")]
		[Index]
		public int ProductId
		{
			get;
			set;
		}

        /// <summary>Gets or sets the quantity.</summary>
        /// <value>The quantity.</value>
		public short Quantity
		{
			get;
			set;
		}

        /// <summary>Gets or sets the unit price.</summary>
        /// <value>The unit price.</value>
		public decimal UnitPrice
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.OrderDetail class.
        /// </summary>
		public OrderDetail()
		{
		}
	}
}