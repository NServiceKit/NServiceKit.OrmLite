using System;
using System.Collections.Generic;
using NUnit.Framework;
using NServiceKit.DataAnnotations;

namespace NServiceKit.Common.Tests.Models{

    /// <summary>A model with complex types.</summary>
	[Alias("ModelWComplexT")]
	public class ModelWithComplexTypes
	{
        /// <summary>Gets or sets the child.</summary>
        /// <value>The child.</value>
		public ModelWithComplexTypes Child
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		public long Id
		{
			get;
			set;
		}

        /// <summary>Gets or sets a list of ints.</summary>
        /// <value>A List of ints.</value>
		public List<int> IntList
		{
			get;
			set;
		}

        /// <summary>Gets or sets the int map.</summary>
        /// <value>The int map.</value>
		public Dictionary<int, int> IntMap
		{
			get;
			set;
		}

        /// <summary>Gets or sets a list of strings.</summary>
        /// <value>A List of strings.</value>
		public List<string> StringList
		{
			get;
			set;
		}

        /// <summary>Gets or sets the string map.</summary>
        /// <value>The string map.</value>
		public Dictionary<string, string> StringMap
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Common.Tests.Models.ModelWithComplexTypes
        /// class.
        /// </summary>
		public ModelWithComplexTypes()
		{
			this.StringList = new List<string>();
			this.IntList = new List<int>();
			this.StringMap = new Dictionary<string, string>();
			this.IntMap = new Dictionary<int, int>();
		}

        /// <summary>Assert is equal.</summary>
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public static void AssertIsEqual(ModelWithComplexTypes actual, ModelWithComplexTypes expected)
		{
			Assert.That(actual.Id, Is.EqualTo(expected.Id));
			Assert.That(actual.StringList, Is.EquivalentTo(expected.StringList));
			Assert.That(actual.IntList, Is.EquivalentTo(expected.IntList));
			Assert.That(actual.StringMap, Is.EquivalentTo(expected.StringMap));
			Assert.That(actual.IntMap, Is.EquivalentTo(expected.IntMap));
			if (expected.Child == null)
			{
				Assert.That(actual.Child, Is.Null);
				return;
			}
			Assert.That(actual.Child, Is.Not.Null);
			ModelWithComplexTypes.AssertIsEqual(actual.Child, expected.Child);
		}

        /// <summary>Creates a new ModelWithComplexTypes.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The ModelWithComplexTypes.</returns>
		public static ModelWithComplexTypes Create(int id)
		{
			ModelWithComplexTypes modelWithComplexType1 = new ModelWithComplexTypes();
			modelWithComplexType1.Id = (long)id;
			modelWithComplexType1.StringList.Add(string.Concat("val", id, 1));
			modelWithComplexType1.StringList.Add(string.Concat("val", id, 2));
			modelWithComplexType1.StringList.Add(string.Concat("val", id, 3));
			modelWithComplexType1.IntList.Add(id + 1);
			modelWithComplexType1.IntList.Add(id + 2);
			modelWithComplexType1.IntList.Add(id + 3);
			modelWithComplexType1.StringMap.Add(string.Concat("key", id, 1), string.Concat("val", id, 1));
			modelWithComplexType1.StringMap.Add(string.Concat("key", id, 2), string.Concat("val", id, 2));
			modelWithComplexType1.StringMap.Add(string.Concat("key", id, 3), string.Concat("val", id, 3));
			modelWithComplexType1.IntMap.Add(id + 1, id + 2);
			modelWithComplexType1.IntMap.Add(id + 3, id + 4);
			modelWithComplexType1.IntMap.Add(id + 5, id + 6);
			modelWithComplexType1.Child = new ModelWithComplexTypes(){
				Id= (long)(id*2),
			};
			
			return modelWithComplexType1;
		}

        /// <summary>Creates a constant.</summary>
        /// <param name="i">Zero-based index of the.</param>
        /// <returns>The new constant.</returns>
		public static ModelWithComplexTypes CreateConstant(int i)
		{
			return ModelWithComplexTypes.Create(i);
		}
	}
}