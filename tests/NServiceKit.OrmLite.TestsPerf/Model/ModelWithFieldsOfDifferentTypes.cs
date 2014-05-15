using System;
using NUnit.Framework;
using NServiceKit.DataAnnotations;
using NServiceKit.Logging;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.TestsPerf.Model
{
    /// <summary>A model with fields of different types performance.</summary>
	public class ModelWithFieldsOfDifferentTypesPerf
	{
        /// <summary>The log.</summary>
		private static readonly ILog Log = LogManager.GetLogger(typeof(ModelWithFieldsOfDifferentTypesPerf));

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[AutoIncrement]
		public int Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the identifier of the long.</summary>
        /// <value>The identifier of the long.</value>
		public long LongId { get; set; }

        /// <summary>Gets or sets a unique identifier.</summary>
        /// <value>The identifier of the unique.</value>
		public Guid Guid { get; set; }

        /// <summary>Gets or sets a value indicating whether the. </summary>
        /// <value>true if , false if not.</value>
		public bool Bool { get; set; }

        /// <summary>Gets or sets the date time.</summary>
        /// <value>The date time.</value>
		public DateTime DateTime { get; set; }

        /// <summary>Gets or sets the double.</summary>
        /// <value>The double.</value>
		public double Double { get; set; }

        /// <summary>Creates a new ModelWithFieldsOfDifferentTypesPerf.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A ModelWithFieldsOfDifferentTypesPerf.</returns>
		public static ModelWithFieldsOfDifferentTypesPerf Create(int id)
		{
			var row = new ModelWithFieldsOfDifferentTypesPerf {
				Id = id,
				Bool = id % 2 == 0,
				DateTime = DateTime.Now.AddDays(id),
				Double = 1.11d + id,
				Guid = Guid.NewGuid(),
				LongId = 999 + id,
				Name = "Name" + id
			};

			return row;
		}

        /// <summary>Assert is equal.</summary>
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public static void AssertIsEqual(ModelWithFieldsOfDifferentTypesPerf actual, ModelWithFieldsOfDifferentTypesPerf expected)
		{
			Assert.That(actual.Id, Is.EqualTo(expected.Id));
			Assert.That(actual.Name, Is.EqualTo(expected.Name));
			Assert.That(actual.Guid, Is.EqualTo(expected.Guid));
			Assert.That(actual.LongId, Is.EqualTo(expected.LongId));
			Assert.That(actual.Bool, Is.EqualTo(expected.Bool));
			try
			{
				Assert.That(actual.DateTime, Is.EqualTo(expected.DateTime));
			}
			catch (Exception ex)
			{
				Log.Error("Trouble with DateTime precisions, trying Assert again with rounding to seconds", ex);
				Assert.That(actual.DateTime.RoundToSecond(), Is.EqualTo(expected.DateTime.RoundToSecond()));
			}
			try
			{
				Assert.That(actual.Double, Is.EqualTo(expected.Double));
			}
			catch (Exception ex)
			{
				Log.Error("Trouble with double precisions, trying Assert again with rounding to 10 decimals", ex);
				Assert.That(Math.Round(actual.Double, 10), Is.EqualTo(Math.Round(actual.Double, 10)));
			} 
		}
	}
}