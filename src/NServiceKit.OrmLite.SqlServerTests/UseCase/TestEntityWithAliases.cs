using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.SqlServerTests.UseCase
{
    /// <summary>A test entity with aliases.</summary>
    public class TestEntityWithAliases
    {
        #region Properties
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        [Alias("Id Column")]
        public int Id { get; set; }

        /// <summary>Gets or sets the foo.</summary>
        /// <value>The foo.</value>
        [Alias("Foo Column")]
        public String Foo { get; set; }

        /// <summary>Gets or sets the bar.</summary>
        /// <value>The bar.</value>
        [Alias("Bar Column")]
        public String Bar { get; set; }

        //[Index]

        /// <summary>Gets or sets the baz.</summary>
        /// <value>The baz.</value>
        [Alias("Baz Column")]
        public Decimal Baz { get; set; }
        
        #endregion
    }
}
