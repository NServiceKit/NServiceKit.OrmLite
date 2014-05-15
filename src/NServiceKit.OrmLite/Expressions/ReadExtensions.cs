using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace NServiceKit.OrmLite
{
    /// <summary>A read extensions.</summary>
	public static class ReadExtensions
	{
        /// <summary>Creates the expression.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>The new expression.</returns>
		public static SqlExpressionVisitor<T> CreateExpression<T>()
		{
			return OrmLiteConfig.DialectProvider.ExpressionVisitor<T>();            
		}

        /// <summary>An IDbCommand extension method that selects.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">    The dbCmd to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A List&lt;T&gt;</returns>
		public static List<T> Select<T>(this IDbCommand dbCmd, Expression<Func<T, bool>> predicate)
		{
			var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<T>();
			string sql = ev.Where(predicate).ToSelectStatement();
			using (var reader = dbCmd.ExecReader(sql))
			{
				return ConvertToList<T>(reader);
			}
		}

        /// <summary>An IDbCommand extension method that selects.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">     The dbCmd to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A List&lt;T&gt;</returns>
		public static List<T> Select<T>(this IDbCommand dbCmd, Func<SqlExpressionVisitor<T>, SqlExpressionVisitor<T>> expression)
		{
			var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<T>();
			string sql = expression(ev).ToSelectStatement();
			using (var reader = dbCmd.ExecReader(sql))
			{
				return ConvertToList<T>(reader);
			}
		}

        /// <summary>An IDbCommand extension method that selects.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">     The dbCmd to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A List&lt;T&gt;</returns>
		public static List<T> Select<T>(this IDbCommand dbCmd, SqlExpressionVisitor<T> expression)
		{
			string sql = expression.ToSelectStatement();
			using (var reader = dbCmd.ExecReader(sql))
			{
				return ConvertToList<T>(reader);
			}
		}

        /// <summary>An IDbCommand extension method that select parameter.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">    The dbCmd to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A List&lt;T&gt;</returns>
        public static List<T> SelectParam<T>(this IDbCommand dbCmd, Expression<Func<T, bool>> predicate)
        {
            var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<T>();
            ev.IsParameterized = true;
            string sql = ev.Where(predicate).ToSelectStatement();

            dbCmd.Parameters.Clear();
            List<IDataParameter> paramsToInsert = new List<IDataParameter>();
            foreach (var param in ev.Params)
            {
                var cmdParam = dbCmd.CreateParameter();
                cmdParam.ParameterName = param.Key;
                cmdParam.Value = param.Value ?? DBNull.Value;
                paramsToInsert.Add(cmdParam);
            }

            using (var reader = dbCmd.ExecReader(sql, paramsToInsert))
            {
                return ConvertToList<T>(reader);
            }
        }

        /// <summary>An IDbCommand extension method that firsts.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">    The dbCmd to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A T.</returns>
		public static T First<T>(this IDbCommand dbCmd, Expression<Func<T, bool>> predicate)
		{
			var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<T>();
			
			return First(dbCmd, ev.Where(predicate).Limit(1));
		}

        /// <summary>An IDbCommand extension method that firsts.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">     The dbCmd to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A T.</returns>
		public static T First<T>(this IDbCommand dbCmd,  SqlExpressionVisitor<T> expression)
		{
			var result = FirstOrDefault(dbCmd, expression);
			if (Equals(result, default(T)))
			{
				throw new ArgumentNullException(string.Format(
					"{0}: '{1}' does not exist", typeof(T).Name, expression.WhereExpression));
			}
			return result;
		}

        /// <summary>An IDbCommand extension method that first or default.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">    The dbCmd to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A T.</returns>
		public static T FirstOrDefault<T>(this IDbCommand dbCmd, Expression<Func<T, bool>> predicate)
		{
			var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<T>();
			
			return FirstOrDefault(dbCmd, ev.Where(predicate).Limit(1));
		}

        /// <summary>An IDbCommand extension method that first or default.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">     The dbCmd to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A T.</returns>
		public static T FirstOrDefault<T>(this IDbCommand dbCmd, SqlExpressionVisitor<T> expression)
		{
			string sql= expression.ToSelectStatement();
			using (var dbReader = dbCmd.ExecReader(sql))
			{
				return ConvertTo<T>(dbReader);
			}
		}

        /// <summary>
        /// e.g. dbCmd.GetScalar&lt;MyClass, DateTime&gt;(myClass => Sql.Max(myClass.Timestamp));
        /// </summary>
        /// <typeparam name="T">   Generic type parameter.</typeparam>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <param name="field">The field.</param>
        /// <returns>The scalar.</returns>
		public static TKey GetScalar<T, TKey>(this IDbCommand dbCmd, Expression<Func<T, TKey>> field)
		{
			var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<T>();
			ev.Select(field);
			var sql = ev.ToSelectStatement();
			return dbCmd.GetScalar<TKey>(sql);
		}

        /// <summary>
        /// dbCmd.GetScalar&lt;MyClass, DateTime&gt;(MyClass=>Sql.Max(myClass.Timestamp),
        ///     MyClass=> MyClass.SomeProp== someValue);
        /// </summary>
        /// <typeparam name="T">   Generic type parameter.</typeparam>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="dbCmd">    The dbCmd to act on.</param>
        /// <param name="field">    The field.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The scalar.</returns>
		public static TKey GetScalar<T,TKey>(this IDbCommand dbCmd,Expression<Func<T, TKey>> field,
		                                     Expression<Func<T, bool>> predicate)
		{
			var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<T>();
			ev.Select(field).Where(predicate);
			string sql = ev.ToSelectStatement();
			return dbCmd.GetScalar<TKey>(sql);
		}

        /// <summary>An IDbCommand extension method that counts.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">The dbCmd to act on.</param>
        /// <returns>A long.</returns>
        public static long Count<T>(this IDbCommand dbCmd)
        {
            SqlExpressionVisitor<T> expression = OrmLiteConfig.DialectProvider.ExpressionVisitor<T>();
            string sql = expression.ToCountStatement();
            return dbCmd.GetScalar<long>(sql);
        }

        /// <summary>An IDbCommand extension method that counts.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">     The dbCmd to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A long.</returns>
        public static long Count<T>(this IDbCommand dbCmd, SqlExpressionVisitor<T> expression)
        {
            string sql = expression.ToCountStatement();
            return dbCmd.GetScalar<long>(sql);
        }

        /// <summary>An IDbCommand extension method that counts.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd">    The dbCmd to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A long.</returns>
        public static long Count<T>(this IDbCommand dbCmd, Expression<Func<T, bool>> predicate)
        {
            var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<T>();
            ev.Where(predicate);
            string sql = ev.ToCountStatement();
            return dbCmd.GetScalar<long>(sql);
        }

        /// <summary>Convert to.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dataReader">The data reader.</param>
        /// <returns>to converted.</returns>
		private static T ConvertTo<T>(IDataReader dataReader)
        {
			var fieldDefs = ModelDefinition<T>.Definition.AllFieldDefinitionsArray;

			using (dataReader)
			{
				if (dataReader.Read())
				{
					var row = OrmLiteUtilExtensions.CreateInstance<T>();

				    var namingStrategy = OrmLiteConfig.DialectProvider.NamingStrategy;

					for (int i = 0; i<dataReader.FieldCount; i++)
					{
					    var fieldDef = fieldDefs.FirstOrDefault(
					        x =>
					        namingStrategy.GetColumnName(x.FieldName).ToUpper() ==
					        dataReader.GetName(i).ToUpper());

						if (fieldDef == null) continue;
						var value = dataReader.GetValue(i);
						fieldDef.SetValue(row, value);
					}
					
					return row;
				}
				return default(T);
			}
		}

        /// <summary>Converts a dataReader to a list.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dataReader">The data reader.</param>
        /// <returns>The given data converted to a list.</returns>
		private static List<T> ConvertToList<T>(IDataReader dataReader)
		{
			var fieldDefs = ModelDefinition<T>.Definition.AllFieldDefinitionsArray;
			var fieldDefCache = new Dictionary<int, FieldDefinition>();

			var to = new List<T>();
			using (dataReader)
			{
				while (dataReader.Read())
				{
                    var row = OrmLiteUtilExtensions.CreateInstance<T>();

                    var namingStrategy = OrmLiteConfig.DialectProvider.NamingStrategy;

					for (int i = 0; i<dataReader.FieldCount; i++)
					{
						FieldDefinition fieldDef;
						if (!fieldDefCache.TryGetValue(i, out fieldDef))
						{
						    fieldDef = fieldDefs.FirstOrDefault(
						        x =>
						        namingStrategy.GetColumnName(x.FieldName).ToUpper() ==
						        dataReader.GetName(i).ToUpper());
							fieldDefCache[i] = fieldDef;
						}

						if (fieldDef == null) continue;

						var value = dataReader.GetValue(i);
						fieldDef.SetValue(row, value);
					}
					to.Add(row);
				}
			}
			return to;
		}

        /*
        private static T PopulateWithSqlReader<T>( T objWithProperties, IDataReader dataReader, FieldDefinition[] fieldDefs)
        {
            foreach (var fieldDef in fieldDefs)
            {
                try{
                    // NOTE: this is a nasty ineffeciency here when we're calling this for multiple rows!
                    // we should only call GetOrdinal once per column per result set
                    // and one could only wish for a -1 return instead of an IndexOutOfRangeException...
                    // how to get -1 ? 
                    //If the index of the named field is not found, an IndexOutOfRangeException is thrown.
                    //http://msdn.microsoft.com/en-us/library/system.data.idatarecord.getordinal(v=vs.100).aspx
					
                    var index = dataReader.GetColumnIndex(fieldDef.FieldName);
                    var value = dataReader.GetValue(index);
                    fieldDef.SetValue(objWithProperties, value);
                }
                catch(IndexOutOfRangeException){
                    // just ignore not retrived fields
                }
            }
		
			
            return objWithProperties;
        }
        */
        // First FirstOrDefault  // Use LIMIT to retrive only one row ! someone did it

	}
}

