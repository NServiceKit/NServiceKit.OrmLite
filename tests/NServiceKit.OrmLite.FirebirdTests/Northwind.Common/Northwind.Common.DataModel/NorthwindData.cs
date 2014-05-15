using System;
using System.Collections.Generic;
using Northwind.Common;

namespace Northwind.Common.DataModel{

    /// <summary>A northwind data.</summary>
	public static class NorthwindData
	{
        /// <summary>Gets or sets the categories.</summary>
        /// <value>The categories.</value>
		public static List<Category> Categories
		{
			get;set;
		}

        /// <summary>Gets or sets the customer demos.</summary>
        /// <value>The customer demos.</value>
		public static List<CustomerCustomerDemo> CustomerCustomerDemos
		{
			get;set;
		}

        /// <summary>Gets or sets the customers.</summary>
        /// <value>The customers.</value>
		public static List<Customer> Customers
		{
			get;set;
		}

        /// <summary>Gets or sets the employees.</summary>
        /// <value>The employees.</value>
		public static List<Employee> Employees
		{
			get;set;
		}

        /// <summary>Gets or sets the employee territories.</summary>
        /// <value>The employee territories.</value>
		public static List<EmployeeTerritory> EmployeeTerritories
		{
			get;set;
		}

        /// <summary>Gets or sets the order details.</summary>
        /// <value>The order details.</value>
		public static List<OrderDetail> OrderDetails
		{
			get;set;
		}

        /// <summary>Gets or sets the orders.</summary>
        /// <value>The orders.</value>
		public static List<Order> Orders
		{
			get;set;
		}

        /// <summary>Gets or sets the products.</summary>
        /// <value>The products.</value>
		public static List<Product> Products
		{
			get;set;
		}

        /// <summary>Gets or sets the regions.</summary>
        /// <value>The regions.</value>
		public static List<Region> Regions
		{
			get;set;
		}

        /// <summary>Gets or sets the shippers.</summary>
        /// <value>The shippers.</value>
		public static List<Shipper> Shippers
		{
			get;set;
		}

        /// <summary>Gets or sets the suppliers.</summary>
        /// <value>The suppliers.</value>
		public static List<Supplier> Suppliers
		{
			get;set;
		}

        /// <summary>Gets or sets the territories.</summary>
        /// <value>The territories.</value>
		public static List<Territory> Territories
		{
			get;set;
		}

        /// <summary>Converts a dateTime to a date time.</summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>dateTime as a DateTime.</returns>
		public static DateTime ToDateTime(string dateTime)
		{			
			string[] strArrays = dateTime.Split(new char[] { '/' });
			return new DateTime(int.Parse(strArrays[2]), int.Parse(strArrays[0]), int.Parse(strArrays[1]));
		}
	}
}