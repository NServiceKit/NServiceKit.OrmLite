using System;

namespace Northwind.Common.DataModel{

    /// <summary>An order detail blob.</summary>
	public class OrderDetailBlob
	{
        /// <summary>Gets or sets the discount.</summary>
        /// <value>The discount.</value>
		public double Discount
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier of the product.</summary>
        /// <value>The identifier of the product.</value>
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
        /// Initializes a new instance of the Northwind.Common.DataModel.OrderDetailBlob class.
        /// </summary>
		public OrderDetailBlob()
		{
		}
	}
}