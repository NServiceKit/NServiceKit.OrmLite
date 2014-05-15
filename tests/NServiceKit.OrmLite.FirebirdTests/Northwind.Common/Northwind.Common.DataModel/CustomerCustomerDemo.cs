using NServiceKit.DesignPatterns.Model;
using NServiceKit.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System;

namespace Northwind.Common.DataModel{

    /// <summary>A customer demo.</summary>
	[Alias("CustomerDemo")]
	public class CustomerCustomerDemo : IHasStringId, IHasId<string>{

        /// <summary>Gets or sets the identifier of the customer type.</summary>
        /// <value>The identifier of the customer type.</value>
		[Alias("CustomerTypeID")]
		[StringLength(10)]
		public string CustomerTypeId
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[Alias("CustomerID")]
		[StringLength(5)]
		public string Id
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.CustomerCustomerDemo class.
        /// </summary>
		public CustomerCustomerDemo()
		{
		}
	}
}