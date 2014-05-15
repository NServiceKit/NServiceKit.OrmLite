using System;
using System.ComponentModel.DataAnnotations;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.FirebirdTests.Expressions
{
    /// <summary>An author.</summary>
	public class Author
	{
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.FirebirdTests.Expressions.Author
        /// class.
        /// </summary>
		public Author(){}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[AutoIncrement]
		[Alias("AuthorID")]
		public Int32 Id { get; set;}

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		[Index(Unique = true)]
		[StringLength(40)]
		public string Name { get; set;}

        /// <summary>Gets or sets the Date/Time of the birthday.</summary>
        /// <value>The birthday.</value>
		public DateTime Birthday { get; set;}

        /// <summary>Gets or sets the Date/Time of the last activity.</summary>
        /// <value>The last activity.</value>
		public DateTime ? LastActivity  { get; set;}

        /// <summary>Gets or sets the earnings.</summary>
        /// <value>The earnings.</value>
		public Decimal? Earnings { get; set;}  

        /// <summary>Gets or sets a value indicating whether the active.</summary>
        /// <value>true if active, false if not.</value>
		public bool Active { get; set; } 

        /// <summary>Gets or sets the city.</summary>
        /// <value>The city.</value>
		[StringLength(80)]
		[Alias("JobCity")]
		public string City { get; set;}

        /// <summary>Gets or sets the comments.</summary>
        /// <value>The comments.</value>
		[StringLength(80)]
		[Alias("Comment")]
		public string Comments { get; set;}

        /// <summary>Gets or sets the rate.</summary>
        /// <value>The rate.</value>
		public Int16 Rate{ get; set;}
	}
}

