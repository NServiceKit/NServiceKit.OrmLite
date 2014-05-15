using System;

namespace NServiceKit.DataAnnotations
{
    /// <summary>Primary key attribute. use to indicate that property is part of the pk.</summary>
	[AttributeUsage( AttributeTargets.Property)]
	public class PrimaryKeyAttribute : Attribute
	{

	}
}