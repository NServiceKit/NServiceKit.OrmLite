﻿using NUnit.Framework;

namespace NServiceKit.OrmLite.Tests.Expressions
{
    /// <summary>An additive expressions test.</summary>
    public class AdditiveExpressionsTest : ExpressionsTestBase
    {
        /// <summary>Can select constant add expression.</summary>
        [Test]
        public void Can_select_constant_add_expression()
        {
            var expected = new TestType()
            {
                IntColumn = 7,
                BoolColumn = true,
                StringColumn = "test"
            };

            EstablishContext(10, expected);

            var actual = OpenDbConnection().Select<TestType>(q => q.IntColumn == 4 + 3);

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            CollectionAssert.Contains(actual, expected);
        }

        /// <summary>Can select constant subtract expression.</summary>
        [Test]
        public void Can_select_constant_subtract_expression()
        {
            var expected = new TestType()
            {
                IntColumn = 7,
                BoolColumn = true,
                StringColumn = "test"
            };

            EstablishContext(10, expected);

            var actual = OpenDbConnection().Select<TestType>(q => q.IntColumn == 10 - 3);

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            CollectionAssert.Contains(actual, expected);
        }

        /// <summary>Can select variable add expression.</summary>
        [Test]
        public void Can_select_variable_add_expression()
        {
// ReSharper disable ConvertToConstant.Local
            var a = 4;
            var b = 3;
// ReSharper restore ConvertToConstant.Local

            var expected = new TestType()
            {
                IntColumn = 7,
                BoolColumn = true,
                StringColumn = "test"
            };

            EstablishContext(10, expected);

            var actual = OpenDbConnection().Select<TestType>(q => q.IntColumn == a + b);

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            CollectionAssert.Contains(actual, expected);
        }

        /// <summary>Can select variable subtract expression.</summary>
        [Test]
        public void Can_select_variable_subtract_expression()
        {
            // ReSharper disable ConvertToConstant.Local
            var a = 10;
            var b = 3;
            // ReSharper restore ConvertToConstant.Local

            var expected = new TestType()
            {
                IntColumn = 7,
                BoolColumn = true,
                StringColumn = "test"
            };

            EstablishContext(10, expected);

            var actual = OpenDbConnection().Select<TestType>(q => q.IntColumn == a - b);

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            CollectionAssert.Contains(actual, expected);
        }

        /// <summary>Can select method add expression.</summary>
        [Test]
        public void Can_select_method_add_expression()
        {
            var expected = new TestType()
            {
                IntColumn = 7,
                BoolColumn = true,
                StringColumn = "test"
            };

            EstablishContext(10, expected);

            var actual = OpenDbConnection().Select<TestType>(q => q.IntColumn == GetValue(4) + GetValue(3));

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            CollectionAssert.Contains(actual, expected);
        }

        /// <summary>Can select method subtract expression.</summary>
        [Test]
        public void Can_select_method_subtract_expression()
        {
            var expected = new TestType()
            {
                IntColumn = 7,
                BoolColumn = true,
                StringColumn = "test"
            };

            EstablishContext(10, expected);

            var actual = OpenDbConnection().Select<TestType>(q => q.IntColumn == GetValue(10) - GetValue(3));

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            CollectionAssert.Contains(actual, expected);
        }
    }
}