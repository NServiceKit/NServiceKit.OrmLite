using System;
using System.Collections.Generic;

namespace Northwind.Common.DataModel{

    /// <summary>A northwind factory.</summary>
	public static class NorthwindFactory
	{
        /// <summary>List of types of the models.</summary>
		public readonly static List<Type> ModelTypes;

        /// <summary>
        /// Initializes static members of the Northwind.Common.DataModel.NorthwindFactory class.
        /// </summary>
		static NorthwindFactory()
		{
			List<Type> types = new List<Type>();
			types.Add(typeof(Employee));
			types.Add(typeof(Category));
			types.Add(typeof(Customer));
			types.Add(typeof(Shipper));
			types.Add(typeof(Supplier));
			types.Add(typeof(Order));
			types.Add(typeof(Product));
			types.Add(typeof(OrderDetail));
			types.Add(typeof(CustomerCustomerDemo));
			types.Add(typeof(Category));
			types.Add(typeof(CustomerDemographic));
			types.Add(typeof(Region));
			types.Add(typeof(Territory));
			types.Add(typeof(EmployeeTerritory));
			NorthwindFactory.ModelTypes = types;
		}

        /// <summary>Categories.</summary>
        /// <param name="id">          The identifier.</param>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="description"> The description.</param>
        /// <param name="picture">     The picture.</param>
        /// <returns>A Category.</returns>
		public static Category Category(int id, string categoryName, string description, byte[] picture)
		{
			Category category = new Category();
			category.Id = id;
			category.CategoryName = categoryName;
			category.Description = description;
			category.Picture = picture;
			return category;
		}

        /// <summary>Customers.</summary>
        /// <param name="customerId">  Identifier for the customer.</param>
        /// <param name="companyName"> Name of the company.</param>
        /// <param name="contactName"> Name of the contact.</param>
        /// <param name="contactTitle">The contact title.</param>
        /// <param name="address">     The address.</param>
        /// <param name="city">        The city.</param>
        /// <param name="region">      The region.</param>
        /// <param name="postalCode">  The postal code.</param>
        /// <param name="country">     The country.</param>
        /// <param name="phoneNo">     The phone no.</param>
        /// <param name="faxNo">       The fax no.</param>
        /// <param name="picture">     The picture.</param>
        /// <returns>A Customer.</returns>
		public static Customer Customer(string customerId, string companyName, string contactName, string contactTitle, string address, string city, string region, string postalCode, string country, string phoneNo, string faxNo, byte[] picture)
		{
			Customer customer = new Customer();
			customer.Id = customerId;
			customer.CompanyName = companyName;
			customer.ContactName = contactName;
			customer.ContactTitle = contactTitle;
			customer.Address = address;
			customer.City = city;
			customer.Region = region;
			customer.PostalCode = postalCode;
			customer.Country = country;
			customer.Phone = phoneNo;
			customer.Fax = faxNo;
			customer.Picture = picture;
			return customer;
		}

        /// <summary>Customer demo.</summary>
        /// <param name="customerId">    Identifier for the customer.</param>
        /// <param name="customerTypeId">Identifier for the customer type.</param>
        /// <returns>A CustomerCustomerDemo.</returns>
		public static CustomerCustomerDemo CustomerCustomerDemo(string customerId, string customerTypeId)
		{
			CustomerCustomerDemo customerCustomerDemo = new CustomerCustomerDemo();
			customerCustomerDemo.Id = customerId;
			customerCustomerDemo.CustomerTypeId = customerTypeId;
			return customerCustomerDemo;
		}

