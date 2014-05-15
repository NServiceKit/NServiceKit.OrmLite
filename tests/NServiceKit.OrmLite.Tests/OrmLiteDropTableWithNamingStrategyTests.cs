using NUnit.Framework;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>An ORM lite drop table with naming strategy tests.</summary>
    public class OrmLiteDropTableWithNamingStrategyTests
        : OrmLiteTestBase
    {
        /// <summary>Can drop table with namig strategy table prefix.</summary>
        [Test]
        public void Can_drop_TableWithNamigStrategy_table_prefix()
        {
            OrmLiteConfig.DialectProvider.NamingStrategy = new PrefixNamingStrategy
            {
                TablePrefix = "tab_",
                ColumnPrefix = "col_"
            };

            using (var db = OpenDbConnection())
            {
                db.CreateTable<ModelWithOnlyStringFields>(true);

                db.DropTable<ModelWithOnlyStringFields>();

                Assert.False(db.TableExists("tab_ModelWithOnlyStringFields"));
            }
            
            OrmLiteConfig.DialectProvider.NamingStrategy = new OrmLiteNamingStrategyBase();
        }

        /// <summary>Can drop table with namig strategy table lowered.</summary>
        [Test]
        public void Can_drop_TableWithNamigStrategy_table_lowered()
        {
            OrmLiteConfig.DialectProvider.NamingStrategy = new LowercaseNamingStrategy();

            using (var db = OpenDbConnection())
            {
                db.CreateTable<ModelWithOnlyStringFields>(true);

                db.DropTable<ModelWithOnlyStringFields>();

                Assert.False(db.TableExists("modelwithonlystringfields"));
            }

            OrmLiteConfig.DialectProvider.NamingStrategy = new OrmLiteNamingStrategyBase();
        }

        /// <summary>Can drop table with namig strategy table name underscore compound.</summary>
        [Test]
        public void Can_drop_TableWithNamigStrategy_table_nameUnderscoreCompound()
        {
            OrmLiteConfig.DialectProvider.NamingStrategy = new UnderscoreSeparatedCompoundNamingStrategy();

            using (var db = OpenDbConnection())
            {
                db.CreateTable<ModelWithOnlyStringFields>(true);

                db.DropTable<ModelWithOnlyStringFields>();

                Assert.False(db.TableExists("model_with_only_string_fields"));
            }

            OrmLiteConfig.DialectProvider.NamingStrategy = new OrmLiteNamingStrategyBase();
        }
    }
}