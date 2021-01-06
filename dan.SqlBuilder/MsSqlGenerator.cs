using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dan.SqlBuilder
{
    public class MsSqlGenerator : ISqlGenerator
    {
        StringBuilder sql = new StringBuilder();

        public string DefaultTableSchema { get; set; } = "dbo";

        public static string GenerateSql(Query query)
		{
            ISqlGenerator generator = new MsSqlGenerator();
            query.Accept(generator);
            return generator.GenerateSql();
        }

        public static string TranslateComparisonOperator(ComparisonOperator comparisonOperator)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.Between:
                    return "BETWEEN";
                case ComparisonOperator.Equal:
                    return "=";
                case ComparisonOperator.Exists:
                    return "EXISTS";
                case ComparisonOperator.GreaterThan:
                    return ">";
                case ComparisonOperator.GreaterThanOrEqualTo:
                    return ">=";
                case ComparisonOperator.In:
                    return "IN";
                case ComparisonOperator.IsNotNull:
                    return "IS NOT NULL";
                case ComparisonOperator.IsNull:
                    return "IS NULL";
                case ComparisonOperator.LessThan:
                    return "<";
                case ComparisonOperator.LessThanOrEqualTo:
                    return "<=";
                case ComparisonOperator.Like:
                    return "LIKE";
                case ComparisonOperator.NotBetween:
                    return "NOT BETWEEN";
                case ComparisonOperator.NotEqual:
                    return "<>";
                case ComparisonOperator.NotExists:
                    return "NOT EXISTS";
                case ComparisonOperator.NotIn:
                    return "NOT IN";
                case ComparisonOperator.NotLike:
                    return "NOT LIKE";
                default:
                    throw new NotImplementedException($"Operator {Enum.GetName(typeof(ComparisonOperator), comparisonOperator)} is not supported yet");
            }
        }

        public static string TranslateJoinType(TableJoinType joinType)
        {
            switch (joinType)
            {
                case TableJoinType.LeftJoin:
                    return "LEFT JOIN";
                case TableJoinType.InnerJoin:
                    return "INNER JOIN";
                case TableJoinType.RightJoin:
                    return "RIGHT JOIN";
                case TableJoinType.FullJoin:
                    return "FULL JOIN";
                default:
                    throw new NotImplementedException($"Unsupported join type {Enum.GetName(typeof(TableJoinType), joinType)}");
            }
        }

        public static string GetAggregateFunctionName(AggregateFunction aggregateFunction)
        {
            switch (aggregateFunction)
            {
                case AggregateFunction.Count:
                    return "COUNT";
                case AggregateFunction.CountBig:
                    return "COUNT_BIG";
                case AggregateFunction.Average:
                    return "AVG";
                case AggregateFunction.Max:
                    return "MAX";
                case AggregateFunction.Min:
                    return "MIN";
                case AggregateFunction.StandardDeviation:
                    return "STD";
                case AggregateFunction.Sum:
                    return "SUM";
                case AggregateFunction.Variance:
                    return "VAR";
                default:
                    throw new NotImplementedException($"Unsupported aggregate function {Enum.GetName(typeof(AggregateFunction), aggregateFunction)}");
            }
        }

        public string GenerateSql()
        {
            return sql.ToString();
        }

        public void Visit(AggregateField component)
        {
            sql.Append($"{GetAggregateFunctionName(component.Function)}({(component.IsDistinct ? "DISTINCT " : "")}");
            component.Field.Accept(this);
            sql.Append(")");
            if (component.PartitionBy.Count + component.OrderBy.Count > 0)
            {
                sql.Append(" OVER (");
                if (component.PartitionBy.Count > 0)
                {
                    sql.Append("PARTITION BY ");
                    component.PartitionBy.First().Accept(this);
                    foreach (var item in component.PartitionBy.Skip(1))
                    {
                        sql.Append(", ");
                        item.Accept(this);
                    }
                }
                if (component.OrderBy.Count > 0)
                {
                    sql.Append((component.PartitionBy.Count > 0 ? " " : "") + "ORDER BY ");
                    component.OrderBy.First().Accept(this);
                    foreach (var item in component.OrderBy.Skip(1))
                    {
                        sql.Append(", ");
                        item.Accept(this);
                    }
                }
                sql.Append(")");
            }
        }

        public void Visit(ArrayField component)
        {
            if (component.Data.Count == 0)
                throw new ArgumentException("Empty in array field");

            string startEncloser = "";
            string endEncloser = "";
            if (component.DataType?.IsNumericType() == true)
			{

			}
            else if (component.DataType == typeof(Guid))
			{
                startEncloser = endEncloser = "'";
			}
            else
			{
                startEncloser = "N'";
                endEncloser = "'";
			}
            sql.Append("(" + startEncloser + string.Join(endEncloser + ", " + startEncloser, component.Data) + endEncloser + ")");
        }

        public void Visit(BetweenField component)
        {
            component.From.Accept(this);
            sql.Append(" AND ");
            component.To.Accept(this);
        }

        public void Visit(Condition component)
        {
            switch (component.Operator)
            {
                case ComparisonOperator.Exists:
                case ComparisonOperator.NotExists:

                    if (!(component.Field1 is Query))
                        throw new InvalidOperationException("Field1 must be a query for EXISTS operator");

                    sql.Append(component.Operator == ComparisonOperator.NotExists ? "NOT EXISTS (" : "EXISTS (");
                    component.Field1.Accept(this);
                    sql.Append(")");
                    break;
                case ComparisonOperator.IsNull:
                case ComparisonOperator.IsNotNull:
                    component.Field1.Accept(this);
                    sql.Append($" {TranslateComparisonOperator(component.Operator)}");
                    break;
                default:
                    component.Field1.Accept(this);
                    sql.Append($" {TranslateComparisonOperator(component.Operator)} ");
                    component.Field2.Accept(this);
                    break;
            }
        }

        public void Visit(ConditionClause component)
        {
            sql.Append("(");
            component.Condition.Accept(this);

            foreach (var condition in component.OtherConditions)
            {
                condition.Accept(this);
            }
            sql.Append(")");
        }

        public void Visit(MoreCondition component)
        {
            sql.Append(component.LogicalOperator == LogicalOperator.And ? " AND " : " OR ");
            component.Condition.Accept(this);
        }

        public void Visit(CaseField component)
        {
            sql.Append("(CASE ");
            component.Field.Accept(this);
            foreach (var fieldCase in component.When)
            {
                sql.Append(" WHEN ");
                fieldCase.Key.Accept(this);
                sql.Append(" THEN ");
                fieldCase.Value.Accept(this);
            }

            if (component.Else != null)
            {
                sql.Append(" ELSE ");
                component.Else.Accept(this);
            }
            sql.Append(" END)");
        }

        public void Visit(CaseWhenField component)
        {
            sql.Append("(CASE");
            foreach (var conditionValue in component.Conditions)
            {
                sql.Append(" WHEN ");
                conditionValue.Key.Accept(this);
                sql.Append(" THEN ");
                conditionValue.Value.Accept(this);
            }

            if (component.Else != null)
            {
                sql.Append(" ELSE ");
                component.Else.Accept(this);
            }
            sql.Append(" END)");
        }

        public void Visit(RowNumberField component)
        {
            sql.Append("ROW_NUMBER()");
            if (component.PartitionBy.Count + component.OrderBy.Count > 0)
            {
                sql.Append(" OVER (");
                if (component.PartitionBy.Count > 0)
                {
                    sql.Append("PARTITION BY ");
                    component.PartitionBy.First().Accept(this);
                    foreach (var item in component.PartitionBy.Skip(1))
                    {
                        sql.Append(", ");
                        item.Accept(this);
                    }
                }
                if (component.OrderBy.Count > 0)
                {
                    sql.Append((component.PartitionBy.Count > 0 ? " " : "") + "ORDER BY ");
                    component.OrderBy.First().Accept(this);
                    foreach (var item in component.OrderBy.Skip(1))
                    {
                        sql.Append(", ");
                        item.Accept(this);
                    }
                }
                sql.Append(")");
            }
        }

        public void Visit(SelectField component)
        {
            component.Field.Accept(this);
            if (!string.IsNullOrEmpty(component.Alias))
                sql.Append($" AS [{component.Alias}]");
        }

        public void Visit(SortField component)
        {
            component.Field.Accept(this);
            sql.Append(component.Descending ? " DESC" : " ASC");
        }

        public void Visit(Table component)
        {
            string schema = component.Schema ?? DefaultTableSchema;

            if (!string.IsNullOrEmpty(schema))
                sql.Append($"[{schema}].[{component.Name}]");
            else
                sql.Append($"[{component.Name}]");

            if (!string.IsNullOrEmpty(component.Alias))
                sql.Append(" AS [" + component.Alias + "]");
        }

        public void Visit(TableJoin component)
        {
            sql.Append(" CROSS JOIN ");
            component.ForeignTable.Accept(this);
        }

        public void Visit(TableApply component)
        {
            sql.Append($" {(component.JoinType == TableApplyType.CrossApply ? "CROSS APPLY" : "OUTER APPLY")} (");
            component.ForeignTable.Accept(this);
            sql.Append($") AS [{((ISqlTable)component.ForeignTable).Alias}]");
        }

        public void Visit(TableRelation component)
        {
            sql.Append(" " + TranslateJoinType(component.JoinType) + " ");
            component.ForeignTable.Accept(this);
            sql.Append(" ON ");
            component.LocalForeignKeys.First().Accept(this);
            foreach (var key in component.LocalForeignKeys.Skip(1))
            {
                sql.Append(" AND ");
                key.Accept(this);
            }

        }

        public void Visit(Variable component)
        {
            sql.Append("@" + component.Name);
        }

        public void Visit(SqlExpression component)
        {
            sql.Append(component.Expression);
        }

        public void Visit(DbField component)
        {
            if (!string.IsNullOrEmpty(component.TableName))
                sql.Append($"[{component.TableName}].[{component.FieldName}]");
            else
                sql.Append($"[{component.FieldName}]");
        }

        public void Visit(StringField component)
        {
            sql.Append("N'" + component.Value + "'");
        }

        public void Visit(IsNullField component)
        {
            sql.Append("ISNULL(");
            component.Field.Accept(this);
            sql.Append(", ");
            component.WhenNull.Accept(this);
            sql.Append(")");
        }

        public void Visit(CommonTableExpression component)
        {
            throw new NotImplementedException();
        }

        public void Visit(LocalForeignKey component)
        {
            component.LocalKey.Accept(this);
            sql.Append(" = ");
            component.ForeignKey.Accept(this);
        }

        public void Visit(Query component)
        {

            sql.Append("SELECT ");

            if ((component.Offset ?? 0) == 0 && component.PageSize.HasValue)
                sql.Append($"TOP {component.PageSize} ");

            if (component.SelectDistinct)
                sql.Append("DISTINCT ");

            if (component.Select.Count == 0)
            {
                sql.Append("*");
            }
            else
            {
                for (int i = 0; i < component.Select.Count; i++)
                {
                    if (i > 0) sql.Append(", ");
                    component.Select[i].Accept(this);
                }
            }

            if (component.From != null)
            {
                sql.Append(" FROM ");

                component.From.Accept(this);

                for (int i = 0; i < component.JoinTables.Count; i++)
                {
                    component.JoinTables[i].Accept(this);
                }
            }

            if (component.Where != null)
            {
                sql.Append(" WHERE ");

                component.Where.Accept(this);
            }

            if (component.GroupBy.Count > 0)
            {
                sql.Append(" GROUP BY ");

                for (int i = 0; i < component.GroupBy.Count; i++)
                {
                    if (i > 0) sql.Append(", ");
                    component.GroupBy[i].Accept(this);
                }
            }

            if (component.Having != null)
            {
                sql.Append(" HAVING ");

                component.Having.Accept(this);
            }

            if (component.OrderBy.Count > 0)
            {
                sql.Append(" ORDER BY ");

                for (int i = 0; i < component.OrderBy.Count; i++)
                {
                    if (i > 0) sql.Append(", ");
                    component.OrderBy[i].Accept(this);
                }
            }

            if ((component.Offset ?? 0) > 0)
            {
                sql.Append($" OFFSET {component.Offset} ROWS");

                if (component.PageSize.HasValue)
                    sql.Append($" FETCH NEXT {component.PageSize} ROWS ONLY");
            }
        }

        public void Visit(CompositeQuery component)
        {
            foreach (var expr in component.CommonTableExpression)
            {
                expr.Accept(this);
            }
            component.Query.Accept(this);

            foreach (var uQuery in component.UnionQueries)
            {
                uQuery.Accept(this);
            }
        }

        public void Visit(UnionQuery component)
        {
            sql.Append(Environment.NewLine + (component.UnionAll ? "UNION ALL (" : "UNION ("));
            component.Query.Accept(this);
            sql.Append(")");
        }

        public void Visit(NumberField component)
        {
            sql.Append(component.Value);
        }

        public void Visit(IntField component)
        {
            sql.Append(component.Value);
        }

        public void Visit(DateField component)
        {
            sql.Append("'" + component.Value.ToString(component.HasTime ? "yyyy-MM-dd HH:mm:ss" : "yyyy-MM-dd") + "'");
        }
    }
}
