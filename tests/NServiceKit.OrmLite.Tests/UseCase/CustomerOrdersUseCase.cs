using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using NServiceKit.DataAnnotations;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.Tests.UseCase
{
    /// <summary>Values that represent PhoneType.</summary>
    public enum PhoneType
    {
        /// <summary>An enum constant representing the home option.</summary>
        Home,

        /// <summary>An enum constant representing the work option.</summary>
        Work,

        /// <summary>An enum constant representing the mobile option.</summary>
        Mobile,
    }

    /// <summary>Values that represent AddressType.</summary>
    public enum AddressType
    {
        /// <summary>An enum constant representing the home option.</summary>
        Home,

        /// <summary>An enum constant representing the work option.</summary>
        Work,

        /// <summary>An enum constant representing the other option.</summary>
        Other,
    }

    /// <summary>An address.</summary>
    public class Address
    {
        /// <summary>Gets or sets the line 1.</summary>
        /// <value>The line 1.</value>
        public string Line1 { get; set; }

        /// <summary>Gets or sets the line 2.</summary>
        /// <value>The line 2.</value>
        public string Line2 { get; set; }

        /// <summary>Gets or sets the zip code.</summary>
        /// <value>The zip code.</value>
        public string ZipCode { get; set; }

        /// <summary>Gets or sets the state.</summary>
        /// <value>The state.</value>
        public string State { get; set; }

        /// <summary>Gets or sets the city.</summary>
        /// <value>The city.</value>
        public string City { get; set; }

        /// <summary>Gets or sets the country.</summary>
        /// <value>The country.</value>
        public string Country { get; set; }
    }

    /// <summary>A customer.</summary>
    public class Customer
    {
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.Tests.UseCase.Customer class.
        /// </summary>
        public Customer()
        {
            this.PhoneNumbers = new Dictionary<PhoneType, string>();
            this.Addresses = new Dictionary<AddressType, Address>();
        }

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement] // Creates Auto primary key
        public int Id { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        /// <value>The name of the first.</value>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        /// <value>The name of the last.</value>
        public string LastName { get; set; }

        /// <summary>Gets or sets the email.</summary>
        /// <value>The email.</value>
        [Index(Unique = true)] // Creates Unique Index
        public string Email { get; set; }

        /// <summary>Gets or sets the phone numbers.</summary>
        /// <value>The phone numbers.</value>
        public Dictionary<PhoneType, string> PhoneNumbers { get; set; }

        /// <summary>Gets or sets the addresses.</summary>
        /// <value>The addresses.</value>
        public Dictionary<AddressType, Address> Addresses { get; set; }

        /// <summary>Gets or sets the Date/Time of the created at.</summary>
        /// <value>The created at.</value>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>An order.</summary>
    public class Order
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the customer.</summary>
        /// <value>The identifier of the customer.</value>
        [References(typeof(Customer))] //Creates Foreign Key
        public int CustomerId { get; set; }

        /// <summary>Gets or sets the identifier of the employee.</summary>
        /// <value>The identifier of the employee.</value>
        [References(typeof(Employee))] //Creates Foreign Key
        public int EmployeeId { get; set; }

        /// <summary>Gets or sets the shipping address.</summary>
        /// <value>The shipping address.</value>
        public Address ShippingAddress { get; set; } //Blobbed (no Address table)

        /// <summary>Gets or sets the order date.</summary>
        /// <value>The order date.</value>
        public DateTime? OrderDate { get; set; }

        /// <summary>Gets or sets the required date.</summary>
        /// <value>The required date.</value>
        public DateTime? RequiredDate { get; set; }

        /// <summary>Gets or sets the shipped date.</summary>
        /// <value>The shipped date.</value>
        public DateTime? ShippedDate { get; set; }

        /// <summary>Gets or sets the ship via.</summary>
        /// <value>The ship via.</value>
        public int? ShipVia { get; set; }

        /// <summary>Gets or sets the freight.</summary>
        /// <value>The freight.</value>
        public decimal Freight { get; set; }

        /// <summary>Gets or sets the number of. </summary>
        /// <value>The total.</value>
        public decimal Total { get; set; }
    }

    /// <summary>An order detail.</summary>
    public class OrderDetail
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the order.</summary>
        /// <value>The identifier of the order.</value>
        [References(typeof(Order))] //Creates Foreign Key
        public int OrderId { get; set; }

        /// <summary>Gets or sets the identifier of the product.</summary>
        /// <value>The identifier of the product.</value>
        public int ProductId { get; set; }

        /// <summary>Gets or sets the unit price.</summary>
        /// <value>The unit price.</value>
        public decimal UnitPrice { get; set; }

        /// <summary>Gets or sets the quantity.</summary>
        /// <value>The quantity.</value>
        public short Quantity { get; set; }

        /// <summary>Gets or sets the discount.</summary>
        /// <value>The discount.</value>
        public decimal Discount { get; set; }
    }

    /// <summary>An employee.</summary>
    public class Employee
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>A product.</summary>
    public class Product
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the unit price.</summary>
        /// <value>The unit price.</value>
        public decimal UnitPrice { get; set; }
    }

    /// <summary>A customer orders use case.</summary>
    [TestFixture]
    public class CustomerOrdersUseCase
    {
        /// <summary>Stand-alone class, No other configs, nothing but POCOs.</summary>
        [Test]
        public void Run()
        {
            LogManager.LogFactory = new ConsoleLogFactory();

            //var dbFactory = new OrmLiteConnectionFactory(
            //    @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\App_Data\Database1.mdf;Integrated Security=True;User Instance=True",
            //    SqlServerDialect.Provider);

            //Use in-memory Sqlite DB instead
            //var dbFactory = new OrmLiteConnectionFactory(
            //    ":memory:", false, SqliteDialect.Provider);
            
            //If you are trying to get this to build as a standalone example, use one of the dbFactory methods
            //  above, instead of Config.OpenDbConnection in the using statement below.

            //Non-intrusive: All extension methods hang off System.Data.* interfaces
            using (IDbConnection db = Config.OpenDbConnection())
            {
                //Re-Create all table schemas:
                db.DropTable<OrderDetail>();
                db.DropTable<Order>();
                db.DropTable<Customer>();
                db.DropTable<Product>();
                db.DropTable<Employee>();

                db.CreateTable<Employee>();
                db.CreateTable<Product>();
                db.CreateTable<Customer>();
                db.CreateTable<Order>();
                db.CreateTable<OrderDetail>();

                db.Insert(new Employee { Id = 1, Name = "Employee 1" });
                db.Insert(new Employee { Id = 2, Name = "Employee 2" });
                var product1 = new Product { Id = 1, Name = "Product 1", UnitPrice = 10 };
                var product2 = new Product { Id = 2, Name = "Product 2", UnitPrice = 20 };
                db.Save(product1, product2);

                var customer = new Customer {
                    FirstName = "Orm",
                    LastName = "Lite",
                    Email = "ormlite@servicestack.net",
                    PhoneNumbers =
                    {
                        { PhoneType.Home, "555-1234" },
                        { PhoneType.Work, "1-800-1234" },
                        { PhoneType.Mobile, "818-123-4567" },
                    },
                    Addresses =
                    {
                        { AddressType.Work, new Address { Line1 = "1 Street", Country = "US", State = "NY", City = "New York", ZipCode = "10101" } },
                    },
                    CreatedAt = DateTime.UtcNow,
                };
                db.Insert(customer);

                var customerId = db.GetLastInsertId(); //Get Auto Inserted Id
                customer = db.QuerySingle<Customer>(new { customer.Email }); //Query
                Assert.That(customer.Id, Is.EqualTo(customerId));

                //Direct access to System.Data.Transactions:
                using (IDbTransaction trans = db.OpenTransaction(IsolationLevel.ReadCommitted))
                {
                    var order = new Order {
                        CustomerId = customer.Id,
                        EmployeeId = 1,
                        OrderDate = DateTime.UtcNow,
                        Freight = 10.50m,
                        ShippingAddress = new Address { Line1 = "3 Street", Country = "US", State = "NY", City = "New York", ZipCode = "12121" },
                    };
                    db.Save(order); //Inserts 1st time

                    order.Id = (int)db.GetLastInsertId(); //Get Auto Inserted Id

                    var orderDetails = new[] {
                        new OrderDetail {
                            OrderId = order.Id,
                            ProductId = product1.Id,
                            Quantity = 2,
                            UnitPrice = product1.UnitPrice,
                        },
                        new OrderDetail {
                            OrderId = order.Id,
                            ProductId = product2.Id,
                            Quantity = 2,
                            UnitPrice = product2.UnitPrice,
                            Discount = .15m,
                        }
                    };

                    db.Insert(orderDetails);

                    order.Total = orderDetails.Sum(x => x.UnitPrice * x.Quantity * x.Discount) + order.Freight;

                    db.Save(order); //Updates 2nd Time

                    trans.Commit();
                }
            }
        }
    }

}
