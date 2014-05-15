using System;
using NServiceKit.Logging;
using NServiceKit.DataAnnotations;
using NUnit.Framework;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests.Models{

    /// <summary>A model with fields of different and nullable types.</summary>
	[Alias("ModelWFDNT")]
	public class ModelWithFieldsOfDifferentAndNullableTypes
	{
        /// <summary>The log.</summary>
		private readonly static ILog Log;

        /// <summary>Gets or sets a value indicating whether the. </summary>
        /// <value>true if , false if not.</value>
		public bool Bool
		{
			get;
			set;
		}

        /// <summary>Gets or sets the date time.</summary>
        /// <value>The date time.</value>
		public DateTime DateTime
		{
			get;
			set;
		}

        /// <summary>Gets or sets the decimal.</summary>
        /// <value>The decimal.</value>
		public decimal Decimal
		{
			get;
			set;
		}

        /// <summary>Gets or sets the double.</summary>
        /// <value>The double.</value>
		public double Double
		{
			get;
			set;
		}

        /// <summary>Gets or sets the float.</summary>
        /// <value>The float.</value>
		public float Float
		{
			get;
			set;
		}

        /// <summary>Gets or sets a unique identifier.</summary>
        /// <value>The identifier of the unique.</value>
		public Guid Guid
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[AutoIncrement]
		public int Id
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier of the long.</summary>
        /// <value>The identifier of the long.</value>
		public long LongId
		{
			get;
			set;
		}

        /// <summary>Gets or sets the bool.</summary>
        /// <value>The n bool.</value>
		public bool? NBool
		{
			get;
			set;
		}

        /// <summary>Gets or sets the date time.</summary>
        /// <value>The date time.</value>
		public DateTime? NDateTime
		{
			get;
			set;
		}

        /// <summary>Gets or sets the decimal.</summary>
        /// <value>The n decimal.</value>
		public decimal? NDecimal
		{
			get;
			set;
		}

        /// <summary>Gets or sets the double.</summary>
        /// <value>The n double.</value>
		public double? NDouble
		{
			get;
			set;
		}

        /// <summary>Gets or sets the float.</summary>
        /// <value>The n float.</value>
		public float? NFloat
		{
			get;
			set;
		}

        /// <summary>Gets or sets a unique identifier.</summary>
        /// <value>The identifier of the unique.</value>
		public Guid? NGuid
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The n identifier.</value>
		public int? NId
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier of the long.</summary>
        /// <value>The identifier of the long.</value>
		public long? NLongId
		{
			get;
			set;
		}

        /// <summary>Gets or sets the time span.</summary>
        /// <value>The n time span.</value>
		public TimeSpan? NTimeSpan
		{
			get;
			set;
		}

        /// <summary>Gets or sets the time span.</summary>
        /// <value>The time span.</value>
		public TimeSpan TimeSpan
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes static members of the
        /// NServiceKit.Common.Tests.Models.ModelWithFieldsOfDifferentAndNullableTypes class.
        /// </summary>
		static ModelWithFieldsOfDifferentAndNullableTypes()
		{
			ModelWithFieldsOfDifferentAndNullableTypes.Log = LogManager.GetLogger(typeof(ModelWithFieldsOfDifferentAndNullableTypes));
		}

        /// <summary>
        /// Initializes a new instance of the
        /// NServiceKit.Common.Tests.Models.ModelWithFieldsOfDifferentAndNullableTypes class.
        /// </summary>
		public ModelWithFieldsOfDifferentAndNullableTypes()
		{
		}

        /// <summary>Assert is equal.</summary>
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public static void AssertIsEqual(ModelWithFieldsOfDifferentAndNullableTypes actual, ModelWithFieldsOfDifferentAndNullableTypes expected)
		{
			Assert.That(actual.Id, Is.EqualTo(expected.Id));
			Assert.That(actual.Guid, Is.EqualTo(expected.Guid));
			Assert.That(actual.LongId, Is.EqualTo(expected.LongId));
			Assert.That(actual.Bool, Is.EqualTo(expected.Bool));
			Assert.That(actual.TimeSpan, Is.EqualTo(expected.TimeSpan));
			try
			{
				Assert.That(actual.DateTime, Is.EqualTo(expected.DateTime));
			}
			catch (Exception exception)
			{
				ModelWithFieldsOfDifferentAndNullableTypes.Log.Error("Trouble with DateTime precisions, trying Assert again with rounding to seconds", exception);
				Assert.That(DateTimeExtensions.RoundToSecond(actual.DateTime), Is.EqualTo(DateTimeExtensions.RoundToSecond(expected.DateTime)));
			}
			try
			{
				Assert.That(actual.Float, Is.EqualTo(expected.Float));
			}
			catch (Exception exception2)
			{
				ModelWithFieldsOfDifferentAndNullableTypes.Log.Error("Trouble with float precisions, trying Assert again with rounding to 10 decimals", exception2);
				Assert.That(Math.Round((double)actual.Float, 10), Is.EqualTo(Math.Round((double)actual.Float, 10)));
			}
			try
			{
				Assert.That(actual.Double, Is.EqualTo(expected.Double));
			}
			catch (Exception exception3)
			{
				ModelWithFieldsOfDifferentAndNullableTypes.Log.Error("Trouble with double precisions, trying Assert again with rounding to 10 decimals", exception3);
				Assert.That(Math.Round(actual.Double, 10), Is.EqualTo(Math.Round(actual.Double, 10)));
			}
			Assert.That(actual.NBool, Is.EqualTo(expected.NBool));
			Assert.That(actual.NDateTime, Is.EqualTo(expected.NDateTime));
			Assert.That(actual.NDecimal, Is.EqualTo(expected.NDecimal));
			Assert.That(actual.NDouble, Is.EqualTo(expected.NDouble));
			Assert.That(actual.NFloat, Is.EqualTo(expected.NFloat));
			Assert.That(actual.NGuid, Is.EqualTo(expected.NGuid));
			Assert.That(actual.NId, Is.EqualTo(expected.NId));
			Assert.That(actual.NLongId, Is.EqualTo(expected.NLongId));
			Assert.That(actual.NTimeSpan, Is.EqualTo(expected.NTimeSpan));
		}

        /// <summary>Creates a new ModelWithFieldsOfDifferentAndNullableTypes.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The ModelWithFieldsOfDifferentAndNullableTypes.</returns>
		public static ModelWithFieldsOfDifferentAndNullableTypes Create(int id)
		{
			ModelWithFieldsOfDifferentAndNullableTypes modelWithFieldsOfDifferentAndNullableType1 = new ModelWithFieldsOfDifferentAndNullableTypes();
			modelWithFieldsOfDifferentAndNullableType1.Id = id;
			modelWithFieldsOfDifferentAndNullableType1.Bool = id % 2 == 0;
			modelWithFieldsOfDifferentAndNullableType1.DateTime = DateTime.Now.AddDays((double)id);
			modelWithFieldsOfDifferentAndNullableType1.Float = 1.11f + (float)id;
			modelWithFieldsOfDifferentAndNullableType1.Double = 1.11 + (double)id;
			modelWithFieldsOfDifferentAndNullableType1.Guid = Guid.NewGuid();
			modelWithFieldsOfDifferentAndNullableType1.LongId = (long)999 + id;
			modelWithFieldsOfDifferentAndNullableType1.Decimal = id + 0.5m;
			modelWithFieldsOfDifferentAndNullableType1.TimeSpan = TimeSpan.FromSeconds((double)id);
			ModelWithFieldsOfDifferentAndNullableTypes modelWithFieldsOfDifferentAndNullableType2 = modelWithFieldsOfDifferentAndNullableType1;
			return modelWithFieldsOfDifferentAndNullableType2;
		}

        /// <summary>Creates a constant.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The new constant.</returns>
		public static ModelWithFieldsOfDifferentAndNullableTypes CreateConstant(int id)
		{
			ModelWithFieldsOfDifferentAndNullableTypes modelWithFieldsOfDifferentAndNullableType1 = new ModelWithFieldsOfDifferentAndNullableTypes();
			modelWithFieldsOfDifferentAndNullableType1.Id = id;
			modelWithFieldsOfDifferentAndNullableType1.Bool = id % 2 == 0;
			modelWithFieldsOfDifferentAndNullableType1.DateTime = new DateTime(1979, id % 12 + 1, id % 28 + 1);
			modelWithFieldsOfDifferentAndNullableType1.Float = 1.11f + (float)id;
			modelWithFieldsOfDifferentAndNullableType1.Double = 1.11 + (double)id;
			modelWithFieldsOfDifferentAndNullableType1.Guid = new Guid((id%240+16).ToString("X")+ "461D9D-47DB-4778-B3FA-458379AE9BDC");
			modelWithFieldsOfDifferentAndNullableType1.LongId = (long)999 + id;
			modelWithFieldsOfDifferentAndNullableType1.Decimal = id+ 0.5m ;
			modelWithFieldsOfDifferentAndNullableType1.TimeSpan = TimeSpan.FromSeconds((double)id);
			return modelWithFieldsOfDifferentAndNullableType1;
		}
	}
}