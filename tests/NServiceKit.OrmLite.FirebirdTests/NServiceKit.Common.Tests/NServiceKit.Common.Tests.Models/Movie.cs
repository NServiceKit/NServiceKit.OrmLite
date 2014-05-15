using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using NServiceKit.Common;

namespace NServiceKit.Common.Tests.Models{

    /// <summary>A movie.</summary>
	public class Movie{

        /// <summary>Gets or sets the director.</summary>
        /// <value>The director.</value>
		public string Director
		{
			get;
			set;
		}

        /// <summary>Gets or sets the genres.</summary>
        /// <value>The genres.</value>
		public List<string> Genres
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

        /// <summary>Gets or sets the rating.</summary>
        /// <value>The rating.</value>
		public decimal Rating
		{
			get;
			set;
		}

        /// <summary>Gets or sets the release date.</summary>
        /// <value>The release date.</value>
		public DateTime ReleaseDate
		{
			get;
			set;
		}

        /// <summary>Gets or sets the tag line.</summary>
        /// <value>The tag line.</value>
		public string TagLine
		{
			get;
			set;
		}

        /// <summary>Gets or sets the title.</summary>
        /// <value>The title.</value>
		public string Title
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Common.Tests.Models.Movie class.
        /// </summary>
		public Movie()
		{
			this.Genres = new List<string>();
		}

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="other">The movie to compare to this object.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
		public bool Equals(Movie other)
        {
            return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || (object.Equals(other.Id, this.Id) && object.Equals(other.Title, this.Title) && other.Rating == this.Rating && object.Equals(other.Director, this.Director) && other.ReleaseDate.Equals(this.ReleaseDate) && object.Equals(other.TagLine, this.TagLine) && EnumerableExtensions.EquivalentTo<string>(this.Genres, other.Genres)));
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (obj.GetType() == typeof(Movie) && this.Equals((Movie)obj)));
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			int result = (this.Id != null) ? this.Id.GetHashCode() : 0;
	            result = (result * 397 ^ ((this.Title != null) ? this.Title.GetHashCode() : 0));
	            result = (result * 397 ^ this.Rating.GetHashCode());
	            result = (result * 397 ^ ((this.Director != null) ? this.Director.GetHashCode() : 0));
	            result = (result * 397 ^ this.ReleaseDate.GetHashCode());
	            result = (result * 397 ^ ((this.TagLine != null) ? this.TagLine.GetHashCode() : 0));
	            return result * 397 ^ ((this.Genres != null) ? this.Genres.GetHashCode() : 0);
		}
	}
}