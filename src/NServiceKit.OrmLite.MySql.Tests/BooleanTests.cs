using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;

namespace NServiceKit.OrmLite.MySql.Tests
{
    /// <summary>A boolean tests.</summary>
    [TestFixture]
    public class BooleanTests : OrmLiteTestBase
    {
        /// <summary>Can store and read bool values.</summary>
        [Test]
        public void Can_store_and_read_bool_values()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<Boolies>();

                var boolies = new List<Boolies> {
                  new Boolies { plainBool = true,  netBool = true },
                  new Boolies { plainBool = false, netBool = true },
                  new Boolies { plainBool = true,  netBool = false },
                  new Boolies { plainBool = false, netBool = false },
                };
                db.InsertAll(boolies);

                var target = db.Select<Boolies>();

                Assert.AreEqual(boolies.Count, target.Count);

                for (int i = 0; i < boolies.Count; i++)
                {
                    Assert.AreEqual(boolies[i].netBool, target.ElementAt(i).netBool);
                    Assert.AreEqual(boolies[i].plainBool, target.ElementAt(i).plainBool);
                }
            }
        }

        /// <summary>Can store and read bool values mapped as bit column.</summary>
        [Test]
        public void Can_store_and_read_bool_values_mapped_as_bit_column()
        {
            using (var db = OpenDbConnection())
            {
                db.CreateTable<Boolies>(true);

                db.ExecuteSql("ALTER TABLE `Boolies` CHANGE `plainBool` `plainBool` bit( 1 ) NOT NULL, " 
                    + "CHANGE `netBool`   `netBool`   bit( 1 ) NOT NULL");

                var boolies = new List<Boolies> {
                  new Boolies { plainBool = true,  netBool = true },
                  new Boolies { plainBool = false, netBool = true },
                  new Boolies { plainBool = true,  netBool = false },
                  new Boolies { plainBool = false, netBool = false },
                };
                db.InsertAll(boolies);

                var target = db.Select<Boolies>();

                Assert.AreEqual(boolies.Count, target.Count);

                for (int i = 0; i < boolies.Count; i++)
                {
                    Assert.AreEqual(boolies[i].netBool, target.ElementAt(i).netBool);
                    Assert.AreEqual(boolies[i].plainBool, target.ElementAt(i).plainBool);
                }
            }
        }

        /// <summary>A boolies.</summary>
        public class Boolies : IHasId<int>
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement]
            [Alias("id")]
            public int Id { get; set; }

            /// <summary>Gets or sets a value indicating whether the plain bool.</summary>
            /// <value>true if plain bool, false if not.</value>
            public bool plainBool { get; set; }

            /// <summary>Gets or sets a value indicating whether the net bool.</summary>
            /// <value>true if net bool, false if not.</value>
            public Boolean netBool { get; set; }
        }
    }

    
}
