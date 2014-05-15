using NUnit.Framework;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.MySql.Tests
{
    /// <summary>An ORM lite save string value tests.</summary>
    public class OrmLiteSaveStringValueTests : OrmLiteTestBase
    {
        /// <summary>Can save string including single quote.</summary>
        [Test]
        public void Can_save_string_including_single_quote()
        {
            using (var db = OpenDbConnection())
            {
                db.DropTable<StringTable>();
                db.CreateTable<StringTable>(true);

                var text = "It worked! Didn't it?";
                var row = new StringTable() {Value = text};

                db.Save(row);
                var id = db.GetLastInsertId();

                var selectedRow = db.GetById<StringTable>(id);
                Assert.AreEqual(text, selectedRow.Value);
            }
        }

        /// <summary>Can save string including double quote.</summary>
        [Test]
        public void Can_save_string_including_double_quote()
        {
            using (var db = OpenDbConnection())
            {
                db.DropTable<StringTable>();
                db.CreateTable<StringTable>(true);

                var text = "\"It worked!\"";
                var row = new StringTable() { Value = text };

                db.Save(row);
                var id = db.GetLastInsertId();

                var selectedRow = db.GetById<StringTable>(id);
                Assert.AreEqual(text, selectedRow.Value);
            }
        }

        /// <summary>Can save string including backslash.</summary>
        [Test]
        public void Can_save_string_including_backslash()
        {
            using (var db = OpenDbConnection())
            {
                db.DropTable<StringTable>();
                db.CreateTable<StringTable>(true);

                var text = "\\\\mycomputer\\hasashareddirectory";
                var row = new StringTable() { Value = text };

                db.Save(row);
                var id = db.GetLastInsertId();

                var selectedRow = db.GetById<StringTable>(id);
                Assert.AreEqual(text, selectedRow.Value);
            }
        }
    }

    /// <summary>A string table.</summary>
    public class StringTable
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the value.</summary>
        /// <value>The value.</value>
        public string Value { get; set; }
    }
}
