using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using NUnit.Framework;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;

namespace NServiceKit.OrmLite.MySql.Tests
{
    /// <summary>The shippers example.</summary>
	[TestFixture]
	public class ShippersExample
	{
        /// <summary>
        /// Initializes static members of the NServiceKit.OrmLite.MySql.Tests.ShippersExample class.
        /// </summary>
		static ShippersExample()
		{
			OrmLiteConfig.DialectProvider = MySqlDialectProvider.Instance;
		}

        /// <summary>A shipper.</summary>
		[Alias("Shippers")]
		public class Shipper
			: IHasId<int>
		{
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
			[AutoIncrement]
			[Alias("ShipperID")]
			public int Id { get; set; }

            /// <summary>Gets or sets the name of the company.</summary>
            /// <value>The name of the company.</value>
			[Required]
			[Index(Unique = true)]
			[StringLength(40)]
			public string CompanyName { get; set; }

            /// <summary>Gets or sets the phone.</summary>
            /// <value>The phone.</value>
			[StringLength(24)]
			public string Phone { get; set; }

            /// <summary>Gets or sets the identifier of the shipper type.</summary>
            /// <value>The identifier of the shipper type.</value>
			[References(typeof(ShipperType))]
			public int ShipperTypeId { get; set; }
		}

        /// <summary>A shipper type.</summary>
		[Alias("ShipperTypes")]
		public class ShipperType
			: IHasId<int>
		{
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
			[AutoIncrement]
			[Alias("ShipperTypeID")]
			public int Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
			[Required]
			[Index(Unique = true)]
			[StringLength(40)]
			public string Name { get; set; }
		}

        /// <summary>A subset of shipper.</summary>
		public class SubsetOfShipper
		{
            /// <summary>Gets or sets the identifier of the shipper.</summary>
            /// <value>The identifier of the shipper.</value>
			public int ShipperId { get; set; }

            /// <summary>Gets or sets the name of the company.</summary>
            /// <value>The name of the company.</value>
			public string CompanyName { get; set; }
		}

        /// <summary>A shipper type count.</summary>
		public class ShipperTypeCount
		{
            /// <summary>Gets or sets the identifier of the shipper type.</summary>
            /// <value>The identifier of the shipper type.</value>
			public int ShipperTypeId { get; set; }

            /// <summary>Gets or sets the number of. </summary>
            /// <value>The total.</value>
			public int Total { get; set; }
		}

        /// <summary>Shippers use case.</summary>
		[Test]
		public void Shippers_UseCase()
		{
			using (IDbConnection db = ConfigurationManager.ConnectionStrings["testDb"].ConnectionString.OpenDbConnection())
			{
				const bool overwrite = true;
			    db.CreateTable<ShipperType>(overwrite);
                db.CreateTable<Shipper>(overwrite);

				int trainsTypeId, planesTypeId;

				//Playing with transactions
				using (IDbTransaction dbTrans = db.BeginTransaction())
				{
					db.Insert(new ShipperType { Name = "Trains" });
					trainsTypeId = (int)db.GetLastInsertId();

					db.Insert(new ShipperType { Name = "Planes" });
					planesTypeId = (int)db.GetLastInsertId();

					dbTrans.Commit();
				}
				using (IDbTransaction dbTrans = db.BeginTransaction(IsolationLevel.ReadCommitted))
				{
					db.Insert(new ShipperType { Name = "Automobiles" });
					Assert.That(db.Select<ShipperType>(), Has.Count.EqualTo(3));

					dbTrans.Rollback();
				}
				Assert.That(db.Select<ShipperType>(), Has.Count.EqualTo(2));


				//Performing standard Insert's and Selects
				db.Insert(new Shipper { CompanyName = "Trains R Us", Phone = "555-TRAINS", ShipperTypeId = trainsTypeId });
				db.Insert(new Shipper { CompanyName = "Planes R Us", Phone = "555-PLANES", ShipperTypeId = planesTypeId });
				db.Insert(new Shipper { CompanyName = "We do everything!", Phone = "555-UNICORNS", ShipperTypeId = planesTypeId });

				var trainsAreUs = db.First<Shipper>("ShipperTypeId = {0}", trainsTypeId);
				Assert.That(trainsAreUs.CompanyName, Is.EqualTo("Trains R Us"));
				Assert.That(db.Select<Shipper>("CompanyName = {0} OR Phone = {1}", "Trains R Us", "555-UNICORNS"), Has.Count.EqualTo(2));
				Assert.That(db.Select<Shipper>("ShipperTypeId = {0}", planesTypeId), Has.Count.EqualTo(2));

				//Lets update a record
				trainsAreUs.Phone = "666-TRAINS";
				db.Update(trainsAreUs);
				Assert.That(db.GetById<Shipper>(trainsAreUs.Id).Phone, Is.EqualTo("666-TRAINS"));
				
				//Then make it dissappear
				db.Delete(trainsAreUs);
				Assert.That(db.GetByIdOrDefault<Shipper>(trainsAreUs.Id), Is.Null);

				//And bring it back again
				db.Insert(trainsAreUs);


				//Performing custom queries
				//Select only a subset from the table
				var partialColumns = db.Select<SubsetOfShipper>(typeof (Shipper), "ShipperTypeId = {0}", planesTypeId);
				Assert.That(partialColumns, Has.Count.EqualTo(2));

				//Select into another POCO class that matches sql
				var rows = db.Select<ShipperTypeCount>(
					"SELECT ShipperTypeId, COUNT(*) AS Total FROM Shippers GROUP BY ShipperTypeId ORDER BY COUNT(*)");

				Assert.That(rows, Has.Count.EqualTo(2));
				Assert.That(rows[0].ShipperTypeId, Is.EqualTo(trainsTypeId));
				Assert.That(rows[0].Total, Is.EqualTo(1));
				Assert.That(rows[1].ShipperTypeId, Is.EqualTo(planesTypeId));
				Assert.That(rows[1].Total, Is.EqualTo(2));


				//And finally lets quickly clean up the mess we've made:
				db.DeleteAll<Shipper>();
				db.DeleteAll<ShipperType>();

				Assert.That(db.Select<Shipper>(), Has.Count.EqualTo(0));
				Assert.That(db.Select<ShipperType>(), Has.Count.EqualTo(0));
			}
		}
	}
}