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
using System.Reflection;

namespace NServiceKit.OrmLite
{
    /// <summary>A reflection property invoker.</summary>
	public class ReflectionPropertyInvoker
		: IPropertyInvoker
	{
        /// <summary>The instance.</summary>
		public static readonly ReflectionPropertyInvoker Instance = new ReflectionPropertyInvoker();

        /// <summary>Gets or sets the convert value function.</summary>
        /// <value>The convert value function.</value>
		public Func<object, Type, object> ConvertValueFn { get; set; }

        /// <summary>Sets property value.</summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <param name="fieldType">   Type of the field.</param>
        /// <param name="onInstance">  The on instance.</param>
        /// <param name="withValue">   The with value.</param>
		public void SetPropertyValue(PropertyInfo propertyInfo, Type fieldType, object onInstance, object withValue)
		{
			var convertedValue = ConvertValueFn(withValue, fieldType);

			var propertySetMethod = propertyInfo.GetSetMethod();
			if (propertySetMethod == null) return;

			propertySetMethod.Invoke(onInstance, new[] { convertedValue });
		}

        /// <summary>Gets property value.</summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <param name="fromInstance">from instance.</param>
        /// <returns>The property value.</returns>
		public object GetPropertyValue(PropertyInfo propertyInfo, object fromInstance)
		{
			var value = propertyInfo.GetGetMethod().Invoke(fromInstance, new object[] { });
			return value;
		}
	}
}