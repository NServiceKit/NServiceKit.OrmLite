using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceKit.OrmLite;
using System.IO;
using System.Data;
using NServiceKit.DataAnnotations;
using NServiceKit.Common.Utils;
using NServiceKit.OrmLite.SqlServer;

namespace SqlServerExpressionsTest
{
    /// <summary>A user 2.</summary>
    public class User_2
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public long Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        [Index]
        public string Name { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        /// <value>The created date.</value>
        public DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the identifier of the user data.</summary>
        /// <value>The identifier of the user data.</value>
        public long? UserDataId { get; set; }

        /// <summary>Gets or sets the identifier of the user service.</summary>
        /// <value>The identifier of the user service.</value>
        public long UserServiceId { get; set; }

        /// <summary>Gets or sets the user data value.</summary>
        /// <value>The user data value.</value>
        [Ignore]
        [Alias("DataValue")]
        public string UserDataValue { get; set; }

        /// <summary>Gets or sets the name of the user service.</summary>
        /// <value>The name of the user service.</value>
        [Ignore]
        [Alias("ServiceName")]
        public string UserServiceName { get; set; }
    }

    /// <summary>A user data 2.</summary>
    [Alias("UserData_2")]
    public class UserData_2
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public long Id { get; set; }

        /// <summary>Gets or sets the data value.</summary>
        /// <value>The data value.</value>
        public string DataValue { get; set; }
    }

    /// <summary>A user service 2.</summary>
    [Alias("UserService_2")]
    public class UserService_2
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public long Id { get; set; }

        /// <summary>Gets or sets the name of the service.</summary>
        /// <value>The name of the service.</value>
        public string ServiceName { get; set; }
    }

    /// <summary>An ignored field select test.</summary>
    public class IgnoredFieldSelectTest
    {
        /// <summary>Tests.</summary>
        /// <param name="connectionString">The connection string.</param>
        public static void Test(string connectionString)
        {
            //using (IDbConnection db = ":memory:".OpenDbConnection())
            using (IDbConnection db = connectionString.OpenDbConnection())
            {
                db.CreateTable<User_2>(true);
                db.CreateTable<UserData_2>(true);
                db.CreateTable<UserService_2>(true);

                //Insert Test
                db.Insert(new UserData_2 { Id = 5, DataValue = "Value-5" });
                db.Insert(new UserData_2 { Id = 6, DataValue = "Value-6" });

                db.Insert(new UserService_2 { Id = 8, ServiceName = "Value-8" });
                db.Insert(new UserService_2 { Id = 9, ServiceName = "Value-9" });

                var user2 = new User_2 { Id = 1, Name = "A", CreatedDate = DateTime.Now, UserDataId = 5, UserServiceId = 8 };
                db.Insert(user2);
                db.Insert(new User_2 { Id = 2, Name = "B", CreatedDate = DateTime.Now, UserDataId = 5, UserServiceId = 9 });
                db.Insert(new User_2 { Id = 3, Name = "B", CreatedDate = DateTime.Now });
                
                //Update Test
                user2.CreatedDate = DateTime.Now;
                db.Update<User_2>(user2,x=>x.Id == 1);

                //Select Test

                var rowsB = db.Select<User_2>("Name = {0}", "B");
                var rowsB1 = db.Select<User_2>(user => user.Name == "B");

                var rowsUData = db.Select<UserData_2>();
                var rowsUServ = db.Select<UserService_2>();

                var jn2 = new JoinSqlBuilder<User_2, User_2>();
                jn2 = jn2.Join<User_2, UserData_2>(x => x.UserDataId, x => x.Id, x => new { x.Name, x.Id }, x => new { x.DataValue})
                       .Join<User_2, UserService_2>(x => x.UserServiceId, x => x.Id, null, x => new { x.ServiceName })
                       .OrderByDescending<User_2>(x => x.Name)
                       .OrderBy<User_2>(x => x.Id)
                       .Select<User_2>(x => x.Id);

                var sql2 = jn2.ToSql();
                var items2 = db.Query<User_2>(sql2);
                Console.WriteLine("Ignored Field Selected Items - {0}",items2.Count());

                var item = db.FirstOrDefault<User_2>(sql2);
                
            }
        }
    }
}
