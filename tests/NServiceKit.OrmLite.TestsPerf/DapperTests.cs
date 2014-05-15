using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite.SqlServer;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.TestsPerf
{
    /// <summary>A dapper tests.</summary>
    [TestFixture]
    public class DapperTests
    {
        /// <summary>A performance tests.</summary>
        private class PerformanceTests
        {
            /// <summary>A test.</summary>
            private class Test
            {
                /// <summary>Creates a new Test.</summary>
                /// <param name="iteration">The iteration.</param>
                /// <param name="name">     The name.</param>
                /// <returns>A Test.</returns>
                public static Test Create(Action<int> iteration, string name)
                {
                    return new Test {Iteration = iteration, Name = name};
                }

                /// <summary>Gets or sets the iteration.</summary>
                /// <value>The iteration.</value>
                public Action<int> Iteration { get; set; }

                /// <summary>Gets or sets the name.</summary>
                /// <value>The name.</value>
                public string Name { get; set; }

                /// <summary>Gets or sets the watch.</summary>
                /// <value>The watch.</value>
                public Stopwatch Watch { get; set; }
            }

            /// <summary>A tests.</summary>
            private class Tests : List<Test>
            {
                /// <summary>Adds iteration.</summary>
                /// <param name="iteration">The iteration.</param>
                /// <param name="name">     The name.</param>
                public void Add(Action<int> iteration, string name)
                {
                    Add(Test.Create(iteration, name));
                }

                /// <summary>Runs.</summary>
                /// <param name="iterations">The iterations.</param>
                public void Run(int iterations)
                {
                    // warmup 
                    foreach (var test in this)
                    {
                        test.Iteration(iterations + 1);
                        test.Watch = new Stopwatch();
                        test.Watch.Reset();
                    }

                    var rand = new Random();
                    for (int i = 1; i <= iterations; i++)
                    {
                        foreach (var test in this.OrderBy(ignore => rand.Next()))
                        {
                            test.Watch.Start();
                            test.Iteration(i);
                            test.Watch.Stop();
                        }
                    }

                    foreach (var test in this.OrderBy(t => t.Watch.ElapsedMilliseconds))
                    {
                        Console.WriteLine(test.Name + " took " + test.Watch.ElapsedMilliseconds + "ms");
                    }
                }
            }

            /// <summary>Runs.</summary>
            /// <param name="iterations">The iterations.</param>
            public void Run(int iterations)
            {
                OrmLiteConfig.DialectProvider = SqlServerOrmLiteDialectProvider.Instance; //Using SQL Server
                IDbConnection ormLiteCmd = GetOpenConnection();

                var tests = new Tests();
                //tests.Add(id => ormLiteCmd.GetById<Post>(id), "OrmLite Query GetById");
                //tests.Add(id => ormLiteCmd.First<Post>("SELECT * FROM Posts WHERE Id = {0}", id), "OrmLite Query First<SQL>");
                //tests.Add(id => ormLiteCmd.QuerySingle<Post>("Id", id), "OrmLite QuerySingle");
                tests.Add(id => ormLiteCmd.QueryById<Post>(id), "OrmLite Query QueryById");

                tests.Run(iterations);
            }
        }

        [Alias("Posts")]
        //[Soma.Core.Table(Name = "Posts")]

        /// <summary>A post.</summary>
        class Post
        {
            //[Soma.Core.Id(Soma.Core.IdKind.Identity)]

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

		//
		//public static readonly string connectionString = "Data Source=.;Initial Catalog=tempdb;Integrated Security=True";

        /// <summary>Source for the.</summary>
		public static readonly string connectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\App_Data\Dapper.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True";

        /// <summary>The database command.</summary>
        private IDbCommand dbCmd;

        /// <summary>Gets open connection.</summary>
        /// <returns>The open connection.</returns>
        public static SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
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

	while @i <= 5001
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

        /// <summary>Executes the performance tests.</summary>
        [Test]
        public void RunPerformanceTests()
        {
            EnsureDBSetup();
            var test = new PerformanceTests();
            const int iterations = 500;
            Console.WriteLine("Running {0} iterations that load up a post entity", iterations);

            Console.WriteLine("\n\n Run 1:\n");
            test.Run(iterations);
            Console.WriteLine("\n\n Run 2:\n");
            test.Run(iterations);
            Console.WriteLine("\n\n Run 3:\n");
            test.Run(iterations);
        }


    }
}