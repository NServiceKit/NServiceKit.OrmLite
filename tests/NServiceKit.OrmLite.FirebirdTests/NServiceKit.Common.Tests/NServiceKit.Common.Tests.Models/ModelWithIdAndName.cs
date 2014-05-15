using System;
using NUnit.Framework;
using NServiceKit.DataAnnotations;

namespace NServiceKit.Common.Tests.Models{

    /// <summary>A model with identifier and name.</summary>
	[Alias("ModelWIN")]
	public class ModelWithIdAndName
	{
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[Sequence("ModelWIN_Id_GEN")]
		public int Id
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		public string Name
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Common.Tests.Models.ModelWithIdAndName
        /// class.
        /// </summary>
		public ModelWithIdAndName()
		{
		}

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Common.Tests.Models.ModelWithIdAndName
        /// class.
        /// </summary>
        /// <param name="id">The identifier.</param>
		public ModelWithIdAndName(int id)
		{
			this.Id = id;
			this.Name = string.Concat("Name", id);
		}

        /// <summary>Assert is equal.</summary>
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public static void AssertIsEqual(ModelWithIdAndName actual, ModelWithIdAndName expected)
		{
			if (actual == null || expected == null)
			{
				Assert.That(actual == expected, Is.True);
				return;
			}
			Assert.That(actual.Id, Is.EqualTo(expected.Id));
			Assert.That(actual.Name, Is.EqualTo(expected.Name));
		}

        /// <summary>Creates a new ModelWithIdAndName.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A ModelWithIdAndName.</returns>
		public static ModelWithIdAndName Create(int id)
		{
			return new ModelWithIdAndName(id);
		}

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="other">The model with identifier and name to compare to this object.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
		public bool Equals(ModelWithIdAndName other)
		{
			if (object.ReferenceEquals(null, other))
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			if (other.Id == this.Id)
			{
				return object.Equals(other.Name, this.Name);
			}
			return false;
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
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof(ModelWithIdAndName))
			{
				return false;
			}
			return this.Equals((ModelWithIdAndName)obj);
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			return this.Id * 397 ^  ( (this.Name != null) ? this.Name.GetHashCode() : 0) ;
		}
	}
}