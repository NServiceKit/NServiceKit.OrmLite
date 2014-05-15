using System;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Common.DataModel{

    /// <summary>A territory.</summary>
	[Alias("Territories")]
	public class Territory : IHasStringId, IHasId<string>
	{
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[StringLength(20)]
		[Alias("TerritoryID")]
		public string Id
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier of the region.</summary>
        /// <value>The identifier of the region.</value>
		[References(typeof(Region))]
		[Alias("RegionID")]
		public int RegionId
		{
			get;
			set;
		}

        /// <summary>Gets or sets information describing the territory.</summary>
        /// <value>Information describing the territory.</value>
		[StringLength(50)]
		[Required]
		public string TerritoryDescription
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.Territory class.
        /// </summary>
		public Territory()
		{
		}
	}
}