        /// <summary>Employees.</summary>
        /// <param name="employeeId">     Identifier for the employee.</param>
        /// <param name="lastName">       The person's last name.</param>
        /// <param name="firstName">      The person's first name.</param>
        /// <param name="title">          The title.</param>
        /// <param name="titleOfCourtesy">The title of courtesy.</param>
        /// <param name="birthDate">      The birth date.</param>
        /// <param name="hireDate">       The hire date.</param>
        /// <param name="address">        The address.</param>
        /// <param name="city">           The city.</param>
        /// <param name="region">         The region.</param>
        /// <param name="postalCode">     The postal code.</param>
        /// <param name="country">        The country.</param>
        /// <param name="homePhone">      The home phone.</param>
        /// <param name="extension">      The extension.</param>
        /// <param name="photo">          The photo.</param>
        /// <param name="notes">          The notes.</param>
        /// <param name="reportsTo">      The reports to.</param>
        /// <param name="photoPath">      Full pathname of the photo file.</param>
        /// <returns>An Employee.</returns>
		public static Employee Employee(int employeeId, string lastName, string firstName, string title, string titleOfCourtesy, DateTime? birthDate, DateTime? hireDate, string address, string city, string region, string postalCode, string country, string homePhone, string extension, byte[] photo, string notes, int? reportsTo, string photoPath)
		{
			Employee employee = new Employee();
			employee.Id = employeeId;
			employee.LastName = lastName;
			employee.FirstName = firstName;
			employee.Title = title;
			employee.TitleOfCourtesy = titleOfCourtesy;
			employee.BirthDate = birthDate;
			employee.HireDate = hireDate;
			employee.Address = address;
			employee.City = city;
			employee.Region = region;
			employee.PostalCode = postalCode;
			employee.Country = country;
			employee.HomePhone = homePhone;
			employee.Extension = extension;
			employee.Photo = photo;
			employee.Notes = notes;
			employee.ReportsTo = reportsTo;
			employee.PhotoPath = photoPath;
			return employee;
		}

        /// <summary>Employee territory.</summary>
        /// <param name="employeeId"> Identifier for the employee.</param>
        /// <param name="territoryId">Identifier for the territory.</param>
        /// <returns>An EmployeeTerritory.</returns>
		public static EmployeeTerritory EmployeeTerritory(int employeeId, string territoryId)
		{
			EmployeeTerritory employeeTerritory = new EmployeeTerritory();
			employeeTerritory.EmployeeId = employeeId;
			employeeTerritory.TerritoryId = territoryId;
			return employeeTerritory;
		}

        /// <summary>Orders.</summary>
        /// <param name="orderId">     Identifier for the order.</param>
        /// <param name="customerId">  Identifier for the customer.</param>
        /// <param name="employeeId">  Identifier for the employee.</param>
        /// <param name="orderDate">   The order date.</param>
        /// <param name="requiredDate">The required date.</param>
        /// <param name="shippedDate"> The shipped date.</param>
        /// <param name="shipVia">     The ship via.</param>
        /// <param name="freight">     The freight.</param>
        /// <param name="shipName">    Name of the ship.</param>
        /// <param name="address">     The address.</param>
        /// <param name="city">        The city.</param>
        /// <param name="region">      The region.</param>
        /// <param name="postalCode">  The postal code.</param>
        /// <param name="country">     The country.</param>
        /// <returns>An Order.</returns>
		public static Order Order(int orderId, string customerId, int employeeId, DateTime? orderDate, DateTime? requiredDate, DateTime? shippedDate, int shipVia, decimal freight, string shipName, string address, string city, string region, string postalCode, string country)
		{
			Order order = new Order();
			order.Id = orderId;
			order.CustomerId = customerId;
			order.EmployeeId = employeeId;
			order.OrderDate = orderDate;
			order.RequiredDate = requiredDate;
			order.ShippedDate = shippedDate;
			order.ShipVia = new int?(shipVia);
			order.Freight = freight;
			order.ShipName = shipName;
			order.ShipAddress = address;
			order.ShipCity = city;
			order.ShipRegion = region;
			order.ShipPostalCode = postalCode;
			order.ShipCountry = country;
			return order;
		}

        /// <summary>Order detail.</summary>
        /// <param name="orderId">  Identifier for the order.</param>
        /// <param name="productId">Identifier for the product.</param>
        /// <param name="unitPrice">The unit price.</param>
        /// <param name="quantity"> The quantity.</param>
        /// <param name="discount"> The discount.</param>
        /// <returns>An OrderDetail.</returns>
		public static OrderDetail OrderDetail(int orderId, int productId, decimal unitPrice, short quantity, double discount)
		{
			OrderDetail orderDetail = new OrderDetail();
			orderDetail.OrderId = orderId;
			orderDetail.ProductId = productId;
			orderDetail.UnitPrice = unitPrice;
			orderDetail.Quantity = quantity;
			orderDetail.Discount = discount;
			return orderDetail;
		}

