using System;

namespace NServiceKit.Common.Tests.Models{

    /// <summary>A model with identifier only.</summary>
	public class ModelWithIdOnly
	{
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		public long Id
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Common.Tests.Models.ModelWithIdOnly class.
        /// </summary>
		public ModelWithIdOnly()
		{
		}

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Common.Tests.Models.ModelWithIdOnly class.
        /// </summary>
        /// <param name="id">The identifier.</param>
		public ModelWithIdOnly(long id)
		{
			this.Id = id;
		}
	}
}