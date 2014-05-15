using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace NServiceKit.OrmLite
{
    /// <summary>A SQL in values.</summary>
	public class SqlInValues
	{
        /// <summary>The values.</summary>
		private readonly IEnumerable values;

        /// <summary>Gets the number of. </summary>
        /// <value>The count.</value>
        public int Count { get; private set; }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.SqlInValues class.
        /// </summary>
        /// <param name="values">The values.</param>
		public SqlInValues(IEnumerable values)
		{
			this.values = values;

            if(values != null)

            foreach (var value in values)
                ++Count;
		}

        /// <summary>Converts this object to a SQL in string.</summary>
        /// <returns>This object as a string.</returns>
		public string ToSqlInString()
		{
            if(Count == 0)
                return "NULL";

			return OrmLiteUtilExtensions.SqlJoin(values);
		}
	}
}