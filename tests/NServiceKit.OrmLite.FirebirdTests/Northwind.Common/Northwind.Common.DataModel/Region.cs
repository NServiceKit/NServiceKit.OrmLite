using System;
using NServiceKit.DesignPatterns.Model;
using NServiceKit.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Common.DataModel{

    /// <summary>A region.</summary>
	public class Region : IHasIntId, IHasId<int>
	{
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[Alias("RegionID")]
		public int Id
		{
			get;
			set;
		}

        /// <summary>Gets or sets information describing the region.</summary>
        /// <value>Information describing the region.</value>
		[StringLength(50)]
		[Required]
		public string RegionDescription
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.Region class.
        /// </summary>
		public Region()
		{
		}
	}
}