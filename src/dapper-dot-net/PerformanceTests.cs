using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using NServiceKit.OrmLite;
using NServiceKit.OrmLite.SqlServer;
using SqlMapper.Linq2Sql;
using System.Data.Linq;
using System.Diagnostics;
using Massive;

namespace SqlMapper
{
    /// <summary>A performance tests.</summary>
    class PerformanceTests
    {
        /// <summary>A test.</summary>
        class Test
        {
            /// <summary>Creates a new Test.</summary>
            /// <param name="iteration">The iteration.</param>
            /// <param name="name">     The name.</param>
            /// <returns>A Test.</returns>
            public static Test Create(Action<int> iteration, string name)
            {
                return new Test {Iteration = iteration, Name = name };
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
        class Tests : List<Test>
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

        /// <summary>Gets l 2 s context.</summary>
        /// <returns>The l 2 s context.</returns>
        static DataClassesDataContext GetL2SContext()
        {
            return new DataClassesDataContext(Program.GetOpenConnection());
        }

        /// <summary>Runs.</summary>
        /// <param name="iterations">The iterations.</param>
        public void Run(int iterations)
        {
            var tests = new Tests();

            var l2scontext1 = GetL2SContext();
            tests.Add(id => l2scontext1.Posts.First(p => p.Id == id), "Linq 2 SQL");

            var l2scontext2 = GetL2SContext();
            var compiledGetPost = CompiledQuery.Compile((Linq2Sql.DataClassesDataContext ctx, int id) => ctx.Posts.First(p => p.Id == id));
            tests.Add(id => compiledGetPost(l2scontext2,id), "Linq 2 SQL Compiled");

            var l2scontext3 = GetL2SContext();
            tests.Add(id => l2scontext3.ExecuteQuery<Post>("select * from Posts where Id = {0}", id).ToList(), "Linq 2 SQL ExecuteQuery");
            
			//Comment out EF to suppress exception
			//var entityContext = new EntityFramework.tempdbEntities1();
			//entityContext.Connection.Open();
			//tests.Add(id => entityContext.Posts.First(p => p.Id == id), "Entity framework");

			//var entityContext2 = new EntityFramework.tempdbEntities1();
			//entityContext2.Connection.Open();
			//tests.Add(id => entityContext.ExecuteStoreQuery<Post>("select * from Posts where Id = {0}", id).ToList(), "Entity framework ExecuteStoreQuery");

            var mapperConnection = Program.GetOpenConnection();
			tests.Add(id => mapperConnection.Query<Post>("select * from Posts where Id = @Id", new { Id = id }).ToList(), "Mapper Query");

			//var mapperConnection2 = Program.GetOpenConnection();
			//tests.Add(id => mapperConnection2.Query("select * from Posts where Id = @Id", new { Id = id }).ToList(), "Dynamic Mapper Query");

            var massiveModel = new DynamicModel(Program.connectionString);
            var massiveConnection = Program.GetOpenConnection();
            tests.Add(id => massiveModel.Query("select * from Posts where Id = @0", massiveConnection, id).ToList(), "Dynamic Massive ORM Query");
        	

			//NServiceKit.OrmLite Provider:
			OrmLiteConfig.DialectProvider = SqlServerOrmLiteDialectProvider.Instance; //Using SQL Server
			IDbConnection ormLiteConn = Program.GetOpenConnection();
			tests.Add(id => ormLiteConn.Select<Post>("select * from Posts where Id = {0}", id), "OrmLite Query");

            // HAND CODED 
            var connection = Program.GetOpenConnection();

            var postCommand = new SqlCommand();
            postCommand.Connection = connection;
            postCommand.CommandText = @"select Id, [Text], [CreationDate], LastChangeDate, 
                Counter1,Counter2,Counter3,Counter4,Counter5,Counter6,Counter7,Counter8,Counter9 from Posts where Id = @Id";
            var idParam = postCommand.Parameters.Add("@Id", System.Data.SqlDbType.Int);

            tests.Add(id => 
            {
                idParam.Value = id;

                using (var reader = postCommand.ExecuteReader())
                {
                    reader.Read();
                    var post = new Post();
                    post.Id = reader.GetInt32(0);
                    post.Text = reader.GetNullableString(1);
                    post.CreationDate = reader.GetDateTime(2);
                    post.LastChangeDate = reader.GetDateTime(3);

                    post.Counter1 = reader.GetNullableValue<int>(4);
                    post.Counter2 = reader.GetNullableValue<int>(5);
                    post.Counter3 = reader.GetNullableValue<int>(6);
                    post.Counter4 = reader.GetNullableValue<int>(7);
                    post.Counter5 = reader.GetNullableValue<int>(8);
                    post.Counter6 = reader.GetNullableValue<int>(9);
                    post.Counter7 = reader.GetNullableValue<int>(10);
                    post.Counter8 = reader.GetNullableValue<int>(11);
                    post.Counter9 = reader.GetNullableValue<int>(12);
                }
            }, "hand coded");

            tests.Run(iterations);
        }
    }

    /// <summary>A SQL data reader helper.</summary>
    static class SqlDataReaderHelper
    {
        /// <summary>A SqlDataReader extension method that gets nullable string.</summary>
        /// <param name="reader">The reader to act on.</param>
        /// <param name="index"> Zero-based index of the.</param>
        /// <returns>The nullable string.</returns>
        public static string GetNullableString(this SqlDataReader reader, int index) 
        {
            object tmp = reader.GetValue(index);
            if (tmp != DBNull.Value)
            {
                return (string)tmp;
            }
            return null;
        }

        /// <summary>A SqlDataReader extension method that gets nullable value.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="reader">The reader to act on.</param>
        /// <param name="index"> Zero-based index of the.</param>
        /// <returns>The nullable value.</returns>
        public static Nullable<T> GetNullableValue<T>(this SqlDataReader reader, int index) where T : struct
        {
            object tmp = reader.GetValue(index);
            if (tmp != DBNull.Value)
            {
                return (T)tmp;
            }
            return null;
        }
    }
}
