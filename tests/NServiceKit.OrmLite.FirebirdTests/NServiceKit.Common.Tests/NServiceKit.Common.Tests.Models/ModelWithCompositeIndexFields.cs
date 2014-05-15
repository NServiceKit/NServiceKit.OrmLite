using System;
using NServiceKit.DataAnnotations;

namespace NServiceKit.Common.Tests.Models{

    /// <summary>Gets the ].</summary>
    /// <value>.</value>
	[Alias("ModelWCIF")]
	[CompositeIndex(true, new string[] { "Comp1", "Comp2" })]
	public class ModelWithCompositeIndexFields
	{
        /// <summary>Gets or sets the identifier of the album.</summary>
        /// <value>The identifier of the album.</value>
		public string AlbumId
		{
			get;
			set;
		}

        /// <summary>Gets or sets the composite 1.</summary>
        /// <value>The composite 1.</value>
		[Alias("Comp1")]
		public string Composite1
		{
			get;
			set;
		}

        /// <summary>Gets or sets the composite 2.</summary>
        /// <value>The composite 2.</value>
		[Alias("Comp2")]
		public string Composite2
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		public string Id
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		[Index]
		public string Name
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name of the unique.</summary>
        /// <value>The name of the unique.</value>
		[Index(true)]
		public string UniqueName
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the
        /// NServiceKit.Common.Tests.Models.ModelWithCompositeIndexFields class.
        /// </summary>
		public ModelWithCompositeIndexFields()
		{
		}
	}
}