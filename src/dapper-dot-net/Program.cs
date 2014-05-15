using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data.Metadata.Edm;
using System.Reflection;
using System.Data.EntityClient;
using System.Data.Linq;
using NServiceKit.DataAnnotations;

namespace SqlMapper
{
    /// <summary>A post.</summary>
    class Post
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the text.</summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>Gets or sets the creation date.</summary>
        /// <value>The creation date.</value>
        public DateTime CreationDate { get; set; }

        /// <summary>Gets or sets the last change date.</summary>
        /// <value>The last change date.</value>
        public DateTime LastChangeDate { get; set; }

        /// <summary>Gets or sets the counter 1.</summary>
        /// <value>The counter 1.</value>
        public int? Counter1 { get; set; }

        /// <summary>Gets or sets the counter 2.</summary>
        /// <value>The counter 2.</value>
        public int? Counter2 { get; set; }

        /// <summary>Gets or sets the counter 3.</summary>
        /// <value>The counter 3.</value>
        public int? Counter3 { get; set; }

        /// <summary>Gets or sets the counter 4.</summary>
        /// <value>The counter 4.</value>
        public int? Counter4 { get; set; }

        /// <summary>Gets or sets the counter 5.</summary>
        /// <value>The counter 5.</value>
        public int? Counter5 { get; set; }

        /// <summary>Gets or sets the counter 6.</summary>
        /// <value>The counter 6.</value>
        public int? Counter6 { get; set; }

        /// <summary>Gets or sets the counter 7.</summary>
        /// <value>The counter 7.</value>
        public int? Counter7 { get; set; }

        /// <summary>Gets or sets the counter 8.</summary>
        /// <value>The counter 8.</value>
        public int? Counter8 { get; set; }

        /// <summary>Gets or sets the counter 9.</summary>
        /// <value>The counter 9.</value>
        public int? Counter9 { get; set; }

    }

    /// <summary>A program.</summary>
    class Program
    {
        /// <summary>Source for the.</summary>
		public static readonly string connectionString = "Data Source=IO\\SQLEXPRESS;Initial Catalog=tempdb;Integrated Security=True";
		//public static readonly string connectionString = "Data Source=.;Initial Catalog=tempdb;Integrated Security=True";

        /// <summary>Gets open connection.</summary>
        /// <returns>The open connection.</returns>
        public static SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>Executes the performance tests.</summary>
        static void RunPerformanceTests()
        {
            var test = new PerformanceTests();
            Console.WriteLine("Running 500 itrations that load up a post entity");
            test.Run(500);
        }

        /// <summary>Main entry-point for this application.</summary>
        /// <param name="args">Array of command-line argument strings.</param>
        static void Main(string[] args)
        {

#if DEBUG
            RunTests();
#else 
            EnsureDBSetup();
            RunPerformanceTests();
#endif

            Console.ReadKey();
        }

        /// <summary>Ensures that database setup.</summary>
        private static void EnsureDBSetup()
        {
            using (var cnn = GetOpenConnection())
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandText = @"
if (OBJECT_ID('Posts') is null)
begin
	create table Posts
	(
		Id int identity primary key, 
		[Text] varchar(max) not null, 
		CreationDate datetime not null, 
		LastChangeDate datetime not null,
		Counter1 int,
		Counter2 int,
		Counter3 int,
		Counter4 int,
		Counter5 int,
		Counter6 int,
		Counter7 int,
		Counter8 int,
		Counter9 int
	)
	   
	set nocount on 

	declare @i int
	declare @c int

	declare @id int

	set @i = 0

	while @i < 5000
	begin 
		
		insert Posts ([Text],CreationDate, LastChangeDate) values (replicate('x', 2000), GETDATE(), GETDATE())
		set @id = @@IDENTITY
		
		set @i = @i + 1
	end
end
";
                cmd.Connection = cnn;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Executes the tests.</summary>
        private static void RunTests()
        {
            var tester = new Tests();
            foreach (var method in typeof(Tests).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                Console.Write("Running " + method.Name);
                method.Invoke(tester, null);
                Console.WriteLine(" - OK!");
            }
        }
    }
}
