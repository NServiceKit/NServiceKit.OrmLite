using System;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Common.DataModel{

    /// <summary>A customer demographic.</summary>
	[Alias("Demographics")]
	public class CustomerDemographic : IHasStringId, IHasId<string>
	{
        /// <summary>Gets or sets information describing the customer.</summary>
        /// <value>Information describing the customer.</value>
		public string CustomerDesc
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[StringLength(10)]
		[Alias("TypeID")]
		public string Id
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.CustomerDemographic class.
        /// </summary>
		public CustomerDemographic()
		{
		}
	}
}