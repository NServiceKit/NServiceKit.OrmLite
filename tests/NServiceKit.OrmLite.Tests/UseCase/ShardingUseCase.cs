using System;
using System.Data;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Common.Utils;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.Tests.UseCase
{
    /// <summary>A sharding use case.</summary>
    [Ignore("Robots Shard Use Case")]
    [TestFixture]
    public class ShardingUseCase
    {
        /// <summary>Information about the master.</summary>
        public class MasterRecord
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public Guid Id { get; set; }

            /// <summary>Gets or sets the identifier of the robot.</summary>
            /// <value>The identifier of the robot.</value>
            public int RobotId { get; set; }

            /// <summary>Gets or sets the name of the robot.</summary>
            /// <value>The name of the robot.</value>
            public string RobotName { get; set; }

            /// <summary>Gets or sets the Date/Time of the last activated.</summary>
            /// <value>The last activated.</value>
            public DateTime? LastActivated { get; set; }
        }

        /// <summary>A robot.</summary>
        public class Robot
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>Gets or sets a value indicating whether this object is activated.</summary>
            /// <value>true if this object is activated, false if not.</value>
            public bool IsActivated { get; set; }

            /// <summary>Gets or sets the number of cells.</summary>
            /// <value>The number of cells.</value>
            public long CellCount { get; set; }

            /// <summary>Gets or sets the created date.</summary>
            /// <value>The created date.</value>
            public DateTime CreatedDate { get; set; }
        }

        /// <summary>Shard 1000 robots over 10 shards.</summary>
        [Test]
        public void Shard_1000_Robots_over_10_shards()
        {
            const int NoOfShards = 10;
            const int NoOfRobots = 1000;

            var dbFactory = new OrmLiteConnectionFactory(
                "~/App_Data/robots-master.sqlite".MapAbsolutePath(), 
                false, SqliteDialect.Provider);
            
            //var dbFactory = new OrmLiteConnectionFactory(
            //    "Data Source=localhost;Initial Catalog=RobotsMaster;Integrated Security=SSPI", 
            //    SqlServerDialect.Provider);
            
            //Create Master Table in Master DB
            dbFactory.Run(db => db.CreateTable<MasterRecord>(overwrite: false)); 
            NoOfShards.Times(i => {
                var shardId = "robots-shard" + i;
                dbFactory.RegisterConnection(shardId, "~/App_Data/{0}.sqlite".Fmt(shardId).MapAbsolutePath(), SqliteDialect.Provider);

                //Create Robot table in Shard
                dbFactory.OpenDbConnection(shardId).Run(db => db.CreateTable<Robot>(overwrite: false)); 
            });

            var newRobots = NoOfRobots.Times(i => //Create 1000 Robots
                new Robot { Id = i, Name = "R2D" + i, CreatedDate = DateTime.UtcNow, CellCount = DateTime.UtcNow.ToUnixTimeMs() % 100000 });

            foreach (var newRobot in newRobots)
            {
                using (IDbConnection db = dbFactory.OpenDbConnection()) //Open Connection to Master DB
                {
                    db.Insert(new MasterRecord { Id = Guid.NewGuid(), RobotId = newRobot.Id, RobotName = newRobot.Name });
                    using (IDbConnection robotShard = dbFactory.OpenDbConnection("robots-shard" + newRobot.Id % NoOfShards)) //Shard DB
                    {
                        robotShard.Insert(newRobot);
                    }
                }
            }

        }
    }
}