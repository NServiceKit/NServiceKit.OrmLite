using System;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Common.DataModel{

    /// <summary>An employee territory.</summary>
	[Alias("EmployeeTerritories")]
	public class EmployeeTerritory : IHasStringId, IHasId<string>
	{
        /// <summary>Gets or sets the identifier of the employee.</summary>
        /// <value>The identifier of the employee.</value>
		[Alias("EmployeeID")]
		public int EmployeeId
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
				return string.Concat(this.EmployeeId, "/", this.TerritoryId);
			}
		}

        /// <summary>Gets or sets the identifier of the territory.</summary>
        /// <value>The identifier of the territory.</value>
		[StringLength(20)]
		[Required]
		[Alias("TerritoryID")]
		public string TerritoryId
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.EmployeeTerritory class.
        /// </summary>
		public EmployeeTerritory()
		{
		}
	}
	}