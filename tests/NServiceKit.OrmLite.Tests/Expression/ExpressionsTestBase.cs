using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace NServiceKit.OrmLite.Tests.Expression
{
    /// <summary>The expressions test base.</summary>
    public class ExpressionsTestBase : OrmLiteTestBase
    {
        /// <summary>Setups this object.</summary>
        [SetUp]
        public void Setup()
        {
            OpenDbConnection().DropAndCreateTable<TestType>();
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

        /// <summary>Gets file connection string.</summary>
        /// <returns>The file connection string.</returns>
        protected override string GetFileConnectionString()
        {
            var connectionString = Config.SqliteFileDir + this.GetType().Name + ".sqlite";
            if (File.Exists(connectionString))
                File.Delete(connectionString);

            return connectionString;
        }
    }
}