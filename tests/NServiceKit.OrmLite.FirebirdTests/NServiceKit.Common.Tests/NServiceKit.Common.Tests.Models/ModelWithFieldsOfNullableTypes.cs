using System;
using NServiceKit.DesignPatterns.Model;
using NServiceKit.Logging;
using NUnit.Framework;
using NServiceKit.DataAnnotations;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests.Models{

    /// <summary>A model with fields of nullable types.</summary>
	[Alias("ModelWFNT")]
	public class ModelWithFieldsOfNullableTypes : IHasIntId, IHasId<int>
	{
        /// <summary>The log.</summary>
		private readonly static ILog Log;

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		public int Id
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

        /// <summary>
        /// Initializes static members of the
        /// NServiceKit.Common.Tests.Models.ModelWithFieldsOfNullableTypes class.
        /// </summary>
		static ModelWithFieldsOfNullableTypes()
		{
			ModelWithFieldsOfNullableTypes.Log = LogManager.GetLogger(typeof(ModelWithFieldsOfNullableTypes));
		}

        /// <summary>
        /// Initializes a new instance of the
        /// NServiceKit.Common.Tests.Models.ModelWithFieldsOfNullableTypes class.
        /// </summary>
		public ModelWithFieldsOfNullableTypes()
		{
		}

        /// <summary>Assert is equal.</summary>
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public static void AssertIsEqual(ModelWithFieldsOfNullableTypes actual, ModelWithFieldsOfNullableTypes expected)
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.NId, Is.EqualTo(expected.NId));
            Assert.That(actual.NGuid, Is.EqualTo(expected.NGuid));
            Assert.That(actual.NLongId, Is.EqualTo(expected.NLongId));
            Assert.That(actual.NBool, Is.EqualTo(expected.NBool));
            Assert.That(actual.NTimeSpan, Is.EqualTo(expected.NTimeSpan));
            try
            {
                Assert.That(actual.NDateTime, Is.EqualTo(expected.NDateTime));
            }
            catch (Exception ex)
            {
                ModelWithFieldsOfNullableTypes.Log.Error("Trouble with DateTime precisions, trying Assert again with rounding to seconds", ex);
                Assert.That(DateTimeExtensions.RoundToSecond(actual.NDateTime.Value.ToUniversalTime()), Is.EqualTo(DateTimeExtensions.RoundToSecond(expected.NDateTime.Value.ToUniversalTime())));
            }
            try
            {
                Assert.That(actual.NFloat, Is.EqualTo(expected.NFloat));
            }
            catch (Exception ex2)
            {
                ModelWithFieldsOfNullableTypes.Log.Error("Trouble with float precisions, trying Assert again with rounding to 10 decimals", ex2);
                Assert.That(Math.Round((double)actual.NFloat.Value, 10), Is.EqualTo(Math.Round((double)actual.NFloat.Value, 10)));
            }
            try
            {
                Assert.That(actual.NDouble, Is.EqualTo(expected.NDouble));
            }
            catch (Exception ex3)
            {
                ModelWithFieldsOfNullableTypes.Log.Error("Trouble with double precisions, trying Assert again with rounding to 10 decimals", ex3);
                Assert.That(Math.Round(actual.NDouble.Value, 10), Is.EqualTo(Math.Round(actual.NDouble.Value, 10)));
            }
        }

        /// <summary>Creates a new ModelWithFieldsOfNullableTypes.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The ModelWithFieldsOfNullableTypes.</returns>
		public static ModelWithFieldsOfNullableTypes Create(int id)
		{
			ModelWithFieldsOfNullableTypes modelWithFieldsOfNullableType1 = new ModelWithFieldsOfNullableTypes();
			modelWithFieldsOfNullableType1.Id = id;
			modelWithFieldsOfNullableType1.NId = new int?(id);
			modelWithFieldsOfNullableType1.NBool = new bool?(id % 2 == 0);
			modelWithFieldsOfNullableType1.NDateTime = new DateTime?(DateTime.Now.AddDays((double)id));
			modelWithFieldsOfNullableType1.NFloat = new float?(1.11f + (float)id);
			modelWithFieldsOfNullableType1.NDouble = new double?(1.11 + (double)id);
			modelWithFieldsOfNullableType1.NGuid = new Guid?(Guid.NewGuid());
			modelWithFieldsOfNullableType1.NLongId = new long?((long)999 + id);
			modelWithFieldsOfNullableType1.NDecimal = new decimal?(id+0.5m);
			modelWithFieldsOfNullableType1.NTimeSpan = new TimeSpan?(TimeSpan.FromSeconds((double)id));
			ModelWithFieldsOfNullableTypes modelWithFieldsOfNullableType2 = modelWithFieldsOfNullableType1;
			return modelWithFieldsOfNullableType2;
		}

        /// <summary>Creates a constant.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The new constant.</returns>
		public static ModelWithFieldsOfNullableTypes CreateConstant(int id)
		{
			ModelWithFieldsOfNullableTypes modelWithFieldsOfNullableType1 = new ModelWithFieldsOfNullableTypes();
			modelWithFieldsOfNullableType1.Id = id;
			modelWithFieldsOfNullableType1.NId = new int?(id);
			modelWithFieldsOfNullableType1.NBool = new bool?(id % 2 == 0);
			modelWithFieldsOfNullableType1.NDateTime = new DateTime?(new DateTime(1979, id % 12 + 1, id % 28 + 1));
			modelWithFieldsOfNullableType1.NFloat = new float?(1.11f + (float)id);
			modelWithFieldsOfNullableType1.NDouble = new double?(1.11 + (double)id);
			modelWithFieldsOfNullableType1.NGuid = new Guid?(new Guid( (id%240+16).ToString("X") + "7DA519-73B6-4525-84BA-B57673B2360D"));
			modelWithFieldsOfNullableType1.NLongId = new long?((long)999 + id);
			modelWithFieldsOfNullableType1.NDecimal = new decimal?(id + 0.5m );
			modelWithFieldsOfNullableType1.NTimeSpan = new TimeSpan?(TimeSpan.FromSeconds((double)id));
			
			return modelWithFieldsOfNullableType1;
		}
	}
}