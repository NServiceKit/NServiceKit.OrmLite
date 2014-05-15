using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace NServiceKit.OrmLite.MySql
{
    /// <summary>Description of MySqlExpressionVisitor.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public class MySqlExpressionVisitor<T>:SqlExpressionVisitor<T>
	{
        /// <summary>Visit column access method.</summary>
        /// <param name="m">The MethodCallExpression to process.</param>
        /// <returns>An object.</returns>
        protected override object VisitColumnAccessMethod(MethodCallExpression m)
        {
            if (m.Method.Name == "StartsWith")
            {
                List<Object> args = this.VisitExpressionList(m.Arguments);
                var quotedColName = Visit(m.Object);
                return new PartialSqlString(string.Format("LEFT( {0},{1})= {2} ", quotedColName
                                                          , args[0].ToString().Length,
                                                          OrmLiteConfig.DialectProvider.GetQuotedValue(args[0],
                                                                                                       args[0].GetType())));
            }

            return base.VisitColumnAccessMethod(m);
        }
	}
}
