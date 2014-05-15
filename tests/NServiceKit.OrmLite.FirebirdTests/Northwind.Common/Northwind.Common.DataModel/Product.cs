using System;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Common.DataModel{

    /// <summary>A product.</summary>
	[Alias("Products")]
	public class Product : IHasIntId, IHasId<int>
	{
        /// <summary>Gets or sets the identifier of the category.</summary>
        /// <value>The identifier of the category.</value>
		[References(typeof(Category))]
		[Alias("CategoryID")]
		[Index]
		public int CategoryId
		{
			get;
			set;
		}

        /// <summary>Gets or sets a value indicating whether the discontinued.</summary>
        /// <value>true if discontinued, false if not.</value>
		public bool Discontinued
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[AutoIncrement]
		[Alias("ProductID")]
		public int Id
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name of the product.</summary>
        /// <value>The name of the product.</value>
		[Index]
		[Required]
		[StringLength(40)]
		public string ProductName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the quantity per unit.</summary>
        /// <value>The quantity per unit.</value>
		[StringLength(20)]
		public string QuantityPerUnit
		{
			get;
			set;
		}

        /// <summary>Gets or sets the reorder level.</summary>
        /// <value>The reorder level.</value>
		public short ReorderLevel
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier of the supplier.</summary>
        /// <value>The identifier of the supplier.</value>
		[Alias("SupplierID")]
		[References(typeof(Supplier))]
		[Index]
		public int SupplierId
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

        /// <summary>Gets or sets the units in stock.</summary>
        /// <value>The units in stock.</value>
		public short UnitsInStock
		{
			get;
			set;
		}

        /// <summary>Gets or sets the units on order.</summary>
        /// <value>The units on order.</value>
		public short UnitsOnOrder
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.Product class.
        /// </summary>
		public Product()
		{
		}
	}
}