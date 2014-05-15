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
    /// <summary>Interface for property invoker.</summary>
	public interface IPropertyInvoker
	{
        /// <summary>Gets or sets the convert value function.</summary>
        /// <value>The convert value function.</value>
		Func<object, Type, object> ConvertValueFn { get; set; }

        /// <summary>Sets property value.</summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <param name="fieldType">   Type of the field.</param>
        /// <param name="onInstance">  The on instance.</param>
        /// <param name="withValue">   The with value.</param>
		void SetPropertyValue(PropertyInfo propertyInfo, Type fieldType, object onInstance, object withValue);

        /// <summary>Gets property value.</summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <param name="fromInstance">from instance.</param>
        /// <returns>The property value.</returns>
		object GetPropertyValue(PropertyInfo propertyInfo, object fromInstance);
	}
}