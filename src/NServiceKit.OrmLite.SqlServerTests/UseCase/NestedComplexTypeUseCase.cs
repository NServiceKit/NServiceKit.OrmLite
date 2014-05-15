using System.ComponentModel.DataAnnotations;
using System.Data;
using NUnit.Framework;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.SqlServerTests.UseCase
{
    /// <summary>A nested complex type use case.</summary>
    [TestFixture]
    public class NestedComplexTypeUseCase : OrmLiteTestBase
    {
        /// <summary>A location.</summary>
        public class Location
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement]
            public int Id { get; set; }

            /// <summary>Gets or sets the description.</summary>
            /// <value>The description.</value>
            [StringLength(50)]
            public string Description { get; set; }

            /// <summary>Gets or sets the geo location.</summary>
            /// <value>The geo location.</value>
            public GeoLocation GeoLocation { get; set; }
        }

        /// <summary>A geo location.</summary>
        public class GeoLocation
        {
            /// <summary>Gets or sets the latitude.</summary>
            /// <value>The latitude.</value>
            [StringLength(50)]
            public string Latitude { get; set; }

            /// <summary>Gets or sets the longitude.</summary>
            /// <value>The longitude.</value>
            [StringLength(50)]
            public string Longitude { get; set; }
        }

        /// <summary>
        /// Handles null correctly on insert parameter entity with nested complex type where nested
        /// property is null.
        /// </summary>
        [Test]
        public void Handles_NULL_correctly_on_InsertParam_entity_with_nested_complex_type_where_nested_property_is_null()
        {
            using (IDbConnection db = OpenDbConnection())
            {
                db.CreateTable<Location>(true);

                var location = new Location
                {
                    Description = "HQ",
                    GeoLocation = null
                };

                location.Id = (int)db.InsertParam<Location>(location, true);

                var newLocation = db.GetByIdOrDefault<Location>(location.Id);

                Assert.That(newLocation, Is.Not.Null);
                Assert.That(newLocation.Id, Is.EqualTo(location.Id));
                Assert.That(newLocation.GeoLocation, Is.Null);
            }
        }

        /// <summary>
        /// Handles null correctly on insert entity with nested complex type where nested property is
        /// null.
        /// </summary>
        [Test]
        public void Handles_NULL_correctly_on_Insert_entity_with_nested_complex_type_where_nested_property_is_null()
        {
            using (IDbConnection db = OpenDbConnection())
            {
                db.CreateTable<Location>(true);

                var location = new Location
                {
                    Description = "HQ",
                    GeoLocation = null
                };

                db.Insert<Location>(location);
                location.Id = (int)db.GetLastInsertId();

                var newLocation = db.GetByIdOrDefault<Location>(location.Id);

                Assert.That(newLocation, Is.Not.Null);
                Assert.That(newLocation.GeoLocation, Is.Null);
            }
        }
    }
}
