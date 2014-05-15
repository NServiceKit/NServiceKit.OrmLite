using System;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Common.DataModel{

    /// <summary>An employee.</summary>
	[Alias("Employees")]
	public class Employee : IHasIntId, IHasId<int>
	{
        /// <summary>Gets or sets the address.</summary>
        /// <value>The address.</value>
		[StringLength(60)]
		public string Address
		{
			get;
			set;
		}

        /// <summary>Gets or sets the birth date.</summary>
        /// <value>The birth date.</value>
		public DateTime? BirthDate
		{
			get;
			set;
		}

        /// <summary>Gets or sets the city.</summary>
        /// <value>The city.</value>
		[StringLength(15)]
		public string City
		{
			get;
			set;
		}

        /// <summary>Gets or sets the country.</summary>
        /// <value>The country.</value>
		[StringLength(15)]
		public string Country
		{
			get;
			set;
		}

        /// <summary>Gets or sets the extension.</summary>
        /// <value>The extension.</value>
		[StringLength(4)]
		public string Extension
		{
			get;
			set;
		}

        /// <summary>Gets or sets the person's first name.</summary>
        /// <value>The name of the first.</value>
		[Required]
		[StringLength(10)]
		public string FirstName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the hire date.</summary>
        /// <value>The hire date.</value>
		public DateTime? HireDate
		{
			get;
			set;
		}

        /// <summary>Gets or sets the home phone.</summary>
        /// <value>The home phone.</value>
		[StringLength(24)]
		public string HomePhone
		{
			get;
			set;
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[AutoIncrement]
		[Alias("EmployeeID")]
		public int Id
		{
			get;
			set;
		}

        /// <summary>Gets or sets the person's last name.</summary>
        /// <value>The name of the last.</value>
		[Required]
		[StringLength(20)]
		[Index]
		public string LastName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the notes.</summary>
        /// <value>The notes.</value>
		public string Notes
		{
			get;
			set;
		}

        /// <summary>Gets or sets the photo.</summary>
        /// <value>The photo.</value>
		public byte[] Photo
		{
			get;
			set;
		}

        /// <summary>Gets or sets the full pathname of the photo file.</summary>
        /// <value>The full pathname of the photo file.</value>
		[StringLength(255)]
		public string PhotoPath
		{
			get;
			set;
		}

        /// <summary>Gets or sets the postal code.</summary>
        /// <value>The postal code.</value>
		[StringLength(10)]
		[Index]
		public string PostalCode
		{
			get;
			set;
		}

        /// <summary>Gets or sets the region.</summary>
        /// <value>The region.</value>
		[StringLength(15)]
		public string Region
		{
			get;
			set;
		}

        /// <summary>Gets or sets the reports to.</summary>
        /// <value>The reports to.</value>
		[References(typeof(Employee))]
		public int? ReportsTo
		{
			get;
			set;
		}

        /// <summary>Gets or sets the title.</summary>
        /// <value>The title.</value>
		[StringLength(30)]
		public string Title
		{
			get;
			set;
		}

        /// <summary>Gets or sets the title of courtesy.</summary>
        /// <value>The title of courtesy.</value>
		[StringLength(25)]
		public string TitleOfCourtesy
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the Northwind.Common.DataModel.Employee class.
        /// </summary>
		public Employee()
		{
		}
	}
}