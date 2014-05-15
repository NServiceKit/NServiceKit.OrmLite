using NUnit.Framework;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.SqlServerTests
{
    /// <summary>A foreign key attribute tests.</summary>
    [TestFixture]
    public class ForeignKeyAttributeTests : OrmLiteTestBase
    {
        /// <summary>Setups this object.</summary>
        [TestFixtureSetUp]
        public void Setup()
        {
            using (var dbConn = OpenDbConnection())
            {
                dbConn.DropTable<TypeWithOnDeleteAndUpdateCascade>();
                dbConn.DropTable<TypeWithOnDeleteSetNull>();
                dbConn.DropTable<TypeWithOnDeleteSetDefault>();
                dbConn.DropTable<TypeWithOnDeleteRestrict>();
                dbConn.DropTable<TypeWithOnDeleteNoAction>();
                dbConn.DropTable<TypeWithOnDeleteCascade>();
                dbConn.DropTable<TypeWithSimpleForeignKey>();
                dbConn.DropTable<ReferencedType>();

                dbConn.CreateTable<ReferencedType>(true);
            }
        }

        /// <summary>Can create simple foreign key.</summary>
        [Test]
        public void CanCreateSimpleForeignKey()
        {
            using (var dbConn = OpenDbConnection())
            {
                dbConn.CreateTable<TypeWithSimpleForeignKey>(true);
            }
        }

        /// <summary>Can create foreign with on delete cascade.</summary>
        [Test]
        public void CanCreateForeignWithOnDeleteCascade()
        {
            using (var dbConn = OpenDbConnection())
            {
                dbConn.CreateTable<TypeWithOnDeleteCascade>(true);
            }
        }

        /// <summary>Cascades on delete.</summary>
        [Test]
        public void CascadesOnDelete()
        {
            using (var dbConn = OpenDbConnection())
            {
                dbConn.CreateTable<TypeWithOnDeleteCascade>(true);

                dbConn.Save(new ReferencedType { Id = 1 });
                dbConn.Save(new TypeWithOnDeleteCascade { RefId = 1 });

                Assert.AreEqual(1, dbConn.Select<ReferencedType>().Count);
                Assert.AreEqual(1, dbConn.Select<TypeWithOnDeleteCascade>().Count);

                dbConn.Delete<ReferencedType>(r => r.Id == 1);

                Assert.AreEqual(0, dbConn.Select<ReferencedType>().Count);
                Assert.AreEqual(0, dbConn.Select<TypeWithOnDeleteCascade>().Count);
            }
        }

        /// <summary>Can create foreign with on delete cascade and on update cascade.</summary>
        [Test]
        public void CanCreateForeignWithOnDeleteCascadeAndOnUpdateCascade()
        {
            using (var dbConn = OpenDbConnection())
            {
                dbConn.CreateTable<TypeWithOnDeleteAndUpdateCascade>(true);
            }
        }

        /// <summary>Can create foreign with on delete no action.</summary>
        [Test]
        public void CanCreateForeignWithOnDeleteNoAction()
        {
            using (var dbConn = OpenDbConnection())
            {
                dbConn.CreateTable<TypeWithOnDeleteNoAction>(true);
            }
        }

        /// <summary>Can create foreign with on delete restrict.</summary>
        [NUnit.Framework.Ignore("Not supported in SQL Server")]
        [Test]
        public void CanCreateForeignWithOnDeleteRestrict()
        {
            using (var dbConn = OpenDbConnection())
            {
                dbConn.CreateTable<TypeWithOnDeleteRestrict>(true);
            }
        }

        /// <summary>Can create foreign with on delete set default.</summary>
        [Test]
        public void CanCreateForeignWithOnDeleteSetDefault()
        {
            using (var dbConn = OpenDbConnection())
            {
                dbConn.CreateTable<TypeWithOnDeleteSetDefault>(true);
            }
        }

        /// <summary>Can create foreign with on delete set null.</summary>
        [Test]
        public void CanCreateForeignWithOnDeleteSetNull()
        {
            using (var dbConn = OpenDbConnection())
            {
                dbConn.CreateTable<TypeWithOnDeleteSetNull>(true);
            }
        }
    }

    /// <summary>A referenced type.</summary>
    public class ReferencedType
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
    }

    /// <summary>A type with simple foreign key.</summary>
    public class TypeWithSimpleForeignKey
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the reference.</summary>
        /// <value>The identifier of the reference.</value>
        [References(typeof(ReferencedType))]
        public int RefId { get; set; }
    }

    /// <summary>A type with on delete cascade.</summary>
    public class TypeWithOnDeleteCascade
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the reference.</summary>
        /// <value>The identifier of the reference.</value>
        [ForeignKey(typeof(ReferencedType), OnDelete = "CASCADE")]
        public int? RefId { get; set; }
    }

    /// <summary>A type with on delete and update cascade.</summary>
    public class TypeWithOnDeleteAndUpdateCascade
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the reference.</summary>
        /// <value>The identifier of the reference.</value>
        [ForeignKey(typeof(ReferencedType), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public int? RefId { get; set; }
    }

    /// <summary>A type with on delete no action.</summary>
    public class TypeWithOnDeleteNoAction
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the reference.</summary>
        /// <value>The identifier of the reference.</value>
        [ForeignKey(typeof(ReferencedType), OnDelete = "NO ACTION")]
        public int? RefId { get; set; }
    }

    /// <summary>A type with on delete restrict.</summary>
    public class TypeWithOnDeleteRestrict
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the reference.</summary>
        /// <value>The identifier of the reference.</value>
        [ForeignKey(typeof(ReferencedType), OnDelete = "RESTRICT")]
        public int? RefId { get; set; }
    }

    /// <summary>A type with on delete set default.</summary>
    public class TypeWithOnDeleteSetDefault
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the reference.</summary>
        /// <value>The identifier of the reference.</value>
        [Default(typeof(int), "17")]
        [ForeignKey(typeof(ReferencedType), OnDelete = "SET DEFAULT")]
        public int RefId { get; set; }
    }

    /// <summary>A type with on delete set null.</summary>
    public class TypeWithOnDeleteSetNull
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the reference.</summary>
        /// <value>The identifier of the reference.</value>
        [ForeignKey(typeof(ReferencedType), OnDelete = "SET NULL")]
        public int? RefId { get; set; }
    }
}