using NUnit.Framework;
using NServiceKit.Common.Tests.Models;
using NServiceKit.OrmLite.Tests;

namespace NServiceKit.OrmLite.PostgreSQL.Tests
{
    /// <summary>An ORM lite drop table with naming strategy tests.</summary>
    public class OrmLiteDropTableWithNamingStrategyTests
        : OrmLiteTestBase
    {
        /// <summary>Can drop table with namig strategy table postgre SQL naming strategy.</summary>
        [Test]
        public void Can_drop_TableWithNamigStrategy_table_PostgreSqlNamingStrategy()
        {
            OrmLiteConfig.DialectProvider.NamingStrategy = new PostgreSqlNamingStrategy();

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