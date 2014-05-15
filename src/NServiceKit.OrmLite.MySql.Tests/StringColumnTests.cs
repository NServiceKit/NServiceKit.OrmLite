using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using NServiceKit.OrmLite.MySql.DataAnnotations;

namespace NServiceKit.OrmLite.MySql.Tests
{
    /// <summary>A string column tests.</summary>
    [TestFixture]
    public class StringColumnTests
        : OrmLiteTestBase
    {
        /// <summary>Can create primary key varchar with string length 255.</summary>
        [Test]
        public void Can_create_primary_key_varchar_with_string_length_255()
        {
            using (var db = OpenDbConnection())
            {
                db.CreateTable<TypeWithStringId_255>(true);
            }
        }

        /// <summary>Can create primary key varchar without setting string length.</summary>
        [Test]
        public void Can_create_primary_key_varchar_without_setting_string_length()
        {
            using (var db = OpenDbConnection())
            {
                db.CreateTable<TypeWithStringId>(true);
            }
        }

        /// <summary>Can create unique key on varchar without setting string length.</summary>
        [Test]
        public void Can_create_unique_key_on_varchar_without_setting_string_length()
        {
            using (var db = OpenDbConnection())
            {
                db.CreateTable<TypeWithUniqeKeyOnVarchar>(true);
            }
        }

        /// <summary>Cannot create unique key on varchar greater than 255.</summary>
        [ExpectedException(typeof(MySqlException))]
        [Test]
        public void Cannot_create_unique_key_on_varchar_greater_than_255()
        {
            using (var db = OpenDbConnection())
            {
                db.CreateTable<TypeWithUniqeKeyOnVarchar_256>(true);
            }
        }

        /// <summary>Cannot create primary key varchar with string length greater than 255.</summary>
        [ExpectedException(typeof(MySqlException))]
        [Test]
        public void Cannot_create_primary_key_varchar_with_string_length_greater_than_255()
        {
            using (var db = OpenDbConnection())
            {
                db.CreateTable<TypeWithStringId_256>(true);
            }
        }

        /// <summary>Can store and retrieve string with 8000 characters from varchar field.</summary>
        [Test]
        public void Can_store_and_retrieve_string_with_8000_characters_from_varchar_field()
        {
            using (var db = OpenDbConnection())
            {
                db.CreateTable<TypeWithStringId>(true);

                var obj = new TypeWithStringId {
                    Id = "a",
                    Value = CreateString(8000)
                };

                Assert.AreEqual(8000, obj.Value.Length);

                db.Save(obj);
                var target = db.GetById<TypeWithStringId>(obj.Id);

                Assert.AreEqual(obj.Value, target.Value);
                Assert.AreEqual(8000, obj.Value.Length);
            }
        }

        /// <summary>Can store and retrieve string with 8000 characters from text field.</summary>
        [Test]
        public void Can_store_and_retrieve_string_with_8000_characters_from_text_field()
        {
            using (var db = OpenDbConnection())
            {
                db.CreateTable<TypeWithTextField>(true);

                var obj = new TypeWithTextField() {
                    Value = CreateString(8000)
                };

                Assert.AreEqual(8000, obj.Value.Length);

                db.Save(obj);
                obj.Id = (int)db.GetLastInsertId();

                var target = db.GetById<TypeWithTextField>(obj.Id);

                Assert.AreEqual(obj.Value, target.Value);
                Assert.AreEqual(8000, obj.Value.Length);
            }
        }

        #region classes
        /// <summary>A type with uniqe key on varchar.</summary>
        class TypeWithUniqeKeyOnVarchar
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            [Index(true)]
            public string Value { get; set; }
        }

        /// <summary>A type with uniqe key on varchar 256.</summary>
        class TypeWithUniqeKeyOnVarchar_256
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            [StringLength(256)]
            [Index(true)]
            public string Value { get; set; }
        }

        /// <summary>A type with string identifier.</summary>
        class TypeWithStringId : IHasStringId
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public string Id { get; set; }

            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            [StringLength(8000)]
            public string Value { get; set; }
        }

        /// <summary>A type with unique index on text field.</summary>
        class TypeWithUniqueIndexOnTextField
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement]
            public int Id { get; set; }

            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            [Index(true)]
            [Text]
            public string Value { get; set; }
        }

        /// <summary>A type with text field.</summary>
        class TypeWithTextField
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [AutoIncrement]
            public int Id { get; set; }

            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            [Text]
            public string Value { get; set; }
        }

        /// <summary>A type with string identifier 255.</summary>
        class TypeWithStringId_255 : IHasStringId
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [StringLength(255)]
            public string Id { get; set; }

            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            public string Value { get; set; }
        }

        /// <summary>A type with string identifier 256.</summary>
        class TypeWithStringId_256 : IHasStringId
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [StringLength(256)]
            public string Id { get; set; }

            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            public string Value { get; set; }
        }

        #endregion

        /// <summary>Creates a string.</summary>
        /// <param name="length">The length.</param>
        /// <returns>The new string.</returns>
        private static string CreateString(int length)
        {
            const string loremIpsum =
                "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";

            var retVal = "";

            for (int i = 0; i < length / loremIpsum.Length; i++)
                retVal += loremIpsum;

            return retVal + loremIpsum.Substring(0, length - retVal.Length);
        }
    }
}
