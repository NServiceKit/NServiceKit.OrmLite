using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Data;

using NServiceKit.Common.Utils;
using NServiceKit.DataAnnotations;
using NServiceKit.Common;
using System.Reflection;

using NServiceKit.OrmLite;
using NServiceKit.OrmLite.Firebird;

namespace TestLiteFirebird01
{
    /// <summary>An user.</summary>
	[Alias("USERS")]
	public  class User
	{
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		[Alias("ID")]
		[Sequence("USERS_ID_GEN")]
		public int Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		[Alias("NAME")]
    	public string Name { get; set; }

        /// <summary>Gets or sets the password.</summary>
        /// <value>The password.</value>
		[Alias("PASSWORD")]  
    	public string Password { get; set; }

        /// <summary>Gets or sets the col 1.</summary>
        /// <value>The col 1.</value>
		[Alias("COL1")]
    	public string Col1 { get; set; }

        /// <summary>Gets or sets the col 2.</summary>
        /// <value>The col 2.</value>
		[Alias("COL2")]
		public string Col2 { get; set; }

        /// <summary>Gets or sets the col 3.</summary>
        /// <value>The col 3.</value>
		[Alias("COL3")]
		public string Col3 { get; set; }

        /// <summary>Gets or sets a value indicating whether the active.</summary>
        /// <value>true if active, false if not.</value>
		[Alias("ACTIVEINTEGER")]
		public bool Active { get; set; }

        /// <summary>Gets or sets a value indicating whether the active 2.</summary>
        /// <value>true if active 2, false if not.</value>
		[Alias("ACTIVECHAR")]
		public bool Active2 { get; set; }

        /// <summary>Gets some string property.</summary>
        /// <value>some string property.</value>
		[Ignore]
		public string SomeStringProperty { 
			get{ return "SomeValue No from dB!!!";}
		}

        /// <summary>Gets some int 32 property.</summary>
        /// <value>some int 32 property.</value>
		[Ignore]
		public Int32 SomeInt32Property { 
			get{ return 35;}
		}

        /// <summary>Gets the Date/Time of some date time property.</summary>
        /// <value>some date time property.</value>
		[Ignore]
		public DateTime SomeDateTimeProperty { 
			get{ return DateTime.Now ;}
		}

        /// <summary>Gets some int 32 nullable property.</summary>
        /// <value>some int 32 nullable property.</value>
		[Ignore]
		public Int32? SomeInt32NullableProperty { 
			get{ return null;}
		}

        /// <summary>Gets the Date/Time of some date time nullable property.</summary>
        /// <value>some date time nullable property.</value>
		[Ignore]
		public DateTime? SomeDateTimeNullableProperty { 
			get{ return null ;}
		}
		
		
		
	}

    /// <summary>A main class.</summary>
	class MainClass
	{
        /// <summary>Main entry-point for this application.</summary>
        /// <param name="args">Array of command-line argument strings.</param>
		public static void Main (string[] args)
		{
			
			
			//Set one before use (i.e. in a static constructor).
			
			OrmLiteConfig.DialectProvider = new FirebirdOrmLiteDialectProvider();
									
			using (IDbConnection db =
			       "User=SYSDBA;Password=masterkey;Database=employee.fdb;DataSource=localhost;Dialect=3;charset=ISO8859_1;".OpenDbConnection())
			{
				//try{
					
					
    				db.Insert(new User 
					{ 	
						Name= string.Format("Hello, World! {0}", DateTime.Now),
						Password="jkkoo",
						Col1="01",
						Col2="02",
						Col3="03"
							
					});
					
					User user = new User(){
						Name="New User ",
						Password= "kka",
						Col1="XX",
						Col2="YY",
						Col3="ZZ",
						Active=true
					};
					
					
					db.Insert(user);
					
					Console.WriteLine("++++++++++Id for {0} {1}",user.Name,  user.Id);
						
					
    				var rows = db.Select<User>();
					
					Console.WriteLine("++++++++++++++records in users {0}", rows.Count);
					foreach(User u in rows){
						Console.WriteLine("{0} -- {1} -- {2} -- {3} -{4} --{5} ", u.Id, u.Name, u.SomeStringProperty, u.SomeDateTimeProperty,
						                  (u.SomeInt32NullableProperty.HasValue)?u.SomeDateTimeNullableProperty.Value.ToString(): "",
						                  u.Active);						
						db.Delete(u);
					}
					
					rows = db.Select<User>();
					
					Console.WriteLine("-------------records in users after delete {0}", rows.Count);
					
				//}	
				
				//catch(Exception e){
				//	Console.WriteLine(e);
				//}
			}

		}
	}
}

