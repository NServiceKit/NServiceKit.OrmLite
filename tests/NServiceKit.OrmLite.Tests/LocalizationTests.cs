using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite.Sqlite;
using NServiceKit.Text;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>A localization tests.</summary>
	[TestFixture]
	public class LocalizationTests
		: OrmLiteTestBase
	{
        /// <summary>The current culture.</summary>
		private readonly CultureInfo CurrentCulture = Thread.CurrentThread.CurrentCulture;

        /// <summary>The current user interface culture.</summary>
		private readonly CultureInfo CurrentUICulture = Thread.CurrentThread.CurrentUICulture;

        /// <summary>Tests fixture set up.</summary>
		[SetUp]
		public void TestFixtureSetUp()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi-VN");
		}

        /// <summary>Tests fixture tear down.</summary>
		[TearDown]
		public void TestFixtureTearDown()
		{
			Thread.CurrentThread.CurrentCulture = CurrentCulture;
			Thread.CurrentThread.CurrentUICulture = CurrentUICulture;
		}

        /// <summary>A point.</summary>
		public class Point
		{
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
			[AutoIncrement]
			public int Id { get; set; }

            /// <summary>Gets or sets the width.</summary>
            /// <value>The width.</value>
			public short Width { get; set; }

            /// <summary>Gets or sets the height.</summary>
            /// <value>The height.</value>
			public float Height { get; set; }

            /// <summary>Gets or sets the top.</summary>
            /// <value>The top.</value>
			public double Top { get; set; }

            /// <summary>Gets or sets the left.</summary>
            /// <value>The left.</value>
			public decimal Left { get; set; }
		}

        /// <summary>Can query using float in alernate culuture.</summary>
		[Test]
		public void Can_query_using_float_in_alernate_culuture()
		{
			using (var db = OpenDbConnection())
			{
				db.CreateTable<Point>(true);

				db.Insert(new Point { Width = 4, Height = 1.123f, Top = 3.456d, Left = 2.345m});

				var points = db.Select<Point>("Height={0}", 1.123f);

				Console.WriteLine(points.Dump());

				Assert.That(points[0].Width, Is.EqualTo(4));
				Assert.That(points[0].Height, Is.EqualTo(1.123f));
				Assert.That(points[0].Top, Is.EqualTo(3.456d));
				Assert.That(points[0].Left, Is.EqualTo(2.345m));
			}

		}

	}
}