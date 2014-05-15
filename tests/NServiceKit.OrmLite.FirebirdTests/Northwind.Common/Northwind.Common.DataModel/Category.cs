using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using System.ComponentModel.DataAnnotations;
using System;

namespace Northwind.Common.DataModel{

    /// <summary>A category.</summary>
	[Alias("Categories")]
	public class Category : IHasIntId, IHasId<int>
	{
        /// <summary>Gets or sets the name of the category.</summary>
        /// <value>The name of the category.</value>
		[StringLength(15)]
		[Required]
		[Index]
		public string CategoryName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the description.</summary>
        /// <value>The description.</value>
		[StringLength(10)]
		public string Description
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[Alias("CategoryID")]
		public int Id
		{
			get;
			set;
		}

        /// <summary>Gets or sets the picture.</summary>
        /// <value>The picture.</value>
		public byte[] Picture
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.Category class.
        /// </summary>
		public Category()
		{
		}
	}
}