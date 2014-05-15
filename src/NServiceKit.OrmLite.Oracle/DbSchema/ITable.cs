
namespace NServiceKit.OrmLite.Oracle.DbSchema
{
    /// <summary>Interface for table.</summary>
	public interface ITable
	{
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		string Name { get; set; }

        /// <summary>Gets or sets the owner.</summary>
        /// <value>The owner.</value>
		string Owner { get; set; }
	}
}

