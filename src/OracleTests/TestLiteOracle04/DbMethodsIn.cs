using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
namespace NServiceKit.OrmLite
{
    /// <summary>A database methods.</summary>
	public static class DbMethods
	{
        /// <summary>A T extension method that insert.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <param name="list"> The list.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
		public static bool In<T>(this T value, IList<Object> list) {
			foreach( Object obj in list){
				if(obj==null || value==null ) continue;
				if( obj.ToString() == value.ToString() ) return true;
			}
			return false;
		}

        /// <summary>A T extension method that descriptions the given value.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>A string.</returns>
		public static string Desc<T>(this T value) {
			return  value==null? "": value.ToString() + " DESC";
		}

        /// <summary>A T extension method that as.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">  The value to act on.</param>
        /// <param name="asValue">as value.</param>
        /// <returns>A string.</returns>
		public static string As<T>(this T value, string asValue) {
			return  value==null? "": string.Format("{0} AS {1}", value.ToString(), asValue);
		}

        /// <summary>A T extension method that sums the given value.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>A T.</returns>
		public static T Sum<T>(this T value)  {
			return value;
		}

        /// <summary>A T extension method that counts the given value.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>A T.</returns>
		public static T Count<T>(this T value)  {
			return value;
		}

        /// <summary>
        /// A T extension method that determines the minimum of the given parameters.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>The minimum value.</returns>
		public static T Min<T>(this T value)  {
			return value;
		}

        /// <summary>
        /// A T extension method that determines the average of the given parameters.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>The average value.</returns>
		public static T Avg<T>(this T value)  {
			return value;
		}
	}
		
}

//class DbMehods.In, As , Desc, Sum, Count : methdos DbMethodName

/*
 public static T Sum<T>(params T[] args)  where T: struct
		{ 
		
		     T total = default(T); 
		     Func<T, T, T> addMethod = CreateAdd<T>(); 
		
		     foreach (T val in args)
		     {
		           total = addMethod(total, val);
		     } 
		
		     return total;
		}

		
		private static Func<T, T, T> CreateAdd<T>()
        {
            ParameterExpression lhs = Expression.Parameter(typeof(T), "lhs");
            ParameterExpression rhs = Expression.Parameter(typeof(T), "rhs"); 
            Expression<Func<T, T, T>> addExpr = Expression<Func<T, T, T>>.

            Lambda<Func<T, T, T>>(
                    Expression.Add(lhs, rhs),
                    new ParameterExpression[] { lhs, rhs });

            Func<T, T, T> addMethod = addExpr.Compile();

            return addMethod;
        }
		
	} 
*/