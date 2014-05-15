using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceKit.OrmLite;
using System.IO;
using System.Data;
using NServiceKit.DataAnnotations;
using NServiceKit.Common.Utils;
using NServiceKit.OrmLite.Sqlite;

namespace SqliteExpressionsTest
{
    /// <summary>An user.</summary>
    public class User
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

    }

    /// <summary>A user ex.</summary>
    public class UserEx
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [BelongTo(typeof(User))]
        public long Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        [BelongTo(typeof(User))]
        public string Name { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        /// <value>The created date.</value>
        public DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the user data value.</summary>
        /// <value>The user data value.</value>
        [BelongTo(typeof(UserData))]
        public string UserDataValue { get; set; }

        /// <summary>Gets or sets the name of the user service.</summary>
        /// <value>The name of the user service.</value>
        [BelongTo(typeof(UserService))]
        [Alias("ServiceName")]
        public string UserServiceName { get; set; }
    }

    /// <summary>A user data.</summary>
    [Alias("UserData")]
    public class UserData
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public long Id { get; set; }

        /// <summary>Gets or sets the user data value.</summary>
        /// <value>The user data value.</value>
        public string UserDataValue { get; set; }
    }

    /// <summary>A user service.</summary>
    [Alias("UserService")]
    public class UserService
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public long Id { get; set; }

        /// <summary>Gets or sets the name of the service.</summary>
        /// <value>The name of the service.</value>
        public string ServiceName { get; set; }
    }

    /// <summary>A join test.</summary>
    public class JoinTest
    {
        /// <summary>Gets file connection string.</summary>
        /// <returns>The file connection string.</returns>
        private static string GetFileConnectionString()
        {
            var connectionString = "~/db.sqlite".MapAbsolutePath();
            if (File.Exists(connectionString))
                File.Delete(connectionString);

            return connectionString;
        }

        /// <summary>Tests this object.</summary>
        public static void Test()
        {
            OrmLiteConfig.DialectProvider = SqliteOrmLiteDialectProvider.Instance;

            var path = GetFileConnectionString();
            if (File.Exists(path))
                File.Delete(path);
            //using (IDbConnection db = ":memory:".OpenDbConnection())
            using (IDbConnection db = path.OpenDbConnection())
            {
                db.CreateTable<User>(true);
                db.CreateTable<UserData>(true);
                db.CreateTable<UserService>(true);

                db.Insert(new UserData { Id = 5, UserDataValue = "Value-5" });
                db.Insert(new UserData { Id = 6, UserDataValue = "Value-6" });

                db.Insert(new UserService { Id = 8, ServiceName = "Value-8" });
                db.Insert(new UserService { Id = 9, ServiceName = "Value-9" });

                db.Insert(new User { Id = 1, Name = "A", CreatedDate = DateTime.Now, UserDataId = 5, UserServiceId = 8 });
                db.Insert(new User { Id = 2, Name = "B", CreatedDate = DateTime.Now, UserDataId = 5, UserServiceId = 9 });
                db.Insert(new User { Id = 3, Name = "B", CreatedDate = DateTime.Now });


                var rowsB = db.Select<User>("Name = {0}", "B");
                var rowsB1 = db.Select<User>(user => user.Name == "B");

                var jn = new JoinSqlBuilder<UserEx, User>();
                jn = jn.Join<User, UserData>(x => x.UserDataId, x => x.Id, x => new { x.Name, x.Id }, x => new { x.UserDataValue })
                       .LeftJoin<User, UserService>(x => x.UserServiceId, x => x.Id, null, x => new { x.ServiceName })
                       .OrderByDescending<User>(x=>x.Name)
                       .OrderBy<User>(x=>x.Id)
                       .Select<User>(x=>x.Id)
                       .Where<User>(x=> x.Id == 0);

                var sql = jn.ToSql();
                var items = db.Query<UserEx>(sql);
                
                jn.Clear();
                jn = jn.Join<User, UserData>(x => x.UserDataId, x => x.Id)
                       .LeftJoin<User, UserService>(x => x.UserServiceId, x => x.Id)
                       .OrderByDescending<User>(x => x.Name)
                       .OrderBy<User>(x => x.Id)
                       .OrderByDescending<UserService>(x => x.ServiceName)
                       .Where<User>(x => x.Id > 0)
                       .Or<User>(x => x.Id < 10)
                       .And<User>(x => x.Name != "" || x.Name != null);

                var sql2 = jn.ToSql();
                var item = db.QuerySingle<UserEx>(sql2);

                jn.Clear();
                jn = new JoinSqlBuilder<UserEx, User>();
                jn = jn.Join<User, UserData>(x => x.UserDataId, x => x.Id)
                       .LeftJoin<User, UserService>(x => x.UserServiceId, x => x.Id)
                       .OrderByDescending<User>(x=>x.Name)
                       .OrderBy<User>(x=>x.Id)
                       .SelectAll<UserData>()
                       .Where<User>(x=> x.Id == 0);

                var sql3 = jn.ToSql();
                var items3 = db.Query<UserEx>(sql3);

                jn.Clear();
                jn = new JoinSqlBuilder<UserEx, User>();
                jn = jn.Join<User, UserData>(x => x.UserDataId, x => x.Id, x => new { x.Name, x.Id }, x => new { x.UserDataValue })
                       .LeftJoin<User, UserService>(x => x.UserServiceId, x => x.Id, null, x => new { x.ServiceName })
                       .OrderByDescending<User>(x=>x.Name)
                       .OrderBy<User>(x=>x.Id)
                       .SelectDistinct()
                       .SelectAll<UserData>()
                       .Where<User>(x=> x.Id == 0);

                var sql4 = jn.ToSql();
                var items4 = db.Query<UserEx>(sql4);

                jn.Clear();
                jn = new JoinSqlBuilder<UserEx, User>();
                jn = jn.Join<User, UserData>(x => x.UserDataId, x => x.Id)
                       .LeftJoin<User, UserService>(x => x.UserServiceId, x => x.Id)
                       .OrderByDescending<User>(x=>x.Name)
                       .OrderBy<User>(x=>x.Id)
                       .SelectCount<User>(x=>x.Id)
                       .Where<User>(x=> x.Id == 0);

                var sql5 = jn.ToSql();
                var items5 = db.GetScalar<long>(sql5);

                jn.Clear();
                jn = new JoinSqlBuilder<UserEx, User>();
                jn = jn.Join<User, UserData>(x => x.UserDataId, x => x.Id)
                       .LeftJoin<User, UserService>(x => x.UserServiceId, x => x.Id)
                       .OrderByDescending<User>(x => x.Name)
                       .OrderBy<User>(x=>x.Id)
                       .SelectMax<User>(x=>x.Id)
                       .Where<User>(x=> x.Id == 0);

                var sql6 = jn.ToSql();
                var items6 = db.GetScalar<long>(sql6);

                jn.Clear();
                jn = new JoinSqlBuilder<UserEx, User>();
                jn = jn.Join<User, UserData>(x => x.UserDataId, x => x.Id)
                       .LeftJoin<User, UserService>(x => x.UserServiceId, x => x.Id)
                       .OrderByDescending<User>(x => x.Name)
                       .OrderBy<User>(x=>x.Id)
                       .SelectMin<User>(x=>x.Id)
                       .Where<User>(x=> x.Id == 0);

                var sql7 = jn.ToSql();
                var items7 = db.GetScalar<long>(sql7);

                jn.Clear();
                jn = new JoinSqlBuilder<UserEx, User>();
                jn = jn.Join<User, UserData>(x => x.UserDataId, x => x.Id)
                       .LeftJoin<User, UserService>(x => x.UserServiceId, x => x.Id)
                       .OrderByDescending<User>(x => x.Name)
                       .OrderBy<User>(x=>x.Id)
                       .SelectAverage<User>(x=>x.Id)
                       .Where<User>(x=> x.Id == 0);

                var sql8 = jn.ToSql();
                var items8 = db.GetScalar<long>(sql8);

                jn.Clear();
                jn = new JoinSqlBuilder<UserEx, User>();
                jn = jn.Join<User, UserData>(x => x.UserDataId, x => x.Id)
                       .LeftJoin<User, UserService>(x => x.UserServiceId, x => x.Id)
                       .OrderByDescending<User>(x => x.Name)
                       .OrderBy<User>(x=>x.Id)
                       .SelectSum<User>(x=>x.Id)
                       .Where<User>(x=> x.Id == 0);

                var sql9 = jn.ToSql();
                var items9 = db.GetScalar<long>(sql9);

            }

            File.Delete(path);
        }
    }
}
