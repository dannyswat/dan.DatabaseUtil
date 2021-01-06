using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dan.SqlBuilder
{
	public class QueryBuilder
	{
		Query query = new Query();

		public QueryBuilder Select(params SelectField[] fields)
		{
			if (fields != null && fields.Length > 0)
				query.Select.AddRange(fields);
			return this;
		}

		public QueryBuilder SelectDistinct(params SelectField[] fields)
		{
			query.SelectDistinct = true;
			return Select(fields);
		}

		public QueryBuilder From(ISqlTable table)
		{
			query.From = table;
			return this;
		}

		public QueryBuilder InnerJoin(ISqlTable table, ISqlTable foreignTable, ISqlField localKey, ISqlField foreignKey)
		{
			var relation = new TableRelation
			{
				Table = table,
				ForeignTable = foreignTable,
				JoinType = TableJoinType.InnerJoin
			};

			relation.LocalForeignKeys.Add(new LocalForeignKey(localKey, foreignKey));

			query.JoinTables.Add(relation);

			return this;
		}

		public QueryBuilder InnerJoin(ISqlTable table, ISqlTable foreignTable, params LocalForeignKey[] foreignKeys)
		{
			var relation = new TableRelation
			{
				Table = table,
				ForeignTable = foreignTable,
				JoinType = TableJoinType.InnerJoin,
				LocalForeignKeys = foreignKeys.ToList()
			};

			query.JoinTables.Add(relation);

			return this;
		}

		public QueryBuilder LeftJoin(ISqlTable table, ISqlTable foreignTable, params LocalForeignKey[] foreignKeys)
		{
			var relation = new TableRelation
			{
				Table = table,
				ForeignTable = foreignTable,
				JoinType = TableJoinType.LeftJoin,
				LocalForeignKeys = foreignKeys.ToList()
			};

			query.JoinTables.Add(relation);

			return this;
		}

		public QueryBuilder RightJoin(ISqlTable table, ISqlTable foreignTable, params LocalForeignKey[] foreignKeys)
		{
			var relation = new TableRelation
			{
				Table = table,
				ForeignTable = foreignTable,
				JoinType = TableJoinType.RightJoin,
				LocalForeignKeys = foreignKeys.ToList()
			};

			query.JoinTables.Add(relation);

			return this;
		}

		public QueryBuilder Where(ISqlField field1, ComparisonOperator @operator, ISqlField field2 = null)
		{
			if (query.Where == null)
			{
				query.Where = new ConditionClause(field1, @operator, field2);
			}
			else
			{
				query.Where.OtherConditions.Add(new MoreCondition(LogicalOperator.And, field1, @operator, field2));
			}
			return this;
		}

		public QueryBuilder Where(ConditionClause conditionClause)
		{
			if (query.Where == null)
			{
				query.Where = conditionClause;
			}
			else
			{
				query.Where.OtherConditions.Add(new MoreCondition(LogicalOperator.And, conditionClause));
			}
			return this;
		}

		public QueryBuilder AndWhere(ISqlField field1, ComparisonOperator @operator, ISqlField field2 = null)
		{
			query.Where.OtherConditions.Add(new MoreCondition(LogicalOperator.And, field1, @operator, field2));
			return this;
		}

		public QueryBuilder OrWhere(ISqlField field1, ComparisonOperator @operator, ISqlField field2 = null)
		{
			query.Where.OtherConditions.Add(new MoreCondition(LogicalOperator.Or, field1, @operator, field2));
			return this;
		}

		public QueryBuilder Skip(int rowCount)
		{
			query.Offset = rowCount;
			return this;
		}

		public QueryBuilder Take(int rowCount)
		{
			query.PageSize = rowCount;
			return this;
		}


		public Query Build()
		{
			return query;
		}
	}
}
