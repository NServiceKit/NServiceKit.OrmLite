using System;
using NServiceKit.DesignPatterns.Model;
using System.Collections.Generic;
using NServiceKit.DataAnnotations;

namespace Northwind.Common.DataModel{

    /// <summary>An order blob.</summary>
	public class OrderBlob : IHasIntId, IHasId<int>{

        /// <summary>Gets or sets the character map.</summary>
        /// <value>The character map.</value>
		public Dictionary<int, string> CharMap
		{
			get;
			set;
		}

        /// <summary>Gets or sets the customer.</summary>
        /// <value>The customer.</value>
		public Customer Customer
		{
			get;
			set;
		}

        /// <summary>Gets or sets the employee.</summary>
        /// <value>The employee.</value>
		public Employee Employee
		{
			get;
			set;
		}

        /// <summary>Gets or sets the freight.</summary>
        /// <value>The freight.</value>
		public decimal Freight
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[AutoIncrement]
		public int Id
		{
			get;
			set;
		}

        /// <summary>Gets or sets a list of identifiers of the ints.</summary>
        /// <value>A list of identifiers of the ints.</value>
		public List<int> IntIds
		{
			get;
			set;
		}

        /// <summary>Gets or sets the order date.</summary>
        /// <value>The order date.</value>
		public DateTime? OrderDate
		{
			get;
			set;
		}

        /// <summary>Gets or sets the order details.</summary>
        /// <value>The order details.</value>
		public List<OrderDetailBlob> OrderDetails
		{
			get;
			set;
		}

        /// <summary>Gets or sets the required date.</summary>
        /// <value>The required date.</value>
		public DateTime? RequiredDate
		{
			get;
			set;
		}

        /// <summary>Gets or sets the ship address.</summary>
        /// <value>The ship address.</value>
		public string ShipAddress
		{
			get;
			set;
		}

        /// <summary>Gets or sets the ship city.</summary>
        /// <value>The ship city.</value>
		public string ShipCity
		{
			get;
			set;
		}

        /// <summary>Gets or sets the ship country.</summary>
        /// <value>The ship country.</value>
		public string ShipCountry
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name of the ship.</summary>
        /// <value>The name of the ship.</value>
		public string ShipName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the shipped date.</summary>
        /// <value>The shipped date.</value>
		public DateTime? ShippedDate
		{
			get;
			set;
		}

        /// <summary>Gets or sets the ship postal code.</summary>
        /// <value>The ship postal code.</value>
		public string ShipPostalCode
		{
			get;
			set;
		}

        /// <summary>Gets or sets the ship region.</summary>
        /// <value>The ship region.</value>
		public string ShipRegion
		{
			get;
			set;
		}

        /// <summary>Gets or sets the ship via.</summary>
        /// <value>The ship via.</value>
		public int? ShipVia
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.OrderBlob class.
        /// </summary>
		public OrderBlob()
		{
			this.OrderDetails = new List<OrderDetailBlob>();
		}

        /// <summary>Creates a new OrderBlob.</summary>
        /// <param name="orderId">Identifier for the order.</param>
        /// <returns>An OrderBlob.</returns>
		public static OrderBlob Create(int orderId)
		{
			
			OrderBlob orderBlob = new OrderBlob();
			orderBlob.Id = orderId;
			orderBlob.Customer = NorthwindFactory.Customer("ALFKI", "Alfreds Futterkiste", "Maria Anders", "Sales Representative", "Obere Str. 57", "Berlin", null, "12209", "Germany", "030-0074321", "030-0076545", null);
			orderBlob.Employee = NorthwindFactory.Employee(1, "Davolio", "Nancy", "Sales Representative", "Ms.", new DateTime?(NorthwindData.ToDateTime("12/08/1948")), new DateTime?(NorthwindData.ToDateTime("05/01/1992")), "507 - 20th Ave. E. Apt. 2A", "Seattle", "WA", "98122", "USA", "(206) 555-9857", "5467", null, "Education includes a BA in psychology from Colorado State University in 1970.  She also completed 'The Art of the Cold Call.'  Nancy is a member of Toastmasters International.", new int?(2), "http://accweb/emmployees/davolio.bmp");
			orderBlob.OrderDate = new DateTime?(NorthwindData.ToDateTime("7/4/1996"));
			orderBlob.RequiredDate = new DateTime?(NorthwindData.ToDateTime("8/1/1996"));
			orderBlob.ShippedDate = new DateTime?(NorthwindData.ToDateTime("7/16/1996"));
			orderBlob.ShipVia = new int?(5);
			orderBlob.Freight = new decimal(3238, 0, 0, false, 2);
			orderBlob.ShipName = "Vins et alcools Chevalier";
			orderBlob.ShipAddress = "59 rue de l'Abbaye";
			orderBlob.ShipCity = "Reims";
			orderBlob.ShipRegion = null;
			orderBlob.ShipPostalCode = "51100";
			orderBlob.ShipCountry = "France";
			
			orderBlob.OrderDetails = new List<OrderDetailBlob>
            {
                new OrderDetailBlob
                {
                    ProductId = 11, 
                    UnitPrice = 11m, 
                    Quantity = 14, 
                    Discount = 0.0
                }, 
                new OrderDetailBlob
                {
                    ProductId = 42, 
                    UnitPrice = 9.8m, 
                    Quantity = 10, 
                    Discount = 0.0
                }, 
                new OrderDetailBlob
                {
                    ProductId = 72, 
                    UnitPrice = 34.8m, 
                    Quantity = 5, 
                    Discount = 0.0
                }
            }; 
			
            orderBlob.IntIds = new List<int>
            {
                10, 20, 30
            };
			
            orderBlob.CharMap = new Dictionary<int, string>
            {
                
                {
                    1, "A"
                },              
                {
                    2, "B"
                }, 
                {
                    3, "C"
                }
            };
		
			return orderBlob;
		}
	}
}