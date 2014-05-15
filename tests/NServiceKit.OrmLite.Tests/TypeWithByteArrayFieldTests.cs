using NUnit.Framework;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>A type with byte array field tests.</summary>
    public class TypeWithByteArrayFieldTests : OrmLiteTestBase
    {
        /// <summary>Can insert and select byte array.</summary>
        [Test]
        public void CanInsertAndSelectByteArray()
        {
            var orig = new TypeWithByteArrayField { Id = 1, Content = new byte[] { 0, 17, 0, 17, 0, 7 } };

            using (var db = OpenDbConnection())
            {
                db.CreateTable<TypeWithByteArrayField>(true);

                db.Save(orig);

                var target = db.GetById<TypeWithByteArrayField>(orig.Id);

                Assert.AreEqual(orig.Id, target.Id);
                Assert.AreEqual(orig.Content, target.Content);
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