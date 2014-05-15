using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading;
using NServiceKit.Text;

namespace NServiceKit.OrmLite
{
#if !NO_EXPRESSIONS
    /// <summary>
    /// Nice SqlBuilder class by @samsaffron from Dapper.Contrib:
    /// http://samsaffron.com/archive/2011/09/05/Digging+ourselves+out+of+the+mess+Linq-2-SQL+created
    /// Modified to work in .NET 3.5.
    /// </summary>
	public class SqlBuilder
	{
        /// <summary>The data.</summary>
		readonly Dictionary<string, Clauses> data = new Dictionary<string, Clauses>();

        /// <summary>The sequence.</summary>
		int seq;

        /// <summary>A clause.</summary>
		class Clause
		{
            /// <summary>Gets or sets the SQL.</summary>
            /// <value>The SQL.</value>
			public string Sql { get; set; }

            /// <summary>Gets or sets options for controlling the operation.</summary>
            /// <value>The parameters.</value>
			public object Parameters { get; set; }
		}

        /// <summary>A dynamic parameters.</summary>
		class DynamicParameters
		{
            /// <summary>A property.</summary>
			class Property
			{
                /// <summary>
                /// Initializes a new instance of the NServiceKit.OrmLite.SqlBuilder.DynamicParameters.Property
                /// class.
                /// </summary>
                /// <param name="name"> The name.</param>
                /// <param name="type"> The type.</param>
                /// <param name="value">The value.</param>
				public Property(string name, Type type, object value)
				{
					Name = name;
					Type = type;
					Value = value;
				}

                /// <summary>The name.</summary>
				public readonly string Name;

                /// <summary>The type.</summary>
				public readonly Type Type;

                /// <summary>The value.</summary>
				public readonly object Value;
			}

            /// <summary>The properties.</summary>
			private readonly List<Property> properties = new List<Property>();

            /// <summary>
            /// Initializes a new instance of the NServiceKit.OrmLite.SqlBuilder.DynamicParameters class.
            /// </summary>
            /// <param name="initParams">Options for controlling the initialise.</param>
			public DynamicParameters(object initParams)
			{
				AddDynamicParams(initParams);
			}

            /// <summary>Adds a dynamic parameters.</summary>
            /// <param name="cmdParams">Options for controlling the command.</param>
			public void AddDynamicParams(object cmdParams)
			{
				if (cmdParams == null) return;
				foreach (var pi in cmdParams.GetType().GetPublicProperties())
				{
					var getterFn = pi.GetPropertyGetterFn();
					if (getterFn == null) continue;
					var value = getterFn(cmdParams);
					properties.Add(new Property(pi.Name, pi.PropertyType, value));
				}
			}

            /// <summary>The property set and get methods require a special attrs:</summary>
			private const MethodAttributes GetSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            /// <summary>Creates dynamic type.</summary>
            /// <returns>The new dynamic type.</returns>
			public object CreateDynamicType()
			{
				var assemblyName = new AssemblyName { Name = "tmpAssembly" };
				var typeBuilder = 
                    Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
					.DefineDynamicModule("tmpModule")
					.DefineType("SqlBuilderDynamicParameters", TypeAttributes.Public | TypeAttributes.Class);

				var emptyCtor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
				var ctorIL = emptyCtor.GetILGenerator();

				var unsetValues = new List<Property>();

				// Loop over the attributes that will be used as the properties names in out new type
				foreach (var p in properties)
				{
					// Generate a private field
					var field = typeBuilder.DefineField("_" + p.Name, p.Type, FieldAttributes.Private);

					//set default values with Emit for popular types
					if (p.Type == typeof(int))
					{
						ctorIL.Emit(OpCodes.Ldarg_0);
						ctorIL.Emit(OpCodes.Ldc_I4, (int)p.Value);
						ctorIL.Emit(OpCodes.Stfld, field);
					}
					else if (p.Type == typeof(long))
					{
						ctorIL.Emit(OpCodes.Ldarg_0);
						ctorIL.Emit(OpCodes.Ldc_I8, (long)p.Value);
						ctorIL.Emit(OpCodes.Stfld, field);
					}
					else if (p.Type == typeof(string))
					{
						ctorIL.Emit(OpCodes.Ldarg_0);
						ctorIL.Emit(OpCodes.Ldstr, (string)p.Value);
						ctorIL.Emit(OpCodes.Stfld, field);
					}
					else
					{
						unsetValues.Add(p); //otherwise use reflection
					}

					// Generate a public property
					var property = typeBuilder.DefineProperty(p.Name, PropertyAttributes.None, p.Type, new[] { p.Type });

					// Define the "get" accessor method for current private field.
					var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + p.Name, GetSetAttr, p.Type, Type.EmptyTypes);

