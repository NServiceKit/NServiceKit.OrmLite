using System;
using System.Linq;
using NUnit.Framework;
using NServiceKit.OrmLite.Tests;

namespace NServiceKit.OrmLite.PostgreSQL.Tests.Expressions
{
    /// <summary>The expressions test base.</summary>
    public class ExpressionsTestBase : OrmLiteTestBase
    {
        /// <summary>Setups this object.</summary>
        [SetUp]
        public void Setup()
        {
            OpenDbConnection().CreateTable<TestType>(true);
        }

        /// <summary>Gets a value.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>The value.</returns>
        public T GetValue<T>(T item)
        {
            return item;
        }

        /// <summary>Establish context.</summary>
        /// <param name="numberOfRandomObjects">Number of random objects.</param>
        protected void EstablishContext(int numberOfRandomObjects)
        {
            OrmLiteConfig.ClearCache();
            EstablishContext(numberOfRandomObjects, null);
        }

        /// <summary>Establish context.</summary>
        /// <param name="numberOfRandomObjects">Number of random objects.</param>
        /// <param name="obj">                  A variable-length parameters list containing object.</param>
        protected void EstablishContext(int numberOfRandomObjects, params TestType[] obj)
        {
            if (obj == null)
                obj = new TestType[0];

            using (var con = OpenDbConnection())
            {
                foreach (var t in obj)
                {
                    con.Insert(t);
                }

                var random = new Random((int)(DateTime.UtcNow.Ticks ^ (DateTime.UtcNow.Ticks >> 4)));
                for (var i = 0; i < numberOfRandomObjects; i++)
                {
                    TestType o = null;

                    while (o == null)
                    {
                        int intVal = random.Next();

                        o = new TestType
                                {
                                    BoolColumn = random.Next()%2 == 0,
                                    IntColumn = intVal,
                                    StringColumn = Guid.NewGuid().ToString()
                                };

                        if (obj.Any(x => x.IntColumn == intVal))
                            o = null;
                    }

                    con.Insert(o);
                }
            }
        }
    }
}