        /// <summary>Products.</summary>
        /// <param name="productId">   Identifier for the product.</param>
        /// <param name="productName"> Name of the product.</param>
        /// <param name="supplierId">  Identifier for the supplier.</param>
        /// <param name="categoryId">  Identifier for the category.</param>
        /// <param name="qtyPerUnit">  The qty per unit.</param>
        /// <param name="unitPrice">   The unit price.</param>
        /// <param name="unitsInStock">The units in stock.</param>
        /// <param name="unitsOnOrder">The units on order.</param>
        /// <param name="reorderLevel">The reorder level.</param>
        /// <param name="discontinued">true if discontinued.</param>
        /// <returns>A Product.</returns>
		public static Product Product(int productId, string productName, int supplierId, int categoryId, string qtyPerUnit, decimal unitPrice, short unitsInStock, short unitsOnOrder, short reorderLevel, bool discontinued)
		{
			Product product = new Product();
			product.Id = productId;
			product.ProductName = productName;
			product.SupplierId = supplierId;
			product.CategoryId = categoryId;
			product.QuantityPerUnit = qtyPerUnit;
			product.UnitPrice = unitPrice;
			product.UnitsInStock = unitsInStock;
			product.UnitsOnOrder = unitsOnOrder;
			product.ReorderLevel = reorderLevel;
			product.Discontinued = discontinued;
			return product;
		}

        /// <summary>Regions.</summary>
        /// <param name="regionId">         Identifier for the region.</param>
        /// <param name="regionDescription">Information describing the region.</param>
        /// <returns>A Region.</returns>
		public static Region Region(int regionId, string regionDescription)
		{
			Region region = new Region();
			region.Id = regionId;
			region.RegionDescription = regionDescription;
			return region;
		}

        /// <summary>Shippers.</summary>
        /// <param name="id">         The identifier.</param>
        /// <param name="companyName">Name of the company.</param>
        /// <param name="phoneNo">    The phone no.</param>
        /// <returns>A Shipper.</returns>
		public static Shipper Shipper(int id, string companyName, string phoneNo)
		{
			Shipper shipper = new Shipper();
			shipper.Id = id;
			shipper.CompanyName = companyName;
			shipper.Phone = phoneNo;
			return shipper;
		}

        /// <summary>Suppliers.</summary>
        /// <param name="supplierId">  Identifier for the supplier.</param>
        /// <param name="companyName"> Name of the company.</param>
        /// <param name="contactName"> Name of the contact.</param>
        /// <param name="contactTitle">The contact title.</param>
        /// <param name="address">     The address.</param>
        /// <param name="city">        The city.</param>
        /// <param name="region">      The region.</param>
        /// <param name="postalCode">  The postal code.</param>
        /// <param name="country">     The country.</param>
        /// <param name="phoneNo">     The phone no.</param>
        /// <param name="faxNo">       The fax no.</param>
        /// <param name="homePage">    The home page.</param>
        /// <returns>A Supplier.</returns>
		public static Supplier Supplier(int supplierId, string companyName, string contactName, string contactTitle, string address, string city, string region, string postalCode, string country, string phoneNo, string faxNo, string homePage)
		{
			Supplier supplier = new Supplier();
			supplier.Id = supplierId;
			supplier.CompanyName = companyName;
			supplier.ContactName = contactName;
			supplier.ContactTitle = contactTitle;
			supplier.Address = address;
			supplier.City = city;
			supplier.Region = region;
			supplier.PostalCode = postalCode;
			supplier.Country = country;
			supplier.Phone = phoneNo;
			supplier.Fax = faxNo;
			supplier.HomePage = homePage;
			return supplier;
		}

        /// <summary>Territories.</summary>
        /// <param name="territoryId">         Identifier for the territory.</param>
        /// <param name="territoryDescription">Information describing the territory.</param>
        /// <param name="regionId">            Identifier for the region.</param>
        /// <returns>A Territory.</returns>
		public static Territory Territory(string territoryId, string territoryDescription, int regionId)
		{
			Territory territory = new Territory();
			territory.Id = territoryId;
			territory.TerritoryDescription = territoryDescription;
			territory.RegionId = regionId;
			return territory;
		}
	}
}