//
// ServiceStack.OrmLite: Light-weight POCO ORM for .NET and Mono
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2010 Liquidbit Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NServiceKit.OrmLite
{
    /// <summary>A property invoker.</summary>
    public static class PropertyInvoker
    {
        /// <summary>A PropertyInfo extension method that gets property setter function.</summary>
        /// <param name="propertyInfo">The propertyInfo to act on.</param>
        /// <returns>The property setter function.</returns>
        public static PropertySetterDelegate GetPropertySetterFn(this PropertyInfo propertyInfo)
        {
            var propertySetMethod = propertyInfo.GetSetMethod();
            if (propertySetMethod == null) return null;

#if NO_EXPRESSIONS
            return (o, convertedValue) =>
            {
                propertySetMethod.Invoke(o, new[] { convertedValue });
                return;
            };
#else 
            var instance = Expression.Parameter(typeof(object), "i");
            var argument = Expression.Parameter(typeof(object), "a");

            var instanceParam = Expression.Convert(instance, propertyInfo.DeclaringType);
            var valueParam = Expression.Convert(argument, propertyInfo.PropertyType);

            var setterCall = Expression.Call(instanceParam, propertyInfo.GetSetMethod(), valueParam);

            return Expression.Lambda<PropertySetterDelegate>(setterCall, instance, argument).Compile();
#endif
        }

        /// <summary>A PropertyInfo extension method that gets property getter function.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="propertyInfo">The propertyInfo to act on.</param>
        /// <returns>The property getter function.</returns>
        public static PropertyGetterDelegate GetPropertyGetterFn(this PropertyInfo propertyInfo)
        {
            var getMethodInfo = propertyInfo.GetGetMethod();
            if (getMethodInfo == null) return null;

#if NO_EXPRESSIONS
			return o => propertyInfo.GetGetMethod().Invoke(o, new object[] { });
#else
try 
	{	        
		    var oInstanceParam = Expression.Parameter(typeof(object), "oInstanceParam");
            var instanceParam = Expression.Convert(oInstanceParam, propertyInfo.DeclaringType);

            var exprCallPropertyGetFn = Expression.Call(instanceParam, getMethodInfo);
            var oExprCallPropertyGetFn = Expression.Convert(exprCallPropertyGetFn, typeof(object));

            var propertyGetFn = Expression.Lambda<PropertyGetterDelegate>
                (
                    oExprCallPropertyGetFn,
                    oInstanceParam
                ).Compile();

            return propertyGetFn;

	}
	catch (Exception ex)
	{
		Console.Write(ex.Message);
		throw;
	}
#endif
        }
    }


}