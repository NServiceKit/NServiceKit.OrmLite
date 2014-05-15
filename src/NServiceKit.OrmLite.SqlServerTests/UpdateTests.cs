using NUnit.Framework;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.SqlServerTests
{
    /// <summary>An update tests.</summary>
    public class UpdateTests : OrmLiteTestBase
    {
        /// <summary>Can execute update using expression.</summary>
        [Test]
        public void Can_execute_update_using_expression()
        {
            using (var con = OpenDbConnection())
            {
                con.CreateTable<SimpleType>(true);
                var obj = new SimpleType { Name = "Somename" };
                con.Save(obj);
                var storedObj = con.GetById<SimpleType>(con.GetLastInsertId());

                Assert.AreEqual(obj.Name, storedObj.Name);

                obj.Id = storedObj.Id;
                obj.Name = "Someothername";
                con.Update(obj, q => q.Id == storedObj.Id);

                var target = con.GetById<SimpleType>(storedObj.Id);

                Assert.AreEqual(obj.Name, target.Name);
            }
        }

        /// <summary>Can execute update only.</summary>
        [Test]
        public void Can_execute_update_only()
        {
            using (var con = OpenDbConnection())
            {
                con.CreateTable<SimpleType>(true);
                var obj = new SimpleType { Name = "Somename" };
                con.Save(obj);
                var storedObj = con.GetById<SimpleType>(con.GetLastInsertId());

                Assert.AreEqual(obj.Name, storedObj.Name);

                var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<SimpleType>();
                ev.Update();
                ev.Where(q => q.Id == storedObj.Id); 
                storedObj.Name = "Someothername";

                con.UpdateOnly(storedObj, ev);

                var target = con.GetById<SimpleType>(storedObj.Id);

                Assert.AreEqual("Someothername", target.Name);
            }
        }

        /// <summary>Can execute update.</summary>
        [Test]
        public void Can_execute_update()
        {
            using (var con = OpenDbConnection())
            {
                con.CreateTable<SimpleType>(true);
                var obj = new SimpleType { Name = "Somename" };
                con.Save(obj);
                var storedObj = con.GetById<SimpleType>(con.GetLastInsertId());

                Assert.AreEqual(obj.Name, storedObj.Name);

                obj.Id = storedObj.Id;
                obj.Name = "Someothername";
                con.Update(obj);

                var target = con.GetById<SimpleType>(storedObj.Id);

                Assert.AreEqual(obj.Name, target.Name);
            }
        }

        /// <summary>Can execute update using aliased columns.</summary>
        [Test]
        public void Can_execute_update_using_aliased_columns()
        {
            using (var con = OpenDbConnection())
            {
                con.CreateTable<SimpleAliasedType>(true);
                var obj = new SimpleAliasedType { Name = "Somename" };
                con.Save(obj);
                var storedObj = con.GetById<SimpleAliasedType>(con.GetLastInsertId());

                Assert.AreEqual(obj.Name, storedObj.Name);

                obj.Id = storedObj.Id;
                obj.Name = "Someothername";
                con.Update(obj);

                var target = con.GetById<SimpleAliasedType>(storedObj.Id);

                Assert.AreEqual(obj.Name, target.Name);
            }
        }

        /// <summary>Can execute update parameter.</summary>
        [Test]
        public void Can_execute_updateParam()
        {
            using (var con = OpenDbConnection())
            {
                con.CreateTable<SimpleType>(true);
                var obj = new SimpleType { Name = "Somename" };
                con.Save(obj);
                var storedObj = con.GetById<SimpleType>(con.GetLastInsertId());

                Assert.AreEqual(obj.Name, storedObj.Name);

                obj.Id = storedObj.Id;
                obj.Name = "Someothername";
                con.UpdateParam(obj);

                var target = con.GetById<SimpleType>(storedObj.Id);

                Assert.AreEqual(obj.Name, target.Name);
            }
        }

        /// <summary>Can execute update parameter using aliased columns.</summary>
        [Test]
        public void Can_execute_updateParam_using_aliased_columns()
        {
            using (var con = OpenDbConnection())
            {
                con.CreateTable<SimpleAliasedType>(true);
                var obj = new SimpleAliasedType { Name = "Somename" };
                con.Save(obj);
                var storedObj = con.GetById<SimpleAliasedType>(con.GetLastInsertId());

                Assert.AreEqual(obj.Name, storedObj.Name);

                obj.Id = storedObj.Id;
                obj.Name = "Someothername";
                con.UpdateParam(obj);

                var target = con.GetById<SimpleAliasedType>(storedObj.Id);

                Assert.AreEqual(obj.Name, target.Name);
            }
        }
    }

    /// <summary>A simple type.</summary>
    public class SimpleType
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>A simple aliased type.</summary>
    public class SimpleAliasedType
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        [Alias("NewName")]
        public string Name { get; set; }
      
    }
}