					// Get Property impl
					var currGetIL = currGetPropMthdBldr.GetILGenerator();
					currGetIL.Emit(OpCodes.Ldarg_0);
					currGetIL.Emit(OpCodes.Ldfld, field);
					currGetIL.Emit(OpCodes.Ret);

					// Define the "set" accessor method for current private field.
					var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + p.Name, GetSetAttr, null, new[] { p.Type });

					// Set Property impl
					var currSetIL = currSetPropMthdBldr.GetILGenerator();
					currSetIL.Emit(OpCodes.Ldarg_0);
					currSetIL.Emit(OpCodes.Ldarg_1);
					currSetIL.Emit(OpCodes.Stfld, field);
					currSetIL.Emit(OpCodes.Ret);

					// Hook up, getters and setters.
					property.SetGetMethod(currGetPropMthdBldr);
					property.SetSetMethod(currSetPropMthdBldr);
				}

				ctorIL.Emit(OpCodes.Ret);

				var generetedType = typeBuilder.CreateType();
				var instance = Activator.CreateInstance(generetedType);

				//Using reflection for less property types. Not caching since it's a generated type.
				foreach (var p in unsetValues)
				{
					generetedType.GetProperty(p.Name).GetSetMethod().Invoke(instance, new[] { p.Value });
				}

