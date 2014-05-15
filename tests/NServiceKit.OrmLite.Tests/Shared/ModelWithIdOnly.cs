namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A model with identifier only.</summary>
	public class ModelWithIdOnly
	{
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
			Id = id;
		}

        /// <summary>must be long as you cannot have a table with only an autoincrement field.</summary>
        /// <value>The identifier.</value>
		public long Id { get; set; }

	}
}