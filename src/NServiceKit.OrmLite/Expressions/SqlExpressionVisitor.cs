using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using NServiceKit.Text;

namespace NServiceKit.OrmLite
{
    /// <summary>A SQL expression visitor.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public abstract class SqlExpressionVisitor<T>
    {
        /// <summary>The underlying expression.</summary>
        private Expression<Func<T, bool>> underlyingExpression;

        /// <summary>The order by properties.</summary>
        private List<string> orderByProperties = new List<string>();

        /// <summary>The select expression.</summary>
        private string selectExpression = string.Empty;

        /// <summary>The where expression.</summary>
        private string whereExpression;

        /// <summary>Describes who group this object.</summary>
        private string groupBy = string.Empty;

        /// <summary>The having expression.</summary>
        private string havingExpression;

        /// <summary>Describes who order this object.</summary>
        private string orderBy = string.Empty;

        /// <summary>The update fields.</summary>
        IList<string> updateFields = new List<string>();

        /// <summary>The insert fields.</summary>
        IList<string> insertFields = new List<string>();

        /// <summary>The separator.</summary>
        private string sep = string.Empty;

        /// <summary>true to use field name.</summary>
        private bool useFieldName = false;

        /// <summary>The model definition.</summary>
        private ModelDefinition modelDef;

        /// <summary>
        /// Gets or sets a value indicating whether the prefix field with table name.
        /// </summary>
        /// <value>true if prefix field with table name, false if not.</value>
        public bool PrefixFieldWithTableName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the where statement without where string.
        /// </summary>
        /// <value>true if where statement without where string, false if not.</value>
        public bool WhereStatementWithoutWhereString { get; set; }

        /// <summary>Gets the separator.</summary>
        /// <value>The separator.</value>
        protected string Sep
        {
            get { return sep; }
        }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.SqlExpressionVisitor&lt;T&gt; class.
        /// </summary>
        public SqlExpressionVisitor()
        {
            modelDef = typeof(T).GetModelDefinition();
            PrefixFieldWithTableName = false;
            WhereStatementWithoutWhereString = false;
        }

        /// <summary>Clear select expression. All properties will be selected.</summary>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Select()
        {
            return Select(string.Empty);
        }

        /// <summary>set the specified selectExpression.</summary>
        /// <param name="selectExpression">raw Select expression: "Select SomeField1, SomeField2 from
        /// SomeTable".</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Select(string selectExpression)
        {

            if (string.IsNullOrEmpty(selectExpression))
            {
                BuildSelectExpression(string.Empty, false);
            }
            else
            {
                this.selectExpression = selectExpression;
            }
            return this;
        }

        /// <summary>Fields to be selected.</summary>
        /// <typeparam name="TKey">objectWithProperties.</typeparam>
        /// <param name="fields">x=> x.SomeProperty1 or x=> new{ x.SomeProperty1, x.SomeProperty2}</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Select<TKey>(Expression<Func<T, TKey>> fields)
        {
            sep = string.Empty;
            useFieldName = true;
            BuildSelectExpression(Visit(fields).ToString(), false);
            return this;
        }

        /// <summary>Select distinct.</summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="fields">x=> x.SomeProperty1 or x=> new{ x.SomeProperty1, x.SomeProperty2}</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> SelectDistinct<TKey>(Expression<Func<T, TKey>> fields)
        {
            sep = string.Empty;
            useFieldName = true;
            BuildSelectExpression(Visit(fields).ToString(), true);
            return this;
        }

        /// <summary>Wheres the given predicate.</summary>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Where()
        {
            if (underlyingExpression != null) underlyingExpression = null; //Where() clears the expression
            return Where(string.Empty);
        }

        /// <summary>Wheres the given predicate.</summary>
        /// <param name="sqlFilter">   A filter specifying the SQL.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Where(string sqlFilter, params object[] filterParams)
        {
            whereExpression = !string.IsNullOrEmpty(sqlFilter) ? sqlFilter.SqlFormat(filterParams) : string.Empty;
            if (!string.IsNullOrEmpty(whereExpression)) whereExpression = (WhereStatementWithoutWhereString ? "" : "WHERE ") + whereExpression;
            return this;
        }

        /// <summary>Wheres the given predicate.</summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                And(predicate);
            }
            else
            {
                underlyingExpression = null;
                whereExpression = string.Empty;
            }

