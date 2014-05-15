using NUnit.Framework;
using NServiceKit.OrmLite.Tests;

namespace NServiceKit.OrmLite.PostgreSQL.Tests
{
    /// <summary>A type with byte array field tests.</summary>
    public class TypeWithByteArrayFieldTests : OrmLiteTestBase
    {
        /// <summary>Gets sample object.</summary>
        /// <returns>The sample object.</returns>
        TypeWithByteArrayField getSampleObject()
        {
            var testByteArray = new byte[256];
            for(int i = 0; i < 256; i++) { testByteArray[i] = (byte)i; }
            
            return new TypeWithByteArrayField { Id = 1, Content = testByteArray };
        }

        /// <summary>Can insert and select byte array.</summary>
        [Test]
        public void CanInsertAndSelectByteArray()
        {
            var orig = getSampleObject();

            using (var db = OpenDbConnection())
            {
                db.CreateTable<TypeWithByteArrayField>(true);

                db.Save(orig);

                var target = db.GetById<TypeWithByteArrayField>(orig.Id);

                Assert.AreEqual(orig.Id, target.Id);
                Assert.AreEqual(orig.Content, target.Content);
            }
        }

        /// <summary>Can insert and select byte array manual insert manual select.</summary>
        [Test]
        public void CanInsertAndSelectByteArray__manual_insert__manual_select()
        {
            var orig = getSampleObject();

            using(var db = OpenDbConnection()) {
                //insert and select manually - ok
                db.CreateTable<TypeWithByteArrayField>(true);
                _insertManually(orig, db);

                _selectAndVerifyManually(orig, db);
            }
        }

        /// <summary>
        /// Can insert and select byte array insert parameter insert manual select.
        /// </summary>
        [Test]
        public void CanInsertAndSelectByteArray__InsertParam_insert__manual_select()
        {
            var orig = getSampleObject();

            using(var db = OpenDbConnection()) {
                //insert using InsertParam, and select manually - ok
                db.CreateTable<TypeWithByteArrayField>(true);
                db.InsertParam(orig);

                _selectAndVerifyManually(orig, db);
            }
        }

        /// <summary>
        /// Can insert and select byte array insert parameter insert get by identifier select.
        /// </summary>
        [Test]
        public void CanInsertAndSelectByteArray__InsertParam_insert__GetById_select()
        {
            var orig = getSampleObject();

            using(var db = OpenDbConnection()) {
                //InsertParam + GetByID - fails
                db.CreateTable<TypeWithByteArrayField>(true);
                db.InsertParam(orig);

                var target = db.GetById<TypeWithByteArrayField>(orig.Id);

                Assert.AreEqual(orig.Id, target.Id);
                Assert.AreEqual(orig.Content, target.Content);
            }
        }

        /// <summary>Can insert and select byte array insert get by identifier select.</summary>
        [Test]
        public void CanInsertAndSelectByteArray__Insert_insert__GetById_select()
        {
            var orig = getSampleObject();

            using(var db = OpenDbConnection()) {
                //InsertParam + GetByID - fails
                db.CreateTable<TypeWithByteArrayField>(true);
                db.Insert(orig);

                var target = db.GetById<TypeWithByteArrayField>(orig.Id);

                Assert.AreEqual(orig.Id, target.Id);
                Assert.AreEqual(orig.Content, target.Content);
            }
        }

        /// <summary>Can insert and select byte array insert manual select.</summary>
        [Test]
        public void CanInsertAndSelectByteArray__Insert_insert__manual_select()
        {
            var orig = getSampleObject();

            using(var db = OpenDbConnection()) {
                //InsertParam + GetByID - fails
                db.CreateTable<TypeWithByteArrayField>(true);
                db.Insert(orig);

                _selectAndVerifyManually(orig, db);
            }
        }

        /// <summary>Select and verify manually.</summary>
        /// <param name="orig">The original.</param>
        /// <param name="db">  The database.</param>
        private static void _selectAndVerifyManually(TypeWithByteArrayField orig, System.Data.IDbConnection db)
        {
            using(var cmd = db.CreateCommand()) {
                cmd.CommandText = @"select ""Content"" from ""TypeWithByteArrayField"" where ""Id"" = 1 --manual select";
                using(var reader = cmd.ExecuteReader()) {
                    reader.Read();
                    var ba = reader["Content"] as byte[];
                    Assert.AreEqual(orig.Content.Length, ba.Length);
                    Assert.AreEqual(orig.Content, ba);
                }
            }
        }

        /// <summary>Inserts a manually.</summary>
        /// <param name="orig">The original.</param>
        /// <param name="db">  The database.</param>
        private static void _insertManually(TypeWithByteArrayField orig, System.Data.IDbConnection db)
        {
            using(var cmd = db.CreateCommand()) {
                cmd.CommandText = @"INSERT INTO ""TypeWithByteArrayField"" (""Id"",""Content"") VALUES (@Id, @Content) --manual parameterized insert";

                var p_id = cmd.CreateParameter();
                p_id.ParameterName = "@Id";
                p_id.Value = orig.Id;

                cmd.Parameters.Add(p_id);

                var p_content = cmd.CreateParameter();
                p_content.ParameterName = "@Content";
                p_content.Value = orig.Content;

                cmd.Parameters.Add(p_content);

                cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>A type with byte array field.</summary>
    class TypeWithByteArrayField
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the content.</summary>
        /// <value>The content.</value>
        public byte[] Content { get; set; }
    }
}