				return instance;
			}
		}

        /// <summary>A clauses.</summary>
		class Clauses : List<Clause>
		{
            /// <summary>The joiner.</summary>
			readonly string joiner;

            /// <summary>The prefix.</summary>
			readonly string prefix;

            /// <summary>The postfix.</summary>
			readonly string postfix;

            /// <summary>
            /// Initializes a new instance of the NServiceKit.OrmLite.SqlBuilder.Clauses class.
            /// </summary>
            /// <param name="joiner"> The joiner.</param>
            /// <param name="prefix"> The prefix.</param>
            /// <param name="postfix">The postfix.</param>
			public Clauses(string joiner, string prefix = "", string postfix = "")
			{
				this.joiner = joiner;
				this.prefix = prefix;
				this.postfix = postfix;
			}

            /// <summary>Resolve clauses.</summary>
            /// <param name="p">The DynamicParameters to process.</param>
            /// <returns>A string.</returns>
			public string ResolveClauses(DynamicParameters p)
			{
				foreach (var item in this)
				{
					p.AddDynamicParams(item.Parameters);
				}
				return prefix + string.Join(joiner, this.Select(c => c.Sql).ToArray()) + postfix;
			}
		}

        /// <summary>A template.</summary>
		public class Template
		{
            /// <summary>The SQL.</summary>
			readonly string sql;

            /// <summary>The builder.</summary>
			readonly SqlBuilder builder;

            /// <summary>Options for controlling the initialise.</summary>
			readonly object initParams;

            /// <summary>Unresolved.</summary>
			int dataSeq = -1;

            /// <summary>
            /// Initializes a new instance of the NServiceKit.OrmLite.SqlBuilder.Template class.
            /// </summary>
            /// <param name="builder">   The builder.</param>
            /// <param name="sql">       The SQL.</param>
            /// <param name="parameters">Options for controlling the operation.</param>
			public Template(SqlBuilder builder, string sql, object parameters)
			{
				this.initParams = parameters;
				this.sql = sql;
				this.builder = builder;
			}

            /// <summary>The regular expression.</summary>
			static readonly Regex regex = new Regex(@"\/\*\*.+\*\*\/", RegexOptions.Compiled | RegexOptions.Multiline);

            /// <summary>Resolve SQL.</summary>
			void ResolveSql()
			{
				if (dataSeq != builder.seq)
				{
					var p = new DynamicParameters(initParams);

					rawSql = sql;

					foreach (var pair in builder.data)
					{
						rawSql = rawSql.Replace("/**" + pair.Key + "**/", pair.Value.ResolveClauses(p));
					}
					parameters = p.CreateDynamicType();

					// replace all that is left with empty
					rawSql = regex.Replace(rawSql, "");

					dataSeq = builder.seq;
				}
			}

            /// <summary>The raw SQL.</summary>
			string rawSql;

            /// <summary>Options for controlling the operation.</summary>
			object parameters;

            /// <summary>Gets the raw SQL.</summary>
            /// <value>The raw SQL.</value>
			public string RawSql { get { ResolveSql(); return rawSql; } }

            /// <summary>Gets options for controlling the operation.</summary>
            /// <value>The parameters.</value>
			public object Parameters { get { ResolveSql(); return parameters; } }
		}

        /// <summary>Adds a template to 'parameters'.</summary>
        /// <param name="sql">       The SQL.</param>
        /// <param name="parameters">Options for controlling the operation.</param>
        /// <returns>A Template.</returns>
		public Template AddTemplate(string sql, object parameters = null)
		{
			return new Template(this, sql, parameters);
		}

        /// <summary>Adds a clause.</summary>
        /// <param name="name">      The name.</param>
        /// <param name="sql">       The SQL.</param>
        /// <param name="parameters">Options for controlling the operation.</param>
        /// <param name="joiner">    The joiner.</param>
        /// <param name="prefix">    The prefix.</param>
        /// <param name="postfix">   The postfix.</param>
		void AddClause(string name, string sql, object parameters, string joiner, string prefix = "", string postfix = "")
		{
			Clauses clauses;
			if (!data.TryGetValue(name, out clauses))
			{
				clauses = new Clauses(joiner, prefix, postfix);
				data[name] = clauses;
			}
			clauses.Add(new Clause { Sql = sql, Parameters = parameters });
			seq++;
		}

        /// <summary>Left join.</summary>
        /// <param name="sql">       The SQL.</param>
        /// <param name="parameters">Options for controlling the operation.</param>
        /// <returns>A SqlBuilder.</returns>
		public SqlBuilder LeftJoin(string sql, object parameters = null)
		{
			AddClause("leftjoin", sql, parameters, joiner: "\nLEFT JOIN ", prefix: "\nLEFT JOIN ", postfix: "\n");
			return this;
		}

        /// <summary>Wheres.</summary>
        /// <param name="sql">       The SQL.</param>
        /// <param name="parameters">Options for controlling the operation.</param>
        /// <returns>A SqlBuilder.</returns>
		public SqlBuilder Where(string sql, object parameters = null)
		{
			AddClause("where", sql, parameters, " AND ", prefix: "WHERE ", postfix: "\n");
			return this;
		}

        /// <summary>Order by.</summary>
        /// <param name="sql">       The SQL.</param>
        /// <param name="parameters">Options for controlling the operation.</param>
        /// <returns>A SqlBuilder.</returns>
		public SqlBuilder OrderBy(string sql, object parameters = null)
		{
			AddClause("orderby", sql, parameters, " , ", prefix: "ORDER BY ", postfix: "\n");
			return this;
		}

        /// <summary>Selects.</summary>
        /// <param name="sql">       The SQL.</param>
        /// <param name="parameters">Options for controlling the operation.</param>
        /// <returns>A SqlBuilder.</returns>
		public SqlBuilder Select(string sql, object parameters = null)
		{
			AddClause("select", sql, parameters, " , ", prefix: "", postfix: "\n");
			return this;
		}

        /// <summary>Adds the parameters.</summary>
        /// <param name="parameters">Options for controlling the operation.</param>
        /// <returns>A SqlBuilder.</returns>
		public SqlBuilder AddParameters(object parameters)
		{
			AddClause("--parameters", "", parameters, "");
			return this;
		}

        /// <summary>Joins.</summary>
        /// <param name="sql">       The SQL.</param>
        /// <param name="parameters">Options for controlling the operation.</param>
        /// <returns>A SqlBuilder.</returns>
		public SqlBuilder Join(string sql, object parameters = null)
		{
			AddClause("join", sql, parameters, joiner: "\nJOIN ", prefix: "\nJOIN", postfix: "\n");
			return this;
		}
	}
#endif

}