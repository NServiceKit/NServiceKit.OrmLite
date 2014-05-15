using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NServiceKit.OrmLite.Firebird.DbSchema
{
    /// <summary>A poco creator.</summary>
    /// <typeparam name="TTable">    Type of the table.</typeparam>
    /// <typeparam name="TColumn">   Type of the column.</typeparam>
    /// <typeparam name="TProcedure">Type of the procedure.</typeparam>
    /// <typeparam name="TParameter">Type of the parameter.</typeparam>
	public abstract class PocoCreator<TTable, TColumn, TProcedure, TParameter>
		where TTable : ITable, new()
		where TColumn : IColumn, new()
		where TProcedure : IProcedure, new()
		where TParameter : IParameter, new(){

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.Firebird.DbSchema.PocoCreator&lt;TTable,
        /// TColumn, TProcedure, TParameter&gt; class.
        /// </summary>
		public PocoCreator()
		{

			OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "src");
			Usings ="using System;\n" +
					"using System.ComponentModel.DataAnnotations;\n" +
					"using NServiceKit.Common;\n" +
					"using NServiceKit.DataAnnotations;\n" +
					"using NServiceKit.DesignPatterns.Model;\n";

			SpaceName = "Database.Records";
			MetadataClassName="Me";
			IdField = OrmLiteConfig.IdField;
		}

        /// <summary>Gets or sets a value indicating whether the generate metadata.</summary>
        /// <value>true if generate metadata, false if not.</value>
		public bool GenerateMetadata{get;set;}

        /// <summary>Gets or sets the name of the metadata class.</summary>
        /// <value>The name of the metadata class.</value>
		public string MetadataClassName{get; set;}

        /// <summary>Gets or sets the identifier field.</summary>
        /// <value>The identifier field.</value>
		public string IdField{ get; set;}

        /// <summary>Gets or sets the name of the space.</summary>
        /// <value>The name of the space.</value>
		public string SpaceName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the service name space.</summary>
        /// <value>The service name space.</value>
		public string ServiceNameSpace
		{
			get;
			set;
		}

        /// <summary>Gets or sets the usings.</summary>
        /// <value>The usings.</value>
		public string Usings
		{
			get;
			set;
		}

        /// <summary>Gets or sets the pathname of the output directory.</summary>
        /// <value>The pathname of the output directory.</value>
		public string OutputDirectory
		{
			get;
			set;
		}

        /// <summary>Gets or sets the schema.</summary>
        /// <value>The schema.</value>
		public ISchema<TTable, TColumn, TProcedure, TParameter> Schema
		{
			get;
			set;
		}

        /// <summary>Writes the class.</summary>
        /// <param name="table">The table.</param>
		public virtual void WriteClass(TTable table)
		{
			WriteClass(table, table.Name);
		}

        /// <summary>Writes the class.</summary>
        /// <param name="table">    The table.</param>
        /// <param name="className">Name of the class.</param>
		public virtual void WriteClass(TTable table, string className)
		{
			if(string.IsNullOrEmpty( ServiceNameSpace)) ServiceNameSpace="Interface";
			className = ToDotName(className);
			StringBuilder properties= new StringBuilder();
			StringBuilder meProperties= new StringBuilder();
			List<TColumn> columns = Schema.GetColumns(table.Name);

			bool hasIdField = columns.Count(r => ToDotName(r.Name) == IdField) == 1;
			string idType= string.Empty;

			foreach (var cl in columns)
			{
				properties.AppendFormat("\t\t[Alias(\"{0}\")]\n", cl.Name);
				if (!string.IsNullOrEmpty(cl.Sequence)) properties.AppendFormat("\t\t[Sequence(\"{0}\")]\n", cl.Sequence);
				if (cl.IsPrimaryKey) properties.Append("\t\t[PrimaryKey]\n");
				if (cl.AutoIncrement) properties.Append("\t\t[AutoIncrement]\n");
				if ( TypeToString(cl.NetType)=="System.String"){
					if (!cl.Nullable) properties.Append("\t\t[Required]\n");
					properties.AppendFormat("\t\t[StringLength({0})]\n",cl.Length);
				}
				if(cl.DbType.ToUpper()=="DECIMAL" || cl.DbType.ToUpper()=="NUMERIC")
					properties.AppendFormat("\t\t[DecimalLength({0},{1})]\n",cl.Presicion, cl.Scale);
				if (cl.IsComputed) properties.Append("\t\t[Compute]\n");
					
				string propertyName;
				if(cl.AutoIncrement && cl.IsPrimaryKey && !hasIdField){
					propertyName= IdField;
					idType = TypeToString(cl.NetType);
					hasIdField=true;
				}
				else{
					propertyName= ToDotName(cl.Name);
					if(propertyName==IdField) idType= TypeToString(cl.NetType);
					else if(propertyName==className) propertyName= propertyName+"Name";
				}
				
				properties.AppendFormat("\t\tpublic {0}{1} {2} {{ get; set;}} \n\n",
										TypeToString(cl.NetType),
										(cl.Nullable && cl.NetType != typeof(string)) ? "?" : "",
										 propertyName);
				
				if(GenerateMetadata){
					if(meProperties.Length==0)
						meProperties.AppendFormat("\n\t\t\tpublic static string ClassName {{ get {{ return \"{0}\"; }}}}",
						                          className);
					meProperties.AppendFormat("\n\t\t\tpublic static string {0} {{ get {{ return \"{0}\"; }}}}",
					                         propertyName);
				}
				
			}
				    
			if (!Directory.Exists(OutputDirectory))
				Directory.CreateDirectory(OutputDirectory);
			
			string typesDir=Path.Combine(OutputDirectory,"Types");
			
			if(!Directory.Exists(typesDir))		
				Directory.CreateDirectory(typesDir);
						
			string attrDir=Path.Combine(OutputDirectory,"Attributes");
			if(!Directory.Exists(attrDir))		
				Directory.CreateDirectory(attrDir);
			
			string servDir=Path.Combine(OutputDirectory,"Services");
			if(!Directory.Exists(servDir))		
				Directory.CreateDirectory(servDir);
			
			using (TextWriter tw = new StreamWriter(Path.Combine(typesDir, className + ".cs")))
			{
				StringBuilder ns = new StringBuilder();
				StringBuilder cl =  new StringBuilder();
				StringBuilder me = new StringBuilder();
				cl.AppendFormat("\t[Alias(\"{0}\")]\n", table.Name);
				if(GenerateMetadata){
					me.AppendFormat("\n\t\tpublic static class {0} {{\n\t\t\t{1}\n\n\t\t}}\n",
					                MetadataClassName, meProperties.ToString());
					
				}
				cl.AppendFormat("\tpublic partial class {0}{1}{{\n\n\t\tpublic {0}(){{}}\n\n{2}{3}\t}}",
								className, 
				                hasIdField?string.Format( ":IHasId<{0}>",idType):"", 
				                properties.ToString(),
				                me.ToString());
			
				ns.AppendFormat("namespace {0}\n{{\n{1}\n}}", SpaceName, cl.ToString());
				tw.WriteLine(Usings);
				tw.WriteLine(ns.ToString());	
				
				tw.Close();
			}
			
			using (TextWriter twp = new StreamWriter(Path.Combine(attrDir, className + ".cs")))
			{
				twp.Write(string.Format(partialTemplate,SpaceName,className,IdField));				
				twp.Close();
			}
			
			using (TextWriter twp = new StreamWriter(Path.Combine(servDir, className + "Service.cs")))
			{
				twp.Write(string.Format(serviceTemplate,SpaceName, ServiceNameSpace, className));				
				twp.Close();
			}
			
		}

        /// <summary>Writes the class.</summary>
        /// <param name="procedure">The procedure.</param>
		public virtual void WriteClass(TProcedure procedure)
		{
			WriteClass(procedure, procedure.Name);
		}

        /// <summary>Writes the class.</summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="className">Name of the class.</param>
		public virtual  void WriteClass(TProcedure procedure, string className)
		{

		}

        /// <summary>Converts a name to a dot name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>name as a string.</returns>
		protected string ToDotName(string name)
		{

			StringBuilder t = new StringBuilder();
			string [] parts = name.Split('_');
			foreach (var s in parts)
			{
				t.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower()));
			}
			return t.ToString();
		}

        /// <summary>Type to string.</summary>
        /// <param name="type">The type.</param>
        /// <returns>A string.</returns>
		protected string TypeToString(Type type)
		{
			string st = type.ToString();
			return (!st.Contains("[")) ? st : st.Substring(st.IndexOf("[") + 1, st.IndexOf("]") - st.IndexOf("[") - 1);
		}

        /// <summary>The partial template.</summary>
		private string partialTemplate=@"using System;
using NServiceKit.ServiceHost;

namespace {0}
{{
	[RestService(""/{1}/create"",""post"")]
	[RestService(""/{1}/read"",""get"")]
	[RestService(""/{1}/read/{{{2}}}"",""get"")]
	[RestService(""/{1}/update/{{{2}}}"",""put"")]
	[RestService(""/{1}/destroy/{{{2}}}"",""delete"")]
	public partial class {1}
	{{
	}}
}}";

        /// <summary>The service template.</summary>
		private string serviceTemplate =@"using System;
﻿using NServiceKit.CacheAccess;
using NServiceKit.Common;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;

using {0};

namespace {1}
{{
	[Authenticate]
	[RequiredPermission(""{2}.read"")]
	[RequiredPermission(ApplyTo.Post, ""{2}.create"")]	
	[RequiredPermission(ApplyTo.Put , ""{2}.update"")]	
	[RequiredPermission(ApplyTo.Delete, ""{2}.destroy"")]
	public class {2}Service:AppRestService<{2}>
	{{
		
	}}
}}";
		

	}
}