using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.SqlServerTests.UseCase
{
    /// <summary>A test entity.</summary>
    public class TestEntity
    {
        #region Properties
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the foo.</summary>
        /// <value>The foo.</value>
        public String Foo { get; set; }

        /// <summary>Gets or sets the bar.</summary>
        /// <value>The bar.</value>
        public String Bar { get; set; }

        /// <summary>Gets or sets the null int.</summary>
        /// <value>The null int.</value>
        public int? NullInt { get; set; }

        /// <summary>Gets or sets the baz.</summary>
        /// <value>The baz.</value>
        [Index]
        public Decimal Baz { get; set; }

        #endregion
    }
}
