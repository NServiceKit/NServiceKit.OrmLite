using System;
using NServiceKit.Common;

namespace NServiceKit.OrmLite.PostgreSQL
{
    /// <summary>A postgre SQL naming strategy.</summary>
    public class PostgreSqlNamingStrategy : INamingStrategy
    {
        /// <summary>Gets table name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The table name.</returns>
        public string GetTableName(string name)
        {
            return name.ToLowercaseUnderscore();
        }

        /// <summary>Gets column name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The column name.</returns>
        public string GetColumnName(string name)
        {
            return name.ToLowercaseUnderscore();
        }
    }
}