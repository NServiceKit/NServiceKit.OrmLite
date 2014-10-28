using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using NServiceKit.Text;

namespace NServiceKit.OrmLite
{
#if !NO_EXPRESSIONS
    /// <summary>
    /// Nice SqlBuilder class by @samsaffron from Dapper.Contrib:
    /// http://samsaffron.com/archive/2011/09/05/Digging+ourselves+out+of+the+mess+Linq-2-SQL+created
    /// Modified to work in .NET 3.5.
    /// Modifications taken from former Ormlite.SqlBuilder
    /// </summary>
    public class SqlBuilder
    {
        readonly Dictionary<string, Clauses> data = new Dictionary<string, Clauses>();
        int seq;

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

        class Clause
        {
            public string Sql { get; set; }
            public object Parameters { get; set; }
            public bool IsInclusive { get; set; }
        }

        class Clauses : List<Clause>
        {
            string joiner;
            string prefix;
            string postfix;

            public Clauses(string joiner, string prefix = "", string postfix = "")
            {
                this.joiner = joiner;
                this.prefix = prefix;
                this.postfix = postfix;
            }

            public string ResolveClauses(DynamicParameters p)
            {
                foreach (var item in this)
                {
                    p.AddDynamicParams(item.Parameters);
                }
                return this.Any(a => a.IsInclusive)
                    ? prefix +
                      string.Join(joiner,
                          this.Where(a => !a.IsInclusive)
                              .Select(c => c.Sql)
                              .Union(new[]
                              {
                                  " ( " +
                                  string.Join(" OR ", this.Where(a => a.IsInclusive).Select(c => c.Sql).ToArray()) +
                                  " ) "
                              }).ToArray()) + postfix
                    : prefix + string.Join(joiner, this.Select(c => c.Sql).ToArray()) + postfix;
            }
        }

        public class Template
        {
            readonly string sql;
            readonly SqlBuilder builder;
            readonly object initParams;
            int dataSeq = -1; // Unresolved

            public Template(SqlBuilder builder, string sql, object parameters)
            {
                this.initParams = parameters;
                this.sql = sql;
                this.builder = builder;
            }

            static System.Text.RegularExpressions.Regex regex =
                new System.Text.RegularExpressions.Regex(@"\/\*\*.+\*\*\/", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Multiline);

            void ResolveSql()
            {
                if (dataSeq != builder.seq)
                {
                    DynamicParameters p = new DynamicParameters(initParams);

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

            string rawSql;
            object parameters;

            public string RawSql { get { ResolveSql(); return rawSql; } }
            public object Parameters { get { ResolveSql(); return parameters; } }
        }


        public SqlBuilder()
        {
        }

        public Template AddTemplate(string sql, object parameters = null)
        {
            return new Template(this, sql, parameters);
        }

        void AddClause(string name, string sql, object parameters, string joiner, string prefix = "", string postfix = "", bool IsInclusive = false)
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

        public SqlBuilder Intersect(string sql, object parameters = null)
        {
            AddClause("intersect", sql, parameters, joiner: "\nINTERSECT\n ", prefix: "\n ", postfix: "\n");
            return this;
        }

        public SqlBuilder InnerJoin(string sql, object parameters = null)
        {
            AddClause("innerjoin", sql, parameters, joiner: "\nINNER JOIN ", prefix: "\nINNER JOIN ", postfix: "\n");
            return this;
        }

        public SqlBuilder LeftJoin(string sql, object parameters = null)
        {
            AddClause("leftjoin", sql, parameters, joiner: "\nLEFT JOIN ", prefix: "\nLEFT JOIN ", postfix: "\n");
            return this;
        }

        public SqlBuilder RightJoin(string sql, object parameters = null)
        {
            AddClause("rightjoin", sql, parameters, joiner: "\nRIGHT JOIN ", prefix: "\nRIGHT JOIN ", postfix: "\n");
            return this;
        }

        public SqlBuilder Where(string sql, object parameters = null)
        {
            AddClause("where", sql, parameters, " AND ", prefix: "WHERE ", postfix: "\n");
            return this;
        }

        public SqlBuilder OrWhere(string sql, object parameters = null)
        {
            AddClause("where", sql, parameters, " AND ", prefix: "WHERE ", postfix: "\n", IsInclusive: true);
            return this;
        }

        public SqlBuilder OrderBy(string sql, object parameters = null)
        {
            AddClause("orderby", sql, parameters, " , ", prefix: "ORDER BY ", postfix: "\n");
            return this;
        }

        public SqlBuilder Select(string sql, object parameters = null)
        {
            AddClause("select", sql, parameters, " , ", prefix: "", postfix: "\n");
            return this;
        }

        public SqlBuilder AddParameters(object parameters)
        {
            AddClause("--parameters", "", parameters, "");
            return this;
        }

        public SqlBuilder Join(string sql, object parameters = null)
        {
            AddClause("join", sql, parameters, joiner: "\nJOIN ", prefix: "\nJOIN ", postfix: "\n");
            return this;
        }

        public SqlBuilder GroupBy(string sql, object parameters = null)
        {
            AddClause("groupby", sql, parameters, joiner: " , ", prefix: "\nGROUP BY ", postfix: "\n");
            return this;
        }

        public SqlBuilder Having(string sql, object parameters = null)
        {
            AddClause("having", sql, parameters, joiner: "\nAND ", prefix: "HAVING ", postfix: "\n");
            return this;
        }
    }
#endif
}