            return this;
        }

        /// <summary>Ands the given predicate.</summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> And(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                if (underlyingExpression == null)
                    underlyingExpression = predicate;
                else
                    underlyingExpression = underlyingExpression.And(predicate);

                ProcessInternalExpression();
            }
            return this;
        }

        /// <summary>Ors the given predicate.</summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Or(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                if (underlyingExpression == null)
                    underlyingExpression = predicate;
                else
                    underlyingExpression = underlyingExpression.Or(predicate);

                ProcessInternalExpression();
            }
            return this;
        }

        /// <summary>Process the internal expression.</summary>
        private void ProcessInternalExpression()
        {
            useFieldName = true;
            sep = " ";
            whereExpression = Visit(underlyingExpression).ToString();
            if (!string.IsNullOrEmpty(whereExpression)) whereExpression = (WhereStatementWithoutWhereString ? "" : "WHERE ") + whereExpression;
        }

        /// <summary>Group by.</summary>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> GroupBy()
        {
            return GroupBy(string.Empty);
        }

        /// <summary>Group by.</summary>
        /// <param name="groupBy">Describes who group this object.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> GroupBy(string groupBy)
        {
            this.groupBy = groupBy;
            return this;
        }

        /// <summary>Group by.</summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> GroupBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            sep = string.Empty;
            useFieldName = true;
            groupBy = Visit(keySelector).ToString();
            if (!string.IsNullOrEmpty(groupBy)) groupBy = string.Format("GROUP BY {0}", groupBy);
            return this;
        }

        /// <summary>Havings the given predicate.</summary>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Having()
        {
            return Having(string.Empty);
        }

        /// <summary>Havings the given predicate.</summary>
        /// <param name="sqlFilter">   A filter specifying the SQL.</param>
        /// <param name="filterParams">Options for controlling the filter.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Having(string sqlFilter, params object[] filterParams)
        {
            havingExpression = !string.IsNullOrEmpty(sqlFilter) ? sqlFilter.SqlFormat(filterParams) : string.Empty;
            if (!string.IsNullOrEmpty(havingExpression)) havingExpression = "HAVING " + havingExpression;
            return this;
        }

        /// <summary>Havings the given predicate.</summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Having(Expression<Func<T, bool>> predicate)
        {

            if (predicate != null)
            {
                useFieldName = true;
                sep = " ";
                havingExpression = Visit(predicate).ToString();
                if (!string.IsNullOrEmpty(havingExpression)) havingExpression = "HAVING " + havingExpression;
            }
            else
                havingExpression = string.Empty;

            return this;
        }

        /// <summary>Order by.</summary>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> OrderBy()
        {
            return OrderBy(string.Empty);
        }

        /// <summary>Order by.</summary>
        /// <param name="orderBy">Describes who order this object.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> OrderBy(string orderBy)
        {
            orderByProperties.Clear();
            this.orderBy = orderBy;
            return this;
        }

        /// <summary>Order by.</summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            sep = string.Empty;
            useFieldName = true;
            orderByProperties.Clear();
            var property = Visit(keySelector).ToString();
            orderByProperties.Add(property + " ASC");
            BuildOrderByClauseInternal();
            return this;
        }

        /// <summary>Then by.</summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            sep = string.Empty;
            useFieldName = true;
            var property = Visit(keySelector).ToString();
            orderByProperties.Add(property + " ASC");
            BuildOrderByClauseInternal();
            return this;
        }

        /// <summary>Order by descending.</summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            sep = string.Empty;
            useFieldName = true;
            orderByProperties.Clear();
            var property = Visit(keySelector).ToString();
            orderByProperties.Add(property + " DESC");
            BuildOrderByClauseInternal();
            return this;
        }

        /// <summary>Then by descending.</summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            sep = string.Empty;
            useFieldName = true;
            var property = Visit(keySelector).ToString();
            orderByProperties.Add(property + " DESC");
            BuildOrderByClauseInternal();
            return this;
        }

        /// <summary>Builds order by clause internal.</summary>
        private void BuildOrderByClauseInternal()
        {
            if (orderByProperties.Count > 0)
            {
                orderBy = "ORDER BY ";
                foreach (var prop in orderByProperties)
                {
                    orderBy += prop + ",";
                }
                orderBy = orderBy.TrimEnd(',');
            }
            else
            {
                orderBy = null;
            }
        }

        /// <summary>Set the specified offset and rows for SQL Limit clause.</summary>
        /// <param name="skip">Offset of the first row to return. The offset of the initial row is 0.</param>
        /// <param name="rows">Number of rows returned by a SELECT statement.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Limit(int skip, int rows)
        {
            Rows = rows;
            Skip = skip;
            return this;
        }

        /// <summary>Set the specified rows for Sql Limit clause.</summary>
        /// <param name="rows">Number of rows returned by a SELECT statement.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Limit(int rows)
        {
            Rows = rows;
            Skip = 0;
            return this;
        }

        /// <summary>Clear Sql Limit clause.</summary>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Limit()
        {
            Skip = null;
            Rows = null;
            return this;
        }

        /// <summary>Fields to be updated.</summary>
        /// <param name="updateFields">The update fields.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        /// ### <param name="updatefields">IList&lt;string&gt; containing Names of properties to be
        /// updated.</param>
        public virtual SqlExpressionVisitor<T> Update(IList<string> updateFields)
        {
            this.updateFields = updateFields;
            return this;
        }

        /// <summary>Fields to be updated.</summary>
        /// <typeparam name="TKey">objectWithProperties.</typeparam>
        /// <param name="fields">x=> x.SomeProperty1 or x=> new{ x.SomeProperty1, x.SomeProperty2}</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Update<TKey>(Expression<Func<T, TKey>> fields)
        {
            sep = string.Empty;
            useFieldName = false;
            updateFields = Visit(fields).ToString().Split(',').ToList();
            return this;
        }

        /// <summary>Clear UpdateFields list ( all fields will be updated)</summary>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Update()
        {
            this.updateFields = new List<string>();
            return this;
        }

        /// <summary>Fields to be inserted.</summary>
        /// <typeparam name="TKey">objectWithProperties.</typeparam>
        /// <param name="fields">x=> x.SomeProperty1 or x=> new{ x.SomeProperty1, x.SomeProperty2}</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Insert<TKey>(Expression<Func<T, TKey>> fields)
        {
            sep = string.Empty;
            useFieldName = false;
            insertFields = Visit(fields).ToString().Split(',').ToList();
            return this;
        }

        /// <summary>fields to be inserted.</summary>
        /// <param name="insertFields">IList&lt;string&gt; containing Names of properties to be inserted.</param>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Insert(IList<string> insertFields)
        {
            this.insertFields = insertFields;
            return this;
        }

        /// <summary>Clear InsertFields list ( all fields will be inserted)</summary>
        /// <returns>A SqlExpressionVisitor&lt;T&gt;</returns>
        public virtual SqlExpressionVisitor<T> Insert()
        {
            this.insertFields = new List<string>();
            return this;
        }

        /// <summary>Converts this object to a delete row statement.</summary>
        /// <returns>This object as a string.</returns>
        public virtual string ToDeleteRowStatement()
        {
            return string.Format("DELETE FROM {0} {1}",
                                                   OrmLiteConfig.DialectProvider.GetQuotedTableName(modelDef),
                                                   WhereExpression);
        }

        /// <summary>Converts this object to an update statement.</summary>
        /// <param name="item">           The item.</param>
        /// <param name="excludeDefaults">true to exclude, false to include the defaults.</param>
        /// <returns>The given data converted to a string.</returns>
        public virtual string ToUpdateStatement(T item, bool excludeDefaults = false)
        {
            var setFields = new StringBuilder();
            var dialectProvider = OrmLiteConfig.DialectProvider;

            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (updateFields.Count > 0 && !updateFields.Contains(fieldDef.Name)) continue; // added
                var value = fieldDef.GetValue(item);
                if (excludeDefaults && (value == null || value.Equals(value.GetType().GetDefaultValue()))) continue; //GetDefaultValue?

                fieldDef.GetQuotedValue(item);

                if (setFields.Length > 0) setFields.Append(",");
                setFields.AppendFormat("{0} = {1}",
                    dialectProvider.GetQuotedColumnName(fieldDef.FieldName),
                    dialectProvider.GetQuotedValue(value, fieldDef.FieldType));
            }

            return string.Format("UPDATE {0} SET {1} {2}",
                                                dialectProvider.GetQuotedTableName(modelDef), setFields, WhereExpression);
        }

        /// <summary>Converts this object to a select statement.</summary>
        /// <returns>This object as a string.</returns>
        public virtual string ToSelectStatement()
        {
            var sql = new StringBuilder();

            sql.Append(SelectExpression);
            sql.Append(string.IsNullOrEmpty(WhereExpression) ?
                       "" :
                       "\n" + WhereExpression);
            sql.Append(string.IsNullOrEmpty(GroupByExpression) ?
                       "" :
                       "\n" + GroupByExpression);
            sql.Append(string.IsNullOrEmpty(HavingExpression) ?
                       "" :
                       "\n" + HavingExpression);
            sql.Append(string.IsNullOrEmpty(OrderByExpression) ?
                       "" :
                       "\n" + OrderByExpression);

            return ApplyPaging(sql.ToString());
        }

        /// <summary>Converts this object to a count statement.</summary>
        /// <returns>This object as a string.</returns>
        public virtual string ToCountStatement()
        {
            return OrmLiteConfig.DialectProvider.ToCountStatement(modelDef.ModelType, WhereExpression, null);
        }

        /// <summary>Gets or sets the select expression.</summary>
        /// <value>The select expression.</value>
        public string SelectExpression
        {
            get
            {
                if (string.IsNullOrEmpty(selectExpression))
                    BuildSelectExpression(string.Empty, false);
                return selectExpression;
            }
            set
            {
                selectExpression = value;
            }
        }

        /// <summary>Gets or sets the where expression.</summary>
        /// <value>The where expression.</value>
        public string WhereExpression
        {
            get
            {
                return whereExpression;
            }
            set
            {
                whereExpression = value;
            }
        }

        /// <summary>Gets or sets the group by expression.</summary>
        /// <value>The group by expression.</value>
        public string GroupByExpression
        {
            get
            {
                return groupBy;
            }
            set
            {
                groupBy = value;
            }
        }

        /// <summary>Gets or sets the having expression.</summary>
        /// <value>The having expression.</value>
        public string HavingExpression
        {
            get
            {
                return havingExpression;
            }
            set
            {
                havingExpression = value;
            }
        }

        /// <summary>Gets or sets the order by expression.</summary>
        /// <value>The order by expression.</value>
        public string OrderByExpression
        {
            get
            {
                return orderBy;
            }
            set
            {
                orderBy = value;
            }
        }

        /// <summary>Gets the limit expression.</summary>
        /// <value>The limit expression.</value>
        public virtual string LimitExpression
        {
            get
            {
                if (!Skip.HasValue) return "";
                string rows;
                if (Rows.HasValue)
                {
                    rows = string.Format(",{0}", Rows.Value);
                }
                else
                {
                    rows = string.Empty;
                }
                return string.Format("LIMIT {0}{1}", Skip.Value, rows);
            }
        }

        /// <summary>Gets or sets the rows.</summary>
        /// <value>The rows.</value>
        public int? Rows { get; set; }

        /// <summary>Gets or sets the skip.</summary>
        /// <value>The skip.</value>
        public int? Skip { get; set; }

        /// <summary>Gets or sets the update fields.</summary>
        /// <value>The update fields.</value>
        public IList<string> UpdateFields
        {
            get
            {
                return updateFields;
            }
            set
            {
                updateFields = value;
            }
        }

        /// <summary>Gets or sets the insert fields.</summary>
        /// <value>The insert fields.</value>
        public IList<string> InsertFields
        {
            get
            {
                return insertFields;
            }
            set
            {
                insertFields = value;
            }
        }

        /// <summary>Gets or sets the model definition.</summary>
        /// <value>The model definition.</value>
        protected internal ModelDefinition ModelDef
        {
            get
            {
                return modelDef;
            }
            set
            {
                modelDef = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether this object use field name.</summary>
        /// <value>true if use field name, false if not.</value>
        protected internal bool UseFieldName
        {
            get
            {
                return useFieldName;
            }
            set
            {
                useFieldName = value;
            }
        }

        /// <summary>Visits the given exponent.</summary>
        /// <param name="exp">The exponent.</param>
        /// <returns>An object.</returns>
        protected internal virtual object Visit(Expression exp)
        {

            if (exp == null) return string.Empty;
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return VisitLambda(exp as LambdaExpression);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess(exp as MemberExpression);
                case ExpressionType.Constant:
                    return VisitConstant(exp as ConstantExpression);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    //return "(" + VisitBinary(exp as BinaryExpression) + ")";
                    return VisitBinary(exp as BinaryExpression);
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary(exp as UnaryExpression);
                case ExpressionType.Parameter:
                    return VisitParameter(exp as ParameterExpression);
                case ExpressionType.Call:
                    return VisitMethodCall(exp as MethodCallExpression);
                case ExpressionType.New:
                    return VisitNew(exp as NewExpression);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray(exp as NewArrayExpression);
                case ExpressionType.MemberInit:
                    return VisitMemberInit(exp as MemberInitExpression);
                default:
                    return exp.ToString();
            }
        }

        /// <summary>Visit lambda.</summary>
        /// <param name="lambda">The lambda.</param>
        /// <returns>An object.</returns>
        protected virtual object VisitLambda(LambdaExpression lambda)
        {
            if (lambda.Body.NodeType == ExpressionType.MemberAccess && sep == " ")
            {
                MemberExpression m = lambda.Body as MemberExpression;

                if (m.Expression != null)
                {
                    string r = VisitMemberAccess(m).ToString();
                    return string.Format("{0}={1}", r, GetQuotedTrueValue());
                }

            }
            return Visit(lambda.Body);
        }

        /// <summary>Visit binary.</summary>
        /// <param name="b">The BinaryExpression to process.</param>
        /// <returns>An object.</returns>
        protected virtual object VisitBinary(BinaryExpression b)
        {
            object left, right;
            var operand = BindOperant(b.NodeType);   //sep= " " ??
            if (operand == "AND" || operand == "OR")
            {
                var m = b.Left as MemberExpression;
                if (m != null && m.Expression != null
                    && m.Expression.NodeType == ExpressionType.Parameter)
                    left = new PartialSqlString(string.Format("{0}={1}", VisitMemberAccess(m), GetQuotedTrueValue()));
                else
                    left = Visit(b.Left);

                m = b.Right as MemberExpression;
                if (m != null && m.Expression != null
                    && m.Expression.NodeType == ExpressionType.Parameter)
                    right = new PartialSqlString(string.Format("{0}={1}", VisitMemberAccess(m), GetQuotedTrueValue()));
                else
                    right = Visit(b.Right);

                if (left as PartialSqlString == null && right as PartialSqlString == null)
                {
                    var result = Expression.Lambda(b).Compile().DynamicInvoke();
                    return new PartialSqlString(OrmLiteConfig.DialectProvider.GetQuotedValue(result, result.GetType()));
                }

                if (left as PartialSqlString == null)
                    left = ((bool)left) ? GetTrueExpression() : GetFalseExpression();
                if (right as PartialSqlString == null)
                    right = ((bool)right) ? GetTrueExpression() : GetFalseExpression();
            }
            else
            {
                left = Visit(b.Left);
                right = Visit(b.Right);

                if (left as EnumMemberAccess != null && right as PartialSqlString == null)
                {
                    var enumType = ((EnumMemberAccess)left).EnumType;

                    //enum value was returned by Visit(b.Right)
                    long numvericVal;
                    if (Int64.TryParse(right.ToString(), out numvericVal))
                        right = OrmLiteConfig.DialectProvider.GetQuotedValue(Enum.ToObject(enumType, numvericVal).ToString(),
                                                                     typeof(string));
                    else
                        right = OrmLiteConfig.DialectProvider.GetQuotedValue(right, right.GetType());
                }
                else if (right as EnumMemberAccess != null && left as PartialSqlString == null)
                {
                    var enumType = ((EnumMemberAccess)right).EnumType;

                    //enum value was returned by Visit(b.Left)
                    long numvericVal;
                    if (Int64.TryParse(left.ToString(), out numvericVal))
                        left = OrmLiteConfig.DialectProvider.GetQuotedValue(Enum.ToObject(enumType, numvericVal).ToString(),
                                                                     typeof(string));
                    else
                        left = OrmLiteConfig.DialectProvider.GetQuotedValue(left, left.GetType());
                }
                else if (left as PartialSqlString == null && right as PartialSqlString == null)
                {
                    var result = Expression.Lambda(b).Compile().DynamicInvoke();
                    return result;
                }
                else if (left as PartialSqlString == null)
                    left = OrmLiteConfig.DialectProvider.GetQuotedValue(left, left != null ? left.GetType() : null);
                else if (right as PartialSqlString == null)
                    right = OrmLiteConfig.DialectProvider.GetQuotedValue(right, right != null ? right.GetType() : null);

            }

            if (operand == "=" && right.ToString().Equals("null", StringComparison.InvariantCultureIgnoreCase)) operand = "is";
            else if (operand == "<>" && right.ToString().Equals("null", StringComparison.InvariantCultureIgnoreCase)) operand = "is not";

            switch (operand)
            {
                case "MOD":
                case "COALESCE":
                    return new PartialSqlString(string.Format("{0}({1},{2})", operand, left, right));
                default:
                    return new PartialSqlString("(" + left + sep + operand + sep + right + ")");
            }
        }

        /// <summary>Visit member access.</summary>
        /// <param name="m">The MethodCallExpression to process.</param>
        /// <returns>An object.</returns>
        protected virtual object VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null
                && (m.Expression.NodeType == ExpressionType.Parameter || m.Expression.NodeType == ExpressionType.Convert))
            {
                var propertyInfo = m.Member as PropertyInfo;

                if (propertyInfo.PropertyType.IsEnum)
                    return new EnumMemberAccess((PrefixFieldWithTableName ? OrmLiteConfig.DialectProvider.GetQuotedTableName(modelDef.ModelName) + "." : "") + GetQuotedColumnName(m.Member.Name), propertyInfo.PropertyType);

                return new PartialSqlString((PrefixFieldWithTableName ? OrmLiteConfig.DialectProvider.GetQuotedTableName(modelDef.ModelName) + "." : "") + GetQuotedColumnName(m.Member.Name));
            }

            var member = Expression.Convert(m, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(member);
            var getter = lambda.Compile();
            return getter();
        }

        /// <summary>Visit member initialise.</summary>
        /// <param name="exp">The exponent.</param>
        /// <returns>An object.</returns>
        protected virtual object VisitMemberInit(MemberInitExpression exp)
        {
            return Expression.Lambda(exp).Compile().DynamicInvoke();
        }

        /// <summary>Visit new.</summary>
        /// <param name="nex">The nex.</param>
        /// <returns>An object.</returns>
        protected virtual object VisitNew(NewExpression nex)
        {
            // TODO : check !
            var member = Expression.Convert(nex, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(member);
            try
            {
                var getter = lambda.Compile();
                return getter();
            }
            catch (System.InvalidOperationException)
            { // FieldName ?
                List<Object> exprs = VisitExpressionList(nex.Arguments);
                StringBuilder r = new StringBuilder();
                foreach (Object e in exprs)
                {
                    r.AppendFormat("{0}{1}",
                                   r.Length > 0 ? "," : "",
                                   e);
                }
                return r.ToString();
            }

        }

        /// <summary>Visit parameter.</summary>
        /// <param name="p">The ParameterExpression to process.</param>
        /// <returns>An object.</returns>
        protected virtual object VisitParameter(ParameterExpression p)
        {
            return p.Name;
        }

        /// <summary>Gets or sets a value indicating whether this object is parameterized.</summary>
        /// <value>true if this object is parameterized, false if not.</value>
        public bool IsParameterized { get; set; }

        /// <summary>Options for controlling the operation.</summary>
        public Dictionary<string, object> Params = new Dictionary<string, object>();

        /// <summary>The parameter counter.</summary>
        int paramCounter = 0;

        /// <summary>Visit constant.</summary>
        /// <param name="c">The ConstantExpression to process.</param>
        /// <returns>An object.</returns>
        protected virtual object VisitConstant(ConstantExpression c)
        {
            if (c.Value == null)
                return new PartialSqlString("null");

            if (!IsParameterized)
            {
                return c.Value;
            }
            else
            {
                string paramPlaceholder = OrmLiteConfig.DialectProvider.ParamString + paramCounter++;
                Params.Add(paramPlaceholder, c.Value);
                return new PartialSqlString(paramPlaceholder);
            }
        }

        /// <summary>Visit unary.</summary>
        /// <param name="u">The UnaryExpression to process.</param>
        /// <returns>An object.</returns>
        protected virtual object VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    var o = Visit(u.Operand);

                    if (o as PartialSqlString == null)
                        return !((bool)o);

                    if (IsFieldName(o))
                        o = o + "=" + GetQuotedTrueValue();

                    return new PartialSqlString("NOT (" + o + ")");
                case ExpressionType.Convert:
                    if (u.Method != null)
                        return Expression.Lambda(u).Compile().DynamicInvoke();
                    break;
            }

            return Visit(u.Operand);

        }

        /// <summary>Query if 'm' is column access.</summary>
        /// <param name="m">The MethodCallExpression to process.</param>
        /// <returns>true if column access, false if not.</returns>
        private bool IsColumnAccess(MethodCallExpression m)
        {
            if (m.Object != null && m.Object as MethodCallExpression != null)
                return IsColumnAccess(m.Object as MethodCallExpression);

            var exp = m.Object as MemberExpression;
            return exp != null
                && exp.Expression != null
                && exp.Expression.Type == typeof(T)
                && exp.Expression.NodeType == ExpressionType.Parameter;
        }

        /// <summary>Visit method call.</summary>
        /// <param name="m">The MethodCallExpression to process.</param>
        /// <returns>An object.</returns>
        protected virtual object VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Sql))
                return VisitSqlMethodCall(m);

            if (IsArrayMethod(m))
                return VisitArrayMethodCall(m);

            if (IsColumnAccess(m))
                return VisitColumnAccessMethod(m);

            return Expression.Lambda(m).Compile().DynamicInvoke();
        }

        /// <summary>Visit expression list.</summary>
        /// <param name="original">The original.</param>
        /// <returns>A List&lt;Object&gt;</returns>
        protected virtual List<Object> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Object> list = new List<Object>();
            for (int i = 0, n = original.Count; i < n; i++)
            {
                if (original[i].NodeType == ExpressionType.NewArrayInit ||
                 original[i].NodeType == ExpressionType.NewArrayBounds)
                {

                    list.AddRange(VisitNewArrayFromExpressionList(original[i] as NewArrayExpression));
                }
                else
                    list.Add(Visit(original[i]));

            }
            return list;
        }

        /// <summary>Visit new array.</summary>
        /// <param name="na">The na.</param>
        /// <returns>An object.</returns>
        protected virtual object VisitNewArray(NewArrayExpression na)
        {

            List<Object> exprs = VisitExpressionList(na.Expressions);
            StringBuilder r = new StringBuilder();
            foreach (Object e in exprs)
            {
                r.Append(r.Length > 0 ? "," + e : e);
            }

            return r.ToString();
        }

        /// <summary>Visit new array from expression list.</summary>
        /// <param name="na">The na.</param>
        /// <returns>A List&lt;Object&gt;</returns>
        protected virtual List<Object> VisitNewArrayFromExpressionList(NewArrayExpression na)
        {

            List<Object> exprs = VisitExpressionList(na.Expressions);
            return exprs;
        }

        /// <summary>Bind operant.</summary>
        /// <param name="e">The ExpressionType to process.</param>
        /// <returns>A string.</returns>
        protected virtual string BindOperant(ExpressionType e)
        {

            switch (e)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.Multiply:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "MOD";
                case ExpressionType.Coalesce:
                    return "COALESCE";
                default:
                    return e.ToString();
            }
        }

        /// <summary>Gets quoted column name.</summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>The quoted column name.</returns>
        protected virtual string GetQuotedColumnName(string memberName)
        {

            if (useFieldName)
            {
                FieldDefinition fd = modelDef.FieldDefinitions.FirstOrDefault(x => x.Name == memberName);
                string fn = fd != default(FieldDefinition) ? fd.FieldName : memberName;
                return OrmLiteConfig.DialectProvider.GetQuotedColumnName(fn);
            }
            else
            {
                return memberName;
            }
        }

        /// <summary>Removes the quote from alias described by exp.</summary>
        /// <param name="exp">The exponent.</param>
        /// <returns>A string.</returns>
        protected string RemoveQuoteFromAlias(string exp)
        {

            if ((exp.StartsWith("\"") || exp.StartsWith("`") || exp.StartsWith("'"))
                &&
                (exp.EndsWith("\"") || exp.EndsWith("`") || exp.EndsWith("'")))
            {
                exp = exp.Remove(0, 1);
                exp = exp.Remove(exp.Length - 1, 1);
            }
            return exp;
        }

        /// <summary>Query if 'quotedExp' is field name.</summary>
        /// <param name="quotedExp">The quoted exponent.</param>
        /// <returns>true if field name, false if not.</returns>
        protected bool IsFieldName(object quotedExp)
        {
            FieldDefinition fd =
                modelDef.FieldDefinitions.
                    FirstOrDefault(x =>
                        OrmLiteConfig.DialectProvider.
                        GetQuotedColumnName(x.FieldName) == quotedExp.ToString());
            return (fd != default(FieldDefinition));
        }

        /// <summary>Gets true expression.</summary>
        /// <returns>The true expression.</returns>
        protected object GetTrueExpression()
        {
            return new PartialSqlString(string.Format("({0}={1})", GetQuotedTrueValue(), GetQuotedTrueValue()));
        }

        /// <summary>Gets false expression.</summary>
        /// <returns>The false expression.</returns>
        protected object GetFalseExpression()
        {
            return new PartialSqlString(string.Format("({0}={1})", GetQuotedTrueValue(), GetQuotedFalseValue()));
        }

        /// <summary>Gets quoted true value.</summary>
        /// <returns>The quoted true value.</returns>
        protected static object GetQuotedTrueValue()
        {
            return new PartialSqlString(OrmLiteConfig.DialectProvider.GetQuotedValue(true, typeof(bool)));
        }

        /// <summary>Gets quoted false value.</summary>
        /// <returns>The quoted false value.</returns>
        protected static object GetQuotedFalseValue()
        {
            return new PartialSqlString(OrmLiteConfig.DialectProvider.GetQuotedValue(false, typeof(bool)));
        }

        /// <summary>Builds select expression.</summary>
        /// <param name="fields">  x=> x.SomeProperty1 or x=> new{ x.SomeProperty1, x.SomeProperty2}</param>
        /// <param name="distinct">true to distinct.</param>
        private void BuildSelectExpression(string fields, bool distinct)
        {

            selectExpression = string.Format("SELECT {0}{1} \nFROM {2}",
                (distinct ? "DISTINCT " : ""),
                (string.IsNullOrEmpty(fields) ?
                    OrmLiteConfig.DialectProvider.GetColumnNames(modelDef) :
                    fields),
                OrmLiteConfig.DialectProvider.GetQuotedTableName(modelDef));
        }

        /// <summary>Gets all fields.</summary>
        /// <returns>all fields.</returns>
        public IList<string> GetAllFields()
        {
            return modelDef.FieldDefinitions.ConvertAll(r => r.Name);
        }

        /// <summary>Applies the paging described by SQL.</summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>A string.</returns>
        protected virtual string ApplyPaging(string sql)
        {
            sql = sql + (string.IsNullOrEmpty(LimitExpression) ? "" : "\n" + LimitExpression);
            return sql;
        }

        /// <summary>Query if 'm' is array method.</summary>
        /// <param name="m">The MethodCallExpression to process.</param>
        /// <returns>true if array method, false if not.</returns>
        private bool IsArrayMethod(MethodCallExpression m)
        {
            if (m.Object == null && m.Method.Name == "Contains")
            {
                if (m.Arguments.Count == 2)
                    return true;
            }

            return false;
        }

        /// <summary>Visit array method call.</summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="m">The MethodCallExpression to process.</param>
        /// <returns>An object.</returns>
        protected virtual object VisitArrayMethodCall(MethodCallExpression m)
        {
            string statement;

            switch (m.Method.Name)
            {
                case "Contains":
                    List<Object> args = this.VisitExpressionList(m.Arguments);
                    object quotedColName = args[1];

                    var memberExpr = m.Arguments[0];
                    if (memberExpr.NodeType == ExpressionType.MemberAccess)
                        memberExpr = (m.Arguments[0] as MemberExpression);

                    var member = Expression.Convert(memberExpr, typeof(object));
                    var lambda = Expression.Lambda<Func<object>>(member);
                    var getter = lambda.Compile();

                    var inArgs = Sql.Flatten(getter() as IEnumerable);

                    StringBuilder sIn = new StringBuilder();

                    if (inArgs.Any())
                    {
                        foreach (Object e in inArgs)
                        {
                            sIn.AppendFormat("{0}{1}",
                                             sIn.Length > 0 ? "," : "",
                                             OrmLiteConfig.DialectProvider.GetQuotedValue(e, e.GetType()));
                        }
                    }
                    else
                    {
                        // The collection is empty, so avoid generating invalid SQL syntax of "ColumnName IN ()".
                        // Instead, just select from the null set via "ColumnName IN (NULL)"
                        sIn.Append("NULL");
                    }

                    statement = string.Format("{0} {1} ({2})", quotedColName, "In", sIn.ToString());
                    break;

                default:
                    throw new NotSupportedException();
            }

            return new PartialSqlString(statement);
        }

        /// <summary>Visit SQL method call.</summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="m">The MethodCallExpression to process.</param>
        /// <returns>An object.</returns>
        protected virtual object VisitSqlMethodCall(MethodCallExpression m)
        {
            List<Object> args = this.VisitExpressionList(m.Arguments);
            object quotedColName = args[0];
            args.RemoveAt(0);

            string statement;

            switch (m.Method.Name)
            {
                case "In":

                    var member = Expression.Convert(m.Arguments[1], typeof(object));
                    var lambda = Expression.Lambda<Func<object>>(member);
                    var getter = lambda.Compile();

                    var inArgs = Sql.Flatten(getter() as IEnumerable);

                    StringBuilder sIn = new StringBuilder();
                    foreach (Object e in inArgs)
                    {
                        if (!typeof(ICollection).IsAssignableFrom(e.GetType()))
                        {
                            sIn.AppendFormat("{0}{1}",
                                         sIn.Length > 0 ? "," : "",
                                         OrmLiteConfig.DialectProvider.GetQuotedValue(e, e.GetType()));
                        }
                        else
                        {
                            var listArgs = e as ICollection;
                            foreach (Object el in listArgs)
                            {
                                sIn.AppendFormat("{0}{1}",
                                         sIn.Length > 0 ? "," : "",
                                         OrmLiteConfig.DialectProvider.GetQuotedValue(el, el.GetType()));
                            }
                        }
                    }

                    statement = string.Format("{0} {1} ({2})", quotedColName, m.Method.Name, sIn.ToString());
                    break;
                case "Desc":
                    statement = string.Format("{0} DESC", quotedColName);
                    break;
                case "As":
                    statement = string.Format("{0} As {1}", quotedColName,
                        OrmLiteConfig.DialectProvider.GetQuotedColumnName(RemoveQuoteFromAlias(args[0].ToString())));
                    break;
                case "Sum":
                case "Count":
                case "Min":
                case "Max":
                case "Avg":
                    statement = string.Format("{0}({1}{2})",
                                         m.Method.Name,
                                         quotedColName,
                                         args.Count == 1 ? string.Format(",{0}", args[0]) : "");
                    break;
                default:
                    throw new NotSupportedException();
            }

            return new PartialSqlString(statement);
        }

        /// <summary>Visit column access method.</summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="m">The MethodCallExpression to process.</param>
        /// <returns>An object.</returns>
        protected virtual object VisitColumnAccessMethod(MethodCallExpression m)
        {
            List<Object> args = this.VisitExpressionList(m.Arguments);
            var quotedColName = Visit(m.Object);
            var statement = "";

            switch (m.Method.Name)
            {
                case "Trim":
                    statement = string.Format("ltrim(rtrim({0}))", quotedColName);
                    break;
                case "LTrim":
                    statement = string.Format("ltrim({0})", quotedColName);
                    break;
                case "RTrim":
                    statement = string.Format("rtrim({0})", quotedColName);
                    break;
                case "ToUpper":
                    statement = string.Format("upper({0})", quotedColName);
                    break;
                case "ToLower":
                    statement = string.Format("lower({0})", quotedColName);
                    break;
                case "StartsWith":
                    statement = string.Format("upper({0}) like {1} ", quotedColName, OrmLiteConfig.DialectProvider.GetQuotedParam(args[0].ToString().ToUpper() + "%"));
                    break;
                case "EndsWith":
                    statement = string.Format("upper({0}) like {1}", quotedColName, OrmLiteConfig.DialectProvider.GetQuotedParam("%" + args[0].ToString().ToUpper()));
                    break;
                case "Contains":
                    statement = string.Format("upper({0}) like {1}", quotedColName, OrmLiteConfig.DialectProvider.GetQuotedParam("%" + args[0].ToString().ToUpper() + "%"));
                    break;
                case "Substring":
                    var startIndex = Int32.Parse(args[0].ToString()) + 1;
                    if (args.Count == 2)
                    {
                        var length = Int32.Parse(args[1].ToString());
                        statement = string.Format("substring({0} from {1} for {2})",
                                                  quotedColName,
                                                  startIndex,
                                                  length);
                    }
                    else
                        statement = string.Format("substring({0} from {1})",
                                         quotedColName,
                                         startIndex);
                    break;
                default:
                    throw new NotSupportedException();
            }
            return new PartialSqlString(statement);
        }

        public string ToInsertWhereNotExistsStatement(object obj)
        {
            var sql = new StringBuilder();
            var fieldDefs = modelDef.FieldDefinitions.Where(t => t.AutoIncrement == false && t.IsComputed == false).ToList();
            sql.AppendFormat("Insert Into {0} ({1})", OrmLiteConfig.DialectProvider.GetQuotedTableName(modelDef), string.Join(",", fieldDefs.Select(t => OrmLiteConfig.DialectProvider.GetQuotedColumnName(t.FieldName)).ToArray()));
            sql.Append("\n");
            sql.AppendFormat("Select {0}", string.Join(",", fieldDefs.Select(t => t.GetQuotedValue(obj)).ToArray()));
            sql.Append("\n");
            BuildSelectExpression("1", false);
            sql.AppendFormat(" where not exists({0} {1})", selectExpression, WhereExpression);
            return sql.ToString();
        }
    }

    /// <summary>A partial SQL string.</summary>
    public class PartialSqlString
    {
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.PartialSqlString class.
        /// </summary>
        /// <param name="text">The text.</param>
        public PartialSqlString(string text)
        {
            Text = text;
        }

        /// <summary>Gets or sets the text.</summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return Text;
        }
    }

    /// <summary>An enum member access.</summary>
    public class EnumMemberAccess : PartialSqlString
    {
        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.EnumMemberAccess class.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        /// illegal values.</exception>
        /// <param name="text">    The text.</param>
        /// <param name="enumType">The type of the enum.</param>
        public EnumMemberAccess(string text, Type enumType)
            : base(text)
        {
            if (!enumType.IsEnum) throw new ArgumentException("Type not valid", "enumType");

            EnumType = enumType;
        }

        /// <summary>Gets the type of the enum.</summary>
        /// <value>The type of the enum.</value>
        public Type EnumType { get; private set; }
    }

}

