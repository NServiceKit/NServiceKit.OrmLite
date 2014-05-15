using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SqlMapper
{
    /// <summary>A test assertions.</summary>
    static class TestAssertions
    {
        /// <summary>A T extension method that is equals.</summary>
        /// <exception cref="ApplicationException">Thrown when an Application error condition occurs.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">  The obj to act on.</param>
        /// <param name="other">The other.</param>
        public static void IsEquals<T>(this T obj, T other)
        {
            if (!obj.Equals(other))
            {
                throw new ApplicationException(string.Format("{0} should be equals to {1}", obj, other));
            }
        }

        /// <summary>An IEnumerable&lt;T&gt; extension method that is sequence equal.</summary>
        /// <exception cref="ApplicationException">Thrown when an Application error condition occurs.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">  The obj to act on.</param>
        /// <param name="other">The other.</param>
        public static void IsSequenceEqual<T>(this IEnumerable<T> obj, IEnumerable<T> other)
        {
            if (!obj.SequenceEqual(other))
            {
                throw new ApplicationException(string.Format("{0} should be equals to {1}", obj, other));
            }
        }

        /// <summary>A bool extension method that is false.</summary>
        /// <exception cref="ApplicationException">Thrown when an Application error condition occurs.</exception>
        /// <param name="b">The b to act on.</param>
        public static void IsFalse(this bool b)
        {
            if (b)
            {
                throw new ApplicationException("Expected false");
            }
        }

        /// <summary>An object extension method that is null.</summary>
        /// <exception cref="ApplicationException">Thrown when an Application error condition occurs.</exception>
        /// <param name="obj">The obj to act on.</param>
        public static void IsNull(this object obj)
        {
            if (obj != null)
            {
                throw new ApplicationException("Expected null");
            }
        }

    }

    /// <summary>A tests.</summary>
    class Tests
    {
        /// <summary>The connection.</summary>
        SqlConnection connection = Program.GetOpenConnection();

        /// <summary>Select list int.</summary>
        public void SelectListInt()
        {
            connection.Query<int>("select 1 union all select 2 union all select 3")
              .IsSequenceEqual(new[] { 1, 2, 3 });
        }

        /// <summary>Pass in int array.</summary>
        public void PassInIntArray()
        {
            connection.Query<int>("select * from (select 1 as Id union all select 2 union all select 3) as X where Id in @Ids", new { Ids = new int[] { 1, 2, 3 }.AsEnumerable() })
             .IsSequenceEqual(new[] { 1, 2, 3 });
        }

        /// <summary>Tests double parameter.</summary>
        public void TestDoubleParam()
        {
			connection.Query<double>("select @d", new { d = 0.1d }).First()
                .IsEquals(0.1d);
        }

        /// <summary>Tests bool parameter.</summary>
        public void TestBoolParam()
        {
			connection.Query<bool>("select @b", new { b = false }).First()
                .IsFalse();
        }

        /// <summary>Tests strings.</summary>
        public void TestStrings()
        {
			connection.Query<string>(@"select 'a' a union select 'b'")
                .IsSequenceEqual(new[] { "a", "b" });
        }

        /// <summary>A dog.</summary>
        public class Dog
        {
            /// <summary>Gets or sets the age.</summary>
            /// <value>The age.</value>
            public int? Age { get; set; }

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public Guid Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>Gets or sets the weight.</summary>
            /// <value>The weight.</value>
            public float? Weight { get; set; }

            /// <summary>Gets the ignored property.</summary>
            /// <value>The ignored property.</value>
            public int IgnoredProperty { get { return 1; } }
        }

        /// <summary>Tests strong type.</summary>
        public void TestStrongType()
        {
            var guid = Guid.NewGuid();
			var dog = connection.Query<Dog>("select Age = @Age, Id = @Id", new { Age = (int?)null, Id = guid });
            
            dog.Count()
                .IsEquals(1);

            dog.First().Age
                .IsNull();

            dog.First().Id
                .IsEquals(guid);
        }

		//public void TestExpando()
		//{
		//    var rows = connection.Query("select 1 A, 2 B union all select 3, 4");

		//    ((int)rows[0].A)
		//        .IsEquals(1);

		//    ((int)rows[0].B)
		//        .IsEquals(2);

		//    ((int)rows[1].A)
		//        .IsEquals(3);

		//    ((int)rows[1].B)
		//        .IsEquals(4);
		//}

        /// <summary>Tests string list.</summary>
        public void TestStringList()
        {
			connection.Query<string>("select * from (select 'a' as x union all select 'b' union all select 'c') as T where x in @strings", new { strings = new[] { "a", "b", "c" } })
                .IsSequenceEqual(new[] {"a","b","c"});
        }

        /// <summary>Tests execute command.</summary>
        public void TestExecuteCommand()
        {
			connection.Execute(@"
    set nocount on 
    create table #t(i int) 
    set nocount off 
    insert #t 
    select @a a union all select @b 
    set nocount on 
    drop table #t", new {a=1, b=2 }).IsEquals(2);
        }

        /// <summary>Tests massive strings.</summary>
        public void TestMassiveStrings()
        { 
            var str = new string('X', 20000);
			connection.Query<string>("select @a", new { a = str }).First()
                .IsEquals(str);
        }

    }
}
